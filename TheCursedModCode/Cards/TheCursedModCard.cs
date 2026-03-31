using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using TheCursedMod.TheCursedModCode.Character;
using TheCursedMod.TheCursedModCode.Extensions;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

[Pool(typeof(TheCursedModCardPool))]
public abstract class TheCursedModCard(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : CustomCardModel(cost, type, rarity, target, showInCardLibrary, autoAdd)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants: fullart 250x350, normalart 250x190
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>
    /// GracePower가 활성화된 경우 KarmaTurn3Power를, 그렇지 않으면 KarmaTurn2Power를 적용합니다.
    /// </summary>
    protected Task ApplyKarma(decimal amount)
    {
        if (Owner.Creature.HasPower<GracePower>()) {
            Owner.Creature.GetPower<GracePower>()!.TriggerFlash();
            return PowerCmd.Apply<KarmaTurn3Power>(Owner.Creature, amount, Owner.Creature, this);
        }
        return PowerCmd.Apply<KarmaTurn2Power>(Owner.Creature, amount, Owner.Creature, this);
    }
}
