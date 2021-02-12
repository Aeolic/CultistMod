using System;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using static CultistPlugin.CultistMod;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class KillButtonPatch
    {
        public static void Postfix()
        {
            if (GameSettings.IsCultistUsed)
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

            if (GameSettings.IsCultistUsed && CurrentTarget != null)
            {
                PlayerControl target = CurrentTarget;

                if (PlayerControl.LocalPlayer == GameSettings.InitialCultist)
                {
                    CLog.Info("CULTIST TRYING TO CONVERT!");
                    CLog.Info("Target is Impostor:" + target.Data.IsImpostor);
                    CLog.Info("Target is Cultist:" + IsCultist(target.PlayerId));

                    if (!target.Data.IsImpostor && !IsCultist(target.PlayerId))
                    {
                        if (ConversionsLeft > 0)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                                PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.ConvertAction, Hazel.SendOption.None,
                                -1);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write(target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            AddCultistToLists(target);
                            ConversionsLeft--;

                        }
                      
                    }

                    if (target.Data.IsImpostor && GameSettings.ImpostorConversionAttemptUsesConversion)
                    {
                        ConversionsLeft--;
                    }
                    
                    return false;
                }
            }

            return true;
        }
    }
}