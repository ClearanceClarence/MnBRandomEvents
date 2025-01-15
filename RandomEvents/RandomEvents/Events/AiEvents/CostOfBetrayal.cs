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

namespace Bannerlord.RandomEvents.Events.AiEvents
{
    public sealed class CostOfBetrayal : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minBaseMoraleLoss;
        private readonly int maxBaseMoraleLoss;

        public CostOfBetrayal() : base(ModSettings.RandomEvents.CostOfBetrayalData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("CostOfBetrayal", "EventDisabled");
            minBaseMoraleLoss = ConfigFile.ReadInteger("CostOfBetrayal", "MinBaseMoraleLoss");
            maxBaseMoraleLoss = ConfigFile.ReadInteger("CostOfBetrayal", "MaxBaseMoraleLoss");
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
            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement != null;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
            
            var heroName = Hero.MainHero.FirstName;
            
            var baseMoraleLoss = MBRandom.RandomInt(minBaseMoraleLoss, maxBaseMoraleLoss);
            
            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription()).SetTextVariable("closestSettlement", closestSettlement).ToString();

            var eventOption1 = new TextObject("{=CostOfBetrayal_Event_Option_1}Torture and Execute").ToString();
            var eventOption1Hover = new TextObject("{=CostOfBetrayal_Event_Option_1_Hover}Surround, torture and execute them as an example.").ToString();

            var eventOption2 = new TextObject("{=CostOfBetrayal_Event_Option_2}Enslave Them").ToString();
            var eventOption2Hover = new TextObject("{=CostOfBetrayal_Event_Option_2_Hover}Capture the deserters.").ToString();

            var eventOption3 = new TextObject("{=CostOfBetrayal_Event_Option_3}Public Humiliation").ToString();
            var eventOption3Hover = new TextObject("{=CostOfBetrayal_Event_Option_3_Hover}Publicly humiliate and mutilate the deserters.").ToString();

            var eventOption4 = new TextObject("{=CostOfBetrayal_Event_Option_4}Let Them Go").ToString();
            var eventOption4Hover = new TextObject("{=CostOfBetrayal_Event_Option_4_Hover}Spare the deserters and allow them to escape.").ToString();

