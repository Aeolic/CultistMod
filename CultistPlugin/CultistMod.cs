using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Reactor.Extensions;
using Reactor.Unstrip;
using UnityEngine;

namespace ExamplePlugin
{
    [HarmonyPatch]
    public static class CultistMod
    {
        public static AssetBundle bundle =
            AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + "\\Assets\\cultistbundle");

        public static Sprite convertIcon = bundle.LoadAsset<Sprite>("CO").DontUnload();
        public static KillButtonManager KillButton;
        public static double DistLocalClosest;
        public static PlayerControl CurrentTarget = null;

        public static List<byte> CultistList = new List<byte>();

        public static bool DidCultistsWin = false;
        public static bool IsLastMurderFromCultistWin = false;
        public static int ImpostorDummyCount = 0;
        public static bool DisableGameEndDuringMeeting = false;
        public static bool CrewmatesWinWhenImpostorsDead = true; //TODO implement this - obv cult wins if cult > non cult (e.g. 6 players, 1 imp, 3 cult 2 crew, imp gets voted)
        public static bool ImpostorConversionTryUsesConversion = true;

        public static bool IsCultistOn = true; //TODO change to setting

        public static bool isCultist(byte PlayerId)
        {
            return CultistList.IndexOf(PlayerId) != -1;
        }

        public static class ModdedPalette
        {
            public static Color CultistColor = new Color(100f / 255f, 20f / 255f, 200f / 255f, 1);
        }

        public static bool CheckCultistWin()
        {
            int alive = 0;
            int cultists = 0;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead)
                {
                    alive++;
                    if (isCultist(player.PlayerId))
                    {
                        cultists++;
                    }
                }
            }

            CLog.Info("Alive: " + alive + " Alive/2 " + (alive / 2) + " cultists: " + cultists);

            if (alive / 2 < cultists)
            {
                CLog.Info("-----CULTIST WIN-----");
                return true;
            }

            return false;
        }

        public static void ExecuteCultistWin()
        {
            DidCultistsWin = true;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                // for each player check if cultist, if not kill
                if (!isCultist(player.PlayerId))
                {
                    if (!player.Data.IsDead)
                    {
                        IsLastMurderFromCultistWin = true;
                        player.MurderPlayer(player);
                        player.Data.IsDead = true;
                    }

                    player.RemoveInfected();
                    player.Data.IsImpostor = false;
                }
                //
                else
                {
                    player.Data.IsImpostor = true;
                }
            }
        }

        public static void ClearCultistTasks()
        {
            foreach (var playerInfo in PlayerControl.AllPlayerControls)
            {
                if (isCultist(playerInfo.PlayerId))
                {
                    var removeTask = new List<PlayerTask>();
                    foreach (PlayerTask task in playerInfo.myTasks)
                        if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights &&
                            task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic &&
                            task.TaskType != TaskTypes.RestoreOxy)
                            removeTask.Add(task);
                    foreach (PlayerTask task in removeTask)
                    {
                        playerInfo.RemoveTask(task);
                    }
                }
            }
        }
    }
}