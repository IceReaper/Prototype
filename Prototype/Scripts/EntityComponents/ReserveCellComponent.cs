namespace Prototype.Scripts.EntityComponents;

using Entities;
using Extensions;
using Stride.Engine;
using Systems.Navigation;

public sealed class ReserveCellComponent : SyncScript
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

		var cell = this.gridComponent.GetCellContaining(this.Entity.Transform.Position);

		if (this.occupyCell != cell)
			return;

		this.occupyCell?.Occupiers.Remove(this.Entity);
		this.occupyCell = cell;

		if (this.occupyCell?.Occupiers.Contains(this.Entity) == false)
			this.occupyCell.Occupiers.Add(this.Entity);
	}
}
