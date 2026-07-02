using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Threading.Tasks;

namespace hillo.Scripts.Power;

public class BinaryPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/BinaryPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/BinaryPower.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner != Owner.Player)
            return ;
        if(cardPlay.Resources.EnergySpent % 2 == 1)
            await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}
