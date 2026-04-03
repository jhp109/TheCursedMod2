using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 금단의 몽둥이질(Forbidden Bludgeon) - 피해를 60 줍니다. 업보 25. (강화 시 피해 80)
/// </summary>
public sealed class ForbiddenBludgeon() : TheCursedModCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IKarmaAttack
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(60, ValueProp.Move),
        new PowerVar<KarmaTurn2Power>("KarmaPower", 25m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions
            .CardAttack(this, play, vfx: "vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
        await ApplyKarma(DynamicVars["KarmaPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(20m);
    }
}
