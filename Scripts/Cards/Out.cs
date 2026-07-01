using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.CardSelection;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class Out : HilloCardModel
{
    public Out() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None,
        [new HilloDrawCardStep("Draw", 1), new RandomMinionTransformStep()]) {}

    // 选择 2 张手牌，各自变化为一张随机仆从牌
    private class RandomMinionTransformStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var player = cardPlay.Card.Owner;
            if(player.Creature.CombatState is not { } combatState)
                return;

            var prefs = new CardSelectorPrefs(new LocString("card_selection", "OUT_SELECT"), 2);
            var selected = await CardSelectCmd.FromHand(choiceContext, player, prefs, filter: null, source: cardPlay.Card);

            if(selected == null || !selected.Any())
                return;

            var allMinionCards = ModelDb.AllCards.Where(c => c.Tags.Contains(CardTag.Minion)).ToList();
            if(!allMinionCards.Any())
                return;

            var rng = player.PlayerRng.Rewards;

            foreach(var original in selected)
            {
                var minion = rng.NextItem(allMinionCards);
                var replacement = combatState.CreateCard(minion, player);

                if(cardPlay.Card.IsUpgraded && replacement.IsUpgradable)
                {
                    replacement.UpgradeInternal();
                    replacement.FinalizeUpgradeInternal();
                }

                await CardCmd.Transform(original, replacement);
            }
        }

        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            // HostCard 由 HilloCardModel 在迭代 tip 时 set，让 minion 预览跟随母卡升级态
            bool up = HostCard?.IsUpgraded ?? false;
            foreach(var card in ModelDb.AllCards.Where(c => c.Tags.Contains(CardTag.Minion)))
                yield return HoverTipFactory.FromCard(card, upgrade: up);
        }
    }
}
