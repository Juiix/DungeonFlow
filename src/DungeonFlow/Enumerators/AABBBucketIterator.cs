using System;
using System.Runtime.CompilerServices;

namespace DungeonFlow;

public readonly ref struct AABBBucketIterator
{
	private readonly Span<AABBBucket> _buckets;
	private readonly int _bucketWidth;
	private readonly DungeonInt2 _min;
	private readonly DungeonInt2 _max;

	public AABBBucketIterator(Span<AABBBucket> buckets, int bucketWidth, DungeonInt2 min, DungeonInt2 max)
	{
		_buckets = buckets;
		_bucketWidth = bucketWidth;
		_min = min;
		_max = max;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public AABBBucketEnumerator GetEnumerator()
	{
		return new(_buckets, _bucketWidth, _min, _max);
	}
}
