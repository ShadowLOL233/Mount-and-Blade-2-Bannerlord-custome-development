namespace OSABalance.RBMBridge
{
    // Phase 0 smoke-test subset. Only high-confidence OSA IDs picked from
    // _scratch/OSA_phase0/*_candidates.json. Full classification is a Phase 1
    // follow-up (see PHASE0_RECON.md §2.3).
    //
    // Scheme per RBMCombat semantics (verified against RBMCombat.decompiled.cs):
    //   PlateOverMail      = Plate chest where shoulder/arm is visually mail →
    //                        RBM demotes shoulder material from Plate(4) to Chainmail(3)
    //   PlateBelowShoulder = Plate chest with full plate below shoulder →
    //                        RBM keeps below-shoulder on Plate curve (no demotion)
    //   ClosedFaceMail     = Helmet with closed face + mail aventail →
    //                        RBM gives face hit mail protection instead of none
    internal static class OSAIds
    {
        public static readonly string[] PlateOverMail = new[]
        {
            "AR_aserai_armor_f",                                // Southern Steel Scale Over Mail
            "AR_aserai_armor_f2",                               // Southern Brass Scale Over Mail
            "AR_aserai_armor_g",                                // Aserai Decorated Scale Cuirass With Chain Mail
            "ao_battanian_bronze_scale_over_tunic_with_kilt",
            "ao_battanian_iron_scale_over_tunic_with_kilt",
            "ao_durkhan_lamellar",
            "ao_durkhan_light_lamellar",
        };

        public static readonly string[] PlateBelowShoulder = new[]
        {
            "AR_khuzait_armor_a",                               // body=45 heavy plate
            "tv_battania_warlord_armor_b2",                     // body=38 warlord plate
        };

        public static readonly string[] ClosedFaceMail = new[]
        {
            "ao_imperial_cataphracts_closed_mail_helmet",
            "ao_imperial_cataphracts_plumed_closed_mail_helmet",
            "ao_imperial_closed_mail_nasal_spangenhelmet",
            "ao_imperial_plumed_cone_helmet_with_closed_mail",
            "ao_sturgian_feathered_helmet_with_closed_mail",
            "ao_eorling_guarded_nasal_helmet_with_closed_mail",
        };
    }
}
