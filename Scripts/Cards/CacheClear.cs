using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(DefectCardPool))]
public class CacheClear : HilloCardModel
{
    public CacheClear() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self,
        [
            new HilloBlockSelfStep(3, upgradeDiff:3),
            new HilloShuffleDiscardStep(),
            new HilloDrawCardStep("Draw", 1)
        ]) {}
}
