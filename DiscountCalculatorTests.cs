using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Week09QualityLab.Core;

namespace Week09QualityLab.Tests
{
    [TestClass]
    public class DiscountCalculatorTests
    {
        // ---- Step 1 (Red -> Green): Regular customer -------------------------------------
        [TestMethod]
        public void CalculateFinalPrice_ShouldReturnOriginalPrice_ForRegularCustomer()
        {
            // Arrange
            var calculator = new DiscountCalculator();

            // Act
            decimal result = calculator.CalculateFinalPrice(100m, "Regular");

            // Assert
            Assert.AreEqual(100m, result);
        }

        // ---- Step 3: Premium customer ----------------------------------------------------
        [TestMethod]
        public void CalculateFinalPrice_ShouldApplyTenPercentDiscount_ForPremiumCustomer()
        {
            // Arrange
            var calculator = new DiscountCalculator();

            // Act
            decimal result = calculator.CalculateFinalPrice(100m, "Premium");

            // Assert
            Assert.AreEqual(90m, result);
        }

        // ---- Step 4: VIP customer --------------------------------------------------------
        [TestMethod]
        public void CalculateFinalPrice_ShouldApplyTwentyPercentDiscount_ForVipCustomer()
        {
            // Arrange
            var calculator = new DiscountCalculator();

            // Act
            decimal result = calculator.CalculateFinalPrice(100m, "VIP");

            // Assert
            Assert.AreEqual(80m, result);
        }

        // ---- Step 5: Invalid (negative) price --------------------------------------------
        [TestMethod]
        public void CalculateFinalPrice_ShouldThrowException_WhenOriginalPriceIsNegative()
        {
            // Arrange
            var calculator = new DiscountCalculator();

            // Act and Assert
            Assert.ThrowsException<ArgumentException>(() =>
                calculator.CalculateFinalPrice(-50m, "Regular"));
        }

        // ---- Extra edge cases added after reviewing the requirements ---------------------
        [TestMethod]
        [DataRow("Regular")]
        [DataRow("Premium")]
        [DataRow("VIP")]
        public void CalculateFinalPrice_ShouldReturnZero_WhenOriginalPriceIsZero(string customerType)
        {
            // Arrange
            var calculator = new DiscountCalculator();

            // Act
            decimal result = calculator.CalculateFinalPrice(0m, customerType);

            // Assert
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        [DataRow(0.01, "Regular")]
        [DataRow(0.01, "Premium")]
        [DataRow(0.01, "VIP")]
        [DataRow(1000000.0, "VIP")]
        public void CalculateFinalPrice_ShouldNeverReturnNegative_ForAnyValidInput(double price, string customerType)
        {
            // Arrange
            var calculator = new DiscountCalculator();

            // Act
            decimal result = calculator.CalculateFinalPrice((decimal)price, customerType);

            // Assert
            Assert.IsTrue(result >= 0m, $"Final price was negative: {result}");
        }

        [TestMethod]
        public void CalculateFinalPrice_ShouldReturnOriginalPrice_ForUnknownCustomerType()
        {
            // Design decision: anything that is not Premium or VIP is treated like Regular.
            // Arrange
            var calculator = new DiscountCalculator();

            // Act
            decimal result = calculator.CalculateFinalPrice(100m, "Gold");

            // Assert
            Assert.AreEqual(100m, result);
        }
    }
}
