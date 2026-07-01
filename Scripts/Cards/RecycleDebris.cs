using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class RecycleDebris : HilloCardModel
{
    public RecycleDebris() : base(1, CardType.Skill, CardRarity.Common, TargetType.None,
        [new ExhaustDebrisStep(), new HilloEnergyUpgradeStep(-1)],
        extraHoverTips: DebrisTips()) {}

    private static IEnumerable<IHoverTip> DebrisTips()
    {
        var card = ModelDb.Card<Debris>();
        if(card != null)
            yield return HoverTipFactory.FromCard(card, upgrade: false);
    }

    // 消耗手牌中所有「碎屑」，每张得 1 星
    private class ExhaustDebrisStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var player = CurrentPlayer(cardPlay);
            var hand = PileType.Hand.GetPile(player).Cards.ToList();
            var debris = hand.Where(c => c is Debris).ToList();
            if(debris.Count == 0)
                return;

            foreach(var card in debris)
            {
                await CardCmd.Exhaust(choiceContext, card, causedByEthereal: false);
                await PlayerCmd.GainStars(1, player);
            }
        }
    }
}
