# BlueprintTweaks - Phase 1: PLS/ILS Logistik-Konfiguration UI

## Test Scenarios Documentation

This document outlines the test scenarios for verifying the logistics configuration UI implementation.

---

## Testfall 1: PLS-Slot-Konfiguration

**Objective:** Verify that PLS slots can be configured with Local Supply/Demand/Storage modes and quantity limits.

**Steps:**
1. Initialize BlueprintInspector with StationType.PLS and 12 slots
2. Load existing slot data or start with empty configuration
3. Select Slot 0 and set ItemId to a valid item (e.g., 1001 for Iron Ore)
4. Verify initial state shows "Local Supply" and quantity "5000"
5. Click mode toggle button to cycle: Local Supply → Local Demand → Local Storage → None
6. Set quantity to 3000 via input field
7. Export configuration and verify blueprint data contains updated values

**Expected Results:**
- Mode toggle cycles correctly through 4 states (None, Local Supply, Local Demand, Local Storage)
- Quantity input accepts values between 1 and 5000
- Values outside range are clamped automatically
- Blueprint data updates immediately (live update)
- UI displays correct mode text and color coding

**Validation Code:**
```csharp
var inspector = new BlueprintInspector();
inspector.Initialize(StationType.PLS, null, 12);

// Set up slot 0
var manager = inspector.GetConfigManager();
manager.UpdateSlotItem(0, 1001); // Iron Ore
manager.SetSlotMode(0, SupplyDemandModes.PLSLocalSupply);
manager.UpdateSlotQuantity(0, 5000);

// Toggle mode
manager.ToggleSlotMode(0); // Should go to Local Demand
var slot = manager.GetSlot(0);
Assert.Equal(SupplyDemandModes.PLSLocalDemand, slot.SupplyDemandMode);

// Update quantity
manager.UpdateSlotQuantity(0, 3000);
slot = manager.GetSlot(0);
Assert.Equal(3000, slot.Quantity);

// Validate
var errors = new List<string>();
Assert.True(manager.ValidateAllSlots(out errors));
```

---

## Testfall 2: ILS 3-Wege-Toggle

**Objective:** Verify ILS supports both Local and Remote modes with Supply/Demand/Storage options.

**Steps:**
1. Initialize BlueprintInspector with StationType.ILS and 12 slots
2. Configure Slot 0 with ItemId and initial mode "Local Supply"
3. Click Local/Remote toggle → should switch to "Remote Supply"
4. Click Supply/Demand/Storage cycle → should go Remote Supply → Remote Demand → Remote Storage
5. Click Local/Remote toggle → should switch to "Local Storage"
6. Verify all 6 modes are accessible (Local Supply/Demand/Storage, Remote Supply/Demand/Storage)
7. Export configuration and verify blueprint data

**Expected Results:**
- Local/Remote toggle works independently of Supply/Demand/Storage type
- All 6 modes are accessible via UI
- Mode descriptions are correct
- Color coding: Green (Supply), Orange (Demand), Blue (Storage)
- Blueprint updates immediately

**Validation Code:**
```csharp
var inspector = new BlueprintInspector();
inspector.Initialize(StationType.ILS, null, 12);

var manager = inspector.GetConfigManager();
manager.UpdateSlotItem(0, 1001);
manager.SetSlotMode(0, SupplyDemandModes.ILSLocalSupply);

// Toggle to Remote
manager.ToggleLocalRemote(0);
var slot = manager.GetSlot(0);
Assert.Equal(SupplyDemandModes.ILSRemoteSupply, slot.SupplyDemandMode);

// Cycle type while staying Remote
// Remote Supply (4) → cycle should go through 4,5,6
manager.ToggleSlotMode(0);
slot = manager.GetSlot(0);
Assert.Equal(SupplyDemandModes.ILSRemoteDemand, slot.SupplyDemandMode);

manager.ToggleSlotMode(0);
slot = manager.GetSlot(0);
Assert.Equal(SupplyDemandModes.ILSRemoteStorage, slot.SupplyDemandMode);

// Toggle back to Local
manager.ToggleLocalRemote(0);
slot = manager.GetSlot(0);
Assert.Equal(SupplyDemandModes.ILSLocalStorage, slot.SupplyDemandMode);
```

---

## Testfall 3: Logistics Distributor

**Objective:** Verify Distributor supports Provide/Request/Both modes with correct color coding.

**Steps:**
1. Initialize BlueprintInspector with StationType.Distributor and appropriate slot count
2. Configure Slot 0 with ItemId
3. Set mode to "Provide to other distributors" → UI shows green
4. Toggle to "Request from other distributors" → UI shows orange
5. Toggle to "Both" → UI shows gray
6. Verify Icarus settings section is displayed (placeholder)
7. Export configuration

