using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	internal abstract class SerializePropertyInfo
	{
		public SerializePropertyInfo(string name, int ctorParameterOrder, object value)
		{
			Name = name;
			PropertyType = value.GetType().Name;
			ConstructorParameterOrder = ctorParameterOrder;
			Value = value;
		}

		public SerializePropertyInfo(JsonObject propertyObject)
		{
			Name = propertyObject["Name"].ToString();
			PropertyType = propertyObject["Type"].ToString();

			if (!propertyObject.TryGetPropertyValue("ConstructorParameterOrder", out var ctorParameterOrderValue))
			{
				ctorParameterOrderValue = -1;
			}

			ConstructorParameterOrder = (int)ctorParameterOrderValue;

			Value = PopulateValue(propertyObject["Value"]);
		}

		public string Name { get; }

		public string PropertyType { get; }

		public int ConstructorParameterOrder { get; }

		public object Value { get; private set; }

		public abstract string PopulateJsonString();

		protected abstract object PopulateValue(JsonNode valueNode);
	}
}
