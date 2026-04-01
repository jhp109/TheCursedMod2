using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 의례(Rite) 메카닉의 베이스 클래스.
/// 카드를 사용하면 손에 있는 카드를 한 장 선택하여 소멸시킨다.
/// 소멸된 카드가 저주 카드라면 OnRiteEffect가 발동된다.
/// 이 클래스를 상속할 시 ExtraHoverTips에 Rite keyword를 넣어줘야 함.
/// </summary>
public abstract class RiteCard(
    int cost, CardType type, CardRarity rarity, TargetType target) : TheCursedModCard(cost, type, rarity, target)
{ 
    // 의례 카드 풀 캐시 (최초 1회만 계산)
    private static IReadOnlyList<CardModel>? _riteCardPool;

    /// <summary>
    /// 이 캐릭터의 카드 풀에서 RiteCard를 상속한 카드 목록을 반환합니다. 최초 1회만 계산됩니다.
    /// </summary>
    public static IReadOnlyList<CardModel> GetRiteCardPool(Player owner)
        => _riteCardPool ??= owner.Character.CardPool
            .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
            .Where(c => c is RiteCard)
            .ToList();

    // 이번 턴에 의례로 저주를 소멸시켰는지 추적
    private static int _riteCurseExhaustedRound = -1;
    private static CombatState? _riteCurseExhaustedCombat;

    // 이번 전투에서 의례로 소멸시킨 저주 총 횟수 추적
    private static int _riteCombatCurseCount = 0;
    private static CombatState? _riteCombatCountCombat;

    /// <summary>
    /// 이번 턴(현재 CombatState 기준)에 의례로 저주를 소멸시켰으면 true를 반환합니다.
    /// </summary>
    public static bool WasRiteCurseExhaustedThisTurn(CombatState? combat)
        => combat != null
           && ReferenceEquals(_riteCurseExhaustedCombat, combat)
           && _riteCurseExhaustedRound == combat.RoundNumber;

    /// <summary>
    /// 이번 전투에서 의례로 소멸시킨 저주의 총 횟수를 반환합니다.
    /// </summary>
    public static int GetRiteCurseExhaustedCount(CombatState? combat)
        => combat != null && ReferenceEquals(_riteCombatCountCombat, combat)
           ? _riteCombatCurseCount
           : 0;

    private static readonly LocString SelectPrompt = new("cards", "THECURSEDMOD-RITE.selectionScreenPrompt");

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

                    if (!ReferenceEquals(_riteCombatCountCombat, CombatState))
                    {
                        _riteCombatCurseCount = 0;
                        _riteCombatCountCombat = CombatState;
                    }
                    _riteCombatCurseCount++;

                    // 금단의 형상으로 다음 업보 공격 강화
                    var forbiddenForm = Owner?.Creature.GetPower<ForbiddenFormPower>();
                    if (forbiddenForm != null)
                        await PowerCmd.Apply<MultiplyNextKarmaAttackPower>(
                            Owner!.Creature, forbiddenForm.Amount, Owner.Creature, null);

                    await OnRiteEffect(choiceContext, play);

                    // 고대신의 부름으로 의례 효과 한번 더 발동
                    var ancientPower = Owner?.Creature.GetPower<CallOfTheAncientPower>();
                    if (ancientPower != null)
                    {
                        await PowerCmd.Decrement(ancientPower);
                        await OnRiteEffect(choiceContext, play);
                    }
                } else
                {
                    // 저주가 아니었더라도 고대신의 부름 효과는 제거해야 함.
                    // Decrease the power anyway. it's "The next N Rite cards activate its Rite effect an extra time".
                    var ancientPower = Owner?.Creature.GetPower<CallOfTheAncientPower>();
                    if (ancientPower != null)
                    {
                        await PowerCmd.Decrement(ancientPower);
                    }
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
