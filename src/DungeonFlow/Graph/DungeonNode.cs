using System;

namespace DungeonFlow;

public struct DungeonNode(DungeonRect rect, int roomId, ushort depth, ushort parentId = ushort.MaxValue)
{
	public readonly DungeonRect Rect = rect;
	public readonly int RoomId = roomId;
	public readonly ushort Depth = depth;
	public ushort ParentId = parentId;
	public ushort LinkCount = 0;
	public ushort LinkId0 = ushort.MaxValue;
	public ushort LinkId1 = ushort.MaxValue;
	public ushort LinkId2 = ushort.MaxValue;
	public ushort LinkId3 = ushort.MaxValue;

	public ushort this[int index]
	{
		readonly get
		{
			return index switch
			{
				0 => LinkId0,
				1 => LinkId1,
				2 => LinkId2,
				3 => LinkId3,
				_ => throw new IndexOutOfRangeException()
			};
		}
		set
		{
			switch (index)
			{
				case 0:	LinkId0 = value; break;
				case 1:	LinkId1 = value; break;
				case 2:	LinkId2 = value; break;
				case 3:	LinkId3 = value; break;
				default: throw new IndexOutOfRangeException();
			}
		}
	}
}
