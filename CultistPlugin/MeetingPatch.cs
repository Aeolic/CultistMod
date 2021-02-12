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
            CLog.Info("In Meeting Awake prefix");
            CLog.Info("ImpostorDummyCount: " + ImpostorDummyCount);
            if (ImpostorDummyCount > 0)
            {
                CLog.Info("Found a Dummy.");
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    CLog.Info(player.name + " is Impostor: " + player.Data.IsImpostor);

                    if (player.name == "IMPOSTOR_DUMMY")
                    {
                        CLog.Info("Killing the dummy!");
                        DisableGameEndDuringMeeting = true;
                        //player.Die(DeathReason.Disconnect);
                        GameData.Instance.RemovePlayer(player.PlayerId);

                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                            PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.KillDummy, Hazel.SendOption.None, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
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
                    CLog.Info("Instance was not null (THIS SHOULD ONLY BE REACHED FOR HOST!");
                    bool shouldCreateDummy = false;

                    if (__instance.exiledPlayer != null)
                    {
                        int amntAliveImpostors = 0;
                        int amntAliveCultists = 0;
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
                            }
                        }

                        CLog.Info("Alive Impostors: " + amntAliveImpostors + " amnt alive cultists: " +
                                  amntAliveCultists);
                        //if last impostor just got kicked, might need to create dummy if cultists left in the game
                        if (amntAliveImpostors == 1 && amntAliveCultists > 0 && __instance.exiledPlayer.IsImpostor)
                        {
                            shouldCreateDummy = true;
                        }

                        // if the last Cultist is kicked, we dont need an dummy
                        if (amntAliveCultists <= 1 && IsCultist(__instance.exiledPlayer.PlayerId))
                        {
                            shouldCreateDummy = false;
                        }
                    }
                    // if there was an dummy already we need a new one, if the voting ended in a skip
                    else if (ImpostorDummyCount > 0)
                    {
                        shouldCreateDummy = true;
                    }

                    if (shouldCreateDummy)
                    {
                        CLog.Info("Creating Dummy Impostor");
                        ImpostorDummyCount++;
                        var gameObject = new GameObject(nameof(ImpostorDummy)).DontDestroy();
                        gameObject.AddComponent<ImpostorDummy>();
                    }
                }

                CLog.Info("End of postfix!");
                DisableGameEndDuringMeeting = false;
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
            playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
            playerControl.GetComponent<DummyBehaviour>().enabled = true;
            playerControl.GetComponent<DummyBehaviour>().GetComponent<Renderer>().enabled = false;
            playerControl.NetTransform.enabled = false;
            playerControl.SetName("IMPOSTOR_DUMMY");
            playerControl.SetColor((byte) (i % Palette.PlayerColors.Length));
            playerControl.Data.IsImpostor = true;
            //playerControl.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}