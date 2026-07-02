using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Combat;
using BaseLib.Abstracts;
using BaseLib.Utils;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Relics;

namespace hillo.Scripts.Power;

public class GrandEncorePower : CustomPowerModel
{
    private const string DramaticEntranceId = "DRAMATIC_ENTRANCE";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/GrandEncorePower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/GrandEncorePower.png";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        int times = (int)Amount;
        for(int i=0; i<times; i++)
        {
            if (player.Creature != Owner) return;

            var combatState = player.Creature.CombatState;
            if (combatState == null) return;

            var card = combatState.CreateCard<DramaticEntrance>(player);

            await CardCmd.AutoPlay(choiceContext, card, null);
        }

    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, System.Collections.Generic.IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) return;

        var owner = Owner;
        if (owner == null) return;

        var player = owner.Player;
        if (player == null) return;

        var combatState = owner.CombatState;
        if (combatState == null) return;

        int times = (int)Amount;

        var exhaustPile = player.Piles.FirstOrDefault(p => p.Type == PileType.Exhaust);
        if (exhaustPile != null)
        {
            for(int i=0; i<times; i++)
            {
                var shiningCards = exhaustPile.Cards
                    .Where(c => string.Equals(c.Id.Entry, DramaticEntranceId, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach (var card in shiningCards)
                {
                    await CardCmd.AutoPlay(choiceContext, card, null);
                }
            }
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var card = ModelDb.Card<DramaticEntrance>();
            if (card != null)
                yield return HoverTipFactory.FromCard(card, upgrade: false);
        }
    }
}
