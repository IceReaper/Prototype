namespace Prototype.Scripts.EntityComponents;

using Extensions;
using Stride.Engine;
using Systems.Entities;
using Systems.Navigation;

public class ReserveCellComponent : SyncScript
{
	private GridComponent? gridComponent;
	private Cell? reserveCell;

	public override void Start()
	{
		this.gridComponent = this.Entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();
	}

	public override void Update()
	{
		if (this.gridComponent == null)
			return;

		var reserveCell = this.gridComponent.GetCellContaining(this.Entity.Transform.Position);

		if (this.reserveCell != reserveCell)
			return;

		this.reserveCell?.Reservers.Remove(this.Entity);
		this.reserveCell = reserveCell;

		if (this.reserveCell != null && !this.reserveCell.Reservers.Contains(this.Entity))
			this.reserveCell.Reservers.Add(this.Entity);
	}
}
