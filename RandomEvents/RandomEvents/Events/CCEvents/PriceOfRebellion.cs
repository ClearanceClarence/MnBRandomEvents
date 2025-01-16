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
    public sealed class PriceOfRebellion : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minBaseMoraleLoss;
        private readonly int maxBaseMoraleLoss;
        private readonly int minRenownGain;
        private readonly int maxRenownGain;
        private readonly int minRogueryLevel;

        public PriceOfRebellion() : base(ModSettings.RandomEvents.PriceOfRebellionData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("PriceOfRebellion", "EventDisabled");
            minBaseMoraleLoss = ConfigFile.ReadInteger("PriceOfRebellion", "MinBaseMoraleLoss");
            maxBaseMoraleLoss = ConfigFile.ReadInteger("PriceOfRebellion", "MaxBaseMoraleLoss");
            minRenownGain = ConfigFile.ReadInteger("PriceOfRebellion", "MinRenownGain");
            maxRenownGain = ConfigFile.ReadInteger("PriceOfRebellion", "MaxRenownGain");
            minRogueryLevel = ConfigFile.ReadInteger("PriceOfRebellion", "MinRogueryLevel");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled == false)
            {
                if (minBaseMoraleLoss != 0 || maxBaseMoraleLoss != 0 || minRenownGain != 0 || maxRenownGain != 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement != null && Hero.MainHero.Clan != null;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
            
            
            var heroName = Hero.MainHero.FirstName;
            
            var baseMoraleLoss = MBRandom.RandomInt(minBaseMoraleLoss, maxBaseMoraleLoss);

            var baseRenownGain = MBRandom.RandomInt(minRenownGain, maxRenownGain);
            
            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();

            var rogueryLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Roguery);
                
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription()).SetTextVariable("closestSettlement", closestSettlement).ToString();
            
            var canKillEveryone = false;
            var rogueryAppendedText = "";
            
            if (GeneralSettings.SkillChecks.IsDisabled())
            {
                canKillEveryone = true;
                rogueryAppendedText = new TextObject("{=PriceOfRebellion_Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();
                
            }
            else
            {
                if (rogueryLevel >= minRogueryLevel)
                {
                    canKillEveryone = true;
                    rogueryAppendedText = new TextObject("{=PriceOfRebellion_Roguery_Appended_Text}[Roguery - lvl {minRogueryLevel}]")
                        .SetTextVariable("minRogueryLevel", minRogueryLevel)
                        .ToString();
                }
            }

            var eventOption1 = new TextObject("{=PriceOfRebellion_Event_Option_1}Execute the Entire Family").ToString();
            var eventOption1Hover = new TextObject("{=PriceOfRebellion_Event_Option_1_Hover}Carry out a public execution of the entire family.\n{rogueryAppendedText}").SetTextVariable("rogueryAppendedText", rogueryAppendedText).ToString();

            var eventOption2 = new TextObject("{=PriceOfRebellion_Event_Option_2}Spare the Child, Execute the Rest").ToString();
            var eventOption2Hover = new TextObject("{=PriceOfRebellion_Event_Option_2_Hover}Spare the child but force them to watch as their family is executed.").ToString();

            var eventOption3 = new TextObject("{=PriceOfRebellion_Event_Option_3}Exile the Family").ToString();
            var eventOption3Hover = new TextObject("{=PriceOfRebellion_Event_Option_3_Hover}Send the family into exile.").ToString();

            var eventOption4 = new TextObject("{=PriceOfRebellion_Event_Option_4}Grant Mercy and Reinstate Them").ToString();
            var eventOption4Hover = new TextObject("{=PriceOfRebellion_Event_Option_4_Hover}Forgive the family and restore their status.").ToString();

            var eventButtonText1 = new TextObject("{=PriceOfRebellion_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=PriceOfRebellion_Event_Button_Text_2}Done").ToString();

            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
                new InquiryElement("c", eventOption3, null, true, eventOption3Hover),
                new InquiryElement("d", eventOption4, null, true, eventOption4Hover)
            };
            
            if (canKillEveryone){
                inquiryElements.Add(new InquiryElement("a", eventOption1, null, true, eventOption1Hover));
            }


            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventChoice1())
                .ToString();

            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventChoice2())
                .ToString();

            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventChoice3())
                .ToString();

            var eventOptionDText = new TextObject(EventTextHandler.GetRandomEventChoice4())
                .ToString();

            var eventMsg1 = new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("baseMoraleLoss", baseMoraleLoss)
                .ToString();

            var eventMsg2 = new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("heroName", heroName)
                .ToString();

            var eventMsg3 = new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("heroName", heroName)
                .ToString();

            var eventMsg4 = new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("baseRenownGain", baseRenownGain)
                .SetTextVariable("baseMoraleLoss", baseMoraleLoss - 5)
                .ToString();

            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1,
                eventButtonText1, null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= baseMoraleLoss;
                            MobileParty.MainParty.MoraleExplained.Add(-baseMoraleLoss);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_EVIL_Outcome));
                            break;
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_EVIL_Outcome));
                            break;
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_MED_Outcome));

                            break;
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);

                            Hero.MainHero.Clan.Renown += baseRenownGain;
                            
                            MobileParty.MainParty.RecentEventsMorale -= baseMoraleLoss - 5;
                            MobileParty.MainParty.MoraleExplained.Add(-baseMoraleLoss - 5);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_POS_Outcome));

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
                "{=PriceOfRebellion_Title_A}The Price of Rebellion",
                "{=PriceOfRebellion_Title_B}A Noble Betrayal",
                "{=PriceOfRebellion_Title_C}The Cost of Treachery",
                "{=PriceOfRebellion_Title_D}Judgment of the Betrayers",
                "{=PriceOfRebellion_Title_E}The Noble Conspiracy",
                "{=PriceOfRebellion_Title_F}Rebellion Among the Nobles",
                "{=PriceOfRebellion_Title_G}The Traitors’ Trial",
                "{=PriceOfRebellion_Title_H}A Test of Mercy",
                "{=PriceOfRebellion_Title_I}The Reckoning of the Noble House",
                "{=PriceOfRebellion_Title_J}The Traitor’s Dilemma"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=PriceOfRebellion_Event_Desc_1}Your guards have captured a noble family accused of plotting rebellion against your rule. Among the prisoners is a young child, " +
                "whose tearful pleas for mercy tug at the hearts of your men and the people gathered to witness their fate. The accusations are grave: treason, conspiracy, and the theft of state secrets. " +
                "The populace demands justice, crying out for retribution to safeguard the realm. This moment is more than a trial—it is a test of your leadership and resolve. How you handle " +
                "this family will echo through the halls of power and the whispers of your subjects for years to come.",

                
                //Event Description B
                "{=PriceOfRebellion_Event_Desc_2}The capture of a noble family suspected of treason has set your court ablaze with rumors and demands for swift action. " +
                "The accusations are severe, implicating the family in a plot to overthrow your rule. Their trial has drawn crowds from across the realm, each person eager to see how you will dispense justice. " +
                "Among the accused is a child, whose presence complicates the already delicate situation. To spare them may be seen as a sign of weakness, but to execute them could mark you as cruel and heartless. " +
                "The path forward is fraught with peril, and your decision will define how history remembers this day.",
                
                //Event Description C
                "{=PriceOfRebellion_Event_Desc_3}A noble family, long thought loyal to your rule, stands accused of plotting against you. They are now in your custody, their fate in your hands. " +
                "Though the charges are serious, the presence of a young child among the accused has sparked fierce debate. " +
                "The crowd demands blood, eager to see justice delivered swiftly and decisively. However, the repercussions of your actions extend far beyond the gallows. " +
                "Will you assert your dominance through fear, show mercy to gain favor, or choose a path that blends pragmatism with subtlety? The choice is yours, and the stakes could not be higher.",
                
                //Event Description D
                "{=PriceOfRebellion_Event_Desc_4}The arrest of a noble family accused of treachery has thrown your court into chaos. Among the prisoners is a child, " +
                "too young to fully grasp the gravity of the situation but old enough to plead for their life. The family’s crimes are undeniable: " +
                "conspiracy, theft, and betrayal of their sworn oaths. The people demand harsh punishment, but voices in your inner circle urge restraint, cautioning against the optics of brutality. " +
                "This moment is pivotal, a chance to solidify your rule and set an example for the realm. Whatever path you choose, it will leave a lasting mark on your reign.",
                
                //Event Description E
                "{=PriceOfRebellion_Event_Desc_5}A conspiracy has been uncovered, and the noble family at its heart now kneels before you in chains. " +
                "The accusations of rebellion and betrayal against your rule leave little room for doubt, but the presence of a young child among the prisoners complicates matters. " +
                "The crowd outside clamors for blood, demanding a show of strength to quash any thoughts of further dissent. Your advisors offer conflicting counsel, some urging " +
                "mercy to win favor, others pressing for decisive action to instill fear. In this moment, your decision will not only determine the " +
                "fate of this family but also the future stability of your realm."
            };
            
            private static readonly List<string> eventChoice1 = new List<string>
            {
                //Event Choice 1A
                "{=PriceOfRebellion_Event_Choice_1A}The entire noble family is brought to the center of the town square at dawn, where a large crowd has gathered. " +
                "Their crimes are announced to the public as your executioner sharpens their blade. One by one, the family is executed, including the young child, whose cries echo through the square. " +
                "The crowd watches in a mix of horror and awe as the heads are displayed on spikes around the town. \nYour message is clear: rebellion will be met with merciless punishment, and no one, not even children, is exempt.",
                
                //Event Choice 1B
                "{=PriceOfRebellion_Event_Choice_1B}At midday, the noble family is paraded through the streets, bound in chains. The young child clings to their parents, " +
                "their terrified sobs drowned out by the jeers of the gathered crowd. At the execution site, you personally address the crowd, declaring that treason has no place in your realm. " +
                "The entire family is executed without mercy, their bodies left on display as a warning. \nThe crowd disperses in silence, their fear a testament to the iron grip of your rule.",
                
                //Event Choice 1C
                "{=PriceOfRebellion_Event_Choice_1C}The noble family is dragged to the gallows at sunrise, their pleas for mercy ignored. " +
                "You order the child to go first, knowing the impact it will have on the crowd. Their cries for their family pierce the air as the noose tightens. " +
                "The remaining members of the family are executed in quick succession. \nThe sight of the lifeless bodies swinging from the gallows cements your authority, though whispers of your cruelty begin to circulate.",
                
                //Event Choice 1D
                "{=PriceOfRebellion_Event_Choice_1D}You waste no time in ordering the execution of the entire family. They are taken to the outskirts of the settlement, " +
                "where a massive pyre is built. The family is tied to stakes as the crowd gathers, horrified and mesmerized. With a single nod, the pyre is lit, and the family is consumed by flames. " +
                "Their screams echo through the valley, and the crowd watches in stunned silence. \nYour brutal display ensures no one will dare to question your authority again.",
                
                //Event Choice 1E
                "{=PriceOfRebellion_Event_Choice_1E}The noble family is forced to their knees in the town square as you deliver a scathing condemnation of their treachery. " +
                "The child weeps uncontrollably, clutching at their parents. Ignoring their cries, you signal the executioner to begin. Heads roll one after another, the child’s being the last. \n" +
                "The blood-soaked square becomes a chilling reminder of the consequences of rebellion. The townspeople avoid your gaze, terrified by the ruthlessness of your justice."
            };
            
            private static readonly List<string> eventChoice2 = new List<string>
            {
                //Event Choice 2A
                "{=PriceOfRebellion_Event_Choice_2A}You decree that the noble family will be executed, but the child will be spared. As the parents are dragged to the gallows, " +
                "the child cries out for them, their pleas falling on deaf ears. After the executions, you order the child to be taken in by a loyal family under strict supervision. \n" +
                "Though the child has been given a chance at life, they will always carry the memory of their family’s fate, ensuring their loyalty—or perhaps, their quiet hatred for you.",
                
                //Event Choice 2B
                "{=PriceOfRebellion_Event_Choice_2B}The noble family’s crimes are read aloud to the gathered crowd, and you declare that their punishment will be death. " +
                "However, you announce that the child will be spared and placed with a loyal family to ensure they grow up free of treachery. The executions proceed swiftly, " +
                "and the child, now orphaned, is handed over to their new guardians. \nYou hope that by sparing the child, you have struck a balance between justice and mercy.",
                
                //Event Choice 2C
                "{=PriceOfRebellion_Event_Choice_2C}You order the noble family to be executed, sparing only the young child. The executions are carried out in front of the terrified child, " +
                "who is later taken in by a trusted family under your command. The adoptive family is instructed to raise the child to be loyal to your rule, suppressing any thoughts of rebellion. \n" +
                "Time will tell if this act of mercy will secure their devotion or plant the seeds of vengeance.",
                
                //Event Choice 2D
                "{=PriceOfRebellion_Event_Choice_2D}As the noble family kneels before the executioner, you declare that the child will be spared but must watch their family’s fate. " +
                "The crowd murmurs at your decision as the executions proceed. Afterward, the child is taken to live with a trusted family in a distant settlement, far from their old life. \n" +
                "Your gesture of mercy is seen by some as wisdom, though others whisper that the child may one day seek revenge.",
                
                //Event Choice 2E
                "{=PriceOfRebellion_Event_Choice_2E}You make a calculated decision to execute the noble family while sparing the child. " +
                "As the crowd watches, the child is removed from the square before the grisly scene unfolds. The young orphan is placed with a family loyal to your cause, " +
                "their new guardians tasked with shaping the child’s future. \nYour hope is that mercy shown today will yield loyalty in the years to come, though such a gamble is never without risk."
            };
            
            private static readonly List<string> eventChoice3 = new List<string>
            {
                //Event Choice 3A
                "{=PriceOfRebellion_Event_Choice_3A}You declare that the noble family will be exiled rather than executed. They are escorted out of your lands under heavy guard, " +
                "stripped of their titles and possessions. Though they are free to go, you secretly assign spies to monitor their movements, ensuring they cannot rally support for rebellion. \n" +
                "The family’s exile is seen as a merciful act by some, but whispers of your calculated surveillance ensure your subjects know that no betrayal will go unnoticed.",
                
                //Event Choice 3B
                "{=PriceOfRebellion_Event_Choice_3B}You decide to banish the noble family, sparing them from execution. Before they leave, their oaths of loyalty are revoked, and they are warned " +
                "that any return to your lands will be met with death. Unbeknownst to them, your agents shadow their every move, gathering intelligence on any lingering treachery. \n" +
                "Your decision to exile them spreads throughout the realm, seen as a blend of mercy and cunning leadership.",
                
                //Event Choice 3C
                "{=PriceOfRebellion_Event_Choice_3C}Exile is your chosen punishment for the noble family. Stripped of their status and escorted beyond your borders, they leave under the watchful eyes " +
                "of your guards. Unknown to the family, spies are tasked with infiltrating their ranks and reporting back on their actions. \n" +
                "Your move is viewed by some as lenient, but others recognize the tactical brilliance of keeping your enemies under constant surveillance.",
                
                //Event Choice 3D
                "{=PriceOfRebellion_Event_Choice_3D}You command that the noble family be exiled from your lands. With nothing but the clothes on their backs, they are sent into the wilderness. " +
                "You ensure they believe they are free, but spies hidden among your ranks follow their movements closely. Should they attempt to plot against you again, you will know instantly. \n" +
                "The realm hears of your calculated mercy, leaving your enemies uncertain if exile truly means freedom.",
                
                //Event Choice 3E
                "{=PriceOfRebellion_Event_Choice_3E}The noble family is banished, their titles revoked and their estates seized. They are escorted out of your territory and warned never to return. " +
                "However, you instruct your agents to stay close, monitoring their activities from the shadows. Should they seek revenge, they will be swiftly dealt with. \n" +
                "This act of calculated mercy sends a clear message: you may spare your enemies, but they will never escape your reach."
            };
            
            private static readonly List<string> eventChoice4= new List<string>
            {
                //Event Choice 4A
                "{=PriceOfRebellion_Event_Choice_4A}You announce your decision to forgive the noble family and restore their titles and lands. " +
                "The crowd erupts in applause, praising your wisdom and mercy. However, among your soldiers, discontent brews. " +
                "Many feel that such leniency undermines discipline and encourages rebellion. \nWhile the people celebrate your magnanimity, the morale of your troops takes a sharp hit.",
                
                //Event Choice 4B
                "{=PriceOfRebellion_Event_Choice_4B}You declare that the noble family’s crimes will be pardoned, and they will be reinstated to their former status. " +
                "The townsfolk cheer your decision, viewing it as an act of compassion that unites the realm. Yet, your soldiers exchange uneasy glances, " +
                "their loyalty shaken by what they see as weakness. \nYour choice has strengthened public trust but weakened the resolve of your army.",
                
                //Event Choice 4C
                "{=PriceOfRebellion_Event_Choice_4C}After much deliberation, you choose to spare the noble family and reinstate their honor. " +
                "The crowd hails your mercy, their faith in your leadership reaffirmed. However, your men grumble in disapproval, " +
                "their morale dampened by the lack of retribution. \nYou leave knowing that while the people are pleased, your soldiers may question your resolve.",
                
                //Event Choice 4D
                "{=PriceOfRebellion_Event_Choice_4D}You announce that the noble family will be forgiven and restored to their former position. " +
                "The gathered crowd cheers wildly, their love for you growing stronger. Yet, as you glance at your soldiers, you see their disappointment. " +
                "Many believe justice was not served, and their loyalty wavers. \nThis decision secures the people’s favor but comes at the cost of your army’s morale.",
                
                //Event Choice 4E
                "{=PriceOfRebellion_Event_Choice_4E}Choosing mercy, you pardon the noble family and grant them their lands and titles once more. " +
                "The people cheer your compassion, seeing it as a beacon of hope and unity. However, your soldiers mutter among themselves, " +
                "their faith in your strength faltering. \nThough the people rejoice, the morale of your troops suffers a noticeable decline."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=PriceOfRebellion_Event_Msg_1A}{heroName} executed the noble family, striking fear into the people. Morale decreased by {baseMoraleLoss}.",
                "{=PriceOfRebellion_Event_Msg_1B}{heroName} ordered the public execution of the noble family. Troop morale fell by {baseMoraleLoss}.",
                "{=PriceOfRebellion_Event_Msg_1C}{heroName} showed no mercy, executing the noble family. Fear spread, but morale dropped by {baseMoraleLoss}.",
                "{=PriceOfRebellion_Event_Msg_1D}{heroName} ensured the noble family’s deaths were a warning to all. Morale declined by {baseMoraleLoss}.",
                "{=PriceOfRebellion_Event_Msg_1E}Through ruthless justice, {heroName} executed the noble family. Troop morale suffered a loss of {baseMoraleLoss}."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=PriceOfRebellion_Event_Msg_2A}{heroName} spared the child but executed the rest of the noble family.",
                "{=PriceOfRebellion_Event_Msg_2B}{heroName} showed partial mercy, sparing the child and executing the family.",
                "{=PriceOfRebellion_Event_Msg_2C}The noble family was executed, but {heroName} spared the child’s life.",
                "{=PriceOfRebellion_Event_Msg_2D}{heroName} allowed the child to live while the rest of the family faced execution.",
                "{=PriceOfRebellion_Event_Msg_2E}With calculated mercy, {heroName} spared the child but condemned the family."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=PriceOfRebellion_Event_Msg_3A}{heroName} exiled the noble family, ensuring they could no longer threaten the realm.",
                "{=PriceOfRebellion_Event_Msg_3B}{heroName} banished the noble family, stripping them of their titles and lands.",
                "{=PriceOfRebellion_Event_Msg_3C}Exile was {heroName}'s chosen punishment for the noble family’s betrayal.",
                "{=PriceOfRebellion_Event_Msg_3D}{heroName} spared the noble family’s lives, sentencing them to exile instead.",
                "{=PriceOfRebellion_Event_Msg_3E}The noble family was exiled under {heroName}'s orders, a calculated act of mercy."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            {
                "{=PriceOfRebellion_Event_Msg_4A}{heroName} pardoned the noble family, gaining {baseRenownGain} renown but losing {baseMoraleLoss} morale among the troops.",
                "{=PriceOfRebellion_Event_Msg_4B}{heroName} restored the noble family’s honor, earning {baseRenownGain} renown while troop morale fell by {baseMoraleLoss}.",
                "{=PriceOfRebellion_Event_Msg_4C}{heroName}'s mercy gained {baseRenownGain} renown with the people, but it cost {baseMoraleLoss} morale among the soldiers.",
                "{=PriceOfRebellion_Event_Msg_4D}The noble family was spared by {heroName}, increasing renown by {baseRenownGain} but reducing morale by {baseMoraleLoss}.",
                "{=PriceOfRebellion_Event_Msg_4E}Through mercy, {heroName} gained {baseRenownGain} renown but lost {baseMoraleLoss} morale among their men."
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
            
            public static string GetRandomEventChoice1()
            {
                var index = random.Next(eventChoice1.Count);
                return eventChoice1[index];
            }
            
            public static string GetRandomEventChoice2()
            {
                var index = random.Next(eventChoice2.Count);
                return eventChoice2[index];
            }
            
            public static string GetRandomEventChoice3()
            {
                var index = random.Next(eventChoice3.Count);
                return eventChoice3[index];
            }
            
            public static string GetRandomEventChoice4()
            {
                var index = random.Next(eventChoice4.Count);
                return eventChoice4[index];
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


    public class PriceOfRebellionData : RandomEventData
    {
        public PriceOfRebellionData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new PriceOfRebellion();
        }
    }
}