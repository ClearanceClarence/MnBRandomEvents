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
    public sealed class BeeKind : BaseEvent
    {
        private readonly int damage;
        private readonly bool eventDisabled;
        private readonly float reactionChance;
        private readonly int reactionDamage;


        public BeeKind() : base(ModSettings.RandomEvents.BeeKindData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());

            eventDisabled = ConfigFile.ReadBoolean("BeeKind", "EventDisabled");
            damage = ConfigFile.ReadInteger("BeeKind", "Damage");
            reactionChance = ConfigFile.ReadFloat("BeeKind", "reactionChance");
            reactionDamage = ConfigFile.ReadInteger("BeeKind", "reactionDamage");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (!eventDisabled)
                if (damage != 0 || reactionChance != 0 || reactionDamage != 0)
                    return true;

            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData();
        }

        public override void StartEvent()
        {
            var extraDialogue = "";
            var damageToInflict = damage;

            var eventTitle = new TextObject("{=BeeKind_Title}Bee Kind").ToString();

            var eventExtraDialogue =
                new TextObject("{=BeeKind_Event_Extra_Dialogue}Your body reacts painfully to the sting. ").ToString();

            if (MBRandom.RandomFloatRanged(0.0f, 1.0f) <= reactionChance)
            {
                extraDialogue = eventExtraDialogue;
                damageToInflict = reactionDamage;
            }

            var eventText =
                new TextObject(
                        "{=BeeKind_Event_Text}As you sit down next to some flowers you get stung by a bee! {extraDialogue}Why is nature so cruel?")
                    .SetTextVariable("extraDialogue", extraDialogue)
                    .ToString();

            var eventButtonText = new TextObject("{=BeeKind_Event_Button_Text}Ouch").ToString();

            Hero.MainHero.HitPoints -= damageToInflict;

            InformationManager.ShowInquiry(
                new InquiryData(eventTitle, eventText, true, false, eventButtonText, null, null, null), true);

            StopEvent();
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


    public class BeeKindData : RandomEventData
    {
        public BeeKindData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new BeeKind();
        }
    }
}