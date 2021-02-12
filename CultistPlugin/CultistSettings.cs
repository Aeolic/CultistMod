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

        public static void SetCultistSettings()
        {
            IsCultistUsed = CultistPlugin.UseCultist.GetValue();
            CultistVisionModifier = CultistPlugin.CultistVisionModifier.GetValue();
            CultistConversionCooldown = CultistPlugin.CultistConversionCooldown.GetValue();
            MaxCultistConversions = (int) CultistPlugin.CultistConversions.GetValue();
            ImpostorConversionAttemptUsesConversion = CultistPlugin.ImpostorConversionAttemptUsesConversion.GetValue();
            DoCrewmatesWinWhenImpostorsAreDead = CultistPlugin.CrewWinsWhenImpDead.GetValue();
            CultistsKnowEachOther = CultistPlugin.CultistsKnowEachOther.GetValue();

            // CLog.Info("After Settings set:");
            // CLog.Info("IsUsed:" + IsCultistUsed);
            // CLog.Info("CultistVisionModifier " + CultistVisionModifier);
            // CLog.Info("CultistConversionCooldown " + CultistConversionCooldown);
            // CLog.Info("MaxCultistConversions " + MaxCultistConversions);
            // CLog.Info("ImpostorConversionAttemptUsesConversion " + ImpostorConversionAttemptUsesConversion);
            // CLog.Info("DoCrewmatesWinWhenImpostorsAreDead " + DoCrewmatesWinWhenImpostorsAreDead);
            // CLog.Info("CultistsKnowEachOther " + CultistsKnowEachOther);
        }
    }
}