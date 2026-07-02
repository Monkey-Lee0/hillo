using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloStep
{
    // 由宿主（HilloCardModel / HilloPowerModel）在生成 hovertip 前后注入，
    // 让 tip 能读到宿主当前的具名变量（如召唤数值随升级变化）。
    public DynamicVarSet? HostVars { get; set; }
    // 仅供需要「宿主卡是否升级」的 tip（如 HilloCreateCardStep）；能力宿主为 null。
    public CardModel? HostCard { get; set; }

    public virtual Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
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

    // 把一组 step 包装成「对每个玩家依次执行一遍」的单个 step（详见 MultiplayerStep.cs）。
    public static HilloStep AllPlayers(params HilloStep[] inner)
        => new HilloAllPlayerStep(inner);

    // 把一组 step 包装成「对 selector 选中的另一个玩家执行一遍」的单个 step。
    public static HilloStep OtherPlayer(Func<HilloContext, Player?> selector, params HilloStep[] inner)
        => new HilloOtherPlayerStep(selector, inner);

    // 语法糖：作用于 ctx.Target 所属的玩家（target 是友军生物时生效，敌人怪物时 skip）。
    public static HilloStep TargetPlayer(params HilloStep[] inner)
        => new HilloOtherPlayerStep(ctx => ctx.Target?.Player, inner);

    // 把一组 step 包装成「仅当 predicate 成立时执行」的单个 step（详见 ConditionStep.cs）。
    public static HilloStep When(Func<HilloContext, bool> predicate, params HilloStep[] inner)
        => new HilloConditionalStep(predicate, inner);
}

// 语法糖：单步 .ForAllPlayers()
public static class HilloStepExtensions
{
    public static HilloStep ForAllPlayers(this HilloStep step)
        => HilloStep.AllPlayers(step);
}
