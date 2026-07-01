using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace hillo.Modules.Step;

// 召唤奥斯提（已存在则提升其最大生命）。数值用 SummonVar，可升级、带召唤提示。
public class HilloSummonStep : HilloStep
{
    protected SummonVar _summonVar;
    protected int _amount;
    protected readonly int _diff;

    public HilloSummonStep(int amount, int upgradeDiff=0)
    {
        _amount = amount;
        _diff = upgradeDiff;
        _summonVar = new SummonVar(amount);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await OstyCmd.Summon(
            choiceContext,
            CurrentPlayer(cardPlay),
            cardPlay.Card.DynamicVars.Summon.BaseValue,
            cardPlay.Card
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
        // 取当前显示卡上的活 var（升级后会反映新值），而非 step 持有的模板 _summonVar。
        DynamicVar summon = HostCard != null ? HostCard.DynamicVars.Summon : _summonVar;
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

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        if(Osty.CheckMissingWithAnim(player))
            return;
        if(player.Creature.CombatState is not { } combatState)
            return;
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(cardPlay.Card.DynamicVars.OstyDamage.BaseValue)
                .FromOsty(player.Osty!, cardPlay.Card)
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

// 奥斯提攻击 cardPlay.Target
public class HilloOstyAttackSingleStep : HilloOstyAttackAllStep
{
    public HilloOstyAttackSingleStep(int damage, int upgradeDiff=0, int times=1)
        :base(damage, upgradeDiff, times) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        if(Osty.CheckMissingWithAnim(player))
            return;
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(cardPlay.Card.DynamicVars.OstyDamage.BaseValue)
                .FromOsty(player.Osty!, cardPlay.Card)
                .Targeting(cardPlay.Target)
                .WithAttackerAnim("attack_poke", 0.3f)
                .Execute(choiceContext);
    }
}

// 奥斯提攻击随机敌人
public class HilloOstyAttackRandomStep : HilloOstyAttackAllStep
{
    public HilloOstyAttackRandomStep(int damage, int upgradeDiff=0, int times=1)
        :base(damage, upgradeDiff, times) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        if(Osty.CheckMissingWithAnim(player))
            return;
        if(player.Creature.CombatState is not { } combatState)
            return;
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(cardPlay.Card.DynamicVars.OstyDamage.BaseValue)
                .FromOsty(player.Osty!, cardPlay.Card)
                .TargetingRandomOpponents(combatState)
                .WithAttackerAnim("attack_poke", 0.3f)
                .Execute(choiceContext);
    }
}

// 奥斯提死亡
public class HilloOstyDieStep : HilloStep
{
    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        if(player.IsOstyAlive)
            await CreatureCmd.Kill(player.Osty!);
    }
}
