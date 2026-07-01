using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(ColorlessCardPool))]
public class PerfectPlan : HilloCardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public PerfectPlan() : base(0, CardType.Skill, CardRarity.Rare, TargetType.None,
        [
            HilloStep.AllPlayers(
                new HilloDrawCardStep("Draw", 1, upgradeDiff:1),
                new HilloGainEnergyStep(1),
                new HilloPowerSelfStep<VigorPower>("Vigor", 3, upgradeDiff:2, needTips:true),
                new HilloPowerSelfStep<BufferPower>("Buffer", 1, needTips:true)
            )
        ],
        keywords: [CardKeyword.Exhaust, CardKeyword.Innate]) {}
}
