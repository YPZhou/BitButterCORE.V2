using System.Collections.Generic;
using System.Linq;

namespace BitButterCORE.V2
{
	internal class EventCallFrame
	{
		internal void BeginChildFrame(EventCallFrame childFrame)
		{
			if (IsCurrentFrame)
			{
				IsCurrentFrame = false;

				childFrame.BeginFrame();
				Frames.Add(childFrame);
			}
		}

		internal void EndChildFrame()
		{
			var lastChildFrame = Frames.Last();
			if (!IsCurrentFrame && lastChildFrame.IsCurrentFrame)
			{
				Frames.Last().EndFrame();
				IsCurrentFrame = true;
			}
		}

		internal void BeginFrame()
		{
			if (!IsCurrentFrame)
			{
				IsCurrentFrame = true;
			}
		}

		internal void EndFrame()
		{
			if (IsCurrentFrame)
			{
				IsCurrentFrame = false;
			}
		}

		internal bool IsCurrentFrame { get; private set; }

		internal EventCallFrame CurrentFrame => IsCurrentFrame ? this : FindCurrentFrameInAllChildren();

		EventCallFrame FindCurrentFrameInAllChildren()
		{
			return Frames.FirstOrDefault(frame => frame.CurrentFrame != null).CurrentFrame;
		}

		List<EventCallFrame> Frames => frames ?? (frames = new List<EventCallFrame>());
		List<EventCallFrame> frames;

		internal void PrintCallStack()
		{
		}
	}
}
