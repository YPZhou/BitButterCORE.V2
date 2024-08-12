using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	internal class ObjectReferenceSerializePropertyInfo : SerializePropertyInfo
	{
		public ObjectReferenceSerializePropertyInfo(string name, string propertyType, int ctorParameterOrder, IObjectReference value)
			: base(name, propertyType, ctorParameterOrder, value)
		{
			if (value != null)
			{
				ObjectType = value.Type;
				ID = value.ID;
			}
			else
			{
				ObjectType = null;
				ID = 0;
			}
		}

		public ObjectReferenceSerializePropertyInfo(JsonObject propertyObject)
			: base(propertyObject)
		{
		}

		Type ObjectType { get; }

		uint ID { get; }

		protected override string PopulateValueJsonString()
		{
			var result = "{}";
			if (ObjectType != null)
			{
				result = $"{{\"ObjectType\":\"{ObjectType.Name}\",\"ID\":{ID}}}";
			}

			return result;
		}

		protected override object PopulateValue(JsonNode valueNode)
		{
			var result = default(IObjectReference);
			if (valueNode.GetValueKind() == JsonValueKind.Object)
			{
				var valueObject = valueNode.AsObject();
				if (valueObject.TryGetPropertyValue("ObjectType", out var objectTypeName))
				{
					var objectType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).SingleOrDefault(type => type.Name.Equals(objectTypeName.ToString()));
					var id = uint.Parse(valueObject["ID"].ToString());
					result = CreateTypedReference(objectType, id);
				}
			}

			return result;
		}

		IObjectReference CreateTypedReference(Type objectType, uint id)
		{
			var result = default(IObjectReference);
			if (objectType != null)
			{
				var referenceType = typeof(ObjectReference<>).MakeGenericType(objectType);
				result = (IObjectReference)Activator.CreateInstance(referenceType, id);
			}

			return result;
		}
	}
}
