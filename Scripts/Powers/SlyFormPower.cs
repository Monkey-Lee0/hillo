using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace hillo.Scripts.Power;

public class AgilePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    // 可堆叠，每层获得 1 格挡
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string? CustomPackedIconPath => "res://hillo/images/powers/AgilePower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/AgilePower.png";

    // 每丢弃一张牌，获得 Amount 格挡
    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature != Owner) return;

        int blockAmount = (int)Amount; // Amount 表示层数
        await CreatureCmd.GainBlock(Owner, new BlockVar(blockAmount, ValueProp.Move), null);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips 
    {
        get
        {
            yield return HoverTipFactory.Static(StaticHoverTip.Block);
        }
    }
}
public class SlyFormPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single; 
    public override string? CustomPackedIconPath => "res://hillo/images/powers/SlyFormPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/SlyFormPower.png";

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner.Creature != Owner) return;

        if (card.Keywords.Contains(CardKeyword.Sly))
            return;

        card.AddKeyword(CardKeyword.Sly);
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips 
    {
        get
        {
            yield return HoverTipFactory.FromKeyword(CardKeyword.Sly);
        }
    }
}
