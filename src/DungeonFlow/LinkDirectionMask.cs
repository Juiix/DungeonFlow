using System;

namespace DungeonFlow;

[Flags]
public enum LinkDirectionMask : byte
{
	None = 0,
	Right = 1 << 0,
	Up = 1 << 1,
	Left = 1 << 2,
	Down = 1 << 3,
	Horizontal = Right | Left,
	Vertical = Up | Down,
	All = Right | Up | Left | Down,
}

public static class LinkDirectionMaskExtensions
{
	public static bool Contains(this LinkDirectionMask mask, LinkDirection direction)
	{
		if (direction == LinkDirection.Undefined)
			return false;
		return (mask & (LinkDirectionMask)(1 << (int)direction)) != 0;
	}

	public static LinkDirection PickRandom(this LinkDirectionMask mask, Random random)
	{
		Span<LinkDirection> buffer = stackalloc LinkDirection[4];
		var count = 0;
		for (var i = 0; i < 4; i++)
		{
			if ((mask & (LinkDirectionMask)(1 << i)) != 0)
				buffer[count++] = (LinkDirection)i;
		}
		if (count == 0)
			return LinkDirection.Undefined;
		return buffer[random.Next(0, count)];
	}
}
