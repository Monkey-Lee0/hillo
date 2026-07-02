using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class RetraceStrike : HilloCardModel
{
    public RetraceStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy,
        [new HilloDamageSingleStep(6, upgradeDiff:3), new CloneFromDiscardStep()],
        tags: new HashSet<CardTag> { CardTag.Strike }) {}


    // 从弃牌堆选 1 张攻击/技能牌，克隆后加消耗关键字加入手牌
    private class CloneFromDiscardStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var player = ctx.Player;
            if(player.Creature.CombatState is not { } combatState)
                return;

            var prefs = new CardSelectorPrefs(new LocString("card_selection", "RETRACE_SELECT"), 1);
            var selected = await CardSelectCmd.FromCombatPile(
                choiceContext,
                PileType.Discard.GetPile(player),
                player,
                prefs,
                c => c.Type == CardType.Attack || c.Type == CardType.Skill
            );

            var picked = selected?.FirstOrDefault();
            if(picked == null)
                return;

            var clone = combatState.CloneCard(picked);
            clone.AddKeyword(CardKeyword.Exhaust);
            await CardPileCmd.Add(clone, PileType.Hand);
        }
    }
}
