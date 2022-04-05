namespace Prototype.Pathfinding
{
	using System.Linq;
	using System.Numerics;

	public class Pathfinding
	{
		//Cost for movement on the grid:
		private const int MoveStraightCost = 10;

		private const int MoveDiagonalCost = 14;

		//add costs for different speed terrain
		//private const int MOVE_STRAIGT_COST_ROAD - cheaper to go on road because the unit walks on roads faster. Togglable for RTS.

		private readonly Grid<PathNode> grid;

		private List<PathNode> openList;
		private List<PathNode> closedList;

		public Pathfinding(int width, int height)
		{
			this.grid = new(width, height, 10f, Vector3.Zero, (g, x, y) => new(g, x, y));
		}

		public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
		{
			//define start node and end node on the grid: Interface for the method call:
			var startNode = this.grid.GetGridObject(startX, startY);
			var endNode = this.grid.GetGridObject(endX, endY);

			this.openList = new() { startNode };
			this.closedList = new();

			//set initial cost of the nodes:
			for (var x = 0; x < this.grid.GetWidth(); x++)
			{
				for (var y = 0; y < this.grid.GetHeight(); y++)
				{
					var pathNode = this.grid.GetGridObject(x, y);
					pathNode.GCost = int.MaxValue;
					pathNode.CalculateFCost();
					pathNode.CameFromNode = null;
				}
			}

			//start node setup:
			startNode.GCost = 0;
			startNode.HCost = this.CalculateDistanceCost(startNode, endNode);
			startNode.CalculateFCost();

			//openList are all not yet checked nodes. If all nodes were checked and no path was returned, than there is no path.
			while (this.openList.Count > 0)
			{
				var currentNode = this.GetLowestFCostNode(this.openList);

				if (currentNode == endNode)
				{
					//Reached final node
					return this.CalculatePath(endNode);
				}

				//checked nodes are moved into the closedList
				this.openList.Remove(currentNode);
				this.closedList.Add(currentNode);

				//check the neighbours of the current node (list is created and returned):
				foreach (var neighbourNode in this.GetNeighbourList(currentNode).Where(neighbourNode => !this.closedList.Contains(neighbourNode)))
				{
					if (!neighbourNode.IsWalkable)
					{
						this.closedList.Add(neighbourNode);

						continue;
					}

					var tentativeGCost = currentNode.GCost + this.CalculateDistanceCost(currentNode, neighbourNode);

					if (tentativeGCost >= neighbourNode.GCost)
						continue;

					neighbourNode.CameFromNode = currentNode;
					neighbourNode.GCost = tentativeGCost;
					neighbourNode.HCost = this.CalculateDistanceCost(neighbourNode, endNode);
					neighbourNode.CalculateFCost();

					if (!this.openList.Contains(neighbourNode))
						this.openList.Add(neighbourNode);
				}
			}

			//Out of nodes on the openList
			return null;
		}

		/* 
		public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
		{
			this.grid.GetSpecificCell(startWorldPosition, out var startX, out var startY);
			this.grid.GetSpecificCell(endWorldPosition, out var endX, out var endY);

			var path = this.FindPath(startX, startY, endX, endY);

			if (path == null)
				return null;
			else
			{
				var vectorPath = new List<Vector3>();

				foreach (var pathNode in path)
					vectorPath.Add(new Vector3(pathNode.X, 0, pathNode.Y) * this.grid.GetCellSize() + new Vector3(1, 0, 1) * this.grid.GetCellSize() * .5f);

				return vectorPath;
			}
		}

		*/

		//Precalculate alle neighbours when the grid is created. 
		private IEnumerable<PathNode> GetNeighbourList(PathNode currentNode)
		{
			if (currentNode.X - 1 >= 0)
			{
				//Left
				yield return this.GetNode(currentNode.X - 1, currentNode.Y);

				//Left Down
				if (currentNode.Y - 1 >= 0)
					yield return this.GetNode(currentNode.X - 1, currentNode.Y - 1);

				//Left Up
				if (currentNode.Y + 1 < this.grid.GetHeight())
					yield return this.GetNode(currentNode.X - 1, currentNode.Y + 1);
			}

			if (currentNode.X + 1 < this.grid.GetWidth())
			{
				//Right
				yield return this.GetNode(currentNode.X + 1, currentNode.Y);

				//Right Down
				if (currentNode.Y - 1 >= 0)
					yield return this.GetNode(currentNode.X + 1, currentNode.Y - 1);

				//Right Up
				if (currentNode.Y + 1 < this.grid.GetHeight())
					yield return this.GetNode(currentNode.X + 1, currentNode.Y + 1);
			}

			//Down
			if (currentNode.Y - 1 >= 0)
				yield return this.GetNode(currentNode.X, currentNode.Y - 1);

			//Up
			if (currentNode.Y + 1 < this.grid.GetHeight())
				yield return this.GetNode(currentNode.X, currentNode.Y + 1);
		}

		public PathNode GetNode(int x, int y)
		{
			return this.grid.GetGridObject(x, y);
		}

		private List<PathNode> CalculatePath(PathNode endNode)
		{
			var path = new List<PathNode> { endNode };
			var currentNode = endNode;

			while (currentNode.CameFromNode != null)
			{
				path.Add(currentNode.CameFromNode);
				currentNode = currentNode.CameFromNode;
			}

			path.Reverse();

			return path;
		}

		private int CalculateDistanceCost(PathNode a, PathNode b) //Distance between start an end without blocked cells
		{
			var xDistance = Math.Abs(a.X - b.X);
			var yDistance = Math.Abs(a.Y - b.Y);
			var remaining = Math.Abs(xDistance - yDistance);

			return Pathfinding.MoveDiagonalCost * Math.Min(xDistance, yDistance) + Pathfinding.MoveStraightCost * remaining;
		}

		//Upgrade GetLowestFCostNode to binary tree - linear search takes to long.
		private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
		{
			var lowestFCostNode = pathNodeList[0];

			for (var i = 1; i < pathNodeList.Count; i++)
			{
				if (pathNodeList[i].FCost < lowestFCostNode.FCost)
					lowestFCostNode = pathNodeList[i];
			}

			return lowestFCostNode;
		}
	}
}