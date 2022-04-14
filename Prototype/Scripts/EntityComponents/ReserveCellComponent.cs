namespace Prototype.Scripts.EntityComponents;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Systems.Entities;

public class ReserveCellComponent : SyncScript
{
	private GridComponent? gridComponent;
	private Vector3 reservePosition;
	private (int X, int Y) reserveCell = (-1, -1);

	public override void Start()
	{
		this.gridComponent = this.Entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();
	}

	public override void Update()
	{
		if (this.gridComponent == null)
			return;

		var reserveCell = this.gridComponent.GetCell(this.Entity.Transform.Position);

		if (this.reserveCell.X == reserveCell.X && this.reserveCell.Y == reserveCell.Y)
			return;

		this.gridComponent.UnreserveCell(this.reservePosition, this.Entity);
		this.reservePosition = this.Entity.Transform.Position;
		this.gridComponent.ReserveCell(this.reservePosition, this.Entity);
		this.reserveCell = reserveCell;
	}
}
