using NUnit.Framework;

namespace BitButterCORE.V2.Testing
{
	public class ObjectReferenceTest
	{
		[SetUp]
		public void Setup()
		{
			ObjectFactory.Instance.RemoveAll();
			ObjectFactory.Instance.ResetIDFountains();
		}

		[Test]
		public void TestType()
		{
			var objectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference1.Type, Is.EqualTo(typeof(DummyObject)));

			var objectReference2 = ObjectFactory.Instance.Create<DummyObject2>();
			Assert.That(objectReference2.Type, Is.EqualTo(typeof(DummyObject2)));
		}

		[Test]
		public void TestID()
		{
			var dummyObjectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(dummyObjectReference1.ID, Is.EqualTo(1));

			var dummyObjectReference2 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(dummyObjectReference2.ID, Is.EqualTo(2));

			var dummyObject2Reference = ObjectFactory.Instance.Create<DummyObject2>();
			Assert.That(dummyObject2Reference.ID, Is.EqualTo(1));
		}

		[Test]
		public void TestObject()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference.Object, Is.Not.Null, "pre-condition");

			ObjectFactory.Instance.RemoveAll<DummyObject>();
			Assert.That(objectReference.Object, Is.Null, "Object should be null after removal");
		}

		[Test]
		public void TestGetObject()
		{
			var defaultReference = default(ObjectReference);
			Assert.That(defaultReference.GetObject<DummyObject>(), Is.Null, "Default reference should return null");

			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference.GetObject<DummyObject2>(), Is.Null, "Should return null when generic type is not matched");

			var dummyObject = objectReference.GetObject<DummyObject>();
			Assert.That(dummyObject, Is.Not.Null, "Should return non-null object");
			Assert.That(dummyObject.GetType(), Is.EqualTo(typeof(DummyObject)), "Object type should be DummyObject instead of BaseObject");
			Assert.That(dummyObject.ID, Is.EqualTo(objectReference.Object.ID), "Object ID should match that returned by Object property");

			var baseObject = objectReference.GetObject<BaseObject>();
			Assert.That(baseObject, Is.Not.Null, "Should return non-null object when generic type is the base class type");
			Assert.That(baseObject.GetType(), Is.EqualTo(typeof(DummyObject)), "Object type should be DummyObject instead of BaseObject");
			Assert.That(baseObject.ID, Is.EqualTo(objectReference.Object.ID), "Object ID should match that returned by Object property");
		}

		[Test]
		public void TestIsValid()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference.IsValid, Is.True, "pre-condition");

			ObjectFactory.Instance.RemoveAll<DummyObject>();
			Assert.That(objectReference.IsValid, Is.False, "Reference should be no longer valid");
		}

		[Test]
		public void TestIsValidByTypeAndID()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();

			Assert.That(objectReference.IsValidByTypeAndID<DummyObject>(2), Is.False, "Should be non valid when ID is not correct");
			Assert.That(objectReference.IsValidByTypeAndID<DummyObject2>(1), Is.False, "Should be non valid when type is not correct");
			Assert.That(objectReference.IsValidByTypeAndID<DummyObject>(1), Is.True, "Should be valid when both type and ID are correct");
			Assert.That(objectReference.IsValidByTypeAndID<BaseObject>(1), Is.True, "Should be valid when checking against base class type");
		}

		[Test]
		public void TestDefaultObjectReferenceIsInvalid()
		{
			var defaultReference = default(ObjectReference);
			Assert.That(defaultReference.IsValid, Is.False, "Default reference is not valid");
		}
	}
}
