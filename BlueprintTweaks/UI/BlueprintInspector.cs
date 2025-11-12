using System;
using System.Collections.Generic;
using System.Linq;
using BlueprintTweaks.Core;
using BlueprintTweaks.Models;

namespace BlueprintTweaks.UI
{
    /// <summary>
    /// Blueprint Inspector UI component for logistics station configuration
    /// Provides interactive UI for configuring PLS/ILS/Distributor slots
    /// </summary>
    public class BlueprintInspector
    {
        /// <summary>
        /// Event fired when blueprint data needs to be updated
        /// </summary>
        public event Action<List<StationSlotData>> OnBlueprintDataChanged;

        /// <summary>
        /// Configuration manager instance
        /// </summary>
        private LogisticsConfigManager configManager;

        /// <summary>
        /// Current station being inspected
        /// </summary>
        private StationType currentStationType;

        /// <summary>
        /// UI state tracking
        /// </summary>
        private bool isUIInitialized;
        private Dictionary<int, SlotUIState> slotUIStates;

        /// <summary>
        /// Constructor
        /// </summary>
        public BlueprintInspector()
        {
            configManager = new LogisticsConfigManager();
            slotUIStates = new Dictionary<int, SlotUIState>();
            isUIInitialized = false;

            // Subscribe to config manager events
            configManager.OnSlotChanged += OnSlotConfigChanged;
            configManager.OnValidationComplete += OnValidationCompleted;
        }

        /// <summary>
        /// Initializes the inspector with a station blueprint
        /// </summary>
        public void Initialize(StationType stationType, List<StationSlotData> existingSlots = null, int slotCount = 12)
        {
            currentStationType = stationType;
            configManager.Initialize(stationType, slotCount);

            if (existingSlots != null && existingSlots.Count > 0)
            {
                configManager.LoadSlots(existingSlots);
            }

            InitializeUI();
            isUIInitialized = true;
        }

        /// <summary>
        /// Initializes UI state for all slots
        /// </summary>
        private void InitializeUI()
        {
            slotUIStates.Clear();
            int slotCount = configManager.GetSlotCount();

            for (int i = 0; i < slotCount; i++)
            {
                slotUIStates[i] = new SlotUIState
                {
                    SlotIndex = i,
                    IsExpanded = false,
                    IsHovered = false
                };
            }
        }

        /// <summary>
        /// Renders the logistics configuration panel
        /// This would integrate with the actual game UI system
        /// </summary>
        public void RenderLogisticsPanel()
        {
            if (!isUIInitialized)
                return;

            RenderPanelHeader();

            int slotCount = configManager.GetSlotCount();
            for (int i = 0; i < slotCount; i++)
            {
                RenderSlotConfiguration(i);
            }

            RenderPanelFooter();
        }

        /// <summary>
        /// Renders the panel header with station type information
        /// </summary>
        private void RenderPanelHeader()
        {
            string stationTypeName = currentStationType switch
            {
                StationType.PLS => "Planetary Logistics Station",
                StationType.ILS => "Interstellar Logistics Station",
                StationType.Distributor => "Logistics Distributor",
                _ => "Unknown Station"
            };

            // Pseudo-code for UI rendering
            // In actual implementation, this would use the game's UI framework
            LogUIElement($"=== {stationTypeName} Configuration ===");
        }

        /// <summary>
        /// Renders configuration UI for a single slot
        /// Layout: [Slot Item] [Mode Toggle] [Quantity Input] [Additional Controls]
        /// </summary>
        private void RenderSlotConfiguration(int slotIndex)
        {
            var slot = configManager.GetSlot(slotIndex);
            if (slot == null)
                return;

            var uiState = slotUIStates[slotIndex];

            // Slot row container
            BeginSlotRow(slotIndex);

            // Item icon/selector (left side)
            RenderItemSelector(slotIndex, slot);

            // Mode toggle button(s) (center)
            RenderModeToggle(slotIndex, slot);

            // Quantity input field (center-right)
            RenderQuantityInput(slotIndex, slot);

            // Additional controls based on station type
            RenderAdditionalControls(slotIndex, slot);

            EndSlotRow();
        }

        /// <summary>
        /// Renders the item selector for a slot
        /// </summary>
        private void RenderItemSelector(int slotIndex, StationSlotData slot)
        {
            string itemName = slot.ItemId == 0 ? "Empty" : $"Item {slot.ItemId}";
            
            // Pseudo-code for item picker button
            if (UIButton($"slot_{slotIndex}_item", itemName, 120, 40))
            {
                // Open item picker dialog
                OnItemPickerRequested(slotIndex);
            }

            LogUIElement($"  Slot {slotIndex}: {itemName}");
        }

