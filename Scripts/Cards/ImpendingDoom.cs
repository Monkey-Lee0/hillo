using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class ImpendingDoom : HilloCardModel
{
    public ImpendingDoom() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self,
        [new HilloPowerSelfStep<ImpendingDoomPower>("ImpendingDoom"), new HilloEnergyUpgradeStep(-1)],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.FromPower<DoomPower>();
    }
}
