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
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class FishingSpot : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minSoldiersToGo;
        private readonly int maxSoldiersToGo;
        private readonly int maxFishCatch;
        private readonly int minMoraleGain;
        private readonly int maxMoraleGain;
        

        public FishingSpot() : base(ModSettings.RandomEvents.FishingSpotData)
        {
            
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("FishingSpot", "EventDisabled");
            minSoldiersToGo = ConfigFile.ReadInteger("FishingSpot", "MinSoldiersToGo");
            maxSoldiersToGo = ConfigFile.ReadInteger("FishingSpot", "MaxSoldiersToGo");
            maxFishCatch = ConfigFile.ReadInteger("FishingSpot", "MaxFishCatch");
            minMoraleGain = ConfigFile.ReadInteger("FishingSpot", "MinMoraleGain");
            maxMoraleGain = ConfigFile.ReadInteger("FishingSpot", "MaxMoraleGain");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minSoldiersToGo != 0 || maxSoldiersToGo != 0 || maxFishCatch != 0 || minMoraleGain != 0 || maxMoraleGain != 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.MemberRoster.TotalRegulars >= maxSoldiersToGo && MobileParty.MainParty.CurrentSettlement == null;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

            var soldiersGoneFishing = MBRandom.RandomInt(minSoldiersToGo, maxSoldiersToGo);
            var fishCaught = MBRandom.RandomInt(0, maxFishCatch);
            
            var moraleGained = MBRandom.RandomInt(minMoraleGain, maxMoraleGain);
            
            var fish = MBObjectManager.Instance.GetObject<ItemObject>("fish");
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("soldiersGoneFishing", soldiersGoneFishing)
                .ToString();
            
            var eventOutcome1 = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .ToString();
            
            var eventOutcome2 = new TextObject(EventTextHandler.GetRandomEventOutcome2())
                .SetTextVariable("fishCaught",fishCaught)
                .ToString();
            
            var eventOutcome3 = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .SetTextVariable("fishCaught",fishCaught)
                .ToString();
            
            var eventOutcome4 = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .SetTextVariable("fishCaught",fishCaught)
                .ToString();
            
            var eventButtonText = new TextObject("{=FishingSpot_Event_Button_Text}Continue")
                .ToString();
            
            var eventMsg1 = new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("moraleGained", MathF.Floor(moraleGained * 0.02f))
                .ToString();
            
            var eventMsg2 = new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("fishCaught", fishCaught)
                .SetTextVariable("moraleGained", MathF.Floor(moraleGained * 0.04f))
                .ToString();
            
            var eventMsg3 = new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("fishCaught", fishCaught)
                .SetTextVariable("moraleGained", MathF.Floor(moraleGained * 0.08f))
                .ToString();
            
            var eventMsg4 = new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("fishCaught", fishCaught)
                .SetTextVariable("moraleGained", moraleGained)
                .ToString();
            

            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventDescription, true, false, eventButtonText, null, null, null), true);
            
            MobileParty.MainParty.ItemRoster.AddToCounts(fish, fishCaught);

            if (fishCaught == 0)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome1, true, false,eventButtonText, null, null, null), true);
                
                MobileParty.MainParty.RecentEventsMorale += MathF.Floor(moraleGained * 0.02f);
                MobileParty.MainParty.MoraleExplained.Add(moraleGained);
                
                InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_MED_Outcome));
            }
            else if (fishCaught > 0 && fishCaught <= 5)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome2, true, false, eventButtonText, null, null, null), true);
                
                MobileParty.MainParty.RecentEventsMorale += MathF.Floor(moraleGained * 0.04f);
                MobileParty.MainParty.MoraleExplained.Add(moraleGained);
                
                InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_POS_Outcome));
            }
            else if (fishCaught > 5 && fishCaught <= 15)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome3, true, false, eventButtonText, null, null, null), true);
                
                MobileParty.MainParty.RecentEventsMorale += MathF.Floor(moraleGained * 0.08f);
                MobileParty.MainParty.MoraleExplained.Add(moraleGained);
                
                InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_POS_Outcome));
            }
            else if (fishCaught > 15 && fishCaught <= maxFishCatch)
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
                if (onEventCompleted != null)
                {
                    onEventCompleted.Invoke();
                }
                else
                {
                    MessageManager.DisplayMessage($"onEventCompleted was null while stopping \"{randomEventData.eventType}\" event.");
                }
            }
            catch (Exception ex)
            {
                MessageManager.DisplayMessage($"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }
        
          private static class EventTextHandler
        {
            private static readonly Random random = new Random();
            
            private static readonly List<string> eventTitles = new List<string>
            {
                "{=FishingSpot_Title_A}A Great Fishing Spot",
                "{=FishingSpot_Title_B}Another Great Fishing Spot",
                "{=FishingSpot_Title_C}The Best Fishing Spot Around",
                "{=FishingSpot_Title_D}An Excellent Fishing Spot",
                "{=FishingSpot_Title_E}A Hidden Fishing Spot",
                "{=FishingSpot_Title_F}A Quiet Fishing Spot",
                "{=FishingSpot_Title_G}A Popular Fishing Spot",
                "{=FishingSpot_Title_H}A Scenic Fishing Spot",
                "{=FishingSpot_Title_I}A Serene Fishing Spot",
                "{=FishingSpot_Title_J}A Famous Fishing Spot",
                "{=FishingSpot_Title_K}A Peaceful Fishing Spot"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=FishingSpot_Event_Desc_A}While camping, {soldiersGoneFishing} of your men decide they want to go to the lake " +
                "just outside the camp to try and catch some fish.You could always use the additional resources and it would be "+
                "a great morale booster for the party if they catch some. You tell them to be back before nightfall.",
                
                //Event Description B
                "{=FishingSpot_Event_Desc_B}{soldiersGoneFishing} of your men are eager to visit the nearby river and try their luck at fishing. " +
                "They believe it could provide both food and a chance to relax after a tough day. You allow them to go but remind " +
                "them to return before dark.",
                
                //Event Description C
                "{=FishingSpot_Event_Desc_C}A nearby stream has caught the attention of {soldiersGoneFishing} of your soldiers, who ask " +
                "permission to fish for a while. You recognize this as an opportunity to improve morale and possibly secure some extra supplies.",
                
                //Event Description D
                "=FishingSpot_Event_Desc_D}{soldiersGoneFishing} of your men notice a small pond near the camp and request some time " +
                "to fish. This could be a chance to boost their spirits and add to your provisions. You agree, with a warning to be cautious.",
                
                //Event Description E
                "{=FishingSpot_Event_Desc_F}Your party comes across a picturesque fishing spot. {soldiersGoneFishing} of your men " +
                "request a brief respite to cast their lines and enjoy the moment. You permit them but emphasize the " +
                "importance of returning promptly."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Outcome 1A
                "{=FishingSpot_Outcome_1A}Your men return empty handed just before nightfall. At least they had a good time together.",
                
                //Event Outcome 1B
                "{=FishingSpot_Outcome_1B}Despite their best efforts, your men return with no fish. They seem slightly disappointed " +
                "but grateful for the break.",
                
                //Event Outcome 1C
                "{=FishingSpot_Outcome_1C}The fishing trip ends without any catch. Your men laugh it off, saying the fish were " +
                "too clever for them today.",
                
                //Event Outcome 1D
                "{=FishingSpot_Outcome_1D}Your soldiers come back empty-handed but in high spirits, having enjoyed the peaceful " +
                "time by the water.",
                
                //Event Outcome 1E
                "{=FishingSpot_Outcome_1E}No fish were caught, but your men share stories of their attempts, turning it into " +
                "an evening of camaraderie."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Event Outcome 2A
                "{=FishingSpot_Outcome_2A}Your men return with {fishCaught} fish just before nightfall. At least its better than " +
                "nothing. You let the fishermen enjoy their catch.",
                
                //Event Outcome 2B
                "{=FishingSpot_Outcome_2B}Your men manage to catch {fishCaught} fish—just enough to share a light meal. It’s " +
                "not much, but it’s something.",
                
                //Event Outcome 2C
                "{=FishingSpot_Outcome_2C}With {fishCaught} fish in their nets, your men return modestly satisfied. It’s a small " +
                "haul, but they seem pleased with their efforts.",
                
                //Event Outcome 2D
                "{=FishingSpot_Outcome_2D}The fishermen return with only {fishCaught} fish. It’s not a big catch, but they’re " +
                "happy to have contributed a little to the camp.",
                
                //Event Outcome 2E
                "{=FishingSpot_Outcome_2E}Bringing back {fishCaught} fish, your men acknowledge it’s a modest catch. Still, the " +
                "group enjoys the small bounty together."
                
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Event Outcome 3A
                "{=FishingSpot_Outcome_3A}Your men return with {fishCaught} fish just before nightfall. This is a sizeable catch " +
                "so you congratulate them. You join the fishermen in their feast.",
                
                //Event Outcome 3B
                "{=FishingSpot_Outcome_3B}Your men come back with {fishCaught} fish, enough to make a hearty meal for the group. " +
                "You commend their efforts and share in the feast.",
                
                //Event Outcome 3C
                "{=FishingSpot_Outcome_3C}The fishermen return with {fishCaught} fish—a decent haul that lifts everyone's spirits. " +
                "You gladly join them as they enjoy their catch.",
                
                //Event Outcome 3D
                "{=FishingSpot_Outcome_3D}With {fishCaught} fish in hand, your men are proud of their efforts. The camp buzzes with " +
                "excitement as you join the group for a satisfying meal.",
                
                //Event Outcome 3E
                "{=FishingSpot_Outcome_3E}A respectable catch of {fishCaught} fish returns with your men. You praise their work, " +
                "and the camp gathers for a cheerful dinner."
            };
            
            private static readonly List<string> EventOutcome4= new List<string>
            {
                //Event Outcome 4A
                "{=FishingSpot_Outcome_4A}Your men return triumphant with {fishCaught} fish just after nightfall. This is a " +
                "massive catch so you congratulate them. You order your men to start preparing food for everyone.",
                
                //Event Outcome 4B
                "{=FishingSpot_Outcome_4B}Your men arrive late but victorious, carrying {fishCaught} fish. This impressive " +
                "catch is enough to feed the entire camp and then some. You commend their efforts and order a grand feast.",
                
                //Event Outcome 4C
                "{=FishingSpot_Outcome_4C}Triumphantly returning with {fishCaught} fish, your men have outdone themselves. " +
                "The sheer volume of the catch calls for celebration, and you instruct the camp to prepare a lavish meal.",
                
                //Event Outcome 4D
                "{=FishingSpot_Outcome_4D}The fishermen return beaming, hauling in an extraordinary {fishCaught} fish. This " +
                "bounty is more than enough for the camp, and you ensure everyone gets their fill.",
                
                //Event Outcome 4E
                "{=FishingSpot_Outcome_4E}With an astounding {fishCaught} fish, your men come back just after nightfall. Their " +
                "success sparks excitement in the camp as you organize a feast to honor the achievement."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=FishingSpot_Event_Msg_1A}Your men returned empty handed but it raised morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_1B}The fishing trip yielded no fish, but morale increased by {moraleGained}.",
                "{=FishingSpot_Event_Msg_1C}No fish were caught, but your men enjoyed themselves, boosting morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_1D}Despite returning empty-handed, your men’s spirits lifted, gaining {moraleGained} morale.",
                "{=FishingSpot_Event_Msg_1E}The catch was nonexistent, but the break improved morale by {moraleGained}."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=FishingSpot_Event_Msg_2A}Your men returned with {fishCaught} and it raised morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_2B}The fishing trip brought in {fishCaught} fish, lifting morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_2C}Your men came back with {fishCaught} fish, and the camp’s morale improved by {moraleGained}.",
                "{=FishingSpot_Event_Msg_2D}Returning with {fishCaught} fish, your men’s spirits soared, gaining {moraleGained} morale.",
                "{=FishingSpot_Event_Msg_2E}The modest haul of {fishCaught} fish boosted morale by {moraleGained}."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=FishingSpot_Event_Msg_3A}Your men returned with {fishCaught} and it raised morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_3B}The group brought back a solid haul of {fishCaught} fish, increasing morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_3C}Your men returned proudly with {fishCaught} fish, boosting the camp’s morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_3D}With {fishCaught} fish in hand, your men’s spirits lifted, raising morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_3E}The decent catch of {fishCaught} fish cheered up the camp, gaining {moraleGained} morale."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            { 
                "{=FishingSpot_Event_Msg_4A}Your men returned with {fishCaught} and it raised morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_4B}Triumphantly, your men brought back {fishCaught} fish, significantly boosting morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_4C}With an impressive haul of {fishCaught} fish, your men’s morale soared, increasing by {moraleGained}.",
                "{=FishingSpot_Event_Msg_4D}The bountiful catch of {fishCaught} fish greatly lifted spirits, raising morale by {moraleGained}.",
                "{=FishingSpot_Event_Msg_4E}Returning with an extraordinary {fishCaught} fish, your men celebrated, gaining {moraleGained} morale."
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


    public class FishingSpotData : RandomEventData
    {

        public FishingSpotData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new FishingSpot();
        }
    }
}