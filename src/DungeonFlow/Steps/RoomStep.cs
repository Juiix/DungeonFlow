using System.Collections.Generic;

namespace DungeonFlow;

public sealed class RoomStep(LinkDirection? direction, int[] roomIds, NodeQuery target) : IGenerationStep
{
	private readonly LinkDirection? _direction = direction;
	private readonly int[] _roomIds = roomIds;
	private readonly NodeQuery _target = target;
	private readonly Stack<(ushort NodeIndex, int Depth)> _stack = [];

	public bool Run(DungeonGenerator generator)
	{
		foreach (var parentIndex in generator.Graph.Query(_target))
		{
			var tries = 0;
			while (tries++ < 100)
			{
				var roomId = generator.Random.Next(0, _roomIds.Length);
				var nodeIndex = generator.TryExpand(roomId, parentIndex, _direction);
				if (nodeIndex != null)
					break;
			}
		}
		return true;
	}
}
