namespace DungeonFlow;

public sealed class StartStep(int[] roomIds) : IGenerationStep
{
	private readonly int[] _roomIds = roomIds;

	public bool Run(DungeonGenerator generator)
	{
		var tries = 0;
		while (tries++ < 100)
		{
			var roomId = generator.Random.Next(0, _roomIds.Length);
			var center = generator.Graph.Size / 2;
			var nodeIndex = generator.TryPlace(roomId, center);
			if (nodeIndex != null)
				break;
		}
		return true;
	}
}