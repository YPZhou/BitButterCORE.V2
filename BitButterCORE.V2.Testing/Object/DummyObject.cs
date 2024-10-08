﻿using System.Collections.Generic;

namespace BitButterCORE.V2.Testing
{
	public class DummyObject : BaseObject<DummyObject>
	{
		public DummyObject(uint id)
			: base(id)
		{
		}

		public string Name => string.Format("DummyObject{0}", ID);

		protected override void OnObjectCreatedCore()
		{
			OnObjectCreatedCalled = true;
		}

		protected override void OnObjectLoadedCore()
		{
			OnObjectLoadedCalled = true;
		}

		public bool OnObjectCreatedCalled { get; private set; } = false;
		public bool OnObjectLoadedCalled { get; private set; } = false;
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

	public class DummyObjectWithPropertySetter : DummyObject, ITemplateObject
	{
		public DummyObjectWithPropertySetter(uint id)
			: base(id)
		{
		}

		public string StringProperty { get; private set; }
		public int IntProperty { get; private set; }
		public float FloatProperty { get; private set; }
		public bool BoolProperty { get; private set; }
		public List<object> ListProperty => listProperty ??= new List<object>();
		List<object> listProperty;

		public string AdditionalProperty { get; private set; }

		void ITemplateObject.SetupObjectFromTemplate(string templateName, Dictionary<string, object> _)
		{
			AdditionalProperty = templateName;
		}
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

	public class NonManagedObjectWithPropertySetter : ITemplateObject
	{
		public string StringProperty { get; private set; }
		public int IntProperty { get; private set; }
		public float FloatProperty { get; private set; }
		public bool BoolProperty { get; private set; }
		public List<object> ListProperty => listProperty ??= new List<object>();
		List<object> listProperty;

		public string ReadOnlyProperty => readOnlyField;
		string readOnlyField;

		public string AdditionalProperty { get; private set; }

		void ITemplateObject.SetupObjectFromTemplate(string templateName, Dictionary<string, object> template)
		{
			AdditionalProperty = templateName;

			if (template.ContainsKey("ReadOnlyProperty"))
			{
				readOnlyField = (string)template["ReadOnlyProperty"];
			}
		}
	}

	public enum DummyEnum
	{
		ENUM1,
		ENUM2,
	}

	public class SerializableObject : BaseObject<SerializableObject>
	{
		public SerializableObject(uint id, int intValue, float floatValue, string stringValue, bool boolValue)
			: base(id)
		{
			IntValue = intValue;
			FloatValue = floatValue;
			StringValue = stringValue;
			BoolValue = boolValue;
		}

		[SerializeProperty(ctorParameterOrder: 1)]
		public int IntValue { get; }

		[SerializeProperty(ctorParameterOrder: 2)]
		public float FloatValue { get; }

		[SerializeProperty(ctorParameterOrder: 3)]
		public string StringValue { get; }

		[SerializeProperty(ctorParameterOrder: 4)]
		public bool BoolValue { get; }

		[SerializeProperty]
		public int IntValue2 { get; set; }

		[SerializeProperty]
		public DummyEnum DummyEnum { get; set; }

		[SerializeProperty]
		public int ReadOnlyValue => 100;

		public int PropertySetOnObjectLoaded { get; private set; }

		protected override void OnObjectLoadedCore()
		{
			PropertySetOnObjectLoaded = IntValue2;
		}
	}

	public class String : BaseObject<String>
	{
		public String(uint id, IObjectReference<DummyObject2> dummyObject2)
			: base(id)
		{
			DummyObject2 = dummyObject2;
		}

		[SerializeProperty]
		public IObjectReference<DummyObject> DummyObject { get; set; }

		[SerializeProperty(ctorParameterOrder: 1)]
		public IObjectReference<DummyObject2> DummyObject2 { get; }
	}

	public class Object : BaseObject<Object>
	{
		public Object(uint id)
			: base(id)
		{
		}

		[SerializeProperty]
		public IObjectReference<String> SerializableObject { get; set; }
	}
}
