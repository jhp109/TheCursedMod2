using MegaCrit.Sts2.Core.Entities.Powers;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 타락의 굴레 - 카드 사용으로 업보를 얻을 때마다 카드를 1장 뽑습니다.
/// (드로우 로직은 TheCursedModCard.ApplyKarma에서 처리)
/// </summary>
public class CycleOfDepravityPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
