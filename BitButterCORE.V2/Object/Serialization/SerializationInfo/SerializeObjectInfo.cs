using System;
using System.Collections.Generic;
using System.Reflection;

namespace BitButterCORE.V2
{
	internal class SerializeObjectInfo
	{
		public SerializeObjectInfo(IObjectReference<IBaseObject> baseObject)
		{
			BaseObject = baseObject;
			ObjectType = baseObject.Type;

			PopulateSerializePropertyInfo();
		}

		public IObjectReference<IBaseObject> BaseObject { get; }

		public Type ObjectType { get; }

		public List<SerializePropertyInfo> Properties { get; } = new List<SerializePropertyInfo>();

		public string PopulateJsonString()
		{
			var objectTypeString = WriteObjectType(ObjectType);
			var propertiesString = string.Join(",", WriteProperties());

			return $"{{{objectTypeString},\"Properties\":[{propertiesString}]}}";
		}

		void PopulateSerializePropertyInfo()
		{
			Properties.Clear();
			foreach (var propertyInfo in ObjectType.GetProperties())
			{
				var serializePropertyAttribute = propertyInfo.GetCustomAttribute<SerializePropertyAttribute>();
				if (serializePropertyAttribute != null)
				{
					var propertyValue = propertyInfo.GetValue(BaseObject.Object, null);
					var serializePropertyInfo = CreateSerializePropertyInfo(propertyInfo, propertyValue, serializePropertyAttribute);
					if (serializePropertyInfo != null)
					{
						Properties.Add(serializePropertyInfo);
					}
				}
			}
		}

		SerializePropertyInfo CreateSerializePropertyInfo(PropertyInfo propertyInfo, object propertyValue, SerializePropertyAttribute serializePropertyAttribute)
		{
			var result = default(SerializePropertyInfo);
			if (propertyValue is Enum)
			{
			}
			else if (propertyValue is Array)
			{
			}
			else if (propertyValue is IObjectReference)
			{
			}
			else
			{
				result = new SimpleSerializePropertyInfo(propertyInfo.Name, serializePropertyAttribute.ConstructorParameterOrder, propertyValue);
			}

			return result;
		}

		string WriteObjectType(Type objectType)
		{
			return $"\"ObjectType\":\"{objectType.Name}\"";
		}

		List<string> WriteProperties()
		{
			var result = new List<string>();
			foreach (var property in Properties)
			{
				result.Add(property.PopulateJsonString());
			}

			return result;
		}
	}
}
