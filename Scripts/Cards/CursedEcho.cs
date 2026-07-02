using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class CursedEcho : HilloCardModel
{
    public CursedEcho() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy,
        [new ConditionalOstyAttackStep(9, times:2, upgradeDiff:2)],
        tags: [CardTag.OstyAttack]) {}

    // 奥斯提对目标造成伤害 times 次；若目标带灾厄(Doom)，改为对所有敌人。
    private class ConditionalOstyAttackStep : HilloStep
    {
        private readonly OstyDamageVar _damageVar;
        private readonly int _diff;
        private readonly int _times;

        public ConditionalOstyAttackStep(int damage, int times, int upgradeDiff=0)
        {
            _damageVar = new OstyDamageVar(damage, ValueProp.Move);
            _times = times;
            _diff = upgradeDiff;
        }

        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var player = ctx.Player;
            if(Osty.CheckMissingWithAnim(player))
                return;
            if(player.Creature.CombatState is not { } combatState)
                return;

            bool doomed = ctx.Target?.HasPower<DoomPower>() ?? false;
            for(int i=0; i<_times; i++)
            {
                AttackCommand atk = DamageCmd.Attack(ctx.Vars.OstyDamage.BaseValue)
                    .FromOsty(player.Osty!, ctx.Card);
                atk = doomed
                    ? atk.TargetingAllOpponents(combatState)
                    : atk.Targeting(ctx.Target);
                await atk.WithAttackerAnim("attack_poke", 0.3f).Execute(choiceContext);
            }
        }

        public override void OnUpgrade(CardModel card)
        {
            if(_diff == 0)
                return ;
            card.DynamicVars.OstyDamage.UpgradeValueBy(_diff);
        }
        public override IEnumerable<DynamicVar> GetDynamicVars()
        {
            yield return _damageVar;
        }
        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            yield return HoverTipFactory.FromPower<DoomPower>();
        }
    }
}
