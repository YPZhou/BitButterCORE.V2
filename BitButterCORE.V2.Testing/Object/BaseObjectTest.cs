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
