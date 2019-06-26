using System.Collections.Generic;
using UnityEngine;
using JTUtility;

namespace TripleBladeHorse.Movement
{
	public class MovingEventArgs : System.EventArgs
	{
		MovingEventArgs() { }
		public MovingEventArgs(bool facingRight, Vector2 position, Vector2 velocity, MovingState currentMovingState, MovingState lastMovingState)
		{
			this.facingRight = facingRight;
			this.position = position;
			this.velocity = velocity;
			this.lastMovingState = lastMovingState;
			this.currentMovingState = currentMovingState;
		}

		public bool facingRight;
		public Vector2 position;
		public Vector2 velocity;
		public MovingState lastMovingState;
		public MovingState currentMovingState;
	}

	public class LandingEventArgs : System.EventArgs
	{
		LandingEventArgs() { }
		public LandingEventArgs(LandingState currentLandingState, LandingState lastLandingState)
		{
			this.lastLandingState = lastLandingState;
			this.currentLandingState = currentLandingState;
		}

		public LandingState lastLandingState;
		public LandingState currentLandingState;
	}

	public interface ICanChangeMoveState
	{
		event Action<ICanChangeMoveState, MovingEventArgs> OnMovingStateChanged;
	}

	public interface ICanMove : ICanChangeMoveState
	{
		void Move(Vector2 direction);
	}

	public interface ICanJump : ICanChangeMoveState
	{
		void Jump();

		event Action OnJump;
	}

	public interface ICanDash : ICanChangeMoveState
	{
		void Dash(Vector2 direction);

		event Action OnDashingBegin;
		event Action OnDashingDelayBegin;
		event Action OnDashingFinished;
	}

	public interface ICanDetectGround
	{
		bool IsOnGround { get; }

		event Action<ICanDetectGround, LandingEventArgs> OnLandingStateChanged;
	}
}
