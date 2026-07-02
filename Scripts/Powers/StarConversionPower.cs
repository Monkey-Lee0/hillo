using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using System.Threading.Tasks;

namespace hillo.Scripts.Power;
public class StarsToEnergy : CustomPowerModel
{
    // 辉星计数（内部字段，不暴露给游戏）
    private int _starCount=0;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/Stars.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/Stars.png";


    public override async Task AfterStarsSpent(int amount, Player Spenter)
    {
        var owner = Owner;
        if (owner == null) return;

        if(Owner != Spenter.Creature) return; // 只关注玩家自己打出的牌（非自动打出？但需求未说明，默认包含）

        _starCount += amount;
        if(_starCount >= 3)
        {
            await PlayerCmd.GainEnergy(_starCount/3, Spenter);
            _starCount %= 3;
        }
        InvokeDisplayAmountChanged();
    }
    public override int DisplayAmount => _starCount;
}

public class EnergyToStars : CustomPowerModel
{
    // 辉星计数（内部字段，不暴露给游戏）
    private int _energyCost=0;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/Energy.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/Energy.png";


    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        var owner = Owner;
        if (owner == null) return;

        if(Owner != card.Owner.Creature) return; // 只关注玩家自己打出的牌（非自动打出？但需求未说明，默认包含）

        _energyCost += amount;
        if(_energyCost >= 3)
        {
            await PlayerCmd.GainStars(_energyCost/3, card.Owner);
            _energyCost %= 3;
        }
        InvokeDisplayAmountChanged();
    }
    public override int DisplayAmount => _energyCost;
}
