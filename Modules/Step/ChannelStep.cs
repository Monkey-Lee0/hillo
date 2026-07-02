using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloChannelOrbStep<T> : HilloStep where T : OrbModel
{
    protected IntVar _countVar;
    protected string _name;
    protected int _count;
    protected readonly int _diff;

    public HilloChannelOrbStep(string name, int count=1, int upgradeDiff=0)
    {
        _name = name;
        _count = count;
        _diff = upgradeDiff;
        _countVar = new IntVar(name, count);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        int count = (int)ctx.Vars[_name].BaseValue;
        for(int i=0; i<count; i++)
            await OrbCmd.Channel<T>(choiceContext, player);
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _countVar;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Channeling);
        yield return HoverTipFactory.FromOrb<T>();
    }
}

public class HilloChannelRandomStep : HilloStep
{
    protected IntVar _countVar;
    protected string _name;
    protected int _count;
    protected readonly int _diff;

    public HilloChannelRandomStep(string name, int count=1, int upgradeDiff=0)
    {
        _name = name;
        _count = count;
        _diff = upgradeDiff;
        _countVar = new IntVar(name, count);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        int count = (int)ctx.Vars[_name].BaseValue;
        for(int i=0; i<count; i++)
        {
            var orb = OrbModel.GetRandomOrb(player.RunState.Rng.CombatOrbGeneration).ToMutable();
            await OrbCmd.Channel(choiceContext, orb, player);
        }
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _countVar;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Channeling);
    }
}
