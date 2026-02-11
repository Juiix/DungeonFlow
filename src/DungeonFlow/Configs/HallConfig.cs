using System;

namespace DungeonFlow;

public sealed class HallConfig(
	Func<Random, int> getWidth,
	Func<Random, int> getHeight)
{
	public Func<Random, int> GetWidth { get; } = getWidth;
	public Func<Random, int> GetHeight { get; } = getHeight;
}
