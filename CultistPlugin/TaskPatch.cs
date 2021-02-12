using HarmonyLib;
using Il2CppSystem.Text;
using UnhollowerBaseLib;
using UnityEngine;

namespace CultistPlugin
{
    // [HarmonyPatch(typeof(GameData), nameof(GameData))]
    // public class GameDataTaskPatch
    // {
    //     public static bool Prefix(ref byte NHOCGFDHKKK, ref Il2CppStructArray<byte> NHAFLOEMFKF)
    //     {
    //         CLog.Info("In RPC SET TASKS:");
    //         if (NHOCGFDHKKK == CultistSettings.InitialCultist.PlayerId)
    //         {
    //             CLog.Info("FOUND INITIAL CULTIST SETTING TASK to 0");
    //             NHAFLOEMFKF = new Il2CppStructArray<byte>(0);
    //         }
    //
    //         return true;
    //     }
    // }


    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
    public class TaskPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            int originalTaskCount = 0;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
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