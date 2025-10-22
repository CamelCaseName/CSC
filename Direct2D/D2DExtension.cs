using Silk.NET.Core.Native;
using Silk.NET.Direct2D;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using Rectangle = System.Drawing.Rectangle;

namespace CSC.Direct2D
{
    public unsafe static class D2DExtension
    {
        public static D3Dcolorvalue ToD3D(this Color color)
        {
            D3Dcolorvalue val = new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            return val;
        }

        public static Box2D<T> ToBox<T>(this Rectangle rectangle) where T : unmanaged, IEquatable<T>, IComparable<T>, IFormattable
        {
            return new((T)(object)rectangle.Left, (T)(object)rectangle.Top, (T)(object)rectangle.Right, (T)(object)rectangle.Bottom);
        }

        public static Box2D<T> ToBox<T>(this RectangleF rectangle) where T : unmanaged, IEquatable<T>, IComparable<T>, IFormattable
        {
            return new((T)(object)(int)rectangle.Left, (T)(object)(int)rectangle.Top, (T)(object)(int)rectangle.Right, (T)(object)(int)rectangle.Bottom);
        }

        public static ID2D1Brush* AsBrush(this ComPtr<ID2D1SolidColorBrush> br)
        {
            return (ID2D1Brush*)br.Handle;
        }

        public static ID2D1Brush* AsBrush(this ComPtr<ID2D1LinearGradientBrush> br)
        {
            return (ID2D1Brush*)br.Handle;
        }
    }
}
