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

        public static CustomToggleOption CrewWinsWhenImpDead =
            CustomOption.AddToggle("Crewmates win on Impostor Death", true);

        public static CustomToggleOption ImpostorConversionAttemptUsesConversion =
            CustomOption.AddToggle("Impostor Conversion Attempts reduces Conversions", true);

        public static CustomToggleOption CultistsKnowEachOther =
            CustomOption.AddToggle("Cultists know who the other cultists are", false);

        public static CustomNumberOption CultistConversionCooldown =
            CustomOption.AddNumber("Cooldown between Conversions", 90f, 10f, 180f, 5f);

        public static CustomNumberOption CultistVisionModifier =
            CustomOption.AddNumber("Cultist Vision Modifier", 0.7f, 0.3f, 1.5f, 0.1f);

        public static CustomNumberOption
            CultistConversions = CustomOption.AddNumber("Cultist Conversions", 2, 1, 10, 1);

        public override void Load()
        {
            RegisterInIl2CppAttribute.Register();
            Harmony.PatchAll();
        }
    }
}