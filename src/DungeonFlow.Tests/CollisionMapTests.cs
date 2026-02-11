namespace DungeonFlow.Tests;

public sealed class CollisionMapTests
{
	// Slow, obviously-correct reference implementation:
	// checks per-bit overlap within the rect intersection.
	private static bool ReferenceOverlap(CollisionMap a, DungeonInt2 aPos, CollisionMap b, DungeonInt2 bPos)
	{
		int minX = Math.Max(aPos.X, bPos.X);
		int minY = Math.Max(aPos.Y, bPos.Y);
		int maxX = Math.Min(aPos.X + a.Size.X, bPos.X + b.Size.X);
		int maxY = Math.Min(aPos.Y + a.Size.Y, bPos.Y + b.Size.Y);

		if (maxX <= minX || maxY <= minY)
			return false;

		for (int y = minY; y < maxY; y++)
		{
			for (int x = minX; x < maxX; x++)
			{
				var aLocal = new DungeonInt2(x - aPos.X, y - aPos.Y);
				var bLocal = new DungeonInt2(x - bPos.X, y - bPos.Y);

				if (a.Get(aLocal) && b.Get(bLocal))
					return true;
			}
		}

		return false;
	}

	private static void ClearAll(CollisionMap ht)
	{
		for (int y = 0; y < ht.Size.Y; y++)
			for (int x = 0; x < ht.Size.X; x++)
				ht.Clear(new DungeonInt2(x, y));
	}

	private static void SetBit(CollisionMap ht, int x, int y) => ht.Set(new DungeonInt2(x, y));

	// ---- Test cases ----

	public static TheoryData<string, DungeonInt2, DungeonInt2, DungeonInt2, DungeonInt2, Action<CollisionMap>, Action<CollisionMap>> Cases =>
		new()
		{
                // 1) No overlap (separated on X)
                {
					"No overlap on X => false",
					new DungeonInt2(16, 16), new DungeonInt2(100, 100),
					new DungeonInt2(0, 0), new DungeonInt2(1000, 0),
					a => { ClearAll(a); SetBit(a, 0, 0); },
					b => { ClearAll(b); SetBit(b, 0, 0); }
				},

                // 2) No overlap (separated on Y) - also catches maxY bugs
                {
					"No overlap on Y => false",
					new DungeonInt2(16, 16), new DungeonInt2(100, 100),
					new DungeonInt2(0, 0), new DungeonInt2(0, 1000),
					a => { ClearAll(a); SetBit(a, 0, 0); },
					b => { ClearAll(b); SetBit(b, 0, 0); }
				},

                // 3) Single-bit overlap at origin
                {
					"Single-bit overlap at (0,0) => true",
					new DungeonInt2(8, 8), new DungeonInt2(8, 8),
					new DungeonInt2(10, 10), new DungeonInt2(10, 10),
					a => { ClearAll(a); SetBit(a, 0, 0); },
					b => { ClearAll(b); SetBit(b, 0, 0); }
				},

                // 4) Single-bit non-overlap inside intersecting rect (different bits)
                {
					"Intersecting rect but bits don't overlap => false",
					new DungeonInt2(8, 8), new DungeonInt2(8, 8),
					new DungeonInt2(10, 10), new DungeonInt2(10, 10),
					a => { ClearAll(a); SetBit(a, 1, 1); },
					b => { ClearAll(b); SetBit(b, 2, 2); }
				},

                // 5) Offset start within a 32-bit block (xOffsetIndex = 1)
                {
					"Overlap when intersection starts at x offset 1 => true",
					new DungeonInt2(40, 4), new DungeonInt2(40, 4),
					new DungeonInt2(0, 0), new DungeonInt2(1, 0),
					a => { ClearAll(a); SetBit(a, 1, 0); }, // world x=1
                    b => { ClearAll(b); SetBit(b, 0, 0); }  // world x=1 (since bPosX=1)
                },

                // 6) Boundary at 31 (xOffsetIndex = 31)
                {
					"Overlap when intersection starts at x offset 31 => true",
					new DungeonInt2(64, 2), new DungeonInt2(64, 2),
					new DungeonInt2(0, 0), new DungeonInt2(31, 0),
					a => { ClearAll(a); SetBit(a, 31, 0); }, // world x=31
                    b => { ClearAll(b); SetBit(b, 0, 0); }   // world x=31
                },

                // 7) Crosses 32-bit boundary (bit at x=32), with intersection start unaligned
                {
					"Overlap across 32-bit boundary (x=32) => true",
					new DungeonInt2(96, 2), new DungeonInt2(96, 2),
					new DungeonInt2(0, 0), new DungeonInt2(1, 0),
					a => { ClearAll(a); SetBit(a, 32, 0); }, // world x=32
                    b => { ClearAll(b); SetBit(b, 31, 0); }  // b local 31 => world x = 1+31 = 32
                },

                // 8) Width 33 intersection (forces two 32-bit blocks), overlap in second block
                {
					"Intersection width 33, overlap in second block => true",
					new DungeonInt2(80, 1), new DungeonInt2(80, 1),
					new DungeonInt2(0, 0), new DungeonInt2(0, 0),
					a => { ClearAll(a); SetBit(a, 40, 0); },
					b => { ClearAll(b); SetBit(b, 40, 0); }
				},

                // 9) Multiple rows: overlap only on last row
                {
					"Overlap only on last intersecting row => true",
					new DungeonInt2(48, 10), new DungeonInt2(48, 10),
					new DungeonInt2(5, 5), new DungeonInt2(5, 5),
					a => { ClearAll(a); SetBit(a, 10, 9); },
					b => { ClearAll(b); SetBit(b, 10, 9); }
				},

                // 10) Multiple rows: no overlap but same column different rows
                {
					"Same x, different y => false",
					new DungeonInt2(48, 10), new DungeonInt2(48, 10),
					new DungeonInt2(5, 5), new DungeonInt2(5, 5),
					a => { ClearAll(a); SetBit(a, 10, 2); },
					b => { ClearAll(b); SetBit(b, 10, 3); }
				},
		};

