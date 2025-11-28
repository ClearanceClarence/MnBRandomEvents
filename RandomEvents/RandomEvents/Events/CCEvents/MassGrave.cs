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

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class MassGrave : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minSoldiers;
        private readonly int maxSoldiers;
        private readonly int minBodies;
        private readonly int maxBodies;
        private readonly int minBaseMoraleLoss;
        private readonly int maxBaseMoraleLoss;
        

        public MassGrave() : base(ModSettings.RandomEvents.MassGraveData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("MassGrave", "EventDisabled");
            minSoldiers = ConfigFile.ReadInteger("MassGrave", "MinSoldiers");
            maxSoldiers = ConfigFile.ReadInteger("MassGrave", "MaxSoldiers");
            minBodies = ConfigFile.ReadInteger("MassGrave", "MinBodies");
            maxBodies = ConfigFile.ReadInteger("MassGrave", "MaxBodies");
            minBaseMoraleLoss = ConfigFile.ReadInteger("MassGrave", "MinBaseMoraleLoss");
            maxBaseMoraleLoss = ConfigFile.ReadInteger("MassGrave", "MaxBaseMoraleLoss");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minSoldiers != 0 || maxSoldiers != 0 || minBodies != 0 || maxBodies != 0 ||  minBaseMoraleLoss != 0 || maxBaseMoraleLoss != 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool CanExecuteEvent()
        {

            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement == null && MobileParty.MainParty.MemberRoster.TotalRegulars >= maxSoldiers + minSoldiers;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
            
            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();

            var baseMoraleLoss = MBRandom.RandomInt(minBaseMoraleLoss, maxBaseMoraleLoss);

            var soldiersDiscovery = MBRandom.RandomInt(minSoldiers, maxSoldiers);
            var bodiesInGrave = MBRandom.RandomInt(minBodies, maxBodies);
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("soldiersDiscovery", soldiersDiscovery)
                .ToString();
            
            var eventOption1 = new TextObject("{=MassGrave_Event_Option_1}Make individual graves").ToString();
            var eventOption1Hover = new TextObject("{=MassGrave_Event_Option_1_Hover}They should be given a proper burial").ToString();
            
            var eventOption2 = new TextObject("{=MassGrave_Event_Option_2}Fill the grave with dirt").ToString();
            var eventOption2Hover = new TextObject("{=MassGrave_Event_Option_2_Hover}The least you can do is fill the hole").ToString();
            
            var eventOption3 = new TextObject("{=MassGrave_Event_Option_3}Burn the bodies").ToString();
            var eventOption3Hover = new TextObject("{=MassGrave_Event_Option_3_Hover}Quickest and easiest way").ToString();
            
            var eventOption4 = new TextObject("{=MassGrave_Event_Option_4}Leave them").ToString();
            var eventOption4Hover = new TextObject("{=MassGrave_Event_Option_4_Hover}Not your problem").ToString();
            
            var eventButtonText1 = new TextObject("{=MassGrave_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=MassGrave_Event_Button_Text_2}Done").ToString();
            
            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
                new InquiryElement("c", eventOption3, null, true, eventOption3Hover),
                new InquiryElement("d", eventOption4, null, true, eventOption4Hover)
            };
            
            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .SetTextVariable("bodiesInGrave", bodiesInGrave)
                .ToString();
            
            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventOutcome2())
                .ToString();
            
            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .ToString();
            
            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .ToString();
            
            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessages())
                .SetTextVariable("baseMoraleLoss", baseMoraleLoss - 2)
                .ToString();
            
            var eventMsg2=new TextObject(EventTextHandler.GetRandomEventMessages())
                .SetTextVariable("baseMoraleLoss", baseMoraleLoss - 3)
                .ToString();
            
            var eventMsg3 =new TextObject(EventTextHandler.GetRandomEventMessages())
                .SetTextVariable("baseMoraleLoss", baseMoraleLoss - 4)
                .ToString();
            
            var eventMsg4 =new TextObject(EventTextHandler.GetRandomEventMessages())
                .SetTextVariable("baseMoraleLoss", baseMoraleLoss - 5)
                .ToString();


            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1, eventButtonText1, null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= baseMoraleLoss - 2;
                            MobileParty.MainParty.MoraleExplained.Add(-baseMoraleLoss - 2);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            
                            break;
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= baseMoraleLoss - 3;
                            MobileParty.MainParty.MoraleExplained.Add(-baseMoraleLoss - 3);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            
                            break;
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= baseMoraleLoss - 4;
                            MobileParty.MainParty.MoraleExplained.Add(-baseMoraleLoss - 4);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            
                            break;
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= baseMoraleLoss - 5;
                            MobileParty.MainParty.MoraleExplained.Add(-baseMoraleLoss - 5);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            
                            break;
                        default:
                            MessageManager.DisplayMessage($"Error while selecting option for \"{randomEventData.eventType}\"");
                            break;
                    }
                },
                null, null);
            
            MBInformationManager.ShowMultiSelectionInquiry(msid, true);


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
                "{=MassGrave_Title_A}The Mass Grave",
                "{=MassGrave_Title_B}A Field of Bones",
                "{=MassGrave_Title_C}The Forgotten Burial",
                "{=MassGrave_Title_D}Shadows of the Dead",
                "{=MassGrave_Title_E}The Grave of Many",
                "{=MassGrave_Title_F}The Silent Resting Place",
                "{=MassGrave_Title_G}The Pit of Sorrows",
                "{=MassGrave_Title_H}The Lost Tomb",
                "{=MassGrave_Title_I}Echoes of the Fallen",
                "{=MassGrave_Title_J}The Unmarked Grave"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=MassGrave_Event_Desc_A}Your party has set up camp near {closestSettlement}, and you have sent out some men to gather resources and hunt. Out of the blue, {soldiersDiscovery} " +
                "of your men come back and tell you that there is something you need to see. You join your men as they escort you to whatever it is they want to show you.\n" +
                "When you arrive, you are shocked to see a fresh mass grave filled with men, women, and children. Your men ask you what they think we should do.",
                
                //Event Description B
                "{=MassGrave_Event_Desc_B}While camped near {closestSettlement}, you send out {soldiersDiscovery} of your men to gather resources. Unexpectedly, they return, urging you " +
                "to follow them. Intrigued and uneasy, you accompany them to the site they’ve discovered.\n" +
                "Upon arrival, you are horrified to find a mass grave, recently filled with the bodies of men, women, and children. Your men turn to you for guidance, asking what should be done.",
                
                //Event Description C
                "{=MassGrave_Event_Desc_C}As your party camps near {closestSettlement}, {soldiersDiscovery} of your men return from a scouting trip, insisting you come with them to see " +
                "something they’ve found. With growing concern, you follow them to the site.\n" +
                "You are stunned to find a mass grave filled with recently buried men, women, and children. Your men look to you for direction, unsure of how to proceed.",
                
                //Event Description D
                "{=MassGrave_Event_Desc_D}Camping near {closestSettlement}, you send a few men to gather supplies. Not long after, {soldiersDiscovery} return with urgency, claiming they’ve " +
                "found something disturbing. Following them, you come upon the site.\n" +
                "To your horror, it’s a fresh mass grave containing men, women, and children. The grim discovery leaves your men uncertain, and they ask for your decision on how to respond.",
                
                //Event Description E
                "{=MassGrave_Event_Desc_E}While stationed near {closestSettlement}, {soldiersDiscovery} of your men approach you with urgency, saying they’ve found something alarming. " +
                "Curiosity and dread push you to follow them to the site.\n" +
                "You’re horrified to find a fresh mass grave, the bodies of men, women, and children laid within. Your men, shaken, ask you what should be done about the grisly discovery."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Outcome 1A
                "{=MassGrave_Event_Choice_1A}You order two of your men to return to camp for shovels, linen, and additional help, while you and the others begin carefully removing the bodies from the grave. " +
                "Each body is laid out in neat lines, and with every child you recover, a wave of sorrow washes over you. Tears stream down your face, and many of your men weep silently alongside you. " +
                "When the supplies and reinforcements arrive, some of your men start digging graves while others wrap the bodies in linen. " +
                "In total, you recover {bodiesInGrave} bodies from the grave. Hours later, as night approaches, the task is complete. " +
                "You gather around the campfire with your men, discussing the emotional weight of the day’s events.",

                //Event Outcome 1B
                "{=MassGrave_Event_Choice_1B}You send two men back to camp to retrieve tools and reinforcements while you and the others begin the grim task of recovering the bodies. " +
                "You carefully lay them out in orderly rows, feeling an unbearable sadness when children are brought up from the grave. Many of your men, like you, are moved to tears. " +
                "When the others return with shovels, linen, and more hands, the process of digging graves and preparing the bodies begins. " +
                "In total, you recover {bodiesInGrave} bodies, finishing the somber task just as the sun dips below the horizon. " +
                "Back at camp, you and your men sit around the fire, sharing thoughts and emotions stirred by the harrowing experience.",

                //Event Outcome 1C
                "{=MassGrave_Event_Choice_1C}You instruct two of your men to head back to camp to fetch shovels, linen, and additional help. Meanwhile, you and the remaining men carefully lift the bodies from the grave. " +
                "Each one is placed respectfully in rows, but the sight of a child’s body brings a deep ache to your heart, and you can’t help but shed tears. Your men share in the grief, their faces solemn. " +
                "As reinforcements arrive, graves are dug and the bodies are wrapped in linen. " +
                "By the time the {bodiesInGrave} bodies are properly buried, night is beginning to fall. " +
                "You return to camp, where the fire becomes a place of shared sorrow and reflection on the day’s grim discoveries.",

                //Event Outcome 1D
                "{=MassGrave_Event_Choice_1D}Two men are sent back to camp to gather tools and more help, while you and the rest begin the arduous task of recovering the bodies. " +
                "One by one, they are laid out in orderly rows, but the sight of children among the dead weighs heavily on your heart. Tears fall freely, and your men silently grieve alongside you. " +
                "Once the reinforcements arrive, some men dig individual graves while others carefully wrap the bodies. " +
                "By nightfall, all {bodiesInGrave} bodies are buried, and you return to camp emotionally drained. " +
                "You and your men gather around the fire, discussing the sadness and unease left by the day's events.",

                //Event Outcome 1E
                "{=MassGrave_Event_Choice_1E}You order two men to return to camp for supplies while you and the others begin the grim work of removing the bodies. " +
                "With each body you lay out in rows, the weight of the tragedy deepens, especially when a child is recovered. Tears fall freely from you and your men as the emotional toll sets in. " +
                "When the others return with shovels, linen, and more hands, the burial begins in earnest. " +
                "A total of {bodiesInGrave} bodies are recovered, and the task stretches until nightfall. " +
                "Around the campfire, you and your men reflect on the tragedy and share your thoughts on what you witnessed."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Event Outcome 2A
                "{=MassGrave_Event_Choice_2A}You order two of your men to return to camp to fetch shovels and reinforcements while you and the others begin filling the grave. " +
                "Before long, additional men arrive to assist. Together, you spend several hours moving dirt until the grave is fully covered. " +
                "Before leaving, you and your men stand in silence, reciting prayers for the souls of the departed.",
                
                //Event Outcome 2B
                "{=MassGrave_Event_Choice_2B}You instruct two men to retrieve shovels and reinforcements from camp while you and the rest start filling the grave. " +
                "More men soon arrive to help, and after hours of work, the grave is completely filled. " +
                "Before departing, you and your men solemnly recite prayers, paying respect to the lost lives.",
                
                //Event Outcome 2C
                "{=MassGrave_Event_Choice_2C}Two men are sent to camp for supplies while you and the others begin the somber task of filling the grave. " +
                "When more men join, the work progresses steadily, taking several hours to complete. " +
                "Once the grave is filled, your group pauses to offer prayers for the dead before returning to camp.",
                
                //Event Outcome 2D
                "{=MassGrave_Event_Choice_2D}You send two men to fetch shovels and additional help while you and the others start filling the grave. " +
                "The reinforcements arrive shortly, and together, you spend hours working until the grave is completely covered. " +
                "Before leaving, you gather your men to recite prayers, ensuring the dead receive a final tribute.",
                
                //Event Outcome 2E
                "{=MassGrave_Event_Choice_2E}While two men retrieve shovels and more help from camp, you and the rest begin filling the grave. " +
                "The additional hands make the work go faster, though it still takes hours to complete. " +
                "Once the grave is filled, you and your men stand together, offering prayers for those buried below."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Event Outcome 3A
                "{=MassGrave_Event_Choice_3A}You order your men to begin burning the bodies. The flames take quickly, and within a few hours, only ash and bone remain in the grave. " +
                "The grim task leaves a heavy silence among your men as you return to camp.",
                
                //Event Outcome 3B
                "{=MassGrave_Event_Choice_3B}Your orders are clear: burn the bodies. The fire consumes them swiftly, and after a few hours, nothing remains but ash and fragments of bone. " +
                "The somber atmosphere lingers as you and your men walk back to camp.",
                
                //Event Outcome 3C
                "{=MassGrave_Event_Choice_3C}You instruct your men to set the bodies alight. The flames roar to life, and within hours, the grave is reduced to a mix of ash and bone. " +
                "The sight weighs heavily on everyone as you quietly return to camp.",
                
                //Event Outcome 3D
                "{=MassGrave_Event_Choice_3D}Under your orders, the men prepare and ignite the bodies. The fire burns hot and fast, and by the time you return, only ash and bone remain. " +
                "The haunting scene stays with you as you make your way back to camp.",
                
                //Event Outcome 3E
                "{=MassGrave_Event_Choice_3E}You command your men to burn the bodies. It takes only a few hours for the flames to reduce the contents of the grave to ash and bone. " +
                "The task complete, you and your men return to camp in somber silence."
            };
            
            private static readonly List<string> EventOutcome4= new List<string>
            {
                //Event Outcome 4A
                "{=MassGrave_Event_Choice_4A}You decide to leave the area as it is, instructing your men to handle it however they see fit. " +
                "Later that evening, your men return, reporting that they have buried the bodies.",
                
                //Event Outcome 4B
                "{=MassGrave_Event_Choice_4B}Choosing not to intervene directly, you tell your men to deal with the situation as they deem appropriate. " +
                "By nightfall, they return and inform you that the bodies have been buried.",
                
                //Event Outcome 4C
                "{=MassGrave_Event_Choice_4C}You decide to move on without taking action yourself, leaving your men to handle the situation. " +
                "That evening, they return to camp and report that they’ve buried the bodies with care.",
                
                //Event Outcome 4D
                "{=MassGrave_Event_Choice_4D}Opting not to take charge, you allow your men to address the situation in their own way. " +
                "Hours later, they return, having completed the burial of the bodies.",
                
                //Event Outcome 4E
                "{=MassGrave_Event_Choice_4E}You leave the decision to your men, telling them to handle the matter as they see fit. " +
                "By evening, they return, explaining that the bodies have been respectfully buried."
            };
            
            private static readonly List<string> eventMsgs = new List<string>
            {
                "{=MassGrave_Event_Msg_1}Your party lost {baseMoraleLoss} morale due to recent events.",
                "{=MassGrave_Event_Msg_2}The grim discovery has caused your party to lose {baseMoraleLoss} morale.",
                "{=MassGrave_Event_Msg_3}Recent events have shaken your party, resulting in a morale loss of {baseMoraleLoss}.",
                "{=MassGrave_Event_Msg_4}The unsettling events have lowered your party’s morale by {baseMoraleLoss}.",
                "{=MassGrave_Event_Msg_5}Your party struggles to recover from recent events, losing {baseMoraleLoss} morale."
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
            
            public static string GetRandomEventMessages()
            {
                var index = random.Next(eventMsgs.Count);
                return eventMsgs[index];
            }
        }
    }


    public class MassGraveData : RandomEventData
    {
        public MassGraveData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new MassGrave();
        }
    }
}