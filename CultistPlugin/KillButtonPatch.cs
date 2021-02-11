using System;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using static ExamplePlugin.CultistMod;

namespace ExamplePlugin
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class KillButtonPatch
    {
        public static void Postfix()
        {
            if (CultistSettings.InitialCultist != null)
            {
                if (CheckCultistWin())
                {
                    ExecuteCultistWin();
                }
            }
        }

        public static bool Prefix()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                return false;
            }

            if (CurrentTarget != null && CultistSettings.InitialCultist != null)
            {
                PlayerControl target = CurrentTarget;

                if (PlayerControl.LocalPlayer == CultistSettings.InitialCultist)
                {
                    CLog.Info("CULTIST TRYING TO CONVERT!");
                    CLog.Info("Target is Impostor:" + target.Data.IsImpostor);
                    CLog.Info("Target is Cultist:" + isCultist(target.PlayerId));

                    if (!target.Data.IsImpostor && !isCultist(target.PlayerId))
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                            PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.ConvertAction, Hazel.SendOption.None, -1);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        CultistList.Add(target.PlayerId);
                        
                        return false;
                    }
                }
            }

            return true;
        }
    }

}