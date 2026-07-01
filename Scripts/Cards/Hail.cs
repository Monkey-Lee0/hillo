using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(DefectCardPool))]
public class Hail : HilloCardModel
{
    public Hail() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies,
        [new HilloChannelOrbStep<FrostOrb>("Freeze", 1), new FrostScaledDamageStep(2, upgradeDiff:1)]) {}

    // 统计本场战斗已生成的冰霜充能球数
    private static int FrostChanneledThisCombat()
        => CombatManager.Instance?.History?.Entries
            .OfType<OrbChanneledEntry>()
            .Count(e => e.Orb is FrostOrb) ?? 0;

    // 卡面伤害数：预览时 = (本场冰球数 + 1) × 倍率，再由基类套用力量/易伤等。
    // +1 表示这张牌打出时会先生成的那个冰球。
    private class HailDamageVar : DamageVar
    {
        public HailDamageVar() : base("Damage", 0, ValueProp.Move) {}

        public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
        {
            int mult = (int)card.DynamicVars["Multiplier"].BaseValue;
            BaseValue = (FrostChanneledThisCombat() + 1) * mult;
            base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
        }
    }

    // 对所有敌人造成 (本场战斗生成的冰球数 × Multiplier) 点伤害。
    // 冰球生成步骤在前，因此计数已包含刚生成的这个。
    private class FrostScaledDamageStep : HilloStep
    {
        private readonly IntVar _multVar;
        private readonly HailDamageVar _damageVar;
        private readonly string _name = "Multiplier";
        private readonly int _diff;

        public FrostScaledDamageStep(int multiplier, int upgradeDiff=0)
        {
            _diff = upgradeDiff;
            _multVar = new IntVar(_name, multiplier);
            _damageVar = new HailDamageVar();
        }

        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            var owner = CurrentPlayer(cardPlay).Creature;
            if(owner.CombatState is not { } combatState)
                return;

            int mult = (int)cardPlay.Card.DynamicVars[_name].BaseValue;
            int damage = FrostChanneledThisCombat() * mult;
            if(damage <= 0)
                return;

            await DamageCmd.Attack(damage)
                .FromCard(cardPlay.Card)
                .TargetingAllOpponents(combatState)
                .Execute(choiceContext);
        }

        public override void OnUpgrade(CardModel card)
        {
            if(_diff == 0)
                return ;
            card.DynamicVars[_name].UpgradeValueBy(_diff);
        }
        public override IEnumerable<DynamicVar> GetDynamicVars()
        {
            yield return _multVar;
            yield return _damageVar;
        }
    }
}
