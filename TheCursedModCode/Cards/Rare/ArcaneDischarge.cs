using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 비전 방출(Arcane Discharge) - 이번 전투에서 마법진의 효과가 발동된 횟수의 3배만큼 모든 적에게 피해를 줍니다.
/// 강화 시 4배.
/// </summary>
public sealed class ArcaneDischarge() : TheCursedModCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0m),
        new ExtraDamageVar(3),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static (CardModel card, Creature? _) =>
        {
            var piles = new[] { PileType.Hand, PileType.Draw, PileType.Discard };
            int triggers = piles.Sum(pt => pt.GetPile(card.Owner).Cards.OfType<CircleCard>().Sum(c => c.TriggerCount));
            return triggers;
        })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();

        foreach (var enemy in enemies)
            VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_lightning");
        SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_evoke");

        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}
