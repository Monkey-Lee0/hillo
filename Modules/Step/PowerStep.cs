using System.Linq;
using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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

    public HilloPowerSelfStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false)
    {
        _name = name;
        _stacks = stacks;
        _diff = upgradeDiff;
        _stacksVar = new IntVar(name, stacks);
        _needTips = needTips;
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<T>(
            choiceContext,
            [CurrentPlayer(cardPlay).Creature],
            cardPlay.Card.DynamicVars[_name].BaseValue,
            applier: null,
            cardSource: cardPlay.Card
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
    public HilloPowerAllStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false)
        :base(name, stacks, upgradeDiff, needTips) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = CurrentPlayer(cardPlay).Creature;
        if(owner.CombatState is not { } combatState)
            return;
        var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
        if(enemies.Count == 0)
            return;
        await PowerCmd.Apply<T>(
            choiceContext,
            enemies,
            cardPlay.Card.DynamicVars[_name].BaseValue,
            applier: owner,
            cardSource: cardPlay.Card
        );
    }
}

public class HilloPowerSingleStep<T> : HilloPowerAllStep<T> where T : PowerModel, new()
{
    public HilloPowerSingleStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false)
        :base(name, stacks, upgradeDiff, needTips) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<T>(
            choiceContext,
            cardPlay.Target,
            cardPlay.Card.DynamicVars[_name].BaseValue,
            applier: CurrentPlayer(cardPlay).Creature,
            cardSource: cardPlay.Card
        );
    }
}

public class HilloPowerRandomStep<T> : HilloPowerAllStep<T> where T : PowerModel, new()
{
    public HilloPowerRandomStep(string name, int stacks=1, int upgradeDiff=0, bool needTips=false)
        :base(name, stacks, upgradeDiff, needTips) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
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
            cardPlay.Card.DynamicVars[_name].BaseValue,
            applier: owner,
            cardSource: cardPlay.Card
        );
    }
}
