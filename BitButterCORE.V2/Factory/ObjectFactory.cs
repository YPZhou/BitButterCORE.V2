using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BitButterCORE.V2
{
	public class ObjectFactory : BaseSingleton<ObjectFactory>
	{
		public ObjectReference Create<TObject>(params object[] args) where TObject : BaseObject
		{
			return Create(typeof(TObject), args);
		}

		public ObjectReference Create(Type objectType, params object[] args)
		{
			var result = default(ObjectReference);
			if (!objectType.IsAbstract)
			{
				var newID = GetObjectIDFountain(objectType).NextID;
				var constructors = objectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
				var matchingConstructors = FindMatchingConstructors(constructors, args);
				if (matchingConstructors.Count() == 1)
				{
					var constructor = matchingConstructors.Single();
					var inputParameters = new object[] { newID }.Concat(args);
					if (constructor.GetParameters().Length > inputParameters.Count())
					{
						inputParameters = inputParameters.Concat(constructor.GetParameters().Skip(inputParameters.Count()).Select(x => x.DefaultValue));
					}

					var obj = (BaseObject)constructor.Invoke(inputParameters.ToArray());
					AddObjectToFactory(obj);
					result = obj.Reference;
					UpdateFactoryChangeRecordsForAddObject(result);
				}
				else if (matchingConstructors.Count() > 1)
				{
					throw new InvalidOperationException(string.Format("Instantiation of {0} failed as multiple matching constructors found for parameters ({1}).", objectType.FullName, string.Join(", ", args)));
				}
				else
				{
					throw new InvalidOperationException(string.Format("Instantiation of {0} failed as no matching constructor found for parameters ({1}).", objectType.FullName, string.Join(", ", args)));
				}
			}
			return result;
		}

		IEnumerable<ConstructorInfo> FindMatchingConstructors(IEnumerable<ConstructorInfo> constructors, IEnumerable<object> inputParameters)
		{
			return constructors.Where(constructor =>
			{
				var result = true;
				var parameters = constructor.GetParameters();
				if (inputParameters.Count() <= parameters.Length - 1)
				{
					if (parameters.Length > 1)
					{
						for (var i = 1; i < parameters.Length; i++)
						{
							var parameter = parameters[i];
							var inputParameterIndex = i - 1;
							if (inputParameterIndex < inputParameters.Count())
							{
								var inputParameter = inputParameters.ElementAt(inputParameterIndex);
								var parameterType = parameter.ParameterType;
								if (!parameterType.IsAssignableFrom(inputParameter.GetType()))
								{
									result = false;
								}
							}
							else
							{
								if (!parameter.HasDefaultValue)
								{
									result = false;
									break;
								}
							}
						}
					}
				}
				else
				{
					result = false;
				}
				return result;
			});
		}

		public void Remove(ObjectReference reference)
		{
			var objectToRemove = GetObjectByReference(reference);
			if (objectToRemove != null)
			{
				var objectType = objectToRemove.GetType();
				if (Factory.ContainsKey(objectType) && Factory[objectType].ContainsKey(objectToRemove.ID))
				{
					Factory[objectType].Remove(objectToRemove.ID);
					UpdateFactoryChangeRecordsForRemoveObject(reference);
				}
			}
		}

		public void RemoveAll<TObject>()
		{
			var objectType = typeof(TObject);
			if (Factory.ContainsKey(objectType))
			{
				foreach (var reference in Factory[objectType].Values.Select(obj => obj.Reference))
				{
					UpdateFactoryChangeRecordsForRemoveObject(reference);
				}

				Factory[objectType].Clear();
			}
		}

		public void RemoveAll()
		{
			foreach (var reference in Factory.SelectMany(pair => pair.Value.Values).Select(obj => obj.Reference))
			{
				UpdateFactoryChangeRecordsForRemoveObject(reference);
			}

			Factory.Clear();
		}

		public void ResetIDFountain<TObject>()
		{
			var objectType = typeof(TObject);
			if (IDFountains.ContainsKey(objectType))
			{
				IDFountains[objectType].Reset();
			}
		}

		public void ResetIDFountains()
		{
			IDFountains.Clear();
		}

		public bool HasChanges => AddedObjects.Any() || RemovedObjects.Any();

		public void ClearChanges()
		{
			AddedObjects.Clear();
			RemovedObjects.Clear();
		}

		void UpdateFactoryChangeRecordsForAddObject(ObjectReference reference)
		{
			AddObjectReferenceToFactoryChangeRecords(reference, AddedObjects);
		}

		void UpdateFactoryChangeRecordsForRemoveObject(ObjectReference reference)
		{
			var objectType = reference.Type;
			if (AddedObjects.ContainsKey(objectType) && addedObjects[objectType].Contains(reference))
			{
				AddedObjects[objectType].Remove(reference);
				if (AddedObjects[objectType].Count == 0)
				{
					AddedObjects.Remove(objectType);
				}
			}
			else
			{
				AddObjectReferenceToFactoryChangeRecords(reference, RemovedObjects);
			}
		}

		void AddObjectReferenceToFactoryChangeRecords(ObjectReference reference, Dictionary<Type, List<ObjectReference>> factoryChangeRecords)
		{
			var objectType = reference.Type;
			if (!factoryChangeRecords.ContainsKey(objectType))
			{
				factoryChangeRecords.Add(objectType, new List<ObjectReference>());
			}
			factoryChangeRecords[objectType].Add(reference);
		}

		public IEnumerable<ObjectReference> GetAddedObjects()
		{
			return AddedObjects.SelectMany(x => x.Value);
		}

		public IEnumerable<ObjectReference> GetRemovedObjects()
		{
			return RemovedObjects.SelectMany(x => x.Value);
		}

		Dictionary<Type, List<ObjectReference>> AddedObjects => addedObjects ?? (addedObjects = new Dictionary<Type, List<ObjectReference>>());
		Dictionary<Type, List<ObjectReference>> addedObjects;

		Dictionary<Type, List<ObjectReference>> RemovedObjects => removedObjects ?? (removedObjects = new Dictionary<Type, List<ObjectReference>>());
		Dictionary<Type, List<ObjectReference>> removedObjects;

		internal bool HasObjectWithReference(ObjectReference reference) => Factory.ContainsKey(reference.Type) && Factory[reference.Type].ContainsKey(reference.ID);

		internal BaseObject GetObjectByReference(ObjectReference reference) => GetObjectByTypeAndID(reference.Type, reference.ID);

		BaseObject GetObjectByTypeAndID(Type type, uint id)
		{
			var result = default(BaseObject);
			if (Factory.ContainsKey(type) && Factory[type].ContainsKey(id))
			{
				result = Factory[type][id];
			}
			return result;
		}

		public IEnumerable<ObjectReference> Query<TObject>(Predicate<TObject> predicate = null) where TObject : BaseObject
		{
			var objectType = typeof(TObject);
			var objectCollection = new List<TObject>();

			if (Factory.ContainsKey(objectType))
			{
				objectCollection.AddRange(Factory[objectType].Values.OfType<TObject>());
			}

			foreach (var key in Factory.Keys)
			{
				if (objectType != key && objectType.IsAssignableFrom(key))
				{
					objectCollection.AddRange(Factory[key].Values.OfType<TObject>());
				}
			}

			if (objectCollection.Count > 0)
			{
				foreach (var obj in objectCollection)
				{
					var queryResult = predicate?.Invoke(obj) ?? true;
					if (queryResult)
					{
						yield return obj.Reference;
					}
				}
			}
		}

		public ObjectReference QueryFirst<TObject>(Predicate<TObject> predicate = null) where TObject : BaseObject => Query(predicate).FirstOrDefault();

		void AddObjectToFactory(BaseObject objectToAdd)
		{
			if (objectToAdd != null)
			{
				var objectType = objectToAdd.GetType();
				if (!Factory.ContainsKey(objectType))
				{
					Factory.Add(objectType, new Dictionary<uint, BaseObject>());
				}

				if (!Factory[objectType].ContainsKey(objectToAdd.ID))
				{
					Factory[objectType].Add(objectToAdd.ID, objectToAdd);
				}
			}
		}

		Dictionary<Type, Dictionary<uint, BaseObject>> Factory => factory ?? (factory = new Dictionary<Type, Dictionary<uint, BaseObject>>());
		Dictionary<Type, Dictionary<uint, BaseObject>> factory;

		ObjectIDFountain GetObjectIDFountain(Type objectType)
		{
			if (!IDFountains.ContainsKey(objectType))
			{
				IDFountains.Add(objectType, new ObjectIDFountain());
			}
			return IDFountains[objectType];
		}

		internal bool IsObjectIDUsed(Type objectType, uint id)
		{
			var result = false;
			if (IDFountains.ContainsKey(objectType))
			{
				result = IDFountains[objectType].currentID >= id;
			}
			return result;
		}

		Dictionary<Type, ObjectIDFountain> IDFountains => idFountains ?? (idFountains = new Dictionary<Type, ObjectIDFountain>());
		Dictionary<Type, ObjectIDFountain> idFountains;
	}
}
