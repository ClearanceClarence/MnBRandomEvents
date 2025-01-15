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
	public sealed class RedMoon : BaseEvent
	{
		private readonly bool eventDisabled;
		private readonly int minGoldLost;
		private readonly int maxGoldLost;
		private readonly int minMenLost;
		private readonly int maxMenLost;

		public RedMoon() : base(ModSettings.RandomEvents.RedMoonData)
		{
			var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
			
			eventDisabled = ConfigFile.ReadBoolean("RedMoon", "EventDisabled");
			minGoldLost = ConfigFile.ReadInteger("RedMoon", "MinGoldLost");
			maxGoldLost = ConfigFile.ReadInteger("RedMoon", "MaxGoldLost");
			minMenLost = ConfigFile.ReadInteger("RedMoon", "MinMenLost");
			maxMenLost = ConfigFile.ReadInteger("RedMoon", "MaxMenLost");
		}

		public override void CancelEvent()
		{
		}
		
		private bool HasValidEventData()
		{
			if (eventDisabled == false)
			{
				if (minGoldLost != 0 || maxGoldLost != 0 || minMenLost != 0 || maxMenLost != 0)
				{
					return true;
				}
			}
            
			return false;
		}

		public override bool CanExecuteEvent()
		{
			return HasValidEventData() && MobileParty.MainParty.MemberRoster.TotalRegulars >= maxMenLost && MobileParty.MainParty.CurrentSettlement == null && CurrentTimeOfDay.IsNight;
		}

		public override void StartEvent()
		{
			
			var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

			var heroName = Hero.MainHero.FirstName;

			var goldLostToReligion = MBRandom.RandomInt(minGoldLost, maxGoldLost);

			var menLostToReligion = MBRandom.RandomInt(minMenLost, maxMenLost);
			
			var closestSettlement = ClosestSettlements.GetClosestTownOrVillage(MobileParty.MainParty).ToString();
			
			
			var eventDescription =new TextObject(EventTextHandler.GetRandomEventDescription()) 
				.ToString();
			
			var eventOption1 = new TextObject("{=RedMoon_Event_Option_1}Panic with your men").ToString();
			var eventOption1Hover = new TextObject("{=RedMoon_Event_Option_1_Hover}This really **is** the end!").ToString();
            
			var eventOption2 = new TextObject("{=RedMoon_Event_Option_2}Call your men to you").ToString();
			var eventOption2Hover = new TextObject("{=RedMoon_Event_Option_2_Hover}Let's talk instead of acting like idiots").ToString();
            
			var eventOption3 = new TextObject("{=RedMoon_Event_Option_3}Order your men to stop").ToString();
			var eventOption3Hover = new TextObject("{=RedMoon_Event_Option_3_Hover}This is embarrassing!").ToString();
            
			var eventOption4 = new TextObject("{=RedMoon_Event_Option_4}Ignore everything").ToString();
			var eventOption4Hover = new TextObject("{=RedMoon_Event_Option_4_Hover}Just head back to your tent").ToString();
            
			var eventButtonText1 = new TextObject("{=RedMoon_Event_Button_Text_1}Choose").ToString();
			var eventButtonText2 = new TextObject("{=RedMoon_Event_Button_Text_2}Done").ToString();
			
			var inquiryElements = new List<InquiryElement>
			{
				new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
				new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
				new InquiryElement("c", eventOption3, null, true, eventOption3Hover),
				new InquiryElement("d", eventOption4, null, true, eventOption4Hover)
			};
			
			var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1())
				.SetTextVariable("goldLostToReligion", goldLostToReligion)
				.SetTextVariable("closestSettlement", closestSettlement)
				.ToString();
            
            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventOutcome2())
	            .SetTextVariable("menLostToReligion", menLostToReligion)
	            .SetTextVariable("closestSettlement", closestSettlement)
	            .SetTextVariable("goldLostToReligion", goldLostToReligion)
	            .ToString();
            
            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .ToString();
            
            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventOutcome4())
	            .SetTextVariable("closestSettlement", closestSettlement)
	            .SetTextVariable("goldLostToReligion", goldLostToReligion)
                .ToString();
            
            var eventMsg1 = new TextObject(EventTextHandler.GetRandomEventMessage1())
	            .SetTextVariable("goldLostToReligion", goldLostToReligion)
	            .SetTextVariable("heroName", heroName)
	            .ToString();
            
            var eventMsg2 = new TextObject(EventTextHandler.GetRandomEventMessage2())
	            .SetTextVariable("goldLostToReligion", goldLostToReligion)
	            .SetTextVariable("menLostToReligion", menLostToReligion)
	            .SetTextVariable("heroName", heroName)
	            .ToString();
            
            var eventMsg3 = new TextObject(EventTextHandler.GetRandomEventMessage3())
	            .SetTextVariable("menLostToReligion", menLostToReligion)
	            .SetTextVariable("heroName", heroName)
	            .ToString();
            
            var eventMsg4 = new TextObject(EventTextHandler.GetRandomEventMessage4())
	            .SetTextVariable("goldLostToReligion", goldLostToReligion)
	            .SetTextVariable("menLostToReligion", menLostToReligion)
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
                            
                            Hero.MainHero.ChangeHeroGold(-goldLostToReligion);
                            
                            break;
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            
                            Hero.MainHero.ChangeHeroGold(-goldLostToReligion);
                            MobileParty.MainParty.MemberRoster.KillNumberOfNonHeroTroopsRandomly(menLostToReligion);
                            
                            break;
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                            
                            MobileParty.MainParty.MemberRoster.KillNumberOfNonHeroTroopsRandomly(menLostToReligion);
                            
                            break;
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                            
                            Hero.MainHero.ChangeHeroGold(-goldLostToReligion);
                            MobileParty.MainParty.MemberRoster.KillNumberOfNonHeroTroopsRandomly(menLostToReligion);
                            
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
				MessageBox.Show($"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n { ex.StackTrace}");
			}
		}
		
		private static class EventTextHandler
        {
            private static readonly Random random = new Random();
            
            private static readonly List<string> eventTitles = new List<string>
            {
	            "{=RedMoon_Title_A}The Red Moon",
	            "{=RedMoon_Title_B}The Crimson Sky",
	            "{=RedMoon_Title_C}Blood on the Moon",
	            "{=RedMoon_Title_D}The Scarlet Eclipse",
	            "{=RedMoon_Title_E}The Lunar Omen",
	            "{=RedMoon_Title_F}The Blood Moon",
	            "{=RedMoon_Title_G}The Ruby Night",
	            "{=RedMoon_Title_H}The Sky’s Warning",
	            "{=RedMoon_Title_I}The Night of Red Light",
	            "{=RedMoon_Title_J}The Fiery Orb"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=RedMoon_Event_Desc_A}You are in your tent late one night when you are awoken by your men starting a commotion. Annoyed, you step outside to quiet them, " +
                "but as soon as you leave the tent, you understand the chaos. The moon hangs in the sky, blood red and ominous. Your men are panicking, running in all directions, " +
                "crying out that this is the end of days. The only question that lingers in your mind is how you will handle this.",
                
                //Event Description B
                "{=RedMoon_Event_Desc_B}Late at night, your slumber is interrupted by the sounds of shouting and confusion among your men. Frustrated, you step outside to put an end " +
                "to the noise, only to freeze in shock as you see the source of their panic. The moon glows blood red, casting an eerie light over the camp. Your men are in disarray, " +
                "many proclaiming this to be an apocalyptic sign. What will you do to restore order?",
                
                //Event Description C
                "{=RedMoon_Event_Desc_C}You wake abruptly to the sound of chaos outside your tent. Frustrated, you go out to confront your men, but the sight above stops you in your tracks. " +
                "The moon is crimson red, bathing the camp in an otherworldly light. Your men are terrified, shouting that the end of days has arrived. It’s up to you to decide how to calm them.",
                
                //Event Description D
                "{=RedMoon_Event_Desc_D}The commotion outside your tent wakes you late one night. Irritated, you step outside to confront your men, only to be met with the haunting sight of " +
                "a blood-red moon looming above. Fear grips the camp as your men scatter, proclaiming doom is upon them. In this moment of panic, you must choose how to lead.",
                
                //Event Description E
                "{=RedMoon_Event_Desc_E}Awakened by frantic shouting, you emerge from your tent to a scene of chaos. The moon glows red as blood, casting an eerie light over your camp. " +
                "Your men are in a frenzy, convinced this marks the end of days. Faced with their fear, you must decide how to respond to this unsettling omen."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Outcome 1A
                "{=RedMoon_Event_Choice_1A}You fall to your knees, overwhelmed by the sight, and begin praying fervently to the gods. Some of your men, inspired or desperate, join you in prayer.\n\n" +
                "After nearly 10 minutes, it dawns on you that prayer alone might not suffice. You order your men to hand over {goldLostToReligion} gold to take to the nearest chapel. " +
                "Mounting your steed, you set off at a frantic pace toward {closestSettlement}, where you know there is a place of worship. In your haste, as your horse clears a fence, " +
                "you are thrown off and lose consciousness.\n" +
                "You awaken to find the sun rising. Weary and bruised, you return to camp, only to realize the chest of gold is gone.",
                
                //Event Outcome 1B
                "{=RedMoon_Event_Choice_1B}Stricken by fear, you drop to your knees and begin to pray, urging your men to join you. Many follow your lead, hoping to appease the gods.\n\n" +
                "After some time, you realize prayers won’t solve the problem. Gathering {goldLostToReligion} gold, you mount your horse and ride with urgency toward {closestSettlement}, " +
                "knowing the chapel there might offer aid. As you leap over a fence, your horse stumbles, throwing you to the ground. The impact leaves you unconscious.\n" +
                "When you regain consciousness, it is already morning. Returning to camp, you recall the chest of gold you lost in your desperate ride.",
                
                //Event Outcome 1C
                "{=RedMoon_Event_Choice_1C}Unable to contain your fear, you kneel and begin praying, calling on the gods for guidance. Several of your men follow suit, their voices joining yours in fervent prayer.\n\n" +
                "Realizing that prayer alone won’t bring a solution, you hastily collect {goldLostToReligion} gold and ride toward {closestSettlement}, hoping the priests can intervene. " +
                "Riding recklessly, your horse jumps a fence, causing you to fall and black out.\n" +
                "You wake to the dawn’s light and return to camp. Only then does the memory of the lost gold return to you, deepening your regret.",
                
                //Event Outcome 1D
                "{=RedMoon_Event_Choice_1D}You drop to your knees, overwhelmed, and begin praying. Some of your men join you, their voices trembling with fear.\n\n" +
                "After nearly 10 minutes, you realize more action is needed. Collecting {goldLostToReligion} gold, you ride at breakneck speed toward {closestSettlement}, where the local chapel might help. " +
                "In your haste, your horse leaps a fence, throwing you off and rendering you unconscious.\n" +
                "When you awaken, the sun is already high in the sky. Returning to camp, you recall the gold you carried—and lost—on your desperate journey.",
                
                //Event Outcome 1E
                "{=RedMoon_Event_Choice_1E}You collapse to your knees in desperation, praying loudly to the gods for mercy. Several of your men join you, seeking solace in prayer.\n\n" +
                "When it becomes clear this won’t resolve anything, you collect {goldLostToReligion} gold and set off for {closestSettlement}, hoping the priests can help. " +
                "Riding recklessly, your horse stumbles while jumping a fence, throwing you violently to the ground. You lose consciousness.\n" +
                "Awakening at dawn, you slowly make your way back to camp. Only then do you remember the chest of gold you carried—and lost along the way."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Event Outcome 2A
                "{=RedMoon_Event_Choice_2A}You call your men to gather, trying to quell the panic and fear you see in their faces. " +
                "Despite your best efforts to calm them, they scatter like headless chickens, ignoring your commands. Frustrated, you decide " +
                "to let them be and return to your tent.\n\nThe next morning, you learn that {menLostToReligion} of your men left the party, " +
                "heading toward {closestSettlement}. They also took {goldLostToReligion} gold from the treasury as an offering to the church. " +
                "At least the fanatics are gone!",
                
                //Event Outcome 2B
                "{=RedMoon_Event_Choice_2B}You try to address your men, who are consumed by panic and fear. Despite your attempts to reason with them, " +
                "chaos ensues as they run around in a frenzy. Realizing there’s no point in trying further, you retreat to your tent.\n\n" +
                "By morning, you discover that {menLostToReligion} of your men have left, taking {goldLostToReligion} gold with them as an offering " +
                "to the church in {closestSettlement}. Though frustrating, you can’t help but feel relieved that the fanatics are no longer your problem.",
                
                //Event Outcome 2C
                "{=RedMoon_Event_Choice_2C}Gathering your men, you attempt to calm their frayed nerves, but your words fall on deaf ears. The panic spreads, " +
                "and they ignore your orders, running around aimlessly. Frustrated, you decide to let them act as they wish and retreat to your tent for the night.\n\n" +
                "The next day, you learn that {menLostToReligion} men have abandoned the party, taking {goldLostToReligion} gold from the treasury. They’ve " +
                "gone toward {closestSettlement}, likely seeking solace in the church. At least the commotion has died down.",
                
                //Event Outcome 2D
                "{=RedMoon_Event_Choice_2D}Calling your men to you, you attempt to restore order, but the panic is too great. " +
                "They scatter in fear, ignoring your pleas for calm. Accepting that nothing can be done, you retire to your tent.\n\n" +
                "By morning, you find out that {menLostToReligion} of your men have left, taking {goldLostToReligion} gold from the treasury " +
                "with them. They’ve gone toward {closestSettlement}, seeking refuge at the church. At least the situation is quieter now.",
                
                //Event Outcome 2E
                "{=RedMoon_Event_Choice_2E}You gather your men and try to reassure them, but their terror is overwhelming. " +
                "Your words are drowned out by the chaos as they run around in a state of panic. Recognizing the futility, " +
                "you decide to retire to your tent and let them sort themselves out.\n\nThe following morning, you learn that " +
                "{menLostToReligion} of your men have deserted, taking {goldLostToReligion} gold from the treasury as an offering " +
                "to the church in {closestSettlement}. At least their absence brings some peace to the camp."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Event Outcome 3A
                "{=RedMoon_Event_Choice_3A}You call your men to attention, most of whom fall in line quickly, though a few continue to panic. " +
                "You address them firmly, explaining that the red moon is a naturally occurring phenomenon. You reassure them by recounting " +
                "how you witnessed the same event years ago, and nothing ill came of it. Your men seem to calm down, though some fools in the " +
                "back persist in their prayers. Deciding you don’t need unstable warriors in your ranks, you order your guards to kick them out of camp.",
                
                //Event Outcome 3B
                "{=RedMoon_Event_Choice_3B}With authority, you command your men to gather. Most obey at once, though a few remain caught up in their hysteria. " +
                "You explain to them that the red moon is a natural event, having seen it yourself years ago with no ill consequences. Your calm demeanor reassures most of your men, " +
                "but a handful continue praying fervently. Frustrated, you decide such unstable individuals have no place in your party and have your guards escort them out of camp.",
                
                //Event Outcome 3C
                "{=RedMoon_Event_Choice_3C}Raising your voice, you order your men to stand down and listen. While most comply, a few still act irrationally. " +
                "You calmly explain that the red moon is a natural event, one you personally witnessed years ago with no harm resulting from it. " +
                "This reassures the majority of your men, but a small group of fanatics continues their prayers. You decide to expel them from the party, " +
                "instructing your guards to remove them from the camp.",
                
                //Event Outcome 3D
                "{=RedMoon_Event_Choice_3D}You assert your authority, commanding your men to assemble. While most obey, a few remain lost in their panic. " +
                "You explain to them that this red moon is nothing more than a natural occurrence, something you witnessed years ago without any dire consequences. " +
                "Your words soothe the majority of your men, but those still praying are deemed unfit for your party. You have your guards remove them from camp to maintain order.",
                
                //Event Outcome 3E
                "{=RedMoon_Event_Choice_3E}You order your men to gather and listen. Most quickly fall in line, but a handful remain frantic. " +
                "Speaking with confidence, you explain that the red moon is a natural phenomenon, recounting how you witnessed the same event years ago without any trouble. " +
                "Your men appear reassured, though some fanatics persist in their prayers. Deciding they are a liability, you have your guards escort them out of camp immediately."

            };
            
	        private static readonly List<string> EventOutcome4 = new List<string>
            {
                //Event Outcome 4A
                "{=RedMoon_Event_Choice_4A}Nope...\n\nYou decide not to deal with the chaos and return to your tent to sleep.\n\n" +
                "The next morning, you’re informed that {menLostToReligion} of your men left the party during the night, heading toward {closestSettlement}. " +
                "They also took {goldLostToReligion} gold from the treasury as an offering to the church. At least the fanatics are gone!",
                
                //Event Outcome 4B
                "{=RedMoon_Event_Choice_4B}You shake your head and mutter, 'Nope...' before retreating into your tent and going straight to bed.\n\n" +
                "When morning comes, you learn that {menLostToReligion} men deserted the camp, heading toward {closestSettlement}. " +
                "They also took {goldLostToReligion} gold from the treasury for their religious offering. At least you won’t have to deal with them anymore.",
                
                //Event Outcome 4C
                "{=RedMoon_Event_Choice_4C}With a simple 'Nope,' you turn back to your tent, ignoring the chaos outside, and settle in for the night.\n\n" +
                "Upon waking, you’re informed that {menLostToReligion} men left the party, making their way toward {closestSettlement}. " +
                "They also helped themselves to {goldLostToReligion} gold from the treasury. At least the fanatics are no longer your problem.",
                
                //Event Outcome 4D
                "{=RedMoon_Event_Choice_4D}Deciding you want no part in the commotion, you say, 'Nope,' and head back to your tent for some sleep.\n\n" +
                "The following morning, you’re told that {menLostToReligion} of your men abandoned the party, heading toward {closestSettlement}. " +
                "They also took {goldLostToReligion} gold as an offering to the church. You sigh, relieved to see them gone.",
                
                //Event Outcome 4E
                "{=RedMoon_Event_Choice_4E}You glance at the chaos, shrug, and simply say, 'Nope.' Returning to your tent, you go straight to sleep.\n\n" +
                "When you wake, you’re informed that {menLostToReligion} men left during the night, taking {goldLostToReligion} gold from the treasury " +
                "as an offering to the church. At least the troublemakers are gone!"
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
	            "{=RedMoon_Event_Msg_1A}{heroName} lost {goldLostToReligion} gold due to an... unfortunate accident.",
	            "{=RedMoon_Event_Msg_1B}An unfortunate series of events caused {heroName} to lose {goldLostToReligion} gold.",
	            "{=RedMoon_Event_Msg_1C}{heroName} lost {goldLostToReligion} gold under less-than-ideal circumstances.",
	            "{=RedMoon_Event_Msg_1D}{goldLostToReligion} gold was lost by {heroName} in an unfortunate turn of events.",
	            "{=RedMoon_Event_Msg_1E}{heroName} managed to misplace {goldLostToReligion} gold due to unforeseen complications."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
	            "{=RedMoon_Event_Msg_2A}{heroName} lost {menLostToReligion} men and {goldLostToReligion} gold after the red moon.",
	            "{=RedMoon_Event_Msg_2B}Following the red moon, {heroName} lost {menLostToReligion} men and {goldLostToReligion} gold.",
	            "{=RedMoon_Event_Msg_2C}The red moon caused {heroName} to lose {menLostToReligion} men and {goldLostToReligion} gold.",
	            "{=RedMoon_Event_Msg_2D}{heroName} suffered the loss of {menLostToReligion} men and {goldLostToReligion} gold due to the red moon.",
	            "{=RedMoon_Event_Msg_2E}After the red moon, {heroName} found {menLostToReligion} men and {goldLostToReligion} gold missing."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
	            "{=RedMoon_Event_Msg_3A}{heroName} calmed the party but expelled several fanatics from the group.",
	            "{=RedMoon_Event_Msg_3B}Order was restored by {heroName}, though a few fanatics were removed from the party.",
	            "{=RedMoon_Event_Msg_3C}{heroName} reassured the group but decided to expel unstable members.",
	            "{=RedMoon_Event_Msg_3D}The red moon caused panic, but {heroName} calmed the group and removed the fanatics.",
	            "{=RedMoon_Event_Msg_3E}{heroName} maintained order by calming the group and expelling troublemakers."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            {
	            "{=RedMoon_Event_Msg_4A}{heroName} lost {menLostToReligion} men and {goldLostToReligion} gold after ignoring the men last night.",
	            "{=RedMoon_Event_Msg_4B}By choosing to ignore the chaos, {heroName} lost {menLostToReligion} men and {goldLostToReligion} gold.",
	            "{=RedMoon_Event_Msg_4C}Ignoring the men’s fears cost {heroName} {menLostToReligion} men and {goldLostToReligion} gold.",
	            "{=RedMoon_Event_Msg_4D}{heroName} suffered the loss of {menLostToReligion} men and {goldLostToReligion} gold due to inaction during the red moon.",
	            "{=RedMoon_Event_Msg_4E}After choosing not to intervene, {heroName} lost {menLostToReligion} men and {goldLostToReligion} gold."
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
	

	public class RedMoonData : RandomEventData
	{
		public RedMoonData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
		{
		}

		public override BaseEvent GetBaseEvent()
		{
			return new RedMoon();
		}
	}
}
