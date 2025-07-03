using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.RandomEvents.Events
{
	public sealed class AheadOfTime : BaseEvent
	{
		private List<Settlement> eligibleSettlements;
		private readonly bool eventDisabled;

		public AheadOfTime() : base(ModSettings.RandomEvents.AheadOfTimeData)
		{
			var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
			eventDisabled = ConfigFile.ReadBoolean("AheadOfTime", "EventDisabled");
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

			if (HasValidEventData())
			{
				if (!Hero.MainHero.Clan.Settlements.Any()) return false;
				eligibleSettlements = new List<Settlement>();
				
				foreach (var s in Hero.MainHero.Clan.Settlements.Where(s => (s.IsTown || s.IsCastle) && s.Town.BuildingsInProgress.Count > 0))
				{
					eligibleSettlements.Add(s);
				}

				return eligibleSettlements.Count > 0;
			}

			return false;

		}

		public override void StartEvent()
		{	
			try
			{
				var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

				var randomElement = MBRandom.RandomInt(eligibleSettlements.Count);
				var settlement = eligibleSettlements[randomElement];

				settlement.Town.CurrentBuilding.BuildingProgress += settlement.Town.CurrentBuilding.GetConstructionCost() - settlement.Town.CurrentBuilding.BuildingProgress;
				settlement.Town.CurrentBuilding.LevelUp();
				settlement.Town.BuildingsInProgress.Dequeue();

				var eventText =new TextObject(EventTextHandler.GetRandomEventText())
					.SetTextVariable("settlement", settlement.ToString())
					.ToString();
				
				var eventButtonText1 = new TextObject("{=AheadOfTimeEvent_Event_Button_Text_1}Done").ToString();

				InformationManager.ShowInquiry(new InquiryData(eventTitle, eventText, true, false, eventButtonText1, null, null, null), true);

				StopEvent();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error while playing \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n { ex.StackTrace}");
			}
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
				"{=AheadOfTime_Title_A}Ahead of Time!",
				"{=AheadOfTime_Title_B}Finished Early!",
				"{=AheadOfTime_Title_C}Project Ahead of Schedule!",
				"{=AheadOfTime_Title_D}Ahead of the Curve!",
				"{=AheadOfTime_Title_E}A Swift Completion!",
				"{=AheadOfTime_Title_F}Progress Ahead of Time!",
				"{=AheadOfTime_Title_G}Done in Record Time!",
				"{=AheadOfTime_Title_H}Ahead of the Deadline!",
				"{=AheadOfTime_Title_I}Beating the Clock!",
				"{=AheadOfTime_Title_J}Ahead of Expectations!"
			};
            
			private static readonly List<string> eventText = new List<string>
			{
				//Event Text A
				"{=AheadOfTimeEvent_Text_A}You receive word that {settlement} has completed its current project earlier than expected.",
                
				//Event Text B
				"{=AheadOfTimeEvent_Text_B}News reaches you that {settlement} has finished its project ahead of schedule.",
                
				//Event Text C
				"{=AheadOfTimeEvent_Text_C}You are informed that the current project in {settlement} has been completed ahead of time.",
                
				//Event Text D
				"{=AheadOfTimeEvent_Text_D}A messenger brings great news: the project in {settlement} is finished earlier than planned.",
                
				//Event Text E
				"{=AheadOfTimeEvent_Text_E}Word arrives that {settlement} has successfully completed its project ahead of the expected timeline."
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

	public class AheadOfTimeData : RandomEventData
	{
		public AheadOfTimeData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
		{
		}

		public override BaseEvent GetBaseEvent()
		{
			return new AheadOfTime();
		}
	}
}