**Expected Results:**
- Mode toggle cycles: Provide → Request → Both → None
- Color coding: Green (Provide), Orange (Request), Gray (Both)
- Mode descriptions match in-game text
- Blueprint updates immediately
- Icarus settings placeholder visible

**Validation Code:**
```csharp
var inspector = new BlueprintInspector();
inspector.Initialize(StationType.Distributor, null, 6);

var manager = inspector.GetConfigManager();
manager.UpdateSlotItem(0, 1001);

// Set Provide mode
manager.SetSlotMode(0, SupplyDemandModes.DistributorProvide);
var slot = manager.GetSlot(0);
Assert.Equal("Provide to other distributors", slot.GetModeDescription());

// Toggle to Request
manager.ToggleSlotMode(0);
slot = manager.GetSlot(0);
Assert.Equal(SupplyDemandModes.DistributorRequest, slot.SupplyDemandMode);
Assert.Equal("Request from other distributors", slot.GetModeDescription());

// Toggle to Both
manager.ToggleSlotMode(0);
slot = manager.GetSlot(0);
Assert.Equal(SupplyDemandModes.DistributorBoth, slot.SupplyDemandMode);
Assert.Equal("Both", slot.GetModeDescription());
```

---

## Testfall 4: Mengen-Validierung

**Objective:** Verify quantity validation clamps values to valid ranges.

**Steps:**
1. Initialize PLS station (max 5000) and ILS station (max 10000)
2. Attempt to set PLS slot quantity to 0 → should clamp to 1
3. Attempt to set PLS slot quantity to 15000 → should clamp to 5000
4. Attempt to set ILS slot quantity to 15000 → should clamp to 10000
5. Attempt to set negative quantity → should clamp to 1
6. Verify validation reports errors when out of range

**Expected Results:**
- Quantity < 1 is clamped to 1
- PLS quantity > 5000 is clamped to 5000
- ILS quantity > 10000 is clamped to 10000
- Validation method returns false and provides error message
- UI displays clamped value after validation

**Validation Code:**
```csharp
// Test PLS limits
var plsInspector = new BlueprintInspector();
plsInspector.Initialize(StationType.PLS, null, 12);
var plsManager = plsInspector.GetConfigManager();
plsManager.UpdateSlotItem(0, 1001);

// Try invalid values
plsManager.UpdateSlotQuantity(0, 0);
var slot = plsManager.GetSlot(0);
Assert.Equal(1, slot.Quantity); // Clamped to min

plsManager.UpdateSlotQuantity(0, 15000);
slot = plsManager.GetSlot(0);
Assert.Equal(5000, slot.Quantity); // Clamped to PLS max

// Test ILS limits
var ilsInspector = new BlueprintInspector();
ilsInspector.Initialize(StationType.ILS, null, 12);
var ilsManager = ilsInspector.GetConfigManager();
ilsManager.UpdateSlotItem(0, 1001);

ilsManager.UpdateSlotQuantity(0, 15000);
slot = ilsManager.GetSlot(0);
Assert.Equal(10000, slot.Quantity); // Clamped to ILS max

// Test validation error reporting
slot.Quantity = 0; // Manually set invalid
var errors = new List<string>();
bool isValid = slot.Validate(out string error);
Assert.False(isValid);
Assert.Contains("must be at least 1", error);
```

---

## Testfall 5: Multi-Slot-Bearbeitung

**Objective:** Verify multiple slots can be configured and persist correctly.

**Steps:**
1. Initialize ILS station with 12 slots
2. Configure all 12 slots with different items, modes, and quantities:
   - Slot 0: Iron Ore, Local Supply, 5000
   - Slot 1: Copper Ore, Remote Demand, 8000
   - Slot 2: Stone, Local Storage, 3000
   - Slot 3: Coal, Remote Supply, 10000
   - Slot 4: Hydrogen, Remote Demand, 7000
   - ... (continue for all slots)
3. Export configuration to blueprint data
4. Create new inspector and load exported data
5. Verify all slot configurations match original

**Expected Results:**
- All 12 slots can be configured independently
- No interference between slot configurations
- Export produces complete slot data list
- Imported data exactly matches exported data
- All modes, quantities, and items preserved

