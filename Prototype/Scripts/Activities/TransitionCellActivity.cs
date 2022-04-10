namespace Prototype.Scripts.Activities;

using EntityComponents;
using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Systems.Activities;
using Systems.Entities;

public class TransitionCellActivity : Activity
{
	private readonly Entity entity;
	private readonly Vector3 target;
	private Vector3? origin;

	public TransitionCellActivity(Entity entity, Vector3 target)
	{
		this.entity = entity;
		this.target = target;
	}

	protected override void UpdateInner(GameTime updateTime)
	{
		var gridComponent = this.entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();

		if (gridComponent == null)
		{
			this.Complete();

			return;
		}

		if (this.origin == null)
		{
			this.origin = this.entity.Transform.Position;
			gridComponent.ReserveCell(this.target, this.entity);
		}

		var deltaTime = (float)gridComponent.Game.UpdateTime.Elapsed.TotalSeconds;
		var distance = (this.target - this.entity.Transform.Position).Length();
		var direction = Vector3.Normalize(this.target - this.entity.Transform.Position);

		if (distance > 0)
			this.entity.Transform.Position += direction * Math.Min(CharacterComponent.Movespeed * deltaTime, distance);
		else
		{
			gridComponent.UnreserveCell(this.target, this.entity);
			this.Complete();
		}
	}
}
