namespace DungeonFlow.Renderer
{
	public partial class DungeonFlowRenderer : Form
	{
		public DungeonFlowRenderer()
		{
			InitializeComponent();
		}

		private void Generate_Click(object sender, EventArgs e)
		{
			RoomConfig[] rooms = [
				new(r => r.Next(16, 16), r => r.Next(14, 16), r => 0, r => (LinkDirection)r.Next(0, 4), GetRandomHallPosition, null),
				new(r => r.Next(18, 24), r => r.Next(18, 24), r => 0, r => (LinkDirection)r.Next(0, 4), GetRandomHallPosition, null),
				new(r => 32, r => 32, r => 1, r => (LinkDirection)r.Next(0, 4), GetRandomHallPosition, null),
			];

			HallConfig[] halls = [
				new(r => 4, r => r.Next(12, 24)),
				new(r => 6, r => r.Next(8, 12)),
			];

			IGenerationStep[] steps = [
				new StartStep([0]),
				new GoalStep(28, [2], [1], NodeQuery.First),
				new GoalStep(14, [2], [1], NodeQuery.First),
				new GoalStep(6, [2], [1], new NodeQuery(QueryDirection.Forward, 2, 4, 1)),
				new DeadEndStep(1, 2, 13, 50, [1], new NodeQuery(QueryDirection.Forward, 1, 13, int.MaxValue))
			];

			var config = new DungeonConfig(i => rooms[i], i => halls[i]);
			var random = Random.Shared;
			var generator = new DungeonGenerator(config, random);

			bool success = false;
			int tries = 0;
			while (!success && tries++ < 100)
			{
				generator.Clear();
				foreach (var step in steps)
				{
					success = step.Run(generator);
					if (!success)
						break;
				}
			}

			DungeonPreview.Image = RenderGraph(generator.Graph);
		}

		private static DungeonInt2 GetRandomHallPosition(Random random, LinkContext context)
		{
			return context.Direction switch
			{
				LinkDirection.Right => new DungeonInt2(context.RoomSize.X, random.Next(0, context.RoomSize.Y - context.HallSize.Y)),
				LinkDirection.Up => new DungeonInt2(random.Next(0, context.RoomSize.X - context.HallSize.X), context.RoomSize.Y),
				LinkDirection.Left => new DungeonInt2(0, random.Next(0, context.RoomSize.Y - context.HallSize.Y)),
				LinkDirection.Down => new DungeonInt2(random.Next(0, context.RoomSize.X - context.HallSize.X), 0),
				_ => throw new InvalidOperationException("Unhandled LinkDirection: " + context.Direction)
			};
		}

		private static Bitmap RenderGraph(DungeonGraph graph)
		{
			var bitmap = new Bitmap(graph.Size.X, graph.Size.Y);
			foreach (ref readonly var node in graph.Nodes)
			{
				var color = Color.Yellow;
				if (node.ParentId == ushort.MaxValue) color = Color.Green;
				if (node.RoomId == 2) color = Color.Red;
				DrawRect(bitmap, node.Rect, color);
			}

			foreach (ref readonly var link in graph.Links)
			{
				DrawRect(bitmap, link.Rect, Color.Blue);
			}
			return bitmap;
		}

		private static void DrawRect(Bitmap bitmap, DungeonRect rect, Color color)
		{
			for (int y = 0; y < rect.Height; y++)
			{
				for (int x = 0; x < rect.Width; x++)
				{
					bitmap.SetPixel(rect.Position.X + x, rect.Position.Y + y, color);
				}
			}
		}
	}
}
