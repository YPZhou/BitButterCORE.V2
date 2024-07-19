using System.Collections.Generic;

namespace BitButterCORE.V2
{
	public interface ITemplateObject
	{
		void SetupObjectFromTemplate(string templateName, Dictionary<string, object> template);
	}
}
