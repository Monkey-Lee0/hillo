using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class SlyForm : HilloCardModel
{
    public SlyForm() : base(3, CardType.Power, CardRarity.Ancient, TargetType.None,
        [
            new HilloPowerSelfStep<AgilePower>("Agile", 4, upgradeDiff:2),
            new HilloPowerSelfStep<SlyFormPower>("SlyForm")
        ],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.FromKeyword(CardKeyword.Sly);
    }
}
