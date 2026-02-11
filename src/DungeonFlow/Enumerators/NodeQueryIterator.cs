using System;
using System.Runtime.CompilerServices;

namespace DungeonFlow;

public readonly ref struct NodeQueryIterator(ReadOnlySpan<DungeonNode> nodes, ReadOnlySpan<ushort> map, NodeQuery query)
{
	private readonly ReadOnlySpan<DungeonNode> _nodes = nodes;
	private readonly ReadOnlySpan<ushort> _map = map;
	private readonly NodeQuery _query = query;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodeQueryEnumerator GetEnumerator()
	{
		return new(_nodes, _map, _query);
	}

	public ushort First()
	{
		foreach (var id in this)
		{
			return id;
		}
		throw new InvalidOperationException("Query result is empty");
	}
}
