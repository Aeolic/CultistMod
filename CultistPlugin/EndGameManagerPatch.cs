using System.Collections.Generic;
using HarmonyLib;
using static ExamplePlugin.CultistMod;

namespace ExamplePlugin
{
    [HarmonyPatch(typeof(EndGameManager), "SetEverythingUp")]
    public class EndGameManagerPatch
    {
        public static void Prefix(EndGameManager __instance)
        {
            if (TempData.DidHumansWin(TempData.EndReason))
            {
                var toRemove = new List<WinningPlayerData>();
                
                foreach (var winner in TempData.winners)
                {
                    CLog.Info("WINNER:" + winner.Name);
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (winner.Name == player.name)
                        {
                            if (isCultist(player.PlayerId))
                            {
                                toRemove.Add(winner);
                            }
                        }
                    }
                }

                foreach (var winnerToRemove in toRemove)
                {
                    CLog.Info("Removing 1 Cultist!");
                    TempData.winners.Remove(winnerToRemove);
                }

                foreach (var winner in TempData.winners)
                {
                    CLog.Info("WINNER AFTER REMOVE:" + winner.Name);
                }

            }
        }

        public static void Postfix(EndGameManager __instance)
        {
            if (DidCultistsWin)
            {
                __instance.WinText.Color = ModdedPalette.CultistColor;
                __instance.BackgroundBar.material.color = ModdedPalette.CultistColor;


                if (isCultist(PlayerControl.LocalPlayer.PlayerId))
                {
                    __instance.WinText.Text = "Victory: The Cult";
                }
                else
                {
                    __instance.WinText.Text = "Defeat: The Cult recruited to many members.";
                }
            }
            else if (TempData.DidHumansWin(TempData.EndReason))
            {
                if (isCultist(PlayerControl.LocalPlayer.PlayerId))
                {
                    __instance.WinText.Color = Palette.ImpostorRed;
                    __instance.BackgroundBar.material.color = ModdedPalette.CultistColor;
                    __instance.WinText.Text = "Defeat: You did not convert enough members.";
                }
            }
        }
    }
}