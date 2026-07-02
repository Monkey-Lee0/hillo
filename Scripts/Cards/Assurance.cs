using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using hillo.Scripts.Power;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using hillo.Modules.Model;
using hillo.Modules.Step;

namespace hillo.Scripts.Cards;
[Pool(typeof(SilentCardPool))]
public class Assurance : HilloCardModel 
{
    public Assurance(): base(2, CardType.Power, CardRarity.Rare, TargetType.Self,
        [new HilloEnergyUpgradeStep(-1), new HilloPowerSelfStep<AssurancePower>("Assurance")], keywords:[CardKeyword.Sly]){}
}