        /// <summary>
        /// Renders mode toggle button(s)
        /// Different layouts for PLS/ILS/Distributor
        /// </summary>
        private void RenderModeToggle(int slotIndex, StationSlotData slot)
        {
            switch (currentStationType)
            {
                case StationType.PLS:
                    RenderPLSModeToggle(slotIndex, slot);
                    break;
                case StationType.ILS:
                    RenderILSModeToggle(slotIndex, slot);
                    break;
                case StationType.Distributor:
                    RenderDistributorModeToggle(slotIndex, slot);
                    break;
            }
        }

        /// <summary>
        /// Renders PLS mode toggle: Local Supply <-> Local Demand <-> Local Storage
        /// </summary>
        private void RenderPLSModeToggle(int slotIndex, StationSlotData slot)
        {
            string modeText = slot.GetModeDescription();
            string buttonColor = GetModeColor(slot.SupplyDemandMode, StationType.PLS);

            if (UIButton($"slot_{slotIndex}_mode", modeText, 150, 40, buttonColor))
            {
                configManager.ToggleSlotMode(slotIndex);
            }

            LogUIElement($"    Mode: {modeText}");
        }

        /// <summary>
        /// Renders ILS mode toggle with 3-way system
        /// Left button: Local/Remote toggle
        /// Right button: Supply/Demand/Storage cycle
        /// </summary>
        private void RenderILSModeToggle(int slotIndex, StationSlotData slot)
        {
            // Local/Remote toggle button
            string localRemoteText = (slot.SupplyDemandMode >= 4 && slot.SupplyDemandMode <= 6) ? "Remote" : "Local";
            if (UIButton($"slot_{slotIndex}_localremote", localRemoteText, 80, 40, "#4A90E2"))
            {
                configManager.ToggleLocalRemote(slotIndex);
            }

            // Supply/Demand/Storage button
            string typeText = slot.SupplyDemandMode switch
            {
                1 or 4 => "Supply",
                2 or 5 => "Demand",
                3 or 6 => "Storage",
                _ => "None"
            };
            string buttonColor = GetModeColor(slot.SupplyDemandMode, StationType.ILS);

            if (UIButton($"slot_{slotIndex}_type", typeText, 100, 40, buttonColor))
            {
                // Cycle through Supply/Demand/Storage while maintaining Local/Remote
                CycleILSType(slotIndex);
            }

            LogUIElement($"    Mode: {slot.GetModeDescription()}");
        }

        /// <summary>
        /// Renders Distributor mode toggle: Provide <-> Request <-> Both
        /// </summary>
        private void RenderDistributorModeToggle(int slotIndex, StationSlotData slot)
        {
            string modeText = slot.GetModeDescription();
            string buttonColor = slot.SupplyDemandMode switch
            {
                1 => "#50C878", // Green for Provide
                2 => "#FF8C00", // Orange for Request
                3 => "#808080", // Gray for Both
                _ => "#CCCCCC"  // Light gray for None
            };

            if (UIButton($"slot_{slotIndex}_mode", modeText, 200, 40, buttonColor))
            {
                configManager.ToggleSlotMode(slotIndex);
            }

            LogUIElement($"    Mode: {modeText}");
        }

        /// <summary>
        /// Cycles ILS type (Supply/Demand/Storage) while maintaining Local/Remote
        /// </summary>
        private void CycleILSType(int slotIndex)
        {
            var slot = configManager.GetSlot(slotIndex);
            if (slot == null)
                return;

            bool isRemote = slot.SupplyDemandMode >= 4 && slot.SupplyDemandMode <= 6;
            int baseMode = isRemote ? 4 : 1;

            int currentType = slot.SupplyDemandMode - baseMode; // 0=Supply, 1=Demand, 2=Storage
            int nextType = (currentType + 1) % 3;
            int newMode = baseMode + nextType;

            configManager.SetSlotMode(slotIndex, newMode);
        }

        /// <summary>
        /// Renders quantity input field
        /// </summary>
        private void RenderQuantityInput(int slotIndex, StationSlotData slot)
        {
            int maxQuantity = slot.GetMaxQuantityForStationType();
            string quantityText = slot.Quantity.ToString();

            if (UIInputField($"slot_{slotIndex}_quantity", ref quantityText, 80, 40))
            {
                if (int.TryParse(quantityText, out int newQuantity))
                {
                    configManager.UpdateSlotQuantity(slotIndex, newQuantity);
                }
            }

            LogUIElement($"    Quantity: {slot.Quantity}/{maxQuantity}");
        }

