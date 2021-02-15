using HarmonyLib;
using UnityEngine;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;


namespace CultistPlugin
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudUpdatePatch
    {
        static bool lastQ = false;

        //TODO there is some null ref exception thrown in this Postfix
        static void Postfix(HudManager __instance)
        {
            if (IsCultistUsed && __instance != null)
            {
                
                if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started && __instance.KillButton != null)
                {
                    if (!PlayerControl.LocalPlayer.Data.IsImpostor && Input.GetKeyDown(KeyCode.Q) && !lastQ)
                    {
                        KillButtonPatch.Prefix();
                    }

                    lastQ = Input.GetKeyUp(KeyCode.Q);
                    KillButton = __instance.KillButton;
                    PlayerTools.closestPlayer = PlayerTools.getClosestPlayer(PlayerControl.LocalPlayer);
                    DistLocalClosest =
                        PlayerTools.getDistBetweenPlayers(PlayerControl.LocalPlayer, PlayerTools.closestPlayer);

                    if (ConversionsLeft <= 0)
                    {
                        KillButton.gameObject.SetActive(false);
                        KillButton.isActive = false;
                    }

                    else if (InitialCultist.PlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        __instance.UseButton.isActiveAndEnabled)
                    {
                        KillButton.gameObject.SetActive(true);
                        KillButton.isActive = true;
                        KillButton.SetCoolDown(PlayerTools.GetConversionCooldown(),
                            PlayerControl.GameOptions.KillCooldown + 15.0f);
                        KillButton.renderer.sprite = convertIcon;
                        KillButton.renderer.color = Palette.EnabledColor;
                        KillButton.renderer.material.SetFloat("_Desat", 0f);

                        if (DistLocalClosest < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance])
                        {
                            KillButton.SetTarget(PlayerTools.closestPlayer);
                            CurrentTarget = PlayerTools.closestPlayer;
                        }
                        else
                        {
                            KillButton.SetTarget(null);
                            CurrentTarget = null;
                        }
                    }

                    // adds purple color during the game and meetings depending on settings
                    if (IsCultist(PlayerControl.LocalPlayer.PlayerId))
                    {
                        PlayerControl.LocalPlayer.nameText.Color = CultistColor;
                        var isLocalCultLeader = PlayerControl.LocalPlayer.PlayerId == InitialCultist.PlayerId;

                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            if (((CultistsKnowEachOther || isLocalCultLeader) && IsCultist(player.PlayerId)) ||
                                player.PlayerId == InitialCultist.PlayerId)
                            {
                                player.nameText.Color = CultistColor;
                            }
                        }

                        if (MeetingHud.Instance != null)
                        {
                            foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                            {
                                if ((player.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) ||
                                    (player.NameText != null && InitialCultist.PlayerId == player.TargetPlayerId) ||
                                    ((CultistsKnowEachOther || isLocalCultLeader) &&
                                     IsCultist((byte) player.TargetPlayerId)))
                                {
                                    player.NameText.Color = CultistColor;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_24))]
    class GameOptionsScalePatch
    {
        static void Postfix(ref string __result)
        {
            DestroyableSingleton<HudManager>.Instance.GameSettings.scale = 0.66f;
        }
    }
}