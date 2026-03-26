using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheCursedMod.TheCursedModCode.Extensions;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 찌꺼기(Dregs) - 사용불가 저주 카드.
/// 의례 카드의 재료로 사용되는 저주 카드.
/// CurseCardPool에 추가하여 보라색 모드 프레임 대신 기본 저주 카드 프레임(검은색)을 사용.
/// </summary>
[Pool(typeof(CurseCardPool))]
public sealed class Dregs() : CustomCardModel(-1, CardType.Curse, CardRarity.Curse, TargetType.None, showInCardLibrary: false)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    protected override bool IsPlayable => false;
}
