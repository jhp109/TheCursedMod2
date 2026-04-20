using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheCursedMod.TheCursedModCode.Cards;

/// <summary>
/// 공명하는 방패(Resonating Shield) - 방어도를 12 얻습니다.
/// 이번 전투에서 마법진의 효과가 발동된 횟수만큼 에너지 소모가 1 감소합니다. (강화 시 방어도 16)
/// </summary>
public sealed class ResonatingShield() : TheCursedModCard(5, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(12, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(TheCursedModCode.Keywords.Circle),
        EnergyHoverTip
    ];

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this) return Task.CompletedTask;
        if (Owner == null) return Task.CompletedTask;
        int pastTriggers = 0;
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard, PileType.Exhaust })
            foreach (var circle in pile.GetPile(Owner).Cards.OfType<CircleCard>())
                pastTriggers += circle.TheCursedMod_CircleTriggerCount;
        for (int i = 0; i < pastTriggers; i++)
            EnergyCost.AddThisCombat(-1);
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    /// <summary>
    /// 마법진 효과가 발동될 때마다 CircleCard.TriggerEffect에서 호출됩니다.
    /// </summary>
    public void OnCircleTrigger()
    {
        EnergyCost.AddThisCombat(-1);
        NCard.FindOnTable(this)?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
