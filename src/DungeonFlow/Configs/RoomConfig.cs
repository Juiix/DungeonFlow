using System;

namespace DungeonFlow;

public sealed class RoomConfig(
	Func<Random, int> getWidth,
	Func<Random, int> getHeight,
	Func<Random, int> getHall,
	LinkDirectionMask allowedDirections,
	Func<Random, LinkContext, DungeonInt2> getLinkPosition,
	CollisionMap? collision = null)
{
	public Func<Random, int> GetWidth { get; } = getWidth;
	public Func<Random, int> GetHeight { get; } = getHeight;
	public Func<Random, int> GetHall { get; } = getHall;
	public LinkDirectionMask AllowedDirections { get; } = allowedDirections;
	public Func<Random, LinkContext, DungeonInt2> GetLinkPosition { get; } = getLinkPosition;
	public CollisionMap? Collision { get; } = collision;
}
