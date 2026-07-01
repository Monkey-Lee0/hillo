using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class Revere : HilloCardModel
{
    public Revere() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.None,
        [new HilloGainStarStep(6), new HilloEnergyUpgradeStep(-1)]) {}
}
