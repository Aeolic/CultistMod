using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using Reactor.Unstrip;
using UnityEngine;
using StringBuilder = Il2CppSystem.Text.StringBuilder;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
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

        public static List<String>
            CultistNameList = new List<String>(); //necessary, because for win condition checks the Ids are unknown

        public static bool DidCultistsWin = false;
        public static bool IsLastMurderFromCultistWin = false;
        public static int ImpostorDummyCount = 0;
        public static bool DisableGameEndDuringMeeting = false;
        public static int ConversionsLeft;

        public static bool
            CrewmatesWinWhenImpostorsDead =
                true; //TODO implement this - obv cult wins if cult > non cult (e.g. 6 players, 1 imp, 3 cult 2 crew, imp gets voted)

        public static bool IsCultist(byte playerId)
        {
            return CultistList.IndexOf(playerId) != -1;
        }

        public static bool IsCultist(String playerName)
        {
            return CultistNameList.IndexOf(playerName) != -1;
        }

        public static class ModdedPalette
        {
            public static Color CultistColor = new Color(100f / 255f, 20f / 255f, 200f / 255f, 1);
        }

        public static void AddCultistToLists(PlayerControl playerControl)
        {
            if (!IsCultist(playerControl.PlayerId))
            {
                CultistList.Add(playerControl.PlayerId);
                CultistNameList.Add(playerControl.Data.PlayerName);
            }
        }

        public static void AddCultistToLists(byte playerId)
        {
            if (!IsCultist(playerId))
            {
                CultistList.Add(playerId);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == playerId)
                    {
                        CultistNameList.Add(player.Data.PlayerName);
                    }
                }
            }
        }

        public static void ClearCultistLists()
        {
            CultistList.Clear();
            CultistNameList.Clear();
        }


        public static bool CheckCultistWin()
        {
            CLog.Info("CULTISTS IN LISTS:");

            var x = "";
            foreach (var name in CultistNameList)
            {
                x += name + " ";
            }
            CLog.Info(x);
            
            
            int alive = 0;
            int cultists = 0;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead)
                {
                    alive++;
                    if (IsCultist(player.PlayerId))
                    {
                        cultists++;
                    }
                }
            }

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
                if (!IsCultist(player.PlayerId))
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

        // TODO try to get this to work instead of patching AppendTaskText
        // [RegisterInIl2Cpp]
        // public class CultTask : ImportantTextTask
        // {
        //     public CultTask(IntPtr ptr) : base(ptr)
        //     {
        //         CLog.Info("Creating CultTask");
        //     }
        // }
        
        public static void ClearCultistTasks() //TODO this works, but some error is thrown
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == null)
                {
                    return;
                }
                if (IsCultist(player.PlayerId))
                {
                    // var removeTask = new List<PlayerTask>();
                    // foreach (PlayerTask task in player.myTasks)
                    //     if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights &&
                    //         task.TaskType != TaskTypes.ResetReactor && task.TaskType != TaskTypes.ResetSeismic &&
                    //         task.TaskType != TaskTypes.RestoreOxy && task.name != "CultistTask" && task.name != "CultistLeaderTask")
                    //         removeTask.Add(task);
                    // foreach (PlayerTask task in removeTask)
                    // {
                    //     player.RemoveTask(task);
                    // }
                }
            }
        }

        [HarmonyPatch(typeof(ImportantTextTask), nameof(ImportantTextTask.AppendTaskText))]
        public static class TaskPatch
        {
            public static bool Prefix(ImportantTextTask __instance, StringBuilder DOJIEDCICAJ)
            {
                if (IsCultist(PlayerControl.LocalPlayer.PlayerId))
                {
                    DOJIEDCICAJ.AppendLine("[6414C8FF]" + __instance.Text + "[]");
                    return false;
                }

                return true;
            }
        }
    }
}