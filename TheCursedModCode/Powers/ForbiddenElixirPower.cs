using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 금단의 영약 - 내 턴 시작 시, 에너지를 1 얻습니다.
/// </summary>
public class ForbiddenElixirPower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        Flash();
        await PlayerCmd.GainEnergy(1, player);
    }
}
