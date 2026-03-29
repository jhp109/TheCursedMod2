using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 날뛰는 지팡이(Raving Staff) - 무작위 적에게 피해를 11만큼 X번 줍니다. X번 업보 5. (강화 시 피해 14)
/// </summary>
public sealed class RavingStaff() : TheCursedModCard(0, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(11, ValueProp.Move),
        new PowerVar<KarmaTurn2Power>(5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int xValue = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, play, xValue).Execute(choiceContext);
        if (xValue > 0)
            await PowerCmd.Apply<KarmaTurn2Power>(Owner!.Creature, xValue * DynamicVars["KarmaTurn2Power"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
