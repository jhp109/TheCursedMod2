using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 비전 휩쓸기(Arcane Sweep) - 모든 적에게 피해를 6 줍니다. 손에 마법진 카드가 있다면, 한번 더 피해를 6 줍니다.
/// 강화 시 피해 8.
/// </summary>
public sealed class ArcaneSweep() : TheCursedModCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle)
    ];

    private System.Threading.Tasks.Task BuildAndExecuteAttack(PlayerChoiceContext ctx) =>
        DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await BuildAndExecuteAttack(choiceContext);

        if (PileType.Hand.GetPile(Owner).Cards.Any(c => c is CircleCard))
            await BuildAndExecuteAttack(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
