using System;
using HarmonyLib;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(IntroCutscene.CoBegin__d), nameof(IntroCutscene.CoBegin__d.MoveNext))]
    public class IntroCutscenePatch
    {
        static void Postfix(IntroCutscene.CoBegin__d __instance)
        {
            
            CultistMod.LastConversion = DateTime.UtcNow.AddSeconds((CultistConversionCooldown * -1) + 30 + __instance.timer);
            
            if (PlayerControl.LocalPlayer == InitialCultist)
            {
                __instance.__this.Title.Text = "Cultist";
                __instance.__this.Title.Color = CultistMod.CultistColor;
                __instance.__this.ImpostorText.Text = "Recruit Crewmates to join your Cult";
                __instance.__this.BackgroundBar.material.color = CultistMod.CultistColor;
            }
        }
    }
}