	[Theory]
	[MemberData(nameof(Cases))]
	public void Test_matches_reference(
		string name,
		DungeonInt2 aSize,
		DungeonInt2 bSize,
		DungeonInt2 aPos,
		DungeonInt2 bPos,
		Action<CollisionMap> fillA,
		Action<CollisionMap> fillB)
	{
		var a = new CollisionMap(aSize);
		var b = new CollisionMap(bSize);

		// Ensure deterministic start (in case constructor data changes).
		ClearAll(a);
		ClearAll(b);

		fillA(a);
		fillB(b);

		bool expected = ReferenceOverlap(a, aPos, b, bPos);
		bool actual = CollisionMap.Test(a, aPos, b, bPos);

		Assert.True(
			expected == actual,
			$"{name}\nExpected={expected}, Actual={actual}\n" +
			$"aSize={aSize} aPos={aPos}\n" +
			$"bSize={bSize} bPos={bPos}");
	}

	[Fact]
	public void Set_then_Get_returns_true_and_Clear_returns_false()
	{
		var ht = new CollisionMap(new DungeonInt2(64, 4));
		ClearAll(ht);

		var p = new DungeonInt2(31, 2);
		Assert.False(ht.Get(p));

		ht.Set(p);
		Assert.True(ht.Get(p));

		ht.Clear(p);
		Assert.False(ht.Get(p));
	}

	[Fact]
	public void Randomized_smoke_matches_reference()
	{
		// This is a small randomized test to catch packing/offset bugs.
		// Keep it deterministic.
		var rng = new Random(12345);

		var a = new CollisionMap(new DungeonInt2(96, 16));
		var b = new CollisionMap(new DungeonInt2(96, 16));

		for (int iter = 0; iter < 50; iter++)
		{
			ClearAll(a);
			ClearAll(b);

			// Sprinkle random bits
			int aBits = rng.Next(1, 60);
			int bBits = rng.Next(1, 60);

			for (int i = 0; i < aBits; i++)
				a.Set(new DungeonInt2(rng.Next(0, a.Size.X), rng.Next(0, a.Size.Y)));

			for (int i = 0; i < bBits; i++)
				b.Set(new DungeonInt2(rng.Next(0, b.Size.X), rng.Next(0, b.Size.Y)));

			// Random world positions (including some negative to exercise offsets)
			var aPos = new DungeonInt2(rng.Next(-20, 21), rng.Next(-10, 11));
			var bPos = new DungeonInt2(rng.Next(-20, 21), rng.Next(-10, 11));

			bool expected = ReferenceOverlap(a, aPos, b, bPos);
			bool actual = CollisionMap.Test(a, aPos, b, bPos);

			Assert.Equal(expected, actual);
		}
	}
}