using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 전화위복(Blessing in Disguise) - 손에 있는 저주 카드를 전부 소멸시킵니다. 그 수 만큼 방어도를 6 얻습니다.
/// (강화 시 방어도 8)
/// </summary>
public sealed class BlessingInDisguise() : TheCursedModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var card in GetCurseCards().ToList())
        {
            await CardCmd.Exhaust(choiceContext, card);
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }

    private IEnumerable<CardModel> GetCurseCards()
    {
        var pile = PileType.Hand.GetPile(Owner);
        return pile.Cards.Where(c => c.Type == CardType.Curse);
    }
}
