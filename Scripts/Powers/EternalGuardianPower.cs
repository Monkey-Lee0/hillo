using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

using hillo.Scripts.Cards;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

public class EternalGuardianPower : HilloPowerModel
{
    public EternalGuardianPower() : base(PowerType.Buff, PowerStackType.Counter)
    {
        OnCardPlayed(new SummonPerCardStep());
    }

    private class SummonPerCardStep : HilloStep
    {
        private readonly SummonVar _summonVar = new SummonVar(1);

        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            int amount = (int)ctx.Amount;
            var card = ctx.CardPlay?.Card;
            if(card is EternalGuardian guardian)
            {
                amount -= 1;
                if(card.IsUpgraded)
                    amount -= 1;
            }
            if(amount <= 0)
                return;

            await OstyCmd.Summon(choiceContext, ctx.Player, amount, ctx.Card);
        }

        public override IEnumerable<DynamicVar> GetDynamicVars()
        {
            yield return _summonVar;
        }
        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            yield return HoverTipFactory.Static(StaticHoverTip.SummonDynamic, _summonVar);
        }
    }
}
