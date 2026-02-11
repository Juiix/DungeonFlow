namespace DungeonFlow;

public readonly struct DungeonLink(DungeonRect rect, int hallId, ushort parentId, ushort childId)
{
	public readonly DungeonRect Rect = rect;
	public readonly int HallId = hallId;
	public readonly ushort ParentId = parentId;
	public readonly ushort ChildId = childId;
}
