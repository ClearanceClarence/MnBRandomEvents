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
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.RandomEvents.Events.CommunityEvents
{
    public sealed class SecretsOfSteel : BaseEvent
    {
        private readonly bool eventDisabled;

        public SecretsOfSteel() : base(ModSettings.RandomEvents.SecretsOfSteelData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
			
            eventDisabled = ConfigFile.ReadBoolean("SecretsOfSteel", "EventDisabled");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            return eventDisabled == false && Hero.MainHero.GetSkillValue(DefaultSkills.Crafting) >= 120 && MobileParty.MainParty.MemberRoster.TotalRegulars >= 50;
        }

        public override bool CanExecuteEvent()
        {

            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement == null;
        }

        public override void StartEvent()
        {
            var mainHero = Hero.MainHero;

            var heroName = mainHero.FirstName;
            
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty);

            var smithingLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Crafting);

            var canHelp = false;

            var smithingAppendedText = "";

            if (GeneralSettings.SkillChecks.IsDisabled())
            {
                canHelp = true;

                //Fakes a smithing level
                smithingLevel = MBRandom.RandomInt(100, 275);

                smithingAppendedText = new TextObject("{=Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();
            }
            else
            {
                if (smithingLevel >= 100)
                {
                    canHelp = true;
                                    
                    smithingAppendedText = new TextObject("{=Smithing_Appended_Text}[Smithing - lvl {smithingLevel}]")
                        .SetTextVariable("smithingLevel",smithingLevel)
                        .ToString();
                }
            }

            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                    .SetTextVariable("closestSettlement", closestSettlement.Name.ToString())
                    .ToString();
            
            var eventOption1 = new TextObject("{=SecretsOfSteel_Event_Option_1}Decline").ToString();
            var eventOption1Hover = new TextObject("{=SecretsOfSteel_Event_Option_1_Hover}You cannot assist him").ToString();
            
            var eventOption2 = new TextObject("{=SecretsOfSteel_Event_Option_2}[Smithing] Accept").ToString();
            var eventOption2Hover = new TextObject("{=SecretsOfSteel_Event_Option_2_Hover}Agree to help him. {smithingAppendedText}").SetTextVariable("smithingAppendedText",smithingAppendedText).ToString();

            var eventButtonText1 = new TextObject("{=SecretsOfSteel_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=SecretsOfSteel_Event_Button_Text_2}Done").ToString();
            
            var inquiryElements = new List<InquiryElement> { new InquiryElement("a", eventOption1, null, true, eventOption1Hover) };

            if (canHelp)
            {
                inquiryElements.Add(new InquiryElement("b", eventOption2, null, true, eventOption2Hover));
            }

            var craftingSkillValue = Hero.MainHero.GetSkillValue(DefaultSkills.Crafting);
            
            var charcoal = MBObjectManager.Instance.GetObject<ItemObject>("charcoal");
            var steel = MBObjectManager.Instance.GetObject<ItemObject>("ironIngot4");
            var fineSteel = MBObjectManager.Instance.GetObject<ItemObject>("ironIngot5");
            var thamaskeneSteel = MBObjectManager.Instance.GetObject<ItemObject>("ironIngot6");

            var randomCharcoal = MBRandom.RandomInt(5, 20);
            var randomSteel = MBRandom.RandomInt(5, 20);
            var randomFineSteel = MBRandom.RandomInt(2, 10);
            var randomThamaskeneSteel = MBRandom.RandomInt(2, 10);

            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .ToString();
            
            var eventOptionBTextOutcome1 = new TextObject(EventTextHandler.GetRandomEventOutcome2A())
                .SetTextVariable("closestSettlement", closestSettlement.Name.ToString())
                .SetTextVariable("randomCharcoal",randomCharcoal)
                .ToString();
            
            var eventOptionBTextOutcome2 = new TextObject(EventTextHandler.GetRandomEventOutcome2B())
                .SetTextVariable("closestSettlement", closestSettlement.Name.ToString())
                .SetTextVariable("randomSteel",randomSteel)
                .ToString();
            
            var eventOptionBTextOutcome3 = new TextObject(EventTextHandler.GetRandomEventOutcome2C())
                .SetTextVariable("closestSettlement", closestSettlement.Name.ToString())
                .SetTextVariable("randomFineSteel",randomFineSteel)
                .SetTextVariable("randomThamaskeneSteel",randomThamaskeneSteel)
                .ToString();

            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            var eventMsg2A =new TextObject(EventTextHandler.GetRandomEventMessage2A())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            var eventMsg2B =new TextObject(EventTextHandler.GetRandomEventMessage2B())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            var eventMsg2C =new TextObject(EventTextHandler.GetRandomEventMessage2C())
                .SetTextVariable("heroName", heroName)
                .ToString();
            
            
            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1,
                    eventButtonText1, null,
                    elements =>
                    {
                        switch ((string)elements[0].Identifier)
                        {
                            case "a":
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                                InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                                break;
                            case "b" when smithingLevel >= 100 && smithingLevel <= 125:
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBTextOutcome1, true, false, eventButtonText2, null, null, null), true);
                                
                                MobileParty.MainParty.ItemRoster.AddToCounts(charcoal, randomCharcoal);
                                Hero.MainHero.AddSkillXp(DefaultSkills.Crafting,craftingSkillValue * 1.0075f);

                                InformationManager.DisplayMessage(new InformationMessage(eventMsg2A, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                                break;
                            case "b" when smithingLevel >= 126 && smithingLevel <= 175:
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBTextOutcome2, true, false, eventButtonText2, null, null, null), true);
                                
                                MobileParty.MainParty.ItemRoster.AddToCounts(steel, randomSteel);
                                Hero.MainHero.AddSkillXp(DefaultSkills.Crafting,craftingSkillValue * 1.015f);
                                
                                InformationManager.DisplayMessage(new InformationMessage(eventMsg2B, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                                break;
                            case "b" when smithingLevel >= 176:
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBTextOutcome3, true, false, eventButtonText2, null, null, null), true);
                                
                                MobileParty.MainParty.ItemRoster.AddToCounts(fineSteel, randomFineSteel);
                                MobileParty.MainParty.ItemRoster.AddToCounts(thamaskeneSteel, randomThamaskeneSteel);
                                
                                Hero.MainHero.AddSkillXp(DefaultSkills.Crafting,craftingSkillValue * 1.0225f);
                                
                                InformationManager.DisplayMessage(new InformationMessage(eventMsg2C, RandomEventsSubmodule.Msg_Color_POS_Outcome));
                                break;
                            default:
                                MessageManager.DisplayMessage($"Error while selecting option for \"{randomEventData.eventType}\"");
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
                MessageManager.DisplayMessage(
                    $"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }
        private static class EventTextHandler
        {
            private static readonly Random random = new Random();
            
            private static readonly List<string> eventTitles = new List<string>
            {
                "{=SecretsOfSteel_Title_A}Secrets Of Steel",
                "{=SecretsOfSteel_Title_B}Forging the Future",
                "{=SecretsOfSteel_Title_C}The Steelmaker's Secret",
                "{=SecretsOfSteel_Title_D}Anvil and Mysteries",
                "{=SecretsOfSteel_Title_E}Legends of the Forge",
                "{=SecretsOfSteel_Title_F}The Hidden Alloy",
                "{=SecretsOfSteel_Title_G}Mastering Metalcraft",
                "{=SecretsOfSteel_Title_H}The Lost Forge",
                "{=SecretsOfSteel_Title_I}Mysteries of the Blacksmith",
                "{=SecretsOfSteel_Title_J}Crafts of Iron and Fire"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=SecretsOfSteel_Event_Desc_A}While your party is travelling near {closestSettlement}, you come across a secluded forge with an old blacksmith in it. You hear him calling out to you, so you decide to " +
                "check what he wants. He explains to you that he once was one of the greatest blacksmiths in all of Calradia, but now he has become way too old to work alone. He explains that he had an apprentice, " +
                "but they left a few days ago, vowing never to return. He also explains that he has an order to fill but cannot do it without help. He looks you straight in the eyes and asks if you could consider helping " +
                "an old man complete his final order. He promises a generous reward for your assistance. What do you do?",

                //Event Description B
                "{=SecretsOfSteel_Event_Desc_B}Your journey near {closestSettlement} leads you to a hidden forge, where an elderly blacksmith beckons you closer. Once renowned as one of the greatest smiths in Calradia, " +
                "he now struggles with his craft due to his advanced age. He tells you about his apprentice, who recently left under bitter circumstances, leaving him alone with an unfinished order. The old man pleads for " +
                "your help, promising a worthwhile reward. Will you lend your skills to aid him in his time of need?",

                //Event Description C
                "{=SecretsOfSteel_Event_Desc_C}Travelling through the lands near {closestSettlement}, you stumble upon a small forge tucked away in the wilderness. An old blacksmith greets you, his hands trembling as he " +
                "explains his plight. Once a master of his trade, he now faces the challenge of completing an urgent order without assistance. His apprentice has abandoned him, leaving him desperate and alone. He offers a " +
                "reward in exchange for your help, his voice tinged with both hope and despair. What is your decision?",

                //Event Description D
                "{=SecretsOfSteel_Event_Desc_D}Near {closestSettlement}, you discover a forge hidden among the trees, its chimney puffing faint smoke. An elderly blacksmith emerges, calling out to you with a weary yet " +
                "determined expression. He shares the tale of his once-glorious career, now reduced to solitude after his apprentice’s unexpected departure. With an important commission to complete and no means to do so " +
                "alone, he implores your help. In return, he promises to reward you handsomely. Will you step in to aid this once-great craftsman?",

                //Event Description E
                "{=SecretsOfSteel_Event_Desc_E}While traversing the lands near {closestSettlement}, you come across an aging blacksmith working in a secluded forge. He calls out to you, his voice heavy with urgency. As " +
                "you approach, he recounts his story of being one of Calradia’s finest blacksmiths, now unable to continue his craft due to his failing strength. His apprentice has deserted him, and he is unable to complete " +
                "a critical order alone. The old man begs for your assistance, promising a generous reward for your efforts. How will you respond?"
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Choice 1A
                "{=SecretsOfSteel_Event_Choice_1A}You tell him that you are unable to help him. He looks back to you and smiles and says that he understands. He then gets up, enters the house, and closes the door. " +
                "You and your men proceed to leave.",

                //Event Choice 1B
                "{=SecretsOfSteel_Event_Choice_1B}You decline his request, explaining that you are pressed for time and cannot assist. The old blacksmith nods in quiet acceptance, rises from his seat, and retreats into " +
                "the forge. Without another word, you and your party continue on your journey.",

                //Event Choice 1C
                "{=SecretsOfSteel_Event_Choice_1C}You apologize and tell him you cannot offer any help. The blacksmith sighs but forces a weak smile, muttering that he understands. He walks back into his home, " +
                "closing the door behind him. You and your men leave, the sound of the forge fading in the distance.",

                //Event Choice 1D
                "{=SecretsOfSteel_Event_Choice_1D}You explain that your journey cannot be delayed and politely refuse his plea for help. The blacksmith gives you a resigned nod, quietly thanking you for at least " +
                "listening. He turns and disappears into the shadows of his forge. Your party resumes its travels without another glance back.",

                //Event Choice 1E
                "{=SecretsOfSteel_Event_Choice_1E}You shake your head, expressing regret that you cannot help him. The blacksmith smiles faintly, his eyes reflecting both disappointment and understanding. " +
                "Without a word, he steps inside and shuts the door, leaving you and your men to carry on your way."
            };
            
            private static readonly List<string> EventOutcome2A = new List<string>
            {
                //Event Choice 2 Outcome 1A
                "{=SecretsOfSteel_Event_Choice_2AA}You agree to help him. You send your men to {closestSettlement} and tell them to wait for you there. You and the blacksmith get to work. The hours pass, " +
                "but after some time, you are ready to start tempering your blades. You heat the first one and lower it into the oil. You hear the metal clinging and making a lot of noise under the oil. You pull it " +
                "up only to realize the entire blade is cracked and useless. You try again, but same result. And again, but same result again and before you know it, you've ruined all of your blades. You walk over " +
                "to the blacksmith and apologize for your bad work. To your surprise, he just smiles at you and says that he appreciated the help nonetheless. He informs you to take some charcoal with you as payment " +
                "as he does not have any gold. You pick up {randomCharcoal} pieces of charcoal and stuff them in your satchel. You and the blacksmith then part ways.",

                //Event Choice 2 Outcome 1B
                "{=SecretsOfSteel_Event_Choice_2AB}You decide to assist him, sending your men ahead to {closestSettlement}. Together, you and the blacksmith begin the meticulous process of forging the blades. " +
                "As the hours wear on, the forge grows hotter, and finally, it is time to temper the blades. The first blade cracks as it cools in the oil, and so does the second. Despite your best efforts, each attempt " +
                "results in failure. Frustrated, you apologize to the blacksmith for wasting his materials. He simply chuckles and assures you that your effort means more than the outcome. He hands you {randomCharcoal} " +
                "pieces of charcoal, saying it is all he can offer. You accept his gift and bid him farewell.",

                //Event Choice 2 Outcome 1C
                "{=SecretsOfSteel_Event_Choice_2AC}Agreeing to help, you send your men to wait for you at {closestSettlement} and join the blacksmith in his forge. The day stretches long as you hammer and " +
                "shape the metal, only for each blade to crack during tempering. Your inexperience shows, and you cannot produce a single usable piece. Ashamed, you apologize to the old man. Instead of scolding you, " +
                "he thanks you for the company and offers you {randomCharcoal} pieces of charcoal as a token of gratitude. Grateful for his understanding, you take the charcoal and leave, pondering the blacksmith's patience.",

                //Event Choice 2 Outcome 1D
                "{=SecretsOfSteel_Event_Choice_2AD}You agree to assist and dismiss your men to {closestSettlement}, then roll up your sleeves to join the blacksmith. Hours of intense labor lead to the moment of truth— " +
                "tempering the blades. Unfortunately, every attempt ends with the metal cracking, each blade ruined. Embarrassed, you admit your failure, but the blacksmith waves it off with a warm smile. He hands you " +
                "{randomCharcoal} pieces of charcoal as thanks for your effort. You accept the modest reward, parting ways with the old smith and leaving the forge behind.",

                //Event Choice 2 Outcome 1E
                "{=SecretsOfSteel_Event_Choice_2AE}You decide to help the blacksmith, sending your men ahead to {closestSettlement} while you stay behind. The day is spent at the forge, hammering and shaping metal. " +
                "When it comes time to temper the blades, each one cracks under the heat and oil. Frustrated and defeated, you apologize profusely, but the old blacksmith merely smiles and thanks you for trying. He offers " +
                "you {randomCharcoal} pieces of charcoal as a token of gratitude. You accept the humble gift, say your farewells, and rejoin your men, wiser from the experience."
            };
            
            private static readonly List<string> EventOutcome2B = new List<string>
            {
                //Event Choice 2 Outcome 2A
                "{=SecretsOfSteel_Event_Choice_2BA}You agree to help him. You send your men to {closestSettlement} and tell them to wait for you there. You and the blacksmith get to work. The hours pass, " +
                "and eventually, you manage to temper all the blades and begin sharpening them. The blacksmith picks up one of the blades and examines it closely. He then tells you to stop, pointing out flaws in " +
                "your sharpening technique. Grateful for your efforts, he assures you that he will finish the work himself. As a token of appreciation, he hands you {randomSteel} pieces of steel, apologizing for not " +
                "having gold to offer. You thank him, pack the steel into your satchel, and part ways.",

                //Event Choice 2 Outcome 2B
                "{=SecretsOfSteel_Event_Choice_2BB}Agreeing to help, you send your men ahead to {closestSettlement} while you assist the blacksmith. Together, you temper all the blades and begin the sharpening process. " +
                "After inspecting one of the blades, the blacksmith gently stops you, pointing out issues with the edge work. He praises your efforts on the hard labor and insists on taking over the finishing touches. " +
                "As a reward, he gives you {randomSteel} pieces of steel, explaining it is all he can spare. You accept the gift, bid him farewell, and rejoin your men.",

                //Event Choice 2 Outcome 2C
                "{=SecretsOfSteel_Event_Choice_2BC}You agree to stay behind and help while your men head to {closestSettlement}. After hours of intense work, the blades are tempered, and you begin sharpening them. " +
                "The blacksmith examines your work and politely points out errors in the sharpening process. He thanks you for handling the heavy lifting and assures you that he will complete the blades himself. " +
                "For your help, he gives you {randomSteel} pieces of steel as payment. You gratefully accept the steel and leave the forge with a sense of accomplishment.",

                //Event Choice 2 Outcome 2D
                "{=SecretsOfSteel_Event_Choice_2BD}You stay to help the blacksmith, sending your men ahead to {closestSettlement}. After tempering the blades, you begin the task of sharpening them. The blacksmith " +
                "inspects one of the blades and kindly corrects your technique. He decides to take over the final steps of the process and thanks you for your labor. He offers you {randomSteel} pieces of steel in lieu of " +
                "gold as a reward for your effort. You pack the steel and say your goodbyes, leaving him to finish the work on his own.",

                //Event Choice 2 Outcome 2E
                "{=SecretsOfSteel_Event_Choice_2BE}You decide to assist, asking your men to wait for you at {closestSettlement}. You spend hours tempering and sharpening blades with the blacksmith. When he inspects " +
                "your sharpening, he notices imperfections and decides to complete the final steps himself. He expresses gratitude for your hard work and rewards you with {randomSteel} pieces of steel, apologizing for not " +
                "having gold. You accept the gift, thank him for the opportunity, and set off to rejoin your party."
            };
            
            private static readonly List<string> EventOutcome2C = new List<string>
            {
                //Event Choice 2 Outcome 3A
                "{=SecretsOfSteel_Event_Choice_2CA}You agree to help him. You send your men to {closestSettlement} and tell them to wait for you there. You and the blacksmith get to work. The hours pass, " +
                "and eventually, all the blades are tempered, and you are midway through sharpening them. The blacksmith inspects one of the blades and compliments your exceptional craftsmanship. Together, you finish " +
                "the remaining blades. Grateful for your assistance, the blacksmith hands you {randomFineSteel} pieces of Fine Steel and {randomThamaskeneSteel} pieces of Thamaskene Steel as a reward, apologizing for " +
                "not having gold. You thank him for the opportunity to help and part ways, satisfied with your work.",

                //Event Choice 2 Outcome 3B
                "{=SecretsOfSteel_Event_Choice_2CB}Agreeing to help, you send your men ahead to {closestSettlement} and stay to assist the blacksmith. Hours of focused labor lead to perfectly tempered blades, " +
                "and you begin sharpening them. The blacksmith picks up one of the blades and praises your work, saying it is as fine as any he has ever seen. Together, you finish the task, and the old smith expresses " +
                "his deep gratitude. As payment, he gives you {randomFineSteel} pieces of Fine Steel and {randomThamaskeneSteel} pieces of Thamaskene Steel. You accept the reward and leave with a sense of accomplishment.",

                //Event Choice 2 Outcome 3C
                "{=SecretsOfSteel_Event_Choice_2CC}You decide to help and ask your men to wait for you at {closestSettlement}. After hours of hard work, the blades are tempered, and you are halfway through sharpening " +
                "them when the blacksmith inspects one of your finished pieces. He smiles warmly, complimenting your excellent craftsmanship. With his help, the task is completed, and the blacksmith thanks you profusely, " +
                "offering {randomFineSteel} pieces of Fine Steel and {randomThamaskeneSteel} pieces of Thamaskene Steel as a reward. You bid him farewell, pleased with the outcome.",

                //Event Choice 2 Outcome 3D
                "{=SecretsOfSteel_Event_Choice_2CD}You stay behind to assist the blacksmith, sending your men to wait at {closestSettlement}. Together, you spend hours working the forge, tempering and sharpening " +
                "the blades. As you near the end of your task, the blacksmith inspects one of the finished blades and declares your work exceptional. He helps you complete the final steps and rewards you with {randomFineSteel} " +
                "pieces of Fine Steel and {randomThamaskeneSteel} pieces of Thamaskene Steel. With heartfelt thanks exchanged, you part ways, leaving the forge behind.",

                //Event Choice 2 Outcome 3E
                "{=SecretsOfSteel_Event_Choice_2CE}You agree to assist, sending your men to {closestSettlement} and dedicating your time to the blacksmith. Together, you temper the blades and begin sharpening them. " +
                "The blacksmith carefully examines your work and praises the quality of your craftsmanship, saying it rivals his own. After a few more hours of work, the task is complete. Grateful beyond words, he offers you " +
                "{randomFineSteel} pieces of Fine Steel and {randomThamaskeneSteel} pieces of Thamaskene Steel as a token of his appreciation. You accept the reward and leave, knowing you have made a difference."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=SecretsOfSteel_Event_Msg_1A}{heroName} declined to help the old blacksmith.",
                "{=SecretsOfSteel_Event_Msg_1B}{heroName} refused the blacksmith’s request for assistance and left the forge.",
                "{=SecretsOfSteel_Event_Msg_1C}{heroName} chose not to assist the blacksmith and departed.",
                "{=SecretsOfSteel_Event_Msg_1D}{heroName} rejected the blacksmith's plea for help, leaving him to his work.",
                "{=SecretsOfSteel_Event_Msg_1E}{heroName} turned down the blacksmith’s request and moved on."
            };
            
            private static readonly List<string> eventMsg2A = new List<string>
            {
                "{=SecretsOfSteel_Event_Msg_2AA}{heroName} tried to help the blacksmith but destroyed all the blades during tempering.",
                "{=SecretsOfSteel_Event_Msg_2AB}{heroName} assisted the blacksmith, but all the blades cracked in the tempering process.",
                "{=SecretsOfSteel_Event_Msg_2AC}{heroName} failed to temper the blades, ruining all of them in the process.",
                "{=SecretsOfSteel_Event_Msg_2AD}{heroName} attempted to help but ended up ruining every blade during tempering.",
                "{=SecretsOfSteel_Event_Msg_2AE}{heroName} agreed to assist the blacksmith, but their efforts left all the blades destroyed."
            };
            
            private static readonly List<string> eventMsg2B = new List<string>
            {
                "{=SecretsOfSteel_Event_Msg_2BA}{heroName} helped the blacksmith, but mistakes during sharpening rendered the blades imperfect.",
                "{=SecretsOfSteel_Event_Msg_2BB}{heroName} assisted the blacksmith but made errors in sharpening the blades.",
                "{=SecretsOfSteel_Event_Msg_2BC}{heroName} worked with the blacksmith, but the sharpening process left the blades flawed.",
                "{=SecretsOfSteel_Event_Msg_2BD}{heroName} tried to sharpen the blades, but mistakes marred the final product.",
                "{=SecretsOfSteel_Event_Msg_2BE}{heroName} helped temper the blades, but their sharpening mistakes left them unfinished."
            };
            
            private static readonly List<string> eventMsg2C = new List<string>
            { 
                "{=SecretsOfSteel_Event_Msg_2CA}{heroName} successfully worked with the blacksmith to complete all the blades.",
                "{=SecretsOfSteel_Event_Msg_2CB}{heroName} collaborated with the blacksmith, and together they finished all the blades.",
                "{=SecretsOfSteel_Event_Msg_2CC}{heroName} and the blacksmith completed the blades without any issues.",
                "{=SecretsOfSteel_Event_Msg_2CD}{heroName} assisted the blacksmith, and their combined efforts produced perfect blades.",
                "{=SecretsOfSteel_Event_Msg_2CE}{heroName} worked alongside the blacksmith, resulting in a flawless set of blades."
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
            
            public static string GetRandomEventOutcome2A()
            {
                var index = random.Next(EventOutcome2A.Count);
                return EventOutcome2A[index];
            }
            
            public static string GetRandomEventOutcome2B()
            {
                var index = random.Next(EventOutcome2B.Count);
                return EventOutcome2B[index];
            }
            
            public static string GetRandomEventOutcome2C()
            {
                var index = random.Next(EventOutcome2C.Count);
                return EventOutcome2C[index];
            }
            
            public static string GetRandomEventMessage1()
            {
                var index = random.Next(eventMsg1.Count);
                return eventMsg1[index];
            }
            
            public static string GetRandomEventMessage2A()
            {
                var index = random.Next(eventMsg2A.Count);
                return eventMsg2A[index];
            }
            
            public static string GetRandomEventMessage2B()
            {
                var index = random.Next(eventMsg2B.Count);
                return eventMsg2B[index];
            }
            
            public static string GetRandomEventMessage2C()
            {
                var index = random.Next(eventMsg2C.Count);
                return eventMsg2C[index];
            }
        }
    }


    public class SecretsOfSteelData : RandomEventData
    {
        public SecretsOfSteelData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new SecretsOfSteel();
        }
    }
}