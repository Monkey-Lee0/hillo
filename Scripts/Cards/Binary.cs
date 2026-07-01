using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using hillo.Scripts.Power;
using BaseLib.Utils;
using hillo.Modules.Card;
using hillo.Modules.Step;

namespace hillo.Scripts.Cards;
[Pool(typeof(DefectCardPool))]
public class Binary : HilloCardModel
{
    public Binary() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, 
    [new HilloEnergyUpgradeStep(-1), new HilloPowerSelfStep<BinaryPower>("Binary")]) {}
}