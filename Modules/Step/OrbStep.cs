using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace hillo.Modules.Step;

// 获得充能球栏位；数值为负则为失去。数目可升级。
// bypassCap=true 时绕过原版 10 上限（OrbCmd.AddSlots 里的 const），直接改 OrbQueue 容量。
public class HilloOrbSlotStep : HilloStep
{
    protected IntVar _slotsVar;
    protected string _name;
    protected int _slots;
    protected readonly int _diff;
    protected bool _bypassCap;

    public HilloOrbSlotStep(string name, int slots, int upgradeDiff=0, bool bypassCap=false)
    {
        _name = name;
        _slots = slots;
        _diff = upgradeDiff;
        _bypassCap = bypassCap;
        _slotsVar = new IntVar(name, slots);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var player = ctx.Player;
        int amount = (int)ctx.Vars[_name].BaseValue;
        if(amount == 0)
            return;

        if(amount < 0)
        {
            OrbCmd.RemoveSlots(player, -amount);
            return;
        }

        if(!_bypassCap)
        {
            await OrbCmd.AddSlots(player, amount);
            return;
        }

        // 绕过 10 上限：OrbQueue.AddCapacity 本身不夹上限，手动补加栏位动画。
        player.PlayerCombatState.OrbQueue.AddCapacity(amount);
        NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.OrbManager?.AddSlotAnim(amount);
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _slotsVar;
    }
}
