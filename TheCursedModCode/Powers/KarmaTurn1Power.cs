using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 업보 1턴 남음 - 이번 턴 종료 시 Amount 피해를 받습니다.
/// KarmaTurn2Power가 한 턴 경과 시 이 Power로 변환됩니다.
/// </summary>
public class KarmaTurn1Power : TheCursedModPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        StartPulsing();
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            Flash();

            var vfx = NGroundFireVfx.Create(base.Owner);
            if (vfx != null) NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
            SfxCmd.Play("event:/sfx/characters/attack_fire");

            base.Owner.GetPower<IgnorePainPower>()?.TriggerFlash();
            await (base.Owner.GetPower<RollingOverDebtPower>()?.TriggerBlock() ?? Task.CompletedTask);

            var dieTogether = base.Owner.GetPower<DieTogetherPower>();
            if (dieTogether != null)
            {
                dieTogether.TriggerFlash();
                foreach (var enemy in base.CombatState.HittableEnemies)
                {
                    var enemyVfx = NGroundFireVfx.Create(enemy);
                    if (enemyVfx != null) NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(enemyVfx);
                }
                await CreatureCmd.Damage(choiceContext, base.CombatState.HittableEnemies.Append(base.Owner), Amount, ValueProp.Unpowered, base.Owner, null);
            }
            else
            {
                await CreatureCmd.Damage(choiceContext, base.Owner, Amount, ValueProp.Unpowered, base.Owner, null);
            }
            await PowerCmd.Remove(this);
        }
    }
}
