using NUnit.Framework;

namespace BitButterCORE.V2.Testing
{
	public class ObjectSerializationTest
	{
		[SetUp]
		public void Setup()
		{
			ObjectFactory.Instance.RemoveAll();
			ObjectFactory.Instance.ResetIDFountains();
			ObjectFactory.Instance.ClearChanges();
		}

		[Test]
		public void TestSerializeObjects()
		{
			var objectReference = ObjectFactory.Instance.Create<SerializableObject>(2, 3.3f, "4", false);
			objectReference.Object.IntValue2 = 100;
			objectReference.Object.DummyEnum = DummyEnum.ENUM2;

			var jsonString = ObjectFactory.Instance.SerializeObjects();

			Assert.That(jsonString, Is.EqualTo("[{\"ObjectType\":\"SerializableObject\",\"Properties\":[{\"Name\":\"IntValue\",\"Type\":\"Int32\",\"ConstructorParameterOrder\":1,\"Value\":2},{\"Name\":\"FloatValue\",\"Type\":\"Single\",\"ConstructorParameterOrder\":2,\"Value\":3.3},{\"Name\":\"StringValue\",\"Type\":\"String\",\"ConstructorParameterOrder\":3,\"Value\":\"4\"},{\"Name\":\"BoolValue\",\"Type\":\"Boolean\",\"ConstructorParameterOrder\":4,\"Value\":false},{\"Name\":\"IntValue2\",\"Type\":\"Int32\",\"Value\":100},{\"Name\":\"DummyEnum\",\"Type\":\"Enum\",\"Value\":{\"EnumType\":\"DummyEnum\",\"Value\":\"ENUM2\"}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]}]"));
		}

		[Test]
		public void TestSerializeObjectsWithObjectReference()
		{
			var objectReference = ObjectFactory.Instance.Create<Object>();
			var dummyObject = ObjectFactory.Instance.Create<DummyObject2>();
			var objectReference2 = ObjectFactory.Instance.Create<String>(dummyObject);
			objectReference.Object.SerializableObject = objectReference2;
			var jsonString = ObjectFactory.Instance.SerializeObjects();

			Assert.That(jsonString, Is.EqualTo("[{\"ObjectType\":\"Object\",\"Properties\":[{\"Name\":\"SerializableObject\",\"Type\":\"ObjectReference\",\"Value\":{\"ObjectType\":\"String\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"DummyObject2\",\"Properties\":[{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"String\",\"Properties\":[{\"Name\":\"DummyObject\",\"Type\":\"ObjectReference\",\"Value\":{}},{\"Name\":\"DummyObject2\",\"Type\":\"ObjectReference\",\"ConstructorParameterOrder\":1,\"Value\":{\"ObjectType\":\"DummyObject2\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]}]"));

			objectReference2.Object.DummyObject = ObjectFactory.Instance.Create<DummyObject>();
			jsonString = ObjectFactory.Instance.SerializeObjects();

			Assert.That(jsonString, Is.EqualTo("[{\"ObjectType\":\"Object\",\"Properties\":[{\"Name\":\"SerializableObject\",\"Type\":\"ObjectReference\",\"Value\":{\"ObjectType\":\"String\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"DummyObject2\",\"Properties\":[{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"String\",\"Properties\":[{\"Name\":\"DummyObject\",\"Type\":\"ObjectReference\",\"Value\":{\"ObjectType\":\"DummyObject\",\"ID\":1}},{\"Name\":\"DummyObject2\",\"Type\":\"ObjectReference\",\"ConstructorParameterOrder\":1,\"Value\":{\"ObjectType\":\"DummyObject2\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"DummyObject\",\"Properties\":[{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]}]"));
		}

