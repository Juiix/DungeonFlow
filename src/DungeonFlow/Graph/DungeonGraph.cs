using System;

namespace DungeonFlow;

public sealed class DungeonGraph(int initialCapacity, DungeonInt2 size)
{
	private DungeonNode[] _nodes = new DungeonNode[initialCapacity];
	private DungeonLink[] _links = new DungeonLink[initialCapacity];
	private readonly AABBBucket[] _aabb = CreateAABBBuckets(size);
	private readonly DungeonInt2 _bucketSize = new((size.X + 31) >> 5, (size.Y + 31) >> 5);
	private ushort[] _nodeMap = new ushort[initialCapacity];
	private ushort[] _linkMap = new ushort[initialCapacity];
	private ushort[] _nodeReverseMap = new ushort[initialCapacity];
	private ushort[] _linkReverseMap = new ushort[initialCapacity];
	private ushort _nodeIdCount = 0;
	private ushort _linkIdCount = 0;

	public ushort NodeCount { get; private set; } = 0;
	public ushort LinkCount { get; private set; } = 0;
	public DungeonInt2 Size { get; } = size;
	public ReadOnlySpan<DungeonNode> Nodes => _nodes.AsSpan(0, NodeCount);
	public ReadOnlySpan<DungeonLink> Links => _links.AsSpan(0, LinkCount);

	public void Clear()
	{
		NodeCount = 0;
		LinkCount = 0;
		for (int i = 0; i < _aabb.Length; i++)
			_aabb[i].Count = 0;
		_nodeIdCount = 0;
		_linkIdCount = 0;
	}

	public ref DungeonNode AddNode(out ushort nodeId, DungeonRect rect, int roomId, ushort parentNodeId = ushort.MaxValue)
	{
		var parentIndex = parentNodeId == ushort.MaxValue
			? ushort.MaxValue
			: _nodeMap[parentNodeId];
		if (parentNodeId != ushort.MaxValue && parentIndex >= NodeCount)
			throw new IndexOutOfRangeException("Parent node index out of range");

		ushort nodeIndex;
		(nodeId, nodeIndex) = NextNodeSlot();

		AddAABB(new AABBEntry(AABBEntryType.Node, nodeId, rect));

		var depth = (ushort)(parentNodeId == ushort.MaxValue ? 0 : _nodes[parentIndex].Depth + 1);
		ref var node = ref _nodes[nodeIndex];
		node = new DungeonNode(rect, roomId, depth, parentNodeId);
		return ref node;
	}

	public ref DungeonLink AddLink(out ushort linkId, DungeonRect rect, int hallId, ushort parentNodeId, ushort childNodeId)
	{
		var parentIndex = _nodeMap[parentNodeId];
		var childIndex = _nodeMap[childNodeId];
		if (parentIndex >= NodeCount ||
			childIndex >= NodeCount)
			throw new IndexOutOfRangeException("Node index out of range");

		if (parentNodeId == childNodeId)
			throw new InvalidOperationException("Cannot link a node to itself");

		ref var parent = ref _nodes[parentIndex];
		if (parent.LinkCount >= 4)
			throw new InvalidOperationException("Cannot link more than 4 children");

		ushort linkIndex;
		(linkId, linkIndex) = NextLinkSlot();

		AddAABB(new AABBEntry(AABBEntryType.Link, linkId, rect));

		parent[parent.LinkCount++] = linkId;
		ref var link = ref _links[linkIndex];
		link = new DungeonLink(rect, hallId, parentNodeId, childNodeId);
		return ref link;
	}

	public ref DungeonNode GetNode(ushort nodeId)
	{
		return ref _nodes[_nodeMap[nodeId]];
	}

	public ref DungeonLink GetLink(ushort linkId)
	{
		return ref _links[_linkMap[linkId]];
	}

	public void RemoveNode(ushort nodeId)
	{
		var nodeIndex = _nodeMap[nodeId];
		if (nodeIndex >= NodeCount)
			throw new IndexOutOfRangeException("Node index out of range");

		ref var node = ref _nodes[nodeIndex];

		// remove parent -> this links
		if (node.ParentId != ushort.MaxValue)
		{
			ref var parent = ref _nodes[node.ParentId];
			var parentLinkIndex = IndexOfChildLink(ref parent, nodeId);
			RemoveLink(ref parent, parentLinkIndex);
		}

		// remove child links
		for (int i = node.LinkCount - 1; i >= 0; i--)
		{
			RemoveLink(ref node, i);
		}

		// swapback link index
		var lastIndex = --NodeCount;
		var lastId = _nodeReverseMap[lastIndex];
		_nodes[nodeIndex] = _nodes[lastIndex];
		(_nodeMap[lastId], _nodeMap[nodeId]) = (lastIndex, nodeIndex);
		(_nodeReverseMap[nodeIndex], _nodeReverseMap[lastIndex]) = (lastId, nodeId);
	}

	public void RemoveLink(ushort linkId)
	{
		var linkIndex = _linkMap[linkId];
		if (linkIndex >= LinkCount)
			throw new IndexOutOfRangeException("Link index out of range");

		ref var link = ref _links[linkIndex];

		var parentIndex = _nodeMap[link.ParentId];
		var childIndex = _nodeMap[link.ChildId];

		ref var parent = ref _nodes[parentIndex];
		ref var child = ref _nodes[childIndex];

		// dereference
		child.ParentId = ushort.MaxValue;

		var parentLinkIndex = IndexOfChildLink(ref parent, link.ChildId);
		parent[parentLinkIndex] = parent[parent.LinkCount - 1];
		parent[parent.LinkCount - 1] = ushort.MaxValue;
		parent.LinkCount--;

		// swapback link index
		var lastIndex = --LinkCount;
		var lastId = _linkReverseMap[lastIndex];
		_links[linkIndex] = _links[lastIndex];
		(_linkMap[lastId], _linkMap[linkId]) = (lastIndex, linkIndex);
		(_linkReverseMap[linkIndex], _linkReverseMap[lastIndex]) = (lastId, linkId);
	}

