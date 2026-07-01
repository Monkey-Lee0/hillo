using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

// 把一组内层 step 包装成「对每个玩家依次执行一遍」的单个 step。
// 通过基类 HilloStep.PlayerOverride 把内层 step 重定向到当前遍历到的玩家。
// 不直接实例化；调用 HilloStep.AllPlayers(...) 或 step.ForAllPlayers() 即可。
public class HilloAllPlayerStep : HilloStep
{
    private readonly HilloStep[] _inner;

    public HilloAllPlayerStep(params HilloStep[] inner)
    {
        _inner = inner;
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var players = cardPlay.Card.Owner.RunState?.Players;
        if(players == null)
            return;

        foreach(var player in players)
        {
            if(player.Creature == null)
                continue;
            foreach(var step in _inner)
            {
                step.PlayerOverride = player;
                try
                {
                    await step.OnStep(choiceContext, cardPlay);
                }
                finally
                {
                    step.PlayerOverride = null;
                }
            }
        }
    }

    // 把动态变量 / hovertip / 升级 都转发给内层 step，
    // HilloCardModel 装配时就能自动接管描述变量和 tip 显示。
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

// 把一组内层 step 包装成「对指定的另一个玩家执行一遍」的单个 step。
// 玩家通过 selector(cardPlay) 在运行时解析（出牌时才知道有谁在场）。
// 典型用法：HilloStep.OtherPlayer(cp => cp.Card.Owner.RunState.Players.First(p => p != cp.Card.Owner), ...)
public class HilloOtherPlayerStep : HilloStep
{
    private readonly Func<CardPlay, Player?> _selector;
    private readonly HilloStep[] _inner;

    public HilloOtherPlayerStep(Func<CardPlay, Player?> selector, params HilloStep[] inner)
    {
        _selector = selector;
        _inner = inner;
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = _selector(cardPlay);
        if(target == null || target.Creature == null)
            return;

        foreach(var step in _inner)
        {
            step.PlayerOverride = target;
            try
            {
                await step.OnStep(choiceContext, cardPlay);
            }
            finally
            {
                step.PlayerOverride = null;
            }
        }
    }

    // 同样把动态变量 / hovertip / 升级转发给内层 step。
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
