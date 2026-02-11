namespace DungeonFlow;

public readonly struct LinkContext(LinkDirection direction, DungeonInt2 roomSize, DungeonInt2 hallSize)
{
	public readonly LinkDirection Direction = direction;
	public readonly DungeonInt2 RoomSize = roomSize;
	public readonly DungeonInt2 HallSize = hallSize;
}
