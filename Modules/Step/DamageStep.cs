using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace hillo.Modules.Step;

public class HilloDamageAllStep: HilloStep
{
    protected DamageVar _damageVar;
    protected int _damage;
    protected int _diff;
    protected int _times;
    public HilloDamageAllStep(int damage, int upgradeDiff=0, int times=1)
    {
        _damage = damage;
        _diff = upgradeDiff;
        _times = times;
        _damageVar = new DamageVar(damage, ValueProp.Move);
    }
    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(cardPlay.Card.DynamicVars.Damage.BaseValue)
                .FromCard(cardPlay.Card)
                .TargetingAllOpponents(CurrentPlayer(cardPlay).Creature.CombatState)
                .Execute(choiceContext);
    }
    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.Damage.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _damageVar;
    }
}

public class HilloDamageSingleStep: HilloDamageAllStep
{
    public HilloDamageSingleStep(int damage, int upgradeDiff=0, int times=1)
        :base(damage, upgradeDiff, times) {}
    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(cardPlay.Card.DynamicVars.Damage.BaseValue)
                .FromCard(cardPlay.Card)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
    }
}

public class HilloDamageRandomStep: HilloDamageAllStep
{
    public HilloDamageRandomStep(int damage, int upgradeDiff=0, int times=1)
        :base(damage, upgradeDiff, times) {}
    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for(int i=0; i<_times; i++)
            await DamageCmd.Attack(cardPlay.Card.DynamicVars.Damage.BaseValue)
                .FromCard(cardPlay.Card)
                .TargetingRandomOpponents(CurrentPlayer(cardPlay).Creature.CombatState)
                .Execute(choiceContext);
    }
}
