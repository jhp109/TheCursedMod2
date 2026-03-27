using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 저주받은 부메랑(Cursed Boomerang) - 무작위 적들에게 피해를 4만큼 3번 줍니다. 찌꺼기를 얻습니다. (강화 - 4만큼 4번)
/// </summary>
public sealed class CursedBoomerang() : TheCursedModCard(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Dregs>(false)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
        new RepeatVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState != null)
        {
            await CommonActions.CardAttack(this, play, DynamicVars.Repeat.IntValue, "vfx/vfx_attack_slash", "", "")
                .Execute(choiceContext);

            var dregs = CombatState.CreateCard<Dregs>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(dregs, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}
