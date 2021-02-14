using HarmonyLib;

namespace CultistPlugin
{
    public class OtherPatches
    {
        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionStartPatch
        {
            static void Postfix(VersionShower __instance)
            {
                __instance.text.Text = __instance.text.Text + "\n\n\n\n\n\n\n\nCultist Mod " + CultistPlugin.ModVersion +
                                       " by Aeolic\nhttps://github.com/Aeolic/CultistMod";
            }
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
        public static class AmBannedPatch
        {
            public static void Postfix(out bool __result)
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingPatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.Text += "\nCultist Mod " + CultistPlugin.ModVersion;
            }
        }
    }
}