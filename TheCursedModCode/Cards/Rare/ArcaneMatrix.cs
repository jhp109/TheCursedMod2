using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 비전 행렬(Arcane Matrix) - 마법진 카드를 뽑을 때 마다, 에너지를 1 얻고 카드를 1장 뽑습니다.
/// 비용 2, Uncommon, Power. 강화 시 에너지 2 및 카드 2장.
/// </summary>
public sealed class ArcaneMatrix() : TheCursedModCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ArcaneMatrixPower>(1m),
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        EnergyHoverTip
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ArcaneMatrixPower>
            (Owner.Creature, DynamicVars["ArcaneMatrixPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ArcaneMatrixPower"].UpgradeValueBy(1);
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
