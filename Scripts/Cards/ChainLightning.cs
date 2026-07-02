using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(DefectCardPool))]
public class ChainLightning : HilloCardModel
{
    public ChainLightning() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self,
        [
            new HilloChannelOrbStep<LightningOrb>("Channel", 3),
            new HilloPassiveAllStep<LightningOrb>("Passive", 1, upgradeDiff:1),
            new HilloEnergyUpgradeStep(-1)
        ]) {}
}
