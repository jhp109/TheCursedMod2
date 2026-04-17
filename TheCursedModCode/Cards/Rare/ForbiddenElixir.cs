using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheCursedMod.TheCursedModCode.Powers;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 금단의 영약(Forbidden Elixir) - 내 턴 시작 시, 에너지를 1 얻고 업보 3.
/// 강화 시 선천성 추가 및 매 턴 업보 획득량 3 → 2.
/// </summary>
public sealed class ForbiddenElixir() : TheCursedModCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new PowerVar<KarmaEveryTurnPower>("KarmaEveryTurn", 3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        EnergyHoverTip,
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Karma)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ForbiddenElixirPower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<KarmaEveryTurnPower>(Owner.Creature, DynamicVars["KarmaEveryTurn"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
        DynamicVars["KarmaEveryTurn"].UpgradeValueBy(-1m);
    }
}
