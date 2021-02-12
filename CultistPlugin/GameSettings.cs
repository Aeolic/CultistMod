namespace CultistPlugin
{
    public static class GameSettings
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
        }
    }
}