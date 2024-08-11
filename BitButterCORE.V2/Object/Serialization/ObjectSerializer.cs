using System.Collections.Generic;

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
	}
}
