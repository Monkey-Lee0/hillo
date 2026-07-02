using MegaCrit.Sts2.Core.Entities.Powers;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 你的回合结束时，斩杀灾厄层数 >= 当前生命的敌人（灾厄 hovertip 由 DoomKillStep 提供）。
public class ImpendingDoomPower : HilloPowerModel
{
    public ImpendingDoomPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        OnSideTurnEnd(new HilloDoomKillStep());
    }
}
