using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BlackHole;

public abstract class Camera
    {
        public Vector3 Front { get; private set; } = Vector3.UnitZ;
        public Vector3 Up { get; private set; } = Vector3.UnitY;
        public Vector3 Right { get; private set; } = Vector3.UnitX;
        public float Aspect { get; set; } = 16f / 9f;
        public Vector3 Position { get; private set; } = new Vector3(0, 6, 0.5f);
        public float Sensitivity { get; set; } = 0.0015f;
        public float Speed { get; set; } = 5.0f;
        
        private float _pitch = MathHelper.DegreesToRadians(-10);
        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                _pitch = MathHelper.Clamp(_pitch, -MathHelper.PiOver2 * 0.99f, MathHelper.PiOver2 * 0.99f);
                UpdateVectors();
            }
        }
        
        private float _yaw = MathHelper.PiOver2;
        public float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                UpdateVectors();
            }
        }

        public abstract void HandleInput(KeyboardState keyboard, MouseState mouse, float dt);
        public abstract Matrix4 GetViewMatrix();
        public abstract Matrix4 GetProjectionMatrix();
        
        public void Rotate(float dpitch, float dyaw)
        {
            _pitch += dpitch;
            _pitch = MathHelper.Clamp(_pitch, -MathHelper.PiOver2 * 0.99f, MathHelper.PiOver2 * 0.99f);
            _yaw += dyaw;
            UpdateVectors();
        }

        public void Move(float dx, float dy, float dz)
        {
            Position += Front * dz + Up * dy + Right * dx;
        }
        
        public void UpdateVectors()
        {
            Front = new Vector3
            {
                X = (float) Math.Cos(Pitch) * (float) Math.Cos(Yaw),
                Y = (float) Math.Sin(Pitch),
                Z = (float) Math.Cos(Pitch) * (float) Math.Sin(Yaw)
            };

            Front = Vector3.Normalize(Front);
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        public Matrix4 GetProjectionViewMatrix()
        {
            return GetViewMatrix() * GetProjectionMatrix();
        }
    }