using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using UnhollowerBaseLib;
using UnityEngine;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public class SetInfectedPatch
    {
        public static void Postfix(Il2CppReferenceArray<GameData.PlayerInfo> JPGEIBIBJPJ)
        {
            List<PlayerControl> crewmates = PlayerControl.AllPlayerControls.ToArray().ToList();
            crewmates.RemoveAll(x => x.Data.IsImpostor);

            if (CultistPlugin.UseCultist.GetValue())
            {
                ClearCultistLists();
                DidCultistsWin = false;
                CLog.Info("Cultist is on for this game.");
                int cultistRandomId = new System.Random().Next(0, crewmates.Count);
                InitialCultist = crewmates[cultistRandomId];
                crewmates.RemoveAt(cultistRandomId);
                byte CultistId = InitialCultist.PlayerId;
                CLog.Info("Randomly chose cultist on host side: " + InitialCultist.name);

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.SetCultist, Hazel.SendOption.None, -1);
                writer.Write(CultistId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                AddCultistToLists(CultistId);
                
                SetCultistSettings();
                ConversionsLeft = MaxCultistConversions;
                LastConversion = null;

                if (PlayerControl.LocalPlayer.PlayerId == cultistRandomId)
                {
                    ImportantTextTask cultistLeaderTask =
                        new GameObject("CultistLeaderTask").AddComponent<ImportantTextTask>();
                    cultistLeaderTask.transform.SetParent(PlayerControl.LocalPlayer.transform, false);

                    cultistLeaderTask.Text =
                        "You are the cult leader.\nConvert crewmates to join your cult.\nConversions left: " +
                        ConversionsLeft + "/" + MaxCultistConversions;

                    PlayerControl.LocalPlayer.myTasks.Insert(0, cultistLeaderTask);
                }
            }
        }
    }
}