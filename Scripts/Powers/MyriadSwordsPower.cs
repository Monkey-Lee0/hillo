using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Power;

// 回合结束时，无论何处（手牌/抽牌堆/弃牌堆/消耗堆），对随机敌人打出你所有的君王之剑。
public class MyriadSwordsPower : HilloPowerModel
{
    public MyriadSwordsPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        OnSideTurnEnd(new PlayAllBladesStep());
    }

    private class PlayAllBladesStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var player = ctx.Player;
            if(ctx.Owner.CombatState is not { } combatState)
                return;

            var blades = PileType.Hand.GetPile(player).Cards
                .Concat(PileType.Draw.GetPile(player).Cards)
                .Concat(PileType.Discard.GetPile(player).Cards)
                .Concat(PileType.Exhaust.GetPile(player).Cards)
                .Where(c => c is SovereignBlade)
                .ToList();
            if(!blades.Any())
                return;

            var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
            if(!enemies.Any())
                return;

            var rng = new Random();
            foreach(var card in blades)
            {
                var target = enemies[rng.Next(enemies.Count)];
                await CardCmd.AutoPlay(choiceContext, card, target);
            }
        }

        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            var card = ModelDb.Card<SovereignBlade>();
            if(card != null)
                yield return HoverTipFactory.FromCard(card, upgrade: false);
        }
    }
}
