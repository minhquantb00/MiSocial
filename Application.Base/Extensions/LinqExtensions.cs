using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Extensions
{
    public static class LinqExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo ExtractPropertyInfo(this LambdaExpression propertyAccessor)
        {
            return propertyAccessor.ExtractMemberInfo() as PropertyInfo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo ExtractFieldInfo(this LambdaExpression propertyAccessor)
        {
            return propertyAccessor.ExtractMemberInfo() as FieldInfo;
        }

        [SuppressMessage("ReSharper", "CanBeReplacedWithTryCastAndCheckForNull")]
        public static MemberInfo ExtractMemberInfo(this LambdaExpression propertyAccessor)
        {
            if (propertyAccessor == null)
                throw new ArgumentNullException(nameof(propertyAccessor));

            MemberInfo info;
            try
            {
                MemberExpression operand;
                LambdaExpression expression = propertyAccessor;

                if (expression.Body is UnaryExpression body)
                {
                    operand = (MemberExpression)body.Operand;
                }
                else
                {
                    operand = (MemberExpression)expression.Body;
                }

                MemberInfo member = operand.Member;
                info = member;
            }
            catch (Exception e)
            {
                throw new ArgumentException("The property or field accessor expression is not in the expected format 'o => o.PropertyOrField'.", e);
            }

            return info;
        }

    }
}
