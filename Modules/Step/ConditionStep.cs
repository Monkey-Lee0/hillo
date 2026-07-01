using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

// 仅当 predicate(cardPlay) 成立时才执行内层 step；无论条件如何都转发内层的
// 动态变量 / hovertip / 升级，保证卡面正常显示。用 HilloStep.When(...) 构造。
public class HilloConditionalStep : HilloStep
{
    private readonly Func<CardPlay, bool> _predicate;
    private readonly HilloStep[] _inner;

    public HilloConditionalStep(Func<CardPlay, bool> predicate, params HilloStep[] inner)
    {
        _predicate = predicate;
        _inner = inner;
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(!_predicate(cardPlay))
            return;
        foreach(var step in _inner)
            await step.OnStep(choiceContext, cardPlay);
    }

    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        foreach(var step in _inner)
            foreach(var v in step.GetDynamicVars())
                yield return v;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        foreach(var step in _inner)
        {
            step.HostCard = HostCard;
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
    public override void OnUpgrade(CardModel card)
    {
        foreach(var step in _inner)
            step.OnUpgrade(card);
    }
}
