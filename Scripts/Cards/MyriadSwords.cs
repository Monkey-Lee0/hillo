using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class MyriadSwords : HilloCardModel
{
    public MyriadSwords() : base(2, CardType.Power, CardRarity.Ancient, TargetType.None,
        [
            new HilloForgeSelfStep("Forge", 15),
            new HilloPowerSelfStep<MyriadSwordsPower>("MyriadSwords"),
            new HilloEnergyUpgradeStep(-1)
        ]) {}
}
