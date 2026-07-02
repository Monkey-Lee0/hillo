using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 每回合结束时：奥斯提死亡，给予所有敌人「奥斯提血量上限 × Amount」层灾厄。
public class TemporalDominionPower : HilloPowerModel
{
    public TemporalDominionPower() : base(PowerType.Buff, PowerStackType.Counter)
    {
        OnSideTurnEnd(new OstyDoomStep());
    }

    // 读奥斯提血量上限（需在死亡前读）× Amount 作为灾厄量，施加给全体敌人，然后杀死奥斯提。
    private class OstyDoomStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var player = ctx.Player;
            if(!player.IsOstyAlive)
                return;
            if(ctx.Owner.CombatState is not { } combatState)
                return;

            int doom = player.Osty!.MaxHp * (int)ctx.Amount;
            await CreatureCmd.Kill(player.Osty!);
            if(doom > 0)
                await PowerCmd.Apply<DoomPower>(choiceContext, combatState.HittableEnemies, doom, ctx.Owner, null);
        }

        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            yield return HoverTipFactory.FromPower<DoomPower>();
        }
    }
}
