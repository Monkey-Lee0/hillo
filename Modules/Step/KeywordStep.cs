using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

// 升级时为卡牌添加关键字（如 Innate / Exhaust 等）
public class HilloAddKeywordUpgradeStep: HilloStep
{
    private readonly CardKeyword _keyword;

    public HilloAddKeywordUpgradeStep(CardKeyword keyword)
    {
        _keyword = keyword;
    }
    public override void OnUpgrade(CardModel card)
    {
        card.AddKeyword(_keyword);
    }
}
