using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BlackHole;

public class PerspectiveCamera : Camera
{
    private float _fov = MathHelper.PiOver3;

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(value);
    }
    
    public float NearPlane { get; set; } = 1f;
    public float FarPlane { get; set; } = 100f;
    
    public override void HandleInput(KeyboardState keyboard, MouseState mouse, float dt)
    {
        if (mouse.IsButtonDown(MouseButton.Button1) && mouse.WasButtonDown(MouseButton.Button1))
        {
            Vector2 delta = mouse.Delta;
            Rotate(-Sensitivity * delta.Y, Sensitivity * delta.X);
        }

        if (keyboard.IsKeyDown(Keys.W))
        {
            Move(0, 0, dt * Speed);
        }
        if (keyboard.IsKeyDown(Keys.S))
        {
            Move(0, 0, -dt * Speed);
        }
        if (keyboard.IsKeyDown(Keys.D))
        {
            Move(dt * Speed, 0, 0);
        }
        if (keyboard.IsKeyDown(Keys.A))
        {
            Move(-dt * Speed, 0, 0);
        }
        if (keyboard.IsKeyDown(Keys.E))
        {
            Move(0, dt * Speed, 0);
        }
        if (keyboard.IsKeyDown(Keys.Q))
        {
            Move(0, -dt * Speed, 0);
        }
    }

    public override Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Front, Up);
    }

    public override Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, Aspect, NearPlane, FarPlane);
    }
}
