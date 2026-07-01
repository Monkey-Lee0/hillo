using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace hillo.Modules.Step;

public class HilloDrawCardStep : HilloStep
{
    protected IntVar _drawVar;
    protected string _name;
    protected int _draw;
    protected readonly int _diff;

    public HilloDrawCardStep(string name, int draw, int upgradeDiff=0)
    {
        _name = name;
        _draw = draw;
        _diff = upgradeDiff;
        _drawVar = new IntVar(name, draw);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(
            choiceContext,
            cardPlay.Card.DynamicVars[_name].BaseValue,
            CurrentPlayer(cardPlay)
        );
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _drawVar;
    }
}

// 将弃牌堆洗入抽牌堆
public class HilloShuffleDiscardStep : HilloStep
{
    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Shuffle(choiceContext, CurrentPlayer(cardPlay));
    }
}

public class HilloDiscardCardStep : HilloStep
{
    protected IntVar _discardVar;
    protected string _name;
    protected int _discard;
    protected readonly int _diff;
    protected string _locKey;

    public HilloDiscardCardStep(string name, int discard, string locKey, int upgradeDiff=0)
    {
        _name = name;
        _discard = discard;
        _locKey = locKey;
        _diff = upgradeDiff;
        _discardVar = new IntVar(name, discard);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        int count = (int)cardPlay.Card.DynamicVars[_name].BaseValue;

        var locString = new LocString("card_selection", _locKey);

        var prefs = new CardSelectorPrefs(locString, count);
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            filter: null,
            source: cardPlay.Card
        );

        if(selected == null || !selected.Any())
            return;

        foreach(var card in selected)
            await CardCmd.Discard(choiceContext, card);
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _discardVar;
    }
}

public class HilloExhaustCardStep : HilloStep
{
    protected IntVar _exhaustVar;
    protected string _name;
    protected int _exhaust;
    protected readonly int _diff;
    protected string _locKey;

    public HilloExhaustCardStep(string name, int exhaust, string locKey, int upgradeDiff=0)
    {
        _name = name;
        _exhaust = exhaust;
        _locKey = locKey;
        _diff = upgradeDiff;
        _exhaustVar = new IntVar(name, exhaust);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        int count = (int)cardPlay.Card.DynamicVars[_name].BaseValue;

        var locString = new LocString("card_selection", _locKey);

        var prefs = new CardSelectorPrefs(locString, count);
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            filter: null,
            source: cardPlay.Card
        );

        if(selected == null || !selected.Any())
            return;

        foreach(var card in selected)
            await CardCmd.Exhaust(choiceContext, card);
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _exhaustVar;
    }
}

public class HilloCreateCardStep<T> : HilloStep where T : CardModel, new()
{
    protected int _count;
    protected bool _upgrade;

    // upgrade:true 表示「母卡升级时，生成的牌也升级」（跟随母卡升级态），而非总是升级。
    public HilloCreateCardStep(int count=1, bool upgrade=false)
    {
        _count = count;
        _upgrade = upgrade;
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        if(player.Creature.CombatState is not { } combatState)
            return;

        bool upgrade = _upgrade && cardPlay.Card.IsUpgraded;
        for(int i=0; i<_count; i++)
        {
            var card = combatState.CreateCard<T>(player);
            if(upgrade && card.IsUpgradable)
            {
                card.UpgradeInternal();
                card.FinalizeUpgradeInternal();
            }
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        var model = ModelDb.Card<T>();
        if(model != null)
            yield return HoverTipFactory.FromCard(model, upgrade: _upgrade && (HostCard?.IsUpgraded ?? false));
    }
}

public class HilloTransformCardStep<T> : HilloStep where T : CardModel, new()
{
    protected IntVar _countVar;
    protected string _name;
    protected int _count;
    protected readonly int _diff;
    protected string _locKey;
    protected bool _upgradeT;
    protected int _max;

    public HilloTransformCardStep(string name, int count, string locKey, int max=-1, int upgradeDiff=0, bool upgradeT=false)
    {
        _name = name;
        _count = count;
        _locKey = locKey;
        _max = max;
        _diff = upgradeDiff;
        _upgradeT = upgradeT;
        _countVar = new IntVar(name, count);
    }

    public override async Task OnStep(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = CurrentPlayer(cardPlay);
        if(player.Creature.CombatState is not { } combatState)
            return;

        int count = (int)cardPlay.Card.DynamicVars[_name].BaseValue;

        var locString = new LocString("card_selection", _locKey);
        // max < 0 表示恰好 count 张，否则为 [count, max] 区间
        var prefs = _max < 0
            ? new CardSelectorPrefs(locString, count)
            : new CardSelectorPrefs(locString, count, _max);
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            filter: null,
            source: cardPlay.Card
        );

        if(selected == null || !selected.Any())
            return;

        bool upgrade = _upgradeT && cardPlay.Card.IsUpgraded;
        foreach(var original in selected)
        {
            var replacement = combatState.CreateCard<T>(player);
            if(upgrade && replacement.IsUpgradable)
            {
                replacement.UpgradeInternal();
                replacement.FinalizeUpgradeInternal();
            }
            await CardCmd.Transform(original, replacement);
        }
    }

    public override void OnUpgrade(CardModel card)
    {
        if(_diff == 0)
            return ;
        card.DynamicVars[_name].UpgradeValueBy(_diff);
    }
    public override IEnumerable<DynamicVar> GetDynamicVars()
    {
        yield return _countVar;
    }
    public override IEnumerable<IHoverTip> GetIHoverTips()
    {
        var model = ModelDb.Card<T>();
        if(model != null)
            yield return HoverTipFactory.FromCard(model, upgrade: _upgradeT);
    }
}
