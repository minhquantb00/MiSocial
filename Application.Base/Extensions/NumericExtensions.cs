using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Extensions
{
    public static class NumericExtensions
    {
        #region int

        public static int GetRange(this int id, int size, out int lower)
        {
            var range = (int)Math.Ceiling((decimal)id / size) * size;

            lower = range - (size - 1);

            return range;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? ZeroToNull(this int? value)
        {
            return value <= 0 ? null : value;
        }

        #endregion

        #region decimal

        public static decimal ToTaxPercentage(this decimal inclTax, decimal exclTax, int? decimals = null)
        {
            if (exclTax == decimal.Zero)
            {
                return decimal.Zero;
            }

            var result = ((inclTax / exclTax) - 1.0M) * 100.0M;

            return (decimals.HasValue ? Math.Round(result, decimals.Value) : result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToSmallestCurrencyUnit(this decimal value, MidpointRounding midpoint = MidpointRounding.AwayFromZero)
        {
            return Convert.ToInt32(Math.Round(value * 100, 0, midpoint));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal RoundToNearest(this decimal value, decimal denomination, MidpointRounding midpoint = MidpointRounding.AwayFromZero)
        {
            if (denomination == decimal.Zero)
            {
                return value;
            }

            return Math.Round(value / denomination, midpoint) * denomination;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal RoundToNearest(this decimal value, decimal denomination, bool roundUp)
        {
            if (denomination == decimal.Zero)
            {
                return value;
            }

            var roundedValueBase = roundUp
                ? Math.Ceiling(value / denomination)
                : Math.Floor(value / denomination);

            return Math.Round(roundedValueBase) * denomination;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatInvariant(this decimal value, int decimals = 2)
        {
            return Math.Round(value, decimals).ToString("0.00", CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
