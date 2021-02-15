using HarmonyLib;
using Il2CppSystem.Text;
using UnityEngine;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    public class TaskPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (IsCultistUsed)
            {
                int originalTaskCount = 0;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    //TODO create proper reusable methods for this
                    if (player.PlayerId == CultistSettings.InitialCultist.PlayerId)
                    {
                        CLog.Info("Removing tasks for cultist:");
                        originalTaskCount = player.myTasks.Count;
                        for (int i = 0; i < originalTaskCount; i++)
                        {
                            PlayerTask playerTask = player.myTasks[i];
                            player.RemoveTask(playerTask);
                        }

                        player.myTasks.Clear();
                        CLog.Info("Done with removing tasks");

                        ImportantTextTask convertedTask =
                            new GameObject("CultistLeaderTask").AddComponent<ImportantTextTask>();
                        convertedTask.transform.SetParent(player.transform, false);

                        convertedTask.Text =
                            "You are the cult leader.\nConvert crewmates to your cult.\nConversions left: " +
                            CultistMod.ConversionsLeft + "/" + CultistSettings.MaxCultistConversions;
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