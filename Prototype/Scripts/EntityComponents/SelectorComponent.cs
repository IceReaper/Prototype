namespace Prototype.Scripts.EntityComponents;

using Entities;
using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;

public sealed class SelectorComponent : SyncScript
{
	private Entity? entity;
	private ModelComponent? modelComponent;

	private Vector3 mouseStart;
	private Vector3 mouseEnd;

	public override void Start()
	{
		this.entity = this.Entity.Scene.Entities.FirstOrDefault(nameof(Cursor));
		this.modelComponent = this.Entity.Components.FirstOrDefault<ModelComponent>();
	}

	public override void Update()
	{
		if (this.modelComponent == null || this.entity == null)
			return;

		if (this.Input.IsMouseButtonDown(MouseButton.Left))
		{
			if (!this.modelComponent.Enabled && this.Input.MouseDelta.Length() != 0)
			{
				this.modelComponent.Enabled = true;
				this.mouseStart = this.entity.Transform.Position;
			}

			if (!this.modelComponent.Enabled)
				return;

			this.mouseEnd = this.entity.Transform.Position;

			var distance = this.mouseEnd - this.mouseStart;

			this.Entity.Transform.Position = this.mouseStart + distance / 2;
			this.Entity.Transform.Scale.X = Math.Max(Math.Abs(distance.X), .1f);
			this.Entity.Transform.Scale.Z = Math.Max(Math.Abs(distance.Z), .1f);
		}
		else if (this.modelComponent.Enabled)
		{
			this.modelComponent.Enabled = false;

			foreach (var character in this.Entity.Scene.Entities.OfType(nameof(Character))
				         .SelectMany(static entity => entity.Components.OfType<CharacterComponent>()))
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
