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
using MegaCrit.Sts2.Core.Models;
using hillo.Scripts.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace hillo.Scripts.Power;
public class WindWalkTemporaryDexterityPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override string? CustomPackedIconPath => "res://hillo/images/powers/WindWalkTemporaryDexterityPower.png";
        public override string? CustomBigIconPath => "res://hillo/images/powers/WindWalkTemporaryDexterityPower.png";

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, System.Collections.Generic.IEnumerable<Creature> participants)
        {
            if (side != CombatSide.Player) return;

            var owner = Owner;
            if (owner == null) return;

            var player = owner.Player;
            if (player == null) return;

            var combatState = owner.CombatState;
            if (combatState == null) return;

            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, -Amount, Owner, null);
            await PowerCmd.Remove(this);
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips
        {
            get
            {
                yield return HoverTipFactory.FromCard<WindWalk>();
            }
        }
    }

public class WindWalkPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/WindWalkPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/WindWalkPower.png";
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner;
        if (owner.CombatState is not { } combatState) return;

        var card = cardPlay.Card;
        if(card.Type != CardType.Attack)
            return ;

        await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            owner,
            Amount,
            applier: owner,
            cardSource: cardPlay.Card
        );
    
        await PowerCmd.Apply<WindWalkTemporaryDexterityPower>(
            choiceContext,
            owner,
            Amount,
            applier: owner,
            cardSource: cardPlay.Card
        );
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<DexterityPower>();
        }
    }
}
