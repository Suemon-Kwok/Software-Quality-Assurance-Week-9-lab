using System;

namespace Week09QualityLab.Core
{
    /// <summary>
    /// Calculates a parking fee for a single-day stay.
    /// First hour = $4, each additional hour = $2, full-day maximum = $20.
    /// Hours are whole, already-rounded chargeable hours between 1 and 24.
    /// </summary>
    public class ParkingFeeCalculator
    {
        private const decimal FirstHourFee = 4m;
        private const decimal AdditionalHourFee = 2m;
        private const decimal FullDayMaximumFee = 20m;
        private const int MinimumHours = 1;
        private const int MaximumHours = 24;

        public decimal CalculateFee(int hours)
        {
            if (hours < MinimumHours || hours > MaximumHours)
            {
                throw new ArgumentException("Hours must be between 1 and 24.");
            }

            decimal fee = FirstHourFee + (hours - 1) * AdditionalHourFee;

            return Math.Min(fee, FullDayMaximumFee);
        }
    }
}
