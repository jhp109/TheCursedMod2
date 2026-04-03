using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 마력탄(Mana Bullet) - 피해를 6 줍니다. 손에 있는 마법진의 수 만큼 반복합니다. (강화 시 피해 8)
/// </summary>
public sealed class ManaBullet() : TheCursedModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedRepeat").WithMultiplier(static (card, _) =>
            PileType.Hand.GetPile(card.Owner).Cards.Count(c => c is CircleCard))
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int circleCount = PileType.Hand.GetPile(Owner).Cards.Count(c => c is CircleCard);
        await CommonActions.CardAttack(this, play, circleCount + 1, vfx: "vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
