using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Extensions
{
    public static class TypeExtensions
    {
        private static readonly Type[] s_predefinedTypes = new Type[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(TimeSpan), typeof(Guid) };

        public static string AssemblyQualifiedNameWithoutVersion(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.AssemblyQualifiedName != null)
            {
                var strArray = type.AssemblyQualifiedName.Split(new char[] { ',' });
                return string.Format("{0}, {1}", strArray[0].Trim(), strArray[1].Trim());
            }

            return null;
        }

        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    if (type.IsNullable(out var innerType))
                    {
                        return innerType.IsNumericType();
                    }
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsSequenceType(this Type type)
        {
            if (type == typeof(string))
                return false;

            return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsSequenceType(this Type type, out Type elementType)
        {
            elementType = null;

            if (type == typeof(string))
                return false;

            if (type.IsArray)
            {
                elementType = type.GetElementType();
            }
            else if (type.IsSubClass(typeof(IEnumerable<>), out var implType))
            {
                var genericArgs = implType.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    elementType = genericArgs[0];
                }
            }

            return elementType != null;
        }

        public static bool IsPredefinedSimpleType(this Type type)
        {
            if ((type.IsPrimitive && (type != typeof(IntPtr))) && (type != typeof(UIntPtr)))
            {
                return true;
            }

            if (type.IsEnum)
            {
                return true;
            }

            return s_predefinedTypes.Any(t => t == type);
        }

        public static bool IsStruct(this Type type)
        {
            if (type.IsValueType)
            {
                return !type.IsPredefinedSimpleType();
            }

            return false;
        }

        public static bool IsPredefinedGenericType(this Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }
            else
            {
                return false;
            }

            return type == typeof(Nullable<>);
        }

        public static bool IsPredefinedType(this Type type)
        {
            if ((!IsPredefinedSimpleType(type) && !IsPredefinedGenericType(type)) && ((type != typeof(byte[]))))
            {
                return (string.Compare(type.FullName, "System.Xml.Linq.XElement", StringComparison.Ordinal) == 0);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlainObjectType(this Type type)
        {
            return type.IsClass && !type.IsSequenceType() && !type.IsPredefinedType();
        }

        public static bool IsInteger(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static Type GetNonNullableType(this Type type)
        {
            if (!IsNullable(type, out var wrappedType))
            {
                return type;
            }

            return wrappedType;
        }

        public static bool IsNullable(this Type type, out Type elementType)
        {
            if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                elementType = type.GetGenericArguments()[0];
            else
                elementType = type;

            return elementType != type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnumType(this Type type)
        {
            return type.GetNonNullableType().IsEnum;
        }

        public static bool IsConstructable(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsAbstract || type.IsInterface || type.IsArray || type.IsGenericTypeDefinition || type == typeof(void))
                return false;

            if (!HasDefaultConstructor(type))
                return false;

            return true;
        }

        [DebuggerStepThrough]
        public static bool IsAnonymous(this Type type)
        {
            if (type.IsGenericType)
            {
                var d = type.GetGenericTypeDefinition();
                if (d.IsClass && d.IsSealed && d.Attributes.HasFlag(TypeAttributes.NotPublic))
                {
                    var attributes = d.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool HasDefaultConstructor(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsValueType)
                return true;

            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Any(ctor => ctor.GetParameters().Length == 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSubClass(this Type type, Type check)
        {
            return IsSubClass(type, check, out Type _);
        }

        public static bool IsSubClass(this Type type, Type check, out Type implementingType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return IsSubClassInternal(type, type, check, out implementingType);
        }

        private static bool IsSubClassInternal(Type initialType, Type currentType, Type check, out Type implementingType)
        {
            if (currentType == check)
            {
                implementingType = currentType;
                return true;
            }

            if (check.IsInterface && (initialType.IsInterface || currentType == initialType))
            {
                foreach (Type t in currentType.GetInterfaces())
                {
                    if (IsSubClassInternal(initialType, t, check, out implementingType))
                    {
                        if (check == implementingType)
                            implementingType = currentType;

                        return true;
                    }
                }
            }

            if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
            {
                if (IsSubClassInternal(initialType, currentType.GetGenericTypeDefinition(), check, out implementingType))
                {
                    implementingType = currentType;
                    return true;
                }
            }

            if (currentType.BaseType == null)
            {
                implementingType = null;
                return false;
            }

            return IsSubClassInternal(initialType, currentType.BaseType, check, out implementingType);
        }

        public static bool IsCompatibleWith(this Type source, Type target)
        {
            if (source == target)
                return true;

            if (!target.IsValueType)
                return target.IsAssignableFrom(source);

            var nonNullableType = source.GetNonNullableType();
            var type = target.GetNonNullableType();

            if ((nonNullableType == source) || (type != target))
            {
                var code = nonNullableType.IsEnum ? TypeCode.Object : Type.GetTypeCode(nonNullableType);
                var code2 = type.IsEnum ? TypeCode.Object : Type.GetTypeCode(type);

                switch (code)
                {
                    case TypeCode.SByte:
                        switch (code2)
                        {
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.Byte:
                        switch (code2)
                        {
                            case TypeCode.Byte:
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.Int16:
                        switch (code2)
                        {
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.UInt16:
                        switch (code2)
                        {
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.Int32:
                        switch (code2)
                        {
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.UInt32:
                        switch (code2)
                        {
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.Int64:
                        switch (code2)
                        {
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.UInt64:
                        switch (code2)
                        {
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                return true;
                        }
                        break;
                    case TypeCode.Single:
                        switch (code2)
                        {
                            case TypeCode.Single:
                            case TypeCode.Double:
                                return true;
                        }
                        break;
                    default:
                        if (nonNullableType == type)
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public static string GetTypeName(this Type type)
        {
            if (type.IsNullable(out var wrappedType))
            {
                return wrappedType.Name + "?";
            }

            return type.Name;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider target, bool inherits) where TAttribute : Attribute
        {
            return target.IsDefined(typeof(TAttribute), inherits);
        }

        public static PropertyInfo GetPropertyFromMethod(this MethodBase method)
        {
            PropertyInfo property = null;
            if (method.IsSpecialName)
            {
                Type containingType = method.DeclaringType;
                if (containingType != null)
                {
                    if (method.Name.StartsWith("get_", StringComparison.InvariantCulture) ||
                        method.Name.StartsWith("set_", StringComparison.InvariantCulture))
                    {
                        string propertyName = method.Name.Substring(4);
                        property = containingType.GetProperty(propertyName);
                    }
                }
            }
            return property;
        }

        internal static Type FindIEnumerable(this Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                var args = seqType.GetGenericArguments();
                foreach (var arg in args)
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                        return ienum;
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                        return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                return FindIEnumerable(seqType.BaseType);

            return null;
        }
        public static Type GetFirstGenericArgumentIfNullable(this Type t)
        {
            if (t.GetGenericArguments().Length > 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments().FirstOrDefault();
            }

            return t;
        }
    }
}
