using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 마법진(Circle) 메카닉의 베이스 클래스.
/// 사용불가 카드로, 손에 들고 있는 상태에서 특정 조건의 카드가 사용될 때 효과가 발동된다.
/// 이 클래스를 상속할 시 ExtraHoverTips에 Circle keyword를 넣어줘야 함.
/// </summary>
public abstract class CircleCard(CardRarity rarity)
    : TheCursedModCard(-1, CardType.Skill, rarity, TargetType.None)
{
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

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner) return;
        if (!ShouldTrigger(cardPlay)) return;

        var hand = PileType.Hand.GetPile(Owner);
        if (!hand.Cards.Contains(this)) return;

        NPowerUpVfx.CreateGhostly(Owner.Creature);
        FlashCardInHand();
        await OnCircleEffect(context);
    }
}
