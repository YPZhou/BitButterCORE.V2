using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	internal class DeserializeObjectInfo
	{
		public DeserializeObjectInfo(JsonObject jsonObject)
		{
			JsonObject = jsonObject;
		}

		public JsonObject JsonObject { get; }

		public void PopulateObject()
		{
			var objectTypeValue = JsonObject["ObjectType"].ToString();
			var objectType = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.SingleOrDefault(type => typeof(IBaseObject).IsAssignableFrom(type)
										&& type.Name.Equals(objectTypeValue));

			if (objectType != null)
			{
				var propertiesNode = JsonObject["Properties"];
				if (TryParsePropertyArray(propertiesNode, out var constructorParameters, out var normalProperties))
				{
					CreateObject(objectType, constructorParameters, normalProperties);
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		bool TryParsePropertyArray(JsonNode propertiesNode, out List<SerializePropertyInfo> constructorParameters, out List<SerializePropertyInfo> normalProperties)
		{
			var result = true;
			constructorParameters = new List<SerializePropertyInfo>();
			normalProperties = new List<SerializePropertyInfo>();

			if (propertiesNode.GetValueKind() == JsonValueKind.Array)
			{
				var propertiesArray = propertiesNode.AsArray();
				foreach (var propertyNode in propertiesArray)
				{
					if (propertyNode.GetValueKind() == JsonValueKind.Object)
					{
						var serializePropertyInfo = default(SerializePropertyInfo);
						var propertyObject = propertyNode.AsObject();
						var propertyType = propertyObject["Type"].ToString();
						if (propertyType == "Enum")
						{
							serializePropertyInfo = new EnumSerializePropertyInfo(propertyObject);
						}
						else if (propertyType == "ObjectReference")
						{
							serializePropertyInfo = new ObjectReferenceSerializePropertyInfo(propertyObject);
						}
						else
						{
							serializePropertyInfo = new SimpleSerializePropertyInfo(propertyObject);
						}

						if (serializePropertyInfo != null)
						{
							if (serializePropertyInfo.ConstructorParameterOrder >= 0)
							{
								constructorParameters.Add(serializePropertyInfo);
							}
							else
							{
								normalProperties.Add(serializePropertyInfo);
							}
						}
					}
					else
					{
						result = false;
						break;
					}
				}
			}
			else
			{
				result = false;
			}

			return result;
		}

		void CreateObject(Type objectType, List<SerializePropertyInfo> constructorParameters, List<SerializePropertyInfo> normalProperties)
		{
			var orderedCtorParameters = constructorParameters.OrderBy(parameter => parameter.ConstructorParameterOrder);
			var objectID = (uint)orderedCtorParameters.First().Value;
			var ctorParameters = orderedCtorParameters.Skip(1).Select(parameter => parameter.Value).ToArray();

			var deserializedObject = ObjectFactory.Instance.Create(objectType, objectID, ctorParameters);

			foreach (var otherProperty in normalProperties)
			{
				var propertyInfo = objectType.GetProperty(otherProperty.Name);
				propertyInfo.SetValue(deserializedObject.Object, otherProperty.Value);
			}
		}
	}
}
