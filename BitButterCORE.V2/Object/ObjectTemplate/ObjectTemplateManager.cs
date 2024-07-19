using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
					if (TryGetValueFromJsonNode(propertyValue, out var value))
					{
						propertyParsed = true;
						template.Add(property.Key, value);
					}
					
					if (!propertyParsed && propertyValue.GetValueKind() == JsonValueKind.Array)
					{
						template.Add(property.Key, GetValueListFromJsonArray(propertyValue.AsArray()));
						propertyParsed = true;
					}
				}

				if (!propertyParsed)
				{
					throw new InvalidOperationException(string.Format("Loading object template failed as {0} is not a supported value type.", propertyValue));
				}
			}
		}

		bool TryGetValueFromJsonNode(JsonNode node, out object value)
		{
			var success = false;
			value = null;
			if (node.GetValueKind() == JsonValueKind.String)
			{
				value = (string)node;
				success = true;
			}
			else if (node.GetValueKind() == JsonValueKind.Number)
			{
				if (int.TryParse(node.ToString(), out var intValue))
				{
					value = intValue;
				}
				else
				{
					value = (float)node;
				}

				success = true;
			}
			else if (node.GetValueKind() == JsonValueKind.True)
			{
				value = true;
				success = true;
			}
			else if (node.GetValueKind() == JsonValueKind.False)
			{
				value = false;
				success = true;
			}

			return success;
		}

		List<object> GetValueListFromJsonArray(JsonArray jsonArray)
		{
			var result = new List<object>();
			foreach (var node in jsonArray)
			{
				if (TryGetValueFromJsonNode(node, out var value))
				{
					result.Add(value);
				}
				else
				{
					throw new InvalidOperationException(string.Format("Loading object template failed as {0} is not a supported value type for array.", node));
				}
			}

			return result;
		}

		public void SetupObjectFromTemplate(ITemplateObject templateObject, string templateName)
		{
			var template = this[templateName];
			foreach (var templateProperty in template)
			{
				var objectPropertyInfo = templateObject.GetType().GetProperty(templateProperty.Key, BindingFlags.Public | BindingFlags.Instance);
				if (templateProperty.Value is IList templateList)
				{
					var objectList = objectPropertyInfo?.GetValue(templateObject) as IList;
					if (objectList != null)
					{
						foreach (var value in templateList)
						{
							objectList.Add(value);
						}
					}
				}
				else
				{
					objectPropertyInfo?.SetValue(templateObject, templateProperty.Value);
				}
			}

			templateObject.SetupObjectFromTemplate(templateName, template);
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
