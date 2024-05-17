using System;
using System.Collections.Generic;
using System.Reflection;

namespace BitButterCORE.V2
{
	public interface IEventHandler
	{
		bool IsValidHandler { get; }
	}

	public class EventManager : BaseSingleton<EventManager>
	{
		public void RaiseEvent(string eventName, params object[] eventArgs)
		{
			if (Handlers.ContainsKey(eventName))
			{
				foreach (var handler in Handlers[eventName].ToArray())
				{
					var target = handler.Item1;
					var args = eventArgs.Length > 0 ? new object[] { eventArgs } : new object[] { null };
					if (target is IObjectReference reference)
					{
						if (reference.IsValidHandler)
						{
							handler.Item2.Invoke(reference.Object, args);
						}
					}
					else if (target is IEventHandler eventHandler)
					{
						if (eventHandler.IsValidHandler)
						{
							handler.Item2.Invoke(target, args);
						}
					}
				}

				Handlers[eventName].RemoveAll(handlerTuple => ShouldRemoveHandler(handlerTuple));
			}
		}

		bool ShouldRemoveHandler(Tuple<object, MethodInfo> handlerTuple)
		{
			var result = true;
			if (handlerTuple.Item1 is IEventHandler handler)
			{
				result = !handler.IsValidHandler;
			}

			return result;
		}

		public void AddHandler(string eventName, EventHandler handlerToAdd)
		{
			if (handlerToAdd.Target is IBaseObject || handlerToAdd.Target is IEventHandler)
			{
				if (!Handlers.ContainsKey(eventName))
				{
					Handlers.Add(eventName, new List<Tuple<object, MethodInfo>>());
				}

				var target = handlerToAdd.Target is IBaseObject obj ? obj.Reference : handlerToAdd.Target;
				var method = handlerToAdd.Method;
				var handlerTuple = Tuple.Create(target, method);

				Handlers[eventName].RemoveAll(tuple => tuple.Equals(handlerTuple));
				Handlers[eventName].Add(handlerTuple);
			}
			else
			{
				throw new InvalidOperationException(string.Format("Failed to register event handler for {0}, please make sure it implements interface {1}.", handlerToAdd.Target.GetType().FullName, typeof(IEventHandler).FullName));
			}
		}

		Dictionary<string, List<Tuple<object, MethodInfo>>> Handlers => handlers ?? (handlers = new Dictionary<string, List<Tuple<object, MethodInfo>>>());
		Dictionary<string, List<Tuple<object, MethodInfo>>> handlers;

		public delegate void EventHandler(params object[] eventArgs);
	}
}
