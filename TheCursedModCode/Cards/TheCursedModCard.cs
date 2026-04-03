using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using TheCursedMod.TheCursedModCode.Character;
using TheCursedMod.TheCursedModCode.Extensions;
using TheCursedMod.TheCursedModCode.Powers;
using TheCursedMod.TheCursedModCode.Relics;

namespace TheCursedMod.TheCursedModCode.Cards;

[Pool(typeof(TheCursedModCardPool))]
public abstract class TheCursedModCard(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : CustomCardModel(cost, type, rarity, target, showInCardLibrary, autoAdd)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants: fullart 250x350, normalart 250x190
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>
    /// 카드가 "사용불가"인지 확인합니다.
    /// HasUnplayableKeyword 이거나 마법진(CircleCard) 카드인 경우 사용불가로 간주합니다.
    /// </summary>
    public static bool IsUnplayableCard(CardModel c)
    {
        c.CanPlay(out var reason, out _);
        return reason.HasFlag(UnplayableReason.HasUnplayableKeyword) || c is CircleCard;
    }

    /// <summary>
    /// 손에 있는 사용불가 카드의 수를 반환합니다.
    /// </summary>
    protected int CountUnplayableInHand() =>
        PileType.Hand.GetPile(Owner).Cards.Count(IsUnplayableCard);

    /// <summary>
    /// 손에 사용불가 카드가 있는지 확인합니다.
    /// </summary>
    protected bool HasUnplayableInHand() =>
        PileType.Hand.GetPile(Owner).Cards.Any(IsUnplayableCard);

    /// <summary>
    /// 무작위 저주 카드를 지정한 파일에 추가합니다.
    /// 네잎클로버 부적이 있다면 대신 찌꺼기를 패에 추가합니다.
    /// </summary>
    protected async Task GainRandomCurse(PileType pile)
    {
        var cloverRelic = Owner.Relics.OfType<FourLeafCloverCharmRelic>().FirstOrDefault();
        if (cloverRelic != null)
        {
            cloverRelic.Flash();
            await Dregs.CreateAndAddToHand(Owner, 1);
            return;
        }

        var curseCandidates = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedByModifiers)
            .ToList();

        if (curseCandidates.Count == 0) return;

        var randomCurse = Owner.RunState.Rng.Niche.NextItem(curseCandidates)!;
        var curseCard = CombatState!.CreateCard(randomCurse, Owner);
        if (pile == PileType.Draw)
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    curseCard, pile, addedByPlayer: false, position: CardPilePosition.Random));
            await Cmd.Wait(0.5f);
        }
        else
        {
            await CardPileCmd.AddGeneratedCardToCombat(curseCard, pile, addedByPlayer: false);
        }
    }

    /// <summary>
    /// GracePower가 활성화된 경우 KarmaTurn3Power를, 그렇지 않으면 KarmaTurn2Power를 적용합니다.
    /// </summary>
    protected Task ApplyKarma(decimal amount)
    {
        if (Owner.Creature.HasPower<GracePower>()) {
            Owner.Creature.GetPower<GracePower>()!.TriggerFlash();
            return PowerCmd.Apply<KarmaTurn3Power>(Owner.Creature, amount, Owner.Creature, this);
        }
        return PowerCmd.Apply<KarmaTurn2Power>(Owner.Creature, amount, Owner.Creature, this);
    }
}
