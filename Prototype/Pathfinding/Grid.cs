

//TGridObject is a generic - Used to store more than int's in a cell. Like a Node.
namespace Prototype.Pathfinding
{
	using System.Numerics;

	public class Grid<TGridObject>
	{
		public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;

		public class OnGridValueChangedEventArgs : EventArgs
		{
			public int X;
			public int Y;
		}

		private readonly int width;
		private readonly int height;
		private readonly float cellSize;
		private readonly Vector3 originPosition;
		private readonly TGridObject[,] gridArray;

		public Grid(
			int width,
			int height,
			float cellSize,
			Vector3 originPosition,
			Func<Grid<TGridObject>, int, int, TGridObject> createGridObject
		) //Func fügt ein default value für die celle ein - Grid<TGridObject>, int, int, TGridObject> Method signiture, for Updating the cells
		{
			this.width = width;
			this.height = height;
			this.cellSize = cellSize;
			this.originPosition = originPosition;

			this.gridArray = new TGridObject[width, height];

			for (var x = 0; x < this.gridArray.GetLength(0); x++)
			for (var y = 0; y < this.gridArray.GetLength(1); y++)
				this.gridArray[x, y] = createGridObject(this, x, y); //setting default object
		}

		private Vector3 GetWorldPosition(int x, int y)
		{
			return new Vector3(x, 0, y) * this.cellSize + this.originPosition;
		}

		private Vector3 GetWorldPositionCorrected(int x, int y)
		{
			return this.GetWorldPosition(x, y) + new Vector3(this.cellSize, 0, this.cellSize) * .5f;
		}

		//Check which Cell was hit with Worldposition
		public void GetSpecificCell(Vector3 worldPosition, out int x, out int y)
		{
			x = (int)Math.Floor((worldPosition - this.originPosition).X / this.cellSize);
			y = (int)Math.Floor((worldPosition - this.originPosition).Y / this.cellSize); //Is Z correct?
		}

		//Set Value of cell
		public void SetGridObject(Vector3 worldPosition, TGridObject value)
		{
			this.GetSpecificCell(worldPosition, out var x, out var y);

			//x and y is passed throught GETXY into SetValue
			this.SetGridObject(x, y, value);

			//Debug.Log("Mousecoords:" +worldPosition);
		}

		public void SetGridObject(int x, int y, TGridObject value)
		{
			if (x < 0 || y < 0 || x >= this.width || y >= this.height)
				return;

			this.gridArray[x, y] = value;

			if (this.OnGridValueChanged != null)
				this.OnGridValueChanged(this, new() { X = x, Y = y });
		}

		//Get Value of Cell 
		public TGridObject GetGridObject(Vector3 worldPosition)
		{
			this.GetSpecificCell(worldPosition, out var x, out var y);

			return this.GetGridObject(x, y);
		}

		public TGridObject GetGridObject(int x, int y)
		{
			if (x >= 0 && y >= 0 && x < this.width && y < this.height)
				return this.gridArray[x, y];

			return default; //Return the default value of TGridObject
		}

		public int GetWidth()
		{
			return this.width;
		}

		public int GetHeight()
		{
			return this.height;
		}

		public float GetCellSize()
		{
			return this.cellSize;
		}

		public void TriggerGridObjectChanged(int x, int y)
		{
			if (this.OnGridValueChanged != null)
				this.OnGridValueChanged(this, new() { X = x, Y = y });
		}
	}
}