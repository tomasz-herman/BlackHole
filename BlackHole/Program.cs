using BlackHole.ImGuiUtils;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace BlackHole
{
    public class Program : GameWindow
    {
        private Shader shader;
        private ImGuiController controller;
        private Mesh rectangle;
        private TextureCube texture;
        private Camera camera;
        private float mass = 1e8f;
        private Vector3 blackHolePosition = new (0, 0, 1e9f);

        public static void Main(string[] args)
        {
            using var program = new Program(GameWindowSettings.Default, NativeWindowSettings.Default);
            program.Title = "Black Hole";
            program.Size = new Vector2i(1280, 800);
            program.Run();
        }

         public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            shader = new Shader(("shader.vert", ShaderType.VertexShader), ("shader.frag", ShaderType.FragmentShader));
            controller = new ImGuiController(ClientSize.X, ClientSize.Y);

            float[] vertices = {
                1,  1, 0,
                1, -1, 0,
                -1, -1, 0,
                -1,  1, 0
            };
            int[] indices= {
                0, 1, 3,
                1, 2, 3
            };  
            rectangle = new Mesh(PrimitiveType.Triangles, indices, (vertices, 0, 3));
            
            (string Path, TextureTarget side)[] textures =
            {
                ("GalaxyTex_NegativeX.png", TextureTarget.TextureCubeMapNegativeX), 
                ("GalaxyTex_NegativeY.png", TextureTarget.TextureCubeMapNegativeY), 
                ("GalaxyTex_NegativeZ.png", TextureTarget.TextureCubeMapNegativeZ), 
                ("GalaxyTex_PositiveX.png", TextureTarget.TextureCubeMapPositiveX), 
                ("GalaxyTex_PositiveY.png", TextureTarget.TextureCubeMapPositiveY), 
                ("GalaxyTex_PositiveZ.png", TextureTarget.TextureCubeMapPositiveZ), 
            };
            texture = new TextureCube(textures);
            
            camera = new PerspectiveCamera();
            camera.UpdateVectors();
            
            GL.ClearColor(0.4f, 0.7f, 0.9f, 1.0f);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            
            rectangle.Dispose();
            controller.Dispose();
            texture.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            controller.Update(this, (float)args.Time);

            if(ImGui.GetIO().WantCaptureMouse) return;

            KeyboardState keyboard = KeyboardState.GetSnapshot();
            MouseState mouse = MouseState.GetSnapshot();
            
            camera.HandleInput(keyboard, mouse, (float)args.Time);
            
            if (keyboard.IsKeyDown(Keys.Escape)) Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            shader.Use();
            texture.Use();
            shader.LoadInteger("sampler", 0);
            shader.LoadFloat2("resolution", new Vector2(Size.X, Size.Y));
            shader.LoadMatrix4("invView", camera.GetProjectionViewMatrix().Inverted());
            shader.LoadFloat("mass", mass);
            shader.LoadFloat3("blackHolePosition", blackHolePosition);
            rectangle.Render();

            RenderGui();

            Context.SwapBuffers();
        }

        private void RenderGui()
        {
            ImGui.Begin("Options");
            ImGui.SliderFloat("mass", ref mass, 1e7f, 1e8f);
            System.Numerics.Vector3 position =
                new System.Numerics.Vector3(blackHolePosition.X, blackHolePosition.Y, blackHolePosition.Z);
            if (ImGui.SliderFloat3("position", ref position, -1e9f, 1e9f))
            {
                blackHolePosition = new Vector3(position.X, position.Y, position.Z);
            }
            ImGui.End();
            
            controller.Render();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            
            controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            controller.MouseScroll(e.Offset);
        }
    }
}