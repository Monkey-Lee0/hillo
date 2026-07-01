using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Models.Cards;
using BaseLib.Abstracts;
using BaseLib;
using hillo.Scripts.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace hillo.Scripts.Patches
{
    /// <summary>
    /// 覆盖 DustyTome 遗物效果：
    /// 20% 获得 GrandEncore，80% 从当前角色卡池的非升级 Ancient 卡中随机抽取。
    /// 排除 GrandEncore 和所有 TranscendenceUpgrades 中的卡牌（包含 Revere）。
    /// </summary>
    [HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
    [HarmonyPriority(Priority.First)] // 确保优先于 BaseLib 补丁执行
    public static class DustyTomePatch
    {
        private static ModelId? _grandEncoreId;
        private static HashSet<ModelId>? _upgradeCardIds;
        private static bool _initialized;

        private static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            // 获取 GrandEncore 的 ID
            try
            {
                _grandEncoreId = ModelDb.Card<GrandEncore>().Id;
            }
            catch
            {
                BaseLibMain.Logger.Error("DustyTomePatch: GrandEncore card not found in ModelDb!");
                _grandEncoreId = ModelId.none;
            }

            // 通过反射获取升级卡 ID 集合
            var upgradeDict = GetTranscendenceUpgrades();
            _upgradeCardIds = upgradeDict?.Values.Select(c => c.Id).ToHashSet() ?? new HashSet<ModelId>();
            BaseLibMain.Logger.Info($"DustyTomePatch: Found {_upgradeCardIds.Count} transcendence upgrade cards.");
        }

        private static Dictionary<ModelId, CardModel>? GetTranscendenceUpgrades()
        {
            var prop = typeof(ArchaicTooth).GetProperty("TranscendenceUpgrades",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (prop == null)
            {
                BaseLibMain.Logger.Warn("DustyTomePatch: Cannot find TranscendenceUpgrades property.");
                return null;
            }
            return prop.GetValue(null) as Dictionary<ModelId, CardModel>;
        }

        [HarmonyPrefix]
        static bool Prefix(DustyTome __instance, Player player)
        {
            Initialize();

            // 如果 GrandEncore 不存在，回退到原逻辑（由 BaseLib 补丁处理）
            if (_grandEncoreId == null || _grandEncoreId == ModelId.none)
            {
                return true;
            }

            var rng = player.PlayerRng.Rewards;
            ModelId selectedCardId;

            // 20% 概率 GrandEncore
            if (rng.NextInt(100) < 20)
            {
                selectedCardId = _grandEncoreId;
            }
            else
            {
                // 80% 概率从当前角色卡池中筛选 eligible 卡
                var cardPool = player.Character.CardPool;
                var eligible = new List<ModelId>();

                foreach (var card in ModelDb.AllCards)
                {
                    // 只考虑 Ancient 稀有度
                    if (card.Rarity != CardRarity.Ancient) continue;
                    var id = card.Id;

                    // 排除 GrandEncore 和升级卡
                    if (id == _grandEncoreId || _upgradeCardIds.Contains(id)) continue;

                    // 检查该卡是否属于当前角色卡池
                    if (cardPool.AllCards.Any(c => c.Id == id))
                    {
                        eligible.Add(id);
                    }
                }

                if (eligible.Count == 0)
                {
                    selectedCardId = _grandEncoreId;
                    BaseLibMain.Logger.Warn($"DustyTomePatch: No eligible cards for {player.Character.Id.Entry}; falling back to GrandEncore.");
                }
                else
                {
                    selectedCardId = rng.NextItem(eligible);
                }
            }

            __instance.AncientCard = selectedCardId;
            return false;
        }
    }
}