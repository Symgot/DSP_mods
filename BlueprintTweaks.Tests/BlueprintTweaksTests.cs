using System;
using System.Collections.Generic;
using Xunit;
using BlueprintTweaks.Models;
using BlueprintTweaks.Core;

namespace BlueprintTweaks.Tests
{
    /// <summary>
    /// Unit tests for StationSlotData model
    /// </summary>
    public class StationSlotDataTests
    {
        [Fact]
        public void Constructor_DefaultValues_ShouldInitializeCorrectly()
        {
            var slot = new StationSlotData();

            Assert.Equal(0, slot.ItemId);
            Assert.Equal(0, slot.SupplyDemandMode);
            Assert.Equal(1000, slot.Quantity);
            Assert.Equal(0, slot.MinLoadPercentage);
            Assert.Equal(StationType.PLS, slot.StationType);
        }

        [Fact]
        public void Constructor_WithParameters_ShouldSetValues()
        {
            var slot = new StationSlotData(1001, 1, 5000, 50, StationType.ILS);

            Assert.Equal(1001, slot.ItemId);
            Assert.Equal(1, slot.SupplyDemandMode);
            Assert.Equal(5000, slot.Quantity);
            Assert.Equal(50, slot.MinLoadPercentage);
            Assert.Equal(StationType.ILS, slot.StationType);
        }

        [Fact]
        public void Validate_PLSWithValidData_ShouldReturnTrue()
        {
            var slot = new StationSlotData(1001, 1, 3000, 0, StationType.PLS);

            bool isValid = slot.Validate(out string error);

            Assert.True(isValid);
            Assert.Empty(error);
        }

        [Fact]
        public void Validate_QuantityTooLow_ShouldClampAndReturnFalse()
        {
            var slot = new StationSlotData(1001, 1, 0, 0, StationType.PLS);

            bool isValid = slot.Validate(out string error);

            Assert.False(isValid);
            Assert.Equal(1, slot.Quantity);
            Assert.Contains("must be at least 1", error);
        }

        [Fact]
        public void Validate_PLSQuantityTooHigh_ShouldClampTo5000()
        {
            var slot = new StationSlotData(1001, 1, 15000, 0, StationType.PLS);

            bool isValid = slot.Validate(out string error);

            Assert.False(isValid);
            Assert.Equal(5000, slot.Quantity);
            Assert.Contains("cannot exceed 5000", error);
        }

        [Fact]
        public void Validate_ILSQuantityTooHigh_ShouldClampTo10000()
        {
            var slot = new StationSlotData(1001, 1, 15000, 0, StationType.ILS);

            bool isValid = slot.Validate(out string error);

            Assert.False(isValid);
            Assert.Equal(10000, slot.Quantity);
            Assert.Contains("cannot exceed 10000", error);
        }

        [Fact]
        public void Validate_MinLoadPercentageOutOfRange_ShouldClamp()
        {
            var slot = new StationSlotData(1001, 1, 5000, 150, StationType.ILS);

            bool isValid = slot.Validate(out string error);

            Assert.False(isValid);
            Assert.Equal(100, slot.MinLoadPercentage);
        }

        [Theory]
        [InlineData(StationType.PLS, 5000)]
        [InlineData(StationType.ILS, 10000)]
        [InlineData(StationType.Distributor, 1000)]
        public void GetMaxQuantityForStationType_ShouldReturnCorrectMax(StationType type, int expected)
        {
            var slot = new StationSlotData { StationType = type };

            int max = slot.GetMaxQuantityForStationType();

            Assert.Equal(expected, max);
        }

        [Theory]
        [InlineData(StationType.PLS, 0, true)]
        [InlineData(StationType.PLS, 1, true)]
        [InlineData(StationType.PLS, 2, true)]
        [InlineData(StationType.PLS, 3, true)]
        [InlineData(StationType.PLS, 4, false)]
        [InlineData(StationType.ILS, 6, true)]
        [InlineData(StationType.ILS, 7, false)]
        [InlineData(StationType.Distributor, 3, true)]
        [InlineData(StationType.Distributor, 4, false)]
        public void IsValidSupplyDemandMode_ShouldValidateCorrectly(StationType type, int mode, bool expected)
        {
            var slot = new StationSlotData { StationType = type, SupplyDemandMode = mode };

            bool isValid = slot.IsValidSupplyDemandMode();

            Assert.Equal(expected, isValid);
        }

