using System;
using System.Collections.Generic;
using System.Windows;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.RandomEvents.Events.CCEvents
{
    public class LightsInTheSkies : BaseEvent
    {
        private readonly bool eventDisabled;

        public LightsInTheSkies() : base(ModSettings.RandomEvents.LightsInTheSkiesData)
        {
            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            
            eventDisabled = ConfigFile.ReadBoolean("LightsInTheSkies", "EventDisabled");
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
            return HasValidEventData() && GeneralSettings.SciFiEvents.IsDisabled() == false && CurrentTimeOfDay.IsNight;
        }

        public override void StartEvent()
        {
            var eventTitle = new TextObject(EventTextHandler.GetRandomEventTitle()).ToString();

            var closestSettlement = ClosestSettlements.GetClosestAny(MobileParty.MainParty).ToString();

        /*
         * Follow these steps to decode the text:
         * 1. Visit https://cryptii.com/pipes/caesar-cipher. Choose 'Decode' and select the 'Nihilist cipher'.
         * 2. Input "elana" as the key and 0123456789 as the alphabet.
         * 3. Copy the decoded output and go to https://crypt-online.ru/en/crypts/text2hex/.
         * 4. Choose the 'XX UTF-8' option and decode.
         * 5. Next, visit https://www.devglan.com/online-tools/text-encryption-decryption.
         * 6. Select 'decrypt' and use "bannerlord" as the Secret Key.
         * 7. The resulting string is the correctly decoded text.
         * 8. Search the string on Google to find its reference.
         */

            const string unknownText = "46 62 54 75 42 46 66 45 64 42 57 66 42 64 46 50 62 42 68 66 46 62 46 84 42 46 " +
                                       "66 52 64 42 50 82 42 64 44 76 62 42 68 44 46 62 54 77 42 46 65 52 64 42 50 74 " +
                                       "42 64 46 67 62 42 74 42 46 62 46 85 42 46 74 54 64 42 56 74 42 64 53 70 62 42 " +
                                       "76 52 46 62 53 86 42 46 65 52 64 42 58 73 42 64 45 46 62 42 68 56 46 62 52 74 " +
                                       "42 46 65 44 64 42 49 72 42 64 46 68 62 42 75 45 46 62 53 76 42 46 65 44 64 42 " +
                                       "57 84 42 64 52 56 62 42 68 45 46 62 46 74 42 46 74 44 64 42 58 74 42 64 46 67 " +
                                       "62 42 67 53 46 62 46 78 42 46 65 43 64 42 49 74 42 64 53 48 62 42 68 62 46 62 " +
                                       "46 85 42 46 65 54 64 42 58 75 42 64 46 59 62 42 76 53 46 62 46 84 42 46 65 54 " +
                                       "64 42 56 64 42 64 46 66 62 42 75 46 46 62 46 76 42 46 72 53 64 42 50 84 42 64 " +
                                       "53 69 62 42 74 55 46 62 52 76 42 46 73 53 64 42 58 66 42 64 53 67";

            var eventText1 =new TextObject(EventTextHandler.GetRandomEventPart1())
                .SetTextVariable("closestSettlement", closestSettlement)
                .ToString();
            
            var eventText2 =new TextObject(EventTextHandler.GetRandomEventPart2())
                .ToString();
            
            
            var eventText3 =new TextObject(EventTextHandler.GetRandomEventPart3())
                .ToString();
            
            var eventText4 =new TextObject(EventTextHandler.GetRandomEventPart4())
                .ToString();
            
            var eventTextUnknown =new TextObject(EventTextHandler.GetRandomUnknownPart())
                .SetTextVariable("unknownText", unknownText)
                .ToString();
            
            var eventText5 =new TextObject(EventTextHandler.GetRandomEventPart5())
                .ToString();
            
            var eventMsg1 =new TextObject(EventTextHandler.GetRandomEventMessage1())
                .SetTextVariable("heroName", Hero.MainHero.FirstName.ToString())
                .SetTextVariable("closestSettlement",closestSettlement)
                .ToString();
            
            var eventButtonText1 = new TextObject("{=LightsInTheSkies_Event_Button_Text_1}Read More").ToString();
            var eventButtonText2 = new TextObject("{=LightsInTheSkies_Event_Button_Text_2}Done").ToString();
            var eventButtonText3 = new TextObject("{=LightsInTheSkies_Event_Button_Text_3}What?").ToString();
            
            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventText1, true, false, eventButtonText1, null, null, null), true);
            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventText2, true, false, eventButtonText1, null, null, null), true);
            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventText3, true, false, eventButtonText1, null, null, null), true);
            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventText4, true, false, eventButtonText2, null, null, null), true);
            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventTextUnknown, true, false, eventButtonText3, null, null, null), true);
            InformationManager.ShowInquiry(new InquiryData(eventTitle, eventText5, true, false, eventButtonText2, null, null, null), true);

            InformationManager.DisplayMessage(new InformationMessage(eventMsg1, RandomEventsSubmodule.Msg_Color_MED_Outcome));

            MobileParty.MainParty.SetDisorganized(true);

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
                "{=LightsInTheSkies_Title_A}Lights In The Skies",
                "{=LightsInTheSkies_Title_B}Mysterious Lights Above",
                "{=LightsInTheSkies_Title_C}The Sky Illuminated",
                "{=LightsInTheSkies_Title_D}Shadows of the Unknown",
                "{=LightsInTheSkies_Title_E}Dancing Lights in the Night",
                "{=LightsInTheSkies_Title_F}Signals from Beyond",
                "{=LightsInTheSkies_Title_G}The Sky's Cryptic Message",
                "{=LightsInTheSkies_Title_H}Eerie Glows in the Darkness",
                "{=LightsInTheSkies_Title_I}Anomalies in the Sky",
                "{=LightsInTheSkies_Title_J}The Night of Unseen Forces"
            };
            
            
            private static readonly List<string> EventPart1 = new List<string>
            {
                //Event Part 1A
                "{=LightsInTheSkies_Event_Text_1A}Late one night, you are taking a walk in the woods near " +
                "{closestSettlement} to clear your head. You manage to find a nice clearing not too far into the " +
                "woods where you sit down and lean against a big boulder to relax. As you sit there, you notice " +
                "a light approaching in the sky from the south. You stand up and look at the light that " +
                "now has come even closer. It's glowing and keeps changing colors, but it hovers in the sky without " +
                "a sound. After you've stared at the light for a few minutes, it disappears over the treeline. " +
                "Having no clue to what you witnessed, you make your way back but you are somewhat shocked when you " +
                "see the sun is rising in the east, as by your calculations, it's just after midnight. You make haste " +
                "back to the main camp.\nWhen you arrive, some of your high-ranking men run towards you and ask you " +
                "where you have been. Perplexed, you answer them and tell them about the strange light. You are " +
                "shocked when the men tell you that you have been gone for over 5 hours.\nA few hours later your " +
                "party is on the move again, but the men can clearly notice that you are preoccupied with last " +
                "night's events.",
                
                //Event Part 1B
                "{=LightsInTheSkies_Event_Text_1B}One evening, you decide to clear your head with a walk near " +
                "{closestSettlement}. As you venture into the woods, you come across a peaceful clearing and rest " +
                "against a large rock. While relaxing, you notice a light moving across the southern sky. Curious, " +
                "you stand and watch as the light draws closer, glowing and shifting colors. It moves silently " +
                "above the trees before vanishing into the distance. Time feels strange, and as you return to the " +
                "main camp, you realize the sun is rising despite it feeling like midnight. Upon your arrival, " +
                "your high-ranking men inform you that you've been missing for over 5 hours. The next day, you " +
                "remain deeply unsettled, lost in thought over the night's events.",
                
                //Event Part 1C
                "{=LightsInTheSkies_Event_Text_1C}As the night grows late, you take a stroll near {closestSettlement} " +
                "to gather your thoughts. A short walk into the woods leads you to a serene clearing. Leaning back " +
                "against a boulder, you spot a peculiar light in the southern sky. It approaches slowly, pulsating " +
                "and shifting hues without making a sound. The light hovers, then abruptly vanishes beyond the trees. " +
                "Confused, you head back but are startled to see dawn breaking. When you return to camp, your men " +
                "ask where you’ve been, claiming you’ve been gone for hours. Their words only deepen the mystery of " +
                "the light you saw. The next day, the incident weighs heavily on your mind.",
                
                //Event Part 1D
                "{=LightsInTheSkies_Event_Text_1D}Taking a late-night walk to clear your thoughts, you wander near " +
                "{closestSettlement} and find a clearing among the trees. While resting, you notice a distant light " +
                "in the southern sky. It draws closer, glowing and shifting colors eerily in complete silence. You " +
                "watch, entranced, as it moves over the treetops before disappearing. As you walk back, the rising " +
                "sun catches you off guard—it feels far too early. Reaching the camp, your men confront you, " +
                "claiming you've been missing for hours. Their revelation leaves you shaken, unable to comprehend " +
                "what you experienced.",
                
                //Event Part 1E
                "{=LightsInTheSkies_Event_Text_1E}While walking through the woods near {closestSettlement} to clear " +
                "your mind, you find a quiet clearing and sit against a large rock. A glowing light appears in the " +
                "sky to the south, drawing closer. It pulses with shifting colors and moves silently above the " +
                "trees before vanishing. Returning to camp, you are stunned to see the sun rising, as it feels like " +
                "you were gone only moments. Your men approach, relieved to see you, and tell you that hours have " +
                "passed. You can't shake the feeling of unease as you reflect on the strange encounter."

            };
            
            private static readonly List<string> EventPart2 = new List<string>
            {
                //Event Part 2A
                "{=LightsInTheSkies_Event_Text_2A}When you finally decide to make camp for the night you go to wash " +
                "yourself in your tent. While washing your face, you notice that you are bleeding from the nose. You " +
                "attempt to blow your nose, only to find a small metallic-looking object come out. " +
                "You pick it up and study it for a few seconds before it crumbles to ash before your eyes.\n\nYou " +
                "also notice several small, fresh scars on your extremities. They are identical and perfectly rounded, " +
                "showing no signs of bleeding. To your horror, you discover that you are missing two molars, and the nails " +
                "on your right foot are gone. This realization leaves you trembling with fear.\n\nAs night falls, drifting " +
                "to sleep alongside the last rays of daylight, you suddenly awaken drenched in sweat, heart pounding, " +
                "and discover that you have wet the bed. Shaken, you reach for your diary to write down fragments of a " +
                "dream that now seems all too vivid.",
                
                //Event Part 2B
                "{=LightsInTheSkies_Event_Text_2B}As you settle into camp for the night, you wash your face in your tent. " +
                "Glancing into the basin, you notice blood dripping from your nose. Blowing your nose, a metallic object " +
                "emerges, which disintegrates into ash when you pick it up. Confused and frightened, you inspect your " +
                "body and discover fresh scars on your limbs—perfectly round, with no sign of bleeding. " +
                "The terror deepens when you realize two of your molars are missing, along with the nails on your " +
                "right foot.\n\nFalling asleep hours later, you are jolted awake, drenched in sweat and trembling. Your " +
                "diary offers a faint solace as you try to record the fleeting remnants of a disturbing dream.",
                
                //Event Part 2C
                "{=LightsInTheSkies_Event_Text_2C}While preparing to rest in your tent, you notice a faint trickle of blood " +
                "from your nose. As you blow your nose, a strange metallic object dislodges and crumbles to dust in your hand. " +
                "Inspecting yourself, you find small, circular scars on your arms and legs—silent, bloodless, and uniform. " +
                "Panic sets in when you discover missing molars and toenails.\n\nAs you finally drift to sleep, exhaustion " +
                "overtaking fear, you awaken hours later in a cold sweat, heart pounding. The fragments of a dream cling to " +
                "your mind, compelling you to record them in your diary before they fade completely.",
                
                //Event Part 2D
                "{=LightsInTheSkies_Event_Text_2D}After setting up camp, you retreat to your tent to clean up. As you " +
                "wash your face, a nosebleed starts. When you blow your nose, a metallic object emerges and crumbles into " +
                "dust. Fear escalates as you examine your limbs and discover identical round scars. The realization that " +
                "your molars and toenails are missing strikes like a lightning bolt, leaving you shaken.\n\nThe night’s " +
                "darkness deepens, and exhaustion eventually takes you to sleep. But you awaken drenched in sweat, your " +
                "heart racing, with faint memories of a dream that you hastily jot down in your diary.",
                
                //Event Part 2E
                "{=LightsInTheSkies_Event_Text_2E}As the camp settles for the night, you wash your face in the privacy " +
                "of your tent. Noticing blood from your nose, you blow it and watch in horror as a metallic object falls " +
                "out, disintegrating before you can examine it. Checking your body, you find strange, bloodless scars " +
                "and horrifyingly discover missing teeth and toenails.\n\nThough fear consumes you, sleep eventually claims " +
                "you. Hours later, you wake drenched in sweat, trembling as flashes of a vivid dream haunt you. Desperate to " +
                "preserve what little you recall, you reach for your diary."
            };
            
            private static readonly List<string> EventPart3 = new List<string>
            {
                //Event Part 3A
                "{=LightsInTheSkies_Event_Text_3A}“I am in the clearing at night, staring at the strange light, when " +
                "suddenly an intense beam engulfs me, lifting me off the ground and pulling me toward the light. " +
                "As I get closer, I realize the light is emanating from a silvery metallic object with an opening " +
                "that draws me in. Overcome by terror, I lose consciousness.”\n\n“When I awaken, I am strapped to a " +
                "cold metallic table, surrounded by three strange beings. They resemble humans in size and form, " +
                "but their faces are covered in dense hair, while their ears and noses are oddly hairless.”",
                
                //Event Part 3B
                "{=LightsInTheSkies_Event_Text_3B}“The clearing is bathed in the strange light, and before I can react, " +
                "a beam of energy engulfs me, lifting me from the ground. I am pulled toward the source of the light, " +
                "which reveals itself to be a silvery metallic object with an open entrance. Fear consumes me as I " +
                "approach, and I pass out.”\n\n“When I regain consciousness, I am lying on a freezing metallic table. " +
                "Three strange creatures stand nearby. They appear human-like but smaller, their faces obscured by " +
                "thick hair except for their smooth, hairless ears and noses.”",
                
                //Event Part 3C
                "{=LightsInTheSkies_Event_Text_3C}“In the clearing, the light intensifies and suddenly, a beam strikes " +
                "me, lifting me effortlessly toward the sky. As I ascend, I see that the light comes from a silvery " +
                "metal object with an opening drawing me in. Horror overtakes me, and I black out.”\n\n“When I come to, " +
                "I am strapped to a cold, metallic table. Surrounding me are three beings, vaguely human-like but " +
                "smaller. Their faces are covered in thick hair, yet their ears and noses are smooth and hairless, " +
                "making them look even stranger.”",
                
                //Event Part 3D
                "{=LightsInTheSkies_Event_Text_3D}“The strange light envelops me, and I am struck by a powerful beam " +
                "that lifts me toward the source. As I draw closer, I see it is a silvery metallic object with an " +
                "open portal. Fear takes hold as I am pulled closer, and everything fades to black.”\n\n“When I awaken, " +
                "I find myself restrained on a cold metal table. Surrounding me are three odd creatures, humanoid " +
                "in form but smaller, their faces hidden under thick hair, except for their oddly bare ears and noses.”",
                
                //Event Part 3E
                "{=LightsInTheSkies_Event_Text_3E}“In the clearing, I am transfixed by the strange light, when without " +
                "warning, a powerful beam hits me and lifts me off the ground. The source of the light reveals itself " +
                "to be a metallic object, shining silver with an open entrance that draws me in. The sheer terror is " +
                "too much, and I lose consciousness.”\n\n“When I open my eyes, I am lying on a frigid metallic table. " +
                "Three figures stand around me, humanoid but smaller, their faces shrouded in thick hair. Strangely, " +
                "their ears and noses are smooth and bare, adding to their uncanny appearance.”"
            };
            
            private static readonly List<string> EventPart4= new List<string>
            {
                //Event Part 4A
                "{=LightsInTheSkies_Event_Text_4A}“I lie there, paralyzed, as the creatures prod me with strange instruments " +
                "and scribble notes, speaking in an incomprehensible language. After a while, they place a helmet over my head, " +
                "and suddenly, a sequence of images and symbols is burned into my mind. The helmet is removed, and moments later, " +
                "a mask is placed over my mouth and nose. Darkness consumes me as I pass out once again.”\n\n" +
                "“When I regain consciousness, I find myself standing in the clearing, watching the light vanish beyond the treeline.”",
                
                //Event Part 4B
                "{=LightsInTheSkies_Event_Text_4B}“Unable to move, I feel the creatures poke and prod me with instruments " +
                "while they exchange words in an alien language. At some point, they place a helmet over my head, " +
                "and my mind is flooded with a series of burning images and patterns. After removing the helmet, they " +
                "cover my face with a mask. My vision fades, and I black out again.”\n\n" +
                "“When I awaken, I am back in the clearing, staring at the strange light as it disappears into the night.”",
                
                //Event Part 4C
                "{=LightsInTheSkies_Event_Text_4C}“I am frozen in place as the beings prod me with various tools and appear " +
                "to communicate in a language I cannot understand. A helmet is placed on my head, and a vivid, searing sequence " +
                "is etched into my mind. Once the helmet is removed, they place a mask over my face, and the world fades to black.”\n\n" +
                "“When I come to, I am standing in the clearing, watching the light retreat beyond the trees.”",
                
                //Event Part 4D
                "{=LightsInTheSkies_Event_Text_4D}“Paralyzed, I feel the cold instruments of the creatures pressing against my " +
                "skin as they make notes and chatter in an alien tongue. A helmet is placed over my head, and a sequence of " +
                "images burns itself into my consciousness. The helmet is removed, and I am given a mask that makes me lose " +
                "consciousness once more.”\n\n" +
                "“When I wake, I am back in the clearing, the light fading into the treetops.”",
                
                //Event Part 4E
                "{=LightsInTheSkies_Event_Text_4E}“I remain motionless as the creatures poke and examine me with their strange " +
                "devices, speaking in a language I cannot decipher. Then they place a helmet on me, and my mind is flooded " +
                "with burning symbols and images. Afterward, they place a mask over my face, and I lose consciousness once again.”\n\n" +
                "“When I open my eyes, I am standing in the clearing, the mysterious light disappearing over the horizon.”"
            };
            
            private static readonly List<string> EventUnknownPart = new List<string>
            {
                //Event Text Unknown A
                "{=LightsInTheSkies_Event_Text_Unknown_A}You turn the page and write down the sequence now burned into " +
                "your mind.\n\n{unknownText}\n\n Some of the symbols are unlike anything you've ever encountered. You " +
                "sit back, wondering what their purpose could be.",
                
                //Event Text Unknown B
                "{=LightsInTheSkies_Event_Text_Unknown_B}Flipping open your journal, you hastily jot down the sequence " +
                "etched into your memory.\n\n{unknownText}\n\n The symbols seem alien, beyond any language or code " +
                "you recognize. You are left pondering their meaning.",
                
                //Event Text Unknown C
                "{=LightsInTheSkies_Event_Text_Unknown_C}You quickly grab your journal and transcribe the strange sequence " +
                "from your mind.\n\n{unknownText}\n\n The unfamiliar symbols leave you unsettled as you wonder " +
                "what message they might hold.",
                
                //Event Text Unknown D
                "{=LightsInTheSkies_Event_Text_Unknown_D}Your hands shake as you write the mysterious sequence into " +
                "your journal.\n\n{unknownText}\n\n The symbols are foreign, and their meaning eludes you, " +
                "but you feel compelled to understand them.",
                
                //Event Text Unknown E
                "{=LightsInTheSkies_Event_Text_Unknown_E}You carefully record the sequence burned into your memory.\n\n{unknownText}\n\n " +
                "The symbols are unlike anything you've seen before, and their presence leaves you deeply unsettled. " +
                "You can't help but wonder what it all signifies."
            };
            
            private static readonly List<string> EventPart5= new List<string>
            {
                //Event Text 5A
                "{=LightsInTheSkies_Event_Text_5A}As you finish writing and stare at the gibberish you have just written, " +
                "you realize your hands are trembling uncontrollably. Seeking fresh air, you leave your tent. A few of your " +
                "men notice your distress, but you reassure them that everything is fine. You make your way to the lookout " +
                "point and gaze over the quiet land, struggling to piece together the events of the night. Minutes pass " +
                "as you stand there, lost in thought.\n\nJust as you turn to leave, you catch a glimpse of the same strange " +
                "light streaking across the distant sky one final time.",
                
                //Event Text 5B
                "{=LightsInTheSkies_Event_Text_5B}Shaken, you finish writing the cryptic symbols and step out of your tent " +
                "for fresh air. A few of your men glance at you, concern evident in their faces, but you wave them off, " +
                "insisting that you are fine. You head to the camp's lookout and stand there, staring out at the landscape, " +
                "trying to make sense of what happened. Time seems to blur as you stand motionless.\n\nAs you finally decide " +
                "to leave, a streak of the strange light flashes across the sky once more, sending chills down your spine.",
                
                //Event Text 5C
                "{=LightsInTheSkies_Event_Text_5C}You finish writing the strange sequence, unable to stop your hands from " +
                "shaking. Needing to clear your head, you leave the tent. Some of your men notice and approach, but you " +
                "assure them everything is fine. You climb to the camp's lookout point, scanning the dark expanse before you " +
                "as you try to comprehend the night’s events. Minutes pass as your thoughts swirl.\n\nAs you turn to leave, " +
                "you see a faint streak of the same strange light in the far-off sky, leaving you even more unsettled.",
                
                //Event Text 5D
                "{=LightsInTheSkies_Event_Text_5D}The symbols now on the page do little to ease your trembling hands. " +
                "You step outside to gather yourself, brushing off concerned glances from your men with reassurances. " +
                "You make your way to the lookout, gazing at the vast expanse of land, trying to quiet your mind and make " +
                "sense of the inexplicable. Minutes pass in silence.\n\nAs you prepare to leave, the sight of the strange " +
                "light streaking across the distant sky one last time freezes you in place.",
                
                //Event Text 5E
                "{=LightsInTheSkies_Event_Text_5E}The strange symbols you’ve written stare back at you, and the tremor in " +
                "your hands refuses to subside. Seeking solace, you step outside, brushing off the concerns of a few awake " +
                "men with reassurances. You climb to the lookout point, staring out into the night as your thoughts race. " +
                "The cool air does little to calm you as the minutes tick by.\n\nAs you turn to leave, you catch a final " +
                "glimpse of the mysterious light darting across the horizon, leaving you more unsettled than before."
            };
            
            private static readonly List<string> eventMsg1 = new List<string>
            {
                "{=LightsInTheSkies_Event_Msg_1}{heroName} had a strange experience one night near {closestSettlement}.",
                "{=LightsInTheSkies_Event_Msg_2}Rumors spread that {heroName} witnessed something unusual near {closestSettlement}.",
                "{=LightsInTheSkies_Event_Msg_3}One night, {heroName} encountered something unexplainable near {closestSettlement}.",
                "{=LightsInTheSkies_Event_Msg_4}{heroName} returned from the outskirts of {closestSettlement} visibly shaken after a bizarre incident.",
                "{=LightsInTheSkies_Event_Msg_5}Something extraordinary happened to {heroName} near {closestSettlement}, though details remain unclear."
            };

            
            public static string GetRandomEventTitle()
            {
                var index = random.Next(eventTitles.Count);
                return eventTitles[index];
            }
            
            public static string GetRandomEventPart1()
            {
                var index = random.Next(EventPart1.Count);
                return EventPart1[index];
            }
            
            public static string GetRandomEventPart2()
            {
                var index = random.Next(EventPart2.Count);
                return EventPart2[index];
            }
            
            public static string GetRandomEventPart3()
            {
                var index = random.Next(EventPart3.Count);
                return EventPart3[index];
            }
            
            public static string GetRandomEventPart4()
            {
                var index = random.Next(EventPart4.Count);
                return EventPart4[index];
            }
            
            public static string GetRandomUnknownPart()
            {
                var index = random.Next(EventUnknownPart.Count);
                return EventUnknownPart[index];
            }
            
            public static string GetRandomEventPart5()
            {
                var index = random.Next(EventPart5.Count);
                return EventPart5[index];
            }
            
            public static string GetRandomEventMessage1()
            {
                var index = random.Next(eventMsg1.Count);
                return eventMsg1[index];
            }
        }
    }


    public class LightsInTheSkiesData : RandomEventData
    {
        public LightsInTheSkiesData(string eventType, float chanceWeight) : base(eventType, chanceWeight)
        {
        }

        public override BaseEvent GetBaseEvent()
        {
            return new LightsInTheSkies();
        }
    }
}