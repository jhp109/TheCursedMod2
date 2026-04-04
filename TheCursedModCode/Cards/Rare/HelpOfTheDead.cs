using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 망자의 도움(Help of the Dead) - 의례 : 뽑을 카드 더미에 있는 카드 1장에 재사용을 1 추가합니다.
/// (강화 시 비용 1)
/// </summary>
public sealed class HelpOfTheDead() : RiteCard(2, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    private static readonly LocString SelectPrompt = new("cards", "THECURSEDMOD-HELP_OF_THE_DEAD.selectionScreenPrompt");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
    ];

    protected override Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
        => Task.CompletedTask;

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = await CommonActions.SelectSingleCard(this, SelectPrompt, choiceContext, PileType.Draw);
        if (target != null)
            target.BaseReplayCount++;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
