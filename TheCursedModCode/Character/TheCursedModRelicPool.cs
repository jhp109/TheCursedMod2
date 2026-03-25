using BaseLib.Abstracts;
using TheCursedMod.TheCursedModCode.Extensions;
using Godot;

namespace TheCursedMod.TheCursedModCode.Character;

public class TheCursedModRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => TheCursedMod.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}