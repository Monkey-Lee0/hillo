using MegaCrit.Sts2.Core.Entities.Powers;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 每当你打出一张奇数耗能的牌，抽 1 张牌。
public class BinaryPower : HilloPowerModel
{
    public BinaryPower() : base(PowerType.Buff, PowerStackType.Counter)
    {
        OnCardPlayed(HilloStep.When(
            ctx => (ctx.CardPlay?.Resources.EnergySpent ?? 0) % 2 == 1,
            new HilloDrawCardStep("Draw", 1)));
    }
}
