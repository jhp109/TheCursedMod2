using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 마나 요새(Mana Bastion) - 마법진의 효과가 발동될 때 마다, 방어도를 3 얻습니다.
/// 강화 시 방어도 2.
/// </summary>
public sealed class ManaBastion() : TheCursedModCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ManaBastionPower>("ManaBastionPower", 3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ManaBastionPower>(Owner.Creature, DynamicVars["ManaBastionPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ManaBastionPower"].UpgradeValueBy(1m);
    }
}
