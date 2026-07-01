using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloStep
{
    // 当包装器（如 HilloAllPlayerStep）需要把内层 step 重定向到某个玩家时，
    // 在调用 OnStep 之前 set，调用结束后 clear。null 表示按默认 cardPlay.Card.Owner 取。
    public Player? PlayerOverride { get; set; }

    // 由 HilloCardModel 在调用 GetIHoverTips 前后 set/clear，让 step 的 hovertip 能查询
    // 自身所属卡的当前状态（例如 IsUpgraded）。包装器 step 需向内层 step 转发。
    public CardModel? HostCard { get; set; }

    // step 内部统一通过此方法取「当前作用玩家」，便于被外层包装器重定向。
    protected Player CurrentPlayer(CardPlay cardPlay)
        => PlayerOverride ?? cardPlay.Card.Owner;

    public virtual Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
    public virtual void OnUpgrade(CardModel card)
    {
        return ;
    }
    public virtual IEnumerable<DynamicVar> GetDynamicVars()
    {
        return [];
    }
    public virtual IEnumerable<IHoverTip> GetIHoverTips()
    {
        return [];
    }

    // 把一组 step 包装成「对每个玩家依次执行一遍」的单个 step。
    // 详见 MultiplayerStep.cs 中的 HilloAllPlayerStep。
    public static HilloStep AllPlayers(params HilloStep[] inner)
        => new HilloAllPlayerStep(inner);

    // 把一组 step 包装成「对 selector 选中的另一个玩家执行一遍」的单个 step。
    // selector 在出牌时取 CardPlay 决定目标 Player（运行时解析）。
    public static HilloStep OtherPlayer(Func<CardPlay, Player?> selector, params HilloStep[] inner)
        => new HilloOtherPlayerStep(selector, inner);

    // 语法糖：作用于 cardPlay.Target 所属的玩家（target 是友军生物时生效，敌人怪物时 skip）。
    public static HilloStep TargetPlayer(params HilloStep[] inner)
        => new HilloOtherPlayerStep(cp => cp.Target?.Player, inner);

    // 把一组 step 包装成「仅当 predicate 成立时执行」的单个 step（详见 ConditionStep.cs）。
    public static HilloStep When(Func<CardPlay, bool> predicate, params HilloStep[] inner)
        => new HilloConditionalStep(predicate, inner);
}

// 语法糖：单步 .ForAllPlayers()
public static class HilloStepExtensions
{
    public static HilloStep ForAllPlayers(this HilloStep step)
        => HilloStep.AllPlayers(step);
}
