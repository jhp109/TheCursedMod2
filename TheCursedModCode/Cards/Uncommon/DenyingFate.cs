using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 운명 거부(Denying Fate) - 피해를 9 줍니다. 이번 턴에 예정된 업보를 1턴 뒤로 미룹니다. 소멸. (강화 시 피해 12)
/// </summary>
public sealed class DenyingFate() : TheCursedModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override bool ShouldGlowGoldInternal =>
        Owner?.Creature.HasPower<KarmaTurn1Power>() == true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var k1 = Owner!.Creature.GetPower<KarmaTurn1Power>();
        if (k1 != null)
        {
            int k1Amount = k1.Amount;
            await PowerCmd.Remove(k1);
            // Set null to CardModel because it's just delaying the Karma, not applying the new Karma.
            await PowerCmd.Apply<KarmaTurn2Power>(Owner.Creature, k1Amount, Owner.Creature, null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
