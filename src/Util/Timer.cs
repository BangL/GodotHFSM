using Godot;

namespace GodotHFSM
{
	/// <summary>
	/// Default timer that calculates the elapsed time based on Time.time.
	/// </summary>
	public class Timer : ITimer
	{
		private static float GodotTimeAsSeconds => Time.GetTicksMsec() / 1000f;

		public float startTime;
		public float Elapsed => GodotTimeAsSeconds - startTime;

		public Timer()
		{
			startTime = GodotTimeAsSeconds;
		}

		public void Reset()
		{
			startTime = GodotTimeAsSeconds;
		}

		public static bool operator >(Timer timer, float duration)
			=> timer.Elapsed > duration;

		public static bool operator <(Timer timer, float duration)
			=> timer.Elapsed < duration;

		public static bool operator >=(Timer timer, float duration)
			=> timer.Elapsed >= duration;

		public static bool operator <=(Timer timer, float duration)
			=> timer.Elapsed <= duration;
	}
}
