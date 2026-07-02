using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class StarSword : HilloCardModel
{
    public StarSword() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self,
        [new HilloPowerSelfStep<StarSwordPower>("StarSword"), new HilloAddKeywordUpgradeStep(CardKeyword.Innate)],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Energy);
    }
}
