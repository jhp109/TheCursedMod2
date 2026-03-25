using BaseLib.Abstracts;
using TheCursedMod.TheCursedModCode.Extensions;
using TheCursedMod.TheCursedModCode.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace TheCursedMod.TheCursedModCode.Character;

public class TheCursedMod : PlaceholderCharacterModel
{
    public const string CharacterId = "CharMod";
    
    public static readonly Color Color = new("5757d4");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeTheCursed>(),
        ModelDb.Card<StrikeTheCursed>(),
        ModelDb.Card<StrikeTheCursed>(),
        ModelDb.Card<StrikeTheCursed>(),
        ModelDb.Card<DefendTheCursed>(),
        ModelDb.Card<DefendTheCursed>(),
        ModelDb.Card<DefendTheCursed>(),
        ModelDb.Card<DefendTheCursed>(),
        ModelDb.Card<CursedWand>(),
        ModelDb.Card<CleanUpWorkshop>(),
        ModelDb.Card<Dregs>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BurningBlood>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<TheCursedModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<TheCursedModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<TheCursedModPotionPool>();
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override string CustomCharacterSelectBg => "char_select_bg_the_cursed.tscn".ScenePath();
    public override string CustomIconTexturePath => "character_icon_the_cursed.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_the_cursed.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_the_cursed_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_the_cursed.png".CharacterUiPath();
}