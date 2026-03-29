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
/// 금단의 몽둥이질(Forbidden Bludgeon) - 피해를 60 줍니다. 업보 30. (강화 시 피해 80)
/// </summary>
public sealed class ForbiddenBludgeon() : TheCursedModCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(60, ValueProp.Move),
        new PowerVar<KarmaTurn2Power>(30m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var k1 = Owner.Creature.GetPower<KarmaTurn1Power>();
        if (k1 != null && !Owner.Creature.HasPower<KarmaTurn2Power>())
        {
            int k1Amount = k1.Amount;
            await PowerCmd.Remove(k1);
            await PowerCmd.Apply<KarmaTurn1Power>(Owner.Creature, k1Amount, Owner.Creature, this);
            await PowerCmd.Apply<KarmaTurn2Power>(Owner.Creature, DynamicVars["KarmaTurn2Power"].IntValue, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<KarmaTurn2Power>(Owner.Creature, DynamicVars["KarmaTurn2Power"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(20m);
    }
}
