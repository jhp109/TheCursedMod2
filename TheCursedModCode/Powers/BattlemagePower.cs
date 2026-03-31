using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheCursedMod.TheCursedModCode.Powers;

/// <summary>
/// 전투마법사 - 마법진의 효과가 발동될 때 마다, 활력을 Amount 얻습니다.
/// CircleCard.AfterCardPlayed에서 효과 발동 직후 VigorPower를 적용합니다.
/// </summary>
public class BattlemagePower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        HoverTipFactory.FromPower<VigorPower>()
    ];

    public void TriggerFlash() => Flash();
}
