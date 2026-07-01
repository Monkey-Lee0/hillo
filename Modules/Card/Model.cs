using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;

namespace hillo.Modules.Card;


public abstract class HilloCardModel: CustomCardModel
{
    protected sealed override IEnumerable<DynamicVar> CanonicalVars
        => _steps.SelectMany(s => s.GetDynamicVars());
    public sealed override IEnumerable<CardKeyword> CanonicalKeywords => _keywords;
    protected sealed override HashSet<CardTag> CanonicalTags => _cardTags ?? base.CanonicalTags;
    // 把每个 step 的 HostCard 在迭代它的 hovertip 时 set 成 this（让 step 能查到自己所属卡的
    // IsUpgraded 等状态），结束后清空。
    protected sealed override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach(var t in _tips)
                yield return t;
            foreach(var step in _steps)
            {
                step.HostCard = this;
                try
                {
                    foreach(var t in step.GetIHoverTips())
                        yield return t;
                }
                finally
                {
                    step.HostCard = null;
                }
            }
        }
    }
    private IEnumerable<HilloStep> _steps;
    private IEnumerable<CardKeyword> _keywords;
    private IEnumerable<IHoverTip> _tips;
    private HashSet<CardTag> _cardTags;
    public HilloCardModel() : this(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, Enumerable.Empty<HilloStep>()) {}
    public HilloCardModel(int baseCost, CardType type, CardRarity rarity, TargetType target, IEnumerable<HilloStep> steps, IEnumerable<CardKeyword> keywords = null, IEnumerable<IHoverTip> extraHoverTips = null, bool autoAdd = true, IEnumerable<CardTag> tags = null)
        : base(baseCost, type, rarity, target, true, autoAdd)
    {
        _steps = steps;
        _keywords = keywords ?? Enumerable.Empty<CardKeyword>();
        _tips = extraHoverTips ?? Enumerable.Empty<IHoverTip>();
        _cardTags = tags != null ? new HashSet<CardTag>(tags) : null;
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var step in _steps)
        {
            await step.OnStep(choiceContext, cardPlay);
        }
    }
    protected override void OnUpgrade()
    {
        foreach(var step in _steps)
            step.OnUpgrade(this);
    }
    // 暴露受保护的星级消耗升级，供 step 调用
    public void UpgradeStarCost(int diff) => UpgradeStarCostBy(diff);
    public override string? PortraitPath => $"res://hillo/images/cards/{this.GetType().Name}.jpg";
}