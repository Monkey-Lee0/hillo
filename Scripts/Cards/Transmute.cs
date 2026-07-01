using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(SilentCardPool))]   // 猎手专属卡池
public class Transmute : HilloCardModel
{
    public Transmute() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None,
        [new HilloTransformCardStep<Shiv>("Transform", 0, "TRANSMUTE_SELECT", max:10, upgradeT:true)]) {}
}
