namespace BitButterCORE.V2
{
	internal class EventCallStack : BaseSingleton<EventCallStack>
	{
		internal void StartMonitoring()
		{
			rootFrame = new EventCallFrame();
			rootFrame.BeginFrame();
		}

		internal void EndMonitoring()
		{
			rootFrame.EndFrame();
			rootFrame.PrintCallStack();
			rootFrame = null;
		}

		internal void StartChildFrame(EventCallFrame childFrame)
		{
			if (rootFrame != null && rootFrame.CurrentFrame != null)
			{
				rootFrame.CurrentFrame.BeginChildFrame(childFrame);
			}
		}

		internal void EndChildFrame()
		{
			if (rootFrame != null && rootFrame.CurrentFrame != null)
			{
				rootFrame.CurrentFrame.EndChildFrame();
			}
		}

		EventCallFrame rootFrame;
	}
}
