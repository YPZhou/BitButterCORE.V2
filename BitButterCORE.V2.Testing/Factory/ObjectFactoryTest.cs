using System;
using System.Linq;
using System.Reflection;
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
			var methodInfo = typeof(ObjectFactory).GetMethod("GetObjectIDFountain", BindingFlags.Instance | BindingFlags.NonPublic);
			var idFountain = methodInfo.Invoke(ObjectFactory.Instance, new object[] { typeof(DummyObject) }) as ObjectIDFountain;
			_ = idFountain.NextID;
			_ = idFountain.NextID;

			var objectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference1.ID, Is.EqualTo(3), "pre-condition");

			ObjectFactory.Instance.Remove(objectReference1);

			var objectReference2 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference2.ID, Is.EqualTo(4), "Object ID should be incremental");
		}

		[Test]
		public void TestCreateWithDuplicateIDThrowsException()
		{
			ObjectFactory.Instance.Create<DummyObject>();

			var methodInfo = typeof(ObjectFactory).GetMethod("Create", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(() => methodInfo.Invoke(ObjectFactory.Instance, new object[] { typeof(DummyObject), 1u, Array.Empty<object>() }),
				Throws.TargetInvocationException.With.InnerException.With.Message.EqualTo("Instantiation of BitButterCORE.V2.Testing.DummyObject failed because of duplicate ID 1"),
				"Should throw exception when create object with duplicate ID");
		}

		[Test]
		public void TestCreateWithIDUpdateIDFountain()
		{
			var methodInfo = typeof(ObjectFactory).GetMethod("Create", BindingFlags.Instance | BindingFlags.NonPublic);
			methodInfo.Invoke(ObjectFactory.Instance, new object[] { typeof(DummyObject), 1u, Array.Empty<object>() });
			methodInfo.Invoke(ObjectFactory.Instance, new object[] { typeof(DummyObject), 3u, Array.Empty<object>() });

			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(objectReference.ID, Is.EqualTo(4), "Should create new object with next available ID 4");
		}

		[Test]
		public void TestCreateWithDefaultParameter()
		{
			var objectReferenceWithDefaultParam = ObjectFactory.Instance.Create<DummyObjectWithDefaultParameterInConstructor>();
			Assert.That(objectReferenceWithDefaultParam.Object.DefaultParam, Is.EqualTo(1), "DefaultParam should be default value 1");

			var objectReferenceWithExplicitParam = ObjectFactory.Instance.Create<DummyObjectWithDefaultParameterInConstructor>(2);
			Assert.That(objectReferenceWithExplicitParam.Object.DefaultParam, Is.EqualTo(2), "DefaultParam should be explicit value 2");
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
		public void TestCreateWithNullParameter()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObjectWithNullableParameterInConstructor>(new object[] { null });
			Assert.That(objectReference.IsValid, Is.True, "Create object with null parameter should not fail");
		}

		[Test]
		public void TestCreateWithTypeParameter()
		{
			var objectReference = ObjectFactory.Instance.Create(typeof(DummyObject));
			Assert.That(objectReference.IsValid, Is.True, "Create with type parameter should work for empty parameter list");

			objectReference = ObjectFactory.Instance.Create(typeof(DummyObjectWithMultipleConstructor), 2);
			Assert.That(objectReference.IsValid, Is.True, "Create with type parameter should work for explicitly given parameter");

			objectReference = ObjectFactory.Instance.Create(typeof(DummyObjectWithDefaultParameterInConstructor));
			Assert.That(objectReference.IsValid, Is.True, "Create with type parameter should work for default parameter");
		}

		[Test]
		public void TestCreateFromTemplate()
		{
			ObjectTemplateManager.Instance.LoadObjectTemplate("Object/TestJsonFiles/DummyObjectTemplate.json");
			
			var objectReference1 = ObjectFactory.Instance.CreateFromTemplate<DummyObjectWithPropertySetter>("DummyObject1");
			Assert.That(objectReference1, Is.Not.Null, "Object created");
			Assert.That(objectReference1.Object.StringProperty, Is.EqualTo("TestString1"), "String property set");
			Assert.That(objectReference1.Object.IntProperty, Is.EqualTo(123), "Int property set");
			Assert.That(objectReference1.Object.FloatProperty, Is.EqualTo(1.23f), "Float property set");
			Assert.That(objectReference1.Object.BoolProperty, Is.EqualTo(true), "Bool property set");
			Assert.That(objectReference1.Object.ListProperty, Is.EquivalentTo(new object[] { "123", 123, 1.23f, true }));
			Assert.That(objectReference1.Object.AdditionalProperty, Is.EqualTo("DummyObject1"), "Additional property set");

			var objectReference2 = ObjectFactory.Instance.CreateFromTemplate<DummyObjectWithPropertySetter>("DummyObject2");
			Assert.That(objectReference2, Is.Not.Null, "Object created");
			Assert.That(objectReference2.Object.StringProperty, Is.EqualTo("TestString2"), "String property set");
			Assert.That(objectReference2.Object.IntProperty, Is.EqualTo(321), "Int property set");
			Assert.That(objectReference2.Object.FloatProperty, Is.EqualTo(3.21f), "Float property set");
			Assert.That(objectReference2.Object.BoolProperty, Is.EqualTo(false), "Bool property set");
			Assert.That(objectReference2.Object.ListProperty, Is.EquivalentTo(new object[] { "456", 456, 4.56f, false }));
			Assert.That(objectReference2.Object.AdditionalProperty, Is.EqualTo("DummyObject2"), "Additional property set");
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
		public void TestHasChangesWhenRemoveJustAddedObject()
		{
			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "pre-condition");

			ObjectFactory.Instance.Remove(objectReference);
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "Removing just added object should cause no changes in ObjectFactory");
		}

		[Test]
		public void TestHasChangesWhenRemoveAll()
		{
			ObjectFactory.Instance.Create<DummyObject>();
			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "pre-condition");

			ObjectFactory.Instance.RemoveAll();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after remove all objects");

			ObjectFactory.Instance.ClearChanges();
			ObjectFactory.Instance.RemoveAll();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should not have changes when RemoveAll deletes no objects");

			ObjectFactory.Instance.Create<DummyObject>();
			ObjectFactory.Instance.RemoveAll();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should not have changes when RemoveAll only deletes just added object");
		}

		[Test]
		public void TestHasChangesWhenRemoveAllByObjectType()
		{
			ObjectFactory.Instance.Create<DummyObject>();
			ObjectFactory.Instance.Create<DummyObject>();
			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "pre-condition");

			ObjectFactory.Instance.RemoveAll<DummyObject>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after remove all DummyObjects");

			ObjectFactory.Instance.ClearChanges();
			ObjectFactory.Instance.RemoveAll<DummyObject2>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should not have changes when no DummyObject2 is removed");

			ObjectFactory.Instance.Create<DummyObject2>();
			ObjectFactory.Instance.RemoveAll<DummyObject2>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should not have changes when RemoveAll only deletes just added object");
		}

		[Test]
		public void TestClearChanges()
		{
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "pre-condition");

			var objectReference = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after create object");

			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should not have changes after clear changes");

			ObjectFactory.Instance.Remove(objectReference);
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should have changes after remove object");

			ObjectFactory.Instance.ClearChanges();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should not have changes after clear changes");
		}

		[Test]
		public void TestClearChangesForObject()
		{
			var objectReference1 = ObjectFactory.Instance.Create<DummyObject>();
			var objectReference2 = ObjectFactory.Instance.Create<DummyObject>();
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "pre-condition");
			Assert.That(ObjectFactory.Instance.GetAddedObjects(), Does.Contain(objectReference1), "pre-condition");
			Assert.That(ObjectFactory.Instance.GetAddedObjects(), Does.Contain(objectReference2), "pre-condition");

			ObjectFactory.Instance.ClearChangesForObject(objectReference1);
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(true), "ObjectFactory should still has change after remove changes for first object");
			Assert.That(ObjectFactory.Instance.GetAddedObjects(), Does.Not.Contain(objectReference1), "ObjectFactory should have no record for adding first object");
			Assert.That(ObjectFactory.Instance.GetAddedObjects(), Does.Contain(objectReference2), "ObjectFactory should have record for adding second object");

			ObjectFactory.Instance.ClearChangesForObject(objectReference2);
			Assert.That(ObjectFactory.Instance.HasChanges, Is.EqualTo(false), "ObjectFactory should have no changes after remove changes for both objects");
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
			ObjectFactory.Instance.ClearChanges();

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
			Assert.That(ObjectFactory.Instance.Query<IBaseObject>().Count(), Is.EqualTo(2), "Should find both dummy objects as they all implement IBaseObject");
		}

		[Test]
		public void TestQueryFirst()
		{
			var dummyObject1 = ObjectFactory.Instance.Create<DummyObject>();
			var dummyObject2 = ObjectFactory.Instance.Create<DummyObject>();

			Assert.That(ObjectFactory.Instance.QueryFirst<DummyObject>().ID, Is.EqualTo(dummyObject1.ID), "Should find 1st dummy object");
			Assert.That(ObjectFactory.Instance.QueryFirst<DummyObject>(obj => obj.ID == dummyObject2.ID).ID, Is.EqualTo(dummyObject2.ID), "Should find 2nd dummy object when query with predicate");
			Assert.That(ObjectFactory.Instance.QueryFirst<DummyObject2>(), Is.EqualTo(default(IObjectReference)), "Should return default reference when no object is found");
		}
	}
}
