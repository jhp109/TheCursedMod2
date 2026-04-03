using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 마도서 찢기(Tear Grimoire) - 비용이 2 이상인 공격 카드들이 한번 더 사용됩니다.
/// 내 턴 시작 시, 업보 13. 강화 시 비용 2.
/// </summary>
public sealed class TearGrimoire() : TheCursedModCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<KarmaEveryTurnPower>("KarmaPower", 13m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<TearGrimoirePower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<KarmaEveryTurnPower>(Owner.Creature, DynamicVars["KarmaPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
