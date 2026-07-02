using hillo.Modules.Step;
using hillo.Modules.Model;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

// 升级时调整卡牌的星级消耗
public class HilloStarUpgradeStep: HilloStep
{
    private readonly int _diff;

    public HilloStarUpgradeStep(int diff)
    {
        _diff = diff;
    }
    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        if(card is HilloCardModel hillo)
            hillo.UpgradeStarCost(_diff);
    }
}

// 获得星星，可升级，使用 StarsVar
public class HilloGainStarStep: HilloStep
{
    protected StarsVar _starsVar;
    protected int _stars;
    protected readonly int _diff;

    public HilloGainStarStep(int stars, int upgradeDiff=0)
    {
        _stars = stars;
        _diff = upgradeDiff;
        _starsVar = new StarsVar(stars);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, HilloContext ctx)
    {
        await PlayerCmd.GainStars(
            ctx.Vars.Stars.BaseValue,
            ctx.Player
        );
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.Stars.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _starsVar;
    }
}
