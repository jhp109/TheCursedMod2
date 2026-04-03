using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 영혼 공물(Spirit Offering) - 매 턴 처음으로 의례로 저주를 소멸시킬 때, 힘을 1 얻습니다.
/// 비용 1, Uncommon, Power. 강화 시 힘 2 (파워 수치 증가).
/// </summary>
public sealed class SpiritOffering() : TheCursedModCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SpiritOfferingPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SpiritOfferingPower>(Owner.Creature, DynamicVars["SpiritOfferingPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SpiritOfferingPower"].UpgradeValueBy(1m);
    }
}
