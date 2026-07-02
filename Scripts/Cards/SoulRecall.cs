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
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class SoulRecall : HilloCardModel
{
    public SoulRecall() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self,
        [new RecallSoulsStep()],
        keywords: [CardKeyword.Exhaust]) {}

    // 将消耗堆的所有灵魂加入抽牌堆；升级版会先把它们升级。
    private class RecallSoulsStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var player = CurrentPlayer(cardPlay);
            var souls = PileType.Exhaust.GetPile(player).Cards.Where(c => c is Soul).ToList();
            if(souls.Count == 0)
                return;

            if(cardPlay.Card.IsUpgraded)
                foreach(var soul in souls)
                    if(soul.IsUpgradable)
                    {
                        soul.UpgradeInternal();
                        soul.FinalizeUpgradeInternal();
                    }

            await CardPileCmd.Add(souls, PileType.Draw);
        }

        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            var model = ModelDb.Card<Soul>();
            if(model != null)
            {
                yield return HoverTipFactory.FromCard(model, upgrade: false);
                yield return HoverTipFactory.FromCard(model, upgrade: true);
            }
        }
    }
}
