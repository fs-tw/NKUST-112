using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Further.Abp.Operation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OperationOwnerAttribute : OperationInfoAttributeBase
    {
        public Type EntityType { get; set; } = null!;

        public OperationOwnerAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        public OperationOwnerAttribute(Type entityType, Dictionary<string, object> metadata)
        {
            EntityType = entityType;
            Metadata = metadata;
        }

        public override void UpdateOperationInfo(OperationInfo operationInfo, object methodResult)
        {
            var entityId = FindEntityIdInObject(methodResult, EntityType);
            if (entityId == null) return;

            var ownerInfo = new OperationOwnerInfo();

            ownerInfo.EntityId = (Guid)entityId;
            ownerInfo.EntityType = EntityType.FullName;

            foreach (var metadata in Metadata)
            {
                ownerInfo.MetaData.Add(metadata.Key, metadata.Value);
            }

            operationInfo.Owners.Add(ownerInfo);

        }

        protected Guid? FindEntityIdInObject(object obj, Type entityType)
        {
            if (obj == null) return null;

            // 檢查目前物件是否符合指定的EntityType
            if (obj.GetType().FullName == entityType.FullName || obj.GetType().GetInterfaces().Any(i => i.FullName == entityType.FullName))
            {
                var idProperty = obj.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    return (Guid)idProperty.GetValue(obj);
                }
            }

            // 如果目前物件是IEnumerable，則遍歷集合中的每個元素
            if (obj is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    var result = FindEntityIdInObject(item, entityType);
                    if (result != null)
                    {
                        return result; // 返回找到的第一個id
                    }
                }
            }
            else
            {
                // 遍歷目前物件的所有屬性，遞歸搜尋
                var properties = obj.GetType().GetProperties().Where(p => p.PropertyType.IsClass && p.PropertyType != typeof(string));
                foreach (var property in properties)
                {
                    var propertyValue = property.GetValue(obj);
                    if (propertyValue != null)
                    {
                        var result = FindEntityIdInObject(propertyValue, entityType);
                        if (result != null)
                        {
                            return result; // 返回找到的第一個id
                        }
                    }
                }
            }

            return null; // 如果没有找到返回null
        }
    }
}
