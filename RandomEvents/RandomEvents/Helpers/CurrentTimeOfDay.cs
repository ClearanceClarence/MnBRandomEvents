using TaleWorlds.CampaignSystem;

namespace Bannerlord.RandomEvents.Helpers
{
    /// <summary>
    /// Provides utility methods to determine the current time of day in the game.
    /// </summary>
    public abstract class CurrentTimeOfDay
    {
        /// <summary>
        /// Gets the current hour of the day based on the game's campaign time.
        /// </summary>
        private static int CurrentHour => CampaignTime.Now.GetHourOfDay;

        /// <summary>
        /// Determines if the current time is during the night (before 6 AM).
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current hour is less than 6; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNight => CurrentHour < 6;

        /// <summary>
        /// Determines if the current time is during the morning (6 AM to 12 PM).
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current hour is between 6 and 12; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMorning => CurrentHour >= 6 && CurrentHour < 12;

        /// <summary>
        /// Determines if the current time is during midday (12 PM to 6 PM).
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current hour is between 12 and 18; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMidday => CurrentHour >= 12 && CurrentHour < 18;

        /// <summary>
        /// Determines if the current time is during the evening (6 PM onward).
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current hour is 18 or later; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEvening => CurrentHour >= 18;
    }
}