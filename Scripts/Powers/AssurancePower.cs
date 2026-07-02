using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 每当你打出一张非奇巧的能力牌时，为其附加奇巧、设其本场耗能为 3、复制它并将其加入弃牌堆。
public class AssurancePower : HilloPowerModel
{
    public AssurancePower() : base(PowerType.Buff, PowerStackType.Single)
    {
        OnCardPlayed(new SlyCopyStep());
    }

    private class SlyCopyStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            if(ctx.Owner.CombatState is not { } combatState)
                return;
            var card = ctx.CardPlay?.Card;
            if(card == null || card.Type != CardType.Power)
                return;
            if(card.Keywords.Contains(CardKeyword.Sly))
                return;

            card.AddKeyword(CardKeyword.Sly);
            card.EnergyCost.SetThisCombat(3);
            combatState.CloneCard(card);
            await CardPileCmd.Add(card, PileType.Discard);
        }

        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            yield return HoverTipFactory.FromKeyword(CardKeyword.Sly);
        }
    }
}
