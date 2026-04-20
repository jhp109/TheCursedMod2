using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace TheCursedMod;

[ModInitializer(nameof(Initialize))]
public partial class TheCursedModMainFile : Node
{
    public const string ModId = "TheCursedMod"; //Used for resource filepath

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        // Register mod C# types with Godot so .tscn scripts can find them
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);

        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
    static bool MockCardPoolFix(ref CardModel[] __result)
    {
        __result = [];
        return false;
    }
}
