using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class Explorer : HilloCardModel
{
    public Explorer() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.None,
        [new HilloBlockSelfStep(14, upgradeDiff:3), new DiscardThenDrawSameStep(2, upgradeDiff:1)]) {}

    // 选 0~N 张手牌丢弃，抽等量
    private class DiscardThenDrawSameStep : HilloStep
    {
        private readonly IntVar _discardVar;
        private readonly string _name = "Discard";
        private readonly int _diff;

        public DiscardThenDrawSameStep(int discard, int upgradeDiff=0)
        {
            _diff = upgradeDiff;
            _discardVar = new IntVar(_name, discard);
        }

        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var player = CurrentPlayer(cardPlay);
            int max = (int)cardPlay.Card.DynamicVars[_name].BaseValue;

            var locString = new LocString("card_selection", "EXPLORER_SELECT");
            locString.Add(cardPlay.Card.DynamicVars[_name]);

            var prefs = new CardSelectorPrefs(locString, 0, max);
            var selected = await CardSelectCmd.FromHand(choiceContext, player, prefs, filter: null, source: cardPlay.Card);

            if(selected == null || !selected.Any())
                return;

            int discardCount = selected.Count();
            foreach(var card in selected)
                await CardCmd.Discard(choiceContext, card);

            await CardPileCmd.Draw(choiceContext, discardCount, player);
        }

        public override void OnUpgrade(CardModel card)
        {
            if(_diff == 0)
                return ;
            card.DynamicVars[_name].UpgradeValueBy(_diff);
        }
        public override IEnumerable<DynamicVar> GetDynamicVars()
        {
            yield return _discardVar;
        }
    }
}
