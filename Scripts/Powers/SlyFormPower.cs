using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 每丢弃一张牌，获得 Amount 点格挡（block = 1 × 层数）。
public class AgilePower : HilloPowerModel
{
    public AgilePower() : base(PowerType.Buff, PowerStackType.Counter)
    {
        OnCardDiscarded(new HilloBlockSelfStep(1, scaleByAmount: true));
    }
}

// 每丢弃一张牌，若该牌未附加奇巧则为其附加奇巧。需要「被丢弃的那张牌」，故保持手写。
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
