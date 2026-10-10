using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace CheatsGuard.CampaignBehaviors
{
    // Passive daily XP for every hero currently in the player's main party.
    // Scope:
    //   * Player (Hero.MainHero) — always eligible when Config.EnablePassiveDailyXp
    //   * Companions in main party — eligible when Config.PassiveDailyXpIncludeCompanions
    //   * Non-player-clan heroes traveling with the party — treated as companions
    //
    // Amount is applied per skill: Config.PassiveDailyXpAmount is fed once per skill per hero
    // per day. E.g. amount=1000 with 18 vanilla skills grants 18000 XP total per eligible hero
    // per day, spread across every skill. Feels roughly like "1 skill per few days" at the
    // low-tier ranges and slows down naturally as the pow-curve scales.
    //
    // Registered from SubModule.OnGameStart via CampaignGameStarter.AddBehavior.
    public sealed class PassiveXpBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore) { }

        private static List<SkillObject> _skillCache;

        private static List<SkillObject> GetAllSkills()
        {
            if (_skillCache != null && _skillCache.Count > 0) return _skillCache;
            var mgr = MBObjectManager.Instance;
            if (mgr == null) return null;
            _skillCache = new List<SkillObject>(mgr.GetObjectTypeList<SkillObject>());
            return _skillCache;
        }

        private void OnDailyTick()
        {
            try
            {
                if (!Config.EnablePassiveDailyXp) return;
                if (Config.PassiveDailyXpAmount <= 0) return;

                var party = MobileParty.MainParty;
                if (party?.MemberRoster == null) return;

                var skills = GetAllSkills();
                if (skills == null || skills.Count == 0) return;

                int amount = Config.PassiveDailyXpAmount;
                bool includeCompanions = Config.PassiveDailyXpIncludeCompanions;

                int heroesTouched = 0;
                for (int i = 0; i < party.MemberRoster.Count; i++)
                {
                    TroopRosterElement elem = party.MemberRoster.GetElementCopyAtIndex(i);
                    CharacterObject chr = elem.Character;
                    if (chr == null || !chr.IsHero) continue;

                    Hero hero = chr.HeroObject;
                    if (hero == null) continue;

                    bool isPlayer = hero == Hero.MainHero;
                    if (!isPlayer && !includeCompanions) continue;

                    for (int s = 0; s < skills.Count; s++)
                    {
                        var skill = skills[s];
                        if (skill == null) continue;
                        hero.HeroDeveloper.AddSkillXp(skill, amount);
                    }
                    heroesTouched++;
                }

                if (heroesTouched > 0 && Config.VerboseLog)
                    LogFile.Write($"PassiveXpBehavior: granted {amount}xp × {skills.Count} skills to {heroesTouched} hero(es).");
            }
            catch (Exception ex)
            {
                LogFile.Write($"PassiveXpBehavior.OnDailyTick error: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
