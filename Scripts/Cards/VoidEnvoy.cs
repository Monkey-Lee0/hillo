using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

using hillo.Modules.Step;
using hillo.Modules.Model;
using MegaCrit.Sts2.Core.Factories;

namespace hillo.Scripts.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class VoidEnvoy : HilloCardModel
{
    public VoidEnvoy() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self,
        [new HilloSummonStep(10, upgradeDiff:3), new AddRandomVoidCardStep()],
        keywords: [CardKeyword.Ethereal]) {}

    // 将一张随机（母卡升级则升级）牌放入手牌，附加虚无 + 消耗，且本回合免费打出。
    private class AddRandomVoidCardStep : HilloStep
    {
        public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
        {
            var player = ctx.Player;
            var pool = player.Character.CardPool
                .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                .ToList();
            if(pool.Count == 0)
                return;

            var card = CardFactory.GetForCombat(player, pool, 1, player.PlayerRng.Rewards).FirstOrDefault();
            if(card == null)
                return;

            if(ctx.IsUpgraded && card.IsUpgradable)
            {
                card.UpgradeInternal();
                card.FinalizeUpgradeInternal();
            }

            CardCmd.ApplyKeyword(card, CardKeyword.Ethereal, CardKeyword.Exhaust);
            card.EnergyCost.SetThisTurn(0);

            await CardPileCmd.AddGeneratedCardsToCombat([card], PileType.Hand, player);
        }
    }
}
