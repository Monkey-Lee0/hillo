using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Orbs;          // 包含 Orb, LightningOrb
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;                  // 包含 Hook 事件
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Abstracts;
using BaseLib.Patches.UI;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Scripts.Power;

public class LightningTimePower : CustomPowerModel
{
    // 标志：防止递归生成
    private bool _isChannellingExtra = false;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/LightningTimePower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/LightningTimePower.png";
    public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if (_isChannellingExtra) return;

        if (player != Owner.Player) return;

        if (orb is not LightningOrb) return;

        int extraCount = (int)Amount;
        _isChannellingExtra = true;

        try
        {
            for (int i = 0; i < extraCount; i++)
                await OrbCmd.Channel<LightningOrb>(choiceContext, player);
        }
        finally
        {
            _isChannellingExtra = false;
        }
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {   
            yield return HoverTipFactory.Static(StaticHoverTip.Channeling);
            yield return HoverTipFactory.FromOrb<LightningOrb>();
        }
    }
}
