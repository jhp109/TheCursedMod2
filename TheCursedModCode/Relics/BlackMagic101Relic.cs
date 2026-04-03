using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class BlackMagic101Relic : TheCursedModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != base.Owner)
        {
            return count;
        }

        bool hasCurseInDrawPile =
            PileType.Draw.GetPile(base.Owner).Cards.Any((CardModel c) => c.Type == CardType.Curse);
        if (!hasCurseInDrawPile)
        {
            return count;
        }

        Flash();
        return count + 1;
    }
}