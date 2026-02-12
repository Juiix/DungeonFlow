namespace DungeonFlow;

public readonly struct DungeonLink(DungeonRect rect, int hallId, LinkDirection direction, ushort parentId, ushort childId)
{
	public readonly DungeonRect Rect = rect;
	public readonly int HallId = hallId;
	public readonly LinkDirection Direction = direction;
	public readonly ushort ParentId = parentId;
	public readonly ushort ChildId = childId;
}
