using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(DefectCardPool))]
public class UmbralIncursion : HilloCardModel
{
    public UmbralIncursion() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self,
        [new HilloPowerSelfStep<UmbralIncursionPower>("UmbralIncursion"), new HilloEnergyUpgradeStep(-1)],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Channeling);
        yield return HoverTipFactory.FromOrb<DarkOrb>();
    }
}
