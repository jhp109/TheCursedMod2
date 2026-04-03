using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class SoulVesselRelic : TheCursedModRelic
{
    private int _exhaustedCurseCount;

    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool ShowCounter => true;

    public override int DisplayAmount => _exhaustedCurseCount;

    public override Task BeforeCombatStart()
    {
        _exhaustedCurseCount = 0;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner == base.Owner && card.Type == CardType.Curse)
        {
            _exhaustedCurseCount = Math.Min(_exhaustedCurseCount + 2, 10);
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (_exhaustedCurseCount <= 0) return;

        Flash();
        await CreatureCmd.Heal(base.Owner.Creature, _exhaustedCurseCount);
    }
}
