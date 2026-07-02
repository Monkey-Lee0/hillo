using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

using hillo.Scripts.Cards;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 回合结束时把本回合临时获得的敏捷扣回并移除自己。敏捷量 = 层数（Amount）。
public class WindWalkTemporaryDexterityPower : HilloPowerModel
{
    public WindWalkTemporaryDexterityPower() : base(PowerType.Buff, PowerStackType.Counter)
    {
        OnSideTurnEnd(
            new HilloPowerSelfStep<DexterityPower>("NegDex", -1, scaleByAmount: true),
            new HilloRemovePowerSelfStep<WindWalkTemporaryDexterityPower>());
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach(var t in base.ExtraHoverTips)
                yield return t;
            yield return HoverTipFactory.FromCard<WindWalk>();
        }
    }
}

// 每打出一张攻击牌，本回合内获得 Amount 点敏捷（用一个临时敏捷能力在回合末扣回）。
public class WindWalkPower : HilloPowerModel
{
    public WindWalkPower() : base(PowerType.Buff, PowerStackType.Counter)
    {
        OnCardPlayed(HilloStep.When(
            ctx => ctx.CardPlay?.Card.Type == CardType.Attack,
            new HilloPowerSelfStep<DexterityPower>("Dex", 1, scaleByAmount: true, needTips: true),
            new HilloPowerSelfStep<WindWalkTemporaryDexterityPower>("TempDex", 1, scaleByAmount: true)));
    }
}
