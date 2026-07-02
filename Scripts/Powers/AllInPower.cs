using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Cards.Variables;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using hillo.Modules.Model;
using hillo.Modules.Step;


namespace hillo.Scripts.Power;      

public class AllInPower: HilloPowerModel
{
    public AllInPower(): base(PowerType.Debuff, PowerStackType.Counter)
    {
        OnPlayerTurnStart(
            new HilloLoseHpSelfStep(10, scaleByAmount: true),
            new HilloRemovePowerSelfStep<AllInPower>()
        );
    }
}