using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class StarConversion : HilloCardModel
{
    public override int CanonicalStarCost => 2;

    public StarConversion() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self,
        [
            new HilloPowerSelfStep<StarsToEnergy>("StarsToEnergy"),
            new HilloPowerSelfStep<EnergyToStars>("EnergyToStars"),
            new HilloAddKeywordUpgradeStep(CardKeyword.Innate)
        ],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Energy);
    }
}
