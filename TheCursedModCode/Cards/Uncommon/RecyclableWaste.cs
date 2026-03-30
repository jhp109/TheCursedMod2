using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 재활용 쓰레기(Recyclable Waste) - 이번 전투 동안 찌꺼기가 보존을 얻습니다. 찌꺼기를 2장 얻습니다.
/// (강화 시 선천성 추가)
/// </summary>
public sealed class RecyclableWaste() : TheCursedModCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Dregs>(false),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<RecyclableWastePower>(Owner!.Creature, 1m, Owner.Creature, this);

        await Dregs.CreateAndAddToHand(Owner!, 2);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
