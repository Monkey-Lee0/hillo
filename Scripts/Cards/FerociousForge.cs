using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class FerociousForge : HilloCardModel
{
    public FerociousForge() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy,
        [
            new HilloDamageSingleStep(10, upgradeDiff:2),
            new HilloForgeSelfStep("Forge", 9, upgradeDiff:2),
            new HilloCreateCardStep<Debris>(1)
        ],
        tags: new HashSet<CardTag> { CardTag.Strike }) {}
}
