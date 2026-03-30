using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 돌려 막기 - 업보 피해를 받기 직전마다 방어도를 Amount 얻습니다.
/// KarmaTurn1Power가 활성화된 턴에 StartPulsing, 아니면 StopPulsing.
/// </summary>
public class RollingOverDebtPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player) return Task.CompletedTask;
        if (base.Owner.HasPower<KarmaTurn1Power>())
            StartPulsing();
        else
            StopPulsing();
        return Task.CompletedTask;
    }

    public async Task TriggerBlock()
    {
        Flash();
        await CreatureCmd.GainBlock(base.Owner, Amount, ValueProp.Move, null);
        await Cmd.Wait(0.5f);
    }
}
