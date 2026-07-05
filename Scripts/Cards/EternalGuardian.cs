using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Model;          

namespace hillo.Scripts.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class EternalGuardian : HilloCardModel
{
    public EternalGuardian() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self,
        [new HilloPowerSelfStep<EternalGuardianPower>("Guardian", 1, upgradeDiff:1)],
        keywords: [CardKeyword.Innate]) {}
}
