using System.Runtime.CompilerServices;
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
    private static readonly ConditionalWeakTable<Player, IReadOnlyList<CardModel>> _riteCardPoolTable = new();

    /// <summary>
    /// 이 캐릭터의 카드 풀에서 RiteCard를 상속한 카드 목록을 반환합니다. Player별로 캐시됩니다.
    /// </summary>
    public static IReadOnlyList<CardModel> GetRiteCardPool(Player owner)
        => _riteCardPoolTable.GetValue(owner, p => p.Character.CardPool
            .GetUnlockedCards(p.UnlockState, p.RunState.CardMultiplayerConstraint)
            .Where(c => c is RiteCard)
            .ToList());

    /// <summary>
    /// 전투별 의례 추적 상태. CombatState를 weak key로 사용하여 전투 종료 시 자동 GC됩니다.
    /// static이지만 CombatState 단위로 격리되므로 플레이어 간 상태 오염이 없습니다.
    /// </summary>
    private sealed class RiteState
    {
        public int RiteCurseExhaustedRound = -1;
        public int RiteEffectTriggerCount = 0;
    }

    private static readonly ConditionalWeakTable<CombatState, RiteState> _stateTable = new();

    private static RiteState GetState(CombatState combat) => _stateTable.GetOrCreateValue(combat);

    /// <summary>
    /// 이번 턴(현재 CombatState 기준)에 의례로 저주를 소멸시켰으면 true를 반환합니다.
    /// </summary>
    public static bool WasRiteEffectTriggeredThisTurn(CombatState? combat)
        => combat != null && GetState(combat).RiteCurseExhaustedRound == combat.RoundNumber;

    /// <summary>
    /// 지난 턴(현재 RoundNumber - 1)에 의례로 저주를 소멸시켰으면 true를 반환합니다.
    /// </summary>
    public static bool WasRiteEffectTriggeredLastTurn(CombatState? combat)
        => combat != null && GetState(combat).RiteCurseExhaustedRound == combat.RoundNumber - 1;

    /// <summary>
    /// 이번 전투에서 의례의 효과가 발동된 총 횟수를 반환합니다.
    /// </summary>
    public static int GetRiteEffectTriggerCount(CombatState? combat)
        => combat == null ? 0 : GetState(combat).RiteEffectTriggerCount;

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
                    GetState(CombatState!).RiteCurseExhaustedRound = CombatState!.RoundNumber;

                    await ExecuteRiteEffect(choiceContext, play);

                    // 고대신의 부름으로 의례 효과 한번 더 발동
                    var ancientPower = Owner?.Creature.GetPower<CallOfTheAncientPower>();
                    if (ancientPower != null)
                    {
                        await PowerCmd.Decrement(ancientPower);
                        await ExecuteRiteEffect(choiceContext, play);
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

    /// <summary>
    /// OnRiteEffect를 호출하고, 이후 의례 효과에 반응하는 모든 사이드 이펙트를 처리합니다.
    /// (금단의 형상, 영혼 공물, 마나 순환, 의식의 마법진)
    /// </summary>
    private async Task ExecuteRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        GetState(CombatState!).RiteEffectTriggerCount++;

        await OnRiteEffect(choiceContext, play);

        // 금단의 형상으로 다음 업보 공격 강화
        var forbiddenForm = Owner?.Creature.GetPower<ForbiddenFormPower>();
        if (forbiddenForm != null)
            await PowerCmd.Apply<MultiplyNextKarmaAttackPower>(
                Owner!.Creature, forbiddenForm.Amount, Owner.Creature, null);

        // 영혼 공물 - 매 턴 처음으로 의례 효과 발동 시 힘 획득
        var spiritOffering = Owner?.Creature.GetPower<SpiritOfferingPower>();
        if (spiritOffering != null)
            await spiritOffering.TriggerOnRiteEffect(choiceContext);

        // 마나 순환 - 매 턴 처음으로 의례 효과 발동 시 에너지 획득
        var manaCirculation = Owner?.Creature.GetPower<ManaCirculationPower>();
        if (manaCirculation != null)
            await manaCirculation.TriggerOnRiteEffect(choiceContext);

        // 제사장의 반지 - 의례 효과 6회마다 에너지 획득
        var ritualistsRing = Owner?.Relics.OfType<Relics.RitualistsRingRelic>().FirstOrDefault();
        if (ritualistsRing != null)
            await ritualistsRing.OnRiteEffectTriggered(choiceContext);

        // 영혼 그릇 - 카운터 UI 갱신
        Owner?.Relics.OfType<Relics.SoulVesselRelic>().FirstOrDefault()?.OnRiteEffectTriggered();

        // 의식의 마법진 효과 발동
        var circles = PileType.Hand.GetPile(Owner!).Cards.OfType<CircleOfRitual>().ToList();
        foreach (var circle in circles)
            await circle.ForceTrigger(choiceContext);
    }
}
