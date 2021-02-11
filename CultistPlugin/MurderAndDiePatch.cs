using HarmonyLib;
using Hazel;
using static ExamplePlugin.CultistMod;

namespace ExamplePlugin

{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class DiePatch
    {
        public static void Postfix(DeathReason OECOPGMHMKC)
        {
            if (CultistSettings.InitialCultist != null && !IsLastMurderFromCultistWin)
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

    // [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    // public static class MurderPlayerPatch
    // {
    //     public static void Postfix(PlayerControl __instance, PlayerControl CAKODNGLPDF)
    //     {
    //  
    //     }
    // }


    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcMurderPlayer))]
    public class RpcMurderPatch
    {
        public static bool Prefix(PlayerControl CAKODNGLPDF)
        {
            if (CultistSettings.InitialCultist == null) //TODO create overall bool cultistUsed
            {
                return true;
            }

            // swaps the execution of the original method, this is necessary, because otherwise the rightful winners are only shown for host, when host is impostor
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