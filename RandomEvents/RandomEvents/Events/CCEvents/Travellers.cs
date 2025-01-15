using System;
using System.Collections.Generic;
using System.Windows;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class Travellers : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minGoldStolen;
        private readonly int maxGoldStolen;
        private readonly int minEngineeringLevel;
        private readonly int minRogueryLevel;
        private readonly int minStewardLevel;

        public Travellers() : base(ModSettings.RandomEvents.TravellersData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());

            eventDisabled = ConfigFile.ReadBoolean("Travellers", "EventDisabled");
            minGoldStolen = ConfigFile.ReadInteger("Travellers", "MinGoldStolen");
            maxGoldStolen = ConfigFile.ReadInteger("Travellers", "MaxGoldStolen");
            minEngineeringLevel = ConfigFile.ReadInteger("Travellers", "MinEngineeringLevel");
            minRogueryLevel = ConfigFile.ReadInteger("Travellers", "MinRogueryLevel");
            minStewardLevel = ConfigFile.ReadInteger("Travellers", "MinStewardLevel");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minGoldStolen != 0 || maxGoldStolen != 0 || minEngineeringLevel != 0 || minRogueryLevel != 0 || minStewardLevel != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && (CurrentTimeOfDay.IsEvening || CurrentTimeOfDay.IsMidday || CurrentTimeOfDay.IsMorning)  && MobileParty.MainParty.MemberRoster.TotalRegulars >= 100;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

            var heroName = Hero.MainHero.Name.ToString();

            var engineeringLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Engineering);
            var rogueryLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Roguery);
            var stewardLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Steward);

            var randomSettlement = Settlement.All.GetRandomElement();
            var familyEthnicity = randomSettlement.Culture.Name.ToString();
            var familyDemonym = Demonym.GetTheDemonym(familyEthnicity, true);

            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();

            var stolenGold = MBRandom.RandomInt(minGoldStolen, maxGoldStolen);

            var canRepairWagon = false;
            var canRaidWagon = false;
            var canOfferDinner = false;

            var engineeringAppendedText = "";
            var rogueryAppendedText = "";
            var stewardAppendedText = "";

            if (GeneralSettings.SkillChecks.IsDisabled())
            {
                
                canRepairWagon = true;
                canRaidWagon = true;
                canOfferDinner = true;
                
                engineeringAppendedText = new TextObject("{=Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();
                rogueryAppendedText = new TextObject("{=Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();
                stewardAppendedText = new TextObject("{=Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();
                
            }
            else
            {
                if (engineeringLevel >= minEngineeringLevel)
                {
                    canRepairWagon = true;
                    
                    engineeringAppendedText = new TextObject("{=Engineering_Appended_Text}[Engineering - lvl {minEngineeringLevel}]")
                        .SetTextVariable("minEngineeringLevel", minEngineeringLevel)
                        .ToString();
                }

                if (rogueryLevel >= minRogueryLevel)
                {
                    canRaidWagon = true;
                    
                    rogueryAppendedText = new TextObject("{=Roguery_Appended_Text}[Roguery - lvl {minRogueryLevel}]")
                        .SetTextVariable("minRogueryLevel", minRogueryLevel)
                        .ToString();
                }

                if (stewardLevel >= minStewardLevel)
                {
                    canOfferDinner = true;
                    
                    stewardAppendedText = new TextObject("{=Steward_Appended_Text}[Steward - lvl {minStewardLevel}]")
                        .SetTextVariable("minStewardLevel", minStewardLevel)
                        .ToString();
                }
            }
            

            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("familyDemonym", familyDemonym)
                .SetTextVariable("closestSettlement", closestSettlement)
                .ToString();

            var eventOption1 = new TextObject("{=Travellers_Event_Option_1}[Engineering] Offer to repair the waggon").ToString();
            var eventOption1Hover = new TextObject("{=Travellers_Event_Option_1_Hover}You should be able to fix it.\n{engineeringAppendedText}")
                .SetTextVariable("engineeringAppendedText", engineeringAppendedText)
                .ToString();

            var eventOption2 = new TextObject("{=Travellers_Event_Option_2}[Roguery] Raid them!").ToString();
            var eventOption2Hover = new TextObject("{=Travellers_Event_Option_2_Hover}Take everything of value\n{rogueryAppendedText}")
                .SetTextVariable("rogueryAppendedText", rogueryAppendedText)
                .ToString();

            var eventOption3 = new TextObject("{=Travellers_Event_Option_3}[Steward] Offer them dinner").ToString();
            var eventOption3Hover = new TextObject("{=Travellers_Event_Option_3_Hover}While your men load all their belongings into a spare wagon you have {stewardAppendedText}")
                .SetTextVariable("stewardAppendedText", stewardAppendedText)
                .ToString();

            var eventOption4 = new TextObject("{=Travellers_Event_Option_4}Offer them directions").ToString();
            var eventOption4Hover = new TextObject("{=Travellers_Event_Option_4_Hover}Give them directions to the nearest settlement").ToString();
            
            var eventOption5 = new TextObject("{=Travellers_Event_Option_5}Ignore them").ToString();
            var eventOption5Hover = new TextObject("{=Travellers_Event_Option_5_Hover}This isn't your problem").ToString();
            
            var eventButtonText1 = new TextObject("{=Travellers_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=Travellers_Event_Button_Text_2}Done").ToString();
            

            var inquiryElements = new List<InquiryElement>();
            
            if (canRepairWagon)
            {
                inquiryElements.Add(new InquiryElement("a", eventOption1, null, true, eventOption1Hover));
            }
            if (canRaidWagon)
            {
                inquiryElements.Add(new InquiryElement("b", eventOption2, null, true, eventOption2Hover));
            }
            if (canOfferDinner)
            {
                inquiryElements.Add(new InquiryElement("c", eventOption3, null, true, eventOption3Hover));
            }
            inquiryElements.Add(new InquiryElement("d", eventOption4, null, true, eventOption4Hover));
            inquiryElements.Add(new InquiryElement("e", eventOption5, null, true, eventOption5Hover));

            
            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .SetTextVariable("closestSettlement", closestSettlement)
                .ToString();

            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventOutcome2())
                .SetTextVariable("stolenGold", stolenGold)
                .ToString();

            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .ToString();

            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .ToString();
            
            var eventOptionEText = new TextObject(EventTextHandler.GetRandomEventOutcome5())
                .ToString();
            
            
            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            var eventMsg2 =new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("stolenGold", stolenGold)
                .ToString();
            
            var eventMsg3 =new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            var eventMsg4 =new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("closestSettlement", closestSettlement)
                .ToString();
            
            var eventMsg5 =new TextObject(EventTextHandler.GetRandomEventMessage5())
                .SetTextVariable("heroName", heroName)
                .ToString();

            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1, eventButtonText1, null, elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_POS_Outcome));
                            
                            break;
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_EVIL_Outcome));
                            
                            break;
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_POS_Outcome));
                            
                            break;
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_POS_Outcome));
                            
                            break;
                        
                        case "e":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionEText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg5, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            
                            break;
                        default:
                            MessageBox.Show($"Error while selecting option for \"{randomEventData.eventType}\"");
                            break;
                    }
                }, null, null);

            MBInformationManager.ShowMultiSelectionInquiry(msid, true);
            
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
                "{=Travellers_Title_A}Travellers",
                "{=Travellers_Title_B}The Broken Wagon",
                "{=Travellers_Title_C}Fugitives on the Road",
                "{=Travellers_Title_D}A Family in Need",
                "{=Travellers_Title_E}Strangers on the Path",
                "{=Travellers_Title_F}The Wagon in the River",
                "{=Travellers_Title_G}Runaways",
                "{=Travellers_Title_H}Fleeing the Bandits",
                "{=Travellers_Title_I}A Family's Desperate Flight",
                "{=Travellers_Title_J}The Refugees"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=Travellers_Event_Desc_A}Your party is traveling near {closestSettlement} when you encounter a {familyDemonym} family with a broken wagon. " +
                "The wheel has come off and rolled into a small river nearby. The family pleads for your help, explaining that they are fleeing from bandits " +
                "who have extorted them for money and even tried to kidnap their two daughters multiple times. They’ve packed all their belongings in hopes of starting anew. " +
                "How will you proceed?",
                
                //Event Description B
                "{=Travellers_Event_Desc_B}As your party moves through the area near {closestSettlement}, you come across a {familyDemonym} family stranded with a broken wagon. " +
                "One of the wheels has detached and rolled into a river close by. The family explains their plight, fleeing bandits who extorted them and threatened to kidnap " +
                "their daughters. Desperate for a fresh start, they ask for your help. What will you do?",
                
                //Event Description C
                "{=Travellers_Event_Desc_C}While traveling near {closestSettlement}, you come upon a {familyDemonym} family in distress. Their wagon is broken, its wheel " +
                "having rolled into a nearby river. They explain they are running from bandits who have terrorized them with extortion and attempted kidnappings of their daughters. " +
                "Carrying all they own, they seek your help to move forward. What is your choice?",
                
                //Event Description D
                "{=Travellers_Event_Desc_D}Your party is passing not far from {closestSettlement} when you spot a {familyDemonym} family stranded with a broken wagon. " +
                "The detached wheel rests in a nearby river, and the family is clearly desperate. They share their story of fleeing bandits who blackmailed them and attempted " +
                "to abduct their daughters. They’ve brought everything they own, seeking refuge and a new beginning. Will you help them?",
                
                //Event Description E
                "{=Travellers_Event_Desc_E}Traveling near {closestSettlement}, you find a {familyDemonym} family struggling with a broken wagon. The wheel has rolled into " +
                "a nearby river, leaving them stranded. They explain they are fleeing from bandits who extorted them and made repeated attempts to kidnap their daughters. " +
                "They’ve risked everything to start over. How do you respond?"
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Choice 1A
                "{=Travellers_Event_Choice_1A}You inspect the wagon and quickly confirm that the damage can be repaired with relative ease. " +
                "You instruct your men to retrieve some tools from the party's supplies and send one man into the river to fetch the missing wheel. " +
                "Meanwhile, you tell the family to rest under the shade of a large oak tree while the children are given drinks and snacks.\n\n" +
                "Working efficiently with a few of your men, you repair the wagon and restore it to working order. You inform the family that their wagon " +
                "is ready to continue their journey. Grateful, the man offers his thanks, but you assure him that no thanks are needed. You help them load " +
                "their belongings and provide directions to {closestSettlement}, advising them to have the wagon repaired properly there. The man promises " +
                "to tell tales of your kindness, but you doubt he’ll heed your insistence that it’s unnecessary.",

                //Choice 1B
                "{=Travellers_Event_Choice_1B}Examining the wagon, you determine that the damage is minimal and can be fixed quickly. You instruct your men " +
                "to fetch tools and send another to retrieve the wheel from the river. You invite the family to rest beneath a nearby oak tree, where the children " +
                "are given refreshments and snacks.\n\nWith the assistance of your men, the wagon is repaired in no time. Approaching the family, you inform them " +
                "that they can continue their journey. The man thanks you profusely, but you wave off his gratitude. Helping them load their belongings, you point " +
                "them toward {closestSettlement}, where they can get a proper repair. The man vows to share tales of your generosity, though you tell him it’s unnecessary.",

                //Choice 1C
                "{=Travellers_Event_Choice_1C}You examine the wagon and confirm that the damage can be repaired without much difficulty. Calling for tools and assistance " +
                "from your men, you also instruct one of them to retrieve the wheel from the river. While you work, you encourage the family to relax under the shade " +
                "of an oak tree, providing refreshments for the children.\n\nWith a few skilled hands, the wagon is repaired quickly. You inform the family that they are " +
                "ready to continue their journey. The man expresses deep gratitude, but you assure him it’s not necessary. You help them reload the wagon and direct them " +
                "to {closestSettlement} for further repairs. Despite your protest, the man insists he will share stories of your kindness.",

                //Choice 1D
                "{=Travellers_Event_Choice_1D}Assessing the damage, you conclude that the wagon can be fixed easily. Directing your men to gather tools and retrieve the wheel " +
                "from the river, you invite the family to rest in the shade of a nearby tree. Snacks and drinks are offered to the children to help them relax.\n\n" +
                "With your men’s assistance, you repair the wagon efficiently. You inform the family of the good news and assist them in reloading their belongings. " +
                "The man thanks you sincerely, but you dismiss his gratitude as unnecessary. Pointing them toward {closestSettlement} for a thorough repair, you wish " +
                "them well, though you suspect the man will still spread tales of your generosity.",

                //Choice 1E
                "{=Travellers_Event_Choice_1E}You take a closer look at the wagon and confirm that it’s a simple fix. Calling for tools from your supplies and " +
                "sending one of your men to fetch the wheel from the river, you instruct the family to rest beneath a nearby oak tree. Refreshments and snacks are " +
                "provided to the children to lift their spirits.\n\nWorking quickly with your men, the wagon is repaired in no time. You let the family know that " +
                "they can now continue their journey and assist them in reloading their wagon. The man thanks you profusely, but you insist it’s not necessary. " +
                "You direct them to {closestSettlement} for a proper repair, though the man promises to tell others of your kindness despite your protests."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Choice 2A
                "{=Travellers_Event_Choice_2A}You order your men to remove the family from the wagon but instruct them not to harm anyone, as there are children involved. " +
                "The father tries to resist but is quickly subdued by your men, who bind his hands. His wife pleads with him to comply, and he reluctantly agrees.\n\n" +
                "Your men begin searching the wagon for valuables, but their efforts yield little of worth. Frustrated, you take the mother aside and, in a threatening tone, " +
                "tell her that if she doesn’t reveal all their valuables, you will kill her husband, sell the children into slavery, and keep her for your own purposes. " +
                "Though you don’t mean it, your tone convinces her otherwise.\n\nTerrified, she reveals the location of their hidden valuables. After searching, you manage to steal {stolenGold} gold from them. " +
                "Before leaving, you free the woman and instruct her to release her family only after your men are out of sight. Despite your actions, you direct them to the nearest settlement where they might find help.",

                //Choice 2B
                "{=Travellers_Event_Choice_2B}You instruct your men to remove the family from the wagon, warning them not to harm anyone, especially the children. " +
                "The father struggles but is overpowered and tied up by your men. His wife begs him to stop resisting, and he finally relents.\n\n" +
                "Your men rummage through the wagon, hoping to find valuables, but the results are disappointing. Turning to the mother, you issue a dire warning: " +
                "if she doesn’t cooperate, you’ll harm her family in unspeakable ways. Although you have no intention of following through, the threat is enough to terrify her. " +
                "She reveals their hidden valuables, and you successfully take {stolenGold} gold.\n\nBefore departing, you release the woman and tell her to free her family once you and your men are gone. " +
                "You also provide directions to the nearest settlement, where they might seek aid.",

                //Choice 2C
                "{=Travellers_Event_Choice_2C}You order your men to separate the family from the wagon, emphasizing that no harm should come to them due to the presence of children. " +
                "The father resists but is quickly subdued and bound by your men. His wife urges him to comply, and he reluctantly does.\n\n" +
                "Your men search the wagon for anything valuable, but their efforts are largely fruitless. Frustrated, you confront the mother and issue a cruel threat, " +
                "pretending you will harm her family if she doesn’t cooperate. Horrified, she reveals the location of their valuables. You manage to take {stolenGold} gold from them.\n\n" +
                "After securing the gold, you release the woman and tell her to wait until your men have left to free her family. Despite your actions, you direct them toward the nearest settlement for assistance.",

                //Choice 2D
                "{=Travellers_Event_Choice_2D}You have your men escort the family away from the wagon, ensuring they don’t harm anyone, particularly the children. " +
                "The father resists but is quickly restrained by your men, who bind his hands. His wife convinces him to stop struggling, and he complies.\n\n" +
                "Your men search the wagon but find little of value. Turning to the mother, you deliver a terrifying ultimatum, claiming you’ll harm her family if she doesn’t cooperate. " +
                "Though it’s an empty threat, she believes you and reveals their hidden valuables. You collect {stolenGold} gold from the family.\n\n" +
                "Once finished, you release the woman and instruct her to free her family only after you and your men are gone. As a final act, you provide directions to the nearest settlement, " +
                "where they can find assistance.",

                //Choice 2E
                "{=Travellers_Event_Choice_2E}You command your men to separate the family from the wagon, cautioning them to handle the situation carefully because of the children. " +
                "The father struggles but is restrained and tied by your men. His wife, in tears, persuades him to stop resisting, and he begrudgingly complies.\n\n" +
                "After searching the wagon to no avail, you confront the mother with a grim threat, pretending you’ll harm her family if she doesn’t reveal where their valuables are hidden. " +
                "Terrified, she divulges their location. Your men collect {stolenGold} gold from the wagon.\n\n" +
                "You release the woman and tell her to wait until your men are out of sight to free her family. Before leaving, you direct them to the nearest settlement, " +
                "suggesting they might find help there."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Choice 3A
                "{=Travellers_Event_Choice_3A}Noticing the exhaustion and hunger etched on the family's faces, you ask if you can prepare a meal for them. " +
                "Though hesitant at first, they eventually agree. You order your men to set up a small tent for the family while you take it upon yourself to cook their dinner. " +
                "Meanwhile, some of your men work on repairing the wagon.\n\n" +
                "You and a few of your men bring drinks for the family as you get to work on the meal. After some time, the family finishes eating just as your men announce that the wagon is repaired. " +
                "You help them reload their belongings and provide directions to the nearest settlement. As they depart, the mother expresses her gratitude, calling you an extraordinary man.",

                //Choice 3B
                "{=Travellers_Event_Choice_3B}Seeing how tired and hungry the family looks, you offer to prepare a meal for them. Initially hesitant, they accept your kindness. " +
                "You instruct your men to set up a small tent for the family while you personally handle the cooking. Meanwhile, your men begin repairing the wagon.\n\n" +
                "Bringing drinks to the family, you set to work preparing dinner. After they finish their meal, your men report that the wagon has been repaired. " +
                "You assist the family in reloading their belongings and guide them toward the nearest settlement. The mother, deeply moved, calls you an extraordinary person.",

                //Choice 3C
                "{=Travellers_Event_Choice_3C}Realizing the family’s dire state, you offer to prepare a meal for them. Though wary, they accept your offer. " +
                "You order your men to erect a small tent for the family while you take charge of cooking. At the same time, your men start work on fixing the wagon.\n\n" +
                "With drinks brought out for the family, you focus on preparing a hearty dinner. By the time they finish eating, your men have completed the wagon repairs. " +
                "You help the family load up their belongings and provide directions to the nearest settlement. The mother praises your kindness, calling you remarkable.",

                //Choice 3D
                "{=Travellers_Event_Choice_3D}You notice the family’s exhaustion and apparent hunger and offer to cook them a meal. While they’re hesitant at first, they eventually agree. " +
                "You instruct your men to set up a small tent for their comfort while you prepare the meal yourself. Meanwhile, the wagon repair begins.\n\n" +
                "Bringing drinks for the family, you get to work on dinner. After their meal, your men inform you that the wagon is repaired. " +
                "You help the family reload their belongings and guide them to the nearest settlement. As they prepare to leave, the mother expresses her heartfelt gratitude, calling you extraordinary.",

                //Choice 3E
                "{=Travellers_Event_Choice_3E}Noticing their exhaustion and likely days without food, you offer to prepare a meal for the family. Though cautious, they accept your help. " +
                "You direct your men to pitch a small tent for their comfort while you personally handle cooking. Simultaneously, your men begin repairing the wagon.\n\n" +
                "After offering drinks to the family, you prepare a meal with care. Once they finish eating, your men report that the wagon is fully repaired. " +
                "You assist them in loading their belongings and provide directions to the nearest settlement. The mother, overwhelmed with gratitude, calls you an extraordinary person."
            };
            
            private static readonly List<string> EventOutcome4 = new List<string>
            {
                //Choice 4A
                "{=Travellers_Event_Choice_4A}You stop to ask what happened and learn that they lost control of their wagon, which ended up in a ditch. " +
                "Inspecting the wagon, you determine that neither you nor your men have the skills to repair it. The family asks if they can buy a wagon " +
                "from your party for 500 gold, but you insist on giving them one for free, recognizing their distress and need for help.\n\n" +
                "You order your men to bring forth a spare wagon. Together with the family and some of your men, you help transfer their belongings to the new wagon. " +
                "Once the transfer is complete, you provide them with directions to the nearest settlement. They thank you profusely as you part ways.",

                //Choice 4B
                "{=Travellers_Event_Choice_4B}You ask the family what happened and they explain that their wagon veered off the road into a ditch. " +
                "After examining the wagon, you conclude that the damage is beyond the abilities of you or your men to repair. They offer to buy a wagon from your party " +
                "for 500 gold, but you decline their payment, offering to provide one for free given their dire situation.\n\n" +
                "You instruct your men to bring a spare wagon, and together with the family, you transfer their belongings to the new one. " +
                "Once everything is loaded, you give them directions to the nearest settlement. They express their heartfelt gratitude as you bid them farewell.",

                //Choice 4C
                "{=Travellers_Event_Choice_4C}Stopping to inquire about their situation, you learn that their wagon ended up in a ditch after they lost control of it. " +
                "Looking over the damage, it becomes clear that neither you nor your men can fix it. The family asks if they can purchase a wagon for 500 gold, but you " +
                "decline the payment, deciding to help them by giving them one for free.\n\n" +
                "Your men retrieve a spare wagon, and with the family’s assistance, you transfer their belongings to the new one. Once the task is complete, you provide " +
                "directions to the nearest settlement. The family thanks you repeatedly as they prepare to continue their journey.",

                //Choice 4D
                "{=Travellers_Event_Choice_4D}You approach the family and ask what happened. They explain that their wagon ran into a ditch and became unusable. " +
                "After inspecting the damage, you realize it’s too complex for you or your men to fix. They offer to purchase a wagon from your party for 500 gold, " +
                "but you refuse to take their money, opting to provide one for free instead.\n\n" +
                "Your men bring out a spare wagon, and with everyone’s help, the family’s belongings are transferred. Once the transfer is complete, you guide them toward " +
                "the nearest settlement. They express their profound gratitude as you leave them to continue their journey.",

                //Choice 4E
                "{=Travellers_Event_Choice_4E}The family explains their predicament, saying their wagon ended up in a ditch and they cannot continue. " +
                "After inspecting the wagon, you confirm that neither you nor your men can repair it. When they ask to buy a wagon for 500 gold, you refuse their payment, " +
                "deciding to provide one for free out of compassion.\n\n" +
                "You instruct your men to bring a spare wagon, and together with the family, you transfer their belongings. When everything is ready, you give them directions " +
                "to the nearest settlement. They thank you warmly as you part ways."
            };
            
            private static readonly List<string> EventOutcome5 = new List<string>
            {
                //Choice 5A
                "{=Travellers_Event_Choice_5A}You decide that you cannot be bothered with this situation and inform the family that you cannot help. " +
                "As they try to ask for directions, you walk away without answering. A few moments later, you overhear some of your men stopping " +
                "to offer the family directions to the nearest settlement and some coin.",
                
                //Choice 5B
                "{=Travellers_Event_Choice_5B}Unwilling to deal with the situation, you tell the family that you cannot assist them. Ignoring their request for directions, " +
                "you walk on. However, you soon hear that a few of your men have paused to provide the family with directions to the nearest settlement " +
                "and even offer them some coin.",
                
                //Choice 5C
                "{=Travellers_Event_Choice_5C}You dismiss the family’s plight, telling them you are unable to help. When they ask for directions, you refuse to respond " +
                "and continue on your way. After a short distance, you overhear some of your men stopping to guide the family to the nearest settlement " +
                "and giving them some money.",
                
                //Choice 5D
                "{=Travellers_Event_Choice_5D}Not wanting to involve yourself, you tell the family you cannot help and ignore their plea for directions. " +
                "As you walk away, you hear that some of your men have stayed behind to assist, offering the family directions to the nearest settlement " +
                "and giving them a bit of coin.",
                
                //Choice 5E
                "{=Travellers_Event_Choice_5E}You inform the family that you cannot help and disregard their request for directions. " +
                "Walking away, you soon hear that a few of your men have taken it upon themselves to stop, give the family directions " +
                "to the nearest settlement, and hand them some coin."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=Travellers_Event_Msg_1A}{heroName} was able to fix the stranded family's wagon.",
                "{=Travellers_Event_Msg_1B}{heroName} successfully repaired the wagon for the stranded family.",
                "{=Travellers_Event_Msg_1C}{heroName} fixed the broken wagon, allowing the family to continue their journey.",
                "{=Travellers_Event_Msg_1D}{heroName} repaired the stranded family's wagon and sent them on their way.",
                "{=Travellers_Event_Msg_1E}The stranded family’s wagon was repaired thanks to {heroName}."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=Travellers_Event_Msg_2A}{heroName} stole {stolenGold} gold from the stranded family and left them.",
                "{=Travellers_Event_Msg_2B}{heroName} took {stolenGold} gold from the stranded family before leaving them behind.",
                "{=Travellers_Event_Msg_2C}{heroName} robbed the stranded family of {stolenGold} gold and departed.",
                "{=Travellers_Event_Msg_2D}After taking {stolenGold} gold from the stranded family, {heroName} left them to their fate.",
                "{=Travellers_Event_Msg_2E}{heroName} stole {stolenGold} gold from the family and walked away without helping them."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=Travellers_Event_Msg_3A}{heroName} made dinner while the men fixed the wagon.",
                "{=Travellers_Event_Msg_3B}{heroName} prepared a meal for the stranded family as the men repaired the wagon.",
                "{=Travellers_Event_Msg_3C}While the men worked on the wagon, {heroName} cooked dinner for the family.",
                "{=Travellers_Event_Msg_3D}As the men fixed the wagon, {heroName} took the time to prepare dinner for the family.",
                "{=Travellers_Event_Msg_3E}{heroName} cooked dinner for the family while the wagon was being repaired."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            { 
                "{=Travellers_Event_Msg_4A}{heroName} gave the stranded family directions to {closestSettlement}.",
                "{=Travellers_Event_Msg_4B}{heroName} provided the stranded family with directions to {closestSettlement}.",
                "{=Travellers_Event_Msg_4C}The stranded family received directions to {closestSettlement} from {heroName}.",
                "{=Travellers_Event_Msg_4D}{heroName} directed the stranded family toward {closestSettlement}.",
                "{=Travellers_Event_Msg_4E}{heroName} pointed the stranded family toward {closestSettlement}."
            };
            
            private static readonly List<string> eventMsg5 = new List<string>
            { 
                "{=Travellers_Event_Msg_5A}{heroName} ignored the stranded family. At least the men aren't that heartless!",
                "{=Travellers_Event_Msg_5B}{heroName} walked away from the stranded family, but the men showed some kindness.",
                "{=Travellers_Event_Msg_5C}While {heroName} ignored the stranded family, the men offered some help.",
                "{=Travellers_Event_Msg_5D}{heroName} showed no concern for the stranded family, but the men stepped in to assist.",
                "{=Travellers_Event_Msg_5E}The stranded family was ignored by {heroName}, but the men offered some aid."
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
            
            public static string GetRandomEventOutcome5()
            {
                var index = random.Next(EventOutcome5.Count);
                return EventOutcome5[index];
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
            
            public static string GetRandomEventMessage5()
            {
                var index = random.Next(eventMsg5.Count);
                return eventMsg5[index];
            }
        }
    }


    public class TravellersData : RandomEventData
    {
        public TravellersData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new Travellers();
        }
    }
}