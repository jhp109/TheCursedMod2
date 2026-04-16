using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using TheCursedMod.TheCursedModCode.Cards;
using TheCursedMod.TheCursedModCode.Character;

namespace TheCursedMod.TheCursedModCode.Relics;

[Pool(typeof(TheCursedModRelicPool))]
public sealed class SoulVesselRelic : TheCursedModRelic
{
    private const int HealPerTrigger = 2;
    private const int MaxHeal = 10;

    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool ShowCounter => true;
    public override int DisplayAmount => Math.Min(TriggersThisCombat * HealPerTrigger, MaxHeal);

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Rite),
    ];

    // 이번 전투 시작 시점의 누적 카운트를 기록해두고 차이로 이번 전투 발동 횟수를 계산
    private int _triggerCountAtCombatStart;

    private int TriggersThisCombat =>
        RiteCard.GetRiteEffectTriggerCount(base.Owner.Creature?.CombatState, base.Owner) - _triggerCountAtCombatStart;

    public void OnRiteEffectTriggered()
    {
        InvokeDisplayAmountChanged();
    }

    public override Task BeforeCombatStart()
    {
        _triggerCountAtCombatStart = RiteCard.GetRiteEffectTriggerCount(base.Owner.Creature?.CombatState, base.Owner);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        int heal = DisplayAmount;

        // heal 후 카운터 초기화: 다음 전투가 0부터 시작하도록 오프셋을 현재 누적값으로 갱신
        _triggerCountAtCombatStart = RiteCard.GetRiteEffectTriggerCount(base.Owner.Creature?.CombatState, base.Owner);
        InvokeDisplayAmountChanged();

        if (heal <= 0) return;

        Flash();
        if (base.Owner.Creature != null)
            await CreatureCmd.Heal(base.Owner.Creature, heal);
    }
}
