using MegaCrit.Sts2.Core.Entities.Cards;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 찌꺼기(Dregs) - 사용불가 저주 카드.
/// 의례 카드의 재료로 사용되는 저주 카드.
/// </summary>
public sealed class Dregs() : TheCursedModCard(-1, CardType.Curse, CardRarity.Curse, TargetType.None, false, false)
{
    // 사용불가 카드. OnPlay 없음.
}
