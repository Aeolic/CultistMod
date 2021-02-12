using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.OxygenFilter;
using UnityEngine;
using Essentials.CustomOptions;
using Hazel;
using UnhollowerBaseLib;
using System.Collections.Generic;
using System.Linq;
using Reactor.Extensions;


namespace CultistPlugin
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class CultistPlugin : BasePlugin
    {
        public const string Id = "gg.reactor.cultistmod";

        public Harmony Harmony { get; } = new Harmony(Id);

        public static CustomToggleOption UseCultist = CustomOption.AddToggle("Play with Cultist", true);


        public override void Load()
        {
            RegisterInIl2CppAttribute.Register();
            Harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Vent), "CanUse")]
        public static class VentPatch
        {
            public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc,
                [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
            {
                PlayerControl localPlayer = pc.Object;
                canUse = true;
                couldUse = true;

                float num = Vector2.Distance(localPlayer.GetTruePosition(), __instance.transform.position);
                canUse &= num <= __instance.UsableDistance;

                __result = num;
                return false;
            }
        }
    }
}