using TaleWorlds.Localization;

namespace Bannerlord.RandomEvents.Helpers
{
    /// <summary>
    ///     Provides utility methods to retrieve the demonym (cultural or citizen name) based on a culture identifier.
    /// </summary>
    /// <remarks>
    ///     In English, demonyms are always capitalized. Defaults to "Imperial" if the culture is not recognized.
    /// </remarks>
    public static class Demonym
    {
        /// <summary>
        ///     Retrieves the demonym (noun or adjective) for a given culture.
        /// </summary>
        /// <param name="culture">The culture identifier (e.g., "Empire", "Vlandia").</param>
        /// <param name="useNounForm">
        ///     If <c>true</c>, returns a noun form with article (e.g., "a Vlandian");
        ///     otherwise returns an adjective (e.g., "Vlandian").
        /// </param>
        /// <returns>
        ///     The localized demonym string, or "Imperial" if the culture is unrecognized.
        /// </returns>
        public static string GetTheDemonym(string culture, bool useNounForm)
        {
            return useNounForm
                ? culture switch
                {
                    "Empire" => new TextObject("{=Demonym_Empire_Noun}an Imperial").ToString(),
                    "Nord" => new TextObject("{=Demonym_Nord_Noun}a Nord").ToString(),
                    "Vlandia" => new TextObject("{=Demonym_Vlandia_Noun}a Vlandian").ToString(),
                    "Sturgia" => new TextObject("{=Demonym_Sturgia_Noun}a Sturgian").ToString(),
                    "Battania" => new TextObject("{=Demonym_Battania_Noun}a Battanian").ToString(),
                    "Aserai" => new TextObject("{=Demonym_Aserai_Noun}an Aserai").ToString(),
                    "Khuzait" => new TextObject("{=Demonym_Khuzait_Noun}a Khuzait").ToString(),
                    _ => new TextObject("{=Demonym_Default_Noun}an Imperial").ToString()
                }
                : culture switch
                {
                    "Empire" => new TextObject("{=Demonym_Empire}Imperial").ToString(),
                    "Nord" => new TextObject("{=Demonym_Nord}Nord").ToString(),
                    "Vlandia" => new TextObject("{=Demonym_Vlandia}Vlandian").ToString(),
                    "Sturgia" => new TextObject("{=Demonym_Sturgia}Sturgian").ToString(),
                    "Battania" => new TextObject("{=Demonym_Battania}Battanian").ToString(),
                    "Aserai" => new TextObject("{=Demonym_Aserai}Aserai").ToString(),
                    "Khuzait" => new TextObject("{=Demonym_Khuzait}Khuzait").ToString(),
                    _ => new TextObject("{=Demonym_Default}Imperial").ToString()
                };
        }
    }
}