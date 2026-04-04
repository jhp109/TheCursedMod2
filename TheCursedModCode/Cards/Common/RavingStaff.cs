using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 날뛰는 지팡이(Raving Staff) - 무작위 적에게 피해를 13만큼 X번 줍니다. X번 업보 4. (강화 시 피해 16)
/// </summary>
public sealed class RavingStaff() : TheCursedModCard(0, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy), IKarmaAttack
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(13, ValueProp.Move),
        new PowerVar<KarmaTurn2Power>("KarmaPower", 4m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, play, xValue).Execute(choiceContext);
        if (xValue > 0)
            await ApplyKarma(xValue * DynamicVars["KarmaPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