**Validation Code:**
```csharp
var inspector = new BlueprintInspector();
inspector.Initialize(StationType.ILS, null, 12);
var manager = inspector.GetConfigManager();

// Configure 5 slots with different settings
var testConfigs = new[]
{
    new { SlotIndex = 0, ItemId = 1001, Mode = SupplyDemandModes.ILSLocalSupply, Quantity = 5000 },
    new { SlotIndex = 1, ItemId = 1002, Mode = SupplyDemandModes.ILSRemoteDemand, Quantity = 8000 },
    new { SlotIndex = 2, ItemId = 1003, Mode = SupplyDemandModes.ILSLocalStorage, Quantity = 3000 },
    new { SlotIndex = 3, ItemId = 1004, Mode = SupplyDemandModes.ILSRemoteSupply, Quantity = 10000 },
    new { SlotIndex = 4, ItemId = 1005, Mode = SupplyDemandModes.ILSRemoteDemand, Quantity = 7000 }
};

foreach (var config in testConfigs)
{
    manager.UpdateSlotItem(config.SlotIndex, config.ItemId);
    manager.SetSlotMode(config.SlotIndex, config.Mode);
    manager.UpdateSlotQuantity(config.SlotIndex, config.Quantity);
}

// Export and verify
var exportedSlots = manager.ExportSlots();
Assert.Equal(12, exportedSlots.Count);

// Verify specific configurations
for (int i = 0; i < testConfigs.Length; i++)
{
    var config = testConfigs[i];
    var slot = exportedSlots[config.SlotIndex];
    
    Assert.Equal(config.ItemId, slot.ItemId);
    Assert.Equal(config.Mode, slot.SupplyDemandMode);
    Assert.Equal(config.Quantity, slot.Quantity);
}

// Create new inspector and load data
var newInspector = new BlueprintInspector();
newInspector.Initialize(StationType.ILS, exportedSlots, 12);
var newManager = newInspector.GetConfigManager();

// Verify all configs persisted
foreach (var config in testConfigs)
{
    var slot = newManager.GetSlot(config.SlotIndex);
    Assert.Equal(config.ItemId, slot.ItemId);
    Assert.Equal(config.Mode, slot.SupplyDemandMode);
    Assert.Equal(config.Quantity, slot.Quantity);
}
```

---

## Additional Test Cases

### Test: MinLoadPercentage for ILS
```csharp
var inspector = new BlueprintInspector();
inspector.Initialize(StationType.ILS, null, 12);
var manager = inspector.GetConfigManager();

manager.UpdateSlotItem(0, 1001);
manager.UpdateMinLoadPercentage(0, 75);

var slot = manager.GetSlot(0);
Assert.Equal(75, slot.MinLoadPercentage);

// Test clamping
manager.UpdateMinLoadPercentage(0, 150); // Over 100
slot = manager.GetSlot(0);
Assert.Equal(100, slot.MinLoadPercentage); // Clamped

manager.UpdateMinLoadPercentage(0, -10); // Below 0
slot = manager.GetSlot(0);
Assert.Equal(0, slot.MinLoadPercentage); // Clamped
```

### Test: Event System
```csharp
var inspector = new BlueprintInspector();
bool eventFired = false;
List<StationSlotData> capturedData = null;

inspector.OnBlueprintDataChanged += (slots) => {
    eventFired = true;
    capturedData = slots;
};

inspector.Initialize(StationType.PLS, null, 12);
var manager = inspector.GetConfigManager();

manager.UpdateSlotItem(0, 1001);
// Event should fire
Assert.True(eventFired);
Assert.NotNull(capturedData);
Assert.Equal(12, capturedData.Count);
```

### Test: Slot Clearing
```csharp
var inspector = new BlueprintInspector();
inspector.Initialize(StationType.PLS, null, 12);
var manager = inspector.GetConfigManager();

manager.UpdateSlotItem(0, 1001);
manager.SetSlotMode(0, SupplyDemandModes.PLSLocalSupply);
manager.UpdateSlotQuantity(0, 3000);

// Clear slot
manager.ClearSlot(0);

var slot = manager.GetSlot(0);
Assert.Equal(0, slot.ItemId);
Assert.Equal(0, slot.SupplyDemandMode);
Assert.Equal(1000, slot.Quantity); // Default
```

---

## UI Integration Notes

The current implementation provides the core logic and pseudo-UI methods. For actual game integration:

1. **UI Framework Integration**: Replace pseudo-UI methods with actual game UI framework calls (likely Unity IMGUI or similar)
2. **Texture/Sprite Loading**: Load item icons, button textures, and color schemes from game assets
3. **Localization**: Add support for multiple languages via game's localization system
4. **Input Handling**: Integrate with game's input manager for mouse/keyboard events
5. **Layout System**: Use game's layout system for proper positioning and scaling
6. **Persistence**: Connect to game's blueprint save/load system

---

## Performance Considerations

- **Event Throttling**: Live updates trigger on every change; consider throttling for rapid input
- **UI Refresh Rate**: Optimize rendering to only update changed slots
- **Memory Management**: StationSlotData uses value types where possible; ensure proper disposal
- **Validation Caching**: Cache validation results to avoid repeated checks

---

## Future Enhancements (Post-Phase 1)

- Auto-adjustment of slots based on recipe changes (Phase 5)
- Bulk operations (copy, paste, clear all)
- Templates for common logistics configurations
- Visual diff when comparing blueprints
- Undo/redo functionality
- Preset configurations for common resource chains

---
