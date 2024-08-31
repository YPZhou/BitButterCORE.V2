using NUnit.Framework;
using System.Reflection;

namespace BitButterCORE.V2.Testing
{
	public class ObjectIDFountainTest
	{
		[Test]
		public void TestConstructor()
		{
			var currentIDFieldInfo = typeof(ObjectIDFountain).GetField("currentID", BindingFlags.Instance | BindingFlags.NonPublic);

			var fountain = new ObjectIDFountain();
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(1));

			fountain = new ObjectIDFountain(3);
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(3));
		}

		[Test]
		public void TestReset()
		{
			var currentIDFieldInfo = typeof(ObjectIDFountain).GetField("currentID", BindingFlags.Instance | BindingFlags.NonPublic);
			var fountain = new ObjectIDFountain();
			
			fountain.Reset(3);
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(3), "Should be reset to 3");

			fountain.Reset();
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(1), "Should be reset to 1 with default argument");
		}

		[Test]
		public void TestSetToNextAvailableID()
		{
			var methodInfo = typeof(ObjectIDFountain).GetMethod("SetToNextAvailableID", BindingFlags.Instance | BindingFlags.NonPublic);
			var currentIDFieldInfo = typeof(ObjectIDFountain).GetField("currentID", BindingFlags.Instance | BindingFlags.NonPublic);
			var fountain = new ObjectIDFountain();

			methodInfo.Invoke(fountain, new object[] { 3u });
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(4), "Next available ID should be 4 which is ID 3 plus 1");

			methodInfo.Invoke(fountain, new object[] { 1u });
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(4), "Next available ID should be 4 as ID 1 is smaller");

			methodInfo.Invoke(fountain, new object[] { 4u });
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(5), "Next available ID should be 5 as ID 4 is equal to current ID");

			methodInfo.Invoke(fountain, new object[] { 6u });
			Assert.That((uint)currentIDFieldInfo.GetValue(fountain), Is.EqualTo(7), "Next available ID should be 7 as ID 6 is larger than current ID");
		}

		[Test]
		public void TestNextID()
		{
			var fountain = new ObjectIDFountain();

			Assert.That(fountain.NextID, Is.EqualTo(1), "NextID should return 1");
			Assert.That(fountain.NextID, Is.EqualTo(2), "NextID should return 2 as id is incremented by 1 for each access");
		}
	}
}
