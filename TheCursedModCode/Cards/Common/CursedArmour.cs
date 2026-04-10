using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 저주받은 갑옷(Cursed Armour) - 방어도를 21 얻습니다. 무작위 저주 카드 2장을 뽑을 카드 더미에 섞습니다.
/// (강화 시 방어도 27)
/// </summary>
public sealed class CursedArmour() : TheCursedModCard(2, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(21, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        for (int i = 0; i < 2; i++)
            await GainRandomCurse(PileType.Draw);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
    }
}
