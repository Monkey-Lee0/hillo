using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class Alchemy : HilloCardModel
{
    public Alchemy() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None,
        [
            new HilloExhaustCardStep("Exhaust", 1, "ALCHEMY_SELECT"),
            new HilloPowerRandomStep<PoisonPower>("Poison", 5, upgradeDiff:2, needTips:true)
        ]) {}
}
