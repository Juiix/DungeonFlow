using System;

namespace DungeonFlow;

public ref struct NodeQueryEnumerator(ReadOnlySpan<DungeonNode> nodes, ReadOnlySpan<ushort> map, NodeQuery query)
{
	private readonly ReadOnlySpan<DungeonNode> _nodes = nodes;
	private readonly ReadOnlySpan<ushort> _map = map;
	private readonly NodeQuery _query = query;
	private int _current = nodes.Length;
	private int _count = 0;

	public bool MoveNext()
	{
		unchecked
		{
			while (--_current >= 0 && _count < _query.MaxCount)
			{
				var index = _query.Direction == QueryDirection.Forward
					? _nodes.Length - _current - 1
					: _current;
				ref readonly var node = ref _nodes[index];
				if (node.Depth >= _query.MinDepth &&
					node.Depth <= _query.MaxDepth)
				{
					_count++;
					return true;
				}
			}
			return false;
		}

	}

	public void Reset()
	{
		_current = _nodes.Length;
		_count = 0;
	}

	public readonly ushort Current
	{
		get => _map[Index];
	}

	private readonly int Index => _query.Direction == QueryDirection.Forward
			? _nodes.Length - _current - 1
			: _current;
}
