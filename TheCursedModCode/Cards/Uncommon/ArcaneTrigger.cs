using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 비전 방아쇠(Arcane Trigger) - 피해를 8 줍니다. 손에 있는 모든 마법진의 효과를 조건을 무시하고 강제로 발동시킵니다.
/// 강화 시 피해 9.
/// </summary>
public sealed class ArcaneTrigger() : TheCursedModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, 1, vfx: "vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var circles = PileType.Hand.GetPile(Owner).Cards.OfType<CircleCard>().ToList();
        foreach (var circle in circles)
            await circle.ForceTrigger(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
