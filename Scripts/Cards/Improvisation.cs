using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;

namespace hillo.Scripts.Cards;

[Pool(typeof(SilentCardPool))]
public class Improvisation : HilloCardModel
{
    public Improvisation() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy,
        [new HilloDamageSingleStep(8, upgradeDiff:3), new ImproviseDebuffStep()]) {}

    // 攻击意图 -> 虚弱，否则 -> 易伤
    private class ImproviseDebuffStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if(cardPlay.Target.Monster.IntendsToAttack)
                await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, 1, cardPlay.Card.Owner.Creature, cardPlay.Card);
            else
                await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, 1, cardPlay.Card.Owner.Creature, cardPlay.Card);
        }
        public override IEnumerable<IHoverTip> GetIHoverTips()
        {
            yield return HoverTipFactory.FromPower<WeakPower>();
            yield return HoverTipFactory.FromPower<VulnerablePower>();
        }
    }
}
