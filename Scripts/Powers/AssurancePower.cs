using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using System;
using System.Linq;

namespace hillo.Scripts.Power;

public class AssurancePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/AssurancePower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/AssurancePower.png";
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner;
        if (owner.CombatState is not { } combatState) return;

        var Card = cardPlay.Card;
        if (Card.Type != CardType.Power)
            return ;
        
        if (Card.Keywords.Contains(CardKeyword.Sly))
            return ;
        
        Card.AddKeyword(CardKeyword.Sly);
        Card.EnergyCost.SetThisCombat(3);
        combatState.CloneCard(Card);

        await CardPileCmd.Add(Card, PileType.Discard);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromKeyword(CardKeyword.Sly);
        }
    }
}
