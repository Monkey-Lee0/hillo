using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class WindWalk : HilloCardModel
{
    public WindWalk() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None,
        [new HilloPowerSelfStep<WindWalkPower>("WindWalk"), new HilloAddKeywordUpgradeStep(CardKeyword.Innate)],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.FromPower<DexterityPower>();
    }
}
