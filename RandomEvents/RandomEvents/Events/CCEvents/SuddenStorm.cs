using System;
using System.Collections.Generic;
using System.Windows;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class SuddenStorm : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minHorsesLost;
        private readonly int maxHorsesLost;
        private readonly int minMenDied;
        private readonly int maxMenDied;
        private readonly int minMenWounded;
        private readonly int maxMenWounded;
        private readonly int minMeatFromHorse;
        private readonly int maxMeatFromHorse;

        public SuddenStorm() : base(ModSettings.RandomEvents.SuddenStormData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("SuddenStorm", "EventDisabled");
            minHorsesLost = ConfigFile.ReadInteger("SuddenStorm", "MinHorsesLost");
            maxHorsesLost = ConfigFile.ReadInteger("SuddenStorm", "MaxHorsesLost");
            minMenDied = ConfigFile.ReadInteger("SuddenStorm", "MinMenDied");
            maxMenDied = ConfigFile.ReadInteger("SuddenStorm", "MaxMenDied");
            minMenWounded = ConfigFile.ReadInteger("SuddenStorm", "MinMenWounded");
            maxMenWounded = ConfigFile.ReadInteger("SuddenStorm", "MaxMenWounded");
            minMeatFromHorse = ConfigFile.ReadInteger("SuddenStorm", "MinMeatFromHorse");
            maxMeatFromHorse = ConfigFile.ReadInteger("SuddenStorm", "MaxMeatFromHorse");
            
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minHorsesLost != 0 || maxHorsesLost != 0 || minMenDied != 0 || maxMenDied != 0 || minMenWounded != 0 || maxMenWounded != 0 || minMeatFromHorse != 0 || maxMeatFromHorse != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && Settlement.CurrentSettlement == null && MobileParty.MainParty.MemberRoster.TotalRegulars >= maxMenDied;

        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();
            
            var horsesLost = MBRandom.RandomInt(minHorsesLost, maxHorsesLost);
            var menDied = MBRandom.RandomInt(minMenDied, maxMenDied);
            var menWounded = MBRandom.RandomInt(minMenWounded, maxMenWounded);

            var meatFromHorseMultiplier = MBRandom.RandomInt(minMeatFromHorse, maxMeatFromHorse);

            var meatFromHorse = horsesLost * meatFromHorseMultiplier;
            
            var meat = MBObjectManager.Instance.GetObject<ItemObject>("meat");

            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("closestSettlement", closestSettlement)
                .ToString();
            
            var eventOption1 = new TextObject("{=SuddenStorm_Event_Option_1}Run for cover in the forest!").ToString();
            var eventOption1Hover = new TextObject("{=SuddenStorm_Event_Option_1_Hover}It offers some protection").ToString();

            var eventOption2 = new TextObject("{=SuddenStorm_Event_Option_2}Hide underneath the wagons!").ToString();
            var eventOption2Hover = new TextObject("{=SuddenStorm_Event_Option_2_Hover}Not all are going to fit").ToString();

            var eventOption3 = new TextObject("{=SuddenStorm_Event_Option_3}Press on a bit further").ToString();
            var eventOption3Hover = new TextObject("{=SuddenStorm_Event_Option_3_Hover}Try to find better shelter").ToString();

            var eventOption4 = new TextObject("{=SuddenStorm_Event_Option_4}The storm won't stop us!").ToString();
            var eventOption4Hover = new TextObject("{=SuddenStorm_Event_Option_4_Hover}You force your men to continue").ToString();

            var eventButtonText1 = new TextObject("{=SuddenStorm_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=SuddenStorm_Event_Button_Text_2}Done").ToString();
            

            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
                new InquiryElement("c", eventOption3, null, true, eventOption3Hover),
                new InquiryElement("d", eventOption4, null, true, eventOption4Hover),
            };

            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("menDied", menDied)
                .SetTextVariable("menWounded", menWounded)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();

            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventOutcome2())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("menDied", menDied)
                .SetTextVariable("menWounded", menWounded)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();

            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("menWounded", menWounded)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();

            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("menDied", menDied)
                .SetTextVariable("menWounded", menWounded)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();
                
            var eventMsg1 = new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", Hero.MainHero.FirstName.ToString())
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("menDied", menDied)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();
                
            var eventMsg2 = new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("heroName", Hero.MainHero.FirstName.ToString())
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("menDied", menDied)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();
                
            var eventMsg3 = new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("heroName", Hero.MainHero.FirstName.ToString())
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();
                
            var eventMsg4 = new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("heroName", Hero.MainHero.FirstName.ToString())
                .SetTextVariable("horsesLost", horsesLost)
                .SetTextVariable("menDied", menDied)
                .SetTextVariable("meatFromHorse", meatFromHorse)
                .ToString();
            
            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1,
                eventButtonText1, null,
                elements =>
                   {
                       switch ((string)elements[0].Identifier)
                       { 
                           case "a":
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                                
                                MobileParty.MainParty.ItemRoster.AddToCounts(meat, meatFromHorse);
                                MobileParty.MainParty.MemberRoster.KillNumberOfNonHeroTroopsRandomly(menDied);
                                MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(menWounded);
                                
                                InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                                break;
                            case "b":
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                                
                                MobileParty.MainParty.ItemRoster.AddToCounts(meat, meatFromHorse);
                                MobileParty.MainParty.MemberRoster.KillNumberOfNonHeroTroopsRandomly(menDied);
                                MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(menWounded);
                                
                                InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                                break;
                            case "c":
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                                
                                MobileParty.MainParty.ItemRoster.AddToCounts(meat, meatFromHorse);
                                MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(menWounded);
                                
                                InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_MED_Outcome));
                                break;
                            case "d":
                                InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                                
                                MobileParty.MainParty.ItemRoster.AddToCounts(meat, meatFromHorse);
                                MobileParty.MainParty.MemberRoster.KillNumberOfNonHeroTroopsRandomly(menDied);
                                MobileParty.MainParty.MemberRoster.WoundNumberOfTroopsRandomly(menWounded);
                                
                                InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                                break;
                            default:
                                MessageBox.Show($"Error while selecting option for \"{randomEventData.eventType}\"");
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
                MessageBox.Show(
                    $"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }
        
        private static class EventTextHandler
        {
            private static readonly Random random = new Random();
            
            private static readonly List<string> eventTitles = new List<string>
            {
                "{=SuddenStorm_Title_A}A Sudden Storm",
                "{=SuddenStorm_Title_B}The Tempest Strikes",
                "{=SuddenStorm_Title_C}Caught in the Storm",
                "{=SuddenStorm_Title_D}The Raging Skies",
                "{=SuddenStorm_Title_E}The Unexpected Tempest",
                "{=SuddenStorm_Title_F}Fury of the Heavens",
                "{=SuddenStorm_Title_G}The Unforgiving Storm",
                "{=SuddenStorm_Title_H}The Storm’s Wrath",
                "{=SuddenStorm_Title_I}Chaos in the Clouds",
                "{=SuddenStorm_Title_J}The Thunder’s Roar"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=SuddenStorm_Event_Desc_A}Your party is traveling near {closestSettlement} when you are suddenly " +
                "caught in a massive storm. Hailstones the size of fists fall around you, the rain pours in torrents, " +
                "the wind howls fiercely, and lightning cracks dangerously close. Wasting no time, you shout orders to your men.",
                
                //Event Description B
                "{=SuddenStorm_Event_Desc_B}While journeying near {closestSettlement}, a ferocious storm catches your party off guard. " +
                "The hailstones pelt down, rain lashes against you, and the winds threaten to topple your tents. Lightning streaks " +
                "across the sky as you quickly direct your men to action.",
                
                //Event Description C
                "{=SuddenStorm_Event_Desc_C}As your party nears {closestSettlement}, an intense storm descends without warning. " +
                "Massive hailstones, relentless rain, and howling winds assault you from all sides, while lightning flashes dangerously close. " +
                "You immediately bark orders to your men to secure the camp.",
                
                //Event Description D
                "{=SuddenStorm_Event_Desc_D}Near {closestSettlement}, your party is caught in the grip of a sudden and violent storm. " +
                "Hailstones hammer down, rain pours in sheets, and gale-force winds whip through the camp. With lightning striking nearby, " +
                "you don’t hesitate to rally your men and take control of the situation.",
                
                //Event Description E
                "{=SuddenStorm_Event_Desc_E}Your party is moving near {closestSettlement} when an unexpected storm strikes. " +
                "The hailstones are immense, rain lashes in waves, and fierce winds tear through the area. With lightning illuminating " +
                "the chaos, you quickly step in to organize your men."
            };
            
            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Event Outcome 1A
                "{=SuddenStorm_Event_Choice_1A}You order your men to take cover in the forest to the right. Dropping whatever they can, they make a desperate dash for the trees. " +
                "Once inside, you glance back and see the bodies of men and horses that couldn’t make it in time. There is nothing you can do for them now, so you huddle " +
                "together with your men and wait for the storm to pass.\n\nThe storm subsides as suddenly as it arrived. You inspect the damage, tending to the wounded " +
                "and deciding to take them to {closestSettlement} for proper care. All told, {horsesLost} horses and {menDied} men perished in the storm, while {menWounded} " +
                "were taken to {closestSettlement}. You bury the fallen in a single grave and salvage {meatFromHorse} meat from the dead horses.",
                
                //Event Outcome 1B
                "{=SuddenStorm_Event_Choice_1B}You shout for your men to make for the forest on the right. They drop their belongings and sprint for cover. " +
                "As you take shelter among the trees, you glance back to see fallen men and horses who weren’t quick enough to escape. " +
                "With no way to help them now, you stay with your men until the storm passes.\n\nWhen the skies clear, you assess the situation. " +
                "{horsesLost} horses and {menDied} men were lost to the storm, and {menWounded} require medical attention, so you head to {closestSettlement}. " +
                "You bury your dead in a single grave and manage to collect {meatFromHorse} meat from the fallen horses.",
                
                //Event Outcome 1C
                "{=SuddenStorm_Event_Choice_1C}You command your men to flee into the nearby forest for safety. They drop their gear and run, " +
                "but as you glance back, you see the lifeless bodies of men and horses who weren’t fast enough. There’s nothing you can do for them now, " +
                "so you stay with your men in the shelter of the trees until the storm subsides.\n\nOnce the storm clears, you evaluate the aftermath. " +
                "{horsesLost} horses and {menDied} men were lost, and {menWounded} are in need of care, prompting you to take them to {closestSettlement}. " +
                "You bury the fallen in a shared grave and salvage {meatFromHorse} meat from the horse carcasses.",
                
                //Event Outcome 1D
                "{=SuddenStorm_Event_Choice_1D}You urgently direct your men to the forest on the right, yelling for them to leave their belongings and move quickly. " +
                "As you find safety among the trees, you glance back at the grim sight of fallen men and horses. Helpless to aid them now, " +
                "you remain huddled with your men until the storm abates.\n\nWhen it’s over, you assess the damage. {horsesLost} horses and {menDied} men " +
                "were claimed by the storm, and {menWounded} are brought to {closestSettlement} for medical care. You bury the dead in a single grave, " +
                "recovering {meatFromHorse} meat from the horses.",
                
                //Event Outcome 1E
                "{=SuddenStorm_Event_Choice_1E}You quickly order your men to seek shelter in the forest to the right. Abandoning their belongings, they rush for the trees. " +
                "Looking back, you see the bodies of those who didn’t make it in time, both men and horses. You stay with your remaining men, waiting for the storm to pass.\n\n" +
                "When the storm finally clears, you assess the toll. {horsesLost} horses and {menDied} men were lost to the storm, while {menWounded} are transported to " +
                "{closestSettlement} for treatment. You bury the deceased in a single grave and salvage {meatFromHorse} meat from the fallen horses."
            };
            
            private static readonly List<string> EventOutcome2 = new List<string>
            {
                //Event Outcome 2A
                "{=SuddenStorm_Event_Choice_2A}You order your men to take shelter beneath the wagons, though some ignore your command and run toward the nearby forest. " +
                "As you lay on the muddy ground, you hear the relentless hail smashing equipment and killing several horses.\n\n" +
                "Suddenly, a bolt of lightning strikes the forest where the others fled, followed by cries for help carried on the wind. " +
                "Unable to assist them in the storm, you and your men remain under the wagons until it passes.\n\n" +
                "Once the storm clears, you assess the aftermath. {horsesLost} horses and {menDied} men were lost, while {menWounded} are taken to " +
                "{closestSettlement} for treatment. You bury the dead in a single grave and salvage {meatFromHorse} meat from the horse carcasses.",

                //Event Outcome 2B
                "{=SuddenStorm_Event_Choice_2B}You command your men to take cover beneath the wagons, but a few panic and dash into the nearby forest. " +
                "Lying on the ground, you hear the hailstones destroy equipment and kill some of the horses.\n\n" +
                "Suddenly, a lightning bolt strikes the forest, and over the roar of the wind, you hear cries for help. " +
                "There is nothing you can do for them now, so you and your men wait out the storm.\n\n" +
                "When the skies finally clear, you take stock of the situation. {horsesLost} horses and {menDied} men perished, and {menWounded} " +
                "are sent to {closestSettlement} for medical aid. You bury the fallen in a shared grave and recover {meatFromHorse} meat from the dead horses.",

                //Event Outcome 2C
                "{=SuddenStorm_Event_Choice_2C}You order everyone to take refuge under the wagons, though some men bolt for the forest in panic. " +
                "Lying in the mud, you can hear the hail ripping through your equipment and killing several horses.\n\n" +
                "A sudden lightning strike hits the forest, and cries of pain reach you through the howling wind. " +
                "Unable to act, you remain with your men under the wagons until the storm subsides.\n\n" +
                "After the storm passes, you survey the damage. {horsesLost} horses and {menDied} men were lost, and {menWounded} are transported to " +
                "{closestSettlement} for care. You bury the deceased in a single grave and salvage {meatFromHorse} meat from the fallen horses.",

                //Event Outcome 2D
                "{=SuddenStorm_Event_Choice_2D}You instruct your men to hide beneath the wagons, but a few panic and run for the forest. " +
                "Lying on the wet ground, you listen to the storm wreak havoc, destroying equipment and killing horses.\n\n" +
                "A bolt of lightning strikes the forest, and faint cries for help echo through the wind. Unable to reach them, you stay put until the storm passes.\n\n" +
                "When it’s over, you tally the losses: {horsesLost} horses and {menDied} men. {menWounded} are taken to {closestSettlement} for treatment. " +
                "You bury the dead and salvage {meatFromHorse} meat from the horse remains.",

                //Event Outcome 2E
                "{=SuddenStorm_Event_Choice_2E}You direct your men to shelter under the wagons, though some rush toward the forest in their panic. " +
                "As hail pounds down, you hear the destruction of your equipment and the death cries of several horses.\n\n" +
                "A lightning strike hits the forest, followed by faint screams. Unable to help, you hunker down with your men until the storm blows over.\n\n" +
                "When the storm clears, you assess the damage. {horsesLost} horses and {menDied} men were lost, and {menWounded} are brought to " +
                "{closestSettlement} for help. You bury your fallen men in a shared grave and recover {meatFromHorse} meat from the horses."
            };
            
            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Event Outcome 3A
                "{=SuddenStorm_Event_Choice_3A}You order your men to press on through the storm, hoping to find better shelter. " +
                "Reluctantly, they follow you toward a nearby rock face. Upon reaching it, you instruct everyone to dismount and stand close to the wall. " +
                "Your men comply, and you all endure what feels like an eternity waiting for the storm to pass. " +
                "A lightning bolt rips across the sky, and the thunderclap that follows spooks the horses, causing them to scatter in all directions. " +
                "Some of your men try to chase after them, but you convince them to wait until the storm subsides.\n\n" +
                "Once the storm passes, you begin assessing the damage. {horsesLost} horses were killed, and {menWounded} men were wounded. " +
                "You decide to take the injured to {closestSettlement} for treatment. After several hours, you manage to recover all surviving horses and salvage " +
                "{meatFromHorse} meat from the dead ones.",

                //Event Outcome 3B
                "{=SuddenStorm_Event_Choice_3B}Determined to find better shelter, you order your men to press on through the storm. " +
                "They hesitantly follow you to a nearby rock face, where you instruct everyone to dismount and gather close to the wall. " +
                "As the storm rages on, a bolt of lightning cracks through the sky, and the ensuing thunder spooks the horses, causing them to bolt. " +
                "Some of your men begin to pursue them, but you manage to convince them to stay put and wait for the storm to pass.\n\n" +
                "When the skies clear, you assess the situation. {horsesLost} horses died, and {menWounded} men were injured. " +
                "You transport the wounded to {closestSettlement} for care and recover the surviving horses after several hours of searching. " +
                "You also manage to salvage {meatFromHorse} meat from the dead horses.",

                //Event Outcome 3C
                "{=SuddenStorm_Event_Choice_3C}You instruct your men to continue through the storm in search of better shelter. " +
                "They reluctantly follow as you lead them toward a rock face. Upon arrival, you order them to dismount and stay close to the wall. " +
                "A lightning bolt cuts through the sky, followed by an earsplitting thunderclap that causes the horses to panic and scatter. " +
                "Some of your men attempt to chase after the horses, but you stop them, urging them to wait until the storm passes.\n\n" +
                "When the storm is over, you survey the damage. {horsesLost} horses were lost, and {menWounded} men were injured and sent to {closestSettlement} for aid. " +
                "After hours of searching, you recover the surviving horses and salvage {meatFromHorse} meat from the fallen ones.",

                //Event Outcome 3D
                "{=SuddenStorm_Event_Choice_3D}Hoping for better shelter, you command your men to move through the storm toward a nearby rock face. " +
                "They reluctantly obey, and upon reaching it, you instruct them to dismount and stand close to the wall. " +
                "The storm’s fury continues until a sudden bolt of lightning startles the horses, sending them running in every direction. " +
                "Your men want to pursue them, but you convince them to wait until the storm subsides.\n\n" +
                "Once the skies clear, you inspect the damage. {horsesLost} horses were killed, and {menWounded} men were wounded. " +
                "You transport the injured to {closestSettlement} and spend hours recovering the surviving horses. " +
                "Additionally, you salvage {meatFromHorse} meat from the horses that perished.",

                //Event Outcome 3E
                "{=SuddenStorm_Event_Choice_3E}You decide to push on through the storm, leading your men toward a nearby rock face for shelter. " +
                "Though hesitant, they follow. Once there, you order them to dismount and huddle close to the wall. " +
                "A bolt of lightning splits the sky, and the thunderous boom that follows sends the horses into a frenzy, scattering in all directions. " +
                "Some men move to chase the horses, but you stop them, insisting they wait for the storm to pass.\n\n" +
                "After the storm, you assess the losses. {horsesLost} horses were killed, and {menWounded} men were injured. " +
                "You transport the wounded to {closestSettlement} and eventually recover the surviving horses. " +
                "From the fallen horses, you manage to salvage {meatFromHorse} meat."
            };
            
            private static readonly List<string> EventOutcome4= new List<string>
            {
                //Event Outcome 4A
                "{=SuddenStorm_Event_Choice_4A}You proclaim that no storm will stop you and order your men to march through it. " +
                "After a few minutes, you hear their desperate pleas to stop, warning that some of your men have already been killed by the massive hail. " +
                "Reluctantly, you agree and signal everyone to take cover. As you dismount, an intense pain shoots through the back of your head, and " +
                "everything goes dark.\n\nYou awaken to your men calling your name and pouring water on your face. They help you to your feet as the sun shines, " +
                "birds sing, and the storm is but a distant memory. One of your men shows you your dented helmet, and you realize it saved your life. " +
                "Without it, you would surely have died.\n\nYou and your men inspect the damage and assist the wounded. {horsesLost} horses and {menDied} men " +
                "were lost, though you can’t help but think the toll would have been smaller had you stopped earlier. {menWounded} men are taken to " +
                "{closestSettlement} for treatment. You bury your dead in a shared grave and salvage {meatFromHorse} meat from the fallen horses.",

                //Event Outcome 4B
                "{=SuddenStorm_Event_Choice_4B}Determined to press on, you declare that no storm will stop your march and order your men forward. " +
                "Before long, the sound of pleading reaches you as your men beg to stop, informing you that several have already been killed by the hail. " +
                "You reluctantly agree and order everyone to take cover. As you dismount, you feel a sudden, sharp pain in the back of your head before " +
                "everything fades to black.\n\nYou awaken to find your men hovering over you, calling your name and pouring water on your face. " +
                "They help you to your feet as sunlight breaks through the clouds, and one of them shows you your helmet, dented from the storm. " +
                "You shudder to think what would have happened if you hadn’t been wearing it.\n\nAssessing the aftermath, you count {horsesLost} horses " +
                "and {menDied} men lost. {menWounded} men are taken to {closestSettlement} for aid. You bury the fallen in a single grave and recover " +
                "{meatFromHorse} meat from the dead horses.",

                //Event Outcome 4C
                "{=SuddenStorm_Event_Choice_4C}You rally your men, shouting that no storm shall halt your progress, and command them to march on. " +
                "The hail intensifies, and soon your men beg you to stop, warning that lives have already been lost. Realizing the danger, you " +
                "order everyone to find shelter. As you dismount, a sharp pain explodes at the back of your head, and the world fades to darkness.\n\n" +
                "You come to with your men surrounding you, calling your name and helping you up. One of them hands you your helmet, now bearing a " +
                "deep dent from the hail, and you realize how close you came to death. The storm has passed, leaving only distant rumbles behind.\n\n" +
                "You inspect the damage: {horsesLost} horses and {menDied} men are gone, and {menWounded} men are transported to {closestSettlement}. " +
                "Reflecting on your choices, you regret not stopping sooner. You bury the dead in a shared grave and salvage {meatFromHorse} meat from the horses.",

                //Event Outcome 4D
                "{=SuddenStorm_Event_Choice_4D}Refusing to be deterred by the storm, you command your men to march forward. The hail grows relentless, " +
                "and their pleas to stop soon reach your ears. They inform you that men have already died, and you reluctantly agree to halt and seek cover. " +
                "As you dismount, a sharp pain strikes the back of your head, and everything goes black.\n\n" +
                "You awaken to your men tending to you, pouring water on your face and calling your name. They help you to your feet, showing you your dented " +
                "helmet, which saved you from certain death. The storm is gone, leaving only distant echoes.\n\n" +
                "You assess the toll: {horsesLost} horses and {menDied} men were lost, and {menWounded} are sent to {closestSettlement}. " +
                "You bury the fallen in a communal grave and recover {meatFromHorse} meat from the dead horses.",

                //Event Outcome 4E
                "{=SuddenStorm_Event_Choice_4E}You boldly order your men to push through the storm, proclaiming that no weather will halt your march. " +
                "However, their cries to stop soon reach you, as they inform you that the hail has already claimed lives. Realizing the danger, " +
                "you halt the march and instruct everyone to take cover. As you dismount, a sudden impact to the back of your head sends you " +
                "into unconsciousness.\n\nYou awaken to the sun shining and your men helping you up. One shows you your helmet, now dented but " +
                "proof of how it saved your life. The storm has passed, but the toll remains. {horsesLost} horses and {menDied} men were lost, " +
                "and {menWounded} men are taken to {closestSettlement}. You bury the dead and salvage {meatFromHorse} meat from the fallen horses."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=SuddenStorm_Event_Msg_1A}{heroName} lost {horsesLost} horses and {menDied} men to a sudden storm. He also received {meatFromHorse} meat from butchering the dead horses.",
                "{=SuddenStorm_Event_Msg_1B}A sudden storm claimed {horsesLost} horses and {menDied} men from {heroName}'s party. {meatFromHorse} meat was salvaged from the dead horses.",
                "{=SuddenStorm_Event_Msg_1C}{heroName}'s party suffered the loss of {horsesLost} horses and {menDied} men to a sudden storm but managed to recover {meatFromHorse} meat.",
                "{=SuddenStorm_Event_Msg_1D}After a sudden storm, {heroName} lost {horsesLost} horses and {menDied} men. However, {meatFromHorse} meat was salvaged from the horses.",
                "{=SuddenStorm_Event_Msg_1E}{heroName} endured a sudden storm, losing {horsesLost} horses and {menDied} men, but salvaged {meatFromHorse} meat from the fallen horses."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=SuddenStorm_Event_Msg_2A}{heroName} lost {horsesLost} horses and {menDied} men to a sudden storm. He also received {meatFromHorse} meat from butchering the dead horses.",
                "{=SuddenStorm_Event_Msg_2B}A sudden storm claimed {horsesLost} horses and {menDied} men from {heroName}'s party. {meatFromHorse} meat was salvaged from the remains.",
                "{=SuddenStorm_Event_Msg_2C}During a fierce storm, {heroName} lost {horsesLost} horses and {menDied} men, but recovered {meatFromHorse} meat from the fallen horses.",
                "{=SuddenStorm_Event_Msg_2D}{heroName} suffered the loss of {horsesLost} horses and {menDied} men in a sudden storm, yet salvaged {meatFromHorse} meat from the dead horses.",
                "{=SuddenStorm_Event_Msg_2E}After enduring a sudden storm, {heroName} lost {horsesLost} horses and {menDied} men, but managed to gather {meatFromHorse} meat from the horses."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=SuddenStorm_Event_Msg_3A}{heroName} lost {horsesLost} horses to a sudden storm. He received {meatFromHorse} meat from butchering the dead horses.",
                "{=SuddenStorm_Event_Msg_3B}A sudden storm claimed {horsesLost} horses from {heroName}'s party, but {meatFromHorse} meat was salvaged from the remains.",
                "{=SuddenStorm_Event_Msg_3C}During the storm, {heroName} lost {horsesLost} horses but recovered {meatFromHorse} meat from their carcasses.",
                "{=SuddenStorm_Event_Msg_3D}{heroName} suffered the loss of {horsesLost} horses in a storm but managed to salvage {meatFromHorse} meat.",
                "{=SuddenStorm_Event_Msg_3E}A fierce storm took {horsesLost} horses from {heroName}, though {meatFromHorse} meat was butchered from the dead horses."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            { 
                "{=SuddenStorm_Event_Msg_4A}In refusing his men shelter from the storm, {heroName} lost {horsesLost} horses and {menDied} men to a sudden storm.",
                "{=SuddenStorm_Event_Msg_4B}{heroName}'s refusal to seek shelter cost his party {horsesLost} horses and {menDied} men during a sudden storm.",
                "{=SuddenStorm_Event_Msg_4C}By denying his men shelter, {heroName} suffered the loss of {horsesLost} horses and {menDied} men to the storm.",
                "{=SuddenStorm_Event_Msg_4D}{heroName}'s decision to press on through the storm resulted in the loss of {horsesLost} horses and {menDied} men.",
                "{=SuddenStorm_Event_Msg_4E}The storm claimed {horsesLost} horses and {menDied} men from {heroName}'s party after he refused to seek shelter."
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


    public class SuddenStormData : RandomEventData
    {
        public SuddenStormData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new SuddenStorm();
        }
    }
}