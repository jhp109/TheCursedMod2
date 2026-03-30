using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 금단의 형상(Forbidden Form) - 무작위 저주 카드 3장을 뽑을 카드 더미에 섞습니다.
/// 저주 카드를 뽑을 때 마다, 힘을 1 얻습니다. 업보 20.
/// 강화 시 힘 2.
/// </summary>
public sealed class ForbiddenForm() : TheCursedModCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ForbiddenFormPower>("StrengthPower", 1m),
        new PowerVar<KarmaTurn2Power>("KarmaPower", 20m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ForbiddenFormPower>(Owner.Creature, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);

        var curseCandidates = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedByModifiers)
            .ToList();

        for (int i = 0; i < 3 && curseCandidates.Count > 0; i++)
        {
            var randomCurse = Owner.RunState.Rng.Niche.NextItem(curseCandidates)!;
            var curseCard = CombatState!.CreateCard(randomCurse, Owner);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    curseCard, PileType.Draw, addedByPlayer: false, position: CardPilePosition.Random));
            await Cmd.Wait(0.5f);
        }

        await ApplyKarma(DynamicVars["KarmaPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}
