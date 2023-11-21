using Godot;

namespace GodotHFSM
{
	public static class TransitionOnMouse
	{
		public class Down<TStateId> : TransitionBase<TStateId>
		{
			private readonly MouseButton button;

			/// <summary>
			/// Initialises a new transition that triggers, while a mouse button is down.
			/// It behaves like Input.IsMouseButtonPressed(...).
			/// </summary>
			/// <param name="button">The mouse button to watch.</param>
			/// <returns></returns>
			public Down(
					TStateId from,
					TStateId to,
					MouseButton button,
					bool forceInstantly = false) : base(from, to, forceInstantly)
			{
				this.button = button;
			}

			public override bool ShouldTransition()
			{
				return Input.IsMouseButtonPressed(button);
			}
		}

		public class Release<TStateId> : TransitionBase<TStateId>
		{
			private readonly MouseButton button;
			private bool? _wasPressed = null;

			/// <summary>
			/// Initialises a new transition that triggers, when a mouse button was just down and is up now.
			/// </summary>
			/// <param name="button">The mouse button to watch.</param>
			public Release(
					TStateId from,
					TStateId to,
					MouseButton button,
					bool forceInstantly = false) : base(from, to, forceInstantly)
			{
				this.button = button;
			}

			public override bool ShouldTransition()
			{
				bool isPressed = Input.IsMouseButtonPressed(button);
				bool shouldTransition = _wasPressed.HasValue && _wasPressed.Value && !isPressed;
				_wasPressed = isPressed;
				return shouldTransition;
			}
		}

		public class Press<TStateId> : TransitionBase<TStateId>
		{
			private readonly MouseButton button;
			private bool? _wasPressed = null;

			/// <summary>
			/// Initialises a new transition that triggers, when a mouse button was just up and is down now.
			/// </summary>
			/// <param name="button">The mouse button to watch.</param>
			public Press(
					TStateId from,
					TStateId to,
					MouseButton button,
					bool forceInstantly = false) : base(from, to, forceInstantly)
			{
				this.button = button;
			}

			public override bool ShouldTransition()
			{
				bool isPressed = Input.IsMouseButtonPressed(button);
				bool shouldTransition = _wasPressed.HasValue && !_wasPressed.Value && isPressed;
				_wasPressed = isPressed;
				return shouldTransition;
			}
		}

		public class Up<TStateId> : TransitionBase<TStateId>
		{
			private readonly MouseButton button;

			/// <summary>
			/// Initialises a new transition that triggers, while a mouse button is up.
			/// It behaves like ! Input.IsMouseButtonPressed(...).
			/// </summary>
			/// <param name="button">The mouse button to watch.</param>
			public Up(
					TStateId from,
					TStateId to,
					MouseButton button,
					bool forceInstantly = false) : base(from, to, forceInstantly)
			{
				this.button = button;
			}

			public override bool ShouldTransition()
			{
				return !Input.IsMouseButtonPressed(button);
			}
		}

		public class Down : Down<string>
		{
			public Down(
				string @from,
				string to,
				MouseButton button,
				bool forceInstantly = false) : base(@from, to, button, forceInstantly)
			{
			}
		}

		public class Release : Release<string>
		{
			public Release(
				string @from,
				string to,
				MouseButton button,
				bool forceInstantly = false) : base(@from, to, button, forceInstantly)
			{
			}
		}

		public class Press : Press<string>
		{
			public Press(
				string @from,
				string to,
				MouseButton button,
				bool forceInstantly = false) : base(@from, to, button, forceInstantly)
			{
			}
		}

		public class Up : Up<string>
		{
			public Up(
				string @from,
				string to,
				MouseButton button,
				bool forceInstantly = false) : base(@from, to, button, forceInstantly)
			{
			}
		}
	}
}
