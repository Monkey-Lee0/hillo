using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Cards;
using BaseLib.Abstracts;
using BaseLib.Utils;
using System.Threading.Tasks;

namespace hillo.Scripts.Power;
public class StarSwordPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/StarSwordPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/StarSwordPower.png";
    
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!(power is StarSwordPower))
        {
            return Task.CompletedTask;
        }

        if (power.Owner != base.Owner)
        {
            return Task.CompletedTask;
        }

        IEnumerable<CardModel> enumerable = base.Owner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();
        foreach (CardModel item in enumerable)
            TryChangeCost(item, (int)amount);

        return Task.CompletedTask;
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card.IsClone)
        {
            return Task.CompletedTask;
        }

        TryChangeCost(card, base.Amount);
        return Task.CompletedTask;
    }

    
    private bool TryChangeCost(CardModel card, int amount)
    {
        if (card.Owner != base.Owner.Player)
        {
            return false;
        }

        if (!(card is SovereignBlade sovereignBlade))
        {
            return false;
        }

        sovereignBlade.EnergyCost.UpgradeBy(-amount);
        int CurrentCost = sovereignBlade.CurrentStarCost;
        if(CurrentCost < 0)
            CurrentCost = 0;
        sovereignBlade.SetStarCostThisCombat(CurrentCost + amount);

        return true;
    }
}
