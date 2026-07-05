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
        (typeof(Bodyguard), typeof(VoidEnvoy)),
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

// 原版 GetTranscendenceStarterCard 用 FirstOrDefault 取牌库里第一张可转世的牌，
// 初始牌序固定 => 只要不删牌就永远是同一张。这里改成从所有可转世的牌里随机选一张。
// 用 postfix 只改结果（不替换方法），避免此前 prefix 全替换导致的事件挂起。
[HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceStarterCard")]
public static class RandomizeTranscendenceStarterCardPatch
{
    [HarmonyPostfix]
    static void Randomize(Player player, ref CardModel __result)
    {
        // __result 为 null 表示牌库里没有可转世的牌，保持不变。
        if(__result == null)
            return;

        var prop = typeof(ArchaicTooth).GetProperty("TranscendenceUpgrades",
            BindingFlags.Static | BindingFlags.NonPublic);
        if(prop?.GetValue(null) is not Dictionary<ModelId, CardModel> upgrades || upgrades.Count == 0)
            return;

        var eligible = player.Deck.Cards
            .Where(c => upgrades.ContainsKey(c.Id))
            .ToList();
        if(eligible.Count <= 1)
            return;   // 0/1 张时无需随机（原版结果已是唯一）

        var rng = new Rng(player.PlayerRng.Rewards.Seed, player.PlayerRng.Rewards.Counter);

        __result = rng.NextItem(eligible);
    }
}
