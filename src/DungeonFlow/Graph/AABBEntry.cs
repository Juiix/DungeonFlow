namespace DungeonFlow;

public readonly struct AABBEntry(AABBEntryType type, ushort id, DungeonRect rect)
{
	public readonly AABBEntryType Type = type;
	public readonly ushort Id = id;
	public readonly DungeonRect Rect = rect;
}
