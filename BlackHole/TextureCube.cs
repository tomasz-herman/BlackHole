using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace BlackHole
{
    public class TextureCube : IDisposable
    {
        private int _handle;

        public TextureCube((string path, TextureTarget side)[] textures, Options options = default)
        {
            _handle = GL.GenTexture();
            Use();
            LoadTexture(textures, options);
            
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
        }

        private void LoadTexture((string path, TextureTarget side)[] textures, Options options = default)
        {
            foreach (var (path, side) in textures)
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream($"BlackHole.Resources.{path}");
    
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
    
                GL.TexImage2D(side, 
                    0, 
                    PixelInternalFormat.Rgba, 
                    image.Width, image.Height, 
                    0, 
                    PixelFormat.Rgba, 
                    PixelType.UnsignedByte, 
                    image.Data);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter,
                (int) options.MinFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter,
                (int) options.MagFilter);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int) options.WrapS);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int) options.WrapT);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int) options.WrapR);
            
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, _handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_handle);
            GC.SuppressFinalize(this);
        }

        public struct Options
        {
            public TextureMinFilter MinFilter = TextureMinFilter.LinearMipmapLinear;
            public TextureMagFilter MagFilter = TextureMagFilter.Linear;
            public TextureWrapMode WrapS = TextureWrapMode.Repeat;
            public TextureWrapMode WrapT = TextureWrapMode.Repeat;
            public TextureWrapMode WrapR = TextureWrapMode.Repeat;

            public Options() { }
        }
    }
}