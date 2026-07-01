using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using BaseLib.Utils;

using hillo.Scripts.Power;
using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(DefectCardPool))]
public class LightningTime : HilloCardModel
{
    public LightningTime() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self,
        [new HilloPowerSelfStep<LightningTimePower>("LightningTime"), new HilloEnergyUpgradeStep(-1)],
        extraHoverTips: Tips()) {}

    // 用迭代器方法而非数组字面量：每次访问 ExtraHoverTips 重新调用 Factory，
    // 避免在卡牌库等场景里命中已 dispose 的 Godot.CompressedTexture2D。
    private static IEnumerable<IHoverTip> Tips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Channeling);
        yield return HoverTipFactory.FromOrb<LightningOrb>();
    }
}
