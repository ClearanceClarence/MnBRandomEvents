﻿using TaleWorlds.Localization;

namespace Bannerlord.RandomEvents.Helpers
{
    /// <summary>
    /// Provides methods for determining gender-specific terms based on specified parameters.
    /// </summary>
    public static class GenderAssignment
    {
        /// <summary>
        /// Retrieves the gender-specific term based on the provided parameters.
        /// </summary>
        /// <param name="Gender">String - Valid inputs are <c>male</c> and <c>female</c>. Defaults to <c>male</c> if invalid.</param>
        /// <param name="Capitalize">Bool - Set to <c>true</c> to return the assignment in capitalized format.</param>
        /// <param name="Case">String - Valid inputs are <c>subjective</c>, <c>objective</c>, <c>adjective</c>, <c>pronoun</c>, and <c>title</c>. Defaults to <c>subjective</c> if invalid.</param>
        /// <returns>
        /// A string containing the gender-specific term, or <c>null</c> if the input is invalid.
        /// </returns>
        public static string GetTheGenderAssignment(string Gender, bool Capitalize, string Case)
        {
            string Pronoun = null;

            Case = Case.ToLower();
            Gender = Gender.ToLower();

            if (Gender != "male" && Gender != "female")
            {
                Gender = "male";
            }

            if (Case != "subjective" && Case != "objective" && Case != "adjective" && Case != "pronoun" && Case != "title")
            {
                Case = "subjective";
            }

            switch (Gender)
            {
                case "male":
                    switch (Case)
                    {
                        case "subjective":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_male_subjective_cap}He").ToString()
                                : new TextObject("{=gender_male_subjective}he").ToString();
                            break;
                        case "objective":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_male_objective_cap}Him").ToString()
                                : new TextObject("{=gender_male_objective}him").ToString();
                            break;
                        case "adjective":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_male_adjective_cap}His").ToString()
                                : new TextObject("{=gender_male_adjective}his").ToString();
                            break;
                        case "pronoun":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_male_pronoun_cap}His").ToString()
                                : new TextObject("{=gender_male_pronoun}his").ToString();
                            break;
                        case "title":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_male_title_cap}Lord").ToString()
                                : new TextObject("{=gender_male_title}lord").ToString();
                            break;
                    }

                    break;
                case "female":
                    switch (Case)
                    {
                        case "subjective":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_female_subjective_cap}She").ToString()
                                : new TextObject("{=gender_female_subjective}she").ToString();
                            break;
                        case "objective":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_female_objective_cap}Her").ToString()
                                : new TextObject("{=gender_female_objective}her").ToString();
                            break;
                        case "adjective":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_female_adjective_cap}Her").ToString()
                                : new TextObject("{=gender_female_adjective}her").ToString();
                            break;
                        case "pronoun":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_female_pronoun_cap}Hers").ToString()
                                : new TextObject("{=gender_female_pronoun}hers").ToString();
                            break;
                        case "title":
                            Pronoun = Capitalize
                                ? new TextObject("{=gender_female_title_cap}Lady").ToString()
                                : new TextObject("{=gender_female_title}lady").ToString();
                            break;
                    }

                    break;
            }

            return Pronoun;
        }
    }
}
