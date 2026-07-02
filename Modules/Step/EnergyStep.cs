using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloEnergyUpgradeStep: HilloStep
{
    private readonly int _diff;

    public HilloEnergyUpgradeStep(int diff)
    {
        _diff = diff;
    }
    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.EnergyCost.UpgradeBy(_diff);
    }
}

public class HilloGainEnergyStep: HilloStep
{
    protected EnergyVar _energyVar;
    protected int _energy;
    protected readonly int _diff;

    public HilloGainEnergyStep(int energy, int upgradeDiff=0)
    {
        _energy = energy;
        _diff = upgradeDiff;
        _energyVar = new EnergyVar(energy);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(
            cardPlay.Card.DynamicVars.Energy.BaseValue,
            CurrentPlayer(cardPlay)
        );
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars.Energy.UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _energyVar;
    }
}
