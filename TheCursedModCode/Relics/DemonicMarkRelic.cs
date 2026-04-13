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
            if (IsActivating)
                return base.DynamicVars.Cards.IntValue;
            return TriggersThisTurn;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4)];

    private int Threshold => base.DynamicVars.Cards.IntValue;

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            UpdateDisplay();
        }
    }

    private int TriggersThisTurn
    {
        get => _triggersThisTurn;
        set
        {
            AssertMutable();
            _triggersThisTurn = value;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        base.Status = (!IsActivating && TriggersThisTurn % Threshold == Threshold - 1)
            ? RelicStatus.Active
            : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player == base.Owner)
        {
            TriggersThisTurn = 0;
            base.Status = RelicStatus.Normal;
        }
        return Task.CompletedTask;
    }

    public async Task OnCircleTrigger(PlayerChoiceContext context)
    {
        TriggersThisTurn++;

        if (TriggersThisTurn % Threshold != 0) return;

        TriggersThisTurn -= Threshold;
        _ = TaskHelper.RunSafely(DoActivateVisuals());
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature!, 1, base.Owner.Creature, null);
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}
