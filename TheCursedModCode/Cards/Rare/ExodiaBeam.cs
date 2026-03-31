using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 엑조디아 빔(Exodia Beam) - 손에 서로 다른 종류의 마법진이 5장 이상 있을때만 사용 가능합니다.
/// 모든 적에게 피해를 50 줍니다. 강화 시 피해 60.
/// </summary>
public sealed class ExodiaBeam() : TheCursedModCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const int RequiredCircleTypes = 5;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override bool IsPlayable =>
        PileType.Hand.GetPile(Owner).Cards
            .OfType<CircleCard>()
            .Select(c => c.GetType())
            .Distinct()
            .Count() >= RequiredCircleTypes;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(50m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(async delegate
            {
                var nHyperbeamVfx = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
                if (nHyperbeamVfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                    await Cmd.Wait(0.5f);
                }
                foreach (var enemy in enemies)
                {
                    var nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(Owner.Creature, enemy);
                    if (nHyperbeamImpactVfx != null)
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                }
            })
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}
