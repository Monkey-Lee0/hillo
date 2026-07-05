using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace hillo.Modules.Step;

// 召唤奥斯提（已存在则提升其最大生命）。数值用 SummonVar，可升级、带召唤提示。
// scaleByAmount=true 时召唤量 × 宿主能力的层数（Amount），tooltip 同步显示缩放值。
public class HilloSummonStep : HilloStep
{
    // SummonVar 变体：显示值 = 基础值 × 宿主能力的 Amount（宿主非能力时 ×1）。
    private class ScalingSummonVar : SummonVar
    {
        public ScalingSummonVar(int amount) : base(amount) {}
        public override string ToString()
        {
            int mult = (_owner as PowerModel)?.Amount ?? 1;
            return ((int)(BaseValue * mult)).ToString();
        }
    }

    protected SummonVar _summonVar;
    protected int _amount;
    protected readonly int _diff;
    protected bool _scaleByAmount;

    public HilloSummonStep(int amount, int upgradeDiff=0, bool scaleByAmount=false)
    {
        _amount = amount;
        _diff = upgradeDiff;
        _scaleByAmount = scaleByAmount;
        _summonVar = scaleByAmount ? new ScalingSummonVar(amount) : new SummonVar(amount);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        decimal amount = ctx.Vars.Summon.BaseValue * (_scaleByAmount ? ctx.Amount : 1m);
        if(amount <= 0)
            return;
        await OstyCmd.Summon(
            choiceContext,
            ctx.Player,
            amount,
            ctx.Card
        );
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.Summon.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _summonVar;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        // 取宿主当前的活 var（升级后反映新值），而非 step 持有的模板 _summonVar。
        DynamicVar summon = HostVars != null ? HostVars.Summon : _summonVar;
        yield return HoverTipFactory.Static(StaticHoverTip.SummonDynamic, summon);
    }
}

// 奥斯提攻击所有敌人。伤害用 OstyDamageVar（随奥斯提的力量/虚弱等结算），可升级、可多段。
public class HilloOstyAttackAllStep : HilloStep
{
    protected OstyDamageVar _damageVar;
    protected int _damage;
    protected readonly int _diff;
    protected int _times;

    public HilloOstyAttackAllStep(int damage, int upgradeDiff=0, int times=1)
    {
        _damage = damage;
        _diff = upgradeDiff;
        _times = times;
        _damageVar = new OstyDamageVar(damage, ValueProp.Move);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        if(Osty.CheckMissingWithAnim(player))
            return;
        if(player.Creature.CombatState is not { } combatState)
            return;
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(ctx.Vars.OstyDamage.BaseValue)
                .FromOsty(player.Osty!, ctx.Card, ctx.CardPlay)
                .TargetingAllOpponents(combatState)
                .WithAttackerAnim("attack_poke", 0.3f)
                .Execute(choiceContext);
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.OstyDamage.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _damageVar;
    }
}

// 奥斯提攻击 ctx.Target
public class HilloOstyAttackSingleStep : HilloOstyAttackAllStep
{
    public HilloOstyAttackSingleStep(int damage, int upgradeDiff=0, int times=1)
        :base(damage, upgradeDiff, times) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        if(Osty.CheckMissingWithAnim(player))
            return;
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(ctx.Vars.OstyDamage.BaseValue)
                .FromOsty(player.Osty!, ctx.Card, ctx.CardPlay)
                .Targeting(ctx.Target)
                .WithAttackerAnim("attack_poke", 0.3f)
                .Execute(choiceContext);
    }
}

// 奥斯提攻击随机敌人
public class HilloOstyAttackRandomStep : HilloOstyAttackAllStep
{
    public HilloOstyAttackRandomStep(int damage, int upgradeDiff=0, int times=1)
        :base(damage, upgradeDiff, times) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        if(Osty.CheckMissingWithAnim(player))
            return;
        if(player.Creature.CombatState is not { } combatState)
            return;
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(ctx.Vars.OstyDamage.BaseValue)
                .FromOsty(player.Osty!, ctx.Card, ctx.CardPlay)
                .TargetingRandomOpponents(combatState)
                .WithAttackerAnim("attack_poke", 0.3f)
                .Execute(choiceContext);
    }
}

// 奥斯提死亡
public class HilloOstyDieStep : HilloStep
{
    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        if(player.IsOstyAlive)
            await CreatureCmd.Kill(player.Osty!);
    }
}
