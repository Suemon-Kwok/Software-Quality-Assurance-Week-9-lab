using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Week09QualityLab.Core;

namespace Week09QualityLab.Tests
{
    [TestClass]
    public class ParkingFeeCalculatorTests
    {
        // Requirement 1: the first hour costs $4.
        [TestMethod]
        public void CalculateFee_ShouldReturnFourDollars_ForOneHour()
        {
            // Arrange
            var calculator = new ParkingFeeCalculator();

            // Act
            decimal result = calculator.CalculateFee(1);

            // Assert
            Assert.AreEqual(4m, result);
        }

        // Requirement 2: each additional hour costs $2.
        [TestMethod]
        public void CalculateFee_ShouldAddTwoDollarsPerAdditionalHour_ForThreeHours()
        {
            // Arrange
            var calculator = new ParkingFeeCalculator();

            // Act
            decimal result = calculator.CalculateFee(3);

            // Assert
            Assert.AreEqual(8m, result); // 4 + 2 + 2
        }

        // Requirement 3: the full-day maximum is $20 (boundary tests around the cap).
        [TestMethod]
        public void CalculateFee_ShouldReturnEighteenDollars_ForEightHours_JustBelowTheCap()
        {
            // Arrange
            var calculator = new ParkingFeeCalculator();

            // Act
            decimal result = calculator.CalculateFee(8);

            // Assert
            Assert.AreEqual(18m, result); // 4 + 7 * 2
        }

        [TestMethod]
        public void CalculateFee_ShouldReturnTwentyDollars_ForNineHours_ExactlyAtTheCap()
        {
            // Arrange
            var calculator = new ParkingFeeCalculator();

            // Act
            decimal result = calculator.CalculateFee(9);

            // Assert
            Assert.AreEqual(20m, result); // 4 + 8 * 2 = 20
        }

        [TestMethod]
        [DataRow(10)]
        [DataRow(12)]
        [DataRow(24)]
        public void CalculateFee_ShouldCapFeeAtTwentyDollars_WhenUncappedFeeWouldBeHigher(int hours)
        {
            // Arrange
            var calculator = new ParkingFeeCalculator();

            // Act
            decimal result = calculator.CalculateFee(hours);

            // Assert
            Assert.AreEqual(20m, result);
        }

        // Requirement 4: invalid hours throw an exception.
        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(25)]
        public void CalculateFee_ShouldThrowException_WhenHoursAreInvalid(int hours)
        {
            // Arrange
            var calculator = new ParkingFeeCalculator();

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() =>
                calculator.CalculateFee(hours));
        }
    }
}
