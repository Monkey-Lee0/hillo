using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class GraveCaller : HilloCardModel
{
    public GraveCaller() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy,
        [
            new HilloSummonStep(3, upgradeDiff:2),
            new HilloPowerSingleStep<DoomPower>("Doom", 7, upgradeDiff:2, needTips:true)
        ],
        keywords: [CardKeyword.Ethereal]) {}
}
