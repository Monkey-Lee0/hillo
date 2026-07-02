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


namespace hillo.Scripts.Power;
    
public class AllInPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://hillo/images/powers/AllInPower.png";
    public override string? CustomBigIconPath => "res://hillo/images/powers/AllInPower.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HpLossVar(10m),
        new DisplayVar<CustomPowerModel>("Blood", (model) => (Amount * 10).ToString())
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        var owner = Owner;
        if (owner == null) return;

        await CreatureCmd.Damage(choiceContext, owner, DynamicVars.HpLoss.BaseValue * Amount, ValueProp.Unblockable, dealer: null, cardSource: null);

        await PowerCmd.Remove(this);
    }
}
