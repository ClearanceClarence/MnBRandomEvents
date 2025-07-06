using System;
using System.Collections.Generic;
using System.Windows;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
	public sealed class PassingComet : BaseEvent
	{
		private readonly bool eventDisabled;
		
		public PassingComet() : base(ModSettings.RandomEvents.PassingCometData)
		{
			var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
			eventDisabled = ConfigFile.ReadBoolean("PassingComet", "EventDisabled");
		}

		public override void CancelEvent()
		{
		}
		
		private bool HasValidEventData()
		{
			return eventDisabled == false;
		}

		public override bool CanExecuteEvent()
		{
			return HasValidEventData() && MobileParty.MainParty.CurrentSettlement == null && CurrentTimeOfDay.IsNight && MobileParty.MainParty.MemberRoster.TotalRegulars >= 25;
		}

		public override void StartEvent()
		{
			var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
			
			var eventText =new TextObject(EventTextHandler.GetRandomEventText()).ToString();
			
			var eventButtonText = new TextObject("{=PassingComet_Event_Button_Text}Done")
				.ToString();
			
			InformationManager.ShowInquiry(new InquiryData(eventTitle, eventText, true, false, eventButtonText, null, null, null), true);

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
					MessageBox.Show($"onEventCompleted was null while stopping \"{randomEventData.eventType}\" event.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
			}
		}
		
		private static class EventTextHandler
        {
            private static readonly Random random = new Random();
            
            private static readonly List<string> eventTitles = new List<string>
            {
	            "{=PassingComet_Title_A}The Celestial Visitor",
	            "{=PassingComet_Title_B}The Wandering Star",
	            "{=PassingComet_Title_C}The Cosmic Traveler",
	            "{=PassingComet_Title_D}The Sky's Messenger",
	            "{=PassingComet_Title_E}The Passing Light",
	            "{=PassingComet_Title_F}The Stellar Nomad",
	            "{=PassingComet_Title_G}The Streak of the Heavens",
	            "{=PassingComet_Title_H}The Astral Wanderer",
	            "{=PassingComet_Title_I}The Distant Flame",
	            "{=PassingComet_Title_J}The Comet's Trail"
            };
            
            private static readonly List<string> eventText = new List<string>
            {
                //Event Text A
                "{=PassingComet_Event_Text_A}You and some of your men are standing in a field at night gazing up at a " +
                "comet. It is one of the most beautiful sights you have ever seen. You cannot help wondering what " +
                "it really is. You have always been fascinated by the stars and night sky. Most people believe it's " +
                "the gods looking down on us, but you think otherwise.\n\nYou have often wondered if the stars " +
                "are the same thing as the Sun just much further away... Or perhaps they're angels of our fallen " +
                "ancestors watching over us. You have never shared these thoughts with anyone as most would think " +
                "you to be crazy. At least for now you can be fascinated by the amazing comet passing by. You and a " +
                "couple of your men end up standing there all night looking up and talking.",
                
                //Event Text B
                "{=PassingComet_Event_Text_B}You find yourself with a few of your men in a quiet field, staring up at a " +
                "comet streaking across the night sky. Its beauty is mesmerizing, filling you with wonder. You ponder " +
                "its nature—could it be a message from the gods, as some believe, or something entirely different?\n\n" +
                "The stars have always fascinated you. Are they distant suns, like your own, or perhaps the spirits " +
                "of those who came before us? Such thoughts linger in your mind, though you keep them to yourself. " +
                "For now, you simply enjoy the sight of the comet, talking quietly with your men through the night.",
                
                //Event Text C
                "{=PassingComet_Event_Text_C}Standing with some of your men in a dark field, you look up to see a comet " +
                "cutting across the night sky. Its brilliance captivates you, sparking questions about its origin. " +
                "Some say it’s the gods showing their presence, but you have other theories.\n\nCould the stars be suns, " +
                "farther than you could ever imagine? Or are they the spirits of those who have left this world? You " +
                "keep your musings private, fearing ridicule, but in this moment, you feel a deep connection to the heavens. " +
                "You and your men stay in the field, marveling at the celestial display and sharing quiet conversations.",
                
                //Event Text D
                "{=PassingComet_Event_Text_D}You stand with a few of your men under the vast night sky, captivated by a " +
                "brilliant comet streaking overhead. It feels otherworldly, and you find yourself wondering what it " +
                "really is. The stars have always been a source of fascination for you. Some believe they are the " +
                "watchful eyes of gods, but you suspect otherwise.\n\nCould the stars be distant suns? Or perhaps they " +
                "are the souls of those who once lived? These thoughts have always stayed with you, unspoken, but now, " +
                "under the comet's light, you feel the vastness of the cosmos. You and your men remain there all night, " +
                "gazing and talking in hushed tones.",
                
                //Event Text E
                "{=PassingComet_Event_Text_E}In the stillness of the night, you and a small group of your men watch a " +
                "comet blaze across the heavens. Its beauty is breathtaking, igniting your curiosity about the stars. " +
                "Some see them as the eyes of gods, but your mind wanders to other possibilities.\n\nCould the stars be " +
                "suns, unimaginably far away? Or are they the spirits of those who came before? You’ve often wondered " +
                "but never voiced these thoughts, fearing how others might react. Tonight, though, the comet provides a " +
                "moment of awe and reflection. You and your men stand together, quietly talking and marveling at the " +
                "wonders of the night sky."
            };
            
            
            public static string GetRandomEventTitle()
            {
                var index = random.Next(eventTitles.Count);
                return eventTitles[index];
            }
            
            public static string GetRandomEventText()
            {
                var index = random.Next(eventText.Count);
                return eventText[index];
            }
        }
	}
	

	public class PassingCometData : RandomEventData
	{
		public PassingCometData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
		{
		}

		public override BaseEvent GetBaseEvent()
		{
			return new PassingComet();
		}
	}
}
