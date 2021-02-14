using HarmonyLib;
using System;

//this class is copied from https://github.com/NotHunter101/ExtraRolesAmongUs and modified/shortened
namespace CultistPlugin
{
    public static class PlayerTools
    {
        public static PlayerControl closestPlayer = null;

        public static PlayerControl getPlayerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }

            return null;
        }

        public static float GetConversionCooldown()
        {
            if (CultistMod.LastConversion == null)
            {
                return CultistSettings.CultistConversionCooldown;
            }

            DateTime now = DateTime.UtcNow;
            TimeSpan diff = (TimeSpan) (now - CultistMod.LastConversion);

            var KillCoolDown = CultistSettings.CultistConversionCooldown * 1000.0f;
            if (KillCoolDown - (float) diff.TotalMilliseconds < 0) return 0;
            else
            {
                return (KillCoolDown - (float) diff.TotalMilliseconds) / 1000.0f;
            }
        }

        public static PlayerControl getClosestPlayer(PlayerControl refplayer)
        {
            double mindist = double.MaxValue;
            PlayerControl closestplayer = null;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead) continue;
                if (player != refplayer)
                {
                    double dist = getDistBetweenPlayers(player, refplayer);
                    if (dist < mindist)
                    {
                        mindist = dist;
                        closestplayer = player;
                    }
                }
            }

            return closestplayer;
        }

        public static double getDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var refpos = refplayer.GetTruePosition();
            var playerpos = player.GetTruePosition();

            return Math.Sqrt((refpos[0] - playerpos[0]) * (refpos[0] - playerpos[0]) +
                             (refpos[1] - playerpos[1]) * (refpos[1] - playerpos[1]));
        }
    }
}