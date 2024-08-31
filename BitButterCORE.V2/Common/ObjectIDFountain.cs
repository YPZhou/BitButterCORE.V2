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

		internal void SetToNextAvailableID(uint newID)
		{
			if (newID >= currentID)
			{
				currentID = newID + 1;
			}
		}

		public uint NextID
		{
			get
			{
				var result = currentID;
				currentID += 1;
				return result;
			}
		}

		uint currentID;
	}
}
