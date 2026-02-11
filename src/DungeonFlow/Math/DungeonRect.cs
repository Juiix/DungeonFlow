namespace DungeonFlow;

public struct DungeonRect
{
	public DungeonInt2 Position;
	public DungeonInt2 Size;

	public DungeonRect(DungeonInt2 position, DungeonInt2 size)
	{
		Position = position;
		Size = size;
	}

	public DungeonRect(int x, int y, int width, int height)
	{
		Position = new DungeonInt2(x, y);
		Size = new DungeonInt2(width, height);
	}

	public int X
	{
		get => Position.X;
		set => Position.X = value;
	}

	public int Y
	{
		get => Position.Y;
		set => Position.Y = value;
	}

	public int Width
	{
		get => Size.X;
		set => Size.X = value;
	}

	public int Height
	{
		get => Size.Y;
		set => Size.Y = value;
	}

	public DungeonInt2 Min => Position;
	public DungeonInt2 Max => new DungeonInt2(Position.X + Size.X, Position.Y + Size.Y);

	public bool Contains(DungeonInt2 point)
	{
		return point.X >= Min.X &&
			   point.Y >= Min.Y &&
			   point.X < Max.X &&
			   point.Y < Max.Y;
	}

	public bool Intersects(DungeonRect other)
	{
		return other.Max.X > Min.X &&
			   other.Min.X < Max.X &&
			   other.Max.Y > Min.Y &&
			   other.Min.Y < Max.Y;
	}

	public bool Envelopes(DungeonRect other)
	{
		return other.Min.X >= Min.X &&
			   other.Min.Y >= Min.Y &&
			   other.Max.X <= Max.X &&
			   other.Max.Y <= Max.Y;
	}

	public override string ToString()
	{
		return $"IntRect(Pos={Position}, Size={Size})";
	}
}
