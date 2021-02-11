using HarmonyLib;
using static ExamplePlugin.CultistMod;

namespace ExamplePlugin
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public class ShipStatusPatch
    {
        public static void Postfix(ref float __result, GameData.PlayerInfo IIEKJBMPELC)
        {
            if (isCultist(IIEKJBMPELC.PlayerId))
            {
                __result = __result * CultistSettings.CultistVisionModifier;
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
    public class CheckEndCriteriaPatch
    {
        public static bool Prefix()
        {
            if (DisableGameEndDuringMeeting)
            {
                return false;
            }

            return true;
        }
    }
}