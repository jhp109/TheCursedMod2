using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 의례(Rite) 메카닉의 베이스 클래스.
/// 카드를 사용하면 손에 있는 카드를 한 장 선택하여 소멸시킨다.
/// 소멸된 카드가 저주 카드라면 OnRiteEffect가 발동된다.
/// </summary>
public abstract class RiteCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : TheCursedModCard(cost, type, rarity, target)
{
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
                    await OnRiteEffect(choiceContext, play);
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
