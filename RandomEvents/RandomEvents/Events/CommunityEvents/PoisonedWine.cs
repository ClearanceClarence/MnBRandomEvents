using System;
using System.Collections.Generic;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.RandomEvents.Events.CommunityEvents
{
    public sealed class PoisonedWine : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int maxSoldiersToDie;
        private readonly int maxSoldiersToHurt;
        private readonly int minSoldiersToDie;
        private readonly int minSoldiersToHurt;


        public PoisonedWine() : base(ModSettings.RandomEvents.PoisonedWineData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());

            eventDisabled = ConfigFile.ReadBoolean("PoisonedWine", "EventDisabled");
            minSoldiersToDie = ConfigFile.ReadInteger("PoisonedWine", "MinSoldiersToDie");
            maxSoldiersToDie = ConfigFile.ReadInteger("PoisonedWine", "MaxSoldiersToDie");
            minSoldiersToHurt = ConfigFile.ReadInteger("PoisonedWine", "MinSoldiersToHurt");
            maxSoldiersToHurt = ConfigFile.ReadInteger("PoisonedWine", "MaxSoldiersToHurt");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (!eventDisabled)
                if (minSoldiersToDie != 0 || maxSoldiersToDie != 0 || minSoldiersToHurt != 0 || maxSoldiersToHurt != 0)
                    return true;

            return false;
        }


        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.MemberRoster.TotalRegulars >= maxSoldiersToDie + 10 &&
                   MobileParty.MainParty.CurrentSettlement == null;
        }

        public override void StartEvent()
        {
            var heroName = Hero.MainHero.FirstName.ToString();

            var menKilled = MBRandom.RandomInt(minSoldiersToDie, maxSoldiersToDie);

            var menHurt = MBRandom.RandomInt(minSoldiersToHurt, maxSoldiersToHurt);

            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

            var eventText = new TextObject(EventTextHandler.GetRandomEventOutcomes()).ToString();

            var eventButtonText = new TextObject("{=PoisonedWine_Event_Button_Text}Continue").ToString();

            var eventMsg = new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("menKilled", menKilled)
                .SetTextVariable("menHurt", menHurt)
                .ToString();

            InformationManager.ShowInquiry(
                new InquiryData(eventTitle, eventText, true, false, eventButtonText, null, null, null), true);

            MobileParty.MainParty.MemberRoster.RemoveNumberOfNonHeroTroopsRandomly(menKilled);
            MobileParty.MainParty.MemberRoster.WoundNumberOfNonHeroTroopsRandomly(menHurt);

            InformationManager.DisplayMessage(new InformationMessage(eventMsg,
                RandomEventsSubmodule.Msg_Color_NEG_Outcome));

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
                "{=PoisonedWine_Title_A}Poisoned Wine",
                "{=PoisonedWine_Title_B}The Bitter Sip",
                "{=PoisonedWine_Title_C}Venom in the Chalice",
                "{=PoisonedWine_Title_D}A Toast to Treachery",
                "{=PoisonedWine_Title_E}Deadly Vintage",
                "{=PoisonedWine_Title_F}Tainted Spirits",
                "{=PoisonedWine_Title_G}The Fatal Drink",
                "{=PoisonedWine_Title_H}Vengeance in a Bottle",
                "{=PoisonedWine_Title_I}The Deadly Toast",
                "{=PoisonedWine_Title_J}Wine of Betrayal"
            };

            private static readonly List<string> eventOutcomes = new List<string>
            {
                // Event Text A
                "{=PoisonedWine_Event_Text_A}Late into the evening, you decide to host a celebration to boost troop morale. Drinks are overflowing, hunger is satiated, " +
                "and some troops have started games such as arm wrestling and cards. Everything seems perfect until your stomach begins to churn, and a commotion erupts " +
                "on the far side of the camp.\n\nYour men are doubled over in pain, some vomiting uncontrollably, others writhing on the ground in agony. A sinking realization " +
                "hits you—you've been poisoned. Panic sets in as questions swirl in your mind: Who did this? Why? But soon your thoughts blur, and darkness edges into your vision. " +
                "The last thing you see are motionless bodies among your men. You can only hope the poison won't claim more lives.",

                // Event Text B
                "{=PoisonedWine_Event_Text_B}As night falls, you throw a lively party to lift your men’s spirits. The camp buzzes with activity—drinks flow, food is devoured, " +
                "and cheers ring out from games of skill and chance. Just when you think the night couldn't be better, a wave of nausea grips you, and cries of distress erupt " +
                "from the crowd.\n\nYour men are collapsing, clutching their stomachs, some unable to stop vomiting. The realization hits hard: the wine is poisoned. As the chaos " +
                "spreads, your head spins with questions and fear. Before you lose consciousness, you catch a glimpse of lifeless figures scattered across the ground. You pray the " +
                "rest of your men will survive.",

                // Event Text C
                "{=PoisonedWine_Event_Text_C}The evening begins with joy and celebration. Your men, eager for a reprieve, dive into the feast and drink with abandon. The air is " +
                "filled with laughter, friendly banter, and the clinking of cups. But as the night deepens, something feels wrong. Your stomach churns violently, and the sounds of " +
                "celebration turn to screams and retching.\n\nMen collapse around you, their faces twisted in pain, some lying motionless. You stumble as dizziness takes over, and " +
                "the horrifying truth dawns on you—you’ve been poisoned. The camp descends into chaos as your vision fades. The last thing you see is the lifeless stare of fallen " +
                "soldiers.",

                // Event Text D
                "{=PoisonedWine_Event_Text_D}A night of revelry was supposed to bring cheer to the camp. Food and wine flowed as your troops enjoyed games and camaraderie under " +
                "the moonlight. But joy turns to horror when men start collapsing, clutching their stomachs in agony.\n\nYou feel the poison’s grip tightening in your own gut as " +
                "dizziness overtakes you. Panic spreads as vomiting, cries, and still bodies replace laughter. As your vision fades, you curse the unseen hand responsible for this " +
                "treachery. Only time will tell how many will survive this night of betrayal.",

                // Event Text E
                "{=PoisonedWine_Event_Text_E}The camp is alive with laughter and celebration. Wine flows freely, and the men seem happier than they’ve been in weeks. But as the " +
                "night deepens, a sinister turn of events unfolds. Your stomach churns violently, and soon you hear groans and cries from all around the camp.\n\nThe horrifying " +
                "realization dawns—your entire party has been poisoned. Men collapse, their pain evident as they clutch their stomachs or vomit uncontrollably. Before darkness " +
                "takes you, you notice several figures lying unnaturally still. This night of celebration has become a nightmare.",

                // Event Text F
                "{=PoisonedWine_Event_Text_F}A night of drinking and games seemed like the perfect way to boost morale. The camp is alive with energy, and spirits are high. But all " +
                "too soon, the atmosphere changes. Pain grips your stomach, and chaos erupts as your men fall ill, some writhing in agony while others remain ominously still.\n\n" +
                "The realization strikes you—you’ve been poisoned. The questions flood in, but the answers elude you as the poison takes hold. Before losing consciousness, you " +
                "witness the horrifying toll the treachery has taken on your men.",

                // Event Text G
                "{=PoisonedWine_Event_Text_G}The celebration begins with laughter, food, and wine. The camp feels alive with joy as men share stories and enjoy a rare moment of " +
                "peace. But the good cheer is short-lived. A wave of sickness spreads through the camp, and soon cries of pain replace the laughter.\n\nYour stomach twists in " +
                "agony, and dizziness sets in. The camp descends into chaos as men vomit, collapse, and cry out. The grim reality hits—you’ve been poisoned. As you struggle to " +
                "remain conscious, the sight of motionless bodies haunts you.",

                // Event Text H
                "{=PoisonedWine_Event_Text_H}You throw a feast to reward your men, hoping to lift their morale. The night begins with songs and cheer, but it doesn’t last. Your " +
                "stomach twists painfully, and you notice men falling to their knees, clutching their stomachs or vomiting violently.\n\nThe realization is chilling—you’ve all " +
                "been poisoned. The chaos around you intensifies as the poison spreads its grip. Darkness creeps into your vision as your strength fades. The last thing you see " +
                "is the lifeless bodies of your comrades, a haunting reminder of this act of betrayal.",

                // Event Text I
                "{=PoisonedWine_Event_Text_I}The night is filled with laughter, music, and celebration as your men enjoy the feast you’ve arranged. But the joy turns to panic when " +
                "sickness spreads through the camp. Your own stomach churns, and cries of pain echo around you.\n\nThe wine has been poisoned, and the effects are devastating. " +
                "Men collapse, some vomiting, others lying completely still. As the poison takes hold of you, your thoughts are consumed with questions of who would do this and " +
                "why. The sight of your fallen comrades is the last thing you see before darkness claims you.",

                // Event Text J
                "{=PoisonedWine_Event_Text_J}A night meant for celebration becomes a night of terror. The wine flows freely, and your men’s spirits are high, but the joy is " +
                "short-lived. Your stomach tightens painfully, and soon you hear the sounds of distress from all around the camp.\n\nYour men are falling, some vomiting, others " +
                "crying out in agony. The realization that the wine is poisoned fills you with dread. As you succumb to the effects, your vision blurs, and you see the lifeless " +
                "forms of your comrades scattered across the ground."
            };

            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=PoisonedWine_Event_Msg_1A}{heroName} lost {menKilled} men, and {menHurt} men were sickened by poisoned wine.",
                "{=PoisonedWine_Event_Msg_1B}After drinking poisoned wine, {heroName} lost {menKilled} men, and {menHurt} were left ill.",
                "{=PoisonedWine_Event_Msg_1C}{heroName}'s celebration turned tragic with {menKilled} men dead and {menHurt} men falling ill from poisoned wine.",
                "{=PoisonedWine_Event_Msg_1D}A night of revelry ended in tragedy as {heroName} lost {menKilled} men and saw {menHurt} others fall ill.",
                "{=PoisonedWine_Event_Msg_1E}The poisoned wine cost {heroName} dearly, with {menKilled} men dead and {menHurt} men suffering sickness.",
                "{=PoisonedWine_Event_Msg_1F}{heroName} counted {menKilled} dead and {menHurt} sick after the poisoned wine incident.",
                "{=PoisonedWine_Event_Msg_1G}The poisoned wine led to {menKilled} deaths and {menHurt} cases of sickness in {heroName}'s camp.",
                "{=PoisonedWine_Event_Msg_1H}The revelry turned to horror as {heroName} lost {menKilled} men and tended to {menHurt} others who fell ill.",
                "{=PoisonedWine_Event_Msg_1I}A tragic turn of events left {heroName} with {menKilled} men dead and {menHurt} men incapacitated by poisoned wine.",
                "{=PoisonedWine_Event_Msg_1J}{heroName} mourned the loss of {menKilled} men and the illness of {menHurt} more after the poisoned wine disaster."
            };

            public static string GetRandomEventTitle()
            {
                var index = random.Next(eventTitles.Count);
                return eventTitles[index];
            }

            public static string GetRandomEventOutcomes()
            {
                var index = random.Next(eventOutcomes.Count);
                return eventOutcomes[index];
            }

            public static string GetRandomEventMessage1()
            {
                var index = random.Next(eventMsg1.Count);
                return eventMsg1[index];
            }
        }
    }


    public class PoisonedWineData : RandomEventData
    {
        public PoisonedWineData(string eventType, float chanceWeight) : base(eventType,
            chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new PoisonedWine();
        }
    }
}