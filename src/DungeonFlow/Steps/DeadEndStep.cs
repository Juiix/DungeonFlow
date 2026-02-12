using System.Collections.Generic;

namespace DungeonFlow;

public sealed class DeadEndStep(int minLength, int maxLength, int chance, int[] roomIds, NodeQuery targets) : IGenerationStep
{
	private readonly int _minLength = minLength;
	private readonly int _maxLength = maxLength;
	private readonly int _chance = chance;
	private readonly int[] _roomIds = roomIds;
	private readonly NodeQuery _targets = targets;
	private readonly List<ushort> _workingList = [];

	public bool Run(DungeonGenerator generator)
	{
		_workingList.Clear();
		foreach (var nodeId in generator.Graph.Query(_targets))
		{
			_workingList.Add(nodeId);
		}

		for (int i = 0; i < _workingList.Count; i++)
		{
			if (generator.Random.Next(0, 100) >= _chance)
				continue;

			var currentNodeId = _workingList[i];
			var length = generator.Random.Next(_minLength, _maxLength + 1);
			var depth = generator.Graph.GetNode(currentNodeId).Depth;
			for (int j = 0; j < length; j++)
			{
				int tries = 0;
				while (tries++ < 100)
				{
					var roomId = _roomIds[generator.Random.Next(0, _roomIds.Length)];
					if (generator.TryExpand(out var nodeId, roomId, currentNodeId))
					{
						currentNodeId = nodeId;
						break;
					}
				}
			}
		}
		return true;
	}
}
