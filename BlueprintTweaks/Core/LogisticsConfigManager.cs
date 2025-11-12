using System;
using System.Collections.Generic;
using System.Linq;
using BlueprintTweaks.Models;

namespace BlueprintTweaks.Core
{
    /// <summary>
    /// Manager class for handling logistics station slot configuration
    /// Provides validation, mode toggling, and blueprint synchronization
    /// </summary>
    public class LogisticsConfigManager
    {
        /// <summary>
        /// Event fired when a slot configuration changes
        /// </summary>
        public event Action<int, StationSlotData> OnSlotChanged;

        /// <summary>
        /// Event fired when all slots are validated
        /// </summary>
        public event Action<bool, string> OnValidationComplete;

        /// <summary>
        /// Current station type being configured
        /// </summary>
        public StationType CurrentStationType { get; private set; }

        /// <summary>
        /// Collection of slots being managed
        /// </summary>
        private Dictionary<int, StationSlotData> slots;

        /// <summary>
        /// Maximum number of slots for current station type
        /// </summary>
        private int maxSlots;

        /// <summary>
        /// Constructor
        /// </summary>
        public LogisticsConfigManager()
        {
            slots = new Dictionary<int, StationSlotData>();
            CurrentStationType = StationType.PLS;
            maxSlots = 12; // Default PLS/ILS slots
        }

        /// <summary>
        /// Initializes the manager with a specific station type
        /// </summary>
        public void Initialize(StationType stationType, int slotCount)
        {
            CurrentStationType = stationType;
            maxSlots = slotCount;
            slots.Clear();

            for (int i = 0; i < slotCount; i++)
            {
                slots[i] = new StationSlotData
                {
                    StationType = stationType
                };
            }
        }

        /// <summary>
        /// Loads existing slot data
        /// </summary>
        public void LoadSlots(List<StationSlotData> slotData)
        {
            if (slotData == null || slotData.Count == 0)
                return;

            slots.Clear();
            for (int i = 0; i < slotData.Count && i < maxSlots; i++)
            {
                slots[i] = slotData[i].Clone();
            }
        }

        /// <summary>
        /// Gets a slot by index
        /// </summary>
        public StationSlotData GetSlot(int slotIndex)
        {
            if (slots.ContainsKey(slotIndex))
                return slots[slotIndex];
            return null;
        }

        /// <summary>
        /// Gets all slots
        /// </summary>
        public List<StationSlotData> GetAllSlots()
        {
            return slots.Values.ToList();
        }

        /// <summary>
        /// Updates a slot's item ID
        /// </summary>
        public bool UpdateSlotItem(int slotIndex, int itemId)
        {
            if (!slots.ContainsKey(slotIndex))
                return false;

            slots[slotIndex].ItemId = itemId;
            OnSlotChanged?.Invoke(slotIndex, slots[slotIndex]);
            return true;
        }

        /// <summary>
        /// Updates a slot's quantity with validation
        /// </summary>
        public bool UpdateSlotQuantity(int slotIndex, int quantity)
        {
            if (!slots.ContainsKey(slotIndex))
                return false;

            var slot = slots[slotIndex];
            int maxQuantity = slot.GetMaxQuantityForStationType();

            // Clamp quantity to valid range
            quantity = Math.Max(1, Math.Min(maxQuantity, quantity));
            slot.Quantity = quantity;

            OnSlotChanged?.Invoke(slotIndex, slot);
            return true;
        }

        /// <summary>
        /// Toggles supply/demand mode for a slot
        /// For PLS: cycles through Local Supply -> Local Demand -> Local Storage -> None
        /// For ILS: cycles through Local Supply -> Local Demand -> Local Storage -> Remote Supply -> Remote Demand -> Remote Storage -> None
        /// For Distributor: cycles through Provide -> Request -> Both -> None
        /// </summary>
        public bool ToggleSlotMode(int slotIndex)
        {
            if (!slots.ContainsKey(slotIndex))
                return false;

            var slot = slots[slotIndex];
            int newMode = GetNextMode(slot.SupplyDemandMode, slot.StationType);
            slot.SupplyDemandMode = newMode;

            OnSlotChanged?.Invoke(slotIndex, slot);
            return true;
        }

        /// <summary>
        /// Sets a specific supply/demand mode for a slot
        /// </summary>
        public bool SetSlotMode(int slotIndex, int mode)
        {
            if (!slots.ContainsKey(slotIndex))
                return false;

            var slot = slots[slotIndex];
            
            // Validate mode for station type
            var tempSlot = new StationSlotData(slot.ItemId, mode, slot.Quantity, slot.MinLoadPercentage, slot.StationType);
            if (!tempSlot.IsValidSupplyDemandMode())
                return false;

            slot.SupplyDemandMode = mode;
            OnSlotChanged?.Invoke(slotIndex, slot);
            return true;
        }

