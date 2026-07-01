using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace hillo.Scripts.Power;

public class ImpendingDoomPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/ImpendingDoomPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/ImpendingDoomPower.png";

    // 你的回合结束时，斩杀灾厄层数 >= 当前生命的敌人（即已被灾厄判定的敌人）。
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if(side != CombatSide.Player)
            return;
        if(Owner?.CombatState is not { } combatState)
            return;

        var doomed = DoomPower.GetDoomedCreatures(combatState.HittableEnemies);
        if(doomed.Count > 0)
            await DoomPower.DoomKill(doomed);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<DoomPower>();
        }
    }
}
