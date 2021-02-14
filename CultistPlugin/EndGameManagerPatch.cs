using HarmonyLib;
using System.Collections.Generic;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(EndGameManager), "SetEverythingUp")]
    public class EndGameManagerPatch
    {
        public static void Prefix(EndGameManager __instance)
        {
            {
                if (DidCultistsWin)
                {
                    Il2CppSystem.Collections.Generic.List<WinningPlayerData> newWinners =
                        new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                    for (int i = 0; i < GameData.Instance.PlayerCount; i++)
                    {
                        GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
                        if (IsCultist(playerInfo.PlayerId))
                        {
                            newWinners.Add(new WinningPlayerData(playerInfo));
                        }
                    }

                    TempData.winners = newWinners;
                }

                if (TempData.DidHumansWin(TempData.EndReason) && !DidCultistsWin)
                {
                    var toRemove = new List<WinningPlayerData>();

                    foreach (var winner in TempData.winners)
                    {
                        if (IsCultist(winner.Name))
                        {
                            toRemove.Add(winner);
                        }
                    }

                    foreach (var winnerToRemove in toRemove)
                    {
                        TempData.winners.Remove(winnerToRemove);
                    }
                }
            }
        }

        public static void Postfix(EndGameManager __instance)
        {
            if (DidCultistsWin)
            {
                __instance.WinText.Color = CultistColor;
                __instance.BackgroundBar.material.color = CultistColor;

                //TODO make text color purple!


                if (IsCultist(PlayerControl.LocalPlayer.PlayerId))
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
                if (IsCultist(PlayerControl.LocalPlayer.PlayerId))
                {
                    __instance.WinText.Color = Palette.ImpostorRed;
                    __instance.BackgroundBar.material.color = CultistColor;
                    __instance.WinText.Text = "Defeat: You did not convert enough members.";
                }
            }

            InitialCultist = null;
        }
    }
}