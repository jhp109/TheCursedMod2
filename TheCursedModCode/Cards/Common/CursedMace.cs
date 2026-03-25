using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 저주받은 철퇴(Cursed Mace) - 피해를 12 줍니다. 취약을 2 부여합니다. 찌꺼기를 얻습니다. (강화 시 피해 14, 취약 3)
/// </summary>
public sealed class CursedMace() : TheCursedModCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move),
        new PowerVar<VulnerablePower>("Vulnerable", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        if (play.Target != null)
            await CommonActions.Apply<VulnerablePower>(play.Target, this, DynamicVars.Vulnerable.BaseValue);

        if (base.CombatState != null) {
            Dregs dregs = base.CombatState.CreateCard<Dregs>(base.Owner);
            await CardPileCmd.AddGeneratedCardToCombat(dregs, PileType.Hand, addedByPlayer:true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}
