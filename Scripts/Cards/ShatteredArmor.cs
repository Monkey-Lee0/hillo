using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;
[Pool(typeof(ColorlessCardPool))]
public class ShatteredArmor : HilloCardModel
{
    public ShatteredArmor() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None,
        [new HilloBlockSelfStep(14, upgradeDiff:4), new HilloPowerSelfStep<NoBlockPower>("NoBlock")]) {}
}
