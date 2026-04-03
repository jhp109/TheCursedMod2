using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 자유와 속박(Freedom and Bondage) - 모든 플레이어들이 에너지를 2 얻고 무작위 저주 카드를 1장 얻습니다. 소멸.
/// 멀티플레이어 전용. 비용 0, Rare, Skill. 강화 시 에너지 3.
/// </summary>
public sealed class FreedomAndBondage() : TheCursedModCard(0, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        EnergyHoverTip
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var curseCandidates = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedByModifiers)
            .ToList();

        foreach (var player in CombatState!.Players)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);

            if (curseCandidates.Count > 0)
            {
                var randomCurse = Owner.RunState.Rng.Niche.NextItem(curseCandidates)!;
                var curseCard = CombatState.CreateCard(randomCurse, player);
                await CardPileCmd.AddGeneratedCardToCombat(curseCard, PileType.Hand, addedByPlayer: false);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
