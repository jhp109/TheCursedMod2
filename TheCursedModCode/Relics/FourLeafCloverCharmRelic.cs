using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class FourLeafCloverCharmRelic : TheCursedModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
}
