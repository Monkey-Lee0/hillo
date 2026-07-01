using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Card;

namespace hillo.Scripts.Cards;

[Pool(typeof(DefectCardPool))]
public class Glitch : HilloCardModel
{
    public Glitch() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy,
        [
            new MultiHitDamageStep(3, times:4, timesUpgradeDiff:1),
            new HilloCreateCardStep<Repair>(1, upgrade:true)
        ],
        keywords: [CardKeyword.Exhaust]) {}

    // 造成 Damage 点伤害 Times 次；升级增加次数（而非每次伤害）。
    private class MultiHitDamageStep : HilloStep
    {
        private readonly DamageVar _damageVar;
        private readonly IntVar _timesVar;
        private readonly int _timesDiff;

        public MultiHitDamageStep(int damage, int times, int timesUpgradeDiff=0)
        {
            _damageVar = new DamageVar(damage, ValueProp.Move);
            _timesVar = new IntVar("Times", times);
            _timesDiff = timesUpgradeDiff;
        }

        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int times = (int)cardPlay.Card.DynamicVars["Times"].BaseValue;
            for(int i=0; i<times; i++)
                await DamageCmd.Attack(cardPlay.Card.DynamicVars.Damage.BaseValue)
                    .FromCard(cardPlay.Card)
                    .Targeting(cardPlay.Target)
                    .Execute(choiceContext);
        }

        public override void OnUpgrade(CardModel card)
        {
            if(_timesDiff == 0)
                return ;
            card.DynamicVars["Times"].UpgradeValueBy(_timesDiff);
        }
        public override IEnumerable<DynamicVar> GetDynamicVars()
        {
            yield return _damageVar;
            yield return _timesVar;
        }
    }
}
