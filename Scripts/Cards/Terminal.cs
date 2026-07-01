using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(DefectCardPool))]
public class Terminal : HilloCardModel
{
    public Terminal() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self,
        [new CostShiftStep(), new HilloEnergyUpgradeStep(-1)],
        keywords: [CardKeyword.Exhaust]) {}

    // 选一张手牌本场耗能 -1；选一张抽牌堆的牌本场耗能 +1。
    private class CostShiftStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var player = CurrentPlayer(cardPlay);

            var handPrefs = new CardSelectorPrefs(new LocString("card_selection", "TERMINAL_HAND_SELECT"), 1);
            var handSel = await CardSelectCmd.FromHand(choiceContext, player, handPrefs, filter: null, source: cardPlay.Card);
            handSel?.FirstOrDefault()?.EnergyCost.AddThisCombat(-1);

            var drawPrefs = new CardSelectorPrefs(new LocString("card_selection", "TERMINAL_DRAW_SELECT"), 1);
            var drawSel = await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(player), player, drawPrefs);
            drawSel?.FirstOrDefault()?.EnergyCost.AddThisCombat(1);
        }
    }
}
