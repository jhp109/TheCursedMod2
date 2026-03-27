using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace TheCursedMod.TheCursedModCode;

/// <summary>
/// 모드 전용 카드 키워드 등록.
/// [CustomEnum("Name")] → 로컬라이제이션 키 THECURSEDMOD-NAME 으로 자동 변환됨.
/// </summary>
public static class Keywords
{
    [CustomEnum("Rite")]
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Rite;

}
