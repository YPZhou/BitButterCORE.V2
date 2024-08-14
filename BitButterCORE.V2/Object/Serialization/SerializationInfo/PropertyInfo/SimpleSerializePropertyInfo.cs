using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	internal class SimpleSerializePropertyInfo : SerializePropertyInfo
	{
		public SimpleSerializePropertyInfo(string name, string propertyType, int ctorParameterOrder, object value)
			: base(name, propertyType, ctorParameterOrder, value)
		{
		}

		public SimpleSerializePropertyInfo(JsonObject propertyObject)
			: base(propertyObject)
		{
		}

		protected override string PopulateValueJsonString()
		{
			var result = Value.ToString();
			if (Value is bool boolValue)
			{
				result = boolValue ? "true" : "false";
			}
			else if (Value is string stringValue)
			{
				result = $"\"{stringValue}\"";
			}

			return result;
		}

		protected override object PopulateValue(JsonNode valueNode)
		{
			var result = default(object);
			if (PropertyType == "UInt32")
			{
				result = uint.Parse(valueNode.ToString());
			}
			else if (PropertyType == "Int32")
			{
				result = int.Parse(valueNode.ToString());
			}
			else if (PropertyType == "Single")
			{
				result = float.Parse(valueNode.ToString());
			}
			else if (PropertyType == "String")
			{
				result = valueNode.ToString();
			}
			else if (PropertyType == "Boolean")
			{
				result = bool.Parse(valueNode.ToString());
			}

			return result;
		}
	}
}
