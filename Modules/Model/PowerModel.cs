using hillo.Modules.Step;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace hillo.Modules.Model;

public abstract class HilloPowerModel: CustomPowerModel
{
    public override sealed PowerType Type => _type;
    public override sealed PowerStackType StackType => _stackType;
    private PowerType _type;
    private PowerStackType _stackType;
    public HilloPowerModel(): base() {}
    public HilloPowerModel(PowerType type, PowerStackType stackType)
    {
        _type = type;
        _stackType = stackType;
    }
    public override string? CustomPackedIconPath => $"res://hillo/images/powers/{this.GetType().Name}.png";
    public override string? CustomBigIconPath => $"res://hillo/images/powers/{this.GetType().Name}.png";
}
