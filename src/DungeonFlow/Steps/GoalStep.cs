namespace DungeonFlow;

public sealed class GoalStep(int length, int[] goalRoomIds, int[] pathRoomIds, NodeQuery target) : IGenerationStep
{
	private readonly int _length = length;
	private readonly int[] _goalRoomIds = goalRoomIds;
	private readonly int[] _pathRoomIds = pathRoomIds;
	private readonly NodeQuery _target = target;
	private readonly ushort[] _path = new ushort[length + 1];
	private readonly int[] _tries = new int[length + 1];

	public bool Run(DungeonGenerator generator)
	{
		int index = 0;
		_tries[index] = 0;
		_path[index] = generator.Graph.Query(_target).First();

		while (index < _length)
		{
			var currentNodeId = _path[index];
			if (_tries[index]++ >= 100)
			{
				if (--index < 0)
					break;
				generator.Graph.RemoveNode(currentNodeId);
				continue;
			}

			var roomIds = index + 1 == _length
				? _goalRoomIds
				: _pathRoomIds;
			var roomId = roomIds[generator.Random.Next(0, roomIds.Length)];
			if (!generator.TryExpand(out var nodeId, roomId, currentNodeId))
				continue;

			index++;
			_path[index] = nodeId;
			_tries[index] = 0;
		}

		return index == _length;
	}
}