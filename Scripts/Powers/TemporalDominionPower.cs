using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace hillo.Scripts.Power;

public class TemporalDominionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // Counter：让 Amount 保留施加的倍率（Single 会强制为 1）。
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/TemporalDominionPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/TemporalDominionPower.png";

    // 每回合（你的回合）结束时：奥斯提死亡，给予所有敌人 奥斯提血量上限 × Amount 层灾厄。
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if(side != CombatSide.Player)
            return;
        var owner = Owner;
        if(owner?.Player is not { } player || !player.IsOstyAlive)
            return;
        if(owner.CombatState is not { } combatState)
            return;

        int doom = player.Osty!.MaxHp * (int)Amount;
        await CreatureCmd.Kill(player.Osty!);
        if(doom > 0)
            await PowerCmd.Apply<DoomPower>(choiceContext, combatState.HittableEnemies, doom, owner, null);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<DoomPower>();
        }
    }
}
