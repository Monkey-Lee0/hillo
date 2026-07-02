using System.Linq;
using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloPassiveAllStep<T> : HilloStep where T : OrbModel
{
    protected IntVar _timesVar;
    protected string _name;
    protected int _times;
    protected readonly int _diff;

    public HilloPassiveAllStep(string name, int times=1, int upgradeDiff=0)
    {
        _name = name;
        _times = times;
        _diff = upgradeDiff;
        _timesVar = new IntVar(name, times);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        if(player.Creature.CombatState is not { } combatState)
            return;

        var orbs = player.PlayerCombatState?.OrbQueue?.Orbs?.OfType<T>().ToList();
        if(orbs == null || orbs.Count == 0)
            return;

        int times = (int)ctx.Vars[_name].BaseValue;
        for(int t=0; t<times; t++)
            foreach(var orb in orbs)
                foreach(var enemy in combatState.Enemies.Where(e => e.IsAlive))
                    await OrbCmd.Passive(choiceContext, orb, enemy);
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _timesVar;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        yield return HoverTipFactory.FromOrb<T>();
    }
}

public class HilloPassiveSingleStep<T> : HilloPassiveAllStep<T> where T : OrbModel
{
    public HilloPassiveSingleStep(string name, int times=1, int upgradeDiff=0)
        :base(name, times, upgradeDiff) {}

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;

        var orbs = player.PlayerCombatState?.OrbQueue?.Orbs?.OfType<T>().ToList();
        if(orbs == null || orbs.Count == 0)
            return;

        int times = (int)ctx.Vars[_name].BaseValue;
        for(int t=0; t<times; t++)
            foreach(var orb in orbs)
                await OrbCmd.Passive(choiceContext, orb, ctx.Target);
    }
}
