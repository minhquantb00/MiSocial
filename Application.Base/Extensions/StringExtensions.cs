using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        [DebuggerStepThrough]
        public static string EmptyNull(this string value)
        {
            return (value ?? string.Empty).Trim();
        }

        [DebuggerStepThrough]
        public static string NullEmpty(this string value)
        {
            return (string.IsNullOrEmpty(value)) ? null : value;
        }

        [DebuggerStepThrough]
        public static string FormatInvariant(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.InvariantCulture, format, objects);
        }

        [DebuggerStepThrough]
        public static string FormatCurrent(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.CurrentCulture, format, objects);
        }

        [DebuggerStepThrough]
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        [DebuggerStepThrough]
        public static string ReplaceNativeDigits(this string value, IFormatProvider provider = null)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            provider = provider ?? NumberFormatInfo.CurrentInfo;
            var nfi = NumberFormatInfo.GetInstance(provider);

            if (nfi.DigitSubstitution == DigitShapes.None)
            {
                return value;
            }

            var nativeDigits = nfi.NativeDigits;
            var rg = new System.Text.RegularExpressions.Regex(@"\d");

            var result = rg.Replace(value, m => nativeDigits[int.Parse(m.Value)]);
            return result;
        }
        [DebuggerStepThrough]
        public static string EnsureStartsWith(this string value, string startsWith)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (startsWith == null)
                throw new ArgumentNullException(nameof(startsWith));

            return value.StartsWith(startsWith) ? value : (startsWith + value);
        }
        private static bool IsWebUrlInternal(string value, bool schemeIsOptional)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            value = value.Trim().ToLowerInvariant();

            if (schemeIsOptional && value.StartsWith("//"))
            {
                value = "http:" + value;
            }

            return Uri.IsWellFormedUriString(value, UriKind.Absolute) &&
                (value.StartsWith("http://") || value.StartsWith("https://") || value.StartsWith("ftp://"));
        }

        [DebuggerStepThrough]
        public static bool IsWebUrl(this string value)
        {
            return IsWebUrlInternal(value, false);
        }

        [DebuggerStepThrough]
        public static bool IsWebUrl(this string value, bool schemeIsOptional)
        {
            return IsWebUrlInternal(value, schemeIsOptional);
        }
        [DebuggerStepThrough]
        public static string NaIfEmpty(this string value)
        {
            return (string.IsNullOrWhiteSpace(value) ? "n/a" : value);
        }
        public static bool IsCaseInsensitiveEqual(this string value, string comparing)
        {
            return string.Compare(value, comparing, StringComparison.OrdinalIgnoreCase) == 0;
        }
        public static string EnsureEndsWith(this string value, string endWith)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (endWith == null)
                throw new ArgumentNullException(nameof(endWith));

            if (value.Length >= endWith.Length)
            {
                if (string.Compare(value, value.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;

                string trimmedString = value.TrimEnd(null);

                if (string.Compare(trimmedString, trimmedString.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;
            }

            return value + endWith;
        }
        public static string FormatWith(this string format, params object[] args)
        {
            return FormatWith(format, CultureInfo.CurrentCulture, args);
        }
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }
        [DebuggerStepThrough]
        public static string Grow(this string value, string grow, string delimiter)
        {
            if (string.IsNullOrEmpty(value))
                return (string.IsNullOrEmpty(grow) ? "" : grow);

            if (string.IsNullOrEmpty(grow))
                return (string.IsNullOrEmpty(value) ? "" : value);

            return value + delimiter + grow;
        }

    }
}
