using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using TheCursedMod.TheCursedModCode.Character;
using TheCursedMod.TheCursedModCode.Extensions;

namespace TheCursedMod.TheCursedModCode.Potions;

[Pool(typeof(TheCursedModPotionPool))]
public abstract class TheCursedModPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".PotionImagePath();
}
