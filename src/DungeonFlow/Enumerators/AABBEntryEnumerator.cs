using System;

namespace DungeonFlow;

public ref struct AABBEntryEnumerator
{
	private readonly Span<AABBBucket> _buckets;
	private readonly int _bucketWidth;
	private readonly DungeonInt2 _min;
	private readonly DungeonInt2 _max;
	private DungeonInt2 _current;
	private Span<AABBEntry> _entries;
	private int _index;

	public AABBEntryEnumerator(Span<AABBBucket> buckets, int bucketWidth, DungeonInt2 min, DungeonInt2 max)
	{
		_buckets = buckets;
		_bucketWidth = bucketWidth;
		_min = min;
		_max = max;
		_current = new DungeonInt2(_min.X - 1, _min.Y);
		_index = -1;
	}

	public bool MoveNext()
	{
		unchecked
		{
			while (true)
			{
				if (--_index >= 0)
				{
					return true;
				}

				_current.X++;
				if (_current.X > _max.X)
				{
					_current.X = _min.X;
					_current.Y++;
				}

				if (_current.Y > _max.Y)
					return false;

				ref var bucket = ref _buckets[_current.Y * _bucketWidth + _current.X];
				_entries = bucket.Entries != null ? bucket.Entries.AsSpan(0, bucket.Count) : Span<AABBEntry>.Empty;
				_index = bucket.Count;
			}
		}

	}

	public void Reset()
	{
		_current = new DungeonInt2(_min.X - 1, _min.Y);
		_index = -1;
	}

	public readonly ref readonly AABBEntry Current
	{
		get => ref _entries[_index];
	}
}
