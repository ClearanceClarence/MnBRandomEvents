using System;
using System.Collections.Generic;
using System.Windows;
using CryingBuffalo.RandomEvents.Helpers;
using CryingBuffalo.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CryingBuffalo.RandomEvents.Events.CCEvents
{
    public sealed class RunawaySon : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minGold;
        private readonly int maxGold;
        private readonly int minRogueryLevel;

        public RunawaySon() : base(ModSettings.RandomEvents.RunawaySonData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("RunawaySon", "EventDisabled");
            minGold = ConfigFile.ReadInteger("RunawaySon", "MinGold");
            maxGold = ConfigFile.ReadInteger("RunawaySon", "MaxGold");
            minRogueryLevel = ConfigFile.ReadInteger("RunawaySon", "MinRogueryLevel");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minGold != 0 || maxGold != 0 || minRogueryLevel != 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement == null;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
            
            var heroName = Hero.MainHero.FirstName;
            
            var goldLooted = MBRandom.RandomInt(minGold, maxGold);
            
            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();
            
            var heroRogueryLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Roguery);

            var canKill = false;
            var rogueryAppendedText = "";
            
            if (GeneralSettings.SkillChecks.IsDisabled())
            {
                
                canKill = true;
                rogueryAppendedText = new TextObject("{=RunawaySon_Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();

            }
            else
            {
                if (heroRogueryLevel >= minRogueryLevel)
                {
                    canKill = true;
                    
                    rogueryAppendedText = new TextObject("{=RunawaySon_Roguery_Appended_Text}[Roguery - lvl {minRogueryLevel}]")
                        .SetTextVariable("minRogueryLevel", minRogueryLevel)
                        .ToString();
                }
            }
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("closestSettlement", closestSettlement)
                .ToString();
            
            var eventOption1 = new TextObject("{=RunawaySon_Event_Option_1}Take him in and train him").ToString();
            var eventOption1Hover = new TextObject("{=RunawaySon_Event_Option_1_Hover}You could use the distraction of having someone to train").ToString();
            
            var eventOption2 = new TextObject("{=RunawaySon_Event_Option_2}Tell him he can tag along").ToString();
            var eventOption2Hover = new TextObject("{=RunawaySon_Event_Option_2_Hover}You really don't have time to babysit him").ToString();
            
            var eventOption3 = new TextObject("{=RunawaySon_Event_Option_3}Go away").ToString();
            var eventOption3Hover = new TextObject("{=RunawaySon_Event_Option_3_Hover}He needs to leave").ToString();
            
            var eventOption4 = new TextObject("{=RunawaySon_Event_Option_4}[Roguery] Kill him").ToString();
            var eventOption4Hover = new TextObject("{=RunawaySon_Event_Option_4_Hover}It's a cruel world.\n{rogueryAppendedText}").SetTextVariable("rogueryAppendedText",rogueryAppendedText).ToString();
            
            var eventButtonText1 = new TextObject("{=RunawaySon_Event_Button_Text_1}Okay").ToString();
            var eventButtonText2 = new TextObject("{=RunawaySon_Event_Button_Text_2}Done").ToString();
            
            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
                new InquiryElement("c", eventOption3, null, true, eventOption3Hover)
            };

            if (canKill)
            {
                inquiryElements.Add(new InquiryElement("d", eventOption4, null, true, eventOption4Hover));
            }
            
            
            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1()).ToString();
            
            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventOutcome2()).ToString();
            
            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventOutcome3()).ToString();
            
            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .SetTextVariable("goldLooted",goldLooted)
                .ToString();
            
            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessages())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("goldLooted", goldLooted)
                .ToString();

            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1, eventButtonText1, null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                            
                            Hero.MainHero.AddSkillXp(DefaultSkills.Leadership, 30);
                            Hero.MainHero.AddSkillXp(DefaultSkills.Steward, 20);
                            
                            GiveOneRandomRecruitFromClosestCulture();

                            break;
                        
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            
                            Hero.MainHero.AddSkillXp(DefaultSkills.Leadership, 20);
                            Hero.MainHero.AddSkillXp(DefaultSkills.Steward, 30);
                            
                            GiveOneRandomRecruitFromClosestCulture();

                            break;
                        
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            break;
                        
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                            
                            Hero.MainHero.ChangeHeroGold(goldLooted);
                            Hero.MainHero.AddSkillXp(DefaultSkills.Roguery, 150);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_EVIL_Outcome));
                            
                            
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

        private static void GiveOneRandomRecruitFromClosestCulture()
        {
            var closestSettlementCulture = ClosestSettlements.GetClosestAny(MobileParty.MainParty).Culture.ToString();

            var CultureDemonym = Demonym.GetTheDemonym(closestSettlementCulture, false);
            
            var troopRoster = TroopRoster.CreateDummyTroopRoster();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var characterObject in CharacterObject.All)
            {
                if (characterObject.StringId.Contains("recruit") &&
                    !characterObject.StringId.Contains("vigla")
                    && characterObject.Culture.ToString() == closestSettlementCulture ||
                    (characterObject.StringId.Contains("footman") &&
                     !characterObject.StringId.Contains("vlandia")
                     && !characterObject.StringId.Contains("aserai") &&
                     characterObject.Culture.ToString() == closestSettlementCulture) ||
                    (characterObject.StringId.Contains("volunteer")
                     && characterObject.StringId.Contains("battanian") &&
                     characterObject.Culture.ToString() == closestSettlementCulture))
                {
                    troopRoster.AddToCounts(characterObject, 1);
                }
            }
            PartyScreenManager.OpenScreenAsReceiveTroops(troopRoster, leftPartyName: new TextObject("{CultureDemonym} Volunteer").SetTextVariable("CultureDemonym", CultureDemonym));
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
                "{=RunawaySon_Title_A}Runaway Son",
                "{=RunawaySon_Title_B}The Fugitive Heir",
                "{=RunawaySon_Title_C}The Wayward Man",
                "{=RunawaySon_Title_D}The Wanderer’s Tale",
                "{=RunawaySon_Title_E}The Lost Scion",
                "{=RunawaySon_Title_F}The Escaped Son",
                "{=RunawaySon_Title_G}The Rebel Son",
                "{=RunawaySon_Title_H}The Missing Man",
                "{=RunawaySon_Title_I}The Exiled Son",
                "{=RunawaySon_Title_J}The Runaway Stranger"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=RunawaySon_Event_Desc_A}As your party travels near {closestSettlement}, a young man approaches you. " +
                "He explains that he fled his family farm after enduring years of abuse from his parents. " +
                "Desperate for a new start, he asks to join your party, claiming to have some skill with weapons.",
                
                //Event Description B
                "{=RunawaySon_Event_Desc_B}While journeying near {closestSettlement}, your party is approached by a young man. " +
                "He shares a grim story of running away from his family farm to escape years of mistreatment by his parents. " +
                "Seeking refuge, he offers his skills with weapons and requests to join your group.",
                
                //Event Description C
                "{=RunawaySon_Event_Desc_C}As you pass through the lands near {closestSettlement}, a young man intercepts your party. " +
                "He recounts a harrowing tale of fleeing his abusive parents and abandoning the family farm. " +
                "Eager to prove himself, he asks to join your ranks, boasting some proficiency with weapons.",
                
                //Event Description D
                "{=RunawaySon_Event_Desc_D}Near {closestSettlement}, a young man steps out of the shadows and approaches your party. " +
                "He reveals that he has run away from an abusive home, leaving his family farm behind. " +
                "Hoping for a new life, he offers his skills with weapons in exchange for a place in your party.",
                
                //Event Description E
                "{=RunawaySon_Event_Desc_E}While traveling near {closestSettlement}, your group encounters a young man seeking sanctuary. " +
                "He tells you he has fled his family farm, unable to endure the abuse of his parents any longer. " +
                "In need of safety and purpose, he requests to join your party, claiming some experience with weapons."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Outcome 1A
                "{=RunawaySon_Event_Choice_1A}You tell him he is welcome in your ranks and that you will personally train him to become a fine soldier.",
                
                //Event Outcome 1B
                "{=RunawaySon_Event_Choice_1B}You accept him into your ranks, promising to mentor him and shape him into a capable warrior.",
                
                //Event Outcome 1C
                "{=RunawaySon_Event_Choice_1C}You welcome him to your party and assure him that, with your guidance, he will become a skilled soldier.",
                
                //Event Outcome 1D
                "{=RunawaySon_Event_Choice_1D}You nod and invite him to join, vowing to train him yourself and turn him into a reliable fighter.",
                
                //Event Outcome 1E
                "{=RunawaySon_Event_Choice_1E}You agree to take him in, promising to teach him the skills he’ll need to thrive as a soldier in your ranks."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Event Choice 2A
                "{=RunawaySon_Event_Choice_2A}You tell him he can tag along but make it clear that he is not to interfere in your affairs.",
                
                //Event Choice 2B
                "{=RunawaySon_Event_Choice_2B}You agree to let him travel with you, warning him to stay out of your business.",
                
                //Event Choice 2C
                "{=RunawaySon_Event_Choice_2C}You permit him to accompany your group but firmly instruct him not to meddle in your matters.",
                
                //Event Choice 2D
                "{=RunawaySon_Event_Choice_2D}You allow him to join, but with a stern warning that he is not to involve himself in your affairs.",
                
                //Event Choice 2E
                "{=RunawaySon_Event_Choice_2E}You let him follow your party but make it clear that he must keep his distance from your dealings."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Event Choice 3A
                "{=RunawaySon_Event_Choice_3A}You tell him to get lost. The man says nothing and promptly turns to leave.",
                
                //Event Choice 3B
                "{=RunawaySon_Event_Choice_3B}You dismiss him, telling him to leave immediately. Without a word, he turns and walks away.",
                
                //Event Choice 3C
                "{=RunawaySon_Event_Choice_3C}You firmly tell him to leave. The man nods silently before turning and walking off.",
                
                //Event Choice 3D
                "{=RunawaySon_Event_Choice_3D}You instruct him to leave at once. Without protest, he turns around and departs.",
                
                //Event Choice 3E
                "{=RunawaySon_Event_Choice_3E}You harshly tell him to get lost. He doesn’t argue, turning on his heel and leaving immediately."
            };
            
	        private static readonly List<string> EventOutcome4 = new List<string>
            {
                //Event Choice 4A
                "{=RunawaySon_Event_Choice_4A}You laugh mockingly at his plea, and soon your men join in the laughter. Approaching the man, you drive a dagger into his stomach, " +
                "watching as he collapses to the ground, screaming in agony.\n" +
                "You kneel beside him, observing as the light fades from his eyes and he succumbs to his wounds. Afterward, you and some of your men cut him open and hang his body " +
                "from a tree as a grim warning to others, but not before looting {goldLooted} gold from his corpse.",
                
                //Event Choice 4B
                "{=RunawaySon_Event_Choice_4B}Hearing his plea, you burst into cruel laughter, and your men follow suit. Without hesitation, you step forward and plunge a dagger into his stomach. " +
                "He falls to the ground, writhing and screaming in pain.\n" +
                "You crouch beside him, watching as his life fades away. Once he’s dead, you and your men carve his body open and string it up in a tree as a warning to others, looting {goldLooted} gold from him first.",
                
                //Event Choice 4C
                "{=RunawaySon_Event_Choice_4C}You laugh cruelly at his desperate plea, and your men join in the jeers. Moving forward, you thrust your dagger into his stomach, " +
                "watching as he collapses in pain.\n" +
                "Kneeling beside his lifeless body, you and your men decide to cut him open and hang him from a tree as a warning. Before leaving, you loot {goldLooted} gold from his belongings.",
                
                //Event Choice 4D
                "{=RunawaySon_Event_Choice_4D}His plea only amuses you, and you laugh coldly, prompting your men to join in. Without a word, you approach and stab him in the stomach, " +
                "listening to his screams as he falls to the ground.\n" +
                "You crouch down, watching as he dies, then order your men to disembowel him and hang his body from a tree as a warning. Before departing, you take {goldLooted} gold from his body.",
                
                //Event Choice 4E
                "{=RunawaySon_Event_Choice_4E}You laugh derisively at his plea, and your men echo your cruel amusement. Stepping forward, you drive a dagger into his stomach, " +
                "smiling as he collapses in agony.\n" +
                "You kneel by his side and wait until he breathes his last. Then, with the help of your men, you cut open his body and hang it from a tree as a warning. " +
                "Before leaving, you search him and recover {goldLooted} gold."
            };
            
            private static readonly List<string> eventMsgs = new List<string>
            {
                "{=RunawaySon_Event_Msgs_A}{heroName} killed a young man and looted {goldLooted} from his corpse.",
                "{=RunawaySon_Event_Msgs_B}{heroName} ended a young man’s life and took {goldLooted} from his belongings.",
                "{=RunawaySon_Event_Msgs_C}After killing a young man, {heroName} looted {goldLooted} gold from his body.",
                "{=RunawaySon_Event_Msgs_D}{heroName} claimed {goldLooted} gold after taking the life of a young man.",
                "{=RunawaySon_Event_Msgs_E}{heroName} looted {goldLooted} gold from the corpse of a young man they killed."
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


    public class RunawaySonData : RandomEventData
    {

        public RunawaySonData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new RunawaySon();
        }
    }
}