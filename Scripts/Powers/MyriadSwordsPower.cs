using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Scripts.Power
{
    public class MyriadSwordsPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override string? CustomPackedIconPath => "res://hillo/images/powers/MyriadSwordsPower.png";
        public override string? CustomBigIconPath => "res://hillo/images/powers/MyriadSwordsPower.png";

        public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
        {
            var owner = Owner;
            if (owner == null) return;
    
            var combatState = owner.CombatState;
            if (combatState == null) return;

            if (combatState.CurrentSide != CombatSide.Player) return;

            var hand = PileType.Hand.GetPile(owner.Player);
            var draw = PileType.Draw.GetPile(owner.Player);
            var discard = PileType.Discard.GetPile(owner.Player);
            var exhaust = PileType.Exhaust.GetPile(owner.Player);

            var allCards = hand.Cards
                .Concat(draw.Cards)
                .Concat(discard.Cards)
                .Concat(exhaust.Cards)
                .ToList();

            var sovereignBlades = allCards.Where(c => c is SovereignBlade).ToList();
            if (!sovereignBlades.Any()) return;

            var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
            if (!enemies.Any()) return;

            var rng = new Random();

            foreach (var card in sovereignBlades)
            {
                var target = enemies[rng.Next(enemies.Count)];
                await CardCmd.AutoPlay(choiceContext, card, target);
            }
        }


        protected override IEnumerable<IHoverTip> ExtraHoverTips
        {
            get
            {
                var card = ModelDb.Card<SovereignBlade>();
                if (card != null)
                    yield return HoverTipFactory.FromCard(card, upgrade: false);
            }
        }
    }
}
