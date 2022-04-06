namespace Prototype.Entities.Components;

using Stride.Core.Mathematics;
using Stride.Engine;

public class CharacterComponent : SyncScript
{
	private const float Movespeed = 5f;

	public readonly List<Vector3> Path = new();

	public override void Update()
	{
		while (this.Path.Count > 0)
		{
			var deltaTime = (float)this.Game.UpdateTime.Elapsed.TotalSeconds;

			var start = this.Entity.Transform.Position;
			var end = this.Path[0];

			var distance = (end - start).Length();
			var direction = Vector3.Normalize(end - start);

			if (distance == 0)
				this.Path.RemoveAt(0);
			else
			{
				this.Entity.Transform.Position += direction * Math.Min(CharacterComponent.Movespeed * deltaTime, distance);

				break;
			}
		}
	}
}
