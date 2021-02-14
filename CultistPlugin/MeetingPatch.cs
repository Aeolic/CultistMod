using System;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using static CultistPlugin.CultistSettings;
using static CultistPlugin.CultistMod;

namespace CultistPlugin
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Awake))]
    public class MeetingAwakePatch
    {
        static void Prefix()
        {
            if (IsCultistUsed)
            {
                if (ImpostorDummyCount > 0)
                {
                    DisableGameEnd = true;
                    KillDummy();
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.HandleProceed))]
    public class MeetingPatch
    {
        static void Postfix(MeetingHud __instance)
        {
            if (IsCultistUsed)
            {
                if (DoCrewmatesWinWhenImpostorsAreDead)
                {
                    return;
                }

                CLog.Info("In Postfix Meeting");
                if (AmongUsClient.Instance.AmHost)
                {
                    bool shouldCreateDummy = false;

                    if (__instance.exiledPlayer != null)
                    {
                        int amntAliveImpostors = 0;
                        int amntAliveCultists = 0;
                        int amntAliveCrewmates = 0;
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            if (!player.Data.IsDead)
                            {
                                if (player.Data.IsImpostor)
                                {
                                    amntAliveImpostors++;
                                }
                                else if (IsCultist(player.PlayerId))
                                {
                                    amntAliveCultists++;
                                }
                                else
                                {
                                    amntAliveCrewmates++;
                                }
                            }
                        }

                        CLog.Info("Alive Impostors: " + amntAliveImpostors + " amnt alive cultists: " +
                                  amntAliveCultists);
                        if (!IsCultist(__instance.exiledPlayer.PlayerId) &&
                            (amntAliveCultists > (amntAliveCrewmates + amntAliveImpostors - 1)))
                        {
                            CLog.Info("CULTISTS WIN BY VOTING OFF, not creating dummy.!");
                            return;
                        }

                        if (IsCultist(__instance.exiledPlayer.PlayerId) && amntAliveCrewmates > 0 &&
                            amntAliveCultists <= 1 && amntAliveImpostors == 0)
                        {
                            CLog.Info("Crewmates WIN BY VOTING last Cultists OFF, not creating dummy.!");
                            return;
                        }

                        //if last impostor just got kicked, might need to create dummy if cultists left in the game
                        if (amntAliveImpostors == 1 && amntAliveCultists > 0 && __instance.exiledPlayer.IsImpostor)
                        {
                            shouldCreateDummy = true;
                        }
                    }
                    // if there was an dummy already we need a new one, if the voting ended in a skip
                    if (ImpostorDummyCount > 0)
                    {
                        shouldCreateDummy = true;
                    }

                    if (shouldCreateDummy)
                    {
                        CLog.Info("Creating Dummy Impostor");
                        ImpostorDummyCount++;
                        var gameObject = new GameObject(nameof(ImpostorDummy)).DontDestroy();
                        var dummy = gameObject.AddComponent<ImpostorDummy>();
                    }
                }

                CLog.Info("End of postfix!");
                DisableGameEnd = false;
            }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    class ExileControllerPatch
    {
        static void Postfix(ExileController __instance)
        {
            if (IsCultistUsed)
            {
                LastConversion = DateTime.UtcNow.AddMilliseconds(__instance.Duration);
                if (CheckCultistWin())
                {
                    ExecuteCultistWin();
                }
            }
        }
    }


    [RegisterInIl2Cpp]
    public class ImpostorDummy : MonoBehaviour
    {
        public ImpostorDummy(IntPtr ptr) : base(ptr)
        {
            var playerControl = Instantiate(AmongUsClient.Instance.PlayerPrefab);
            var i = playerControl.PlayerId = (byte) GameData.Instance.GetAvailableId();
            GameData.Instance.AddPlayer(playerControl);
            AmongUsClient.Instance.Spawn(playerControl, -2, SpawnFlags.None);
            playerControl.transform.position = PlayerControl.LocalPlayer.transform.position; //TODO maybe spawn outside?
            playerControl.GetComponent<DummyBehaviour>().enabled = true;
            playerControl.NetTransform.enabled = false;
            playerControl.SetName("IMPOSTOR_DUMMY");
            playerControl.SetColor((byte) (i % Palette.PlayerColors.Length));
            playerControl.Data.IsImpostor = true;
            playerControl.GetComponent<DummyBehaviour>().transform.localScale = new Vector3(0, 0, 0);
        }
    }
}