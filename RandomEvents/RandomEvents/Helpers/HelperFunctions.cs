using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Bannerlord.RandomEvents.Helpers
{
    public static class HelperFunctions
    {
        public static string CountRandomEvents()
        {
            var randomEvents = new List<string>
            {
                "AFlirtatiousEncounter",
                "AheadOfTime",
                "ArmyGames",
                "ArmyInvite",
                "BeeKind",
                "BeggarBegging",
                "BetMoney",
                "BirdSong",
                "BirthdayParty",
                "BloodToll",
                "BottomsUp",
                "BumperCrops",
                "ChattingCommanders",
                "CompanionAdmire",
                "CostOfBetrayal",
                "Courier",
                "DiseasedCity",
                "DreadedSweats",
                "Duel",
                "Dysentery",
                "ExoticDrinks",
                "FallenSoldierFamily",
                "FantasticFighters",
                "FishingSpot",
                "FleeingFate",
                "FoodFight",
                "GranaryRats",
                "HotSprings",
                "HuntingTrip",
                "LightsInTheSky",
                "LoggingSite",
                "LookUp",
                "MassGrave",
                "Momentum",
                "NotOfThisWorld",
                "OldRuins",
                "PassingComet",
                "PerfectWeather",
                "PoisonedWine",
                "RedMoon",
                "Robbery",
                "SecretSinger",
                "SecretsOfSteel",
                "SpeedyRecovery",
                "SuccessfulDeeds",
                "SuddenStorm",
                "SupernaturalEncounter",
                "Travellers",
                "Undercooked",
                "UnexpectedWedding",
                "ViolatedGirl",
                "WanderingLivestock"
            };

            var numberOfEvents = randomEvents.Count.ToString();

            return numberOfEvents;
        }

        /// <summary>
        ///     Adds a random amount of experience points to a randomly selected skill.
        /// </summary>
        /// <param name="minRandomXP">The minimum XP that can be added to a skill.</param>
        /// <param name="maxRandomXP">The maximum XP that can be added to a skill.</param>
        public static void AssignRandomSkillXp(int minRandomXP, int maxRandomXP)
        {
            var skillList = new List<SkillObject>
            {
                DefaultSkills.Athletics,
                DefaultSkills.Bow,
                DefaultSkills.Charm,
                DefaultSkills.Crafting,
                DefaultSkills.Crossbow,
                DefaultSkills.Engineering,
                DefaultSkills.Leadership,
                DefaultSkills.Medicine,
                DefaultSkills.OneHanded,
                DefaultSkills.Polearm,
                DefaultSkills.Roguery,
                DefaultSkills.Steward,
                DefaultSkills.Tactics,
                DefaultSkills.Throwing,
                DefaultSkills.TwoHanded
            };

            var random = new Random();
            var randomIndex = random.Next(skillList.Count);

            var randomSkillName = skillList[randomIndex];

            var xpToAdd = MBRandom.RandomInt(minRandomXP, maxRandomXP);

            InformationManager.DisplayMessage(new InformationMessage($"Added {minRandomXP} XP to {randomSkillName}.",
                RandomEventsSubmodule.Msg_Color_POS_Outcome));

            Hero.MainHero.AddSkillXp(randomSkillName, xpToAdd);
        }
    }
}