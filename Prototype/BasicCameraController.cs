namespace Prototype;

using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;

public class BasicCameraController : SyncScript
{
	private const float MaximumPitch = MathUtil.PiOverTwo * 0.99f;

	private Vector3 upVector;
	private Vector3 translation;
	private Vector3 translationLinear;
	private float yaw;
	private float pitch;

	public bool Gamepad { get; set; }
	public Vector3 KeyboardMovementSpeed { get; set; } = new(5.0f);
	public Vector3 TouchMovementSpeed { get; set; } = new(0.7f, 0.7f, 0.3f);
	public float SpeedFactor { get; set; } = 5.0f;
	public Vector2 KeyboardRotationSpeed { get; set; } = new(3.0f);
	public Vector2 MouseRotationSpeed { get; set; } = new(1.0f, 1.0f);
	public Vector2 TouchRotationSpeed { get; set; } = new(1.0f, 0.7f);

	public override void Start()
	{
		base.Start();

		this.upVector = Vector3.UnitY;

		if (Platform.IsWindowsDesktop)
			return;

		this.Input.Gestures.Add(new GestureConfigDrag());
		this.Input.Gestures.Add(new GestureConfigComposite());
	}

	public override void Update()
	{
		this.ProcessInput();
		this.UpdateTransform();

		if (this.Input.IsKeyDown(Keys.F1))
			this.Entity.Transform.Position = new(123, 7, 218);

		if (this.Input.IsKeyDown(Keys.F2))
			this.Entity.Transform.Position = new(5, 8, 5);
	}

	private void ProcessInput()
	{
		var deltaTime = (float)this.Game.UpdateTime.Elapsed.TotalSeconds;
		this.translation = Vector3.Zero;
		this.translationLinear = Vector3.Zero;
		this.yaw = 0f;
		this.pitch = 0f;

		{
			var speed = 1f * deltaTime;
			var dir = Vector3.Zero;
			var dirLinear = Vector3.Zero;

			if (this.Gamepad && this.Input.HasGamePad)
			{
				var padState = this.Input.DefaultGamePad.State;

				dir.Z += padState.LeftThumb.Y;
				dir.X += padState.LeftThumb.X;

				dirLinear.Y -= padState.LeftTrigger;
				dirLinear.Y += padState.RightTrigger;

				if ((padState.Buttons & (GamePadButton.A | GamePadButton.LeftShoulder | GamePadButton.RightShoulder)) != 0)
					speed *= this.SpeedFactor;
			}

			if (this.Input.HasKeyboard)
			{
				if (this.Input.IsKeyDown(Keys.W) || this.Input.IsKeyDown(Keys.Up))
					dir.Z += 1;

				if (this.Input.IsKeyDown(Keys.S) || this.Input.IsKeyDown(Keys.Down))
					dir.Z -= 1;

				if (this.Input.IsKeyDown(Keys.A) || this.Input.IsKeyDown(Keys.Left))
					dir.X -= 1;

				if (this.Input.IsKeyDown(Keys.D) || this.Input.IsKeyDown(Keys.Right))
					dir.X += 1;

				if (this.Input.IsKeyDown(Keys.Q))
					dirLinear.Y -= 1;

				if (this.Input.IsKeyDown(Keys.E))
					dirLinear.Y += 1;

				if (this.Input.IsKeyDown(Keys.LeftShift) || this.Input.IsKeyDown(Keys.RightShift))
					speed *= this.SpeedFactor;

				if (dir.Length() > 1f)
					dir = Vector3.Normalize(dir);

				if (dirLinear.Length() > 1f)
					dirLinear = Vector3.Normalize(dirLinear);
			}

			this.translation += dir * this.KeyboardMovementSpeed * speed;
			this.translationLinear += dirLinear * this.KeyboardMovementSpeed * speed;
		}

		{
			var speed = 1f * deltaTime;
			var rotation = Vector2.Zero;

			if (this.Gamepad && this.Input.HasGamePad)
			{
				var padState = this.Input.DefaultGamePad.State;
				rotation.X += padState.RightThumb.Y;
				rotation.Y += -padState.RightThumb.X;
			}

			if (this.Input.HasKeyboard)
			{
				if (this.Input.IsKeyDown(Keys.NumPad2))
					rotation.X += 1;

				if (this.Input.IsKeyDown(Keys.NumPad8))
					rotation.X -= 1;

				if (this.Input.IsKeyDown(Keys.NumPad4))
					rotation.Y += 1;

				if (this.Input.IsKeyDown(Keys.NumPad6))
					rotation.Y -= 1;

				if (rotation.Length() > 1f)
					rotation = Vector2.Normalize(rotation);
			}

			rotation *= this.KeyboardRotationSpeed * speed;

			this.pitch += rotation.X;
			this.yaw += rotation.Y;
		}

		{
			if (this.Input.HasMouse)
			{
				if (this.Input.IsMouseButtonDown(MouseButton.Right))
				{
					this.Input.LockMousePosition();
					this.Game.IsMouseVisible = false;

					this.yaw -= this.Input.MouseDelta.X * this.MouseRotationSpeed.X;
					this.pitch -= this.Input.MouseDelta.Y * this.MouseRotationSpeed.Y;
				}
				else
				{
					this.Input.UnlockMousePosition();
					this.Game.IsMouseVisible = true;
				}
			}

			foreach (var gestureEvent in this.Input.GestureEvents)
			{
				switch (gestureEvent.Type)
				{
					case GestureType.Drag:
						var drag = (GestureEventDrag)gestureEvent;
						var dragDistance = drag.DeltaTranslation;
						this.yaw = -dragDistance.X * this.TouchRotationSpeed.X;
						this.pitch = -dragDistance.Y * this.TouchRotationSpeed.Y;

						break;

					case GestureType.Composite:
						var composite = (GestureEventComposite)gestureEvent;
						this.translation.X = -composite.DeltaTranslation.X * this.TouchMovementSpeed.X;
						this.translation.Y = -composite.DeltaTranslation.Y * this.TouchMovementSpeed.Y;
						this.translation.Z = (float)Math.Log(composite.DeltaScale + 1) * this.TouchMovementSpeed.Z;

						break;
				}
			}
		}
	}

	private void UpdateTransform()
	{
		var rotation = Matrix.RotationQuaternion(this.Entity.Transform.Rotation);
		var right = Vector3.Cross(rotation.Forward, this.upVector);
		var up = Vector3.Cross(right, rotation.Forward);

		right.Normalize();
		up.Normalize();

		var currentPitch = MathUtil.PiOverTwo - (float)Math.Acos(Vector3.Dot(rotation.Forward, this.upVector));
		this.pitch = MathUtil.Clamp(currentPitch + this.pitch, -BasicCameraController.MaximumPitch, BasicCameraController.MaximumPitch) - currentPitch;

		var finalTranslation = this.translation;
		finalTranslation.Z = -finalTranslation.Z;
		finalTranslation = Vector3.TransformCoordinate(finalTranslation, rotation) + this.translationLinear;

		this.Entity.Transform.Position += finalTranslation;
		this.Entity.Transform.Rotation *= Quaternion.RotationAxis(right, this.pitch) * Quaternion.RotationAxis(this.upVector, this.yaw);
	}
}
