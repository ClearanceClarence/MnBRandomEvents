using System;
using System.Collections.Generic;
using System.Windows;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class OldRuins : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minMen;
        private readonly int maxMen;
        private readonly int maxMenToKill;
        private readonly int minGoldFound;
        private readonly int maxGoldFound;

        public OldRuins() : base(ModSettings.RandomEvents.OldRuinsData)
        {
            
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("OldRuins", "EventDisabled");
            minMen = ConfigFile.ReadInteger("OldRuins", "MinMen");
            maxMen = ConfigFile.ReadInteger("OldRuins", "MaxMen");
            maxMenToKill = ConfigFile.ReadInteger("OldRuins", "MaxMenToKill");
            minGoldFound = ConfigFile.ReadInteger("OldRuins", "MinGoldFound");
            maxGoldFound = ConfigFile.ReadInteger("OldRuins", "MaxGoldFound");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minMen != 0 || maxMen != 0 || maxMenToKill != 0 || minGoldFound != 0 || maxGoldFound != 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement == null  && MobileParty.MainParty.MemberRoster.TotalRegulars >= maxMen;
        }

        public override void StartEvent()
        {
            var heroName = Hero.MainHero.FirstName;
            
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
            
            var manCount  = MBRandom.RandomInt(minMen, maxMen);

            var killedMen = MBRandom.RandomInt(1, maxMenToKill);

            var goldFound = MBRandom.RandomInt(minGoldFound, maxGoldFound);

            var goldForYou = goldFound / manCount ;
            
            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("manCount", manCount)
                .ToString();
            
            var eventOption1 = new TextObject("{=OldRuins_Event_Option_1}The old farmhouse").ToString();
            var eventOption1Hover = new TextObject("{=OldRuins_Event_Option_1_Hover}Better to check the homestead").ToString();
            
            var eventOption2 = new TextObject("{=OldRuins_Event_Option_2}The old well").ToString();
            var eventOption2Hover = new TextObject("{=OldRuins_Event_Option_2_Hover}...It's a well").ToString();
            
            var eventOption3 = new TextObject("{=OldRuins_Event_Option_3}The barn").ToString();
            var eventOption3Hover = new TextObject("{=OldRuins_Event_Option_3_Hover}Barns always hold something interesting").ToString();
            
            var eventOption4 = new TextObject("{=OldRuins_Event_Option_4}The small shack").ToString();
            var eventOption4Hover = new TextObject("{=OldRuins_Event_Option_4_Hover}The shack might be interesting").ToString();
            
            var eventOption5 = new TextObject("{=OldRuins_Event_Option_5}None of them, just leave").ToString();
            var eventOption5Hover = new TextObject("{=OldRuins_Event_Option_5_Hover}You don't want to get wet").ToString();
            
            var eventButtonText1 = new TextObject("{=OldRuins_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=OldRuins_Event_Button_Text_2}Done").ToString();

            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
                new InquiryElement("c", eventOption3, null, true, eventOption3Hover),
                new InquiryElement("d", eventOption4, null, true, eventOption4Hover),
                new InquiryElement("e", eventOption5, null, true, eventOption5Hover)
            };
            
            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .SetTextVariable("killedMen", killedMen)
                .ToString();
            
            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventOutcome2())
                .ToString();

            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .ToString();
            
            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .SetTextVariable("goldFound",goldFound)
                .SetTextVariable("men",manCount)
                .SetTextVariable("goldForYou",goldForYou)
                .ToString();
            
            var eventOptionEText = new TextObject(EventTextHandler.GetRandomEventOutcome5())
                .ToString();
            
            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("killedMen", killedMen)
                .ToString();
            
            var eventMsg2 =new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            var eventMsg3 =new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            var eventMsg4 =new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("goldForYou", goldForYou)
                .SetTextVariable("goldFound", goldFound)
                .SetTextVariable("manCount", manCount )
                .ToString();
            
            var eventMsg5 =new TextObject(EventTextHandler.GetRandomEventMessage5())
                .SetTextVariable("heroName", heroName)
                .ToString();
            

            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1, eventButtonText1, null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            
                            MobileParty.MainParty.MemberRoster.KillNumberOfNonHeroTroopsRandomly(killedMen);
                            break;
                        
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                            break;
                        
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                            break;
                        
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_POS_Outcome));
                            
                            Hero.MainHero.ChangeHeroGold(+goldForYou);
                            break;
                        case "e":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionEText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg5, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                            break;
                        
                        default:
                            MessageBox.Show($"Error while selecting option for \"{randomEventData.eventType}\"");
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
                "{=OldRuins_Title_A}The Old Ruins",
                "{=OldRuins_Title_B}The Forgotten Settlement",
                "{=OldRuins_Title_C}Ruins of the Past",
                "{=OldRuins_Title_D}The Abandoned Village",
                "{=OldRuins_Title_E}Echoes of an Empty Town",
                "{=OldRuins_Title_F}The Deserted Hamlet",
                "{=OldRuins_Title_G}Shadows of a Lost Settlement",
                "{=OldRuins_Title_H}The Crumbling Remains",
                "{=OldRuins_Title_I}Relics of a Forgotten Era",
                "{=OldRuins_Title_J}The Silent Ruins"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=OldRuins_Event_Desc_A}You are traveling through the lands near {closestSettlement} when you come across the ruins of a small abandoned settlement. You don't remember there being any " +
                "settlements out here on any map, so you tell your men to set up camp nearby. You and {manCount} of your men decide to investigate. \n\nWhen you enter the settlement, it becomes apparent " +
                "that it has been abandoned for a long time. There is an old farmhouse, well, barn, and a small shack at the settlement. You ask your men where they want to check first, but before " +
                "they can answer, a bolt of lightning splits the sky, followed by a deafening thunderclap. You decide there’s only time to search one location before heading back. Which will it be?",
                
                //Event Description B
                "{=OldRuins_Event_Desc_B}While traveling near {closestSettlement}, you stumble upon the ruins of an old settlement. It's not marked on any maps you’ve seen, so you have your men set up camp nearby. " +
                "Taking {manCount} of your men, you decide to investigate further. \n\nThe settlement appears long abandoned, with an old farmhouse, a well, a barn, and a small shack among the ruins. " +
                "As you deliberate where to start searching, a sudden lightning bolt cuts through the sky, followed by a roar of thunder. Realizing time is short, you decide to explore one location before returning.",
                
                //Event Description C
                "{=OldRuins_Event_Desc_C}As your party journeys near {closestSettlement}, you come across the ruins of an unmarked settlement. Curious, you instruct your men to establish a camp close by " +
                "while you and {manCount} others explore the area. \n\nThe settlement shows signs of long abandonment, with a farmhouse, a well, a barn, and a small shack still standing. Before you can decide " +
                "where to begin searching, a flash of lightning illuminates the ruins, followed by a loud rumble of thunder. You realize there’s only time to investigate one spot before retreating to camp.",
                
                //Event Description D
                "{=OldRuins_Event_Desc_D}While passing through the lands near {closestSettlement}, your party discovers the remnants of a forgotten settlement. Noticing that it’s unmarked on any map, you order " +
                "your men to set up camp nearby and take {manCount} with you to investigate. \n\nThe settlement is clearly abandoned, with only a farmhouse, a well, a barn, and a shack left standing. As you " +
                "begin to decide where to search, a lightning bolt splits the sky, followed by a deafening thunderclap. Recognizing the storm's arrival, you decide there’s time for only one quick search.",
                
                //Event Description E
                "{=OldRuins_Event_Desc_E}Your party is traveling near {closestSettlement} when you come upon the ruins of a small settlement. It doesn’t appear on any maps, so you direct your men to make camp nearby. " +
                "Taking {manCount} of them, you venture into the ruins to investigate. \n\nThe settlement seems to have been abandoned for many years, with a farmhouse, a well, a barn, and a shack still standing. " +
                "As you discuss where to start searching, a sudden bolt of lightning illuminates the ruins, followed by a thunderous roar. You realize you have time to check only one place before returning."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Outcome 1A
                "{=OldRuins_Event_Choice_1A}You decide to check the farmhouse. You approach the door and push it " +
                "inwards, causing it to fall off its hinges as the building groans ominously. Ignoring the warning signs, " +
                "you and your men scatter inside to search for anything valuable. In the living room, you find nothing but " +
                "dust, bugs, and cobwebs. As you step outside to check on the horses, a deafening crash echoes behind you. " +
                "The entire farmhouse collapses.\n\nPanicked, you call out to your men, but there’s no response. Returning to " +
                "camp, you gather additional soldiers to help recover those trapped. Tragically, only two men survive, while " +
                "{killedMen} others are lost in the rubble.",
                
                //Event Outcome 1B
                "{=OldRuins_Event_Choice_1B}You head toward the farmhouse and push the creaking door open. It falls off, " +
                "revealing a structure on the verge of collapse. Undeterred, you and your men spread out to search the house. " +
                "In the living room, you find nothing noteworthy, only insects and decay. As you step outside to check on your " +
                "horses, the building suddenly collapses behind you with a thunderous crash.\n\nYou shout for your men, but " +
                "only silence greets you. Rushing back to camp, you gather reinforcements to rescue those trapped. Sadly, " +
                "only two men are pulled out alive, while {killedMen} others perish in the collapse.",
                
                //Event Outcome 1C
                "{=OldRuins_Event_Choice_1C}You choose to investigate the farmhouse. The door falls off as you push it open, " +
                "and the building creaks ominously. You instruct your men to search the interior while you explore the living room, " +
                "finding only cobwebs and bugs. As you leave the building to check on the horses, a thunderous crash behind you " +
                "marks the farmhouse's collapse.\n\nFrantic, you call for your men but hear no reply. You quickly gather reinforcements " +
                "from camp to dig through the rubble. Tragically, only two men survive, while {killedMen} others are found dead.",
                
                //Event Outcome 1D
                "{=OldRuins_Event_Choice_1D}Deciding to inspect the farmhouse, you push the door open, and it falls off its frame, " +
                "revealing a precariously unstable structure. You send your men to search while you head into the living room, " +
                "only to find it empty except for insects and dust. Stepping outside to check on your horses, you hear a loud crash " +
                "as the building collapses.\n\nShouting for your men yields no response. Back at camp, you organize a rescue effort. " +
                "Despite your best efforts, only two men survive, while {killedMen} are lost under the rubble.",
                
                //Event Outcome 1E
                "{=OldRuins_Event_Choice_1E}The farmhouse draws your attention, and you decide to investigate. The door comes " +
                "off its hinges with a groan, and the entire structure creaks ominously. You instruct your men to search the " +
                "building, while you check the living room, which holds nothing but bugs and cobwebs. As you step back outside, " +
                "the farmhouse collapses with a deafening roar.\n\nPanicked, you call out to your men but receive no answer. " +
                "You quickly organize reinforcements from camp to assist in the rescue. Unfortunately, only two men are saved, " +
                "while {killedMen} others lose their lives."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Event Outcome 2A
                "{=OldRuins_Event_Choice_2A}You decide to investigate the well since it’s the closest structure. Gathering around it, you peer down into the darkness. " +
                "After a moment, you realize there’s nothing but dirt at the bottom. Disappointed, you decide to leave.",
                
                //Event Outcome 2B
                "{=OldRuins_Event_Choice_2B}The well catches your attention, and you decide to inspect it first. Your group circles the well, looking down into its depths. " +
                "To your dismay, there’s nothing inside—just a layer of dirt at the bottom. With nothing else to find, you move on.",
                
                //Event Outcome 2C
                "{=OldRuins_Event_Choice_2C}Choosing the well as your first stop, you and your men gather around and peer inside. The well is dry, and only dirt lines the bottom. " +
                "Finding nothing of interest, you decide to leave and explore elsewhere.",
                
                //Event Outcome 2D
                "{=OldRuins_Event_Choice_2D}You head to the well, hoping it might hold something valuable. Encircling it, you lean over to peer inside, only to discover it’s empty. " +
                "Disappointed, you decide to leave and search elsewhere.",
                
                //Event Outcome 2E
                "{=OldRuins_Event_Choice_2E}The well is your first choice, as it’s nearby. You and your men gather around and inspect it. Looking down, you see nothing but dirt " +
                "at the bottom. Finding no reason to linger, you quickly move on."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Event Outcome 3A
                "{=OldRuins_Event_Choice_3A}The men suggest checking the barn, and you agree. Inside, you find the skeletons of what were likely farm animals, scattered among the debris. " +
                "The men spread out to search, but they find nothing of value. Disappointed, you decide to leave.",
                
                //Event Outcome 3B
                "{=OldRuins_Event_Choice_3B}You agree with the men’s suggestion to investigate the barn. Inside, you see a few animal skeletons lying amidst the rotting wood. " +
                "The men fan out to search every corner, but their efforts yield nothing. With nothing left to find, you leave.",
                
                //Event Outcome 3C
                "{=OldRuins_Event_Choice_3C}Deciding to investigate the barn, you and your men enter cautiously. The interior holds little more than the skeletal remains of farm animals. " +
                "The men search the area thoroughly, but they come back empty-handed. With nothing of interest, you leave.",
                
                //Event Outcome 3D
                "{=OldRuins_Event_Choice_3D}The barn becomes the focus of your investigation, and you step inside with your men. It’s dark, and only the skeletons of long-dead animals remain. " +
                "Your men search the barn diligently but find nothing worth taking. Disheartened, you decide to move on.",
                
                //Event Outcome 3E
                "{=OldRuins_Event_Choice_3E}Agreeing to check the barn, you and your men step inside. The place is in disarray, with skeletal remains of farm animals scattered throughout. " +
                "Despite a thorough search by your men, the barn offers nothing of value. You leave empty-handed."
            };
            
            private static readonly List<string> EventOutcome4 = new List<string>
            {
                //Event Outcome 4A
                "{=OldRuins_Event_Choice_4A}You all agree to head to the shack. Forcing the door open, you find a chest with a lock inside. After a few minutes of effort, you manage to break it open. " +
                "The chest contains mostly old papers, but at the very bottom, you discover a hefty coin purse. Opening it reveals a collection of gold coins. " +
                "You return to camp and split the gold with your men. You found {goldFound} gold, and with {men} men accompanying you, each receives {goldForYou}.",
                
                //Event Outcome 4B
                "{=OldRuins_Event_Choice_4B}The shack catches your group’s interest, and you decide to investigate. Inside, you find an old chest with a rusted lock. " +
                "After some effort, you manage to break it open. Most of the chest’s contents are useless papers, but hidden at the bottom is a coin purse. " +
                "To your surprise, it’s filled with gold coins. Back at camp, you divide the {goldFound} gold among the {men} men, each receiving {goldForYou}.",
                
                //Event Outcome 4C
                "{=OldRuins_Event_Choice_4C}Agreeing to explore the shack, you force the door open and find a chest with a sturdy lock. After some fidgeting, the lock gives way. " +
                "The chest is mostly filled with papers, but buried beneath them is a coin purse brimming with gold coins. Returning to camp, you split the {goldFound} gold evenly among {men} men, " +
                "each receiving {goldForYou}.",
                
                //Event Outcome 4D
                "{=OldRuins_Event_Choice_4D}Your group heads to the shack, where you pry open the door. Inside, you discover a locked chest. After working on it for a while, the lock snaps open, " +
                "revealing papers and, hidden at the bottom, a weighty coin purse. It’s packed with gold coins. Back at camp, you share the {goldFound} gold among the {men} men who joined you, " +
                "with each receiving {goldForYou}.",
                
                //Event Outcome 4E
                "{=OldRuins_Event_Choice_4E}The shack draws your attention, and you decide to check it out. Inside, you find a chest secured with a lock. " +
                "After some effort, you break the lock and rummage through its contents. Most are papers, but at the very bottom, you find a coin purse heavy with gold. " +
                "Returning to camp, you divide the {goldFound} gold equally among the {men} men, resulting in {goldForYou} each."
            };
            
            private static readonly List<string> EventOutcome5 = new List<string>
            {
                //Event Outcome 5A
                "{=OldRuins_Event_Choice_5A}You all agree that getting wet is not worth the risk, so you decide to head back to camp.",
                
                //Event Outcome 5B
                "{=OldRuins_Event_Choice_5B}Deciding to avoid the storm, you and your men agree to return to camp without exploring further.",
                
                //Event Outcome 5C
                "{=OldRuins_Event_Choice_5C}Not wanting to risk getting soaked in the approaching storm, your group unanimously decides to head back to camp.",
                
                //Event Outcome 5D
                "{=OldRuins_Event_Choice_5D}With the storm looming, you all agree it’s best to return to camp and stay dry rather than continue exploring.",
                
                //Event Outcome 5E
                "{=OldRuins_Event_Choice_5E}Unwilling to brave the rain, you and your men decide to abandon the investigation and make your way back to camp."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=OldRuins_Event_Msg_1A}{heroName} lost {killedMen} men to a collapsing structure.",
                "{=OldRuins_Event_Msg_1B}{killedMen} men under {heroName}'s command perished when a structure collapsed.",
                "{=OldRuins_Event_Msg_1C}A collapsing building claimed the lives of {killedMen} of {heroName}'s men.",
                "{=OldRuins_Event_Msg_1D}{heroName} lost {killedMen} men in a tragic structural collapse.",
                "{=OldRuins_Event_Msg_1E}{heroName}'s group suffered the loss of {killedMen} men due to a collapsing structure."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=OldRuins_Event_Msg_2A}{heroName} investigated the well but found nothing.",
                "{=OldRuins_Event_Msg_2B}The well revealed nothing of interest to {heroName} and their men.",
                "{=OldRuins_Event_Msg_2C}{heroName} found the well empty, with only dirt at the bottom.",
                "{=OldRuins_Event_Msg_2D}After inspecting the well, {heroName} and their men discovered it was empty.",
                "{=OldRuins_Event_Msg_2E}The well held no secrets, leaving {heroName} and their men disappointed."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=OldRuins_Event_Msg_3A}{heroName} searched the barn but found nothing of value.",
                "{=OldRuins_Event_Msg_3B}The barn contained only animal skeletons, leaving {heroName} empty-handed.",
                "{=OldRuins_Event_Msg_3C}{heroName} and their men searched the barn thoroughly but found nothing useful.",
                "{=OldRuins_Event_Msg_3D}The barn held no valuables, only remnants of long-dead animals.",
                "{=OldRuins_Event_Msg_3E}{heroName} found the barn empty, with nothing but dust and decay inside."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            {
                "{=OldRuins_Event_Msg_4A}{heroName} received {goldForYou} gold after splitting {goldFound} gold with {manCount} men.",
                "{=OldRuins_Event_Msg_4B}After dividing {goldFound} gold among {manCount} men, {heroName} kept {goldForYou} gold.",
                "{=OldRuins_Event_Msg_4C}{heroName} ended up with {goldForYou} gold after sharing {goldFound} gold with {manCount} others.",
                "{=OldRuins_Event_Msg_4D}{goldFound} gold was found, and {heroName} received {goldForYou} after splitting it among {manCount} men.",
                "{=OldRuins_Event_Msg_4E}Following the division of {goldFound} gold among {manCount} men, {heroName} kept {goldForYou} gold."
            };
            
            private static readonly List<string> eventMsg5 = new List<string>
            {
                "{=OldRuins_Event_Msg_5A}{heroName} decided to return to camp without further exploration.",
                "{=OldRuins_Event_Msg_5B}Facing the looming storm, {heroName} and their men chose to head back to camp.",
                "{=OldRuins_Event_Msg_5C}{heroName} returned to camp, deciding it was best to avoid the approaching rain.",
                "{=OldRuins_Event_Msg_5D}{heroName} and their group abandoned the ruins and made their way back to camp.",
                "{=OldRuins_Event_Msg_5E}Opting for safety, {heroName} led their men back to camp before the storm arrived."
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


    public class OldRuinsData : RandomEventData
    {

        public OldRuinsData(string eventType, float chanceWeight) : base(eventType,
            chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new OldRuins();
        }
    }
}