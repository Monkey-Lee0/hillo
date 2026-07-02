using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(RegentCardPool))]
public class StarExplode : HilloCardModel
{
    public override bool HasStarCostX => true;

    public StarExplode() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies,
        [new SquaredDamageStep()]) {}

    // 对所有敌人造成 (StarsSpent + 升级?1:0) 的平方点伤害
    private class SquaredDamageStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            int xValue = ctx.CardPlay!.Resources.StarsSpent;
            if(ctx.IsUpgraded)
                xValue++;

            var combatState = ctx.Owner.CombatState;
            if(combatState == null)
                return;

            await DamageCmd.Attack(xValue * xValue)
                .FromCard(ctx.Card)
                .TargetingAllOpponents(combatState)
                .Execute(choiceContext);
        }
    }
}
