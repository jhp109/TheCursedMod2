using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 악마군주의 칼날(Demonic Blade) - 피해를 10 줍니다. 이번 전투에서 의례로 소멸시킨 저주 하나당 피해량이 5 증가합니다. 보존.
/// (강화 시 피해 증가량 8)
/// </summary>
public sealed class DemonicBlade() : TheCursedModCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(10),
        new ExtraDamageVar(5),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) => RiteCard.GetRiteCurseExhaustedCount(card.CombatState))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");
        await CommonActions.CardAttack(
            this, play, 1, vfx: "vfx/vfx_giant_horizontal_slash", tmpSfx: "slash_attack.mp3")
            .WithHitVfxNode((Creature t) => NBigSlashVfx.Create(t))
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(3m);
    }
}
