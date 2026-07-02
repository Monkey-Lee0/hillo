using hillo.Modules.Step;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace hillo.Modules.Step;

// 斩杀所有「灾厄（Doom）层数 >= 当前生命」的敌人（DoomPower 的判定与击杀）。
public class HilloDoomKillStep : HilloStep
{
    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        if(ctx.Owner.CombatState is not { } combatState)
            return;
        var doomed = DoomPower.GetDoomedCreatures(combatState.HittableEnemies);
        if(doomed.Count > 0)
            await DoomPower.DoomKill(doomed);
    }

    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        yield return HoverTipFactory.FromPower<DoomPower>();
    }
}
