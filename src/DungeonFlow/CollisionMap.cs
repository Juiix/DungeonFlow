using System;

namespace DungeonFlow;

public sealed class CollisionMap(DungeonInt2 size)
{
	public readonly DungeonInt2 Size = size;
	private readonly int _wordWidth = ((size.X + 31) >> 5) + 2;
	private readonly ulong[] _words = new ulong[GetBlockLength(size)];

	public static bool Test(CollisionMap a, DungeonInt2 aPosition, CollisionMap b, DungeonInt2 bPosition)
	{
		var minX = Math.Max(aPosition.X, bPosition.X);
		var minY = Math.Max(aPosition.Y, bPosition.Y);

		var maxX = Math.Min(aPosition.X + a.Size.X, bPosition.X + b.Size.X);
		var maxY = Math.Min(aPosition.Y + a.Size.Y, bPosition.Y + b.Size.Y);
		if (maxX <= minX || maxY <= minY)
			return false;

		var yOffsetA = minY - aPosition.Y;
		var yOffsetB = minY - bPosition.Y;

		var xOffsetIndexA = (minX - aPosition.X) & 31;
		var xOffsetIndexB = (minX - bPosition.X) & 31;

		var xOffsetBlockA = (minX - aPosition.X) >> 5;
		var xOffsetBlockB = (minX - bPosition.X) >> 5;

		var width = maxX - minX;
		var height = maxY - minY;
		var blocks = (width + 31) >> 5;
		var trailing = width & 31;
		ulong lastMask32 = trailing == 0 ? 0xFFFF_FFFFul : ((1ul << trailing) - 1ul);

		for (int y = 0; y < height; y++)
		{
			var rowA = (y + yOffsetA) * a._wordWidth;
			var rowB = (y + yOffsetB) * b._wordWidth;

			var baseA = rowA + xOffsetBlockA + 1;
			var baseB = rowB + xOffsetBlockB + 1;

			for (int x = 0; x < blocks; x++)
			{
				var mask = x == blocks - 1 ? lastMask32 : 0xFFFF_FFFFul;
				var wordA = (a._words[baseA + x] >> xOffsetIndexA) & mask;
				var wordB = (b._words[baseB + x] >> xOffsetIndexB) & mask;
				if ((wordA & wordB) != 0)
					return true;
			}
		}

		return false;
	}

	public static bool Test(CollisionMap a, DungeonInt2 aPosition, DungeonRect bRect)
	{
		var minX = Math.Max(aPosition.X, bRect.Position.X);
		var minY = Math.Max(aPosition.Y, bRect.Position.Y);

		var maxX = Math.Min(aPosition.X + a.Size.X, bRect.Position.X + bRect.Size.X);
		var maxY = Math.Min(aPosition.Y + a.Size.Y, bRect.Position.Y + bRect.Size.Y);
		if (maxX <= minX || maxY <= minY)
			return false;

		var yOffsetA = minY - aPosition.Y;
		var xOffsetIndexA = (minX - aPosition.X) & 31;
		var xOffsetBlockA = (minX - aPosition.X) >> 5;

		var width = maxX - minX;
		var height = maxY - minY;
		var blocks = (width + 31) >> 5;
		var trailing = width & 31;
		ulong lastMask32 = trailing == 0 ? 0xFFFF_FFFFul : ((1ul << trailing) - 1ul);

		for (int y = 0; y < height; y++)
		{
			var rowA = (y + yOffsetA) * a._wordWidth;
			var baseA = rowA + xOffsetBlockA + 1;

			for (int x = 0; x < blocks; x++)
			{
				var mask = x == blocks - 1 ? lastMask32 : 0xFFFF_FFFFul;
				var wordA = (a._words[baseA + x] >> xOffsetIndexA) & mask;
				if (wordA != 0) return true;
			}
		}

		return false;
	}

	public void Clear(DungeonInt2 position)
	{
		var wordX = position.X >> 5;
		var wordIndex = position.X & 31;
		_words[position.Y * _wordWidth + wordX + 1] &= ~(1ul << wordIndex);
		_words[position.Y * _wordWidth + wordX] &= ~(1ul << (32 + wordIndex)); // set previous for offset checks
	}

	public bool Get(DungeonInt2 position)
	{
		var wordX = position.X >> 5;
		var wordIndex = position.X & 31;
		var word = _words[position.Y * _wordWidth + wordX + 1];
		return ((word >> wordIndex) & 1u) == 1u;
	}

	public void Set(DungeonInt2 position)
	{
		var wordX = position.X >> 5;
		var wordIndex = position.X & 31;
		_words[position.Y * _wordWidth + wordX + 1] |= 1ul << wordIndex;
		_words[position.Y * _wordWidth + wordX] |= 1ul << (32 + wordIndex); // set previous for offset checks
	}

	private static int GetBlockLength(DungeonInt2 size)
	{
		if (size.X <= 0 || size.Y <= 0)
			throw new ArgumentOutOfRangeException("Size values must be greater than 0");
		var blockSize = new DungeonInt2(
			((size.X + 31) >> 5) + 2,
			size.Y
		);
		return blockSize.X * blockSize.Y;
	}
}
