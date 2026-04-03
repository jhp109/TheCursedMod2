using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 금단의 영약 - 내 턴 시작 시, 에너지를 1 얻습니다.
/// </summary>
public class ForbiddenElixirPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == base.Owner.Player)
        {
            Flash();
            await PlayerCmd.GainEnergy(Amount, player);
        }
    }
}
