namespace Prototype.Scripts.EntityComponents;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Systems.Entities;

public class BlockCellComponent : SyncScript
{
	private GridComponent? gridComponent;
	private Vector3 blockPosition;
	private (int X, int Y) blockCell = (-1, -1);

	public override void Start()
	{
		this.gridComponent = this.Entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();
	}

	public override void Update()
	{
		if (this.gridComponent == null)
			return;

		var blockCell = this.gridComponent.GetCell(this.Entity.Transform.Position);

		if (this.blockCell.X == blockCell.X && this.blockCell.Y == blockCell.Y)
			return;

		this.gridComponent.UnblockCell(this.blockPosition, this.Entity);
		this.blockPosition = this.Entity.Transform.Position;
		this.gridComponent.BlockCell(this.blockPosition, this.Entity);
		this.blockCell = blockCell;
	}
}
