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

// 回合开始时打出 1 张闪亮登场；回合结束时打出消耗堆中所有闪亮登场。
public class GrandEncorePower : HilloPowerModel
{
    private const string DramaticEntranceId = "DRAMATIC_ENTRANCE";

    public GrandEncorePower() : base(PowerType.Buff, PowerStackType.Single)
    {
        OnPlayerTurnStart(new PlayEntranceStep());
        OnSideTurnEnd(new ReplayExhaustedEntrancesStep());
    }

    private class PlayEntranceStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var player = ctx.Player;
            int times = (int)ctx.Amount;
            for(int i=0; i<times; i++)
            {
                if(player.Creature.CombatState is not { } combatState)
                    return;
                var card = combatState.CreateCard<DramaticEntrance>(player);
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }

        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            var card = ModelDb.Card<DramaticEntrance>();
            if(card != null)
                yield return HoverTipFactory.FromCard(card, upgrade: false);
        }
    }

    private class ReplayExhaustedEntrancesStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var player = ctx.Player;
            var exhaustPile = player.Piles.FirstOrDefault(p => p.Type == PileType.Exhaust);
            if(exhaustPile == null)
                return;
            int times = (int)ctx.Amount;
            for(int i=0; i<times; i++)
            {
                var shining = exhaustPile.Cards
                    .Where(c => string.Equals(c.Id.Entry, DramaticEntranceId, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach(var card in shining)
                    await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
    }
}
