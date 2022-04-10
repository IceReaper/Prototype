namespace Prototype.Scripts.EntityComponents;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Systems.Entities;

public class SelectorComponent : SyncScript
{
	private Vector3 mouseStart;
	private Vector3 mouseEnd;

	public override void Update()
	{
		var entity = this.Entity.Scene.Entities.FirstOrDefault(nameof(Cursor));
		var modelComponent = this.Entity.Components.FirstOrDefault<ModelComponent>();

		if (modelComponent == null || entity == null)
			return;

		if (this.Input.IsMouseButtonDown(MouseButton.Left))
		{
			if (!modelComponent.Enabled && this.Input.MouseDelta.Length() != 0)
			{
				modelComponent.Enabled = true;
				this.mouseStart = entity.Transform.Position;
			}

			if (!modelComponent.Enabled)
				return;

			this.mouseEnd = entity.Transform.Position;

			var distance = this.mouseEnd - this.mouseStart;

			this.Entity.Transform.Position = this.mouseStart + distance / 2;
			this.Entity.Transform.Scale.X = Math.Max(Math.Abs(distance.X), .1f);
			this.Entity.Transform.Scale.Z = Math.Max(Math.Abs(distance.Z), .1f);
		}
		else if (modelComponent.Enabled)
		{
			modelComponent.Enabled = false;

			foreach (var character in this.Entity.Scene.Entities.OfType(nameof(Character)).SelectMany(entity => entity.Components.OfType<CharacterComponent>()))
			{
				character.IsSelected = this.mouseStart.X < character.Entity.Transform.Position.X
					&& character.Entity.Transform.Position.X < this.mouseEnd.X
					////&& this.mouseStart.Y < character.Entity.Transform.Position.Y
					////&& character.Entity.Transform.Position.Y < this.mouseEnd.Y
					&& this.mouseStart.Z < character.Entity.Transform.Position.Z
					&& character.Entity.Transform.Position.Z < this.mouseEnd.Z;
			}
		}
	}
}
