using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 연막(Smokescreen) - 방어도를 9 얻습니다. 의례 : 모든 적에게 약화를 2 부여합니다.
/// (강화 시 방어도 11, 약화 3)
/// </summary>
public sealed class Smokescreen() : RiteCard(2, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(9, ValueProp.Move),
        new PowerVar<WeakPower>(2m)
    ];

    protected override async Task OnBaseEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnRiteEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(enemy, DynamicVars["WeakPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}
