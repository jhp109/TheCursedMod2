using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 망토 두르기(Wear Cloak) - 방어도를 7 얻습니다. 손에 사용불가 카드가 있다면, 에너지를 1 얻습니다. TODO - refactor
/// (강화 시 방어도 9)
/// </summary>
public sealed class WearCloak() : TheCursedModCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(7, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Unplayable),
    ];

    protected override bool ShouldGlowGoldInternal => HasUnplayableInHand();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        if (HasUnplayableInHand())
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }

    private bool HasUnplayableInHand()
    {
        return PileType.Hand.GetPile(Owner).Cards.Any(c =>
        {
            c.CanPlay(out var reason, out _);
            return reason.HasFlag(UnplayableReason.HasUnplayableKeyword);
        });
    }
}
