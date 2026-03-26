using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using TheCursedMod.TheCursedModCode;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 의례(Rite) 메카닉의 베이스 클래스.
/// 카드를 사용하면 손에 있는 카드를 한 장 선택하여 소멸시킨다.
/// 소멸된 카드가 저주 카드라면 OnRiteEffect가 발동된다.
/// </summary>
public abstract class RiteCard(
    int cost, CardType type, CardRarity rarity, TargetType target, IEnumerable<IHoverTip>? extraTips = null)
    : TheCursedModCard(cost, type, rarity, target)
{ 
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        (extraTips ?? []).Append(HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite));

    // 이번 턴에 의례로 저주를 소멸시켰는지 추적
    private static int _riteCurseExhaustedRound = -1;
    private static CombatState? _riteCurseExhaustedCombat;

    /// <summary>
    /// 이번 턴(현재 CombatState 기준)에 의례로 저주를 소멸시켰으면 true를 반환합니다.
    /// </summary>
    public static bool WasRiteCurseExhaustedThisTurn(CombatState? combat)
        => combat != null
           && ReferenceEquals(_riteCurseExhaustedCombat, combat)
           && _riteCurseExhaustedRound == combat.RoundNumber;

    private static readonly LocString SelectPrompt = new("cards", "THECURSEDMOD-RITE.selectPrompt");

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await OnBaseEffect(choiceContext, play);

        if (Owner?.PlayerCombatState?.Hand.IsEmpty == false)
        {
            var prefs = new CardSelectorPrefs(SelectPrompt, 1);
            var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);

            var card = selected.FirstOrDefault();
            if (card != null)
            {
                bool wasCurse = card.Type == CardType.Curse;
                await CardCmd.Exhaust(choiceContext, card, false, false);

                if (wasCurse)
                {
                    _riteCurseExhaustedRound = CombatState!.RoundNumber;
                    _riteCurseExhaustedCombat = CombatState;
                    await OnRiteEffect(choiceContext, play);
                }
            }
        }
    }

    /// <summary>
    /// 의례 조건(카드 소멸) 이전에 발동하는 기본 효과.
    /// 기본 효과가 없는 카드(재에서 재로 등)는 재정의하지 않아도 된다.
    /// </summary>
    protected virtual Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
        => Task.CompletedTask;

    /// <summary>
    /// 저주 카드를 소멸시켰을 때 발동하는 의례 효과.
    /// </summary>
    protected abstract Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play);
}
