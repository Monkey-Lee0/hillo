using MegaCrit.Sts2.Core.Entities.Cards;
using hillo.Scripts.Power;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;
[Pool(typeof(ColorlessCardPool))]
public class AllIn: HilloCardModel
{
    public AllIn():base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, 
        [new HilloDamageAllStep(18, upgradeDiff:3, times:3), new HilloPowerSelfStep<AllInPower>("AllInPower")]) {}
}