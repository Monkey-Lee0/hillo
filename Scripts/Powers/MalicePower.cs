using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 每当你打出一张牌（本卡自身少触发 1 次），等概率给随机敌人 易伤/虚弱/-1力量，重复 Amount 次。
public class MalicePower : HilloPowerModel
{
    public MalicePower() : base(PowerType.Buff, PowerStackType.Counter)
    {
        OnCardPlayed(new MaliceStep());
    }

    private class MaliceStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var owner = ctx.Owner;
            if(owner.CombatState is not { } combatState)
                return;
            var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
            if(!enemies.Any())
                return;

            int times = (int)ctx.Amount;
            if(ctx.CardPlay?.Card is Cards.Malice)
                times = Math.Max(0, times - 1);

            var rng = new Random();
            var source = ctx.CardPlay?.Card;
            for(int i=0; i<times; i++)
            {
                var target = enemies[rng.Next(enemies.Count)];
                switch(rng.Next(3))
                {
                    case 0: await PowerCmd.Apply<VulnerablePower>(choiceContext, target, 1, owner, source); break;
                    case 1: await PowerCmd.Apply<WeakPower>(choiceContext, target, 1, owner, source); break;
                    case 2: await PowerCmd.Apply<StrengthPower>(choiceContext, target, -1, owner, source); break;
                }
            }
        }

        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            yield return HoverTipFactory.FromPower<WeakPower>();
            yield return HoverTipFactory.FromPower<VulnerablePower>();
            yield return HoverTipFactory.FromPower<StrengthPower>();
        }
    }
}
