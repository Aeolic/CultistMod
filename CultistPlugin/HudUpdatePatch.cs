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

        static void Postfix(HudManager __instance)
        {
            if (IsCultistUsed)
            {
                if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
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

                    if (InitialCultist.PlayerId == PlayerControl.LocalPlayer.PlayerId &&
                        __instance.UseButton.isActiveAndEnabled)
                    {
                        KillButton.gameObject.SetActive(true);
                        KillButton.isActive = true;
                        KillButton.SetCoolDown(0f, PlayerControl.GameOptions.KillCooldown + 15.0f);
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