using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 불똥(Scintilla) - 피해를 8 줍니다. 버린 카드 더미의 카드를 1장 선택하여 찌꺼기로 변화시킵니다. (강화 시 피해 11)
/// </summary>
public sealed class Scintilla() : TheCursedModCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private static readonly LocString SelectPrompt = new("cards", "THECURSEDMOD-SCINTILLA.selectionScreenPrompt");

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Dregs>(false)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var target = await CommonActions.SelectSingleCard(this, SelectPrompt, choiceContext, PileType.Discard);
        if (target != null)
        {
            await Dregs.TransformToDregs(this, target);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
