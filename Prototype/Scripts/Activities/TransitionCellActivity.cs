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

	public TransitionCellActivity(Entity entity, Vector3 target)
	{
		this.entity = entity;
		this.target = target;
	}

	protected override void Start()
	{
		this.entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>()?.ReserveCell(this.target, this.entity);
	}

	protected override void UpdateInner(GameTime updateTime)
	{
		var gridComponent = this.entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();

		if (gridComponent == null)
		{
			this.Complete();

			return;
		}

		var deltaTime = (float)updateTime.Elapsed.TotalSeconds;
		var distance = (this.target - this.entity.Transform.Position).Length();
		var direction = Vector3.Normalize(this.target - this.entity.Transform.Position);

		if (distance > 0)
			this.entity.Transform.Position += direction * Math.Min(CharacterComponent.Movespeed * deltaTime, distance);
		else
			this.Complete();
	}
}
