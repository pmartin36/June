using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Direction { Up, Right, Down, Left}
public enum GameColors { white, yellow, red, green, blue, black }
public enum MenuTypes { Pause, Start, Score }
public enum DifficultyTypes { Direction, Speed, Area, BlackBlade }

public static class EnumHelper {
	public static Direction GetOppositeDirection(Direction d) {
		switch (d) {
			default:
			case Direction.Up:
				return Direction.Down;
			case Direction.Right:
				return Direction.Left;
			case Direction.Down:
				return Direction.Up;
			case Direction.Left:
				return Direction.Right;
		}
	}

	public static bool IsLeftRight(Direction d) {
		return d == Direction.Left || d == Direction.Right;
	}

	public static bool IsMasterDirection(Direction d) {
		return d == Direction.Up || d == Direction.Right;
	}
}
