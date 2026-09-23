using System;

namespace Week09QualityLab.Core
{
    /// <summary>
    /// Calculates the final price for a customer type.
    /// Regular = no discount, Premium = 10% off, VIP = 20% off.
    /// Negative original prices are rejected, so the final price is never negative.
    /// </summary>
    public class DiscountCalculator
    {
        public decimal CalculateFinalPrice(decimal originalPrice, string customerType)
        {
            if (originalPrice < 0)
            {
                throw new ArgumentException("Original price cannot be negative.");
            }

            decimal discountRate = customerType switch
            {
                "Premium" => 0.10m,
                "VIP" => 0.20m,
                _ => 0.00m
            };

            return originalPrice * (1 - discountRate);
        }
    }
}
