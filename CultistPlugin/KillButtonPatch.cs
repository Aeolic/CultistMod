using System;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class KillButtonPatch
    {
        public static void Postfix()
        {
            if (IsCultistUsed)
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

            if (IsCultistUsed && CurrentTarget != null)
            {
                PlayerControl target = CurrentTarget;

                if (PlayerControl.LocalPlayer == InitialCultist)
                {
                    bool createNewTask = false;
                    var player = PlayerControl.LocalPlayer;
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
                            if (CheckCultistWin())
                            {
                                ExecuteCultistWin();
                            }

                            createNewTask = true;
                        }
                    }

                    if (target.Data.IsImpostor && ImpostorConversionAttemptUsesConversion)
                    {
                        ConversionsLeft--;
                        createNewTask = true;
                    }

                    if (createNewTask)
                    {
                        ImportantTextTask cultLeaderTask =
                            new GameObject("CultistLeaderTask").AddComponent<ImportantTextTask>();
                        cultLeaderTask.transform.SetParent(player.transform, false);

                        cultLeaderTask.Text =
                            "You are the cult leader.\nConvert crewmates to join your cult.\nConversions left:" +
                            ConversionsLeft + "/" + MaxCultistConversions;
                        player.myTasks.Clear();
                        player.myTasks.Insert(0, cultLeaderTask);
                    }

                    return false;
                }

                return true;
            }

            return true;
        }
    }
}