		[Test]
		public void TestDeserializeObjects()
		{
			var jsonString = "[{\"ObjectType\":\"SerializableObject\",\"Properties\":[{\"Name\":\"IntValue\",\"Type\":\"Int32\",\"ConstructorParameterOrder\":1,\"Value\":2},{\"Name\":\"FloatValue\",\"Type\":\"Single\",\"ConstructorParameterOrder\":2,\"Value\":3.3},{\"Name\":\"StringValue\",\"Type\":\"String\",\"ConstructorParameterOrder\":3,\"Value\":\"4\"},{\"Name\":\"BoolValue\",\"Type\":\"Boolean\",\"ConstructorParameterOrder\":4,\"Value\":false},{\"Name\":\"IntValue2\",\"Type\":\"Int32\",\"Value\":100},{\"Name\":\"DummyEnum\",\"Type\":\"Enum\",\"Value\":{\"EnumType\":\"DummyEnum\",\"Value\":\"ENUM2\"}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":2}]}]";
			ObjectFactory.Instance.DeserializeObjects(jsonString);

			var objectReference = ObjectFactory.Instance.QueryFirst<SerializableObject>();
			Assert.That(objectReference, Is.Not.Null.With.Property("IsValid").EqualTo(true));
			Assert.That(objectReference.Object, Is.Not.Null
				.With.Property("ID").EqualTo(2)
				.With.Property("IntValue").EqualTo(2)
				.With.Property("FloatValue").EqualTo(3.3f)
				.With.Property("StringValue").EqualTo("4")
				.With.Property("BoolValue").EqualTo(false)
				.With.Property("IntValue2").EqualTo(100)
				.With.Property("DummyEnum").EqualTo(DummyEnum.ENUM2));
		}

		[Test]
		public void TestDeserializeObjectsWithObjectReference()
		{
			var jsonString = "[{\"ObjectType\":\"Object\",\"Properties\":[{\"Name\":\"SerializableObject\",\"Type\":\"ObjectReference\",\"Value\":{\"ObjectType\":\"String\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"DummyObject2\",\"Properties\":[{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"String\",\"Properties\":[{\"Name\":\"DummyObject\",\"Type\":\"ObjectReference\",\"Value\":{}},{\"Name\":\"DummyObject2\",\"Type\":\"ObjectReference\",\"ConstructorParameterOrder\":1,\"Value\":{\"ObjectType\":\"DummyObject2\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]}]";
			ObjectFactory.Instance.DeserializeObjects(jsonString);

			var objectReference = ObjectFactory.Instance.QueryFirst<String>();
			Assert.That(objectReference.Object.DummyObject, Is.Null);
			Assert.That(objectReference.Object.DummyObject2, Is.Not.Null.With.Property("IsValid").EqualTo(true));

			var objectReference2 = ObjectFactory.Instance.QueryFirst<Object>();
			Assert.That(objectReference2.Object.SerializableObject, Is.Not.Null.With.Property("IsValid").EqualTo(true));

			jsonString = "[{\"ObjectType\":\"Object\",\"Properties\":[{\"Name\":\"SerializableObject\",\"Type\":\"ObjectReference\",\"Value\":{\"ObjectType\":\"String\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"DummyObject2\",\"Properties\":[{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"String\",\"Properties\":[{\"Name\":\"DummyObject\",\"Type\":\"ObjectReference\",\"Value\":{\"ObjectType\":\"DummyObject\",\"ID\":1}},{\"Name\":\"DummyObject2\",\"Type\":\"ObjectReference\",\"ConstructorParameterOrder\":1,\"Value\":{\"ObjectType\":\"DummyObject2\",\"ID\":1}},{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]},{\"ObjectType\":\"DummyObject\",\"Properties\":[{\"Name\":\"ID\",\"Type\":\"UInt32\",\"ConstructorParameterOrder\":0,\"Value\":1}]}]";
			ObjectFactory.Instance.DeserializeObjects(jsonString);

			objectReference = ObjectFactory.Instance.QueryFirst<String>();
			Assert.That(objectReference.Object.DummyObject, Is.Not.Null.With.Property("IsValid").EqualTo(true));
			Assert.That(objectReference.Object.DummyObject2, Is.Not.Null.With.Property("IsValid").EqualTo(true));

			objectReference2 = ObjectFactory.Instance.QueryFirst<Object>();
			Assert.That(objectReference2.Object.SerializableObject, Is.Not.Null.With.Property("IsValid").EqualTo(true));
		}
	}
}
