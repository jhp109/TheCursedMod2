using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 마력 집중(Focus Magic) - 의례 : 손에 있는 카드 1장에 재사용을 1 추가합니다.
/// (강화 시 비용 2)
/// </summary>
public sealed class FocusMagic() : RiteCard(3, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
    ];

    protected override Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
        => Task.CompletedTask;

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var card in await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this))
        {
            card.BaseReplayCount++;
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
