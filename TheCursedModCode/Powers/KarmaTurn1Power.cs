using System.Linq;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
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
    private sealed class KarmaState
    {
        public int KarmaDamageRound = -1;
    }

    private static readonly ConditionalWeakTable<CombatState, Dictionary<Player, KarmaState>> _stateTable = new();

    private static KarmaState GetState(CombatState combat, Player player)
    {
        var dict = _stateTable.GetOrCreateValue(combat);
        if (!dict.TryGetValue(player, out var state))
        {
            state = new KarmaState();
            dict[player] = state;
        }
        return state;
    }

    /// <summary>
    /// 지난 턴에 해당 플레이어에게 업보 피해가 발생했는지 여부를 반환합니다.
    /// </summary>
    public static bool WasKarmaHitLastTurn(CombatState? combat, Player? player)
        => combat != null && player != null && GetState(combat, player).KarmaDamageRound == combat.RoundNumber - 1;

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
            base.Owner.GetPower<WorldlineTwistPower>()?.TriggerFlash();

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

    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target == Owner && dealer == Owner && result.UnblockedDamage > 0)
        {
            GetState(CombatState, Owner.Player!).KarmaDamageRound = CombatState.RoundNumber;
        }
        return Task.CompletedTask;
    }
}
