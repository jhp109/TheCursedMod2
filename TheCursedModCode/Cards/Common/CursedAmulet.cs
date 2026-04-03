using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 저주받은 목걸이(Cursed Amulet) - 방어도를 10 얻습니다. 카드를 1장 뽑습니다.
/// 무작위 저주 카드를 뽑을 카드 더미에 섞습니다. (강화 시 방어도 13)
/// </summary>
public sealed class CursedAmulet() : TheCursedModCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await CardPileCmd.Draw(choiceContext, 1, Owner);

        await GainRandomCurse(PileType.Draw);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
