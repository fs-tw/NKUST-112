using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Uow;

namespace Further.Abp.Operation
{
    public static class OperationHelper
    {

        public static List<OperationInfoAttributeBase> GetOperationInfoAttributes(MethodInfo methodInfo)
        {
            return methodInfo
                .GetCustomAttributes(true)
                .OfType<OperationInfoAttributeBase>()
                .Where(x =>　!(x is OperationFailAttribute))
                .ToList();
        }

        public static OperationFailAttribute? GetOperationFailAttributes(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes(true).OfType<OperationFailAttribute>().FirstOrDefault();
        }

        public static bool IsOperationMethod(MethodInfo methodInfo, out OperationScopeAttribute? operationAttribute)
        {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<OperationScopeAttribute>().ToList();

            if (attrs.Any())
            {
                operationAttribute = attrs.First();
                return operationAttribute.IsEnabled;
            }

            if (methodInfo.DeclaringType != null)
            {
                attrs = methodInfo.DeclaringType.GetCustomAttributes(true).OfType<OperationScopeAttribute>().ToList();
                if (attrs.Any())
                {
                    operationAttribute = attrs.First();
                    return operationAttribute.IsEnabled;
                }

                if (typeof(IOperationScopeEnable).GetTypeInfo().IsAssignableFrom(methodInfo.DeclaringType))
                {
                    operationAttribute = null;
                    return true;
                }
            }

            if (HasOperationInfoAttribute(methodInfo))
            {
                operationAttribute = null;
                return true;
            }

            operationAttribute = null;
            return false;
        }

        public static bool IsOperationType(TypeInfo typeInfo)
        {
            if (HasOperationScopeAttribute(typeInfo) || AnyMethodHasOperationkAttribute(typeInfo))
            {
                return true;
            }

            if (typeof(IOperationScopeEnable).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                return true;
            }

            return false;
        }

        public static bool ShouldIntercept(Type type)
        {
            if (DynamicProxyIgnoreTypes.Contains(type))
            {
                return false;
            }

            if (IsOperationType(type.GetTypeInfo()))
            {
                return true;
            }

            return false;
        }

        private static bool AnyMethodHasOperationkAttribute(TypeInfo implementationType)
        {
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(x => HasOperationScopeAttribute(x) || HasOperationInfoAttribute(x));
        }

        private static bool HasOperationScopeAttribute(MemberInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(OperationScopeAttribute), true);
        }

        private static bool HasOperationInfoAttribute(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(OperationInfoAttributeBase), true);
        }
    }
}
