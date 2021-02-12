using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using UnhollowerBaseLib;
using static CultistPlugin.CultistMod;

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
                CultistSettings.InitialCultist = crewmates[cultistRandomId];
                crewmates.RemoveAt(cultistRandomId);
                byte CultistId = CultistSettings.InitialCultist.PlayerId;

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.SetCultist, Hazel.SendOption.None, -1);
                writer.Write(CultistId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                AddCultistToLists(CultistId);
            }
        }
    }
}