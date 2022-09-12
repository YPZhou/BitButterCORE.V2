namespace BitButterCORE.V2
{
	internal class ObjectIDFountain
	{
		internal ObjectIDFountain(uint currentID = 1)
		{
			this.currentID = currentID;
		}

		internal void Reset(uint currentID = 1)
		{
			this.currentID = currentID;
		}

		internal uint NextID => currentID++;
		internal uint currentID;
	}
}