            var eventButtonText1 = new TextObject("{=CostOfBetrayal_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=CostOfBetrayal_Event_Button_Text_2}Done").ToString();


            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover),
                new InquiryElement("c", eventOption3, null, true, eventOption3Hover),
                new InquiryElement("d", eventOption4, null, true, eventOption4Hover)
            };


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
                .ToString();

            var eventMsg2 = new TextObject(EventTextHandler.GetRandomEventMessage2())
                .SetTextVariable("heroName", heroName)
                .ToString();

            var eventMsg3 = new TextObject(EventTextHandler.GetRandomEventMessage3())
                .SetTextVariable("heroName", heroName)
                .ToString();

            var eventMsg4 = new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("heroName", heroName)
                .SetTextVariable("baseMoraleLoss", baseMoraleLoss)
                .ToString();

            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1,
                eventButtonText1, null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_EVIL_Outcome));
                            break;
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            break;
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_MED_Outcome));

                            break;
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= baseMoraleLoss;
                            MobileParty.MainParty.MoraleExplained.Add(-baseMoraleLoss);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_MED_Outcome));

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
                "{=CostOfBetrayal_Title_A}The Cost of Betrayal",
                "{=CostOfBetrayal_Title_B}A Traitor’s Fate",
                "{=CostOfBetrayal_Title_C}Punishment for Treason",
                "{=CostOfBetrayal_Title_D}The Price of Disloyalty",
                "{=CostOfBetrayal_Title_E}Revenge on the Deserters",
                "{=CostOfBetrayal_Title_F}Betrayal in the Ranks",
                "{=CostOfBetrayal_Title_G}The Deserters’ Reckoning",
                "{=CostOfBetrayal_Title_H}Justice for Treachery",
                "{=CostOfBetrayal_Title_I}A Lesson in Loyalty",
                "{=CostOfBetrayal_Title_J}Blood for Betrayal"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=CostOfBetrayal_Event_Desc_1}Your scouts report troubling news: a group of deserters from your army has taken refuge in a nearby forest. " +
                "The deserters, numbering about a dozen, have not only stolen supplies but are reportedly stirring dissent among the locals, " +
                "spreading rumors about your leadership and encouraging rebellion. Their actions threaten to destabilize the region and weaken your authority. " +
                "Though their numbers are small, their influence could grow if left unchecked. Now is the time to act decisively, but how far are you willing to go to crush this treachery?",

                
                //Event Description B
                "{=CostOfBetrayal_Event_Desc_2}A messenger arrives with urgent news: deserters from your ranks have been spotted in the forest near {closestSettlement}. " +
                "These traitors have stolen valuable supplies and are using them to curry favor with the local populace, spreading dangerous propaganda against you. " +
                "Villagers have reported strange meetings in the woods, where the deserters speak of overthrowing your rule. " +
                "This rebellion, however small, cannot be ignored. Do you strike with swift and brutal force, or will you take a more calculated approach to deal with these traitors?",
                
                //Event Description C
                "{=CostOfBetrayal_Event_Desc_2}A messenger arrives with urgent news: deserters from your ranks have been spotted in the forest near {closestSettlement}. " +
                "These traitors have stolen valuable supplies and are using them to curry favor with the local populace, spreading dangerous propaganda against you. " +
                "Villagers have reported strange meetings in the woods, where the deserters speak of overthrowing your rule. " +
                "This rebellion, however small, cannot be ignored. Do you strike with swift and brutal force, or will you take a more calculated approach to deal with these traitors?",
                
                //Event Description D
                "{=CostOfBetrayal_Event_Desc_4}Reports of desertion within your ranks have reached you, and the situation is worse than you imagined. " +
                "A band of deserters has stolen provisions and fled to the forests near {closestSettlement}, where they are rumored to be inciting rebellion. " +
                "The deserters have grown bold, openly criticizing your rule and calling for resistance. Their words, coupled with their stolen resources, " +
                "have begun to attract the desperate and the disillusioned. Time is of the essence—act swiftly, or this spark of rebellion may ignite into a firestorm.",
                
                //Event Description E
                "{=CostOfBetrayal_Event_Desc_5}Treason is afoot. Scouts report a band of deserters has made camp deep within the forests near {closestSettlement}. " +
                "They have not only stolen supplies but are actively working to erode your influence, sowing discord among the villages. " +
                "These deserters are clever, using stolen resources to gain favor with the locals and fuel their campaign of defiance. " +
                "Their very existence undermines your rule and serves as a rallying point for malcontents. It is a delicate situation—one that demands a swift and decisive resolution. " +
                "The question is, how far will you go to crush this rebellion and restore your authority?"
            };
            
            private static readonly List<string> eventChoice1 = new List<string>
            {
                //Event Choice 1A
                "{=CostOfBetrayal_Event_Choice_1A}Your men surround the deserters’ camp under the cover of darkness, capturing them before they have a chance to fight back. " +
                "One by one, the deserters are interrogated, their screams echoing through the forest as they reveal the names of their accomplices and informants. " +
                "The information extracted is damning, implicating several villagers in acts of treachery. After the interrogations are complete, you order the deserters " +
                "to be executed in full view of the local population. \nTheir bodies are hung along the main road as a grim warning to others. Though brutal, your actions send a clear message: betrayal will not be tolerated.",
                
                //Event Choice 1B
                "{=CostOfBetrayal_Event_Choice_1B}With ruthless efficiency, your forces encircle the deserters and capture them without resistance. " +
                "You waste no time interrogating them, using harsh methods to ensure they reveal their secrets. Many break quickly, exposing a web of disloyalty that stretches beyond the deserters themselves. " +
                "Once the information is gathered, you order their immediate execution. The grisly scene is staged in the center of the village, ensuring every resident witnesses the consequences of disloyalty. \n" +
                "The fear instilled in the people is palpable, and you leave with the knowledge that no one will dare defy you soon.",
                
                //Event Choice 1C
                "{=CostOfBetrayal_Event_Choice_1C}Your men descend upon the deserters' camp with overwhelming force, taking them prisoner without much resistance. " +
                "You personally oversee the torturous interrogations, prying every bit of useful information from their broken spirits. " +
                "The deserters beg for mercy, but you remain unmoved. Their confessions lead to the exposure of sympathizers within the nearby villages. " +
                "After extracting every piece of intelligence, you order their public execution, turning the act into a spectacle of terror. \n" +
                "The locals watch in silence as the deserters are hanged, their lifeless bodies serving as a grim reminder of your authority.",
                
                //Event Choice 1D
                "{=CostOfBetrayal_Event_Choice_1D}Your soldiers strike swiftly, capturing the deserters before they can flee deeper into the forest. " +
                "The prisoners are dragged back to camp, where you oversee their brutal interrogations. The deserters scream and plead, but their suffering yields valuable information. " +
                "Names of collaborators and stolen supply caches are revealed under the pressure. When you have what you need, you order their execution. \n" +
                "The grisly act is carried out in the village square, with the deserters' heads displayed on pikes. The people look on in horror, their fear a testament to your iron rule.",
                
                //Event Choice 1E
                "{=CostOfBetrayal_Event_Choice_1E}Under the cover of night, your men surround the deserters' camp, capturing them with ease. " +
                "They are brought to you, and you waste no time extracting information through relentless interrogation. " +
                "The deserters break quickly, revealing not only their plans but also the identities of villagers who aided them. \n" +
                "With the confessions in hand, you order the executions to be carried out immediately. The gruesome spectacle unfolds at dawn, with the deserters' bodies displayed as a warning. " +
                "The villagers are left shaken, their loyalty cemented by fear of your retribution."
            };
            
            private static readonly List<string> eventChoice2 = new List<string>
            {
                //Event Choice 2A
                "{=CostOfBetrayal_Event_Choice_2A}Your men swiftly capture the deserters, chaining them together like cattle. " +
                "They are marched back to camp under heavy guard, their once-defiant expressions replaced with despair. " +
                "You waste no time assigning them to the most grueling and degrading tasks imaginable—quarrying stone, digging trenches, and hauling supplies. " +
                "Their defiance is beaten out of them through endless toil, and their suffering serves as a grim example to others. \n" +
                "By the time they collapse from exhaustion, their spirits are broken, ensuring they will never rebel again.",
                
                //Event Choice 2B
                "{=CostOfBetrayal_Event_Choice_2B}The deserters are subdued with ease, shackled and dragged back to your camp. " +
                "You address them coldly, informing them that they no longer have the privilege of freedom. " +
                "Dividing them into groups, you assign them to hard labor, ensuring they are worked to the brink of collapse. " +
                "Each day, they toil under the harshest conditions, their hands bloodied and their backs bent. \n" +
                "Your men oversee their work with whips, ensuring no one falters. By enslaving them, you demonstrate the price of betrayal to all who dare oppose you.",
                
                //Event Choice 2C
                "{=CostOfBetrayal_Event_Choice_2C}After surrounding the deserters, your men capture them without resistance. " +
                "You strip them of their weapons and dignity, binding them in chains and marching them to your camp. " +
                "From dawn to dusk, they are forced to labor in grueling conditions, digging ditches and repairing roads. " +
                "Any sign of rebellion is met with swift punishment, and the deserters quickly learn that their fate is worse than death. \n" +
                "Their suffering becomes a grim tale that spreads among your ranks, solidifying your reputation as a ruler who tolerates no disloyalty.",
                
                //Event Choice 2D
                "{=CostOfBetrayal_Event_Choice_2D}Your soldiers capture the deserters without much effort, dragging them back to your camp in chains. " +
                "You inform them of their fate: a lifetime of servitude under your rule. Their protests are silenced by the crack of a whip as they are sent to work in harsh conditions. " +
                "Each deserter is tasked with heavy labor, their bodies pushed to the brink as they toil endlessly. " +
                "Their cries for mercy are ignored, and your men take satisfaction in enforcing your will. \n" +
                "The deserters’ enslavement serves as a stark warning to any who would consider betraying you.",
                
                //Event Choice 2E
                "{=CostOfBetrayal_Event_Choice_2E}Your forces surround the deserters, capturing them with ease. " +
                "They are stripped of their weapons, chained, and brought to your camp as prisoners. " +
                "You order them to be put to work immediately, assigning them to the most backbreaking tasks in the harshest conditions. " +
                "Under constant supervision, they labor without respite, their spirits crushed by the weight of their chains. \n" +
                "Their enslavement is a reminder to your army and the people: betrayal will not only cost you freedom but will also bring unending suffering."
            };
            
            private static readonly List<string> eventChoice3 = new List<string>
            {
                //Event Choice 3A
                "{=CostOfBetrayal_Event_Choice_3A}Your soldiers capture the deserters and drag them into the center of the nearest village. " +
                "Under your orders, their uniforms are torn from their bodies, and their faces are painted with symbols of treachery. " +
                "In full view of the villagers, their hands are branded with a searing-hot iron, marking them as traitors for life. " +
                "Mockery and jeers from the crowd grow louder as the deserters are forced to march through the streets in chains. " +
                "Their public humiliation becomes a spectacle, spreading fear among all who witness their downfall. " +
                "By the time they are released, broken and shamed, they are nothing more than shadows of their former selves.",
                
                //Event Choice 3B
                "{=CostOfBetrayal_Event_Choice_3B}Captured without resistance, the deserters are paraded through the local settlement as criminals. " +
                "You order their heads shaved and their clothes replaced with rags, amplifying their disgrace. " +
                "In the village square, your soldiers force them to kneel as their crimes are read aloud to the gathered crowd. " +
                "Their ears are cut and their faces scarred, leaving them permanently marked as traitors. " +
                "The villagers, emboldened by your authority, spit and throw rotten food at the deserters. " +
                "By the time you leave, the deserters are humiliated beyond recognition, serving as a living testament to the price of betrayal.",
                
                //Event Choice 3C
                "{=CostOfBetrayal_Event_Choice_3C}Your forces quickly capture the deserters and bring them to the nearest village. " +
                "Under your direction, stocks are set up in the center of the square, and the deserters are locked into them. " +
                "The villagers are encouraged to hurl insults and debris at the traitors, who hang their heads in shame. " +
                "Later, you have their crimes carved into wooden plaques that are hung around their necks for all to see. " +
                "After a day of enduring this public humiliation, they are released, stripped of dignity and marked as outcasts. " +
                "Their broken spirits serve as a powerful warning to anyone considering rebellion.",
                
                //Event Choice 3D
                "{=CostOfBetrayal_Event_Choice_3D}Captured and subdued, the deserters are forced to march into the nearest town in chains. " +
                "You order them tied to posts in the town square, where they are mocked and pelted by villagers throughout the day. " +
                "As the final act of humiliation, their crimes are loudly proclaimed, and their foreheads are branded with the symbol of betrayal. " +
                "The deserters’ screams of pain and the villagers’ laughter create a chilling scene. " +
                "By the time they are released, the deserters are shadows of their former selves, living reminders of the consequences of treachery.",
                
                //Event Choice 3E
                "{=CostOfBetrayal_Event_Choice_3E}Your men capture the deserters and escort them to the village square, where a stage has been hastily assembled. " +
                "In front of a gathered crowd, their crimes are announced and their sentences declared. " +
                "The deserters are stripped of their uniforms, and their bodies are marked with scars and brands to signify their betrayal. " +
                "Villagers are invited to mock and ridicule them, throwing refuse and spitting in their direction. " +
                "Broken and humiliated, the deserters are released as pariahs, knowing they will never regain their honor. " +
                "The spectacle cements your reputation as a ruler who tolerates no betrayal."
            };
            
            private static readonly List<string> eventChoice4= new List<string>
            {
                //Event Choice 4A
                "{=CostOfBetrayal_Event_Choice_4A}Against the advice of your advisors, you decide to spare the deserters and let them flee. " +
                "Your men, though loyal, are visibly uneasy with your decision, questioning the leniency shown to traitors. " +
                "The deserters disappear into the forest, their fates unknown. Weeks later, troubling rumors surface— " +
                "the deserters have joined a bandit gang, emboldened by your mercy, and are raiding villages under your protection. " +
                "Your decision weighs heavily on you as your subjects begin to question your authority and resolve.",
                
                //Event Choice 4B
                "{=CostOfBetrayal_Event_Choice_4B}You command your men to stand down, sparing the lives of the deserters. " +
                "Grateful but cautious, they quickly gather their meager belongings and disappear into the wilderness. " +
                "While some of your men voice their disapproval, you believe in second chances. " +
                "However, your mercy soon backfires as reports come in of the deserters spreading their tales of survival, " +
                "emboldening others to consider following in their footsteps. What seemed like compassion may now become a dangerous precedent.",
                
                //Event Choice 4C
                "{=CostOfBetrayal_Event_Choice_4C}Deciding that the deserters are not worth the bloodshed, you allow them to leave unharmed. " +
                "With hesitant glances, they flee into the woods, offering promises of reform that you doubt they will keep. " +
                "Your men grumble about the lack of consequences, their loyalty shaken by what they perceive as weakness. " +
                "Months later, a bandit group emerges in the region, and the deserters are rumored to be among its leaders. " +
                "The villagers whisper that your mercy has cost them their safety.",
                
                //Event Choice 4D
                "{=CostOfBetrayal_Event_Choice_4D}You choose mercy, instructing your soldiers to let the deserters go. " +
                "The deserters hesitate, unsure if this is a trick, but eventually disappear into the forest, grateful for their lives. " +
                "Your men are visibly disappointed, murmuring about how leniency only breeds disobedience. " +
                "Over time, whispers of your decision spread, casting doubt on your willingness to punish betrayal. " +
                "In the shadows, others begin to question if defying you carries any real consequences.",
                
                //Event Choice 4E
                "{=CostOfBetrayal_Event_Choice_4E}You wave your hand dismissively, commanding your men to stand down and allow the deserters to leave. " +
                "Relieved, they vanish into the wilderness without a word of thanks. " +
                "Some of your soldiers exchange uneasy glances, worried that such mercy will invite future disloyalty. " +
                "Soon after, reports emerge of the deserters causing trouble in neighboring regions, raiding caravans and recruiting others to their cause. " +
                "Your decision to let them go begins to undermine your authority, casting a long shadow over your leadership."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=CostOfBetrayal_Event_Msg_1A}{heroName} ordered the deserters to be tortured and executed, sending a chilling message to all.",
                "{=CostOfBetrayal_Event_Msg_1B}{heroName} captured, tortured, and executed the deserters, ensuring no one dared to betray again.",
                "{=CostOfBetrayal_Event_Msg_1C}Under {heroName}'s orders, the deserters were interrogated, executed, and their bodies displayed as a warning.",
                "{=CostOfBetrayal_Event_Msg_1D}The deserters' betrayal was met with brutal punishment by {heroName}, who left their fate as an example to all.",
                "{=CostOfBetrayal_Event_Msg_1E}{heroName}'s swift and merciless judgment on the deserters solidified their rule through fear."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=CostOfBetrayal_Event_Msg_2A}{heroName} enslaved the deserters, forcing them into grueling labor as punishment for their betrayal.",
                "{=CostOfBetrayal_Event_Msg_2B}{heroName} captured the deserters and condemned them to a life of servitude and suffering.",
                "{=CostOfBetrayal_Event_Msg_2C}The deserters were enslaved under {heroName}'s command, their defiance crushed through endless toil.",
                "{=CostOfBetrayal_Event_Msg_2D}By enslaving the deserters, {heroName} ensured their betrayal would not go unpunished.",
                "{=CostOfBetrayal_Event_Msg_2E}{heroName}'s decision to enslave the deserters sent a clear message: betrayal leads only to misery."
            };
            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=CostOfBetrayal_Event_Msg_3A}{heroName} publicly humiliated the deserters, marking them as traitors for all to see.",
                "{=CostOfBetrayal_Event_Msg_3B}{heroName} had the deserters paraded through the streets, their shame serving as a warning to others.",
                "{=CostOfBetrayal_Event_Msg_3C}The deserters were branded and publicly humiliated under {heroName}'s orders, breaking their spirits completely.",
                "{=CostOfBetrayal_Event_Msg_3D}{heroName} turned the deserters’ punishment into a spectacle, ensuring their humiliation would not be forgotten.",
                "{=CostOfBetrayal_Event_Msg_3E}Under {heroName}'s rule, the deserters were scarred and shamed, their disgrace a lesson to all."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            {
                "{=CostOfBetrayal_Event_Msg_4A}{heroName} spared the deserters, causing {baseMoraleLoss} morale loss.",
                "{=CostOfBetrayal_Event_Msg_4B}{heroName} let the deserters go, resulting in {baseMoraleLoss} morale loss.",
                "{=CostOfBetrayal_Event_Msg_4C}Sparing the deserters cost {heroName} {baseMoraleLoss} morale.",
                "{=CostOfBetrayal_Event_Msg_4D}{heroName}'s mercy led to {baseMoraleLoss} morale loss among the troops.",
                "{=CostOfBetrayal_Event_Msg_4E}Letting the deserters go caused {baseMoraleLoss} morale loss under {heroName}'s command."
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


    public class CostOfBetrayalData : RandomEventData
    {
        public CostOfBetrayalData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new CostOfBetrayal();
        }
    }
}