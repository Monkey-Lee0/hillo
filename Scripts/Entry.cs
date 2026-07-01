using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using hillo.Scripts.Epochs;

namespace hillo.Scripts
{
    [ModInitializer(nameof(Init))]
    public class Entry
    {
        public const string ModId = "hillo";

        public static void Init()
        {
            var harmony = new Harmony("hillo");
            harmony.PatchAll(); // 会自动扫描所有 HarmonyPatch 特性
        }
    }
}