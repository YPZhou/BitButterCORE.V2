using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace BitButterCORE.V2.Testing
{
	public class EventManagerTest
	{
		[SetUp]
		public void Setup()
		{
			var handlersProperty = typeof(EventManager).GetProperty("Handlers", BindingFlags.Instance | BindingFlags.NonPublic);
			var handlers = handlersProperty.GetMethod.Invoke(EventManager.Instance, null) as Dictionary<string, List<Tuple<object, MethodInfo>>>;
			handlers.Clear();

			DummyObjectWithEventHandler.TotalUpdateCalledCount = 0;
		}

		[Test]
		public void TestAddHandler()
		{
			var dummyObject1 = ObjectFactory.Instance.Create<DummyObjectWithEventHandler>();
			var dummyObject2 = ObjectFactory.Instance.Create<DummyObjectWithEventHandler>();

			var handlersProperty = typeof(EventManager).GetProperty("Handlers", BindingFlags.Instance | BindingFlags.NonPublic);
			var handlers = handlersProperty.GetMethod.Invoke(EventManager.Instance, null) as Dictionary<string, List<Tuple<object, MethodInfo>>>;

			Assert.That(handlers.ContainsKey("TestEvent"), Is.True, "Should contain handler for TestEvent");
			Assert.That(handlers["TestEvent"].Count, Is.EqualTo(2), "Should contain 2 handlers for TestEvent");
			Assert.That(handlers["TestEvent"].Exists(tuple => (IObjectReference<DummyObjectWithEventHandler>)tuple.Item1 == dummyObject1), Is.True, "Should contain handler targeting dummyObject1");
			Assert.That(handlers["TestEvent"].Exists(tuple => (IObjectReference<DummyObjectWithEventHandler>)tuple.Item1 == dummyObject2), Is.True, "Should contain handler targeting dummyObject2");
		}

		[Test]
		public void TestRaiseEvent()
		{
			var dummyObject1 = ObjectFactory.Instance.Create<DummyObjectWithEventHandler>();
			var dummyObject2 = ObjectFactory.Instance.Create<DummyObjectWithEventHandler>();

			EventManager.Instance.RaiseEvent("TestEvent");

			Assert.That(DummyObjectWithEventHandler.TotalUpdateCalledCount, Is.EqualTo(2), "TestEvent handler should be invoked twice, once for each dummyObject");
			Assert.That((dummyObject1.Object).UpdateCalledCount, Is.EqualTo(1), "TestEvent handler is invoked once for dummyObject1");
			Assert.That((dummyObject2.Object).UpdateCalledCount, Is.EqualTo(1), "TestEvent handler is invoked once for dummyObject2");

			ObjectFactory.Instance.Remove(dummyObject2);
			EventManager.Instance.RaiseEvent("TestEvent");

			Assert.That(DummyObjectWithEventHandler.TotalUpdateCalledCount, Is.EqualTo(3), "TestEvent handler should be invoked 3 times as handler for dummyPbject2 is not invoked");
			Assert.That((dummyObject1.Object).UpdateCalledCount, Is.EqualTo(2), "TestEvent handler is invoked twice for dummyObject1");
		}

		[Test]
		public void TestRaiseEventForMultipleHandlers()
		{
			var dummyObject1 = ObjectFactory.Instance.Create<DummyObjectWithEventHandler>();
			var dummyObject2 = ObjectFactory.Instance.Create<DummyObjectWithMultipleEventHandler>();

			EventManager.Instance.RaiseEvent("TestEvent", 1, 2);

			Assert.That(DummyObjectWithEventHandler.TotalUpdateCalledCount, Is.EqualTo(3), "TestEvent handler should be invoked 3 times");
			Assert.That((dummyObject1.Object).UpdateCalledCount, Is.EqualTo(1), "TestEvent handler is invoked once for dummyObject1");
			Assert.That((dummyObject2.Object).UpdateCalledCount, Is.EqualTo(2), "TestEvent handler is invoked twice for dummyObject2");
		}

		[Test]
		public void TestRaiseEventRemoveInvalidHandler()
		{
			var dummyObject1 = ObjectFactory.Instance.Create<DummyObjectWithEventHandler>();
			var dummyObject2 = ObjectFactory.Instance.Create<DummyObjectWithEventHandler>();

			var handlersProperty = typeof(EventManager).GetProperty("Handlers", BindingFlags.Instance | BindingFlags.NonPublic);
			var handlers = handlersProperty.GetMethod.Invoke(EventManager.Instance, null) as Dictionary<string, List<Tuple<object, MethodInfo>>>;

			Assert.That(handlers.ContainsKey("TestEvent"), Is.True, "Should contain handler for TestEvent");
			Assert.That(handlers["TestEvent"].Count, Is.EqualTo(2), "Should contain 2 handlers for TestEvent");
			Assert.That(handlers["TestEvent"].Exists(tuple => (IObjectReference<DummyObjectWithEventHandler>)tuple.Item1 == dummyObject1), Is.True, "Should contain handler targeting dummyObject1");
			Assert.That(handlers["TestEvent"].Exists(tuple => (IObjectReference<DummyObjectWithEventHandler>)tuple.Item1 == dummyObject2), Is.True, "Should contain handler targeting dummyObject2");

			ObjectFactory.Instance.Remove(dummyObject2);
			EventManager.Instance.RaiseEvent("TestEvent");

			Assert.That(handlers.ContainsKey("TestEvent"), Is.True, "Should contain handler for TestEvent");
			Assert.That(handlers["TestEvent"].Count, Is.EqualTo(1), "Should contain 1 handler for TestEvent, as handler for dummyObject2 is removed");
			Assert.That(handlers["TestEvent"].Exists(tuple => (IObjectReference<DummyObjectWithEventHandler>)tuple.Item1 == dummyObject1), Is.True, "Should contain handler targeting dummyObject1");
			Assert.That(handlers["TestEvent"].Exists(tuple => (IObjectReference<DummyObjectWithEventHandler>)tuple.Item1 == dummyObject2), Is.False, "Should not contain handler targeting dummyObject2");
		}

		[Test]
		public void TestRaiseEventForNonManagedObject()
		{
			var nonManagedObject = new NonManagedObjectWithEventHandler();

			Assert.That(nonManagedObject.UpdateCalledCount, Is.EqualTo(0), "pre-condition");

			EventManager.Instance.RaiseEvent("TestEvent", 1, "abc");

			Assert.That(nonManagedObject.UpdateCalledCount, Is.EqualTo(1), "TestEvent handler should be invoked once for non-managed object");
		}

		[Test]
		public void TestRaiseEventForNonManagedIEventHandler()
		{
			var nonManagedObjectImplementingIEventHandler = new NonManagedObjectWithEventHandlerImplementingIEventHandler();

			var handlersProperty = typeof(EventManager).GetProperty("Handlers", BindingFlags.Instance | BindingFlags.NonPublic);
			var handlers = handlersProperty.GetMethod.Invoke(EventManager.Instance, null) as Dictionary<string, List<Tuple<object, MethodInfo>>>;

			Assert.That(handlers["TestEvent"].Count, Is.EqualTo(1), "Should contain 1 handler for TestEvent");
			Assert.That(handlers["TestEvent"].Exists(tuple => (NonManagedObjectWithEventHandlerImplementingIEventHandler)tuple.Item1 == nonManagedObjectImplementingIEventHandler), Is.True, "Should contain handler targeting NonManagedObjectWithEventHandlerImplementingIEventHandler");

			nonManagedObjectImplementingIEventHandler.IsValidHandler = false;
			EventManager.Instance.RaiseEvent("TestEvent");

			Assert.That(handlers["TestEvent"].Count, Is.EqualTo(0), "Should contain no handler for TestEvent");
		}
	}
}
