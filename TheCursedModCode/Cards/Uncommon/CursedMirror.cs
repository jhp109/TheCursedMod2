using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 저주받은 거울(Cursed Mirror) - 피해를 18 줍니다. 매료됨을 얻습니다. (강화 시 피해 23)
/// </summary>
public sealed class CursedMirror() : TheCursedModCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Enthralled>(false)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(18, ValueProp.Move)
    ];
 
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        if (CombatState != null)
        {
            var enthralled = CombatState.CreateCard<Enthralled>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(enthralled, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
