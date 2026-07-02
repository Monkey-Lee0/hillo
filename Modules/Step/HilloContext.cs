using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

// Step 的执行上下文抽象：把 Step 需要的东西从 CardPlay 解耦，
// 让同一套 Step 既能在卡牌 OnPlay（CardContext）也能在能力 Hook（PowerContext）里跑。
public abstract class HilloContext
{
    public abstract Player Player { get; }        // 作用玩家
    public Creature Owner => Player.Creature;      // 便捷：作用生物
    public abstract Creature? Target { get; }      // 单体目标；多数能力 Hook 为 null
    public abstract DynamicVarSet Vars { get; }    // 值来源：卡的或能力的 DynamicVars
    public abstract CardModel? Card { get; }       // FromCard/cardSource 用；能力里为 null
    public abstract CardPlay? CardPlay { get; }     // 传给 GainBlock/history；能力里为 null
    public abstract bool IsUpgraded { get; }
    // 宿主能力的层数（Amount），供需要按层缩放的 Step 用；卡牌上下文默认 1（缩放即无操作）。
    public virtual decimal Amount => 1m;
    // 多人包装器改写「作用玩家」用；返回一个 Player 被替换、其余不变的上下文。
    public abstract HilloContext WithPlayer(Player player);
}

// 卡牌出牌的上下文：包裹 CardPlay。
public class CardContext : HilloContext
{
    private readonly CardPlay _cardPlay;
    private readonly Player _player;

    public CardContext(CardPlay cardPlay) : this(cardPlay, cardPlay.Card.Owner) {}
    private CardContext(CardPlay cardPlay, Player player)
    {
        _cardPlay = cardPlay;
        _player = player;
    }

    public override Player Player => _player;
    public override Creature? Target => _cardPlay.Target;
    public override DynamicVarSet Vars => _cardPlay.Card.DynamicVars;
    public override CardModel? Card => _cardPlay.Card;
    public override CardPlay? CardPlay => _cardPlay;
    public override bool IsUpgraded => _cardPlay.Card.IsUpgraded;
    public override HilloContext WithPlayer(Player player) => new CardContext(_cardPlay, player);
}

// 能力 Hook 的上下文：包裹能力的 DynamicVars + 作用玩家（Hook 传入或 Owner.Player），
// 可选带上 target（部分 Hook 有相关目标）与 cardPlay（AfterCardPlayed）。
public class PowerContext : HilloContext
{
    private readonly Player _player;
    private readonly DynamicVarSet _vars;
    private readonly Creature? _target;
    private readonly CardPlay? _cardPlay;
    private readonly decimal _amount;

    public PowerContext(Player player, DynamicVarSet vars, Creature? target = null, CardPlay? cardPlay = null, decimal amount = 1m)
    {
        _player = player;
        _vars = vars;
        _target = target;
        _cardPlay = cardPlay;
        _amount = amount;
    }

    public override Player Player => _player;
    public override Creature? Target => _target;
    public override DynamicVarSet Vars => _vars;
    public override CardModel? Card => null;           // 能力没有源卡
    public override CardPlay? CardPlay => _cardPlay;
    public override bool IsUpgraded => false;          // 能力不升级（值已固化在 var/Amount）
    public override decimal Amount => _amount;
    public override HilloContext WithPlayer(Player player) => new PowerContext(player, _vars, _target, _cardPlay, _amount);
}
