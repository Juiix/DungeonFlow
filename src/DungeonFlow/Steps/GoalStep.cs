namespace DungeonFlow;

public sealed class GoalStep(int maxDepth, int[] goalRoomIds, int[] pathRoomIds, NodeQuery target) : IGenerationStep
{
	private readonly int _maxDepth = maxDepth;
	private readonly int[] _goalRoomIds = goalRoomIds;
	private readonly int[] _pathRoomIds = pathRoomIds;
	private readonly NodeQuery _target = target;
	private readonly ushort[] _path = new ushort[maxDepth + 1];
	private readonly int[] _tries = new int[maxDepth + 1];

	public bool Run(DungeonGenerator generator)
	{
		int index = 0;
		_tries[index] = 0;
		_path[index] = generator.Graph.Query(_target).First();
		int depth = generator.Graph.GetNode(_path[index]).Depth;

		while (depth < _maxDepth)
		{
			var currentNodeId = _path[index];
			if (_tries[index]++ >= 100)
			{
				if (--index < 0)
					break;
				depth--;
				generator.Graph.RemoveNode(currentNodeId);
				continue;
			}

			var roomIds = depth + 1 == _maxDepth
				? _goalRoomIds
				: _pathRoomIds;
			var roomId = roomIds[generator.Random.Next(0, roomIds.Length)];
			if (!generator.TryExpand(out var nodeId, roomId, currentNodeId))
				continue;

			index++;
			depth++;
			_path[index] = nodeId;
			_tries[index] = 0;
		}

		return depth == _maxDepth;
	}
}