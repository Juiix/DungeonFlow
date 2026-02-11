using System;

namespace DungeonFlow;

public interface IGenerationStep
{
	bool Run(DungeonGenerator generator);
}
