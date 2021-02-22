using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem.Text;
using UnityEngine;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    //FIXME for the host (only for the host), a NRE gets thrown once (only in the second time the coroutine is executed)
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    public class TaskPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (IsCultistUsed && InitialCultist != null)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    //TODO create proper reusable methods for this
                    //TODO enable fake tasks for cultis
                    //TODO Or something interesting like ritual 
                    if (player.PlayerId == InitialCultist.PlayerId)
                    {
                        CLog.Info("Removing tasks for cultist:");
                        var tasksToRemove = new List<PlayerTask>();
                        foreach (var task in player.myTasks)
                        {
                            tasksToRemove.Add(task);
                        }

                        foreach (var taskToRemove in tasksToRemove)
                        {
                            player.RemoveTask(taskToRemove);
                        }

                        player.myTasks.Clear();
                        CLog.Info("Done with removing tasks");

                        ImportantTextTask convertedTask =
                            new GameObject("CultistLeaderTask").AddComponent<ImportantTextTask>();
                        convertedTask.transform.SetParent(player.transform, false);

                        convertedTask.Text =
                            "You are the cult leader.\nConvert crewmates to your cult.\nConversions left: " +
                            CultistMod.ConversionsLeft + "/" + MaxCultistConversions;
                        player.myTasks.Insert(0, convertedTask);
                    }
                }
            }
        }
    }


    [HarmonyPatch(typeof(ImportantTextTask), nameof(ImportantTextTask.AppendTaskText))]
    public static class ImportantTextTaskPatch
    {
        public static bool Prefix(ImportantTextTask __instance, StringBuilder DOJIEDCICAJ)
        {
            if (IsCultistUsed && CultistMod.IsCultist(PlayerControl.LocalPlayer.PlayerId))
            {
                DOJIEDCICAJ.AppendLine("[6414C8FF]" + __instance.Text + "[]");
                return false;
            }

            return true;
        }
    }
}