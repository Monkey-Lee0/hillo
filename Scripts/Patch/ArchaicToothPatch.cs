using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Players;
using hillo.Scripts.Cards;
using MegaCrit.Sts2.Core.Random;

namespace hillo.Scripts.Patch;

[HarmonyPatch(typeof(ArchaicTooth), "get_TranscendenceUpgrades")]
public static class AddVenerateTranscendencePatch
{
   private static readonly (Type source, Type target)[] _transforms =
    [
        (typeof(Venerate), typeof(Revere)),
        (typeof(Survivor), typeof(Explorer)),
        (typeof(Zap), typeof(ChainLightning)),
        // 可以在此添加更多映射，例如：
        // (typeof(SomeCard), typeof(SomeOtherCard)),
    ];

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)] // 确保在 BaseLib 自身补丁之后执行
    static void AddUpgrades(Dictionary<ModelId, CardModel> __result)
    {
        foreach (var (sourceType, targetType) in _transforms)
        {
            try
            {
                // 获取源卡 ID
                var sourceId = ModelDb.GetId(sourceType);
                // 检查是否已存在映射
                if (!__result.ContainsKey(sourceId))
                {
                    // 获取目标卡实例
                    var targetCard = ModelDb.GetByIdOrNull<CardModel>(ModelDb.GetId(targetType));
                    if (targetCard != null)
                    {
                        __result[sourceId] = targetCard;
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误日志（可选）
                // BaseLibMain.Logger.Error($"添加 Transcendence 升级映射失败: {ex.Message}");
            }
        }
    }
}

[HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceStarterCard")]
public static class ArchaicTooth_GetTranscendenceStarterCard_Patch
{
    static bool Prefix(ArchaicTooth __instance, Player player, ref CardModel __result)
    {
        var prop = typeof(ArchaicTooth).GetProperty("TranscendenceUpgrades",
            BindingFlags.Static | BindingFlags.NonPublic);
        if (prop == null)
        {
            return true;
        }
        var transcendenceUpgrades = prop.GetValue(null) as Dictionary<ModelId, CardModel>;
        if (transcendenceUpgrades == null || transcendenceUpgrades.Count == 0)
        {
            __result = null;
            return false;
        }

        var eligibleCards = player.Deck.Cards
            .Where(c => transcendenceUpgrades.ContainsKey(c.Id))
            .ToList();

        if (eligibleCards.Count == 0)
        {
            __result = null;
        }
        else
        {
            var originalRng = player.RunState.Rng.CombatCardGeneration;
            Rng previewRng = new Rng(originalRng.Seed, originalRng.Counter);
            int index = previewRng.NextInt() % eligibleCards.Count();

            __result = eligibleCards[index];
        }

        return false;
    }
}