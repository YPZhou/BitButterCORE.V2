using NUnit.Framework;

namespace BitButterCORE.V2.Testing
{
	[TestFixture]
	public class ObjectReferenceExtensionsTest
	{
		[SetUp]
		public void Setup()
		{
			ObjectFactory.Instance.RemoveAll();
			ObjectFactory.Instance.ResetIDFountains();
		}

		[Test]
		public void TestIsValid()
		{
			var nullReference = default(IObjectReference);
			Assert.That(nullReference.IsValid(), Is.EqualTo(false), "Null reference is not valid");

			var invalidReference = ObjectFactory.Instance.Create<DummyObject>();
			ObjectFactory.Instance.Remove(invalidReference);
			Assert.That(invalidReference.IsValid(), Is.EqualTo(false), "Reference without object is not valid");

			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference.IsValid(), Is.EqualTo(true), "Reference with object is valid");
		}
	}
}
