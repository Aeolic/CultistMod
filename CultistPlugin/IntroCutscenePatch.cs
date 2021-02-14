using System;
using HarmonyLib;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(IntroCutscene.CoBegin__d), nameof(IntroCutscene.CoBegin__d.MoveNext))]
    public class IntroCutscenePatch
    {
        private static String originalText = null;

        static bool Prefix(IntroCutscene.CoBegin__d __instance)
        {
            if (PlayerControl.LocalPlayer == InitialCultist)
            {
                var cultistTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                cultistTeam.Add(PlayerControl.LocalPlayer);
                __instance.yourTeam = cultistTeam;
            }

            return true;
        }

        static void Postfix(IntroCutscene.CoBegin__d __instance)
        {
            if (IsCultistUsed)
            {
                CultistMod.LastConversion = DateTime.UtcNow; //.AddSeconds(__instance.timer);

                if (PlayerControl.LocalPlayer == InitialCultist)
                {
                    __instance.__this.Title.Text = "Cultist";
                    __instance.__this.Title.Color = CultistMod.CultistColor;
                    __instance.__this.ImpostorText.Text = "Convert Crewmates to your Cult";
                    __instance.__this.BackgroundBar.material.color = CultistMod.CultistColor;
                }
                else if (!PlayerControl.LocalPlayer.Data.IsImpostor)
                {
                    if (originalText == null)
                    {
                        originalText = __instance.__this.ImpostorText.Text;
                    }

                    __instance.__this.ImpostorText.Text =
                        originalText + "\nThere is one " + "[6414C8FF]" + "Cultist" + "[]" + ".";
                }
            }
        }
    }
}