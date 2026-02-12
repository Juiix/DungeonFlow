using System;

namespace DungeonFlow;

public sealed class DungeonGenerator(DungeonConfig config, Random random)
{
	public DungeonConfig Config { get; } = config;
	public DungeonGraph Graph { get; } = new(256, new DungeonInt2(512, 512));
	public Random Random { get; } = random;

	private DungeonRect Bounds => new DungeonRect(DungeonInt2.Zero, Graph.Size);

	public void Clear()
	{
		Graph.Clear();
	}

	public bool TryExpand(out ushort nodeId, int roomId, ushort parentNodeId, LinkDirection direction = LinkDirection.Undefined)
	{
		ref readonly var parent = ref Graph.GetNode(parentNodeId);
		var roomConfig = Config.GetRoom(roomId);
		if (direction == LinkDirection.Undefined)
			direction = roomConfig.GetLinkDirection(Random);
		var hallId = roomConfig.GetHall(Random);
		var hallConfig = Config.GetHall(hallId);
		var (room, hall) = CreateLinkedRoom(in parent, direction, roomConfig, hallConfig);
		if (IsValidRect(room, roomConfig) && IsValidRect(hall))
		{
			Graph.AddNode(out nodeId, room, roomId, parentNodeId);
			Graph.AddLink(out _, hall, hallId, parentNodeId, nodeId);
			return true;
		}
		nodeId = ushort.MaxValue;
		return false;
	}

	public bool TryPlace(out ushort nodeId, int roomId, DungeonInt2 center)
	{
		var roomConfig = Config.GetRoom(roomId);
		var roomWidth = roomConfig.GetWidth(Random);
		var roomHeight = roomConfig.GetHeight(Random);
		var roomSize = new DungeonInt2(roomWidth, roomHeight);
		var room = new DungeonRect(center - roomSize / 2, roomSize);
		if (IsValidRect(room))
		{
			Graph.AddNode(out nodeId, room, roomId);
			return true;
		}
		nodeId = ushort.MaxValue;
		return false;
	}

	private (DungeonRect Room, DungeonRect Hall) CreateLinkedRoom(in DungeonNode parentNode, LinkDirection direction, RoomConfig dstRoomConfig, HallConfig hallConfig)
	{
		var srcRoomConfig = Config.GetRoom(parentNode.RoomId);
		var sourceRect = parentNode.Rect;

		var roomWidth = dstRoomConfig.GetWidth(Random);
		var roomHeight = dstRoomConfig.GetHeight(Random);
		var roomSize = new DungeonInt2(roomWidth, roomHeight);

		int hallWidth = hallConfig.GetWidth(Random);
		var hallLength = hallConfig.GetHeight(Random);
		var hallSize = direction switch
		{
			LinkDirection.Right or LinkDirection.Left => new DungeonInt2(hallLength, hallWidth),
			_ => new DungeonInt2(hallWidth, hallLength)
		};

		var hallOffset = direction switch
		{
			LinkDirection.Right => new DungeonInt2(1, 0),
			LinkDirection.Up => new DungeonInt2(0, 1),
			LinkDirection.Left => new DungeonInt2(-1, 0),
			LinkDirection.Down => new DungeonInt2(0, -1),
			_ => DungeonInt2.Zero,
		};
		var hallStartPosition = sourceRect.Position + srcRoomConfig.GetLinkPosition(Random, new(direction, sourceRect.Size, hallSize));
		var hallEndPosition = hallStartPosition + hallSize * hallOffset;
		var hallPosition = direction switch
		{
			LinkDirection.Left or LinkDirection.Down => hallEndPosition,
			_ => hallStartPosition
		};

		var inverseDirection = (LinkDirection)(((int)direction + 2) % 4);
		var roomPosition = hallEndPosition - dstRoomConfig.GetLinkPosition(Random, new(inverseDirection, roomSize, hallSize));

		var room = new DungeonRect(roomPosition, roomSize);
		var hall = new DungeonRect(hallPosition, hallSize);
		return (room, hall);
	}

	private bool IsValidRect(DungeonRect rect, RoomConfig roomConfig)
	{
		var roomCollision = roomConfig.Collision;
		if (roomCollision is null)
			return IsValidRect(rect);

		if (!Bounds.Envelopes(rect))
			return false;

		foreach (var neighbor in Graph.GetNearbyEntries(rect))
		{
			if (!neighbor.Rect.Intersects(rect))
				continue;

			var didCollide = neighbor.Type switch
			{
				AABBEntryType.Node => Config.GetRoom(Graph.GetNode(neighbor.Id).RoomId).Collision is { } neighborCollision
					? CollisionMap.Test(roomCollision, rect.Position, neighborCollision, neighbor.Rect.Position)
					: CollisionMap.Test(roomCollision, rect.Position, neighbor.Rect),
				AABBEntryType.Link => CollisionMap.Test(roomCollision, rect.Position, neighbor.Rect),
				_ => true
			};

			if (didCollide)
				return false;
		}
		return true;
	}

	private bool IsValidRect(DungeonRect rect)
	{
		if (!Bounds.Envelopes(rect))
			return false;

		foreach (var neighbor in Graph.GetNearbyEntries(rect))
		{
			if (!neighbor.Rect.Intersects(rect))
				continue;

			if (neighbor.Type == AABBEntryType.Node &&
				Config.GetRoom(Graph.GetNode(neighbor.Id).RoomId).Collision is { } neighborCollision &&
				!CollisionMap.Test(neighborCollision, neighbor.Rect.Position, rect))
			{
				continue;
			}

			return false;
		}
		return true;
	}
}
