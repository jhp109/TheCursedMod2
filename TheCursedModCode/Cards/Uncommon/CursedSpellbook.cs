using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 저주받은 주문서(Cursed Spellbook) - 카드를 2장 뽑습니다. 무작위 저주 카드 1장을 뽑을 카드 더미에 섞습니다.
/// (강화 시 카드 3장 뽑음)
/// </summary>
public sealed class CursedSpellbook() : TheCursedModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.Draw(this, choiceContext);

        var curseCandidates = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedByModifiers)
            .ToList();

        if (curseCandidates.Count > 0)
        {
            var randomCurse = Owner.RunState.Rng.Niche.NextItem(curseCandidates)!;
            var curseCard = CombatState!.CreateCard(randomCurse, Owner);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(curseCard, PileType.Draw, addedByPlayer: false));
            await Cmd.Wait(0.5f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
