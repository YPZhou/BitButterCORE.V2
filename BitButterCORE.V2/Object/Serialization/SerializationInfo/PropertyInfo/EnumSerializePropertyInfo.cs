using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	internal class EnumSerializePropertyInfo : SerializePropertyInfo
	{
		public EnumSerializePropertyInfo(string name, int ctorParameterOrder, object value)
			: base(name, "Enum", ctorParameterOrder, value)
		{
			EnumType = value.GetType();
		}

		public EnumSerializePropertyInfo(JsonObject propertyObject)
			: base(propertyObject)
		{
		}

		Type EnumType { get; }

		protected override string PopulateValueJsonString()
		{
			return $"{{\"EnumType\":\"{EnumType.Name}\",\"Value\":\"{Value}\"}}"; ;
		}

		protected override object PopulateValue(JsonNode valueNode)
		{
			if (valueNode.GetValueKind() == JsonValueKind.Object)
			{
				var valueObject = valueNode.AsObject();
				var enumTypeName = valueObject["EnumType"];
				var enumType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).SingleOrDefault(type => typeof(Enum).IsAssignableFrom(type) && type.Name.Equals(enumTypeName.ToString()));

				if (enumType != null)
				{
					var enumValueName = valueObject["Value"];
					return Enum.Parse(enumType, enumValueName.ToString());
				}
			}

			return null;
		}
	}
}
