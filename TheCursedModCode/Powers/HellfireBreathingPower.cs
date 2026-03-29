using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Powers;

public class HellfireBreathingPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable)
    ];

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        card.CanPlay(out var reason, out _);
        if (card.Owner != null
            && card.Owner.Creature == Owner
            && reason.HasFlag(UnplayableReason.HasUnplayableKeyword))
        {
            Flash();
            foreach (var enemy in base.CombatState.HittableEnemies)
            {
                var vfx = NGroundFireVfx.Create(enemy);
                if (vfx != null)
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
            }
            SfxCmd.Play("event:/sfx/characters/attack_fire");
            await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies, Amount, ValueProp.Unpowered, base.Owner, null);
        }
    }
}
