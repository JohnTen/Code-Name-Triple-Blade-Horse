using System;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
	Right,
	UpRight,
	Up,
	UpLeft,
	Left,
	DownLeft,
	Down,
	DownRight,
}

public static class DirectionalHelper
{
	#region Fields
	readonly static Vector2[] _normalizedDirections =
	{
		new Vector2( 0,  1),
		new Vector2( 1,  1).normalized,
		new Vector2( 1,  0),
		new Vector2( 1, -1).normalized,
		new Vector2( 0, -1),
		new Vector2(-1, -1).normalized,
		new Vector2(-1,  0),
		new Vector2(-1,  1).normalized,
	};

	static Direction[] _opposite = {
		Direction.Left,
		Direction.DownLeft,
		Direction.Down,
		Direction.DownRight,
		Direction.Right,
		Direction.UpRight,
		Direction.Up,
		Direction.UpLeft,
	};

	public const int Count = 8;
	#endregion

	#region Methods

	public static Direction GetNextClockWise(Direction direction)
	{
		return (Direction)(((int)direction + 1) % Count);
	}

	public static Direction GetNextCounterClockWise(Direction direction)
	{
		return (Direction)(((int)direction - 1 + Count) % Count);
	}

	public static int GetOpposite(int direction)
	{
		return (int)_opposite[direction];
	}

	public static Direction GetOpposite(Direction direction)
	{
		return _opposite[(int)direction];
	}

	public static Vector2 GetVector(int direction)
	{
		return _normalizedDirections[direction];
	}

	public static Vector2 GetVector(Direction direction)
	{
		return GetVector((int)direction);
	}

	public static Vector2 NormalizeOctadDirection(Vector2 direction)
	{
		direction.Normalize();

		var dotResult = float.NegativeInfinity;
		var closestDirection = direction;

		foreach (var dir in _normalizedDirections)
		{
			var dot = Vector2.Dot(direction, dir);

			if (dot > dotResult)
			{
				dotResult = dot;
				closestDirection = dir;
			}
		}

		return closestDirection;
	}

	public static Vector2 NormalizeHorizonalDirection(Vector2 direction)
	{
		return direction.x < 0 ? Vector2.left : Vector2.right;
	}

	public static Vector2 NormalizeVerticalDirection(Vector2 direction)
	{
		return direction.y < 0 ? Vector2.down : Vector2.up;
	}

	public static Vector2 NormalizeQuadDirection(Vector2 direction)
	{
		return
			Mathf.Abs(direction.y) > Mathf.Abs(direction.x)?
				NormalizeVerticalDirection(direction) :
				NormalizeHorizonalDirection(direction);
	}

	#endregion
}
