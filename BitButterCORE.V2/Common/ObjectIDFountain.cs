namespace BitButterCORE.V2
{
	public class ObjectIDFountain
	{
		public ObjectIDFountain(uint currentID = 1)
		{
			this.currentID = currentID;
		}

		public void Reset(uint currentID = 1)
		{
			this.currentID = currentID;
		}

		public uint NextID => currentID++;
		public uint currentID;
	}
}
