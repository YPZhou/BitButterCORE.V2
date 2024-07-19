using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace BitButterCORE.V2.Testing
{
	public class ObjectTemplateManagerTest
	{
		[Test]
		public void TestLoadObjectTemplate()
		{
			Assert.That(() => ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/DummyObjectTemplate.json"), Throws.Nothing);

			var templateProperty = typeof(ObjectTemplateManager).GetProperty("ObjectTemplates", BindingFlags.NonPublic | BindingFlags.Instance);
			var objectTemplates = templateProperty.GetValue(ObjectTemplateManager.Instance) as Dictionary<string, Dictionary<string, object>>;

			Assert.That(objectTemplates.Count, Is.EqualTo(2), "Should load 2 object templates");
			Assert.That(objectTemplates, Contains.Key("DummyObject1"), "Should contain object template with name DummyObject1");
			Assert.That(objectTemplates, Contains.Key("DummyObject2"), "Should contain object template with name DummyObject2");

			var template1 = objectTemplates["DummyObject1"];
			Assert.That(template1.Count, Is.EqualTo(5), "Should contain 5 properties");
			Assert.That(template1["StringProperty"], Is.EqualTo("TestString1"), "String property");
			Assert.That(template1["IntProperty"], Is.EqualTo(123), "Int property");
			Assert.That(template1["FloatProperty"], Is.EqualTo(1.23f), "Float property");
			Assert.That(template1["BoolProperty"], Is.EqualTo(true), "Bool property");
			Assert.That(template1["ListProperty"], Is.EquivalentTo(new object[] { "123", 123, 1.23f, true }), "List property");

			var template2 = objectTemplates["DummyObject2"];
			Assert.That(template2.Count, Is.EqualTo(5), "Should contain 5 properties");
			Assert.That(template2["StringProperty"], Is.EqualTo("TestString2"), "String property");
			Assert.That(template2["IntProperty"], Is.EqualTo(321), "Int property");
			Assert.That(template2["FloatProperty"], Is.EqualTo(3.21f), "Float property");
			Assert.That(template2["BoolProperty"], Is.EqualTo(false), "Bool property");
			Assert.That(template2["ListProperty"], Is.EquivalentTo(new object[] { "456", 456, 4.56f, false }), "List property");
		}

		[Test]
		public void TestLoadObjectTemplateWithInvalidFile()
		{
			Assert.That(() => ObjectTemplateManager.Instance.LoadObjectTemplate("NonExisting.json"),
				Throws.InvalidOperationException.With.Message.EqualTo("Loading object template from \"NonExisting.json\" failed as file does not exist or not accessible."),
				"Should throw exception when object template file is not accessible");

			Assert.That(() => ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/NotArray.json"),
				Throws.InvalidOperationException.With.Message.EqualTo("Loading object template failed as the root node {} is not a json array."),
				"Should throw exception when object template file is of wrong format");

			Assert.That(() => ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/NotArrayOfObjects.json"),
				Throws.InvalidOperationException.With.Message.EqualTo("Loading object template failed as array item 123 is not a json object."),
				"Should throw exception when object template file is of wrong format");

			Assert.That(() => ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/TemplateDataNotObject.json"),
				Throws.InvalidOperationException.With.Message.EqualTo("Loading object template failed as template 123 is not a json object."),
				"Should throw exception when object template file is of wrong format");

			Assert.That(() => ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/TemplateValueNotSupported.json"),
				Throws.InvalidOperationException.With.Message.EqualTo("Loading object template failed as {} is not a supported value type."),
				"Should throw exception when object template file is of wrong format");

			Assert.That(() => ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/TemplateArrayValueNotSupported.json"),
				Throws.InvalidOperationException.With.Message.EqualTo("Loading object template failed as {} is not a supported value type for array."),
				"Should throw exception when object template file is of wrong format");
		}

		[Test]
		public void TestSetupObjectFromTemplate()
		{
			ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/DummyObjectTemplate.json");

			var templateObject = new NonManagedObjectWithPropertySetter();
			ObjectTemplateManager.Instance.SetupObjectFromTemplate(templateObject, "DummyObject1");

			Assert.That(templateObject.StringProperty, Is.EqualTo("TestString1"), "String property set");
			Assert.That(templateObject.IntProperty, Is.EqualTo(123), "Int property set");
			Assert.That(templateObject.FloatProperty, Is.EqualTo(1.23f), "Float property set");
			Assert.That(templateObject.BoolProperty, Is.EqualTo(true), "Bool property set");
			Assert.That(templateObject.ListProperty, Is.EquivalentTo(new object[] { "123", 123, 1.23f, true }));
			Assert.That(templateObject.AdditionalProperty, Is.EqualTo("DummyObject1"), "Additional property set");
		}

		[Test]
		public void TestIndexer()
		{
			ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/DummyObjectTemplate.json");

			Assert.That(() => ObjectTemplateManager.Instance["NonExistingObject"],
				Throws.InvalidOperationException.With.Message.EqualTo("Getting object template failed as key NonExistingObject does not exist."),
				"Should throw exception when object template name does not exist");

			Assert.That(ObjectTemplateManager.Instance["DummyObject1"].Count, Is.EqualTo(5), "Should contain 4 properties");
			Assert.That(ObjectTemplateManager.Instance["DummyObject2"].Count, Is.EqualTo(5), "Should contain 4 properties");
		}
	}
}
