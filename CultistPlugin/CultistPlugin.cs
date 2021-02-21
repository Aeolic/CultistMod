using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Essentials.CustomOptions;


namespace CultistPlugin
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class CultistPlugin : BasePlugin
    {
        public const string Id = "gg.reactor.cultistmod";
        public const string ModVersion = "1.0.2";
        public Harmony Harmony { get; } = new Harmony(Id);

        //I am using Reactor Essentials by DorCoMaNdO for these settings
        // https://github.com/DorCoMaNdO/Reactor-Essentials
        // I'm using a Essentials version without the watermark
        // but I am giving credit here and in the Credits Tab in the readme
        // If there is any Problem with this, hit me up on Discord @ Aeolic#5560

        public static CustomToggleOption UseCultist = CustomOption.AddToggle("Enable Cultist", true);

        public static CustomToggleOption CrewWinsWhenImpDead =
            CustomOption.AddToggle("Crew Wins VS Cult", true);

        public static CustomToggleOption ImpostorConversionAttemptUsesConversion =
            CustomOption.AddToggle("Fail Conversion Uses Attempt", true);

        public static CustomToggleOption CultistsKnowEachOther =
            CustomOption.AddToggle("Cult Knows Who Is Cult", false);

        public static CustomToggleOption CultistLeadIsPassedOnDeath =
            CustomOption.AddToggle("Pass Cult Lead On Death", true);

        public static CustomNumberOption
            CultistConversions = CustomOption.AddNumber("Cultist Conversions", 3, 1, 10, 1);

        public static CustomNumberOption CultistConversionCooldown =
            CustomOption.AddNumber("Conversion Cooldown", 60f, 10f, 180f, 5f);

        public static CustomNumberOption CultistVisionModifier =
            CustomOption.AddNumber("Cultist Vision Modifier", 0.7f, 0.3f, 1.5f, 0.1f);

        public override void Load()
        {
            RegisterInIl2CppAttribute.Register();
            Harmony.PatchAll();
        }
    }
}