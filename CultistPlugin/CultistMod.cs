using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using Reactor.Unstrip;
using UnityEngine;

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

        //necessary, because for win condition checks the Ids are unknown
        public static List<String> CultistNameList = new List<String>();

        public static bool DidCultistsWin = false;
        
        //TODO swap dummy count for bool "hasDummy"
        public static int ImpostorDummyCount = 0;
        public static bool DisableGameEnd = false;
        public static int ConversionsLeft { get; set; }
        public static DateTime? LastConversion { get; set; }

        public static bool IsCultist(byte playerId)
        {
            return CultistList.IndexOf(playerId) != -1;
        }

        public static bool IsCultist(String playerName)
        {
            return CultistNameList.IndexOf(playerName) != -1;
        }


        public static Color CultistColor = new Color(100f / 255f, 20f / 255f, 200f / 255f, 1);

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
            
            CLog.Info("CHECK CULTIST WIN:");
            
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
                        CLog.Info("FOUND ALIVE CULTIST: " + player.name);
                    }
                }
            }

            if (ImpostorDummyCount > 0)
            {
                alive--;
            }

            if (alive / 2 < cultists)
            {
                CLog.Info("-----CULTIST WIN-----");
                return true;
            }
            else{CLog.Info("Cultists did not win.");}

            return false;
        }

        public static void ExecuteCultistWin()
        {
            CLog.Info("Executing WIN");
            DidCultistsWin = true;

            DisableGameEnd = true;

            if (ImpostorDummyCount > 0)
            {
                KillDummy();
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                // for each player check if cultist, if not kill
                if (!IsCultist(player.PlayerId))
                {
                    if (!player.Data.IsDead)
                    {
                        //player.MurderPlayer(player);
                        player.Data.IsDead = true;
                    }

                    player.RemoveInfected();
                    player.Data.IsImpostor = false;
                }
            }

            DisableGameEnd = false;
        }

        public static void KillDummy()
        {
            CLog.Info("Trying to Kill a Dummy.");
            PlayerControl playerToRemove = null;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.name == "IMPOSTOR_DUMMY")
                {
                    playerToRemove = player;
                }
            }

            if (playerToRemove != null)
            {
                CLog.Info("Killing dummy after receiving RPC command.");
                GameData.Instance.RemovePlayer(playerToRemove.PlayerId);
                PlayerControl.AllPlayerControls.Remove(playerToRemove);

                if (AmongUsClient.Instance.AmHost)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.KillDummy, Hazel.SendOption.None, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}