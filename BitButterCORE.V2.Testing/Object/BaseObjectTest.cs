using NUnit.Framework;

namespace BitButterCORE.V2.Testing
{
	[TestFixture]
	public class BaseObjectTest
	{
		[SetUp]
		public void Setup()
		{
			ObjectFactory.Instance.RemoveAll();
			ObjectFactory.Instance.ResetIDFountains();
		}

		[Test]
		public void TestInstantiateByConstructorThrowsException()
		{
			Assert.That(() => new DummyObject(1),
				Throws.InvalidOperationException.With.Message.EqualTo("BitButterCORE.V2.Testing.DummyObject should not be instantiated using its constructor, use ObjectFactory.Instance.Create<DummyObject>() instead."),
				"Instantiate object using constructor should throw exception");
		}

		[Test]
		public void TestInstantiateByObjectFactory()
		{
			Assert.That(() => ObjectFactory.Instance.Create<DummyObject>(), Throws.Nothing, "Should throw no exception");
			Assert.That(ObjectFactory.Instance.Create<DummyObject>().IsValid, Is.True, "Should return valid object reference");
			Assert.That(ObjectFactory.Instance.Create<DummyObject>().GetType(), Is.EqualTo(typeof(ObjectReference<DummyObject>)), "Returned object reference should be typed");
		}

		[Test]
		public void TestID()
		{
			var object1 = ObjectFactory.Instance.Create<DummyObject>().Object;
			Assert.That(object1.ID, Is.EqualTo(1), "object1 should have ID equal to 1");

			var object2 = ObjectFactory.Instance.Create<DummyObject>().Object;
			Assert.That(object2.ID, Is.EqualTo(2), "object2 should have ID equal to 2");
		}

		[Test]
		public void TestReference()
		{
			var baseObject = ObjectFactory.Instance.Create<DummyObject>().Object;
			Assert.That(baseObject.Reference.GetType(), Is.EqualTo(typeof(ObjectReference<DummyObject>)), "Reference of baseObject should be of correct type");
			Assert.That(baseObject.Reference.Object, Is.EqualTo(baseObject), "Reference should return baseObject");

			var derivedObject = ObjectFactory.Instance.Create<DerivedDummyObject>().Object;
			Assert.That(derivedObject.Reference.GetType(), Is.EqualTo(typeof(ObjectReference<DerivedDummyObject>)), "Reference of derivedObject should be of correct type");
			Assert.That(derivedObject.Reference.Object, Is.EqualTo(derivedObject), "Reference should return derivedObject");
		}

		[Test]
		public void TestToString()
		{
			var object1 = ObjectFactory.Instance.Create<DummyObject>().Object;
			Assert.That(object1.ToString(), Is.EqualTo("DummyObject1"));

			var object2 = ObjectFactory.Instance.Create<DummyObject>().Object;
			Assert.That(object2.ToString(), Is.EqualTo("DummyObject2"));

			var object3 = ObjectFactory.Instance.Create<DummyObject2>().Object;
			Assert.That(object3.ToString(), Is.EqualTo("DummyObject21"));
		}
	}
}
