namespace DungeonFlow;

public sealed class GoalStep(int maxDepth, int goalRoomId, int[] pathRoomIds, NodeQuery target) : IGenerationStep
{
	private readonly int _maxDepth = maxDepth;
	private readonly int _goalRoomId = goalRoomId;
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

			var roomId = depth + 1 == _maxDepth
				? _goalRoomId
				: _pathRoomIds[generator.Random.Next(0, _pathRoomIds.Length)];
			var nodeId = generator.TryExpand(roomId, currentNodeId);
			if (nodeId == null)
				continue;

			index++;
			depth++;
			_path[index] = nodeId.Value;
			_tries[index] = 0;
		}

		return depth == _maxDepth;
	}
}