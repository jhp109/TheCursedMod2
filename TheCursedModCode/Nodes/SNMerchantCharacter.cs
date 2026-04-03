using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace TheCursedMod.TheCursedModCode.Nodes;

[GlobalClass]
public partial class SNMerchantCharacter : NMerchantCharacter
{
	public override void _Ready()
	{
		// Do not call base._Ready() — it assumes a Spine sprite as GetChild(0),
		// but we use a static TextureRect instead.
	}
}
