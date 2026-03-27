using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 골칫덩이 사역마(Troublous Minion) - 방어도를 6 얻습니다. 무작위 저주 카드를 얻습니다.
/// 이 카드를 뽑을 카드 더미에 섞어넣습니다.
/// (강화 시 방어도 9)
/// </summary>
public sealed class TroublousMinion() : TheCursedModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var curseCandidates = ModelDb.CardPool<CurseCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedByModifiers)
            .ToList();

        if (curseCandidates.Count > 0)
        {
            var randomCurse = Owner.RunState.Rng.Niche.NextItem(curseCandidates)!;
            var curseCard = CombatState!.CreateCard(randomCurse, Owner);
            await CardPileCmd.AddGeneratedCardToCombat(curseCard, PileType.Hand, addedByPlayer: false);
        }

        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Random);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
