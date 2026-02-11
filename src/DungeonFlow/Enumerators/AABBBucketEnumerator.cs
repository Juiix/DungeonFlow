using System;

namespace DungeonFlow;

public ref struct AABBBucketEnumerator
{
	private readonly Span<AABBBucket> _buckets;
	private readonly int _bucketWidth;
	private readonly DungeonInt2 _min;
	private readonly DungeonInt2 _max;
	private DungeonInt2 _current;

	public AABBBucketEnumerator(Span<AABBBucket> buckets, int bucketWidth, DungeonInt2 min, DungeonInt2 max)
	{
		_buckets = buckets;
		_bucketWidth = bucketWidth;
		_min = min;
		_max = max;
		_current = new DungeonInt2(_min.X - 1, _min.Y);
	}

	public bool MoveNext()
	{
		unchecked
		{
			_current.X++;
			if (_current.X > _max.X)
			{
				_current.X = _min.X;
				_current.Y++;
			}

			if (_current.Y > _max.Y)
				return false;

			return true;
		}

	}

	public void Reset()
	{
		_current = new DungeonInt2(_min.X - 1, _min.Y);
	}

	public readonly ref AABBBucket Current
	{
		get => ref _buckets[_current.Y * _bucketWidth + _current.X];
	}
}
