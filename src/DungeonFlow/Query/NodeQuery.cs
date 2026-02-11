namespace DungeonFlow;

public struct NodeQuery(QueryDirection direction, int minDepth, int maxDepth, int maxCount)
{
	public QueryDirection Direction { get; set; } = direction;
	public int MinDepth { get; set; } = minDepth;
	public int MaxDepth { get; set; } = maxDepth;
	public int MaxCount { get; set; } = maxCount;

	public static NodeQuery First => new(QueryDirection.Forward, 0, 0, 1);
	public static NodeQuery Last => new(QueryDirection.Backward, 0, int.MaxValue, 1);
	public static NodeQuery All => new(QueryDirection.Forward, 0, int.MaxValue, int.MaxValue);
}