using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 금단의 유물(Forbidden Relic) - 유물 수의 2배만큼 피해를 줍니다. 유물 수 만큼 업보. (강화 시 3배)
/// </summary>
public sealed class ForbiddenRelic() : TheCursedModCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IKarmaAttack
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new ExtraDamageVar(2),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(static (card, _) => card.Owner?.Relics.Count ?? 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        int relicCount = Owner.Relics.Count;
        if (relicCount > 0)
            await ApplyKarma(relicCount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ExtraDamage"].UpgradeValueBy(1);
    }
}