        /// <summary>
        /// Renders additional controls based on station type
        /// For ILS: Min Load Percentage slider
        /// For Distributor: Icarus settings
        /// </summary>
        private void RenderAdditionalControls(int slotIndex, StationSlotData slot)
        {
            if (currentStationType == StationType.ILS)
            {
                RenderMinLoadPercentage(slotIndex, slot);
            }
            else if (currentStationType == StationType.Distributor)
            {
                RenderIcarusSettings(slotIndex, slot);
            }
        }

        /// <summary>
        /// Renders min load percentage slider for ILS
        /// </summary>
        private void RenderMinLoadPercentage(int slotIndex, StationSlotData slot)
        {
            int percentage = slot.MinLoadPercentage;
            
            if (UISlider($"slot_{slotIndex}_minload", ref percentage, 0, 100, 80))
            {
                configManager.UpdateMinLoadPercentage(slotIndex, percentage);
            }

            LogUIElement($"    Min Load: {percentage}%");
        }

        /// <summary>
        /// Renders Icarus settings for Logistics Distributor
        /// </summary>
        private void RenderIcarusSettings(int slotIndex, StationSlotData slot)
        {
            // Placeholder for Icarus-specific settings
            // Would include drone behavior, priority, etc.
            LogUIElement($"    Icarus Settings: [TODO]");
        }

        /// <summary>
        /// Renders panel footer with validation status
        /// </summary>
        private void RenderPanelFooter()
        {
            var validationErrors = new List<string>();
            bool isValid = configManager.ValidateAllSlots(out validationErrors);

            if (!isValid && validationErrors.Count > 0)
            {
                LogUIElement("=== Validation Errors ===");
                foreach (var error in validationErrors)
                {
                    LogUIElement($"  ! {error}");
                }
            }

            LogUIElement("=== End Panel ===");
        }

        /// <summary>
        /// Gets color for mode button based on mode and station type
        /// </summary>
        private string GetModeColor(int mode, StationType stationType)
        {
            if (stationType == StationType.PLS || stationType == StationType.ILS)
            {
                return mode switch
                {
                    1 or 4 => "#50C878", // Green for Supply
                    2 or 5 => "#FF8C00", // Orange for Demand
                    3 or 6 => "#4A90E2", // Blue for Storage
                    _ => "#CCCCCC"       // Light gray for None
                };
            }
            return "#CCCCCC";
        }

        /// <summary>
        /// Handles slot configuration changes
        /// Immediately updates blueprint data (live update)
        /// </summary>
        private void OnSlotConfigChanged(int slotIndex, StationSlotData slot)
        {
            // Live update: immediately sync to blueprint
            var allSlots = configManager.GetAllSlots();
            OnBlueprintDataChanged?.Invoke(allSlots);

            LogUIElement($"Slot {slotIndex} updated: {slot.GetModeDescription()}, Qty: {slot.Quantity}");
        }

        /// <summary>
        /// Handles validation completion
        /// </summary>
        private void OnValidationCompleted(bool isValid, string message)
        {
            if (!isValid)
            {
                LogUIElement($"Validation failed: {message}");
            }
        }

        /// <summary>
        /// Handles item picker request
        /// </summary>
        private void OnItemPickerRequested(int slotIndex)
        {
            // Placeholder for item picker dialog
            LogUIElement($"Open item picker for slot {slotIndex}");
        }

        /// <summary>
        /// Exports current configuration for blueprint storage
        /// </summary>
        public List<StationSlotData> ExportConfiguration()
        {
            return configManager.ExportSlots();
        }

        /// <summary>
        /// Gets current station type
        /// </summary>
        public StationType GetStationType()
        {
            return currentStationType;
        }

        // Pseudo-UI methods (would be replaced with actual game UI framework calls)
        private void BeginSlotRow(int slotIndex) { }
        private void EndSlotRow() { }
        private bool UIButton(string id, string text, int width, int height, string color = "#CCCCCC") { return false; }
        private bool UIInputField(string id, ref string text, int width, int height) { return false; }
        private bool UISlider(string id, ref int value, int min, int max, int width) { return false; }
        private void LogUIElement(string message) { Console.WriteLine(message); }

        /// <summary>
        /// UI state for a single slot
        /// </summary>
        private class SlotUIState
        {
            public int SlotIndex { get; set; }
            public bool IsExpanded { get; set; }
            public bool IsHovered { get; set; }
        }
    }
}
