namespace BitButterCORE.V2.Testing
{
	public class DummyObject : BaseObject
	{
		public DummyObject(uint id)
			: base(id)
		{
		}

		public string Name => string.Format("DummyObject{0}", ID);
	}

	public class DummyObject2 : BaseObject
	{
		public DummyObject2(uint id)
			: base(id)
		{
		}

		public string Name => "DummyObject2";
	}

	public class DummyObjectWithDefaultParameterInConstructor : BaseObject
	{
		public DummyObjectWithDefaultParameterInConstructor(uint id, int defaultParam = 1)
			: base(id)
		{
			DefaultParam = defaultParam;
		}

		public int DefaultParam { get; }
	}

	public class DummyObjectWithMultipleConstructor : BaseObject
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

	public class DummyObjectWithEventHandler : BaseObject
	{
		public DummyObjectWithEventHandler(uint id)
			: base(id)
		{
			EventManager.Instance.AddHandler("TestEvent", (_) => Update());
		}

		void Update()
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

	public class NonManagedObjectWithEventHandler
	{
		public NonManagedObjectWithEventHandler()
		{
			EventManager.Instance.AddHandler("TestEvent", (_) => Update());
		}

		void Update()
		{
			UpdateCalledCount += 1;
		}

		public int UpdateCalledCount { get; private set; }
	}
}
