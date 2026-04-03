using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class RitualistsRingRelic : TheCursedModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public async Task OnFirstRiteCurseThisTurn(PlayerChoiceContext choiceContext)
    {
        Flash();
        await PlayerCmd.GainEnergy(1, base.Owner);
    }
}
