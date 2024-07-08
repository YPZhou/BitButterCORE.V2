using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	public class ObjectTemplateManager : BaseSingleton<ObjectTemplateManager>
	{
		public void LoadObjectTemplate(string fileName)
		{
			if (File.Exists(fileName))
			{
				var jsonString = File.ReadAllText(fileName);
				LoadObjectTemplateFromJsonFile(jsonString);
			}
			else
			{
				throw new InvalidOperationException(string.Format("Loading object template from \"{0}\" failed as file does not exist or not accessible.", fileName));
			}
		}

		void LoadObjectTemplateFromJsonFile(string jsonString)
		{
			var rootNode = JsonNode.Parse(jsonString);
			if (rootNode.GetValueKind() == JsonValueKind.Array)
			{
				ObjectTemplates.Clear();
				ParseJsonArrayAsObjectTemplates(rootNode.AsArray());
			}
			else
			{
				throw new InvalidOperationException(string.Format("Loading object template failed as the root node {0} is not a json array.", rootNode));
			}
		}

		void ParseJsonArrayAsObjectTemplates(JsonArray jsonArray)
		{
			foreach (var arrayNode in jsonArray)
			{
				if (arrayNode.GetValueKind() == JsonValueKind.Object)
				{
					var templateData = arrayNode.AsObject().Single();
					if (templateData.Value.GetValueKind() == JsonValueKind.Object)
					{
						if (!ObjectTemplates.ContainsKey(templateData.Key))
						{
							ObjectTemplates.Add(templateData.Key, new Dictionary<string, object>());
						}

						ParseJsonObjectAsObjectTemplate(templateData.Value.AsObject(), ObjectTemplates[templateData.Key]);
					}
					else
					{
						throw new InvalidOperationException(string.Format("Loading object template failed as template {0} is not a json object.", templateData.Value));
					}
				}
				else
				{
					throw new InvalidOperationException(string.Format("Loading object template failed as array item {0} is not a json object.", arrayNode));
				}
			}
		}

		void ParseJsonObjectAsObjectTemplate(JsonObject jsonObject, Dictionary<string, object> template)
		{
			foreach (var property in jsonObject)
			{
				var propertyParsed = false;
				var propertyValue = property.Value;
				if (propertyValue != null)
				{
					var propertyType = propertyValue.GetValueKind();
					if (propertyType == JsonValueKind.String)
					{
						template.Add(property.Key, (string)propertyValue);
						propertyParsed = true;
					}
					else if (propertyType == JsonValueKind.Number)
					{
						template.Add(property.Key, (float)propertyValue);
						propertyParsed = true;
					}
					else if (propertyType == JsonValueKind.True)
					{
						template.Add(property.Key, true);
						propertyParsed = true;
					}
					else if (propertyType == JsonValueKind.False)
					{
						template.Add(property.Key, false);
						propertyParsed = true;
					}
				}

				if (!propertyParsed)
				{
					throw new InvalidOperationException(string.Format("Loading object template failed as {0} is not a supported value type.", propertyValue));
				}
			}
		}

		public Dictionary<string, object> this[string key]
		{
			get
			{
				if (!ObjectTemplates.TryGetValue(key, out var result))
				{
					throw new InvalidOperationException(string.Format("Getting object template failed as key {0} does not exist.", key));
				}

				return result;
			}
		}

		Dictionary<string, Dictionary<string, object>> ObjectTemplates => objectTemplates ?? (objectTemplates = new Dictionary<string, Dictionary<string, object>>());
		Dictionary<string, Dictionary<string, object>> objectTemplates;
	}
}
