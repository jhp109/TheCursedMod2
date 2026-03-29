using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 니오우의 권능(Neow's Might) - 손에 있는 다른 모든 카드의 비용을 이번 턴 동안 1 줄입니다. 업보를 22 얻습니다.
/// 강화 시 Retain 추가.
/// </summary>
public sealed class NeowsMight() : TheCursedModCard(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<KarmaTurn2Power>("KarmaPower", 22m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card == this) continue;
            if (card.EnergyCost.CostsX) continue;
            card.EnergyCost.AddThisTurnOrUntilPlayed(-1);
        }
        await ApplyKarma(choiceContext, DynamicVars["KarmaPower"].IntValue);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
