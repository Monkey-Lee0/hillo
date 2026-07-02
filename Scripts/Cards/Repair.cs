using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

// 衍生牌：放入 TokenCardPool（不进奖励，仅由「故障」生成），图鉴里归到衍生牌区。
[Pool(typeof(TokenCardPool))]
public class Repair : HilloCardModel
{
    public Repair() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self,
        [
            new MultiHitBlockStep(2, times:4, timesUpgradeDiff:1),
            new HilloCreateCardStep<Glitch>(1, upgrade:true)
        ],
        keywords: [CardKeyword.Exhaust]) {}

    // 获得 Block 点格挡 Times 次；升级增加次数（而非每次格挡）。
    private class MultiHitBlockStep : HilloStep
    {
        private readonly BlockVar _blockVar;
        private readonly IntVar _timesVar;
        private readonly int _timesDiff;

        public MultiHitBlockStep(int block, int times, int timesUpgradeDiff=0)
        {
            _blockVar = new BlockVar(block, ValueProp.Move);
            _timesVar = new IntVar("Times", times);
            _timesDiff = timesUpgradeDiff;
        }

        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int times = (int)cardPlay.Card.DynamicVars["Times"].BaseValue;
            for(int i=0; i<times; i++)
                await CreatureCmd.GainBlock(
                    CurrentPlayer(cardPlay).Creature,
                    cardPlay.Card.DynamicVars.Block,
                    cardPlay
                );
        }

        public override void OnUpgrade(CardModel card)
        {
            if(_timesDiff == 0)
                return ;
            card.DynamicVars["Times"].UpgradeValueBy(_timesDiff);
        }
        public override IEnumerable<DynamicVar> GetDynamicVars()
        {
            yield return _blockVar;
            yield return _timesVar;
        }
    }
}
