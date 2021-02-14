using HarmonyLib;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public class ShipStatusPatch
    {
        public static void Postfix(ref float __result, GameData.PlayerInfo IIEKJBMPELC)
        {
            if ( IsCultistUsed && IsCultist(IIEKJBMPELC.PlayerId))
            {
                __result = __result * CultistVisionModifier;
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
    public class CheckEndCriteriaPatch
    {
        public static bool Prefix()
        {
            if (!IsCultistUsed)
            {
                return true;
            }

            return !DisableGameEnd;
        }
    }
}