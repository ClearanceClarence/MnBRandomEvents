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
using TaleWorlds.MountAndBlade;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public sealed class BloodToll : BaseEvent
    {
        private readonly bool eventDisabled;
        private readonly int minRogueryLevel;
        private readonly int minGoldLoot;
        private readonly int maxGoldLoot;
        private readonly int minRandomXP;
        private readonly int maxRandomXP;
        private readonly int minRenownLost;
        private readonly int maxRenownLost;

        public BloodToll() : base(ModSettings.RandomEvents.BloodTollData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("BloodToll", "EventDisabled");
            minRogueryLevel = ConfigFile.ReadInteger("BloodToll", "MinRogueryLevel");
            minGoldLoot = ConfigFile.ReadInteger("BloodToll", "MinGoldLoot");
            maxGoldLoot = ConfigFile.ReadInteger("BloodToll", "MaxGoldLoot");
            minRandomXP = ConfigFile.ReadInteger("BloodToll", "MinRandomXP");
            maxRandomXP = ConfigFile.ReadInteger("BloodToll", "MaxRandomXP");
            minRenownLost = ConfigFile.ReadInteger("BloodToll", "MinRenownLost");
            maxRenownLost = ConfigFile.ReadInteger("BloodToll", "MaxRenownLost");
        }

        public override void CancelEvent()
        {
        }

        private bool HasValidEventData()
        {
            if (eventDisabled) return false;
            
            return minRogueryLevel != 0 || minGoldLoot != 0 || maxGoldLoot != 0 || minRandomXP != 0 || maxRandomXP != 0 || minRenownLost != 0 || maxRenownLost != 0;
        }

        public override bool CanExecuteEvent()
        {
            return HasValidEventData() && MobileParty.MainParty.CurrentSettlement == null && MobileParty.MainParty.PrisonRoster.TotalRegulars >= 12;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();
            
            var heroName = Hero.MainHero.FirstName;
            
            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();
            
            var eventDescription = new TextObject(EventTextHandler.GetRandomEventDescription()).SetTextVariable("closestSettlement", closestSettlement).ToString();
            
            var goldToLoot = MBRandom.RandomInt(minGoldLoot, maxGoldLoot);
            
            var renownLost = MBRandom.RandomInt(minRenownLost, maxRenownLost);
            
            var rogueryLevel = Hero.MainHero.GetSkillValue(DefaultSkills.Roguery);

            var canKill = false;
            
            var rogueryAppendedText = "";
            
            if (GeneralSettings.SkillChecks.IsDisabled())
            {
                
                canKill = true;

                rogueryAppendedText = new TextObject("{=Skill_Check_Disable_Appended_Text}**Skill checks are disabled**").ToString();

            }
            else
            {
                if (rogueryLevel >= minRogueryLevel)
                {
                    canKill = true;
                    
                    rogueryAppendedText = new TextObject("{=Roguery_Appended_Text}[Roguery - lvl {minRogueryLevel}]")
                        .SetTextVariable("minRogueryLevel", minRogueryLevel)
                        .ToString();
                }
            }

            var eventOption1 = new TextObject("{=BloodToll_Event_Option_1}Sacrifice Prisoners").ToString();
            var eventOption1Hover = new TextObject("{=BloodToll_Event_Option_1_Hover}Hand over a dozen prisoners").ToString();

            var eventOption2 = new TextObject("{=BloodToll_Event_Option_2}Kidnap Travelers").ToString();
            var eventOption2Hover = new TextObject("{=BloodToll_Event_Option_2_Hover}Abduct travelers on the road,").ToString();

            var eventOption3 = new TextObject("{=BloodToll_Event_Option_3}Betray and Kill").ToString();
            var eventOption3Hover = new TextObject("{=BloodToll_Event_Option_3_Hover}Execute him, and loot his belongings.\n{rogueryAppendedText}").SetTextVariable("rogueryAppendedText", rogueryAppendedText).ToString();

            var eventOption4 = new TextObject("{=BloodToll_Event_Option_4}Refuse and Banish").ToString();
            var eventOption4Hover = new TextObject("{=BloodToll_Event_Option_4_Hover}Reject the offer and order your men to escort the wanderer away.").ToString();

            var eventButtonText1 = new TextObject("{=BloodToll_Event_Button_Text_1}Choose").ToString();
            var eventButtonText2 = new TextObject("{=BloodToll_Event_Button_Text_2}Done").ToString();
            
            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", eventOption1, null, true, eventOption1Hover),
                new InquiryElement("b", eventOption2, null, true, eventOption2Hover)
            };

            if (canKill)
            { 
                inquiryElements.Add(new InquiryElement("c", eventOption3, null, true, eventOption3Hover));
            }

            inquiryElements.Add(new InquiryElement("d", eventOption4, null, true, eventOption4Hover));


            var eventOptionAText = new TextObject(EventTextHandler.GetRandomEventChoice1())
                .ToString();

            var eventOptionBText = new TextObject(EventTextHandler.GetRandomEventChoice2())
                .ToString();

            var eventOptionCText = new TextObject(EventTextHandler.GetRandomEventChoice3())
                .SetTextVariable("goldToLoot", goldToLoot)
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
                .SetTextVariable("goldToLoot",goldToLoot)
                .ToString();

            var eventMsg4 = new TextObject(EventTextHandler.GetRandomEventMessage4())
                .SetTextVariable("heroName", heroName)
                .ToString();

            var msid = new MultiSelectionInquiryData(eventTitle, eventDescription, inquiryElements, false, 1, 1,
                eventButtonText1, null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionAText, true, false, eventButtonText2, null, null, null), true);

                            MobileParty.MainParty.PrisonRoster.RemoveNumberOfNonHeroTroopsRandomly(12);
                            HelperFunctions.AssignRandomSkillXp(minRandomXP, maxRandomXP);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_EVIL_Outcome));
                            break;
                        case "b":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionBText, true, false, eventButtonText2, null, null, null), true);
                            
                            HelperFunctions.AssignRandomSkillXp(minRandomXP/2, maxRandomXP/2);
                            Hero.MainHero.Clan.Renown -= renownLost;
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg2, RandomEventsSubmodule.Msg_Color_NEG_Outcome));
                            break;
                        case "c":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionCText, true, false, eventButtonText2, null, null, null), true);
                            
                            Hero.MainHero.ChangeHeroGold(+goldToLoot);
                            
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg3, RandomEventsSubmodule.Msg_Color_MED_Outcome));

                            break;
                        case "d":
                            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventOptionDText, true, false, eventButtonText2, null, null, null), true);
                            InformationManager.DisplayMessage(new InformationMessage(eventMsg4, RandomEventsSubmodule.Msg_Color_MED_Outcome));

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
                {
                    onEventCompleted.Invoke();
                }
                else
                {
                    MessageManager.DisplayMessage($"onEventCompleted was null while stopping \"{randomEventData.eventType}\" event.");
                }
            }
            catch (Exception ex)
            {
                MessageManager.DisplayMessage($"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }

        private static class EventTextHandler
        {
            private static readonly Random random = new Random();
            
            private static readonly List<string> eventTitles = new List<string>
            {
                "{=BloodToll_Title_A}The Blood Toll",
                "{=BloodToll_Title_B}Sacrifice for Power",
                "{=BloodToll_Title_C}The Price of Strength",
                "{=BloodToll_Title_D}The Wanderer’s Demand",
                "{=BloodToll_Title_E}Blood for Power",
                "{=BloodToll_Title_F}The Dark Pact",
                "{=BloodToll_Title_G}Strength Through Sacrifice",
                "{=BloodToll_Title_H}A Sinister Bargain",
                "{=BloodToll_Title_I}The Blood Ritual",
                "{=BloodToll_Title_J}The Wanderer’s Toll"
            };
            
            private static readonly List<string> eventDescriptions = new List<string>
            {
                //Event Description A
                "{=BloodToll_Event_Desc_1}As the moon casts an eerie glow over your camp, a cloaked wanderer emerges from the shadows. His voice is barely above a whisper, " +
                "yet it carries an unnatural weight that makes your soldiers uneasy. He looks directly at you, his eyes gleaming with dark intent, and speaks of forbidden power " +
                "beyond imagination. \n\nThe price? A blood sacrifice. He knows of the prisoners you hold and offers you a choice: deliver a dozen captives—innocents or soldiers— " +
                "to fuel his unholy ritual. Their lives, he claims, will grant you strength unlike any mortal. The air grows colder as he awaits your decision, his presence " +
                "chilling to the core.",
                
                //Event Description B
                "{=BloodToll_Event_Desc_2}Beneath the pale light of the moon, a shadowy figure approaches your camp. The wanderer’s voice is soft yet unnerving, as though his words " +
                "carry the weight of an ancient curse. He speaks directly to you, revealing his knowledge of the prisoners in your custody. \n\nHe offers a sinister bargain: " +
                "deliver twelve lives—whether soldiers, captives, or innocents—for a dark ritual that will grant you supernatural strength. His words are tempting, yet they carry " +
                "a foreboding chill. As he awaits your response, the weight of his offer settles heavily on your conscience.",
                
                //Event Description C
                "{=BloodToll_Event_Desc_3}A figure cloaked in darkness emerges from the forest and strides confidently toward your campfire. His gaze sweeps over your men and lingers " +
                "on you with unsettling intensity. In a voice that sends shivers down your spine, he speaks of power—strength that could make you unstoppable. But such power comes " +
                "with a cost.\n\nHe knows you have prisoners and soldiers under your command. He demands a dozen lives, their blood to fuel his wicked ritual. Innocents or warriors, " +
                "it matters not to him. The choice is yours, but the air grows colder, and the shadow of his presence seems to deepen as he waits.",
                
                //Event Description D
                "{=BloodToll_Event_Desc_4}Under the watchful gaze of a crimson moon, a mysterious wanderer steps into your camp, his presence unnervingly calm amid the flickering firelight. " +
                "He speaks with a chilling confidence, revealing his knowledge of the prisoners you hold. He offers a tempting proposition: deliver twelve lives to him, be they captives or " +
                "your own soldiers, to perform a ritual of dark power.\n\nThe strength he promises is tantalizing, a power that could make you feared across the land. But his demand " +
                "sends ripples of unease through your camp, the shadows seeming to press closer as he awaits your answer.",
                
                //Event Description E
                "{=BloodToll_Event_Desc_5}A cloaked figure appears at the edge of your camp as the firelight flickers and the night grows cold. His voice is low, yet it carries an " +
                "authority that silences your men. He knows of the prisoners in your custody, their fate now intertwined with the dark offer he places before you.\n\nThe sacrifice of " +
                "twelve lives, he claims—be they prisoners, soldiers, or innocents—will fuel a ritual to grant you strength beyond imagination. As the chill in the air deepens and " +
                "his eyes glint with unnatural light, you realize the gravity of his offer. What will you decide?"
            };
            
            private static readonly List<string> eventChoice1 = new List<string>
            {
                //Event Choice 1A
                "{=BloodToll_Event_Choice_1A}You give the order, and your guards reluctantly drag a dozen prisoners into the clearing. The captives scream and beg, their voices breaking as they plead for mercy. " +
                "The wanderer’s grin widens as he prepares the ritual, laying out dark runes in a circle and lighting black candles that flicker with unnatural flames. He begins to chant in a guttural language, " +
                "and the first prisoner is dragged to the center. Without hesitation, the wanderer plunges his dagger into the captive’s chest, spilling blood into the runes. The screams of the dying man echo in your ears " +
                "as the next victim is brought forward. One by one, their lives are extinguished, each death fueling a growing sense of dread in the air.\n\nWhen the ritual is complete, the prisoners lie lifeless in a grotesque " +
                "heap, their blood seeping into the earth. A dark mist rises, swirling around you, and you feel an otherworldly power coursing through your veins. Your body feels stronger, faster, but the chilling realization " +
                "of what you’ve done weighs heavy on your soul. The haunted faces of your men say everything they dare not speak.",

                //Event Choice 1B
                "{=BloodToll_Event_Choice_1B}The wanderer’s eyes gleam with malice as your guards force a dozen prisoners into the circle. The captives are bound, their terror palpable as they struggle against their restraints. " +
                "The wanderer begins his ritual, carving intricate symbols into each victim’s flesh with a dagger that gleams unnaturally under the moonlight. Each cut elicits a scream, the sound blending with the chanting to create a cacophony " +
                "of agony.\n\nAs the first throat is slit, a black mist begins to rise from the blood-soaked ground, and the temperature plummets. The ritual grows more frenzied with each death, the wanderer’s voice booming with unholy fervor. " +
                "By the time the final prisoner falls silent, the clearing is drenched in blood, and the air is heavy with an oppressive darkness. A surge of power envelops you, a strength that feels both exhilarating and unnatural. " +
                "But as you look at the lifeless bodies around you, their wide, unseeing eyes fixed on the heavens, you wonder if you’ve traded your humanity for this power.",

                //Event Choice 1C
                "{=BloodToll_Event_Choice_1C}The captives are dragged into the clearing, their cries for mercy falling on deaf ears. The wanderer’s voice is calm yet commanding as he instructs your men to place the prisoners in a circle. " +
                "He begins to chant, the words alien and menacing, sending chills down your spine. The first victim is chosen, their terrified screams piercing the cold night as the wanderer plunges a dagger into their heart. Blood gushes forth, " +
                "forming intricate patterns within the runes etched on the ground.\n\nOne by one, the prisoners are sacrificed, their lives feeding the ritual’s insatiable hunger. The final victim collapses, their life extinguished, " +
                "and the ritual reaches its climax. A dark energy surges through the air, surrounding you. Your body feels invigorated, stronger than ever before, but the weight of the sacrifice is undeniable. The prisoners’ lifeless bodies " +
                "lie sprawled around the clearing, a macabre reminder of the cost of your newfound power. Your men look at you with a mixture of fear and disgust, their loyalty tested by the horror they’ve just witnessed.",

                //Event Choice 1D
                "{=BloodToll_Event_Choice_1D}Twelve prisoners are dragged before the wanderer, their faces etched with despair. The ritual begins as the wanderer lights blackened candles and marks the ground with dark symbols. " +
                "Each prisoner is brought forward, and the wanderer uses a blade to carve symbols into their skin, chanting louder with each sacrifice. Their screams fill the night, mingling with the eerie hum of the ritual’s energy.\n\n" +
                "The air grows colder with every death, the ground beneath your feet trembling as the ritual nears completion. By the time the final life is taken, the clearing is engulfed in a swirling shadow. The wanderer turns to you, " +
                "a sinister smile on his face, and raises his arms. An overwhelming surge of power courses through your body, making you feel invincible. But as you look around at the lifeless bodies of the prisoners, their blood staining the earth, " +
                "you can’t shake the feeling that this power comes at a cost far greater than you imagined.",

                //Event Choice 1E
                "{=BloodToll_Event_Choice_1E}Your men drag twelve prisoners to the clearing, their protests and pleas ignored. The wanderer arranges them in a circle, binding them to wooden stakes driven into the ground. " +
                "He begins to chant in a language that seems to warp the very air around you. The first sacrifice is made, the prisoner’s blood spilling onto the ground and soaking into the runes etched in the dirt. " +
                "Each subsequent death fuels the ritual further, the wanderer’s voice growing louder and more frenzied with each passing moment.\n\nWhen the final sacrifice is complete, the air becomes thick with an oppressive darkness. " +
                "A pulse of energy surges through you, filling your body with an unnatural strength. The wanderer steps back, his work complete, and disappears into the shadows. You look down at your hands, feeling the raw power within you, " +
                "but the sight of the blood-soaked clearing and the mutilated bodies around you leaves you with an emptiness that no amount of strength can fill."
            };
            
            private static readonly List<string> eventChoice2 = new List<string>
            {
                //Event Choice 2A
                "{=BloodToll_Event_Choice_2A}You issue the command to your men, making it clear that children are not to be touched. With grim determination, they set out to the roads, " +
                "targeting unsuspecting travelers. Hours pass before they return, dragging a terrified group of captives—a mix of men and women from various walks of life. The captives beg " +
                "for their freedom, some sobbing uncontrollably, others silently defiant. \n\nThe wanderer observes with a sickening grin, his excitement growing as the prisoners are bound and " +
                "prepared for the ritual. The air fills with their cries and protests as you watch the scene unfold, a sense of unease creeping into your mind. The ritual proceeds, their blood " +
                "spilled in horrific fashion, staining the ground as the wanderer’s chants grow louder. By the end, your body surges with newfound power, but the cost weighs heavily in your mind " +
                "as you see the lifeless, bloodied bodies of those who dared to cross your path.",

                //Event Choice 2B
                "{=BloodToll_Event_Choice_2B}Your men hesitate at first when given the order to kidnap travelers, but your unwavering glare forces them into action. You emphasize one rule: no children. " +
                "As the hours stretch on, you wait in uneasy silence until your soldiers return, leading a group of captives bound in rope. The captives, a mix of men and women, look disheveled and terrified, " +
                "pleading for their lives and protesting their innocence.\n\nThe wanderer barely acknowledges their words as he begins his grim preparations. The captives are dragged to the center of a ritual " +
                "circle, their screams piercing the air as the wanderer carves ancient symbols into their flesh. One by one, their lives are taken, their blood pooling in grotesque patterns. The ritual concludes, " +
                "and a dark energy courses through you, filling you with an unnatural strength. But as you stand among the desecrated remains of innocents, you wonder if this power will ever be worth the weight " +
                "of their souls.",

                //Event Choice 2C
                "{=BloodToll_Event_Choice_2C}The command is given, and your men ride out under the cover of darkness, hunting for travelers along the desolate roads. You make it abundantly clear—children are " +
                "off-limits. Hours later, they return with a group of captives, bound and trembling. The prisoners, a mix of ages and appearances, plead for mercy as they are dragged into camp. " +
                "The wanderer cackles with glee, wasting no time in beginning the ritual.\n\nThe captives’ cries grow louder as they are forced into the center of the ritual circle. The wanderer’s dagger flashes, " +
                "and blood spills freely, each sacrifice more gruesome than the last. You feel the dark energy grow with each life taken, your body pulsing with a power that feels both exhilarating and revolting. " +
                "When the ritual ends, the clearing is silent save for the crackling of the fire and the whispers of your horrified men. You turn away, unwilling to look at the mangled remains of those sacrificed " +
                "for your ambition.",

                //Event Choice 2D
                "{=BloodToll_Event_Choice_2D}With a voice devoid of hesitation, you instruct your men to bring travelers from the roads, making it clear that children are to be spared. They return hours later, " +
                "escorting a frightened group of captives. The prisoners—men and women of various ages—are forced to their knees, their terrified faces illuminated by the campfire. Some beg for their lives, " +
                "others curse your name, but none of it sways your resolve.\n\nThe wanderer begins his ritual, chanting ominously as he prepares the captives. Their cries echo through the night as one by one, they are " +
                "sacrificed in a grotesque display of blood and pain. By the end, the ritual circle is drenched in crimson, and the air is thick with the scent of death. You feel an otherworldly power surge through you, " +
                "but the sight of the mangled bodies and the lingering whispers of the dying stay with you, haunting your every thought.",

                //Event Choice 2E
                "{=BloodToll_Event_Choice_2E}Your men ride out under your strict orders to spare any children, targeting travelers instead. When they return, they bring with them a group of captives, their wrists " +
                "bound and their faces pale with terror. The wanderer’s eyes gleam as he surveys the group, his anticipation palpable. The captives are herded to the center of the ritual circle, their protests and " +
                "pleas falling on deaf ears.\n\nThe ritual begins, the wanderer’s chants rising in intensity as he sacrifices the first victim. The air grows heavier with each life taken, the ground soaked in blood. " +
                "Your men look on in horror, some averting their eyes, but you remain steadfast. When the final victim falls silent, the power surges through you, intoxicating and overwhelming. As the wanderer " +
                "disappears into the shadows, you’re left standing amidst the blood-soaked remains, the weight of your decision pressing heavily on your mind."
            };
            
            private static readonly List<string> eventChoice3 = new List<string>
            {
                //Event Choice 3A
                "{=BloodToll_Event_Choice_3A}You nod as though agreeing to the wanderer’s demands, watching as he begins setting up his ritual circle. The air grows cold as he chants under his breath, his back turned to you. " +
                "With a single motion, you signal your men. They advance in silence, their weapons gleaming under the pale moonlight. The first strike hits the wanderer hard, and he stumbles, letting out an ear-piercing wail. " +
                "His voice echoes unnaturally, sending shivers down your spine, but the assault continues relentlessly. When he finally collapses, his body twitching in death throes, you can’t help but feel the weight of his lifeless gaze.\n\n" +
                "Your men search his belongings and uncover a small chest containing {goldToLoot} gold. Among the scattered items are disturbing artifacts—daggers etched with runes, darkened candles, and vials of thick, crimson liquid. " +
                "You order the gold taken and the rest burned. As the fire consumes his possessions, the night feels darker, heavier. The cries of the dying wanderer still echo faintly in your mind, as though his curse clings to the air.",

                //Event Choice 3B
                "{=BloodToll_Event_Choice_3B}You let the wanderer begin his ritual, his voice growing louder and more guttural as he weaves his spell. The cold light of the moon illuminates his form, casting long shadows over the ritual " +
                "circle. When his focus peaks, you give the signal. Your men charge with brutal efficiency, blades flashing in the moonlight. The wanderer turns, his face twisted with fury and disbelief. His screams are unnatural, more beast " +
                "than man, as your soldiers strike him down. Even in death, his body seems to radiate malice, his blood staining the earth an unnatural black.\n\nSearching his belongings, you find a locked chest hidden among his robes. Breaking it open, " +
                "you discover {goldToLoot} gold. Alongside the treasure, your men uncover relics that defy explanation—bone fragments carved with intricate patterns, ancient scrolls filled with illegible symbols, and a mirror that reflects " +
                "nothing but darkness. You take the gold and order the rest destroyed, but as the fire consumes his possessions, the shadows seem to move unnaturally, and a low hum fills the air. Whatever power he sought may not have died with him.",

                //Event Choice 3C
                "{=BloodToll_Event_Choice_3C}Feigning interest, you allow the wanderer to arrange his ritual tools, watching as he meticulously places candles and draws runes on the ground. His muttering grows louder, each word grating on your " +
                "nerves. When he kneels to light the candles, you step forward, plunging your blade deep into his back. His scream is a sound you’ll never forget—a blend of pain and fury that reverberates in your very bones. He collapses to the ground, " +
                "his blood spreading in unnatural patterns across the ritual circle.\n\nAs silence falls, your men search his belongings. They find {goldToLoot} gold stored in a heavily worn chest, but their unease is palpable. Among the ritual items " +
                "are grotesque relics: jars of preserved organs, scrolls that seem to glow faintly in the dark, and a dagger that hums faintly when touched. You take the gold and order the rest destroyed. The flames devour the items quickly, but a chill " +
                "lingers, as if the wanderer’s essence still watches from the shadows.",

                //Event Choice 3D
                "{=BloodToll_Event_Choice_3D}The wanderer begins chanting, his voice deep and resonant as he calls upon ancient powers. His focus is unbroken as you quietly draw your weapon and signal your men. When you strike, the wanderer spins " +
                "around, his eyes glowing faintly in the moonlight. He fights back with surprising ferocity, lashing out with unnatural strength. Your men hesitate for a moment, but your commands spur them into action, and together you overpower him. " +
                "His final scream is a curse, his blood splattering across the runes as he collapses.\n\nYou search his belongings and uncover a heavy chest containing {goldToLoot} gold. Among the items are strange artifacts: candles that burn with black flames, " +
                "bones carved into intricate shapes, and a vial of liquid that seems to move on its own. You pocket the gold and order the rest burned. As the flames consume his possessions, the air grows heavy, and you can’t shake the feeling that something " +
                "is watching you from the darkness.",

                //Event Choice 3E
                "{=BloodToll_Event_Choice_3E}You nod at the wanderer, feigning agreement as he begins preparing his ritual. His chanting fills the clearing, each word spoken in a language that feels wrong to hear. When the moment is right, " +
                "you strike. Your blade finds its mark, but the wanderer reacts with an unnatural speed, clawing at you with hands that seem to stretch and twist in impossible ways. Your men intervene, striking him down as he lets out a final, ear-splitting wail. " +
                "His body collapses, his blood forming unsettling shapes on the ground.\n\nYou search his belongings, finding a chest containing {goldToLoot} gold. The rest of his possessions are horrifying—human skulls etched with runes, a blood-stained altar cloth, " +
                "and a journal filled with incomprehensible scribbles. You burn everything but the gold, watching as the flames consume the items with an unnatural intensity. As the fire dies down, you feel a weight in the air, as though the wanderer’s presence " +
                "has not fully left this place."
            };
            
            private static readonly List<string> eventChoice4= new List<string>
            {
                //Event Choice 4A
                "{=BloodToll_Event_Choice_4A}You stand firm and reject the wanderer’s offer, your voice steady as you order your men to escort him out of the camp. The wanderer stares at you, his eyes narrowing with a chilling intensity. " +
                "As your soldiers lead him away, he mutters something under his breath—a string of guttural words that make your skin crawl. \n\nThe night feels colder after he’s gone, and an uneasy silence settles over your camp. " +
                "Your men seem on edge, whispering about the stranger’s parting words. As the hours pass, you notice a strange tension among your soldiers, as though something unseen lingers just beyond the firelight.",

                //Event Choice 4B
                "{=BloodToll_Event_Choice_4B}You wave off the wanderer’s offer, disgusted by his proposition. You command your men to escort him out, warning him never to return. The wanderer smirks as he’s led away, his voice low and dripping " +
                "with malice as he mutters, 'You will regret this.' His words echo in your mind long after he disappears into the shadows.\n\nThat night, the camp is restless. Your men report hearing strange whispers in the dark, and a few swear " +
                "they saw glowing eyes watching from the treeline. Though you try to dismiss their fears, a sense of unease creeps into your thoughts. You wonder if refusing the wanderer has brought something worse upon you.",

                //Event Choice 4C
                "{=BloodToll_Event_Choice_4C}With unwavering resolve, you tell the wanderer his offer is rejected. His expression darkens, and he fixes you with a glare that feels like it pierces your very soul. You instruct your men to escort him " +
                "far from the camp, ensuring he won’t return. As he’s led away, he whispers something to himself, the words barely audible but leaving an unsettling resonance in the air.\n\nLater that night, a thick fog rolls into the camp, " +
                "silencing the usual sounds of the wilderness. Some of your men complain of headaches and chills, and the horses seem restless. Though the wanderer is gone, it feels as if his shadow still lingers.",

                //Event Choice 4D
                "{=BloodToll_Event_Choice_4D}You reject the wanderer’s twisted offer, your voice carrying a tone of finality. 'Leave, and never return,' you command. The wanderer chuckles softly, his gaze filled with an unnerving mixture of amusement " +
                "and disdain. Your men drag him from the camp, but his parting words hang heavy in the air: 'You’ve sealed your fate.' \n\nThat night, an oppressive silence blankets the camp. The crackling fire seems dimmer, and the shadows stretch " +
                "unnaturally. Your men avoid meeting your gaze, their unease palpable. Though the wanderer is gone, his presence feels etched into the very fabric of the night.",

                //Event Choice 4E
                "{=BloodToll_Event_Choice_4E}Disgusted by the wanderer’s offer, you refuse outright and demand his immediate removal from your camp. He doesn’t resist as your men seize him, but his parting words chill you to the core: 'You’ll wish you’d " +
                "taken my deal.' The words carry an unnatural weight, lingering long after he disappears into the forest.\n\nAs the hours pass, strange occurrences begin to unsettle your camp. Equipment goes missing, and faint whispers can be heard " +
                "just beyond the firelight. Some of your men refuse to sleep, convinced that something is watching them. Though you stand by your decision, doubt begins to creep in, and you wonder if this refusal will come back to haunt you."

            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=BloodToll_Event_Msg_1A}{heroName} sacrificed prisoners to the wanderer, gaining dark power and unexpected skill XP.",
                "{=BloodToll_Event_Msg_1B}{heroName} handed over prisoners for the ritual, their screams echoing into the night as {heroName} gained skill XP.",
                "{=BloodToll_Event_Msg_1C}{heroName} gave prisoners to the wanderer, exchanging their lives for unholy strength and skill XP in {randomSkill}.",
                "{=BloodToll_Event_Msg_1D}The prisoners' lives were taken under {heroName}'s orders, fueling the sinister ritual and earning {heroName} skill XP.",
                "{=BloodToll_Event_Msg_1E}{heroName}'s pact with the wanderer left a trail of blood and fear behind, but also granted skill XP."
            };
            
            private static readonly List<string> eventMsg2 = new List<string>
            {
                "{=BloodToll_Event_Msg_2A}{heroName}'s men kidnapped travelers to fulfill the wanderer's dark ritual, granting {heroName} skill XP.",
                "{=BloodToll_Event_Msg_2B}Under {heroName}'s command, innocent travelers were taken and sacrificed for power, rewarding {heroName} with skill XP.",
                "{=BloodToll_Event_Msg_2C}{heroName} ordered the capture of travelers, their lives fueling a sinister ritual and gaining skill XP.",
                "{=BloodToll_Event_Msg_2D}The wanderer's demands were met as {heroName} abducted travelers for the sacrifice, earning skill XP.",
                "{=BloodToll_Event_Msg_2E}{heroName}'s quest for power came at the cost of innocent lives stolen from the roads but brought skill XP."
            };

            
            private static readonly List<string> eventMsg3 = new List<string>
            {
                "{=BloodToll_Event_Msg_3A}{heroName} betrayed and killed the wanderer, seizing his gold and ending the ritual.",
                "{=BloodToll_Event_Msg_3B}The wanderer met his end under {heroName}'s blade, his dark plans thwarted.",
                "{=BloodToll_Event_Msg_3C}{heroName} executed the wanderer and looted {goldToLoot} gold from his belongings.",
                "{=BloodToll_Event_Msg_3D}Betraying the wanderer, {heroName} struck him down and claimed his treasures.",
                "{=BloodToll_Event_Msg_3E}{heroName} ended the wanderer's life, putting a stop to his sinister ambitions."
            };
            
            private static readonly List<string> eventMsg4 = new List<string>
            {
                "{=BloodToll_Event_Msg_4A}{heroName} refused the wanderer's offer and banished him from the camp.",
                "{=BloodToll_Event_Msg_4B}{heroName} rejected the dark pact, ordering the wanderer to leave immediately.",
                "{=BloodToll_Event_Msg_4C}{heroName} dismissed the wanderer, standing firm against his sinister demands.",
                "{=BloodToll_Event_Msg_4D}The wanderer was banished by {heroName}, his offer of power left unanswered.",
                "{=BloodToll_Event_Msg_4E}{heroName} refused the ritual, sending the wanderer away and avoiding the blood price."
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


    public class BloodTollData : RandomEventData
    {
        public BloodTollData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new BloodToll();
        }
    }
}