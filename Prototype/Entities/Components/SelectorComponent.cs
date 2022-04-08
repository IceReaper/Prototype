namespace Prototype.Entities.Components;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Input;
using Stride.Rendering;

public class SelectorComponent : SyncScript
{
	private Entity debugcube;
	private Entity? cameraEntity;
	//private CameraControllerComponent? cameraComponent;

	private Entity? cursor;

	private Vector3 mouseStart;
	private Vector3 mouseEnd;
	private Vector3 mouseVector;

	public override void Start()

	{
		this.CreateCube();
		this.cameraEntity = this.SceneSystem.GetAll(nameof(Camera)).FirstOrDefault();
		//this.cameraComponent = this.cameraEntity.GetAll<CameraControllerComponent>().FirstOrDefault();
		this.cursor = this.SceneSystem.GetAll(nameof(Cursor)).FirstOrDefault();
	}

	private void CreateCube()
	{
		var selectorCube = new Model();

		this.debugcube = new()
		{
			new ModelComponent(selectorCube = new() { new Mesh { Draw = GeometricPrimitive.Cube.New(this.GraphicsDevice, 1f, 8).ToMeshDraw() } })
		};

		this.Entity.AddChild(this.debugcube);

		this.Entity.Transform.Position = new(0, 0, 0);
	}

	public override void Update()
	{
		// get Rotation From Camera
		var cameraRotation = new Vector3(this.cameraEntity.Transform.RotationEulerXYZ.X, this.cameraEntity.Transform.RotationEulerXYZ.Y, this.cameraEntity.Transform.RotationEulerXYZ.Z);
		this.Entity.Transform.RotationEulerXYZ = new(0,cameraRotation.Y,0);
		
		this.Entity.Transform.Position = this.cursor.Transform.Position;

		if (this.Input.IsMouseButtonPressed(MouseButton.Left))
			this.mouseStart = this.cursor.Transform.Position;

		if (this.Input.IsMouseButtonDown(MouseButton.Left))
		{
			this.mouseEnd = this.cursor.Transform.Position;

			this.mouseVector = this.mouseEnd - this.mouseStart;

			this.Entity.Transform.Scale.X = this.mouseVector.X;
			this.Entity.Transform.Scale.Z = this.mouseVector.Z;

			this.Entity.Transform.Position = this.mouseStart + this.mouseVector / 2;
		}

		if (this.Input.IsMouseButtonReleased(MouseButton.Left)) // scale the cube entitiy between the start and end vector
		{
			Console.WriteLine("Start is " + this.mouseStart + " , End is " + this.mouseEnd);
			Console.WriteLine("MouseVector is " + this.mouseVector);
			this.Entity.Transform.Scale = Vector3.One;
		}
	}

	private List<Entity> GetSelectedUnits(List<Entity> localunits)
	{
		var selectedUnits = new List<Entity>();
		foreach (var unit in localunits)
		{
			if (this.mouseStart.X > unit.Transform.Position.X && unit.Transform.Position.X < this.mouseEnd.X
			    && this.mouseStart.Y > unit.Transform.Position.Y && unit.Transform.Position.Y < this.mouseEnd.Y
			    && this.mouseStart.Z > unit.Transform.Position.Z && unit.Transform.Position.Z < this.mouseEnd.Z )
			{
				if (unit.Tags == localplayer)
				{
					if (selectedUnits != null)
						selectedUnits.Add(unit);
				}
			}
		}
		return selectedUnits;
	}
}
