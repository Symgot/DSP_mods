# BlueprintTweaks - Logistics Configuration Extension

Phase 1 implementation for PLS/ILS/Logistics Distributor configuration UI.

## Implementation Status

✅ StationSlotData model with validation
✅ LogisticsConfigManager with event system
✅ BlueprintInspector UI component
✅ Support for PLS (Local modes)
✅ Support for ILS (Local + Remote modes)
✅ Support for Logistics Distributor (Provide/Request/Both)
✅ Quantity validation (PLS: 1-5000, ILS: 1-10000)
✅ Live update system
✅ Test scenarios documentation

## Project Structure

```
BlueprintTweaks/
├── Models/
│   └── StationSlotData.cs          # Data structure for slot configuration
├── Core/
│   └── LogisticsConfigManager.cs   # Configuration management logic
├── UI/
│   └── BlueprintInspector.cs       # UI component for blueprint inspector
├── TEST_SCENARIOS.md               # Comprehensive test documentation
└── BlueprintTweaks.csproj          # Project file
```

## Key Features

### 1. StationSlotData Model
- Item ID storage
- Supply/Demand/Storage mode configuration
- Quantity limits with validation
- MinLoadPercentage for ILS
- Station type differentiation (PLS/ILS/Distributor)

### 2. LogisticsConfigManager
- Slot configuration management
- Mode toggling with validation
- Quantity clamping to valid ranges
- Event system for change notifications
- Export/import functionality

### 3. BlueprintInspector UI
- Interactive slot configuration
- Mode toggle buttons with color coding
- Quantity input fields
- Live updates to blueprint data
- Support for all station types

## Mode Systems

### PLS (Planetary Logistics Station)
- Local Supply (Green)
- Local Demand (Orange)
- Local Storage (Blue)
- None

### ILS (Interstellar Logistics Station)
- Local Supply/Demand/Storage (1-3)
- Remote Supply/Demand/Storage (4-6)
- 3-way toggle: Local/Remote + Supply/Demand/Storage

### Logistics Distributor
- Provide to other distributors (Green)
- Request from other distributors (Orange)
- Both (Gray)
- None

## Validation Rules

- **PLS Quantity**: 1 to 5,000
- **ILS Quantity**: 1 to 10,000
- **Distributor Quantity**: 1 to 1,000
- **MinLoadPercentage**: 0 to 100 (ILS only)
- **Mode Compatibility**: Validated per station type

## Integration Notes

This implementation provides the core logic with pseudo-UI methods. For game integration:

1. Replace pseudo-UI methods with Unity IMGUI or game's UI framework
2. Load textures and sprites from game assets
3. Connect to game's blueprint save/load system
4. Add localization support
5. Integrate with existing BlueprintTweaks mod infrastructure

## Testing

See `TEST_SCENARIOS.md` for comprehensive test cases covering:
- PLS slot configuration
- ILS 3-way toggle system
- Distributor modes
- Quantity validation
- Multi-slot operations
- Event system
- Persistence

## References

- Issue: [Feature] Phase 1: PLS/ILS Logistik-Konfiguration UI erweitern
- Based on: kremnev8/DSP-Mods BlueprintTweaks architecture
- Game: Dyson Sphere Program

## Next Steps (Future Phases)

- Phase 2: Recipe-based auto-configuration
- Phase 3: Template system
- Phase 4: Blueprint comparison
- Phase 5: Automatic slot adjustment on recipe changes
