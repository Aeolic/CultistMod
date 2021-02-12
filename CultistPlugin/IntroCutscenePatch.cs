using HarmonyLib;
using static CultistPlugin.CultistMod;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(IntroCutscene.CoBegin__d), nameof(IntroCutscene.CoBegin__d.MoveNext))]
    public class IntroCutscenePatch
    {
        static void Postfix(IntroCutscene.CoBegin__d __instance)
        {
            if (PlayerControl.LocalPlayer == CultistSettings.InitialCultist)
            {
                __instance.__this.Title.Text = "Cultist";
                __instance.__this.Title.Color = CultistMod.ModdedPalette.CultistColor;
                __instance.__this.ImpostorText.Text = "Recruit Crewmates to join your Cult";
                __instance.__this.BackgroundBar.material.color = CultistMod.ModdedPalette.CultistColor;
            }
        }
    }
}