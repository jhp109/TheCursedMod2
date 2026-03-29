using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 업보 3턴 남음 - 다음 턴 종료 시 KarmaTurn2Power로 변환됩니다.
/// GracePower 활성화 중에 카드로 발생한 KarmaTurn2Power가 이 Power로 변환됩니다.
/// </summary>
public class KarmaTurn3Power : TheCursedModPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            int amount = Amount;
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<KarmaTurn2Power>(base.Owner, amount, base.Owner, null);
        }
    }
}
