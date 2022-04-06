namespace Prototype.Pathfinding;

public class Grid
{
	public event EventHandler<OnGridValueChangedEventArgs>? OnGridValueChanged;

	public class OnGridValueChangedEventArgs : EventArgs
	{
		public int X;
		public int Y;
		public PathNode PathNode;

		public OnGridValueChangedEventArgs(int x, int y, PathNode pathNode)
		{
			this.X = x;
			this.Y = y;
			this.PathNode = pathNode;
		}
	}

	public readonly int Width;
	public readonly int Height;
	private readonly PathNode[,] gridArray;

	public Grid(
		int width,
		int height,
		Func<Grid, int, int, PathNode> createGridObject
	) //Func fügt ein default value für die celle ein - Grid<TGridObject>, int, int, TGridObject> Method signiture, for Updating the cells
	{
		this.Width = width;
		this.Height = height;

		this.gridArray = new PathNode[width, height];

		for (var x = 0; x < this.gridArray.GetLength(0); x++)
		for (var y = 0; y < this.gridArray.GetLength(1); y++)
			this.gridArray[x, y] = createGridObject(this, x, y); //setting default object
	}

	public PathNode? GetGridObject(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < this.Width && y < this.Height)
			return this.gridArray[x, y];
		
		return null; //Return the default value of TGridObject
	}

	public void TriggerGridObjectChanged(int x, int y, PathNode pathNode)
	{
		this.OnGridValueChanged?.Invoke(this, new(x,y,pathNode));
	}

	public void ToggleWalkable(int x, int y)
	{
		var node = this.GetGridObject(x, y);
		node?.SetIsWalkable(!node.IsWalkable);
	}
}