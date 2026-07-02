using System.Collections.Generic;
using System.Linq;
using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using BaseLib.Abstracts;

namespace hillo.Modules.Model;

// 用 Step 组装能力（Power）：子类在构造函数里用 On<Hook>(...) 给每个 Hook 挂一组 Step。
// 每个 Hook 触发时，基类套用 owner/side 守卫、构造 PowerContext、顺序执行注册的 Step。
// 复用与卡牌相同的 HilloStep（power-safe 的那些：施加能力/生成充能球/栏位/召唤/得星得能量等）。
public abstract class HilloPowerModel : CustomPowerModel
{
    private enum Hook { PlayerTurnStart, SideTurnEnd, CardPlayed, CardDiscarded, OrbChanneled }

    public override sealed PowerType Type => _type;
    public override sealed PowerStackType StackType => _stackType;
    private readonly PowerType _type;
    private readonly PowerStackType _stackType;

    private readonly Dictionary<Hook, List<HilloStep>> _hooks = new();

    public HilloPowerModel() : this(PowerType.Buff, PowerStackType.Single) {}
    public HilloPowerModel(PowerType type, PowerStackType stackType)
    {
        _type = type;
        _stackType = stackType;
    }

    // ---- 子类构造函数里调用的注册方法 ----
    protected void OnPlayerTurnStart(params HilloStep[] steps) => Register(Hook.PlayerTurnStart, steps);
    protected void OnSideTurnEnd(params HilloStep[] steps) => Register(Hook.SideTurnEnd, steps);
    protected void OnCardPlayed(params HilloStep[] steps) => Register(Hook.CardPlayed, steps);
    protected void OnCardDiscarded(params HilloStep[] steps) => Register(Hook.CardDiscarded, steps);
    protected void OnOrbChanneled(params HilloStep[] steps) => Register(Hook.OrbChanneled, steps);

    private void Register(Hook hook, HilloStep[] steps)
    {
        if(!_hooks.TryGetValue(hook, out var list))
        {
            list = new List<HilloStep>();
            _hooks[hook] = list;
        }
        list.AddRange(steps);
    }

    private IEnumerable<HilloStep> Steps(Hook hook)
        => _hooks.TryGetValue(hook, out var list) ? list : Enumerable.Empty<HilloStep>();

    private IEnumerable<HilloStep> AllSteps => _hooks.Values.SelectMany(l => l);

    // 构造能力上下文并顺序执行某个 Hook 的 Step。
    private async Task RunHook(Hook hook, PlayerChoiceContext choiceContext, Player player, Creature? target = null, CardPlay? cardPlay = null)
    {
        var ctx = new PowerContext(player, DynamicVars, target, cardPlay);
        foreach(var step in Steps(hook))
            await step.OnStep(choiceContext, ctx);
    }

    // ---- 覆写游戏 Hook（都带 PlayerChoiceContext，Step 需要它）----
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player.Creature != Owner)
            return;
        await RunHook(Hook.PlayerTurnStart, choiceContext, player);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if(side != CombatSide.Player)
            return;
        if(Owner.Player is not { } player)
            return;
        await RunHook(Hook.SideTurnEnd, choiceContext, player);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(Owner.Player is not { } player)
            return;
        await RunHook(Hook.CardPlayed, choiceContext, player, cardPlay: cardPlay);
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if(Owner.Player is not { } player)
            return;
        await RunHook(Hook.CardDiscarded, choiceContext, player);
    }

    public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)
    {
        if(player.Creature != Owner)
            return;
        await RunHook(Hook.OrbChanneled, choiceContext, player);
    }

    // ---- 变量 / 提示聚合（与卡牌一致）----
    protected override IEnumerable<DynamicVar> CanonicalVars
        => AllSteps.SelectMany(s => s.GetDynamicVars());

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            foreach(var step in AllSteps)
            {
                step.HostVars = DynamicVars;
                try
                {
                    foreach(var t in step.GetIHoverTips())
                        yield return t;
                }
                finally
                {
                    step.HostVars = null;
                }
            }
        }
    }

    public override string? CustomPackedIconPath => $"res://hillo/images/powers/{this.GetType().Name}.png";
    public override string? CustomBigIconPath => $"res://hillo/images/powers/{this.GetType().Name}.png";
}
