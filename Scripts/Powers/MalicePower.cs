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

public class MalicePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/MalicePower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/MalicePower.png";
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = Owner;
        if (owner.CombatState is not { } combatState) return;

        var enemies = combatState.Enemies.Where(e => e.IsAlive).ToList();
        if (!enemies.Any()) return;

        int times = (int)Amount;
        var rng = new Random();
        if (cardPlay.Card is Cards.Malice)
            times = Math.Max(0, times - 1);

        for (int i = 0; i < times; i++)
        {
            var target = enemies[rng.Next(enemies.Count)];
            int effect = rng.Next(3);

            switch (effect)
            {
                case 0:
                    await PowerCmd.Apply<VulnerablePower>(
                        choiceContext,
                        target,
                        1,
                        applier: owner,
                        cardSource: cardPlay.Card
                    );
                    break;
                case 1:
                    await PowerCmd.Apply<WeakPower>(
                        choiceContext,
                        target,
                        1,
                        applier: owner,
                        cardSource: cardPlay.Card
                    );
                    break;
                case 2:
                    await PowerCmd.Apply<StrengthPower>(
                        choiceContext,
                        target,
                        -1,
                        applier: owner,
                        cardSource: cardPlay.Card
                    );
                    break;
            }
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            yield return HoverTipFactory.FromPower<WeakPower>();
            yield return HoverTipFactory.FromPower<VulnerablePower>();
            yield return HoverTipFactory.FromPower<StrengthPower>();
        }
    }
}