        [Theory]
        [InlineData(StationType.PLS, 0, "None")]
        [InlineData(StationType.PLS, 1, "Local Supply")]
        [InlineData(StationType.PLS, 2, "Local Demand")]
        [InlineData(StationType.PLS, 3, "Local Storage")]
        [InlineData(StationType.ILS, 4, "Remote Supply")]
        [InlineData(StationType.ILS, 5, "Remote Demand")]
        [InlineData(StationType.ILS, 6, "Remote Storage")]
        [InlineData(StationType.Distributor, 1, "Provide to other distributors")]
        [InlineData(StationType.Distributor, 2, "Request from other distributors")]
        [InlineData(StationType.Distributor, 3, "Both")]
        public void GetModeDescription_ShouldReturnCorrectText(StationType type, int mode, string expected)
        {
            var slot = new StationSlotData { StationType = type, SupplyDemandMode = mode };

            string description = slot.GetModeDescription();

            Assert.Equal(expected, description);
        }

        [Fact]
        public void Clone_ShouldCreateDeepCopy()
        {
            var original = new StationSlotData(1001, 2, 3000, 75, StationType.ILS);

            var clone = original.Clone();

            Assert.NotSame(original, clone);
            Assert.Equal(original.ItemId, clone.ItemId);
            Assert.Equal(original.SupplyDemandMode, clone.SupplyDemandMode);
            Assert.Equal(original.Quantity, clone.Quantity);
            Assert.Equal(original.MinLoadPercentage, clone.MinLoadPercentage);
            Assert.Equal(original.StationType, clone.StationType);

            // Modify clone shouldn't affect original
            clone.Quantity = 5000;
            Assert.NotEqual(original.Quantity, clone.Quantity);
        }
    }

    /// <summary>
    /// Unit tests for LogisticsConfigManager
    /// </summary>
    public class LogisticsConfigManagerTests
    {
        [Fact]
        public void Initialize_ShouldCreateSlotsWithCorrectType()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.ILS, 12);

            Assert.Equal(StationType.ILS, manager.CurrentStationType);
            Assert.Equal(12, manager.GetSlotCount());

