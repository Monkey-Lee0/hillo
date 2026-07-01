using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class Malice : HilloCardModel
{
    public Malice() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self,
        [new HilloPowerSelfStep<MalicePower>("Malice"), new HilloEnergyUpgradeStep(-1)],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.FromPower<WeakPower>();
        yield return HoverTipFactory.FromPower<VulnerablePower>();
        yield return HoverTipFactory.FromPower<StrengthPower>();
    }
}
