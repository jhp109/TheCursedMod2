using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 돌려 막기 - 업보 피해를 받기 직전마다 방어도를 Amount 얻습니다.
/// (KarmaTurn1Power.BeforeTurnEnd에서 피해 직전에 호출됨)
/// </summary>
public class RollingOverDebtPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public Task TriggerBlock()
    {
        Flash();
        return CreatureCmd.GainBlock(base.Owner, Amount, ValueProp.Move, null);
    }
}
