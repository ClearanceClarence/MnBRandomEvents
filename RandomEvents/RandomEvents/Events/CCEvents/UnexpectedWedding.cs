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

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class UnexpectedWedding : BaseEvent
    {
        private readonly int embarrassedSoliderMaxGold;
        private readonly bool eventDisabled;
        private readonly int maxGoldRaided;
        private readonly int maxGoldToDonate;
        private readonly int maxPeopleInWedding;
        private readonly int minGoldRaided;
        private readonly int minGoldToDonate;
        private readonly int minPeopleInWedding;
        private readonly int minRogueryLevel;

        public UnexpectedWedding() : base(ModSettings.RandomEvents.UnexpectedWeddingData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());

            eventDisabled = ConfigFile.ReadBoolean("UnexpectedWedding", "EventDisabled");
            minGoldToDonate = ConfigFile.ReadInteger("UnexpectedWedding", "MinGoldToDonate");
            maxGoldToDonate = ConfigFile.ReadInteger("UnexpectedWedding", "MaxGoldToDonate");
            minPeopleInWedding = ConfigFile.ReadInteger("UnexpectedWedding", "MinPeopleInWedding");
            maxPeopleInWedding = ConfigFile.ReadInteger("UnexpectedWedding", "MaxPeopleInWedding");
            embarrassedSoliderMaxGold = ConfigFile.ReadInteger("UnexpectedWedding", "EmbarrassedSoliderMaxGold");
            minGoldRaided = ConfigFile.ReadInteger("UnexpectedWedding", "MinGoldRaided");
            maxGoldRaided = ConfigFile.ReadInteger("UnexpectedWedding", "MaxGoldRaided");
            minRogueryLevel = ConfigFile.ReadInteger("UnexpectedWedding", "MinRogueryLevel");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (!eventDisabled)
                if (minGoldToDonate != 0 || maxGoldToDonate != 0 || minPeopleInWedding != 0 ||
                    maxPeopleInWedding != 0 || embarrassedSoliderMaxGold != 0 || minGoldRaided != 0 ||
                    maxGoldRaided != 0 || minRogueryLevel != 0)
                    return true;

            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement == null &&
                   MobileParty.MainParty.MemberRoster.TotalRegulars >= maxPeopleInWedding;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

            var goldToDonate = MBRandom.RandomInt(minGoldToDonate, maxGoldToDonate);

            var heroName = Hero.MainHero.FirstName;

            var peopleInWedding = MBRandom.RandomInt(minPeopleInWedding, maxPeopleInWedding);

            var partyFood = MobileParty.MainParty.TotalFoodAtInventory;

            var goldBase = MBRandom.RandomInt(minGoldRaided, maxGoldRaided);

            var raidedGold = goldBase * peopleInWedding;

            var embarrassedSoliderGold = MBRandom.RandomInt(10, embarrassedSoliderMaxGold);

            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();

            var rogueryLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Roguery);

            var canRaidWedding = false;
            var rogueryAppendedText = "";

            if (GeneralSettings.SkillChecks.IsDisabled())
            {
                canRaidWedding = true;
                rogueryAppendedText =
                    new TextObject("{=Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();
            }
            else
            {
                if (rogueryLevel >= minRogueryLevel)
                {
                    canRaidWedding = true;
                    rogueryAppendedText = new TextObject("{=Roguery_Appended_Text}[Roguery - lvl {minRogueryLevel}]")
                        .SetTextVariable("minRogueryLevel", minRogueryLevel)
                        .ToString();
                }
            }

            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription())
                .SetTextVariable("closestSettlement", closestSettlement)
                .SetTextVariable("peopleInWedding", peopleInWedding)
                .ToString();

            var eventOption1 =
                new TextObject("{=UnexpectedWedding_Event_Option_1}Give them {goldToDonate} gold as a gift")
                    .SetTextVariable("goldToDonate", goldToDonate).ToString();
            var eventOption1Hover =
                new TextObject("{=UnexpectedWedding_Event_Option_1_Hover}This is a special day after all").ToString();

            var eventOption2 = new TextObject("{=UnexpectedWedding_Event_Option_2}Give them some wine to enjoy")
                .ToString();
            var eventOption2Hover =
                new TextObject(
                        "{=UnexpectedWedding_Event_Option_2_Hover}Who doesn't appreciate a good bottle of wine, right?")
                    .ToString();

            var eventOption3 = new TextObject("{=UnexpectedWedding_Event_Option_3}Stay for the ceremony").ToString();
            var eventOption3Hover =
                new TextObject(
                        "{=UnexpectedWedding_Event_Option_3_Hover}It's beautiful but you really don't want to waste any time")
                    .ToString();

            var eventOption4 = new TextObject("{=UnexpectedWedding_Event_Option_4}Leave").ToString();
            var eventOption4Hover =
                new TextObject("{=UnexpectedWedding_Event_Option_4_Hover}Not interested").ToString();

            var eventOption5 =
                new TextObject("{=UnexpectedWedding_Event_Option_5}[Roguery] Raid the wedding").ToString();
            var eventOption5Hover =
                new TextObject(
                        "{=UnexpectedWedding_Event_Option_5_Hover}You could do with some gold\n{rogueryAppendedText}")
                    .SetTextVariable("rogueryAppendedText", rogueryAppendedText).ToString();

            var eventButtonText1 = new TextObject("{=UnexpectedWedding_Event_Button_Text_1}Okay").ToString();
            var eventButtonText2 = new TextObject("{=UnexpectedWedding_Event_Button_Text_2}Done").ToString();

            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
                new InquiryElement("c", eventOption3, null, true, eventOption3Hover),
                new InquiryElement("d", eventOption4, null, true, eventOption4Hover)
            };
            if (canRaidWedding)
                inquiryElements.Add(new InquiryElement("e", eventOption5, null, true, eventOption5Hover));

            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventOutcome1())
                .SetTextVariable("goldToDonate", goldToDonate)
                .ToString();

            var eventOptionB1Text = new TextObject(EventTextHandler.GetRandomEventOutcome2A())
                .ToString();

            var eventOptionB2Text = new TextObject(EventTextHandler.GetRandomEventOutcome2B())
                .SetTextVariable("embarrassedSoliderGold", embarrassedSoliderGold)
                .ToString();

            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventOutcome3())
                .SetTextVariable("goldToDonate", goldToDonate)
                .ToString();

            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventOutcome4())
                .ToString();

            var eventOptionEText = new TextObject(EventTextHandler.GetRandomEventOutcome5())
                .SetTextVariable("raidedGold", raidedGold)
                .ToString();

            var eventMsg1 = new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("goldToDonate", goldToDonate)
                .ToString();

            var eventMsg2A = new TextObject(EventTextHandler.GetRandomEventMessage2A())
                .SetTextVariable("heroName", heroName)
                .ToString();

            var eventMsg2B = new TextObject(EventTextHandler.GetRandomEventMessage2B())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("embarrassedSoliderGold", embarrassedSoliderGold)
                .ToString();

            var eventMsg3 = new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("goldToDonate", goldToDonate)
                .ToString();

            var eventMsg5 = new TextObject(EventTextHandler.GetRandomEventMessage5())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("raidedGold", raidedGold)
                .ToString();

            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1,
                eventButtonText1, null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(
                                new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null,
                                    null), true);

                            Hero.MainHero.ChangeHeroGold(-goldToDonate);

                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1,
                                RandomEventsSubmodule.Msg_Color_POS_Outcome));

                            break;
                        case "b" when partyFood >= 5:
                            InformationManager.ShowInquiry(
                                new InquiryData(eventTitle, eventOptionB1Text, true, false, eventButtonText2, null,
                                    null, null), true);

                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2A,
                                RandomEventsSubmodule.Msg_Color_MED_Outcome));
                            break;
                        case "b" when partyFood < 5:
                        {
                            InformationManager.ShowInquiry(
                                new InquiryData(eventTitle, eventOptionB2Text, true, false, eventButtonText2, null,
                                    null, null), true);

                            Hero.MainHero.ChangeHeroGold(-embarrassedSoliderGold);

                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2B,
                                RandomEventsSubmodule.Msg_Color_MED_Outcome));

                            break;
                        }
                        case "c":
                            InformationManager.ShowInquiry(
                                new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null,
                                    null), true);
                            Hero.MainHero.ChangeHeroGold(-goldToDonate);

                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3,
                                RandomEventsSubmodule.Msg_Color));

                            break;
                        case "d":
                            InformationManager.ShowInquiry(
                                new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null,
                                    null), true);
                            break;
                        case "e":
                            InformationManager.ShowInquiry(
                                new InquiryData(eventTitle, eventOptionEText, true, false, eventButtonText2, null, null,
                                    null), true);

                            Hero.MainHero.ChangeHeroGold(+raidedGold);

                            InformationManager.DisplayMessage(new InformationMessage(eventMsg5,
                                RandomEventsSubmodule.Msg_Color));

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

        private static class EventTextHandler
        {
            private static readonly Random random = new Random();

            private static readonly List<string> eventTitles = new List<string>
            {
                "{=UnexpectedWedding_Title_A}An Unexpected Wedding",
                "{=UnexpectedWedding_Title_B}The Wedding Celebration",
                "{=UnexpectedWedding_Title_C}A Wedding by Chance",
                "{=UnexpectedWedding_Title_D}Festivities on the Road",
                "{=UnexpectedWedding_Title_E}The Wedding Encounter",
                "{=UnexpectedWedding_Title_F}A Joyful Discovery",
                "{=UnexpectedWedding_Title_G}The Wedding Surprise",
                "{=UnexpectedWedding_Title_H}Unplanned Festivities",
                "{=UnexpectedWedding_Title_I}A Celebration by Chance",
                "{=UnexpectedWedding_Title_J}The Roadside Wedding"
            };

            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=UnexpectedWedding_Event_Desc_A}You and your party are traveling near {closestSettlement} when you come across a group of {peopleInWedding} celebrating a wedding. " +
                "The joyous guests warmly invite you to join the festivities and celebrate this special occasion with them.",

                //Event Description B
                "{=UnexpectedWedding_Event_Desc_B}While traveling in the vicinity of {closestSettlement}, you and your party stumble upon a wedding with {peopleInWedding} guests in attendance. " +
                "The cheerful group extends an invitation for you to join their celebration of this happy event.",

                //Event Description C
                "{=UnexpectedWedding_Event_Desc_C}Near {closestSettlement}, your party encounters a lively gathering of {peopleInWedding} people celebrating a wedding. " +
                "The attendees enthusiastically invite you to join their festivities and share in the joyous occasion.",

                //Event Description D
                "{=UnexpectedWedding_Event_Desc_D}As your party travels close to {closestSettlement}, you come across a wedding celebration with {peopleInWedding} attendees. " +
                "The guests graciously invite you to participate in their joyous event and join the revelry.",

                //Event Description E
                "{=UnexpectedWedding_Event_Desc_E}Traveling near {closestSettlement}, you and your party happen upon a wedding gathering with {peopleInWedding} people. " +
                "The cheerful guests invite you to join in the celebration of this momentous occasion."
            };

            private static readonly List<string> EventOutcome1 = new List<string>
            {
                //Choice 1A
                "{=UnexpectedWedding_Event_Choice_1A}You warmly congratulate the couple, and you and your men gather {goldToDonate} gold as a wedding gift. " +
                "Your party then spends the evening enjoying the festivities and having fun.",

                //Choice 1B
                "{=UnexpectedWedding_Event_Choice_1B}You offer your congratulations to the newlyweds and collect {goldToDonate} gold from your men as a gift. " +
                "The evening is spent celebrating and enjoying the joyous atmosphere with the guests.",

                //Choice 1C
                "{=UnexpectedWedding_Event_Choice_1C}Congratulating the happy couple, you and your men pool together {goldToDonate} gold as a gift. " +
                "Your party spends the rest of the evening reveling in the celebration.",

                //Choice 1D
                "{=UnexpectedWedding_Event_Choice_1D}You congratulate the bride and groom and gather {goldToDonate} gold from your party as a token of goodwill. " +
                "The evening is filled with laughter and celebration as your men join the festivities.",

                //Choice 1E
                "{=UnexpectedWedding_Event_Choice_1E}After congratulating the newlyweds, you and your men donate {goldToDonate} gold as a wedding gift. " +
                "Your party then joins the guests and spends the evening celebrating the joyous occasion."
            };

            private static readonly List<string> EventOutcome2A = new List<string>
            {
                //Choice 2AA
                "{=UnexpectedWedding_Event_Choice_2AA}You have your men bring 5 bottles of your best wine and present it to the bride and groom. " +
                "They are overwhelmed with gratitude, thanking you profusely for such an exquisite gift. The evening continues with laughter and celebration as they share the wine with the guests.",

                //Choice 2AB
                "{=UnexpectedWedding_Event_Choice_2AB}Your men fetch 5 bottles of your finest wine, which you present to the bride and groom. " +
                "They are delighted by the gesture and express their thanks. The wine becomes the highlight of the evening, enjoyed by all the guests.",

                //Choice 2AC
                "{=UnexpectedWedding_Event_Choice_2AC}You offer 5 bottles of your best wine to the bride and groom, who accept the gift with joyful smiles. " +
                "The wine adds to the celebration, and the couple thanks you repeatedly for your generosity.",

                //Choice 2AD
                "{=UnexpectedWedding_Event_Choice_2AD}Your men retrieve 5 bottles of excellent wine, which you gift to the newlyweds. " +
                "The bride and groom are thrilled and express their heartfelt gratitude. The wine enhances the festivities, making the evening even more memorable.",

                //Choice 2AE
                "{=UnexpectedWedding_Event_Choice_2AE}You present 5 bottles of your finest wine to the happy couple. They are genuinely touched by the gesture and " +
                "thank you warmly. The wine is shared among the guests, elevating the joy and celebration of the wedding."
            };

            private static readonly List<string> EventOutcome2B = new List<string>
            {
                //Choice 2BA
                "{=UnexpectedWedding_Event_Choice_2BA}You instruct your men to fetch 5 bottles of your finest wine. After some time, an embarrassed soldier informs you that there is no wine left. " +
                "Furious at this humiliation, you slap him across the face and demand all his coin. He hands over {embarrassedSoliderGold} gold, which you then offer to the bride as an apology. " +
                "She accepts the gold graciously, and your party moves on.",

                //Choice 2BB
                "{=UnexpectedWedding_Event_Choice_2BB}Your men search for 5 bottles of your best wine, but a soldier returns looking ashamed and admits that none can be found. " +
                "Enraged, you strike him across the face and order him to surrender his coin. With {embarrassedSoliderGold} gold in hand, you offer it to the bride as compensation. " +
                "She thanks you politely, and you and your party leave the wedding.",

                //Choice 2BC
                "{=UnexpectedWedding_Event_Choice_2BC}After requesting your men to find 5 bottles of wine, a soldier hesitantly approaches, admitting that your reserves are empty. " +
                "Angered by the embarrassment, you slap him and demand all his coin. You hand over {embarrassedSoliderGold} gold to the bride as an apology, which she accepts. " +
                "Your party then departs the wedding.",

                //Choice 2BD
                "{=UnexpectedWedding_Event_Choice_2BD}You order your men to find wine, but a soldier returns empty-handed, clearly humiliated. " +
                "You reprimand him harshly, slapping him and taking {embarrassedSoliderGold} gold from him. Offering the gold to the bride as a substitute, you apologize. " +
                "The bride thanks you kindly, and you move on with your party.",

                //Choice 2BE
                "{=UnexpectedWedding_Event_Choice_2BE}When your men fail to find any wine, an ashamed soldier informs you of the situation. " +
                "Outraged, you slap him for this failure and demand all his coin, totaling {embarrassedSoliderGold} gold. You offer the gold to the bride as a gesture of goodwill. " +
                "She accepts with gratitude, and your party continues on its way."
            };

            private static readonly List<string> EventOutcome3 = new List<string>
            {
                //Choice 3A
                "{=UnexpectedWedding_Event_Choice_3A}You and your men stay to witness the wedding ceremony. Once it concludes, you leave a small gift of {goldToDonate} gold to the newlyweds and continue on your way.",

                //Choice 3B
                "{=UnexpectedWedding_Event_Choice_3B}You decide to stay for the ceremony alongside your men. After the vows are exchanged, you present a modest gift of {goldToDonate} gold to the couple and depart.",

                //Choice 3C
                "{=UnexpectedWedding_Event_Choice_3C}Your party attends the ceremony, respectfully observing the joyous occasion. As it concludes, you offer the newlyweds {goldToDonate} gold as a small token before leaving.",

                //Choice 3D
                "{=UnexpectedWedding_Event_Choice_3D}You and your men remain for the wedding ceremony. When it ends, you gift the couple {goldToDonate} gold and take your leave.",

                //Choice 3E
                "{=UnexpectedWedding_Event_Choice_3E}Staying for the ceremony, you and your men watch the happy couple exchange vows. At the end, you leave a small gift of {goldToDonate} gold for the newlyweds before departing."
            };

            private static readonly List<string> EventOutcome4 = new List<string>
            {
                //Choice 4A
                "{=UnexpectedWedding_Event_Choice_4A}You politely decline the invitation and order your men to leave the celebration.",

                //Choice 4B
                "{=UnexpectedWedding_Event_Choice_4B}You respectfully decline the offer to join the celebration and tell your men it’s time to leave.",

                //Choice 4C
                "{=UnexpectedWedding_Event_Choice_4C}You graciously refuse the invitation and instruct your men to continue on their way.",

                //Choice 4D
                "{=UnexpectedWedding_Event_Choice_4D}You decline the offer with respect and command your men to leave the wedding festivities.",

                //Choice 4E
                "{=UnexpectedWedding_Event_Choice_4E}With a polite refusal, you order your men to move on and leave the wedding behind."
            };

            private static readonly List<string> EventOutcome5 = new List<string>
            {
                //Choice 5A
                "{=UnexpectedWedding_Event_Choice_5A}You command your men to surround the area as you approach the guests with a sinister smile. " +
                "Demanding that everyone empty their pockets and surrender all valuables, you make it clear there will be dire consequences for resistance. " +
                "Those who hesitate are met with a chilling glare and the menacing presence of your soldiers, forcing them to comply. " +
                "After collecting {raidedGold} gold and other valuables, you order your men to desecrate the area. They gleefully obey, destroying decorations and overturning tables. " +
                "You savor the sight of the bride weeping and the groom staring at you with burning hatred. You know he will remember this day, and that fills you with grim satisfaction. " +
                "Once the wedding is utterly ruined, you order your men to leave, laughing as you walk away.",

                //Choice 5B
                "{=UnexpectedWedding_Event_Choice_5B}With cold determination, you order your men to surround the area and approach the terrified guests. " +
                "You demand that they hand over all valuables, showing no mercy to those who hesitate. A few guests resist, but your men quickly make examples of them, " +
                "forcing the others to fall in line. After looting {raidedGold} gold and all their treasures, you instruct your men to destroy everything in sight. " +
                "Laughter and chaos fill the air as your soldiers carry out your orders with sadistic enthusiasm. The bride’s sobs and the groom’s searing glare only add to your twisted sense of triumph. " +
                "Once the joyous occasion is reduced to ash, you call your men to leave, relishing the devastation left behind.",

                //Choice 5C
                "{=UnexpectedWedding_Event_Choice_5C}You signal your men to surround the wedding, ensuring no one escapes. " +
                "Calmly but coldly, you demand that all guests empty their pockets and surrender every valuable item they possess. " +
                "Resistance is met with brutal efficiency by your men, who intimidate and threaten anyone foolish enough to disobey. " +
                "With {raidedGold} gold and countless treasures in hand, you take it a step further and command your men to destroy the wedding site completely. " +
                "The bride’s wails and the groom’s silent, furious hatred are music to your ears as you oversee the destruction. " +
                "Satisfied with your cruelty, you order your men to march out, leaving behind shattered lives and a ruined celebration.",

                //Choice 5D
                "{=UnexpectedWedding_Event_Choice_5D}You have your men form a tight perimeter around the guests, ensuring none can flee. " +
                "With an icy smile, you demand all valuables be surrendered immediately. Any defiance is quickly crushed under the weight of your men’s menacing presence. " +
                "After looting {raidedGold} gold and numerous treasures, you escalate your tyranny, ordering your men to annihilate everything in sight. " +
                "Tables are smashed, decorations are torn down, and the festive atmosphere is replaced by despair. " +
                "The bride’s tear-streaked face and the groom’s blazing hatred only fuel your dark amusement. With the celebration thoroughly destroyed, you lead your men away, leaving chaos in your wake.",

                //Choice 5E
                "{=UnexpectedWedding_Event_Choice_5E}Commanding your men to surround the wedding, you stride toward the guests with a sinister air. " +
                "You demand all valuables, showing no patience for resistance. A few guests attempt to defy you, but the brutal actions of your men quickly silence them. " +
                "After collecting {raidedGold} gold and every shred of value, you instruct your men to destroy the wedding site completely. " +
                "The once-joyous event is turned into a scene of utter devastation, with shattered decorations and ruined tables strewn everywhere. " +
                "The bride’s anguished cries and the groom’s seething hatred are a satisfying reminder of your cruelty. With a final smirk, you order your men to leave, " +
                "knowing the scars of your actions will linger long after you’re gone."
            };

            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=UnexpectedWedding_Event_Msg_1A}{heroName} gave the newlyweds {goldToDonate} gold.",
                "{=UnexpectedWedding_Event_Msg_1B}{heroName} gifted {goldToDonate} gold to the newlyweds.",
                "{=UnexpectedWedding_Event_Msg_1C}The newlyweds received {goldToDonate} gold from {heroName}.",
                "{=UnexpectedWedding_Event_Msg_1D}{heroName} presented a gift of {goldToDonate} gold to the happy couple.",
                "{=UnexpectedWedding_Event_Msg_1E}{goldToDonate} gold was given to the newlyweds by {heroName}."
            };

            private static readonly List<string> eventMsg2A = new List<string>
            {
                "{=UnexpectedWedding_Event_Msg_2AA}{heroName} gifted 5 bottles of fine wine to the newlyweds, delighting the guests.",
                "{=UnexpectedWedding_Event_Msg_2AB}The newlyweds were thrilled to receive 5 bottles of exquisite wine from {heroName}.",
                "{=UnexpectedWedding_Event_Msg_2AC}{heroName} presented the bride and groom with 5 bottles of wine, making the celebration even more joyous.",
                "{=UnexpectedWedding_Event_Msg_2AD}With a generous gesture, {heroName} gifted 5 bottles of wine to the newlyweds, bringing smiles to all.",
                "{=UnexpectedWedding_Event_Msg_2AE}{heroName}'s gift of 5 bottles of fine wine added to the merriment of the wedding celebration."
            };

            private static readonly List<string> eventMsg2B = new List<string>
            {
                "{=UnexpectedWedding_Event_Msg_2BA}{heroName} was unable to provide wine and instead gave {embarrassedSoliderGold} gold to the newlyweds.",
                "{=UnexpectedWedding_Event_Msg_2BB}After failing to find wine, {heroName} handed over {embarrassedSoliderGold} gold as compensation to the bride.",
                "{=UnexpectedWedding_Event_Msg_2BC}{heroName} gave {embarrassedSoliderGold} gold to the newlyweds after being unable to provide wine.",
                "{=UnexpectedWedding_Event_Msg_2BD}When no wine was found, {heroName} offered {embarrassedSoliderGold} gold to the bride as an apology.",
                "{=UnexpectedWedding_Event_Msg_2BE}Without wine to give, {heroName} compensated the newlyweds with {embarrassedSoliderGold} gold."
            };

            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=UnexpectedWedding_Event_Msg_3A}{heroName} gave the newlyweds {goldToDonate} gold as a token of goodwill.",
                "{=UnexpectedWedding_Event_Msg_3B}{heroName} left {goldToDonate} gold with the newlyweds before departing.",
                "{=UnexpectedWedding_Event_Msg_3C}The newlyweds received a gift of {goldToDonate} gold from {heroName}.",
                "{=UnexpectedWedding_Event_Msg_3D}{heroName} generously donated {goldToDonate} gold to the bride and groom.",
                "{=UnexpectedWedding_Event_Msg_3E}{goldToDonate} gold was given to the happy couple by {heroName}."
            };

            private static readonly List<string> eventMsg5 = new List<string>
            {
                "{=UnexpectedWedding_Event_Msg_5A}{heroName} stole {raidedGold} gold from the wedding, leaving chaos in their wake.",
                "{=UnexpectedWedding_Event_Msg_5B}{heroName} looted {raidedGold} gold from the wedding and ruined the celebration.",
                "{=UnexpectedWedding_Event_Msg_5C}The wedding was left in shambles after {heroName} stole {raidedGold} gold from the guests.",
                "{=UnexpectedWedding_Event_Msg_5D}{heroName} raided the wedding, taking {raidedGold} gold and leaving devastation behind.",
                "{=UnexpectedWedding_Event_Msg_5E}With no regard for the celebration, {heroName} stole {raidedGold} gold from the wedding party."
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

            public static string GetRandomEventOutcome5()
            {
                var index = random.Next(EventOutcome5.Count);
                return EventOutcome5[index];
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

            public static string GetRandomEventMessage3()
            {
                var index = random.Next(eventMsg3.Count);
                return eventMsg3[index];
            }

            public static string GetRandomEventMessage5()
            {
                var index = random.Next(eventMsg5.Count);
                return eventMsg5[index];
            }
        }
    }


    public class UnexpectedWeddingData : RandomEventData
    {
        public UnexpectedWeddingData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new UnexpectedWedding();
        }
    }
}