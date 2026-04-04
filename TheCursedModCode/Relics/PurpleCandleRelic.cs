using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class PurpleCandleRelic : TheCursedModRelic
{
    private bool _isActivating;

    private int _triggersAccumulated;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get
        {
            if (_isActivating)
                return base.DynamicVars.Cards.IntValue;
            return TriggersAccumulated;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(10)];

    [SavedProperty]
    public int TriggersAccumulated
    {
        get => _triggersAccumulated;
        private set
        {
            AssertMutable();
            if (_triggersAccumulated != value)
            {
                _triggersAccumulated = value;
                InvokeDisplayAmountChanged();
            }
        }
    }

    private int Threshold => base.DynamicVars.Cards.IntValue;

    public async Task OnCircleTrigger(PlayerChoiceContext context)
    {
        TriggersAccumulated++;
        if (TriggersAccumulated < Threshold) return;

        TriggersAccumulated -= Threshold;
        await TaskHelper.RunSafely(DoActivateVisuals());
        await CardPileCmd.Draw(context, 1, base.Owner);
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
