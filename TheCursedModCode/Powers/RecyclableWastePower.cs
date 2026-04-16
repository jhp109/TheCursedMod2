using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TheCursedMod.TheCursedModCode.Cards;

namespace TheCursedMod.TheCursedModCode.Powers;

public class RecyclableWastePower : TheCursedModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Dregs>(false),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    internal void OnDregsCreated() => Flash();

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // 이미 패/덱/버림패에 있는 Dregs에도 Retain 추가
        var pcs = base.Owner.Player?.PlayerCombatState;
        if (pcs == null) return Task.CompletedTask;

        foreach (var card in pcs.AllCards)
        {
            if (card is Dregs)
                CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }

        return Task.CompletedTask;
    }
}
