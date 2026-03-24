using System;
using System.Collections;
using System.Collections.Generic;

namespace BitButterCORE.V2
{
	public struct QueryEnumerator<TObject> : IEnumerable<IObjectReference<TObject>>, IEnumerator<IObjectReference<TObject>>
		where TObject : IBaseObject
	{
		public QueryEnumerator(Dictionary<Type, Dictionary<uint, IBaseObject>> factory, Predicate<TObject> predicate)
		{
			this.factory = factory;
			this.predicate = predicate;
			objectType = typeof(TObject);

			current = null;
			hasCurrentType = false;

			objectCollectionEnumerator = default;
			objectTypeEnumerator = factory.Keys.GetEnumerator();
		}

		readonly Dictionary<Type, Dictionary<uint, IBaseObject>> factory;
		readonly Predicate<TObject> predicate;
		readonly Type objectType;

		Dictionary<uint, IBaseObject>.ValueCollection.Enumerator objectCollectionEnumerator;
		Dictionary<Type, Dictionary<uint, IBaseObject>>.KeyCollection.Enumerator objectTypeEnumerator;
		IObjectReference<TObject> current;
		bool hasCurrentType;

		#region IEnumerable<IObjectReference<TObject>>

		public QueryEnumerator<TObject> GetEnumerator() => this;
		IEnumerator<IObjectReference<TObject>> IEnumerable<IObjectReference<TObject>>.GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => this;

		#endregion

		#region IEnumerator<IObjectReference<TObject>>

		public bool MoveNext()
		{
			var keepSearching = true;
			while (keepSearching)
			{
				if (hasCurrentType)
				{
					while (objectCollectionEnumerator.MoveNext())
					{
						if (objectCollectionEnumerator.Current is TObject obj
							&& (predicate == null || predicate.Invoke(obj)))
						{
							current = (IObjectReference<TObject>)obj.Reference;
							return true;
						}
					}

					objectCollectionEnumerator.Dispose();
					hasCurrentType = false;
				}

				if (objectTypeEnumerator.MoveNext())
				{
					var currentType = objectTypeEnumerator.Current;
					if (objectType.IsAssignableFrom(currentType))
					{
						objectCollectionEnumerator = factory[currentType].Values.GetEnumerator();
						hasCurrentType = true;
					}
				}
				else
				{
					objectTypeEnumerator.Dispose();
					keepSearching = false;
				}
			}

			return false;
		}

		public void Reset()
		{
			current = null;
			hasCurrentType = false;

			objectCollectionEnumerator.Dispose();
			objectCollectionEnumerator = default;

			objectTypeEnumerator.Dispose();
			objectTypeEnumerator = factory.Keys.GetEnumerator();
		}

		public void Dispose()
		{
			objectCollectionEnumerator.Dispose();
			objectTypeEnumerator.Dispose();
		}

		public IObjectReference<TObject> Current => current;

		object IEnumerator.Current => current;

		#endregion
	}
}
