namespace Prototype.Scripts.EntityComponents;

using Entities;
using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Systems.Navigation;

public sealed class OccupyCellComponent : SyncScript
{
	private GridComponent? gridComponent;
	private Cell? occupyCell;

	public override void Start()
	{
		this.gridComponent = this.Entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();
	}

	public override void Update()
	{
		if (this.gridComponent == null)
			return;

		var cell = this.gridComponent.GetCellContaining(OccupyCellComponent.GetGridPosition(this.Entity.Transform.Position));

		if (this.occupyCell == cell)
			return;

		this.occupyCell?.Occupiers.Remove(this.Entity);
		this.occupyCell = cell;

		if (this.occupyCell?.Occupiers.Contains(this.Entity) == false)
			this.occupyCell.Occupiers.Add(this.Entity);
	}

	public static Vector3 GetGridPosition(Vector3 position)
	{
		return new(position.X, (float)Math.Ceiling(position.Y), position.Z);
	}
}