        /// <summary>
        /// Updates min load percentage for a slot (ILS only)
        /// </summary>
        public bool UpdateMinLoadPercentage(int slotIndex, int percentage)
        {
            if (!slots.ContainsKey(slotIndex))
                return false;

            var slot = slots[slotIndex];
            if (slot.StationType != StationType.ILS)
                return false;

            slot.MinLoadPercentage = Math.Max(0, Math.Min(100, percentage));
            OnSlotChanged?.Invoke(slotIndex, slot);
            return true;
        }

        /// <summary>
        /// Validates all slots
        /// </summary>
        public bool ValidateAllSlots(out List<string> errors)
        {
            errors = new List<string>();
            bool allValid = true;

            foreach (var kvp in slots)
            {
                var slot = kvp.Value;
                if (!slot.Validate(out string error))
                {
                    allValid = false;
                    errors.Add($"Slot {kvp.Key}: {error}");
                }
            }

            OnValidationComplete?.Invoke(allValid, string.Join("\n", errors));
            return allValid;
        }

        /// <summary>
        /// Gets the next mode in the cycle for a station type
        /// </summary>
        private int GetNextMode(int currentMode, StationType stationType)
        {
            switch (stationType)
            {
                case StationType.PLS:
                    return currentMode switch
                    {
                        0 => 1, // None -> Local Supply
                        1 => 2, // Local Supply -> Local Demand
                        2 => 3, // Local Demand -> Local Storage
                        3 => 0, // Local Storage -> None
                        _ => 0
                    };

                case StationType.ILS:
                    return currentMode switch
                    {
                        0 => 1, // None -> Local Supply
                        1 => 2, // Local Supply -> Local Demand
                        2 => 3, // Local Demand -> Local Storage
                        3 => 4, // Local Storage -> Remote Supply
                        4 => 5, // Remote Supply -> Remote Demand
                        5 => 6, // Remote Demand -> Remote Storage
                        6 => 0, // Remote Storage -> None
                        _ => 0
                    };

                case StationType.Distributor:
                    return currentMode switch
                    {
                        0 => 1, // None -> Provide
                        1 => 2, // Provide -> Request
                        2 => 3, // Request -> Both
                        3 => 0, // Both -> None
                        _ => 0
                    };

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Cycles between local and remote modes for ILS
        /// Local Supply <-> Remote Supply, Local Demand <-> Remote Demand, Local Storage <-> Remote Storage
        /// </summary>
        public bool ToggleLocalRemote(int slotIndex)
        {
            if (!slots.ContainsKey(slotIndex))
                return false;

            var slot = slots[slotIndex];
            if (slot.StationType != StationType.ILS)
                return false;

            int newMode = slot.SupplyDemandMode switch
            {
                1 => 4, // Local Supply -> Remote Supply
                4 => 1, // Remote Supply -> Local Supply
                2 => 5, // Local Demand -> Remote Demand
                5 => 2, // Remote Demand -> Local Demand
                3 => 6, // Local Storage -> Remote Storage
                6 => 3, // Remote Storage -> Local Storage
                _ => slot.SupplyDemandMode
            };

            slot.SupplyDemandMode = newMode;
            OnSlotChanged?.Invoke(slotIndex, slot);
            return true;
        }

        /// <summary>
        /// Clears a slot
        /// </summary>
        public bool ClearSlot(int slotIndex)
        {
            if (!slots.ContainsKey(slotIndex))
                return false;

            slots[slotIndex] = new StationSlotData
            {
                StationType = CurrentStationType
            };

            OnSlotChanged?.Invoke(slotIndex, slots[slotIndex]);
            return true;
        }

        /// <summary>
        /// Exports slots to a format suitable for blueprint storage
        /// </summary>
        public List<StationSlotData> ExportSlots()
        {
            return slots.Values.Select(s => s.Clone()).ToList();
        }

        /// <summary>
        /// Gets the slot count
        /// </summary>
        public int GetSlotCount()
        {
            return slots.Count;
        }

        /// <summary>
        /// Checks if a slot is empty
        /// </summary>
        public bool IsSlotEmpty(int slotIndex)
        {
            if (!slots.ContainsKey(slotIndex))
                return true;
            return slots[slotIndex].ItemId == 0;
        }
    }
}
