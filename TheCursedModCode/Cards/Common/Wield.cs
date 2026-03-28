using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 휘두르기(Wield) - 피해를 7 줍니다. 손에 있는 사용불가 카드의 수 만큼 카드를 뽑습니다. (강화 시 피해 10)
/// </summary>
public sealed class Wield() : TheCursedModCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        int unplayableCount = PileType.Hand.GetPile(Owner).Cards.Count(c =>
        {
            c.CanPlay(out var reason, out _);
            return reason.HasFlag(UnplayableReason.HasUnplayableKeyword);
        });
        if (unplayableCount > 0)
            await CardPileCmd.Draw(choiceContext, unplayableCount, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
