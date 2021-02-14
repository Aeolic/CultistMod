namespace CultistPlugin
{
    public static class CultistSettings
    {
        public static PlayerControl InitialCultist { get; set; }
        public static bool IsCultistUsed;
        public static float CultistVisionModifier;
        public static float CultistConversionCooldown;
        public static int MaxCultistConversions;
        public static bool ImpostorConversionAttemptUsesConversion;
        public static bool DoCrewmatesWinWhenImpostorsAreDead;
        public static bool CultistsKnowEachOther;
        public static bool CultistLeadIsPassedOnDeath;

        public static void SetCultistSettings()
        {
            IsCultistUsed = CultistPlugin.UseCultist.GetValue();
            CultistVisionModifier = CultistPlugin.CultistVisionModifier.GetValue();
            //TODO USE AFTER TESTING CultistConversionCooldown = CultistPlugin.CultistConversionCooldown.GetValue();
            CultistConversionCooldown = 1;
            MaxCultistConversions = (int) CultistPlugin.CultistConversions.GetValue();
            ImpostorConversionAttemptUsesConversion = CultistPlugin.ImpostorConversionAttemptUsesConversion.GetValue();
            DoCrewmatesWinWhenImpostorsAreDead = CultistPlugin.CrewWinsWhenImpDead.GetValue();
            CultistsKnowEachOther = CultistPlugin.CultistsKnowEachOther.GetValue();
            CultistLeadIsPassedOnDeath = CultistPlugin.CultistLeadIsPassedOnDeath.GetValue();

            CultistMod.ConversionsLeft = MaxCultistConversions;
            CultistMod.LastConversion = null;
            CultistMod.ImpostorDummyCount = 0;
        }
    }
}