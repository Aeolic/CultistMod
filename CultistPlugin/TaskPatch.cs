using System.Text;
using HarmonyLib;
using UnityEngine;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    public class TaskPatch
    {
        public static void Postfix()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == CultistSettings.InitialCultist.PlayerId)
                {
                    CLog.Info("Removing tasks for cultist:");
                    for (int i = 0; i < player.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = player.myTasks[i];
                        CLog.Info(playerTask.name);
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    CLog.Info("Done with removing tasks");

                    ImportantTextTask convertedTask =
                        new GameObject("CultistLeaderTask").AddComponent<ImportantTextTask>();
                    convertedTask.transform.SetParent(player.transform, false);

                    convertedTask.Text =
                        "You are the cult leader.\nConvert crewmates to join your cult.\nConversions left:" +
                        CultistMod.ConversionsLeft + "/" + CultistSettings.MaxCultistConversions;
                    player.myTasks.Insert(0, convertedTask);
                }
            }
        }
    }


    [HarmonyPatch(typeof(ImportantTextTask), nameof(ImportantTextTask.AppendTaskText))]
    public static class ImportantTextTaskPatch
    {
        public static bool Prefix(ImportantTextTask __instance, StringBuilder DOJIEDCICAJ)
        {
            if (CultistMod.IsCultist(PlayerControl.LocalPlayer.PlayerId))
            {
                DOJIEDCICAJ.AppendLine("[6414C8FF]" + __instance.Text + "[]");
                return false;
            }

            return true;
        }
    }
}