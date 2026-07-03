using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace hillo.Modules.Step;

// 自身失去生命（不可格挡，类似中毒的 HP loss）。数值用 HpLossVar，可升级、可多次。
// scaleByAmount=true 时失去量 × 宿主能力的层数（Amount），用于按层缩放的能力（如 AllIn），
// 且卡面/tooltip 也显示缩放后的值（ScalingHpLossVar 覆写 ToString）。
public class HilloLoseHpSelfStep : HilloStep
{
    // HpLossVar 变体：显示值 = 基础值 × 宿主能力的 Amount（宿主非能力时按 ×1）。
    // 仍是 HpLossVar，故 DynamicVars.HpLoss 强转与效果读取 BaseValue 均不受影响。
    private class ScalingHpLossVar : HpLossVar
    {
        public ScalingHpLossVar(int hpLoss) : base(hpLoss) {}
        public override string ToString()
        {
            int mult = (_owner as PowerModel)?.Amount ?? 1;
            return ((int)(BaseValue * mult)).ToString();
        }
    }

    protected HpLossVar _hpLossVar;
    protected int _hpLoss;
    protected readonly int _diff;
    protected int _times;
    protected bool _scaleByAmount;

    public HilloLoseHpSelfStep(int hpLoss, int upgradeDiff=0, int times=1, bool scaleByAmount=false)
    {
        _hpLoss = hpLoss;
        _diff = upgradeDiff;
        _times = times;
        _scaleByAmount = scaleByAmount;
        _hpLossVar = scaleByAmount ? new ScalingHpLossVar(hpLoss) : new HpLossVar(hpLoss);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        decimal amount = ctx.Vars.HpLoss.BaseValue * (_scaleByAmount ? ctx.Amount : 1m);
        if(amount <= 0)
            return;
        for(int i=0; i<_times; i++)
            await CreatureCmd.Damage(
                choiceContext,
                ctx.Owner,
                amount,
                ValueProp.Unblockable,
                dealer: null,
                cardSource: ctx.Card,
                cardPlay: ctx.CardPlay
            );
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.HpLoss.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _hpLossVar;
    }
}

// 自身回复生命。数值用 HealVar，可升级、可多次。
// scaleByAmount=true 时回复量 × 宿主能力的层数（Amount），tooltip 同步显示缩放值。
public class HilloGainHpSelfStep : HilloStep
{
    private class ScalingHealVar : HealVar
    {
        public ScalingHealVar(int heal) : base(heal) {}
        public override string ToString()
        {
            int mult = (_owner as PowerModel)?.Amount ?? 1;
            return ((int)(BaseValue * mult)).ToString();
        }
    }

    protected HealVar _healVar;
    protected int _heal;
    protected readonly int _diff;
    protected int _times;
    protected bool _scaleByAmount;

    public HilloGainHpSelfStep(int heal, int upgradeDiff=0, int times=1, bool scaleByAmount=false)
    {
        _heal = heal;
        _diff = upgradeDiff;
        _times = times;
        _scaleByAmount = scaleByAmount;
        _healVar = scaleByAmount ? new ScalingHealVar(heal) : new HealVar(heal);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        decimal amount = ctx.Vars.Heal.BaseValue * (_scaleByAmount ? ctx.Amount : 1m);
        if(amount <= 0)
            return;
        for(int i=0; i<_times; i++)
            await CreatureCmd.Heal(ctx.Owner, amount);
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.Heal.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _healVar;
    }
}
