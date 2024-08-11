using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	public class ObjectSerializer
	{
		public string SerializeObjects(IEnumerable<IObjectReference<IBaseObject>> objects)
		{
			var objectJsonStrings = new List<string>();
			foreach (var serializeObject in objects)
			{
				var serializeObjectInfo = new SerializeObjectInfo(serializeObject);
				objectJsonStrings.Add(serializeObjectInfo.PopulateJsonString());
			}

			return $"[{string.Join(",", objectJsonStrings)}]";
		}

		public void DeserializeObjects(string jsonString)
		{
			var rootNode = JsonNode.Parse(jsonString);
			if (rootNode.GetValueKind() == JsonValueKind.Array)
			{
				var deserializeObjectInfos = ParseJsonArrayAsDeserializeObjectInfos(rootNode.AsArray());
				
				// Build object dependency graph
				// ...

				foreach (var deserializeObjectInfo in deserializeObjectInfos) // populate objects in dependency order
				{
					deserializeObjectInfo.PopulateObject();
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		IEnumerable<DeserializeObjectInfo> ParseJsonArrayAsDeserializeObjectInfos(JsonArray jsonArray)
		{
			var result = new List<DeserializeObjectInfo>();
			foreach (var arrayNode in jsonArray)
			{
				if (arrayNode.GetValueKind() == JsonValueKind.Object)
				{
					var deserializeObjectInfo = new DeserializeObjectInfo(arrayNode.AsObject());
					result.Add(deserializeObjectInfo);
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			return result;
		}
	}
}
