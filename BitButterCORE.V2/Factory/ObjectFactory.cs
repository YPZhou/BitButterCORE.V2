using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BitButterCORE.V2
{
	public class ObjectFactory : BaseSingleton<ObjectFactory>
	{
		public IObjectReference<TObject> Create<TObject>(params object[] args) where TObject : IBaseObject
		{
			return (IObjectReference<TObject>)Create(typeof(TObject), args);
		}

		public IObjectReference Create(Type objectType, params object[] args)
		{
			var result = default(IObjectReference);
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

					var obj = (IBaseObject)constructor.Invoke(inputParameters.ToArray());
					AddObjectToFactory(obj);
					obj.OnObjectCreated();

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
								if (inputParameter != null && !parameterType.IsAssignableFrom(inputParameter.GetType()))
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

		public IObjectReference<TObject> CreateFromTemplate<TObject>(string templateName, params object[] args) where TObject : ITemplateObject
		{
			var result = Create<TObject>(args);
			if (result != null && result.IsValid)
			{
				var template = ObjectTemplateManager.Instance[templateName];
				foreach (var property in template)
				{
					var propertyInfo = typeof(TObject).GetProperty(property.Key, BindingFlags.Public | BindingFlags.Instance);
					propertyInfo?.SetValue(result.Object, property.Value);
				}

				result.Object.SetupObjectFromTemplate(templateName, template);
			}

			return result;
		}

		public void Remove(IObjectReference reference)
		{
			var objectToRemove = GetObjectByTypeAndID(reference.Type, reference.ID);
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

		public void ClearChangesForObject(IObjectReference reference)
		{
			if (AddedObjects.ContainsKey(reference.Type))
			{
				AddedObjects[reference.Type].Remove(reference);
				if (AddedObjects[reference.Type].Count == 0)
				{
					AddedObjects.Remove(reference.Type);
				}
			}

			if (RemovedObjects.ContainsKey(reference.Type))
			{
				RemovedObjects[reference.Type].Remove(reference);
				if (RemovedObjects[reference.Type].Count == 0)
				{
					RemovedObjects.Remove(reference.Type);
				}
			}
		}

		void UpdateFactoryChangeRecordsForAddObject(IObjectReference reference)
		{
			AddObjectReferenceToFactoryChangeRecords(reference, AddedObjects);
		}

		void UpdateFactoryChangeRecordsForRemoveObject(IObjectReference reference)
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

		void AddObjectReferenceToFactoryChangeRecords(IObjectReference reference, Dictionary<Type, List<IObjectReference>> factoryChangeRecords)
		{
			var objectType = reference.Type;
			if (!factoryChangeRecords.ContainsKey(objectType))
			{
				factoryChangeRecords.Add(objectType, new List<IObjectReference>());
			}
			factoryChangeRecords[objectType].Add(reference);
		}

		public IEnumerable<IObjectReference> GetAddedObjects()
		{
			return AddedObjects.SelectMany(x => x.Value);
		}

		public IEnumerable<IObjectReference> GetRemovedObjects()
		{
			return RemovedObjects.SelectMany(x => x.Value);
		}

		Dictionary<Type, List<IObjectReference>> AddedObjects => addedObjects ?? (addedObjects = new Dictionary<Type, List<IObjectReference>>());
		Dictionary<Type, List<IObjectReference>> addedObjects;

		Dictionary<Type, List<IObjectReference>> RemovedObjects => removedObjects ?? (removedObjects = new Dictionary<Type, List<IObjectReference>>());
		Dictionary<Type, List<IObjectReference>> removedObjects;

		internal bool HasObjectWithTypeAndID(Type type, uint id) => Factory.TryGetValue(type, out var typeFactory) && typeFactory.TryGetValue(id, out _);

		internal IBaseObject GetObjectByTypeAndID(Type type, uint id)
		{
			if (Factory.TryGetValue(type, out var typeFactory) && typeFactory.TryGetValue(id, out var result))
			{
				return result;
			}

			return null;
		}

		public IEnumerable<IObjectReference<TObject>> Query<TObject>(Predicate<TObject> predicate = null) where TObject : IBaseObject
		{
			var objectType = typeof(TObject);
			var objectCollection = new List<TObject>();

			if (Factory.TryGetValue(objectType, out var typeFactory))
			{
				objectCollection.AddRange(typeFactory.Values.OfType<TObject>());
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
						yield return (IObjectReference<TObject>)obj.Reference;
					}
				}
			}
		}

		public IObjectReference<TObject> QueryFirst<TObject>(Predicate<TObject> predicate = null) where TObject : IBaseObject => Query(predicate).FirstOrDefault();

		void AddObjectToFactory(IBaseObject objectToAdd)
		{
			if (objectToAdd != null)
			{
				var objectType = objectToAdd.GetType();
				if (!Factory.ContainsKey(objectType))
				{
					Factory.Add(objectType, new Dictionary<uint, IBaseObject>());
				}

				if (!Factory[objectType].ContainsKey(objectToAdd.ID))
				{
					Factory[objectType].Add(objectToAdd.ID, objectToAdd);
				}
			}
		}

		Dictionary<Type, Dictionary<uint, IBaseObject>> Factory => factory ?? (factory = new Dictionary<Type, Dictionary<uint, IBaseObject>>());
		Dictionary<Type, Dictionary<uint, IBaseObject>> factory;

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
