using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 비전 휩쓸기(Arcane Sweep) - 모든 적에게 피해를 7 줍니다. 손에 마법진 카드가 있다면, 한번 더 피해를 7 줍니다.
/// 강화 시 피해 9.
/// </summary>
public sealed class ArcaneSweep() : TheCursedModCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override bool ShouldGlowGoldInternal =>
        PileType.Hand.GetPile(Owner).Cards.Any(c => c is CircleCard);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle)
    ];

    private Task BuildAndExecuteAttack(PlayerChoiceContext ctx)
    {
        var combatState = CombatState!;
        return DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(combatState)
            .BeforeDamage(async delegate
            {
                List<Creature> targets = [..combatState.HittableEnemies];
                NSweepingBeamVfx? vfx = NSweepingBeamVfx.Create(Owner.Creature, targets);
                if (vfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                    await Cmd.Wait(0.5f);
                }
            })
            .Execute(ctx);
    }

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
