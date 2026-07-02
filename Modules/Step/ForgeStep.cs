using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloForgeSelfStep : HilloStep
{
    protected IntVar _forgeVar;
    protected string _name;
    protected int _forge;
    protected readonly int _diff;

    public HilloForgeSelfStep(string name, int forge, int upgradeDiff=0)
    {
        _name = name;
        _forge = forge;
        _diff = upgradeDiff;
        _forgeVar = new IntVar(name, forge);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        await ForgeCmd.Forge(
            ctx.Vars[_name].BaseValue,
            ctx.Player,
            ctx.Card
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
        yield return _forgeVar;
    }
}
