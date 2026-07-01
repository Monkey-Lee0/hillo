using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace hillo.Modules.Step;

public class HilloBlockSelfStep : HilloStep
{
    protected BlockVar _blockVar;
    protected int _block;
    protected readonly int _diff;
    protected int _times;

    public HilloBlockSelfStep(int block, int upgradeDiff=0, int times=1)
    {
        _block = block;
        _diff = upgradeDiff;
        _times = times;
        _blockVar = new BlockVar(block, ValueProp.Move);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for(int i=0; i<_times; i++)
            await CreatureCmd.GainBlock(
                CurrentPlayer(cardPlay).Creature,
                cardPlay.Card.DynamicVars.Block,
                cardPlay
            );
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.Block.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _blockVar;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        yield return HoverTipFactory.Static(StaticHoverTip.Block);
    }
}
