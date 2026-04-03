using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace TheCursedMod;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
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

        // STS2 v0.99.1: MockCardPool.GenerateAllCards() throws in production.
        // Patch it to return [] so CardModel.get_Pool() can continue searching.
        var mockPoolType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Models.CardPools.MockCardPool");
        if (mockPoolType != null)
        {
            var method = AccessTools.Method(mockPoolType, "GenerateAllCards");
            if (method != null)
                harmony.Patch(method, prefix: new HarmonyMethod(typeof(MainFile), nameof(MockCardPoolFix)));
        }
    }

    static bool MockCardPoolFix(ref CardModel[] __result)
    {
        __result = [];
        return false;
    }
}
