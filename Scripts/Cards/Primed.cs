using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class Primed : HilloCardModel
{
    public Primed() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies,
        [
            new HilloPowerAllStep<WeakPower>("Weak", 1, upgradeDiff:1, needTips:true),
            new HilloPowerSelfStep<EnergyNextTurnPower>("EnergyNext", 1),
            new HilloPowerSelfStep<DrawCardsNextTurnPower>("DrawNext", 1)
        ],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Energy);
    }
}
