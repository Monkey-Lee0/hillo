using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

// 把一组内层 step 包装成「对每个玩家依次执行一遍」的单个 step。
// 通过 ctx.WithPlayer(player) 把内层 step 重定向到当前遍历到的玩家。
// 不直接实例化；调用 HilloStep.AllPlayers(...) 或 step.ForAllPlayers() 即可。
public class HilloAllPlayerStep : HilloStep
{
    private readonly HilloStep[] _inner;

    public HilloAllPlayerStep(params HilloStep[] inner)
    {
        _inner = inner;
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var players = ctx.Player.RunState?.Players;
        if(players == null)
            return;

        foreach(var player in players)
        {
            if(player.Creature == null)
                continue;
            var sub = ctx.WithPlayer(player);
            foreach(var step in _inner)
                await step.OnStep(choiceContext, sub);
        }
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
            step.HostVars = HostVars;
            step.HostCard = HostCard;
            try
            {
                foreach(var t in step.GetIHoverTips())
                    yield return t;
            }
            finally
            {
                step.HostVars = null;
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
// 玩家通过 selector(ctx) 在运行时解析（出牌时才知道有谁在场）。
public class HilloOtherPlayerStep : HilloStep
{
    private readonly Func<HilloContext, Player?> _selector;
    private readonly HilloStep[] _inner;

    public HilloOtherPlayerStep(Func<HilloContext, Player?> selector, params HilloStep[] inner)
    {
        _selector = selector;
        _inner = inner;
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        var target = _selector(ctx);
        if(target == null || target.Creature == null)
            return;

        var sub = ctx.WithPlayer(target);
        foreach(var step in _inner)
            await step.OnStep(choiceContext, sub);
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
            step.HostVars = HostVars;
            step.HostCard = HostCard;
            try
            {
                foreach(var t in step.GetIHoverTips())
                    yield return t;
            }
            finally
            {
                step.HostVars = null;
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
