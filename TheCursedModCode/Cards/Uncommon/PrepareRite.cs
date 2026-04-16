using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 의례 준비 (Prepare Rite) - 휘발성. 뽑을 카드 더미의 카드를 1장 찌꺼기로 변화시킵니다. 다음 턴에 카드를 1장 더 뽑습니다.
/// (강화 시 카드 2장)
/// </summary>
public sealed class PrepareRite() : TheCursedModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    private static readonly LocString SelectPrompt = new("cards", "THECURSEDMOD-PREPARE_RITE.selectionScreenPrompt");

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DrawCardsNextTurnPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Dregs>(false)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = await CommonActions.SelectSingleCard(this, SelectPrompt, choiceContext, PileType.Draw);
        if (target != null)
            await Dregs.TransformToDregs(this, target);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            Owner.Creature, DynamicVars["DrawCardsNextTurnPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DrawCardsNextTurnPower"].UpgradeValueBy(1m);
    }
}
