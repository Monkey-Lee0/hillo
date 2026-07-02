using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class GrandEncore : HilloCardModel
{
    public GrandEncore() : base(3, CardType.Power, CardRarity.Ancient, TargetType.None,
        [new HilloPowerSelfStep<GrandEncorePower>("GrandEncore"), new HilloEnergyUpgradeStep(-1)],
        extraHoverTips: Tips()) {}

    private static IEnumerable<IHoverTip> Tips()
    {
        var card = ModelDb.Card<DramaticEntrance>();
        if(card != null)
            yield return HoverTipFactory.FromCard(card, upgrade: false);
    }
}
