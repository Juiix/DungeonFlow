using System;

namespace DungeonFlow;

public struct DungeonInt2(int x, int y)
{
	public int X = x;
	public int Y = y;

	public readonly DungeonInt2 Add(DungeonInt2 other) => new DungeonInt2(X + other.X, Y + other.Y);
	public readonly DungeonInt2 Add(int value) => new DungeonInt2(X + value, Y + value);
	public readonly DungeonInt2 Clamp(DungeonInt2 min, DungeonInt2 max) => new DungeonInt2(Math.Min(Math.Max(X, min.X), max.X), Math.Min(Math.Max(Y, min.Y), max.Y));
	public readonly DungeonInt2 Divide(DungeonInt2 other) => new DungeonInt2(X / other.X, Y / other.Y);
	public readonly DungeonInt2 Divide(int value) => new DungeonInt2(X / value, Y / value);
	public readonly DungeonInt2 Multiply(DungeonInt2 other) => new DungeonInt2(X * other.X, Y * other.Y);
	public readonly DungeonInt2 Multiply(int value) => new DungeonInt2(X * value, Y * value);
	public readonly DungeonInt2 Subtract(DungeonInt2 other) => new DungeonInt2(X - other.X, Y - other.Y);
	public readonly DungeonInt2 Subtract(int value) => new DungeonInt2(X - value, Y - value);

	public readonly bool Equals(DungeonInt2 other) => X == other.X && Y == other.Y;

	public readonly override bool Equals(object? obj)
	{
		if (obj is DungeonInt2 other)
		{
			return Equals(other);
		}
		return base.Equals(obj);
	}

	public readonly override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public readonly override string ToString()
	{
		return $"({X}, {Y})";
	}

	public static DungeonInt2 Zero => new(0, 0);
	public static DungeonInt2 One => new(1, 1);

	public static bool operator ==(DungeonInt2 a, DungeonInt2 b) => a.Equals(b);
	public static bool operator !=(DungeonInt2 a, DungeonInt2 b) => !a.Equals(b);

	public static DungeonInt2 operator +(DungeonInt2 a, DungeonInt2 b) => a.Add(b);
	public static DungeonInt2 operator -(DungeonInt2 a, DungeonInt2 b) => a.Subtract(b);
	public static DungeonInt2 operator *(DungeonInt2 a, DungeonInt2 b) => a.Multiply(b);
	public static DungeonInt2 operator /(DungeonInt2 a, DungeonInt2 b) => a.Divide(b);

	public static DungeonInt2 operator +(DungeonInt2 a, int b) => a.Add(b);
	public static DungeonInt2 operator -(DungeonInt2 a, int b) => a.Subtract(b);
	public static DungeonInt2 operator *(DungeonInt2 a, int b) => a.Multiply(b);
	public static DungeonInt2 operator /(DungeonInt2 a, int b) => a.Divide(b);

	public static implicit operator DungeonInt2(int value) => new(value, value);
}
