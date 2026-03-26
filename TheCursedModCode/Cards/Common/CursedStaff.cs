using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 저주받은 지팡이(Cursed Staff) - 피해를 18 줍니다. 찌꺼기를 2장 얻습니다. (강화 시 피해 24)
/// </summary>
public sealed class CursedStaff() : TheCursedModCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Dregs>(false)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(18, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(
            this, play, vfx: "vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
        if (base.CombatState != null) {
            List<CardModel> list = new List<CardModel>();
            for (int i = 0; i < 2; i++) list.Add(base.CombatState.CreateCard<Dregs>(base.Owner));
            await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer:true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
