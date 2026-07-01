using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;

namespace hillo.Scripts.Power;

public class UmbralIncursionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/UmbralIncursionPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/UmbralIncursionPower.png";

    // 每回合开始：获得 1 个充能球栏位（OrbCmd.AddSlots 自带 10 上限夹取），生成 1 个黑暗充能球。
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player.Creature != Owner)
            return;

        await OrbCmd.AddSlots(player, 1);
        await OrbCmd.Channel<DarkOrb>(choiceContext, player);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.Static(StaticHoverTip.Channeling);
            yield return HoverTipFactory.FromOrb<DarkOrb>();
        }
    }
}
