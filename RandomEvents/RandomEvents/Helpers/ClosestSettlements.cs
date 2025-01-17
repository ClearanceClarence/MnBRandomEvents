using System;
using System.Linq;
using System.Windows;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Bannerlord.RandomEvents.Helpers
{
    public static class ClosestSettlements
    {
        /// <summary>
        /// Finds the closest settlement to the given party based on a condition.
        /// </summary>
        /// <param name="heroParty">MobileParty - The party for which the closest settlement is being searched.</param>
        /// <param name="condition">Func&lt;Settlement, bool&gt; - A condition to filter settlements.</param>
        /// <returns>
        /// The closest settlement matching the condition or <c>null</c> if an error occurs.
        /// </returns>
        private static Settlement GetClosestSettlement(MobileParty heroParty, Func<Settlement, bool> condition)
        {
            try
            {
                var settlements = Settlement.FindAll(condition).ToList();
                return settlements.MinBy(settlement => heroParty.GetPosition().DistanceSquared(settlement.GetPosition()));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when trying to find the closest settlement :\n\n {ex.Message} \n\n {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Finds the closest settlement of any type (town, castle, or village) to the given party.
        /// </summary>
        /// <param name="heroParty">MobileParty - The party for which the closest settlement is being searched.</param>
        /// <returns>
        /// The closest settlement of any type or <c>null</c> if no settlement is found.
        /// </returns>
        public static Settlement GetClosestAny(MobileParty heroParty)
        {
            return GetClosestSettlement(heroParty, settlement => settlement.IsTown || settlement.IsCastle || settlement.IsVillage);
        }

        /// <summary>
        /// Finds the closest town to the given party.
        /// </summary>
        /// <param name="heroParty">MobileParty - The party for which the closest town is being searched.</param>
        /// <returns>
        /// The closest town or <c>null</c> if no town is found.
        /// </returns>
        public static Settlement GetClosestTown(MobileParty heroParty)
        {
            return GetClosestSettlement(heroParty, settlement => settlement.IsTown);
        }

        /// <summary>
        /// Finds the closest castle to the given party.
        /// </summary>
        /// <param name="heroParty">MobileParty - The party for which the closest castle is being searched.</param>
        /// <returns>
        /// The closest castle or <c>null</c> if no castle is found.
        /// </returns>
        public static Settlement GetClosestCastle(MobileParty heroParty)
        {
            return GetClosestSettlement(heroParty, settlement => settlement.IsCastle);
        }

        /// <summary>
        /// Finds the closest village to the given party.
        /// </summary>
        /// <param name="heroParty">MobileParty - The party for which the closest village is being searched.</param>
        /// <returns>
        /// The closest village or <c>null</c> if no village is found.
        /// </returns>
        public static Settlement GetClosestVillage(MobileParty heroParty)
        {
            return GetClosestSettlement(heroParty, settlement => settlement.IsVillage);
        }

        /// <summary>
        /// Finds the closest settlement that is either a town or a village to the given party.
        /// </summary>
        /// <param name="heroParty">MobileParty - The party for which the closest town or village is being searched.</param>
        /// <returns>
        /// The closest town or village or <c>null</c> if no such settlement is found.
        /// </returns>
        public static Settlement GetClosestTownOrVillage(MobileParty heroParty)
        {
            return GetClosestSettlement(heroParty, settlement => settlement.IsTown || settlement.IsVillage);
        }
    }
}
