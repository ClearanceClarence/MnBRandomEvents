using System;
using System.Collections.Generic;
using System.Windows;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class HuntingTrip : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minSoldiersToGo;
        private readonly int maxSoldiersToGo;
        private readonly int maxCatch;
        private readonly int minMoraleGain;
        private readonly int maxMoraleGain;
        private readonly int minYieldMultiplier;
        private readonly int maxYieldMultiplier;
        

        public HuntingTrip() : base(ModSettings.RandomEvents.HuntingTripData)
        {
            
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("HuntingTrip", "EventDisabled");
            minSoldiersToGo = ConfigFile.ReadInteger("HuntingTrip", "MinSoldiersToGo");
            maxSoldiersToGo = ConfigFile.ReadInteger("HuntingTrip", "MaxSoldiersToGo");
            maxCatch = ConfigFile.ReadInteger("HuntingTrip", "MaxCatch");
            minMoraleGain = ConfigFile.ReadInteger("HuntingTrip", "MinMoraleGain");
            maxMoraleGain = ConfigFile.ReadInteger("HuntingTrip", "MaxMoraleGain");
            minYieldMultiplier = ConfigFile.ReadInteger("HuntingTrip", "MinYieldMultiplier");
            maxYieldMultiplier = ConfigFile.ReadInteger("HuntingTrip", "MaxYieldMultiplier");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minSoldiersToGo != 0 || maxSoldiersToGo != 0 || maxCatch != 0 || minMoraleGain != 0 || maxMoraleGain != 0 || minYieldMultiplier != 0 || maxYieldMultiplier != 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.MemberRoster.TotalRegulars >= maxSoldiersToGo;
        }

        public override void StartEvent()
        {

            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
            
            var yieldMultiplier = MBRandom.RandomInt(minYieldMultiplier, maxYieldMultiplier);

            var soldiersGoneHunting = MBRandom.RandomInt(minSoldiersToGo, maxSoldiersToGo);
            var animalsCaught = MBRandom.RandomInt(0, maxCatch);
            
            var yieldedMeatResources = animalsCaught * yieldMultiplier;

            var moraleGained = MBRandom.RandomInt(minMoraleGain, maxMoraleGain);
            
            var meat = MBObjectManager.Instance.GetObject<ItemObject>("meat");
            var hides = MBObjectManager.Instance.GetObject<ItemObject>("hides");
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("soldiersGoneHunting", soldiersGoneHunting)
                .ToString();
            
            var eventOutcome1 = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .ToString();
            
            var eventOutcome2 = new TextObject(EventTextHandler.GetRandomEventOutcome2())
                .SetTextVariable("animalsCaught",animalsCaught)
                .SetTextVariable("yieldedMeatResources",yieldedMeatResources)
                .ToString();
            
            var eventOutcome3 = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .SetTextVariable("animalsCaught",animalsCaught)
                .SetTextVariable("yieldedMeatResources",yieldedMeatResources)
                .ToString();
            
            var eventOutcome4 = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .SetTextVariable("animalsCaught",animalsCaught)
                .SetTextVariable("yieldedMeatResources",yieldedMeatResources)
                .ToString();
            
            var eventButtonText = new TextObject("{=HuntingTrip_Event_Button_Text}Continue")
                .ToString();
            
            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("moraleGained", moraleGained - 3)
                .ToString();
            
            var eventMsg2 =new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("animalsCaught", animalsCaught)
                .SetTextVariable("yieldedMeatResources", yieldedMeatResources)
                .SetTextVariable("moraleGained", moraleGained - 2)
                .ToString();
            
            var eventMsg3 =new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("animalsCaught", animalsCaught)
                .SetTextVariable("yieldedMeatResources", yieldedMeatResources)
                .SetTextVariable("moraleGained", moraleGained - 1)
                .ToString();
            
            var eventMsg4 =new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("animalsCaught", animalsCaught)
                .SetTextVariable("yieldedMeatResources", yieldedMeatResources)
                .SetTextVariable("moraleGained", moraleGained)
                .ToString();
            

            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventDescription, true, false, eventButtonText, null, null, null), true);
            
            MobileParty.MainParty.ItemRoster.AddToCounts(meat, yieldedMeatResources);
            MobileParty.MainParty.ItemRoster.AddToCounts(hides, animalsCaught);

            if (animalsCaught == 0)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome1, true, false, eventButtonText, null, null, null), true);
                
                MobileParty.MainParty.RecentEventsMorale += moraleGained - 3;
                MobileParty.MainParty.MoraleExplained.Add(moraleGained);
                
                InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_MED_Outcome));
            }
            else if (animalsCaught > 0 && animalsCaught <= 5)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome2, true, false, eventButtonText, null, null, null), true);
                
                MobileParty.MainParty.RecentEventsMorale += moraleGained - 2;
                MobileParty.MainParty.MoraleExplained.Add(moraleGained);
                
                InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_POS_Outcome));
            }
            else if (animalsCaught > 5 && animalsCaught <= 15)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome3, true, false, eventButtonText, null, null, null), true);
                
                MobileParty.MainParty.RecentEventsMorale += moraleGained - 1;
                MobileParty.MainParty.MoraleExplained.Add(moraleGained);
                
                InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_POS_Outcome));
            }
            else if (animalsCaught > 15 && animalsCaught <= maxCatch)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome4, true, false, eventButtonText, null, null, null), true);
                
                MobileParty.MainParty.RecentEventsMorale += moraleGained;
                MobileParty.MainParty.MoraleExplained.Add(moraleGained);
                
                InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_POS_Outcome));
            }
            

            StopEvent();
        }

        private void StopEvent()
        {
            try
            {
                onEventCompleted.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }
        
        private static class EventTextHandler
        {
            private static readonly Random random = new Random();
            
            private static readonly List<string> eventTitles = new List<string>
            {
                "{=HuntingTrip_Title_A}The Great Hunt",
                "{=HuntingTrip_Title_B}The Thrill of the Hunt",
                "{=HuntingTrip_Title_C}A Noble Hunting Expedition",
                "{=HuntingTrip_Title_D}The Call of the Wild",
                "{=HuntingTrip_Title_E}A Grand Hunting Party",
                "{=HuntingTrip_Title_F}A Dangerous Hunting Quest",
                "{=HuntingTrip_Title_G}Tracking the Prey",
                "{=HuntingTrip_Title_H}The Hunt for Glory",
                "{=HuntingTrip_Title_I}A Test of Skill and Patience",
                "{=HuntingTrip_Title_J}The Bounty of the Forest",
                "{=HuntingTrip_Title_K}A Legendary Hunting Trip"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=HuntingTrip_Event_Desc_A}While camping, {soldiersGoneHunting} of your men decide they want to go into the " +
                "forest just west of camp to try hunting. You could use the additional resources and it would be a great morale " +
                "booster for the party if they catch some. You tell them to be back before nightfall.",
                
                //Event Description B
                "{=HuntingTrip_Event_Desc_B}A group of {soldiersGoneHunting} soldiers approaches you, asking for permission to " +
                "hunt in the nearby woods. They believe it will provide food and boost morale. You agree but remind them to return " +
                "before dark.",
                
                //Event Description C
                "{=HuntingTrip_Event_Desc_C}Your men notice signs of game near the camp and {soldiersGoneHunting} volunteers ask " +
                "to head out and hunt. It could yield valuable supplies, so you allow them to go with instructions to be cautious " +
                "and return by nightfall.",
                
                //Event Description D
                "{=HuntingTrip_Event_Desc_D}Some of your men suggest hunting in the nearby forest to gather resources and take a " +
                "break from the routine. {soldiersGoneHunting} of them set out after you approve, with a warning to stay safe " +
                "and come back before night.",
                
                //Event Description E
                "{=HuntingTrip_Event_Desc_E}Your camp is surrounded by dense forest, and {soldiersGoneHunting} of your soldiers " +
                "express interest in hunting. They promise to be quick and careful, and you permit them to go, expecting " +
                "them back by nightfall."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Outcome 1A
                "{=HuntingTrip_Outcome_1A}Your men return empty handed just before nightfall. At least they had a good time together.",
                
                //Event Outcome 1B
                "{=HuntingTrip_Outcome_1B}Despite their efforts, your men return without any game. They seem slightly disappointed " +
                "but enjoyed the experience.",
                
                //Event Outcome 1C
                "{=HuntingTrip_Outcome_1C}The hunters come back empty-handed, laughing about the ones that got away. At least morale " +
                "remains intact.",
                
                //Event Outcome 1D
                "{=HuntingTrip_Outcome_1D}Your men return with no success, though they share amusing stories of their failed attempts.",
                
                //Event Outcome 1E
                "{=HuntingTrip_Outcome_1E}No game was caught, but your men return with smiles, claiming it was a good bonding experience."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Event Outcome 2A
                "{=HuntingTrip_Outcome_2A}Your hunters return just before nightfall, having successfully caught {animalsCaught} animals, yielding {animalsCaught} hides " +
                "and {yieldedMeatResources} pieces of meat. Better than nothing. You let the hunters finish butchering the animals.",
                
                //Event Outcome 2B
                "{=HuntingTrip_Outcome_2B}Your men manage to bring back {animalsCaught} small animals. This yields {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. It’s not much, but it’s something to boost the supplies.",
                
                //Event Outcome 2C
                "{=HuntingTrip_Outcome_2C}The hunting party returns with {animalsCaught} animals, providing {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. It’s a modest haul, but it will help sustain the camp.",
                
                //Event Outcome 2D
                "{=HuntingTrip_Outcome_2D}With {animalsCaught} animals in hand, your hunters have something to show for their effort. They bring back {animalsCaught} hides " +
                "and {yieldedMeatResources} pieces of meat.",
                
                //Event Outcome 2E
                "{=HuntingTrip_Outcome_2E}Your men return with {animalsCaught} small game animals, which provide {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. A small but appreciated contribution."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Event Outcome 3A
                "{=HuntingTrip_Outcome_3A}Your hunters return just before nightfall, having successfully caught {animalsCaught} animals, yielding {animalsCaught} hides " +
                "and {yieldedMeatResources} pieces of meat. You join the hunters in storing the meat.",
                
                //Event Outcome 3B
                "{=HuntingTrip_Outcome_3B}The hunting party brings back {animalsCaught} animals, providing {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. It’s a decent haul that will help sustain the camp for a while.",
                
                //Event Outcome 3C
                "{=HuntingTrip_Outcome_3C}Your men return with {animalsCaught} animals, yielding {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. The camp is pleased with this successful outing.",
                
                //Event Outcome 3D
                "{=HuntingTrip_Outcome_3D}With {animalsCaught} animals, your hunters return proudly, having gathered {animalsCaught} hides and " +
                "{yieldedMeatResources} pieces of meat. It’s a worthwhile contribution to the camp's supplies.",
                
                //Event Outcome 3E
                "{=HuntingTrip_Outcome_3E}The hunters arrive back with {animalsCaught} animals, yielding {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. A satisfying result for their efforts."
            };
            
            private static readonly List<string> EventOutcome4= new List<string>
            {
                //Event Outcome 4A
                "{=HuntingTrip_Outcome_4A}Your hunters return triumphantly just before nightfall, having successfully caught {animalsCaught} animals, yielding {animalsCaught} " +
                "hides and {yieldedMeatResources} pieces of meat. You order your men to start preparing a feast for everyone.",
                
                //Event Outcome 4B
                "{=HuntingTrip_Outcome_4B}The hunting party arrives with an impressive {animalsCaught} animals, providing {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. Their success calls for a grand feast, which you order to be prepared immediately.",
                
                //Event Outcome 4C
                "{=HuntingTrip_Outcome_4C}Your men triumphantly return with {animalsCaught} animals, yielding {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. The camp celebrates this remarkable achievement with a well-deserved feast.",
                
                //Event Outcome 4D
                "{=HuntingTrip_Outcome_4D}Returning with an extraordinary {animalsCaught} animals, your hunters provide {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. You ensure the entire camp enjoys the bounty with a festive meal.",
                
                //Event Outcome 4E
                "{=HuntingTrip_Outcome_4E}The hunters return laden with {animalsCaught} animals, yielding {animalsCaught} hides and {yieldedMeatResources} " +
                "pieces of meat. Their exceptional success sparks a celebratory feast for the whole camp."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=HuntingTrip_Event_Msg_1A}Your men returned empty handed but it raised morale by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_1B}The hunting trip yielded no game, but morale increased by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_1C}No animals were caught, but your men enjoyed the outing, boosting morale by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_1D}Despite returning empty-handed, your men’s spirits lifted, gaining {moraleGained} morale.",
                "{=HuntingTrip_Event_Msg_1E}The hunt was unsuccessful, but the time away improved morale by {moraleGained}."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=HuntingTrip_Event_Msg_2A}The hunt yielded {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nMorale was raised by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_2B}Your men brought back {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nThe camp’s morale increased by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_2C}A modest haul of {animalsCaught} hides and {yieldedMeatResources} pieces of meat was secured.\nMorale improved by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_2D}Returning with {animalsCaught} hides and {yieldedMeatResources} pieces of meat, your men lifted spirits by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_2E}The small hunt resulted in {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nMorale was raised by {moraleGained}."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=HuntingTrip_Event_Msg_3A}The hunt yielded {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nMorale was raised by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_3B}Your men returned with {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nThe camp’s morale increased by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_3C}A solid haul of {animalsCaught} hides and {yieldedMeatResources} pieces of meat boosted morale by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_3D}Bringing back {animalsCaught} hides and {yieldedMeatResources} pieces of meat, your men raised spirits by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_3E}The hunt was a success, yielding {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nMorale increased by {moraleGained}."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            { 
                "{=HuntingTrip_Event_Msg_4A}The hunt yielded {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nMorale was raised by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_4B}Your men triumphantly returned with {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nThe camp’s morale soared by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_4C}A remarkable haul of {animalsCaught} hides and {yieldedMeatResources} pieces of meat greatly lifted spirits, raising morale by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_4D}The hunt was a resounding success, yielding {animalsCaught} hides and {yieldedMeatResources} pieces of meat.\nMorale increased by {moraleGained}.",
                "{=HuntingTrip_Event_Msg_4E}An extraordinary {animalsCaught} hides and {yieldedMeatResources} pieces of meat were brought back, boosting morale significantly by {moraleGained}."
            };

            
            public static string GetRandomEventTitle()
            {
                var index = random.Next(eventTitles.Count);
                return eventTitles[index];
            }
            
            public static string GetRandomEventDescription()
            {
                var index = random.Next(eventDescriptions.Count);
                return eventDescriptions[index];
            }
            
            public static string GetRandomEventOutcome1()
            {
                var index = random.Next(EventOutcome1.Count);
                return EventOutcome1[index];
            }
            
            public static string GetRandomEventOutcome2()
            {
                var index = random.Next(EventOutcome2.Count);
                return EventOutcome2[index];
            }
            
            public static string GetRandomEventOutcome3()
            {
                var index = random.Next(EventOutcome3.Count);
                return EventOutcome3[index];
            }
            
            public static string GetRandomEventOutcome4()
            {
                var index = random.Next(EventOutcome4.Count);
                return EventOutcome4[index];
            }
            
            public static string GetRandomEventMessage1()
            {
                var index = random.Next(eventMsg1.Count);
                return eventMsg1[index];
            }
            
            public static string GetRandomEventMessage2()
            {
                var index = random.Next(eventMsg2.Count);
                return eventMsg2[index];
            }
            
            public static string GetRandomEventMessage3()
            {
                var index = random.Next(eventMsg3.Count);
                return eventMsg3[index];
            }
            
            public static string GetRandomEventMessage4()
            {
                var index = random.Next(eventMsg4.Count);
                return eventMsg4[index];
            }
        }
    }


    public class HuntingTripData : RandomEventData
    {

        public HuntingTripData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new HuntingTrip();
        }
    }
}