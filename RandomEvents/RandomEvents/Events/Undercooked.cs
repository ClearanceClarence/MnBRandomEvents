using System;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.RandomEvents.Events
{
    public sealed class Undercooked : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int maxTroopsToInjure;
        private readonly int minTroopsToInjure;

        public Undercooked() : base(ModSettings.RandomEvents.UndercookedData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());

            eventDisabled = ConfigFile.ReadBoolean("Undercooked", "EventDisabled");
            minTroopsToInjure = ConfigFile.ReadInteger("Undercooked", "MinTroopsToInjure");
            maxTroopsToInjure = ConfigFile.ReadInteger("Undercooked", "MaxTroopsToInjure");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (!eventDisabled)
                if (minTroopsToInjure != 0 || maxTroopsToInjure != 0)
                    return true;

            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() &&
                   MobileParty.MainParty.MemberRoster.TotalRegulars -
                   MobileParty.MainParty.MemberRoster.TotalWoundedRegulars >= minTroopsToInjure;
        }

        public override void StartEvent()
        {
            try
            {
                var numberToInjure = MBRandom.RandomInt(minTroopsToInjure, maxTroopsToInjure);

                numberToInjure = Math.Min(numberToInjure, maxTroopsToInjure);

                MobileParty.MainParty.MemberRoster.WoundNumberOfNonHeroTroopsRandomly(numberToInjure);

                var eventTitle = new TextObject("{=Undercooked_Title}Undercooked").ToString();

                var eventOption1 =
                    new TextObject(
                            "{=Undercooked_Event_Text}Some of your troops fall ill to bad food, although you're unsure of what caused it, you're glad it wasn't you.")
                        .ToString();

                var eventButtonText = new TextObject("{=Undercooked_Event_Button_Text}Done").ToString();

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

    public class UndercookedData : RandomEventData
    {
        public UndercookedData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new Undercooked();
        }
    }
}