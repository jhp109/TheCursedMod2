using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 돌려 막기(Rolling over Debt) - 업보가 있다면, 턴 종료 시 방어도를 8 얻습니다.
/// 강화 시 방어도 12.
/// </summary>
public sealed class RollingOverDebt() : TheCursedModCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RollingOverDebtPower>(8m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner!.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<RollingOverDebtPower>(Owner.Creature, DynamicVars["RollingOverDebtPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RollingOverDebtPower"].UpgradeValueBy(4m);
    }
}
