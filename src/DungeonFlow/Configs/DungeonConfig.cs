using System;

namespace DungeonFlow;

public sealed class DungeonConfig(
	Func<int, RoomConfig> getRoom,
	Func<int, HallConfig> getHall)
{
	public Func<int, RoomConfig> GetRoom { get; } = getRoom;
	public Func<int, HallConfig> GetHall { get; } = getHall;
}
