using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Bannerlord.RandomEvents.Helpers
{
    /// <summary>
    /// Provides methods to determine the player's status in the game.
    /// </summary>
    public static class PlayerStatus
    {
        /// <summary>
        /// Checks if the player has a ranged weapon equipped.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the player has a ranged weapon equipped; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasRangedWeaponEquipped()
        {
            var playerEquipment = Hero.MainHero.BattleEquipment;

            for (var equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
            {
                var item = playerEquipment[equipmentIndex].Item;

                if (item != null && (item.Type == ItemObject.ItemTypeEnum.Thrown || item.Type == ItemObject.ItemTypeEnum.Bow || item.Type == ItemObject.ItemTypeEnum.Crossbow))
                {
                    return true;
                }
            }
            return false;
        }
    }
}