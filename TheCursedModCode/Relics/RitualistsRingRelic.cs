using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class RitualistsRingRelic : TheCursedModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override bool ShowCounter => true;
    public override int DisplayAmount => TriggerCount;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
    ];

    private const int Threshold = 6;
    private int _triggerCount;

    [SavedProperty]
    public int TriggerCount
    {
        get => _triggerCount;
        private set
        {
            AssertMutable();
            if (_triggerCount != value)
            {
                _triggerCount = value;
                InvokeDisplayAmountChanged();
            }
        }
    }

    public async Task OnRiteEffectTriggered(PlayerChoiceContext choiceContext)
    {
        TriggerCount++;

        if (TriggerCount % Threshold != 0) return;
        TriggerCount = 0;

        Flash();
        await PlayerCmd.GainEnergy(1, base.Owner);
    }
}
