using System;

namespace BlueprintTweaks.Models
{
    /// <summary>
    /// Data structure for logistics station slot configuration
    /// Supports PLS (Planetary Logistics Station), ILS (Interstellar Logistics Station), 
    /// and Logistics Distributor configurations
    /// </summary>
    public class StationSlotData
    {
        /// <summary>
        /// Item ID for this slot (0 means empty slot)
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Supply/Demand/Storage mode for the slot
        /// Values:
        /// - PLS: 0 = None, 1 = Local Supply, 2 = Local Demand, 3 = Local Storage
        /// - ILS: 0 = None, 1 = Local Supply, 2 = Local Demand, 3 = Local Storage, 
        ///        4 = Remote Supply, 5 = Remote Demand, 6 = Remote Storage
        /// - Distributor: 0 = None, 1 = Provide, 2 = Request, 3 = Both
        /// </summary>
        public int SupplyDemandMode { get; set; }

        /// <summary>
        /// Quantity limit for the slot
        /// PLS: 1-5000
        /// ILS: 1-10000
        /// Distributor: typically smaller values
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Minimum load percentage for ships/drones to depart (0-100)
        /// Only applicable for ILS
        /// </summary>
        public int MinLoadPercentage { get; set; }

        /// <summary>
        /// Station type for this slot
        /// </summary>
        public StationType StationType { get; set; }

        /// <summary>
        /// Constructor with default values
        /// </summary>
        public StationSlotData()
        {
            ItemId = 0;
            SupplyDemandMode = 0;
            Quantity = 1000;
            MinLoadPercentage = 0;
            StationType = StationType.PLS;
        }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        public StationSlotData(int itemId, int supplyDemandMode, int quantity, int minLoadPercentage, StationType stationType)
        {
            ItemId = itemId;
            SupplyDemandMode = supplyDemandMode;
            Quantity = quantity;
            MinLoadPercentage = minLoadPercentage;
            StationType = stationType;
        }

        /// <summary>
        /// Validates the slot data according to station type limits
        /// </summary>
        public bool Validate(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (ItemId < 0)
            {
                errorMessage = "Item ID cannot be negative";
                return false;
            }

            if (Quantity < 1)
            {
                errorMessage = "Quantity must be at least 1";
                Quantity = 1;
                return false;
            }

            int maxQuantity = GetMaxQuantityForStationType();
            if (Quantity > maxQuantity)
            {
                errorMessage = $"Quantity cannot exceed {maxQuantity} for {StationType}";
                Quantity = maxQuantity;
                return false;
            }

            if (MinLoadPercentage < 0 || MinLoadPercentage > 100)
            {
                errorMessage = "MinLoadPercentage must be between 0 and 100";
                MinLoadPercentage = Math.Clamp(MinLoadPercentage, 0, 100);
                return false;
            }

            if (!IsValidSupplyDemandMode())
            {
                errorMessage = $"Invalid SupplyDemandMode {SupplyDemandMode} for {StationType}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the maximum quantity allowed for this station type
        /// </summary>
        public int GetMaxQuantityForStationType()
        {
            switch (StationType)
            {
                case StationType.PLS:
                    return 5000;
                case StationType.ILS:
                    return 10000;
                case StationType.Distributor:
                    return 1000;
                default:
                    return 5000;
            }
        }

        /// <summary>
        /// Validates if the current SupplyDemandMode is valid for the station type
        /// </summary>
        public bool IsValidSupplyDemandMode()
        {
            switch (StationType)
            {
                case StationType.PLS:
                    return SupplyDemandMode >= 0 && SupplyDemandMode <= 3;
                case StationType.ILS:
                    return SupplyDemandMode >= 0 && SupplyDemandMode <= 6;
                case StationType.Distributor:
                    return SupplyDemandMode >= 0 && SupplyDemandMode <= 3;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a human-readable description of the current supply/demand mode
        /// </summary>
        public string GetModeDescription()
        {
            switch (StationType)
            {
                case StationType.PLS:
                    return SupplyDemandMode switch
                    {
                        0 => "None",
                        1 => "Local Supply",
                        2 => "Local Demand",
                        3 => "Local Storage",
                        _ => "Unknown"
                    };
                case StationType.ILS:
                    return SupplyDemandMode switch
                    {
                        0 => "None",
                        1 => "Local Supply",
                        2 => "Local Demand",
                        3 => "Local Storage",
                        4 => "Remote Supply",
                        5 => "Remote Demand",
                        6 => "Remote Storage",
                        _ => "Unknown"
                    };
                case StationType.Distributor:
                    return SupplyDemandMode switch
                    {
                        0 => "None",
                        1 => "Provide to other distributors",
                        2 => "Request from other distributors",
                        3 => "Both",
                        _ => "Unknown"
                    };
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Creates a deep copy of this slot data
        /// </summary>
        public StationSlotData Clone()
        {
            return new StationSlotData(ItemId, SupplyDemandMode, Quantity, MinLoadPercentage, StationType);
        }
    }

    /// <summary>
    /// Enum for station types
    /// </summary>
    public enum StationType
    {
        PLS = 0,
        ILS = 1,
        Distributor = 2
    }

    /// <summary>
    /// Constants for supply/demand modes
    /// </summary>
    public static class SupplyDemandModes
    {
        // Common modes
        public const int None = 0;

        // PLS modes
        public const int PLSLocalSupply = 1;
        public const int PLSLocalDemand = 2;
        public const int PLSLocalStorage = 3;

        // ILS modes (includes PLS modes + remote)
        public const int ILSLocalSupply = 1;
        public const int ILSLocalDemand = 2;
        public const int ILSLocalStorage = 3;
        public const int ILSRemoteSupply = 4;
        public const int ILSRemoteDemand = 5;
        public const int ILSRemoteStorage = 6;

        // Distributor modes
        public const int DistributorProvide = 1;
        public const int DistributorRequest = 2;
        public const int DistributorBoth = 3;
    }
}
