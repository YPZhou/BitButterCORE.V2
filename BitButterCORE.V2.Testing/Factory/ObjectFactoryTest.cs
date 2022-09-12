using System.Linq;
using NUnit.Framework;

namespace BitButterCORE.V2.Testing
{
	public class ObjectFactoryTest
	{
		[SetUp]
		public void Setup()
		{
			ObjectFactory.Instance.RemoveAll();
			ObjectFactory.Instance.ResetIDFountains();
			ObjectFactory.Instance.ClearChanges();
		}

		[Test]
		public void TestCreate()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference, Is.Not.Null
				.With.Property("Type").EqualTo(typeof(DummyObject))
				.With.Property("ID").EqualTo(1)
				.With.Property("Object").Not.Null);

			Assert.That(() => ObjectFactory.Instance.Create<DummyObject>(1),
				Throws.InvalidOperationException.With.Message.EqualTo("Instantiation of BitButterCORE.V2.Testing.DummyObject failed as no matching constructor found for parameters (1)."),
				"Should throw exception when no matching constructor found");
		}

		[Test]
		public void TestCreateWithIncrementalID()
		{
			var objectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference1.ID, Is.EqualTo(1), "pre-condition");

			ObjectFactory.Instance.Remove(objectReference1);

			var objectReference2 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference2.ID, Is.EqualTo(2), "Object ID should be incremental");
		}

		[Test]
		public void TestCreateWithDefaultParameter()
		{
			var objectReferenceWithDefaultParam = ObjectFactory.Instance.Create<DummyObjectWithDefaultParameterInConstructor>();
			Assert.That(objectReferenceWithDefaultParam.GetObject<DummyObjectWithDefaultParameterInConstructor>().DefaultParam, Is.EqualTo(1), "DefaultParam should be default value 1");

			var objectReferenceWithExplicitParam = ObjectFactory.Instance.Create<DummyObjectWithDefaultParameterInConstructor>(2);
			Assert.That(objectReferenceWithExplicitParam.GetObject<DummyObjectWithDefaultParameterInConstructor>().DefaultParam, Is.EqualTo(2), "DefaultParam should be explicit value 2");
		}

		[Test]
		public void TestCreateWithConstructorOverloading()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObjectWithMultipleConstructor>(2);
			Assert.That(objectReference.IsValid, Is.True, "Reference should be valid");

			Assert.That(() => ObjectFactory.Instance.Create<DummyObjectWithMultipleConstructor>(),
				Throws.InvalidOperationException.With.Message.EqualTo("Instantiation of BitButterCORE.V2.Testing.DummyObjectWithMultipleConstructor failed as multiple matching constructors found for parameters ()."),
				"Should throw exception when multiple matching constructors found");
		}

		[Test]
		public void TestRemove()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference.IsValid, Is.True, "pre-condition");

			ObjectFactory.Instance.Remove(objectReference);
			Assert.That(objectReference.IsValid, Is.False, "Reference should be no longer valid");
			Assert.That(objectReference.Object, Is.Null, "Reference should no longer point to a valid object");
		}

		[Test]
		public void TestRemoveAllByObjectType()
		{
			var dummy1Reference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(dummy1Reference.IsValid, Is.True, "pre-condition");

			var dummy2Reference = ObjectFactory.Instance.Create<DummyObject2>();
			Assert.That(dummy2Reference.IsValid, Is.True, "pre-condition");

			ObjectFactory.Instance.RemoveAll<DummyObject2>();

			Assert.That(dummy1Reference.IsValid, Is.True, "DummyObject should not be removed");
			Assert.That(dummy2Reference.IsValid, Is.False, "DummyObject2 should be removed");
		}

		[Test]
		public void TestRemoveAll()
		{
			var dummy1Reference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(dummy1Reference.IsValid, Is.True, "pre-condition");

			var dummy2Reference = ObjectFactory.Instance.Create<DummyObject2>();
			Assert.That(dummy2Reference.IsValid, Is.True, "pre-condition");

			ObjectFactory.Instance.RemoveAll();

			Assert.That(dummy1Reference.IsValid, Is.False, "DummyObject should be removed");
			Assert.That(dummy2Reference.IsValid, Is.False, "DummyObject2 should be removed");
		}

		[Test]
		public void TestResetIDFountain()
		{
			var objectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference1.ID, Is.EqualTo(1), "pre-condition");

			ObjectFactory.Instance.RemoveAll<DummyObject>();
			ObjectFactory.Instance.ResetIDFountain<DummyObject>();

			var objectReference2 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference2.ID, Is.EqualTo(1), "DummyObject ID fountain should be reset to 1");
		}

		[Test]
		public void TestResetIDFountains()
		{
			var dummyObjectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(dummyObjectReference1.ID, Is.EqualTo(1), "pre-condition");

			var dummyObject2Reference1 = ObjectFactory.Instance.Create<DummyObject2>();
			Assert.That(dummyObject2Reference1.ID, Is.EqualTo(1), "pre-condition");

			ObjectFactory.Instance.RemoveAll();
			ObjectFactory.Instance.ResetIDFountains();

			var dummyObjectReference2 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(dummyObjectReference2.ID, Is.EqualTo(1), "DummyObject ID fountain should be reset to 1");

			var dummyObject2Reference2 = ObjectFactory.Instance.Create<DummyObject2>();
			Assert.That(dummyObject2Reference2.ID, Is.EqualTo(1), "DummyObject2 ID fountain should be reset to 1");
		}

		[Test]
		public void TestHasChangesWhenCreateObject()
		{
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "pre-condition");

			ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after create object");
		}

		[Test]
		public void TestHasChangesWhenRemoveObject()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "pre-condition");

			ObjectFactory.Instance.Remove(objectReference);
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after remove object");
		}

		[Test]
		public void TestClearChanges()
		{
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "pre-condition");

			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after create object");

			ObjectFactory.Instance.Remove(objectReference);
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after remove object");

			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should not have changes after clear changes");
		}

		[Test]
		public void TestGetAddedObjects()
		{
			Assert.That(ObjectFactory.Instance.GetAddedObjects().Count(), Is.EqualTo(0), "pre-condition");

			var objectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(ObjectFactory.Instance.GetAddedObjects().Count(), Is.EqualTo(1), "Should get 1 added object");
			Assert.That(ObjectFactory.Instance.GetAddedObjects().Where(x => x.Type == typeof(DummyObject)).Single(), Is.EqualTo(objectReference1), "Should contain object justed added");

			var objectReference2 = ObjectFactory.Instance.Create<DummyObject2>();
			Assert.That(ObjectFactory.Instance.GetAddedObjects().Count(), Is.EqualTo(2), "Should get 2 added objects");
			Assert.That(ObjectFactory.Instance.GetAddedObjects().Where(x => x.Type == typeof(DummyObject2)).Single(), Is.EqualTo(objectReference2), "Should contain object justed added");

			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.GetAddedObjects().Count(), Is.EqualTo(0), "Should get no added object after clear changes");
		}

		[Test]
		public void TestGetRemovedObjects()
		{
			var objectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			var objectReference2 = ObjectFactory.Instance.Create<DummyObject2>();

			Assert.That(ObjectFactory.Instance.GetRemovedObjects().Count(), Is.EqualTo(0), "pre-condition");

			ObjectFactory.Instance.Remove(objectReference1);
			Assert.That(ObjectFactory.Instance.GetRemovedObjects().Count(), Is.EqualTo(1), "Should get 1 removed object");
			Assert.That(ObjectFactory.Instance.GetRemovedObjects().Where(x => x.Type == typeof(DummyObject)).Single(), Is.EqualTo(objectReference1), "Should contain object justed removed");

			ObjectFactory.Instance.Remove(objectReference2);
			Assert.That(ObjectFactory.Instance.GetRemovedObjects().Count(), Is.EqualTo(2), "Should get 2 removed objects");
			Assert.That(ObjectFactory.Instance.GetRemovedObjects().Where(x => x.Type == typeof(DummyObject2)).Single(), Is.EqualTo(objectReference2), "Should contain object justed removed");

			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.GetRemovedObjects().Count(), Is.EqualTo(0), "Should get no removed object after clear changes");
		}

		[Test]
		public void TestQuery()
		{
			var dummyObject1 = ObjectFactory.Instance.Create<DummyObject>();
			var dummyObject2 = ObjectFactory.Instance.Create<DummyObject>();

			Assert.That(ObjectFactory.Instance.Query<DummyObject>().Count(), Is.EqualTo(2), "Should find 2 dummy objects");
			Assert.That(ObjectFactory.Instance.Query<DummyObject>(dummyObject => dummyObject.Name == "DummyObject2").Count(), Is.EqualTo(1), "Should find 1 dummy object with correct name");
			Assert.That(ObjectFactory.Instance.Query<DummyObject>(dummyObject => dummyObject.Name == "InvalidName").Count(), Is.Zero, "Should find no dummy object with invalid name");
		}

		[Test]
		public void TestQueryByType()
		{
			var dummyObject1 = ObjectFactory.Instance.Create<DummyObject>();
			var dummyObject2 = ObjectFactory.Instance.Create<DummyObject2>();

			Assert.That(ObjectFactory.Instance.Query<DummyObject>().Count(), Is.EqualTo(1), "Should find 1 dummy object");
			Assert.That(ObjectFactory.Instance.Query<DummyObject2>().Count(), Is.EqualTo(1), "Should find 1 dummy object 2");
			Assert.That(ObjectFactory.Instance.Query<BaseObject>().Count(), Is.EqualTo(2), "Should find both dummy objects as they all inherit from BaseObject");
		}
	}
}
