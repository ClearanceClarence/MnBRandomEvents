using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Bannerlord.RandomEvents.Helpers
{
    /// <summary>
    /// Provides methods for creating and managing mobile parties in the game.
    /// </summary>
    public static class PartySetup
    {
        /// <summary>
        /// Creates a looter party near the main player's party.
        /// </summary>
        /// <param name="partyName">Optional string - The name of the looter party. If null, a default name is used.</param>
        /// <returns>
        /// A new <see cref="MobileParty"/> representing the looter party, or <c>null</c> if creation fails.
        /// </returns>
        public static MobileParty CreateLooterParty(string partyName = null)
        {
            MobileParty banditParty = null;

            try
            {
                var hideouts = Settlement.FindAll(s => s.IsHideout).ToList();
                var closestHideout = hideouts.MinBy(s => MobileParty.MainParty.GetPosition().DistanceSquared(s.GetPosition()));

                var banditCultureObject = MBObjectManager.Instance.GetObject<CultureObject>("looters");

                partyName ??= $"{banditCultureObject.Name} (Random Event)";

                var partyTemplate = MBObjectManager.Instance.GetObject<PartyTemplateObject>($"{banditCultureObject.StringId}_template");

                banditParty = BanditPartyComponent.CreateLooterParty(
                    $"randomevent_{banditCultureObject.StringId}_{MBRandom.RandomInt(int.MaxValue)}",
                    closestHideout.OwnerClan,
                    closestHideout,
                    false);

                var partyNameTextObject = new TextObject(partyName);

                banditParty.InitializeMobilePartyAroundPosition(partyTemplate, MobileParty.MainParty.Position2D, 0.2f, 0.1f, 20);
                banditParty.SetCustomName(partyNameTextObject);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while trying to create a mobile bandit party :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }

            return banditParty;
        }

        /// <summary>
        /// Creates a bandit party near the main player's party with a specified or fallback culture.
        /// </summary>
        /// <param name="cultureObjectId">
        /// Optional. The string ID of the desired <see cref="CultureObject"/> to use for the bandit party.
        /// If null or invalid, the system will attempt to use the closest hideout's culture.
        /// If that fails, it will repeatedly try known fallback cultures until a valid combination is found.
        /// </param>
        /// <param name="partyName">
        /// Optional. The display name of the created party. If null, a name is automatically generated
        /// based on the selected culture.
        /// </param>
        /// <returns>
        /// A <see cref="MobileParty"/> instance representing the bandit party. Returns <c>null</c> if no valid
        /// culture/template/clan combination could be found, or if hideout data is invalid.
        /// </returns>
        /// <remarks>
        /// This method is resilient to missing data and will try up to 100 times to find a valid combination
        /// of <see cref="CultureObject"/>, <see cref="PartyTemplateObject"/>, and <see cref="Clan"/> before giving up.
        /// The party is spawned near the main player’s position, and initialized using the matching template and hideout.
        /// </remarks>

        public static MobileParty CreateBanditParty(string cultureObjectId = null, string partyName = null)
{
    MobileParty banditParty = null;

    try
    {
        var hideouts = Settlement.FindAll(s => s.IsHideout && s.Hideout != null).ToList();
        var closestHideout = hideouts
            .MinBy(s => MobileParty.MainParty.GetPosition().DistanceSquared(s.GetPosition()));

        if (closestHideout == null || closestHideout.Hideout == null)
        {
            MessageBox.Show("No valid hideout found nearby.");
            return null;
        }

        CultureObject banditCultureObject = null;
        PartyTemplateObject partyTemplate = null;
        Clan matchedClan = null;

        // Try the requested culture first
        if (!string.IsNullOrEmpty(cultureObjectId))
        {
            banditCultureObject = MBObjectManager.Instance.GetObject<CultureObject>(cultureObjectId);
            if (banditCultureObject != null)
            {
                partyTemplate = MBObjectManager.Instance.GetObject<PartyTemplateObject>($"{banditCultureObject.StringId}_template");
                matchedClan = Clan.BanditFactions.FirstOrDefault(clan => clan.DefaultPartyTemplate == partyTemplate);
            }
        }

        // Keep trying random fallback cultures until we find one that works
        var fallbackCultures = new List<string>
        {
            "Vlandia", "westernEmpire", "easternEmpire", "northernEmpire",
            "Khuzait", "Aserai", "Sturgia", "Battania"
        };

        int safetyLimit = 100; // prevent infinite loop
        while ((matchedClan == null || partyTemplate == null) && safetyLimit-- > 0)
        {
            string fallbackId = fallbackCultures[MBRandom.RandomInt(fallbackCultures.Count)];
            var fallbackCulture = MBObjectManager.Instance.GetObject<CultureObject>(fallbackId);
            if (fallbackCulture == null)
                continue;

            var fallbackTemplate = MBObjectManager.Instance.GetObject<PartyTemplateObject>($"{fallbackCulture.StringId}_template");
            if (fallbackTemplate == null)
                continue;

            var fallbackClan = Clan.BanditFactions.FirstOrDefault(clan => clan.DefaultPartyTemplate == fallbackTemplate);
            if (fallbackClan != null)
            {
                banditCultureObject = fallbackCulture;
                partyTemplate = fallbackTemplate;
                matchedClan = fallbackClan;
            }
        }

        if (matchedClan == null || partyTemplate == null)
        {
            MessageBox.Show("Could not find a working fallback culture/clan/template even after multiple tries.");
            return null;
        }

        partyName ??= $"{banditCultureObject.Name} (Random Event)";
        var partyNameTextObject = new TextObject(partyName);

        banditParty = BanditPartyComponent.CreateBanditParty(
            $"randomevent_{banditCultureObject.StringId}_{MBRandom.RandomInt(int.MaxValue)}",
            matchedClan,
            closestHideout.Hideout,
            false);

        banditParty.InitializeMobilePartyAroundPosition(partyTemplate, MobileParty.MainParty.Position2D, 0.2f, 0.1f, 20);
        banditParty.SetCustomName(partyNameTextObject);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error while trying to create a mobile bandit party :\n\n {ex.Message} \n\n {ex.StackTrace}");
    }

    return banditParty;
}


        /// <summary>
        /// Adds random units from a specified or default culture to a party.
        /// </summary>
        /// <param name="party">The <see cref="MobileParty"/> to add units to.</param>
        /// <param name="numberToAdd">The number of units to add.</param>
        /// <param name="overrideCulture">Optional <see cref="CultureObject"/> to override the party's culture.</param>
        public static void AddRandomCultureUnits(MobileParty party, int numberToAdd, CultureObject overrideCulture = null)
        {
            try
            {
                var partyCultureObject = overrideCulture ?? party.Party.Culture;

                var characterObjectList = partyCultureObject.IsBandit ? GetBanditCharacters(partyCultureObject) : GetMainCultureCharacters(partyCultureObject);

                var spawnNumbers = new int[characterObjectList.Count];
                var currentSpawned = 0;

                while (currentSpawned < numberToAdd)
                {
                    var randomInt = MBRandom.RandomInt(0, spawnNumbers.Length);
                    spawnNumbers[randomInt]++;
                    currentSpawned++;
                }

                for (var i = 0; i < characterObjectList.Count; i++)
                {
                    var characterObject = characterObjectList[i];
                    if (characterObject != null)
                        party.AddElementToMemberRoster(characterObject, spawnNumbers[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while trying to add random culture units :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Retrieves a list of bandit character objects for a given culture.
        /// </summary>
        /// <param name="partyCultureObject">The <see cref="CultureObject"/> representing the party's culture.</param>
        /// <returns>A list of <see cref="CharacterObject"/> for the bandit culture.</returns>
        private static List<CharacterObject> GetBanditCharacters(CultureObject partyCultureObject)
        {
            var characterObjectList = new List<CharacterObject>();

            if (partyCultureObject.StringId == "looters")
            {
                characterObjectList.Add(partyCultureObject.BasicTroop);
            }
            else
            {
                characterObjectList.Add(partyCultureObject.BanditBandit);
                characterObjectList.Add(partyCultureObject.BanditRaider);
                characterObjectList.Add(partyCultureObject.BanditChief);
            }

            return characterObjectList;
        }

        /// <summary>
        /// Retrieves a list of main culture character objects for a given culture.
        /// </summary>
        /// <param name="partyCultureObject">The <see cref="CultureObject"/> representing the party's culture.</param>
        /// <returns>A list of <see cref="CharacterObject"/> for the main culture.</returns>
        private static List<CharacterObject> GetMainCultureCharacters(CultureObject partyCultureObject)
        {
            var characterObjectList = new List<CharacterObject>();

            if (partyCultureObject.BasicTroop != null)
            {
                characterObjectList.Add(partyCultureObject.BasicTroop);
            }
            else
            {
                return characterObjectList;
            }

            foreach (var upgradeTarget in partyCultureObject.BasicTroop.UpgradeTargets)
            {
                CollectFromTroopTree(upgradeTarget, characterObjectList);
            }

            return characterObjectList;
        }

        /// <summary>
        /// Recursively collects troop upgrade targets and adds them to the provided list.
        /// </summary>
        /// <param name="co">The current <see cref="CharacterObject"/> to process.</param>
        /// <param name="characterObjectList">The list to which upgrade targets will be added.</param>
        private static void CollectFromTroopTree(CharacterObject co, ICollection<CharacterObject> characterObjectList)
        {
            if (co.UpgradeTargets == null || co.UpgradeTargets.Length == 0)
                return;

            foreach (var upgradeTarget in co.UpgradeTargets)
            {
                if (!characterObjectList.Contains(upgradeTarget))
                    characterObjectList.Add(upgradeTarget);

                CollectFromTroopTree(upgradeTarget, characterObjectList);
            }
        }
    }
}
