using System;
using System.Collections.Generic;
using HarmonyLib;
using Hazel;
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

            if (PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                return true;
            }

            if (IsCultistUsed && CurrentTarget != null && !PlayerControl.LocalPlayer.Data.IsImpostor)
            {
                PlayerControl target = CurrentTarget;

                if (PlayerControl.LocalPlayer == InitialCultist && PlayerTools.GetConversionCooldown() == 0)
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
                            LastConversion = DateTime.UtcNow;

                            if (CheckCultistWin())
                            {
                                ExecuteCultistWin();
                            }

                            var tasksToRemove = new List<PlayerTask>();
                            foreach (var task in target.myTasks)
                            {
                                if (task.TaskType != TaskTypes.FixComms && task.TaskType != TaskTypes.FixLights &&
                                    task.TaskType != TaskTypes.ResetReactor &&
                                    task.TaskType != TaskTypes.ResetSeismic && task.TaskType != TaskTypes.RestoreOxy)
                                    tasksToRemove.Add(task);
                            }

                            foreach (var taskToRemove in tasksToRemove)
                            {
                                player.RemoveTask(taskToRemove);
                            }

                            target.myTasks.Clear();
                            CLog.Info("Done with removing tasks for target!");

                            createNewTask = true;
                        }
                    }

                    if (target.Data.IsImpostor)
                    {
                        LastConversion = DateTime.UtcNow;
                        if (ImpostorConversionAttemptUsesConversion)
                        {
                            if (ConversionsLeft > 0)
                            {
                                ConversionsLeft--;
                                createNewTask = true;
                            }
                        }
                    }

                    if (createNewTask)
                    {
                        
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                            PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.ConversionsLeft, Hazel.SendOption.None,
                            -1);
                        writer.Write(ConversionsLeft);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        
                        
                        ImportantTextTask cultLeaderTask =
                            new GameObject("CultistLeaderTask").AddComponent<ImportantTextTask>();
                        cultLeaderTask.transform.SetParent(player.transform, false);

                        cultLeaderTask.Text =
                            "You are the cult leader.\nConvert crewmates to your cult.\nConversions left: " +
                            ConversionsLeft + "/" + MaxCultistConversions;
                        player.myTasks.Clear();
                        player.myTasks.Insert(0, cultLeaderTask);
                    }

                    return false;
                }
            }

            return true;
        }
    }
}