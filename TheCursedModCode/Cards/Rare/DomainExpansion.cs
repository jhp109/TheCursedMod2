using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 영역 전개(Domain Expansion) - 손에 있는 모든 마법진의 효과를 조건을 무시하고 강제로 X번 발동시킵니다. 업보 5 X번.
/// 강화 시 X+1번 발동.
/// </summary>
public sealed class DomainExpansion() : TheCursedModCard(0, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.KarmaTurn2Power>("KarmaPower", 5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        int triggers = IsUpgraded ? xValue + 1 : xValue;

        var circles = PileType.Hand.GetPile(Owner).Cards.OfType<CircleCard>().ToList();
        for (var i = 0; i < triggers; i++)
            foreach (var circle in circles)
                await circle.ForceTrigger(choiceContext);

        await ApplyKarma(DynamicVars["KarmaPower"].IntValue * triggers);
    }

    protected override void OnUpgrade() { }
}
