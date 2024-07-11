using System;
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

			var objectReference3 = ObjectFactory.Instance.Create<DerivedDummyObject>();
			Assert.That(objectReference3.Type, Is.EqualTo(typeof(DerivedDummyObject)));
		}

		[Test]
		public void TestTypeHeapAllocation()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();

			var allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
			_ = objectReference.Type;

			Assert.That(GC.GetAllocatedBytesForCurrentThread() - allocatedBytes, Is.EqualTo(0), "Access Type should cause no heap allocation");
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
		public void TestIDHeapAllocation()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();

			var allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
			_ = objectReference.ID;

			Assert.That(GC.GetAllocatedBytesForCurrentThread() - allocatedBytes, Is.EqualTo(0), "Access ID should cause no heap allocation");
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
		public void TestObjectHeapAllocation()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();

			var allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
			_ = objectReference.Object;

			Assert.That(GC.GetAllocatedBytesForCurrentThread() - allocatedBytes, Is.EqualTo(0), "Access Object should cause no heap allocation");
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
		public void TestIsValidHeapAllocation()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();

			var allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
			_ = objectReference.IsValid;

			Assert.That(GC.GetAllocatedBytesForCurrentThread() - allocatedBytes, Is.EqualTo(0), "Access IsValid should cause no heap allocation");
		}

		[Test]
		public void TestToString()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference.ToString(), Is.EqualTo("DummyObject1"));

			ObjectFactory.Instance.Remove(objectReference);
			Assert.That(objectReference.ToString(), Is.EqualTo("Invalid reference to BitButterCORE.V2.Testing.DummyObject"));
		}
	}
}
