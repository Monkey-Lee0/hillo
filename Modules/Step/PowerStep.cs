using System.Linq;
using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloPowerSelfStep<T> : HilloStep where T : PowerModel, new()
{
    protected IntVar _stacksVar;
    protected string _name;
    protected int _stacks;
    protected readonly int _diff;
    protected bool _needTips;
    protected bool _scaleByAmount;

    public HilloPowerSelfStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false, bool scaleByAmount=false)
    {
        _name = name;
        _stacks = stacks;
        _diff = upgradeDiff;
        _stacksVar = new IntVar(name, stacks);
        _needTips = needTips;
        _scaleByAmount = scaleByAmount;
    }

    // 施加层数：scaleByAmount 时 × 宿主能力的 Amount（用于「每层施加 N」的能力）。
    protected decimal Stacks(HilloContext ctx)
        => ctx.Vars[_name].BaseValue * (_scaleByAmount ? ctx.Amount : 1m);

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        await PowerCmd.Apply<T>(
            choiceContext,
            [ctx.Owner],
            Stacks(ctx),
            applier: null,
            cardSource: ctx.Card
        );
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _stacksVar;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        if(_needTips)
            yield return HoverTipFactory.FromPower<T>();
    }
}

public class HilloPowerAllStep<T> : HilloPowerSelfStep<T> where T : PowerModel, new()
{
    public HilloPowerAllStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false, bool scaleByAmount=false)
        :base(name, stacks, upgradeDiff, needTips, scaleByAmount) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var owner = ctx.Owner;
        if(owner.CombatState is not { } combatState)
            return;
        var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
        if(enemies.Count == 0)
            return;
        await PowerCmd.Apply<T>(
            choiceContext,
            enemies,
            Stacks(ctx),
            applier: owner,
            cardSource: ctx.Card
        );
    }
}

public class HilloPowerSingleStep<T> : HilloPowerAllStep<T> where T : PowerModel, new()
{
    public HilloPowerSingleStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false, bool scaleByAmount=false)
        :base(name, stacks, upgradeDiff, needTips, scaleByAmount) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        await PowerCmd.Apply<T>(
            choiceContext,
            ctx.Target,
            Stacks(ctx),
            applier: ctx.Owner,
            cardSource: ctx.Card
        );
    }
}

public class HilloPowerRandomStep<T> : HilloPowerAllStep<T> where T : PowerModel, new()
{
    public HilloPowerRandomStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false, bool scaleByAmount=false)
        :base(name, stacks, upgradeDiff, needTips, scaleByAmount) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        var owner = player.Creature;
        if(owner.CombatState is not { } combatState)
            return;
        var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
        if(enemies.Count == 0)
            return;
        var target = player.RunState.Rng.CombatTargets.NextItem(enemies);
        await PowerCmd.Apply<T>(
            choiceContext,
            target,
            Stacks(ctx),
            applier: owner,
            cardSource: ctx.Card
        );
    }
}

// 从自身移除能力 T（若存在）。典型用途：能力在自己的 Hook 里触发后自我移除（如 AllIn）。
public class HilloRemovePowerSelfStep<T> : HilloStep where T : PowerModel
{
    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        await PowerCmd.Remove<T>(ctx.Owner);
    }
}
