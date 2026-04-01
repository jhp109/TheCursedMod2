using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 금단의 맹공(Forbidden Wallop) - 피해를 23 줍니다. 막히지 않은 피해량만큼 방어도를 얻습니다. 업보 14. (강화 시 피해 30)
/// </summary>
public sealed class ForbiddenWallop() : TheCursedModCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IKarmaAttack
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(23, ValueProp.Move),
        new PowerVar<KarmaTurn2Power>("KarmaPower", 14m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await ApplyKarma(DynamicVars["KarmaPower"].IntValue);
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (dealer == Owner.Creature && cardSource == this && result.UnblockedDamage > 0)
            await CreatureCmd.GainBlock(Owner.Creature, result.UnblockedDamage, ValueProp.Move, null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
    }
}
