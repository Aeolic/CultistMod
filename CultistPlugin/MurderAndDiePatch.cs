using System;
using HarmonyLib;
using Hazel;
using UnityEngine;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin

{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class DiePatch
    {
        public static void Prefix()
        {
            if (IsCultistUsed)
            {
                DisableGameEnd = true;
            }
        }

        public static void Postfix(PlayerControl __instance, DeathReason OECOPGMHMKC)
        {
            if (IsCultistUsed)
            {
                if (CheckCultistWin())
                {
                    CLog.Info("CULTIST WIN THROUGH DEATH!");

                    ExecuteCultistWin();
                }

                if (__instance.PlayerId == InitialCultist.PlayerId && CultistLeadIsPassedOnDeath)
                {
                    CLog.Info("CULTIST LEADER DIED, ASSIGNING NEW LEADER");
                    foreach (var player in PlayerControl.AllPlayerControls
                    ) //TODO use another way that guarantees the same order for all playersd
                    {
                        if (IsCultist(player.PlayerId) && player.PlayerId != InitialCultist.PlayerId && !player.Data.IsDead)
                        {
                            CLog.Info("NEW CULT LEADER:" + player.name);
                            InitialCultist = player;
                            LastConversion = DateTime.UtcNow;

                            player.myTasks.Clear();

                            ImportantTextTask convertedTask =
                                new GameObject("CultistLeaderTask").AddComponent<ImportantTextTask>();
                            convertedTask.transform.SetParent(player.transform, false);

                            convertedTask.Text =
                                "The cult leader died.\nYou are the new cult leader.\nConvert crewmates to your cult.\nConversions left:" +
                                ConversionsLeft + "/" + MaxCultistConversions;
                            player.myTasks.Insert(0, convertedTask);
                            break;
                        }
                    }
                }
            }

            DisableGameEnd = false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcMurderPlayer))]
    public class RpcMurderPatch
    {
        public static bool Prefix(PlayerControl CAKODNGLPDF)
        {
            if (!IsCultistUsed)
            {
                return true;
            }

            // swaps the execution of the original method, this is necessary, because otherwise the rightful winners are only shown for host when host is impostor
            MessageWriter messageWriter =
                AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 12, SendOption.Reliable,
                    -1);
            messageWriter.WritePacked(CAKODNGLPDF.NetId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            if (AmongUsClient.Instance.AmClient)
            {
                PlayerControl.LocalPlayer.MurderPlayer(CAKODNGLPDF);
            }

            return false;
        }
    }
}