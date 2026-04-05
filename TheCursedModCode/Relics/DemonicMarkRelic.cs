using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class DemonicMarkRelic : TheCursedModRelic
{
    private bool _isActivating;
    private int _triggersThisTurn;
    private int _lastTrackedRound = -1;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool ShowCounter => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override int DisplayAmount
    {
        get
        {
            if (_isActivating)
                return base.DynamicVars.Cards.IntValue;
            return _triggersThisTurn;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4)];

    private int Threshold => base.DynamicVars.Cards.IntValue;

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner)
        {
            _triggersThisTurn = 0;
            _lastTrackedRound = combatState.RoundNumber;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public async Task OnCircleTrigger(PlayerChoiceContext context)
    {
        var currentRound = base.Owner.Creature?.CombatState?.RoundNumber ?? -1;
        if (_lastTrackedRound != currentRound)
        {
            _triggersThisTurn = 0;
            _lastTrackedRound = currentRound;
            InvokeDisplayAmountChanged();
        }

        _triggersThisTurn++;
        InvokeDisplayAmountChanged();

        if (_triggersThisTurn % Threshold != 0) return;

        _triggersThisTurn = 0;
        InvokeDisplayAmountChanged();
        await TaskHelper.RunSafely(DoActivateVisuals());
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature!, 1, base.Owner.Creature, null);
    }

    private async Task DoActivateVisuals()
    {
        _isActivating = true;
        InvokeDisplayAmountChanged();
        Flash();
        await Cmd.Wait(1f);
        _isActivating = false;
        InvokeDisplayAmountChanged();
    }
}