	public AABBEntryIterator GetNearbyEntries(DungeonRect aabb)
	{
		var (min, max) = GetBucketMinMax(aabb, _bucketSize);
		return new AABBEntryIterator(_aabb, _bucketSize.X, min, max);
	}

	public NodeQueryIterator Query(NodeQuery query)
	{
		return new NodeQueryIterator(_nodes.AsSpan(0, NodeCount), _nodeReverseMap.AsSpan(0, NodeCount), query);
	}

	private static AABBBucket[] CreateAABBBuckets(DungeonInt2 size)
	{
		var bucketSizeX = (size.X + 31) >> 5;
		var bucketSizeY = (size.Y + 31) >> 5;
		var bucketCount = bucketSizeX * bucketSizeY;
		return new AABBBucket[bucketCount];
	}

	private static (DungeonInt2 Min, DungeonInt2 Max) GetBucketMinMax(DungeonRect aabb, DungeonInt2 bucketSize)
	{
		var min = aabb.Min;
		var max = aabb.Max;
		var bucketMin = new DungeonInt2(min.X >> 5, min.Y >> 5);
		var bucketMax = new DungeonInt2((max.X - 1) >> 5, (max.Y - 1) >> 5); // max is inclusive
		bucketMin = new DungeonInt2(
			Math.Clamp(bucketMin.X, 0, bucketSize.X),
			Math.Clamp(bucketMin.Y, 0, bucketSize.Y)
		);
		bucketMax = new DungeonInt2(
			Math.Clamp(bucketMax.X, 0, bucketSize.X),
			Math.Clamp(bucketMax.Y, 0, bucketSize.Y)
		);
		return (bucketMin, bucketMax);
	}

	private void AddAABB(AABBEntry entry)
	{
		foreach (ref var bucket in GetAABBBuckets(entry.Rect))
		{
			bucket.Entries ??= new AABBEntry[16];
			var bucketIndex = bucket.Count++;
			if (bucket.Count > bucket.Entries.Length)
				Array.Resize(ref bucket.Entries, bucket.Entries.Length * 2);
			bucket.Entries[bucketIndex] = entry;
		}
	}

	private AABBBucketIterator GetAABBBuckets(DungeonRect aabb)
	{
		var (min, max) = GetBucketMinMax(aabb, _bucketSize);
		return new AABBBucketIterator(_aabb, _bucketSize.X, min, max);
	}

	private int IndexOfChildLink(ref DungeonNode parent, int childId)
	{
		for (int i = 0; i < parent.LinkCount; i++)
		{
			var linkId = parent[i];
			var linkIndex = _linkMap[linkId];
			ref var link = ref _links[linkIndex];
			if (link.ChildId == childId)
			{
				return i;
			}
		}
		return -1;
	}

	private (ushort Id, ushort Index) NextNodeSlot()
	{
		if (NodeCount == ushort.MaxValue)
			throw new InvalidOperationException("Maximum total nodes reached");
		var index = NodeCount++;
		if (index >= _nodes.Length)
		{
			Array.Resize(ref _nodes, _nodes.Length * 2);
			Array.Resize(ref _nodeMap, _nodeMap.Length * 2);
			Array.Resize(ref _nodeReverseMap, _nodeReverseMap.Length * 2);
		}

		var id = index >= _nodeIdCount ? _nodeIdCount++ : _nodeReverseMap[index];
		_nodeReverseMap[index] = id;
		_nodeMap[id] = index;
		return (id, index);
	}

	private (ushort Id, ushort Index) NextLinkSlot()
	{
		if (LinkCount == ushort.MaxValue)
			throw new InvalidOperationException("Maximum total links reached");
		var index = LinkCount++;
		if (index >= _links.Length)
		{
			Array.Resize(ref _links, _links.Length * 2);
			Array.Resize(ref _linkMap, _linkMap.Length * 2);
			Array.Resize(ref _linkReverseMap, _linkReverseMap.Length * 2);
		}

		var id = index >= _linkIdCount ? _linkIdCount++ : _linkReverseMap[index];
		_linkReverseMap[index] = id;
		_linkMap[id] = index;
		return (id, index);
	}

	private void RemoveLink(ref DungeonNode parent, int parentLinkIndex)
	{
		var linkId = parent[parentLinkIndex];
		var linkIndex = _linkMap[linkId];
		ref var link = ref _links[linkIndex];

		var childIndex = _nodeMap[link.ChildId];
		ref var child = ref _nodes[childIndex];

		// dereference
		child.ParentId = ushort.MaxValue;
		parent[parentLinkIndex] = parent[parent.LinkCount - 1];
		parent[parent.LinkCount - 1] = ushort.MaxValue;
		parent.LinkCount--;

		// swapback link index
		var lastIndex = --LinkCount;
		var lastId = _linkReverseMap[lastIndex];
		_links[linkIndex] = _links[lastIndex];
		(_linkMap[lastId], _linkMap[linkId]) = (lastIndex, linkIndex);
		(_linkReverseMap[linkIndex], _linkReverseMap[lastIndex]) = (lastId, linkId);
	}
}
