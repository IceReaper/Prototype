namespace Prototype.Entities.Components;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;

public class SelectorComponent : SyncScript
{
	private Entity? cursor;
	private Vector3 mouseStart;
	private Vector3 mouseEnd;

	private ModelComponent? model;

	public override void Start()
	{
		this.cursor = this.SceneSystem.GetAll(nameof(Cursor)).FirstOrDefault();
		this.model = this.Entity.GetAll<ModelComponent>().FirstOrDefault();
	}

	public override void Update()
	{
		if (this.model == null || this.cursor == null)
			return;

		if (this.Input.IsMouseButtonDown(MouseButton.Left))
		{
			if (!this.model.Enabled && this.Input.MouseDelta.Length() != 0)
			{
				this.model.Enabled = true;
				this.mouseStart = this.cursor.Transform.Position;
			}

			if (!this.model.Enabled)
				return;

			this.mouseEnd = this.cursor.Transform.Position;

			var distance = this.mouseEnd - this.mouseStart;

			this.Entity.Transform.Position = this.mouseStart + distance / 2;
			this.Entity.Transform.Scale.X = Math.Max(Math.Abs(distance.X), .1f);
			this.Entity.Transform.Scale.Z = Math.Max(Math.Abs(distance.Z), .1f);
		}
		else if (this.model.Enabled)
		{
			this.model.Enabled = false;

			foreach (var character in this.SceneSystem.GetAll(nameof(Character)).SelectMany(entity => entity.GetAll<CharacterComponent>()))
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