            for (int i = 0; i < 12; i++)
            {
                var slot = manager.GetSlot(i);
                Assert.NotNull(slot);
                Assert.Equal(StationType.ILS, slot.StationType);
            }
        }

        [Fact]
        public void UpdateSlotQuantity_ShouldClampToValidRange()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            // Try to set above PLS max (5000)
            manager.UpdateSlotQuantity(0, 15000);
            Assert.Equal(5000, manager.GetSlot(0).Quantity);

            // Try to set below min (1)
            manager.UpdateSlotQuantity(0, 0);
            Assert.Equal(1, manager.GetSlot(0).Quantity);

            // Set valid value
            manager.UpdateSlotQuantity(0, 3000);
            Assert.Equal(3000, manager.GetSlot(0).Quantity);
        }

        [Fact]
        public void ToggleSlotMode_PLS_ShouldCycleThroughModes()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            // Start at None (0)
            Assert.Equal(0, manager.GetSlot(0).SupplyDemandMode);

            // Toggle to Local Supply (1)
            manager.ToggleSlotMode(0);
            Assert.Equal(1, manager.GetSlot(0).SupplyDemandMode);

            // Toggle to Local Demand (2)
            manager.ToggleSlotMode(0);
            Assert.Equal(2, manager.GetSlot(0).SupplyDemandMode);

            // Toggle to Local Storage (3)
            manager.ToggleSlotMode(0);
            Assert.Equal(3, manager.GetSlot(0).SupplyDemandMode);

            // Toggle back to None (0)
            manager.ToggleSlotMode(0);
            Assert.Equal(0, manager.GetSlot(0).SupplyDemandMode);
        }

        [Fact]
        public void ToggleSlotMode_ILS_ShouldCycleThroughAllModes()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.ILS, 12);

            var expectedModes = new[] { 0, 1, 2, 3, 4, 5, 6, 0 };

            for (int i = 0; i < expectedModes.Length; i++)
            {
                Assert.Equal(expectedModes[i], manager.GetSlot(0).SupplyDemandMode);
                manager.ToggleSlotMode(0);
            }
        }

        [Fact]
        public void ToggleLocalRemote_ILS_ShouldSwitchBetweenLocalAndRemote()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.ILS, 12);

            // Set to Local Supply (1)
            manager.SetSlotMode(0, 1);
            Assert.Equal(1, manager.GetSlot(0).SupplyDemandMode);

            // Toggle to Remote Supply (4)
            manager.ToggleLocalRemote(0);
            Assert.Equal(4, manager.GetSlot(0).SupplyDemandMode);

            // Toggle back to Local Supply (1)
            manager.ToggleLocalRemote(0);
            Assert.Equal(1, manager.GetSlot(0).SupplyDemandMode);

            // Set to Local Demand (2)
            manager.SetSlotMode(0, 2);
            // Toggle to Remote Demand (5)
            manager.ToggleLocalRemote(0);
            Assert.Equal(5, manager.GetSlot(0).SupplyDemandMode);
        }

        [Fact]
        public void UpdateMinLoadPercentage_ILS_ShouldClampAndUpdate()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.ILS, 12);

            // Set valid percentage
            manager.UpdateMinLoadPercentage(0, 75);
            Assert.Equal(75, manager.GetSlot(0).MinLoadPercentage);

            // Try to set above 100
            manager.UpdateMinLoadPercentage(0, 150);
            Assert.Equal(100, manager.GetSlot(0).MinLoadPercentage);

            // Try to set below 0
            manager.UpdateMinLoadPercentage(0, -10);
            Assert.Equal(0, manager.GetSlot(0).MinLoadPercentage);
        }

        [Fact]
        public void UpdateMinLoadPercentage_PLS_ShouldReturnFalse()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            // PLS doesn't support MinLoadPercentage
            bool result = manager.UpdateMinLoadPercentage(0, 50);
            Assert.False(result);
        }

        [Fact]
        public void LoadSlots_ShouldRestoreConfiguration()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.ILS, 12);

            var testData = new List<StationSlotData>
            {
                new StationSlotData(1001, 1, 5000, 50, StationType.ILS),
                new StationSlotData(1002, 5, 8000, 75, StationType.ILS),
                new StationSlotData(1003, 3, 3000, 0, StationType.ILS)
            };

            manager.LoadSlots(testData);

            for (int i = 0; i < testData.Count; i++)
            {
                var slot = manager.GetSlot(i);
                Assert.Equal(testData[i].ItemId, slot.ItemId);
                Assert.Equal(testData[i].SupplyDemandMode, slot.SupplyDemandMode);
                Assert.Equal(testData[i].Quantity, slot.Quantity);
                Assert.Equal(testData[i].MinLoadPercentage, slot.MinLoadPercentage);
            }
        }

        [Fact]
        public void ValidateAllSlots_WithValidData_ShouldReturnTrue()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            manager.UpdateSlotItem(0, 1001);
            manager.SetSlotMode(0, 1);
            manager.UpdateSlotQuantity(0, 3000);

            bool isValid = manager.ValidateAllSlots(out var errors);

            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Fact]
        public void OnSlotChanged_Event_ShouldFireOnUpdates()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            bool eventFired = false;
            int capturedSlotIndex = -1;
            StationSlotData capturedSlot = null;

            manager.OnSlotChanged += (index, slot) =>
            {
                eventFired = true;
                capturedSlotIndex = index;
                capturedSlot = slot;
            };

            manager.UpdateSlotItem(0, 1001);

            Assert.True(eventFired);
            Assert.Equal(0, capturedSlotIndex);
            Assert.NotNull(capturedSlot);
            Assert.Equal(1001, capturedSlot.ItemId);
        }

        [Fact]
        public void ExportSlots_ShouldReturnClonedList()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            manager.UpdateSlotItem(0, 1001);
            manager.UpdateSlotQuantity(0, 3000);

            var exported = manager.ExportSlots();

            Assert.Equal(12, exported.Count);
            Assert.Equal(1001, exported[0].ItemId);
            Assert.Equal(3000, exported[0].Quantity);

            // Modify exported shouldn't affect manager
            exported[0].Quantity = 5000;
            Assert.Equal(3000, manager.GetSlot(0).Quantity);
        }

        [Fact]
        public void ClearSlot_ShouldResetToDefault()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            manager.UpdateSlotItem(0, 1001);
            manager.SetSlotMode(0, 2);
            manager.UpdateSlotQuantity(0, 3000);

            manager.ClearSlot(0);

            var slot = manager.GetSlot(0);
            Assert.Equal(0, slot.ItemId);
            Assert.Equal(0, slot.SupplyDemandMode);
            Assert.Equal(1000, slot.Quantity);
        }

        [Fact]
        public void IsSlotEmpty_ShouldDetectEmptySlots()
        {
            var manager = new LogisticsConfigManager();
            manager.Initialize(StationType.PLS, 12);

            Assert.True(manager.IsSlotEmpty(0));

            manager.UpdateSlotItem(0, 1001);
            Assert.False(manager.IsSlotEmpty(0));
        }
    }
}
