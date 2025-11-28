using System;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.RandomEvents.Events
{
    public sealed class FantasticFighters : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int maxRenownGain;
        private readonly int minRenownGain;

        public FantasticFighters() : base(ModSettings.RandomEvents.FantasticFightersData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());

            eventDisabled = ConfigFile.ReadBoolean("FantasticFighters", "EventDisabled");
            minRenownGain = ConfigFile.ReadInteger("FantasticFighters", "MinRenownGain");
            maxRenownGain = ConfigFile.ReadInteger("FantasticFighters", "MaxRenownGain");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (!eventDisabled)
                if (minRenownGain != 0 || maxRenownGain != 0)
                    return true;

            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && Hero.MainHero.Clan != null;
        }

        public override void StartEvent()
        {
            try
            {
                var renownGain = MBRandom.RandomInt(minRenownGain, maxRenownGain);

                Hero.MainHero.Clan.Renown += renownGain;

                var eventTitle = new TextObject("{=FantasticFighters_Title}Fantastic Fighters?").ToString();

                var eventOption1 =
                    new TextObject(
                            "{=FantasticFighters_Event_Text}A rumor spreads that your clan managed to decisively win a battle when outnumbered 10-1.")
                        .ToString();

                var eventButtonText = new TextObject("{=FantasticFighters_Event_Button_Text}Done").ToString();

                InformationManager.ShowInquiry(
                    new InquiryData(eventTitle, eventOption1, true, false, eventButtonText, null, null, null), true);

                StopEvent();
            }
            catch (Exception ex)
            {
                MessageManager.DisplayMessage(
                    $"Error while playing \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }

        private void StopEvent()
        {
            try
            {
                if (onEventCompleted != null)
                    onEventCompleted.Invoke();
                else
                    MessageManager.DisplayMessage($"onEventCompleted was null while stopping \"{randomEventData.eventType}\" event.");
            }
            catch (Exception ex)
            {
                MessageManager.DisplayMessage(
                    $"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }
    }

    public class FantasticFightersData : RandomEventData
    {
        public FantasticFightersData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new FantasticFighters();
        }
    }
}