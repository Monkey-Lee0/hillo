using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Rooms;
using hillo.Scripts.Cards;

namespace hillo.Scripts.Epochs
{
    // ============================================================
    // 1. 自定义纪元类
    // ============================================================
    public class RegentAscension2Epoch : EpochModel
    {
        public const string EpochId = "regent_ascension_2";

        public override string Id => EpochId;
        public override EpochEra Era => EpochEra.Invitation1;
        public override int EraPosition => 1;
        public override string? StoryId => "Regent";

        private static List<CardModel> Cards => new List<CardModel>
        {
            ModelDb.Card<StarConversion>(),
        };

        public override string UnlockText => CreateCardUnlockText(Cards);
        public override void QueueUnlocks() => NTimelineScreen.Instance.QueueCardUnlock(Cards);
    }

    // ============================================================
    // 2. 注册器（纯反射，无 Harmony）
    // ============================================================
    public static class RegentEpochRegistration
    {
        private static bool _registered = false;

        public static void Register()
        {
            if (_registered) return;

            // ① 扩展 EpochModel 的私有字典
            ExtendEpochModelDictionaries();

            // ② 修改 _allEpochIds 缓存，使时间线包含自定义纪元
            ExtendAllEpochIds();

            // ③ 订阅战斗胜利事件
            CombatManager.Instance.CombatWon += OnCombatWon;

            _registered = true;
        }

        private static void ExtendEpochModelDictionaries()
        {
            var epochTypeDictField = typeof(EpochModel).GetField("_epochTypeDictionary", BindingFlags.Static | BindingFlags.NonPublic);
            var typeToIdDictField = typeof(EpochModel).GetField("_typeToIdDictionary", BindingFlags.Static | BindingFlags.NonPublic);

            if (epochTypeDictField == null || typeToIdDictField == null) return;

            var epochTypeDict = (Dictionary<string, Type>)epochTypeDictField.GetValue(null);
            var typeToIdDict = (Dictionary<Type, string>)typeToIdDictField.GetValue(null);

            if (epochTypeDict == null || typeToIdDict == null) return;

            var customType = typeof(RegentAscension2Epoch);
            string epochId = RegentAscension2Epoch.EpochId;

            if (!epochTypeDict.ContainsKey(epochId))
            {
                epochTypeDict[epochId] = customType;
                typeToIdDict[customType] = epochId;
            }
        }

        private static void ExtendAllEpochIds()
        {
            var field = typeof(EpochModel).GetField("_allEpochIds", BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null) return;

            // 如果缓存尚未初始化，则先访问 AllEpochIds 触发初始化
            var currentList = field.GetValue(null) as IReadOnlyList<string>;
            if (currentList == null)
            {
                // 强制初始化
                var dummy = EpochModel.AllEpochIds;
                currentList = field.GetValue(null) as IReadOnlyList<string>;
            }

            if (currentList == null) return;

            // 创建新列表，添加自定义 ID
            var newList = currentList.ToList();
            string epochId = RegentAscension2Epoch.EpochId;
            if (!newList.Contains(epochId))
            {
                newList.Add(epochId);
                // 创建只读列表并赋值
                var readOnlyList = newList.AsReadOnly();
                field.SetValue(null, readOnlyList);
            }
        }

        private static void OnCombatWon(CombatRoom room)
        {
            var progress = SaveManager.Instance?.Progress;
            if (progress == null) return;

            string epochId = RegentAscension2Epoch.EpochId;

            if (progress.Epochs.Any(e => e.Id == epochId && e.State == EpochState.Revealed))
                return;

            var regentId = ModelDb.Character<Regent>().Id;

            if (progress.CharacterStats.TryGetValue(regentId, out var stats))
            {
                if (stats.MaxAscension >= 2)
                {
                    progress.ObtainEpochOverride(epochId, EpochState.Revealed);
                    SaveManager.Instance.SaveProgressFile();
                }
            }
        }
    }
}
