using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Orbs;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 每回合开始：获得 1 个充能球栏位 + 生成 1 个黑暗充能球。
// 用 HilloPowerModel 按 Hook 组装 Step（复用 OrbSlot / ChannelOrb 两个通用 Step）。
public class UmbralIncursionPower : HilloPowerModel
{
    public UmbralIncursionPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        OnPlayerTurnStart(
            new HilloOrbSlotStep("Slot", 1),
            new HilloChannelOrbStep<DarkOrb>("Channel", 1));
    }
}
