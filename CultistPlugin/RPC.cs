using HarmonyLib;
using Hazel;
using UnityEngine;
using static CultistPlugin.CultistMod;
using static CultistPlugin.CultistSettings;

namespace CultistPlugin
{
    enum RPC
    {
        PlayAnimation = 0,
        CompleteTask = 1,
        SyncSettings = 2,
        SetInfected = 3,
        Exiled = 4,
        CheckName = 5,
        SetName = 6,
        CheckColor = 7,
        SetColor = 8,
        SetHat = 9,
        SetSkin = 10,
        ReportDeadBody = 11,
        MurderPlayer = 12,
        SendChat = 13,
        StartMeeting = 14,
        SetScanner = 15,
        SendChatNote = 16,
        SetPet = 17,
        SetStartCounter = 18,
        EnterVent = 19,
        ExitVent = 20,
        SnapTo = 21,
        Close = 22,
        VotingComplete = 23,
        CastVote = 24,
        ClearVote = 25,
        AddVote = 26,
        CloseDoorsOfType = 27,
        RepairSystem = 28,
        SetTasks = 29,
        UpdateGameData = 30,
    }

    enum CustomRPC
    {
        SetCultist = 58,
        ConvertAction = 59,
        KillDummy = 60
    }


    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class HandleRpcPatch
    {
        static void Prefix(byte HKHMBLJFLMC, MessageReader ALMCIJKELCP)
        {
            byte byteParam = HKHMBLJFLMC;
            MessageReader reader = ALMCIJKELCP;

            CLog.Info("Got RPC call: " + byteParam + " Msg Length: " + reader.Length);
        }

        static void Postfix(byte HKHMBLJFLMC, MessageReader ALMCIJKELCP)
        {
            byte packetId = HKHMBLJFLMC;
            MessageReader reader = ALMCIJKELCP;
            switch (packetId)
            {
                case ((byte) CustomRPC.SetCultist):
                    CLog.Info("Cultist Set Through RPC!");
                    ClearCultistLists();
                    DidCultistsWin = false;
                    byte CultistId = ALMCIJKELCP.ReadByte();
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == CultistId)
                        {
                            InitialCultist = player;
                            AddCultistToLists(player);
                            CLog.Info("SET PLAYER TO CULTIST " + player.name);
                        }
                    }

                    CLog.Info("Setting Cultist Settings");
                    SetCultistSettings();
                    ConversionsLeft = MaxCultistConversions;
                    LastConversion = null;

                    CLog.Info("Cultist list after cultist set through RPC:");

                    break;
                case ((byte) CustomRPC.ConvertAction):
                    CLog.Info("Conversion from RPC!");
                    byte CultistThatCalledId = ALMCIJKELCP.ReadByte();
                    byte TargetId = ALMCIJKELCP.ReadByte();

                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == TargetId && !IsCultist(player.PlayerId))
                        {
                            CLog.Info(player.Data.PlayerName + "is now a cultist.");
                            AddCultistToLists(player);

                            for (int i = 0; i < player.myTasks.Count; i++)
                            {
                                PlayerTask playerTask = player.myTasks[i];
                                player.RemoveTask(playerTask);
                            }

                            player.myTasks.Clear();

                            ImportantTextTask convertedTask =
                                new GameObject("CultistTask").AddComponent<ImportantTextTask>();
                            convertedTask.transform.SetParent(player.transform, false);

                            convertedTask.Text =
                                "You got converted to the cult.\nHelp your cult leader convert other crewmates.";

                            player.myTasks.Insert(0, convertedTask);
                        }
                    }

                    if (CheckCultistWin())
                    {
                        CLog.Info("Cultists won by conversion.");
                        ExecuteCultistWin();
                    }

                    break;
                case ((byte) CustomRPC.KillDummy):
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.name == "IMPOSTOR_DUMMY")
                        {
                            CLog.Info("Killing dummy after receiving RPC command.");
                            GameData.Instance.RemovePlayer(player.PlayerId);
                        }
                    }

                    break;
            }
        }
    }
}