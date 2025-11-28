using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.RandomEvents.Events;
using Bannerlord.RandomEvents.Helpers;
using Bannerlord.RandomEvents.Settings;
using Ini.Net;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Bannerlord.RandomEvents
{
    public class RandomEventBehavior : CampaignBehaviorBase
    {
        private readonly Queue<string> eventHistory = new Queue<string>();

        /// <summary>
        ///     Selects the next event to be executed from the event queue.
        ///     If the queue is empty, it first calls PopulateEventQueue to refill it.
        ///     The method dequeues the next event type from the queue and retrieves the corresponding BaseEvent.
        ///     This ensures a consistent flow of events and maintains the queue with a predefined number of events.
        /// </summary>
        private readonly Queue<string> eventQueue = new Queue<string>();

        private readonly RandomEventGenerator randomEventGenerator;

        private BaseEvent currentEvent;


        private uint inGameHoursPassed;

        private DateTime lastEventTime = DateTime.Now;
        private int minutesForNextEvent;

        public RandomEventBehavior()
        {
            Instance = this;

            randomEventGenerator = new RandomEventGenerator();
            PopulateRandomEventGenerator();
        }

        private static RandomEventBehavior Instance { get; set; }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, ProcessRandomEvent);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("run", "randomevent")]
        public static string RunRandomEvent(List<string> args)
        {
            if (args.Count < 1) return "You must provide the type of event to run";

            var ConfigFile = new IniFile(ParseIniFile.GetTheConfigFile());
            var eventDisabled = ConfigFile.ReadBoolean(args[0], "EventDisabled");

            if (eventDisabled) return $"Event {args[0]} is disabled and cannot be run.";

            if (Instance.currentEvent != null)
                return
                    $"Currently running event: {Instance.currentEvent.randomEventData.eventType}. To start another first cancel this one.";

            var evnt = Instance.randomEventGenerator.GetEvent(args[0])?.GetBaseEvent();

            if (evnt == null) return $"Unable to start event {args[0]}, did you spell it correctly?";

            Instance.ExecuteRandomEvent(evnt);
            return $"Starting {args[0]}";
        }


        [CommandLineFunctionality.CommandLineArgumentFunction("next", "randomevent")]
        public static string RunNextEvent(List<string> args)
        {
            if (Instance.currentEvent != null)
                return
                    $"Currently running event: {Instance.currentEvent.randomEventData.eventType}. To start another first cancel this one.";

            // Select which event should be played
            var eventToPlay = Instance.SelectEvent();

            // Start the random event
            Instance.ExecuteRandomEvent(eventToPlay);

            return $"Starting {eventToPlay.randomEventData.eventType}";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("cancelevent", "randomevent")]
        public static string CancelEvent(List<string> args)
        {
            return Instance.CancelEvent() ? "Current random event canceled!" : "No random event running.";
        }


        private void OnSessionLaunched(CampaignGameStarter cgs)
        {
            ResetEventTimer();
            PopulateEventQueue();
        }

        /// <summary>
        ///     Evaluates whether a random event should be executed at the current moment.
        ///     It checks if there is already an ongoing event and increments the in-game hours passed.
        ///     If the in-game hours passed exceed the minimum threshold and the real-time interval since the last event is met,
        ///     it proceeds to select and execute the next event from the event queue.
        ///     This method ensures a controlled and timely execution of random events in the game.
        /// </summary>
        private void ProcessRandomEvent()
        {
            if (currentEvent != null) return;

            inGameHoursPassed++;

            if (inGameHoursPassed < GeneralSettings.Basic.GetMinimumInGameHours() ||
                (DateTime.Now - lastEventTime).Minutes < minutesForNextEvent) return;
            // Select which event should be played
            var eventToPlay = SelectEvent();

            // Start the random event
            ExecuteRandomEvent(eventToPlay);
        }

        private BaseEvent SelectEvent()
        {
            if (eventQueue.Count == 0) PopulateEventQueue();

            var nextEventType = eventQueue.Dequeue();
            return randomEventGenerator.GetEvent(nextEventType).GetBaseEvent();
        }

        /// <summary>
        ///     Populates the event queue with a set number of unique events.
        ///     This method ensures that the event queue is always filled with events ready to be processed.
        ///     It avoids adding duplicate events to maintain event variety and prevent immediate repetition.
        /// </summary>
        private void PopulateEventQueue()
        {
            var configFile = new IniFile(ParseIniFile.GetTheConfigFile());
            var maxQueueDisplayCount = configFile.ReadInteger("GeneralSettings", "MaxQueueDisplayCount");

            while (eventQueue.Count < maxQueueDisplayCount)
            {
                var eventToQueue = randomEventGenerator.GetRandom().GetBaseEvent();
                if (eventToQueue != null && !eventQueue.Contains(eventToQueue.randomEventData.eventType))
                    eventQueue.Enqueue(eventToQueue.randomEventData.eventType);
            }
        }


        /// <summary>
        ///     Executes the specified random event, provided certain conditions are met.
        ///     It checks if the main hero is not a prisoner and if the main party is active.
        ///     If the event passes its specific execution criteria (CanExecuteEvent), the method proceeds to start the event.
        ///     This method serves as the trigger for initiating random events in the game, managing the current event state,
        ///     and hooking into the event completion handling.
        /// </summary>
        private void ExecuteRandomEvent(BaseEvent aEvent)
        {
            if (Hero.MainHero.IsPrisoner || !MobileParty.MainParty.IsActive) return;

            if (!aEvent.CanExecuteEvent()) return;

            // Stop the processing of random events
            currentEvent = aEvent;
            aEvent.onEventCompleted += EventEnded;

            aEvent.StartEvent();
        }

        private bool CancelEvent()
        {
            if (currentEvent == null) return false;
            currentEvent.CancelEvent();
            EventEnded();
            return true;
        }

        /// <summary>
        ///     Handles the completion of a random event.
        ///     It reads the maximum history count from the INI file and adds the current event to the event history queue.
        ///     If the history queue exceeds the maximum count, it dequeues the oldest event.
        ///     After managing the event history, it resets the event timer and clears the current event.
        ///     Finally, it calls PopulateEventQueue to ensure the event queue remains filled for future events.
        /// </summary>
        private void EventEnded()
        {
            var configFile = new IniFile(ParseIniFile.GetTheConfigFile());
            var MaxEventHistoryCount = configFile.ReadInteger("GeneralSettings", "MaxEventHistoryCount");

            // Add the current event to the history queue
            eventHistory.Enqueue(currentEvent.randomEventData.eventType);
            if (eventHistory.Count > MaxEventHistoryCount) eventHistory.Dequeue();

            ResetEventTimer();
            currentEvent = null;
            PopulateEventQueue();
        }


        private void PopulateRandomEventGenerator()
        {
            randomEventGenerator.AddEvents(GetRandomEventData());
        }

        private void ResetEventTimer()
        {
            inGameHoursPassed = 0;
            minutesForNextEvent = MBRandom.RandomInt(GeneralSettings.Basic.GetMinimumRealMinutes(),
                GeneralSettings.Basic.GetMaximumRealMinutes());
            lastEventTime = DateTime.Now;
        }

        private static IEnumerable<RandomEventData> GetRandomEventData()
        {
            var properties = ModSettings.RandomEvents.GetType().GetProperties();

            return properties
                .Select(propertyInfo => (RandomEventData)propertyInfo.GetValue(ModSettings.RandomEvents, null))
                .ToList();
        }
    }
}