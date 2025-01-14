using System;
using System.Collections.Generic;
using System.Windows;
using CryingBuffalo.RandomEvents.Helpers;
using CryingBuffalo.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace CryingBuffalo.RandomEvents.Events.CCEvents
{
    public sealed class LoggingSite : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minSoldiersToGo;
        private readonly int maxSoldiersToGo;
        private readonly int minYield;
        private readonly int maxYield;
        private readonly int minYieldMultiplier;
        private readonly int maxYieldMultiplier;
        

        public LoggingSite() : base(ModSettings.RandomEvents.LoggingSiteData)
        {
            
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("LoggingSite", "EventDisabled");
            minSoldiersToGo = ConfigFile.ReadInteger("LoggingSite", "MinSoldiersToGo");
            maxSoldiersToGo = ConfigFile.ReadInteger("LoggingSite", "MaxSoldiersToGo");
            minYield = ConfigFile.ReadInteger("LoggingSite", "MinYield");
            maxYield = ConfigFile.ReadInteger("LoggingSite", "MaxYield");
            minYieldMultiplier = ConfigFile.ReadInteger("LoggingSite", "MinYieldMultiplier");
            maxYieldMultiplier = ConfigFile.ReadInteger("LoggingSite", "MaxYieldMultiplier");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minSoldiersToGo != 0 || maxSoldiersToGo != 0 || minYield != 0 || maxYield != 0 ||  minYieldMultiplier != 0 || maxYieldMultiplier != 0)
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
            
            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();

            var soldiersGoneLogging = MBRandom.RandomInt(minSoldiersToGo, maxSoldiersToGo);
            
            var treesChopped = MBRandom.RandomInt(minYield, maxYield);

            var yieldHardwood = treesChopped * MBRandom.RandomInt(minYieldMultiplier, maxYieldMultiplier);
            
            var hardwood = MBObjectManager.Instance.GetObject<ItemObject>("hardwood");
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("soldiersGoneLogging", soldiersGoneLogging)
                .ToString();
            
            var eventOutcome1 = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .SetTextVariable("treesChopped",treesChopped)
                .SetTextVariable("yieldHardwood",yieldHardwood)
                .ToString();
            
            var eventOutcome2 = new TextObject(EventTextHandler.GetRandomEventOutcome2())
                .SetTextVariable("treesChopped",treesChopped)
                .SetTextVariable("yieldHardwood",yieldHardwood)
                .ToString();
            
            var eventOutcome3 = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .SetTextVariable("treesChopped",treesChopped)
                .SetTextVariable("yieldHardwood",yieldHardwood)
                .ToString();
            
            
            var eventButtonText = new TextObject("{=LoggingSite_Event_Button_Text}Continue")
                .ToString();

            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("yieldHardwood", yieldHardwood)
                .ToString();
            
            var eventMsg2 =new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("yieldHardwood", yieldHardwood)
                .ToString();
            
            var eventMsg3 =new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("yieldHardwood", yieldHardwood)
                .ToString();

            
            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventDescription, true, false, eventButtonText, null, null, null), true);
            
            MobileParty.MainParty.ItemRoster.AddToCounts(hardwood, yieldHardwood);
            
            if (yieldHardwood > 25 && yieldHardwood <= 35)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome1, true, false, eventButtonText, null, null, null), true);
                InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_POS_Outcome));
            }
            else if (yieldHardwood > 35 && yieldHardwood <= 50)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome2, true, false, eventButtonText, null, null, null), true);
                InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_POS_Outcome));
            }
            else if (yieldHardwood > 50)
            {
                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOutcome3, true, false, eventButtonText, null, null, null), true);
                InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_POS_Outcome));
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
                "{=LoggingSite_Title_A}The Hardwood Forest",
                "{=LoggingSite_Title_B}The Whispering Woods",
                "{=LoggingSite_Title_C}The Lumberjack's Haven",
                "{=LoggingSite_Title_D}The Endless Timbers",
                "{=LoggingSite_Title_E}The Dense Woodland",
                "{=LoggingSite_Title_F}The Echoing Forest",
                "{=LoggingSite_Title_G}The Timberland Grove",
                "{=LoggingSite_Title_H}The Ancient Forest",
                "{=LoggingSite_Title_I}The Verdant Canopy",
                "{=LoggingSite_Title_J}The Greenwood Retreat"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=LoggingSite_Event_Desc_A}While your party is traveling through the lands near {closestSettlement}, you come across a forest rich in hardwood trees. " +
                "You decide that it's time to stock up on hardwood, so you order {soldiersGoneLogging} of your men to get to work. The men agree that this is a good opportunity " +
                "to get some resources, so they do as you say without much complaint.\nYou don't really need much, just enough to start some smithing projects next time you come " +
                "across a forge. The rest you can easily sell for a nice profit.",
                
                //Event Description B
                "{=LoggingSite_Event_Desc_B}As your party journeys near {closestSettlement}, you discover a forest teeming with valuable hardwood trees. " +
                "Seeing an opportunity, you instruct {soldiersGoneLogging} of your men to gather some of the timber. They eagerly agree, recognizing the value " +
                "of the resource. You plan to use some for smithing while selling the surplus for a tidy profit.",
                
                //Event Description C
                "{=LoggingSite_Event_Desc_C}Your party stumbles upon a dense forest full of prime hardwood near {closestSettlement}. " +
                "Recognizing the potential, you order {soldiersGoneLogging} of your men to begin logging. The men set to work willingly, " +
                "knowing the value of the resources. You intend to use the hardwood for smithing and to make some profit from the excess.",
                
                //Event Description D
                "{=LoggingSite_Event_Desc_D}While passing through the lands near {closestSettlement}, you come across a forest known for its hardwood. " +
                "You instruct {soldiersGoneLogging} of your men to gather enough wood to restock your supplies. They agree without hesitation, " +
                "seeing the opportunity to gather resources for smithing and trade.",
                
                //Event Description E
                "{=LoggingSite_Event_Desc_E}Traveling through the area near {closestSettlement}, your party encounters a forest filled with hardwood trees. " +
                "You decide it’s an excellent chance to replenish your stock, so you send {soldiersGoneLogging} of your men to log some timber. " +
                "The men see the value in the task and work without complaint, knowing the hardwood will serve for crafting or selling later."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Outcome 1A
                "{=LoggingSite_Outcome_1A}The logging crew return just as the sun is setting. In total, they chopped down {treesChopped} trees which yielded {yieldHardwood} pieces of hardwood.\n" +
                "You admittedly had hoped for more resources than this, so you berate your men for being lazy.",
                
                //Outcome 1B
                "{=LoggingSite_Outcome_1B}As the sun sets, the logging crew returns with only {treesChopped} trees worth of hardwood, yielding {yieldHardwood} pieces. " +
                "Frustrated with the underwhelming results, you reprimand your men for their lack of effort.",
                
                //Outcome 1C
                "{=LoggingSite_Outcome_1C}The crew returns at dusk with only {yieldHardwood} pieces of hardwood from {treesChopped} trees. Disappointed, you question their dedication " +
                "and remind them of the importance of their work.",
                
                //Outcome 1D
                "{=LoggingSite_Outcome_1D}Your logging crew arrives just as the day ends, having only chopped {treesChopped} trees and yielding {yieldHardwood} pieces of hardwood. " +
                "The result is far less than you expected, and you sternly scold them for their poor performance.",
                
                //Outcome 1E
                "{=LoggingSite_Outcome_1E}The logging crew trudges back to camp at sunset, managing only {treesChopped} trees and yielding {yieldHardwood} pieces of hardwood. " +
                "Disappointed by their effort, you criticize them for wasting valuable time and resources."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Outcome 2A
                "{=LoggingSite_Outcome_2A}The logging crew return just as the sun is setting. In total, they chopped down {treesChopped} trees, yielding {yieldHardwood} pieces of hardwood.\n" +
                "You had hoped for a better result, but all in all, this should be enough to cover your own projects.",
                
                //Outcome 2B
                "{=LoggingSite_Outcome_2B}The crew returns at sunset, reporting {treesChopped} trees felled and {yieldHardwood} pieces of hardwood collected. " +
                "It’s not quite as much as you had hoped, but it should suffice for your immediate needs.",
                
                //Outcome 2C
                "{=LoggingSite_Outcome_2C}As the sun dips below the horizon, the logging crew arrives with {yieldHardwood} pieces of hardwood from {treesChopped} trees. " +
                "While it’s not a stellar outcome, it should be enough to handle your projects for now.",
                
                //Outcome 2D
                "{=LoggingSite_Outcome_2D}The logging team returns at day’s end with {yieldHardwood} pieces of hardwood from {treesChopped} trees. " +
                "Though not the haul you were expecting, it’s sufficient for your current plans.",
                
                //Outcome 2E
                "{=LoggingSite_Outcome_2E}The logging crew makes it back as the sun sets, having chopped {treesChopped} trees and yielding {yieldHardwood} pieces of hardwood. " +
                "It’s an acceptable result, enough to meet your immediate needs, though not as much as you’d hoped."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Outcome 3A
                "{=LoggingSite_Outcome_3A}The logging crew return just as the sun is setting. In total, they chopped down {treesChopped} trees, yielding {yieldHardwood} pieces of hardwood.\n" +
                "This is the result you were hoping for! Enough for your own projects and enough to sell at a settlement for a nice profit. You congratulate your men on a hard day's work.",
                
                //Outcome 3B
                "{=LoggingSite_Outcome_3B}As the sun sets, the logging crew returns with an impressive haul of {yieldHardwood} pieces of hardwood from {treesChopped} trees. " +
                "This is exactly what you needed—plenty for your projects and a surplus to trade. You praise your men for their dedication and effort.",
                
                //Outcome 3C
                "{=LoggingSite_Outcome_3C}The crew arrives just before nightfall, having felled {treesChopped} trees and collected {yieldHardwood} pieces of hardwood. " +
                "You are pleased with the outcome, knowing there’s enough for your plans and to turn a profit. Your men earn your commendation for their hard work.",
                
                //Outcome 3D
                "{=LoggingSite_Outcome_3D}The logging crew finishes the day with {treesChopped} trees cut and {yieldHardwood} pieces of hardwood collected. " +
                "It’s a great result, more than enough for your needs and to sell for extra coin. You thank your men for their excellent effort.",
                
                //Outcome 3E
                "{=LoggingSite_Outcome_3E}As the sun sets, the logging crew returns with {treesChopped} trees worth of timber, yielding {yieldHardwood} pieces of hardwood. " +
                "It’s a fantastic outcome—ample for your smithing projects and to fetch a good price at market. You commend your men for their outstanding work."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=LoggingSite_Event_Msg_1}You got {yieldHardwood} hardwood.",
                "{=LoggingSite_Event_Msg_2}Your men collected {yieldHardwood} pieces of hardwood.",
                "{=LoggingSite_Event_Msg_3}The logging yielded {yieldHardwood} pieces of hardwood.",
                "{=LoggingSite_Event_Msg_4}You’ve added {yieldHardwood} hardwood to your stockpile.",
                "{=LoggingSite_Event_Msg_5}{yieldHardwood} pieces of hardwood were successfully gathered."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=LoggingSite_Event_Msg_2A}You got {yieldHardwood} hardwood. You hoped for more.",
                "{=LoggingSite_Event_Msg_2B}Your men collected {yieldHardwood} pieces of hardwood. It’s enough for now, but not as much as you expected.",
                "{=LoggingSite_Event_Msg_2C}The logging resulted in {yieldHardwood} hardwood. It’s acceptable, but you had higher expectations.",
                "{=LoggingSite_Event_Msg_2D}You’ve gathered {yieldHardwood} pieces of hardwood. It should suffice, though you wish there was more.",
                "{=LoggingSite_Event_Msg_2E}The crew brought back {yieldHardwood} hardwood. Not quite the haul you were hoping for."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=LoggingSite_Event_Msg_3A}You got {yieldHardwood} hardwood. Awesome!",
                "{=LoggingSite_Event_Msg_3B}Your men collected {yieldHardwood} pieces of hardwood. Excellent work!",
                "{=LoggingSite_Event_Msg_3C}An impressive {yieldHardwood} pieces of hardwood were gathered. Fantastic result!",
                "{=LoggingSite_Event_Msg_3D}You’ve secured {yieldHardwood} pieces of hardwood. This is exactly what you needed!",
                "{=LoggingSite_Event_Msg_3E}The haul yielded {yieldHardwood} hardwood. Great job all around!"
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
        }
    }


    public class LoggingSiteData : RandomEventData
    {

        public LoggingSiteData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new LoggingSite();
        }
    }
}