using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 마법진(Circle) 메카닉의 베이스 클래스.
/// 사용불가 카드로, 손에 들고 있는 상태에서 특정 조건의 카드가 사용될 때 효과가 발동된다.
/// 이 클래스를 상속할 시 ExtraHoverTips에 Circle keyword를 넣어줘야 함.
/// </summary>
public abstract class CircleCard(CardRarity rarity)
    : TheCursedModCard(-1, CardType.Skill, rarity, TargetType.None)
{
    private bool _suppressNextTrigger;

    /// <summary>
    /// 이번 전투에서 이 마법진이 발동된 총 횟수. 비전 방출 등에서 참조합니다.
    /// </summary>
    public int TriggerCount { get; private set; }

    protected override bool IsPlayable => false;

    protected sealed override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        => Task.CompletedTask;

    /// <summary>
    /// 플레이된 카드가 마법진 발동 조건을 충족하는지 반환합니다.
    /// </summary>
    protected abstract bool ShouldTrigger(CardPlay cardPlay);

    /// <summary>
    /// 마법진 효과.
    /// </summary>
    protected abstract Task OnCircleEffect(PlayerChoiceContext choiceContext);

    private void FlashCardInHand()
    {
        NCard? nCard = NCard.FindOnTable(this);
        if (nCard == null || !GodotObject.IsInstanceValid(nCard)) return;

        var tween = nCard.CreateTween().SetParallel();
        tween.TweenProperty(nCard, "modulate", new Color("#ffff99"), 0.12f);
        tween.Chain();
        tween.TweenProperty(nCard, "modulate", Colors.White, 0.4f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    }

    /// <summary>
    /// 이 마법진이 해당 카드 플레이에 반응하여 발동될지 여부를 반환합니다.
    /// </summary>
    public bool WillTrigger(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return false;
        var hand = PileType.Hand.GetPile(Owner);
        if (!hand.Cards.Contains(this)) return false;
        return ShouldTrigger(cardPlay);
    }

    private async Task TriggerEffect(PlayerChoiceContext context)
    {
        TriggerCount++;
        NPowerUpVfx.CreateGhostly(Owner.Creature);
        FlashCardInHand();
        await OnCircleEffect(context);

        var battlemage = Owner.Creature.GetPower<BattlemagePower>();
        if (battlemage != null)
        {
            battlemage.TriggerFlash();
            await PowerCmd.Apply<VigorPower>(Owner.Creature, battlemage.Amount, Owner.Creature, null);
        }

        var manaBastion = Owner.Creature.GetPower<ManaBastionPower>();
        if (manaBastion != null)
        {
            manaBastion.TriggerFlash();
            await CreatureCmd.GainBlock(Owner.Creature, manaBastion.Amount, ValueProp.Move, null);
        }
    }

    /// <summary>
    /// 비전 방아쇠 등에서 조건을 무시하고 강제로 마법진 효과를 발동합니다.
    /// 이후 AfterCardPlayed에서의 자연 발동을 억제합니다.
    /// </summary>
    public async Task ForceTrigger(PlayerChoiceContext context)
    {
        var hand = PileType.Hand.GetPile(Owner);
        if (!hand.Cards.Contains(this)) return;

        _suppressNextTrigger = true;
        await TriggerEffect(context);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_suppressNextTrigger)
        {
            _suppressNextTrigger = false;
            return;
        }

        if (!WillTrigger(cardPlay)) return;

        await TriggerEffect(context);
    }
}
