using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Flourish : HilloCardModel
{
    public Flourish() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self,
        [HilloStep.When(OstyFullHp,
            new HilloSummonStep(3, upgradeDiff:1),
            new HilloBlockSelfStep(3, upgradeDiff:1),
            new HilloDrawCardStep("Draw", 1)
        )]) {}

    // 奥斯提血量等于血量上限
    private static bool OstyFullHp(CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        return player.IsOstyAlive && player.Osty!.CurrentHp >= player.Osty!.MaxHp;
    }
}
