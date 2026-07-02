using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.CardSelection;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class BackStep : HilloCardModel
{
    public BackStep() : base(1, CardType.Skill, CardRarity.Common, TargetType.None,
        [new HilloBlockSelfStep(5, upgradeDiff:3), new DiscardThenDrawStep()]) {}

    // 丢弃3张牌；若丢光手牌则抽3张
    private class DiscardThenDrawStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var player = cardPlay.Card.Owner;

            var prefs = new CardSelectorPrefs(new LocString("card_selection", "BACKSTEP_SELECT"), 3);
            var selected = await CardSelectCmd.FromHand(choiceContext, player, prefs, filter: null, source: cardPlay.Card);

            int handCount = player.PlayerCombatState.Hand.Cards.Count();

            if(selected == null || !selected.Any())
                return;

            foreach(var card in selected)
                await CardCmd.Discard(choiceContext, card);

            if(selected.Count() == handCount)
                await CardPileCmd.Draw(choiceContext, 3, player);
        }
    }
}
