using HarmonyLib;
using Hazel;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin

{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class DiePatch
    {
        public static void Postfix(DeathReason OECOPGMHMKC)
        {
            if (CultistSettings.IsCultistUsed && !IsLastMurderFromCultistWin)
            {
                if (CheckCultistWin())
                {
                    CLog.Info("CULTIST WIN THROUGH MURDER!");

                    ExecuteCultistWin();
                }
            }

            IsLastMurderFromCultistWin = false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcMurderPlayer))]
    public class RpcMurderPatch
    {
        public static bool Prefix(PlayerControl CAKODNGLPDF)
        {
            if (!IsCultistUsed)
            {
                return true;
            }

            // swaps the execution of the original method, this is necessary, because otherwise the rightful winners are only shown for host when host is impostor
            MessageWriter messageWriter =
                AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 12, SendOption.Reliable,
                    -1);
            messageWriter.WritePacked(CAKODNGLPDF.NetId);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            if (AmongUsClient.Instance.AmClient)
            {
                PlayerControl.LocalPlayer.MurderPlayer(CAKODNGLPDF);
            }

            return false;
        }
    }
}