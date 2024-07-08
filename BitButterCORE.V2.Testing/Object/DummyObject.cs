namespace BitButterCORE.V2.Testing
{
	public class DummyObject : BaseObject<DummyObject>
	{
		public DummyObject(uint id)
			: base(id)
		{
		}

		public string Name => string.Format("DummyObject{0}", ID);
	}

	public class DummyObject2 : BaseObject<DummyObject2>
	{
		public DummyObject2(uint id)
			: base(id)
		{
		}

		public string Name => "DummyObject2";
	}

	public class DerivedDummyObject : DummyObject
	{
		public DerivedDummyObject(uint id)
			: base(id)
		{
		}
	}

	public class DummyObjectWithDefaultParameterInConstructor : BaseObject<DummyObjectWithDefaultParameterInConstructor>
	{
		public DummyObjectWithDefaultParameterInConstructor(uint id, int defaultParam = 1)
			: base(id)
		{
			DefaultParam = defaultParam;
		}

		public int DefaultParam { get; }
	}

	public class DummyObjectWithMultipleConstructor : BaseObject<DummyObjectWithMultipleConstructor>
	{
		public DummyObjectWithMultipleConstructor(uint id)
			: base(id)
		{
		}

		public DummyObjectWithMultipleConstructor(uint id, int count = 0)
			: base(id)
		{
		}
	}

	public class DummyObjectWithNullableParameterInConstructor : BaseObject<DummyObjectWithNullableParameterInConstructor>
	{
		public DummyObjectWithNullableParameterInConstructor(uint id, object obj)
			: base(id)
		{
		}
	}

	public class DummyObjectWithEventHandler : BaseObject<DummyObjectWithEventHandler>
	{
		public DummyObjectWithEventHandler(uint id)
			: base(id)
		{
			EventManager.Instance.AddHandler("TestEvent", Update);
		}

		public void Update(params object[] args)
		{
			UpdateCalledCount += 1;
			TotalUpdateCalledCount += 1;
		}

		public int UpdateCalledCount { get; protected set; }

		public static int TotalUpdateCalledCount = 0;
	}

	public class DummyObjectWithMultipleEventHandler : DummyObjectWithEventHandler
	{
		public DummyObjectWithMultipleEventHandler(uint id)
			: base(id)
		{
			EventManager.Instance.AddHandler("TestEvent", (args) => Update((int)args[0], (int)args[1]));
		}

		void Update(int x, int y)
		{
			UpdateCalledCount += 1;
			TotalUpdateCalledCount += 1;
		}
	}

	public class DummyObjectAccessObjectPropertyByReferenceInOnObjectCreated : DummyObject
	{
		public DummyObjectAccessObjectPropertyByReferenceInOnObjectCreated(uint id)
			: base(id)
		{
		}

		protected override void OnObjectCreatedCore()
		{
			_ = Reference.Object.ID;
		}
	}

	public class DummyObjectWithPropertySetter : DummyObject
	{
		public DummyObjectWithPropertySetter(uint id)
			: base(id)
		{
		}

		public string StringProperty { get; private set; }
		public float NumberProperty { get; private set; }
		public bool BoolProperty { get; private set; }
	}

	public class NonManagedObjectWithEventHandler
	{
		public NonManagedObjectWithEventHandler()
		{
			EventManager.Instance.AddHandler("TestEvent", (_) => OnEvent());
		}

		void OnEvent() { }
	}

	public class NonManagedObjectWithEventHandlerImplementingIEventHandler : NonManagedObjectWithEventHandler, IEventHandler
	{
		public bool IsValidHandler { get; set; }
	}
}
