using System;

using System.Collections.Generic;

using System.ComponentModel;

using System.Data;

using System.Drawing;

using System.Linq;

using System.Runtime.InteropServices;

using System.Text;

using System.Windows.Forms;

namespace ScreenToDecklink
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        ///    Performs a bit-block transfer of the color data corresponding to a
        ///    rectangle of pixels from the specified source device context into
        ///    a destination device context.
        /// </summary>
        /// <param name="hdc">Handle to the destination device context.</param>
        /// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
        /// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
        /// <param name="hdcSrc">Handle to the source device context.</param>
        /// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
        /// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
        /// <param name="dwRop">A raster-operation code.</param>
        /// <returns>
        ///    <c>true</c> if the operation succeedes, <c>false</c> otherwise. To get extended error information, call <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/>.
        /// </returns>
        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        public Form1()
        {
            InitializeComponent();
            captureVideo();


        }



        private void captureVideo()
        {
            Rectangle bounds = Screen.FromControl(this).Bounds;

            int nScreenWidth = bounds.Width;

            int nScreenHeight = bounds.Height;

            System.Diagnostics.Debug.WriteLine(nScreenWidth + "x" + nScreenHeight);

            IntPtr desktopHandle = GetDesktopWindow();



            IntPtr desktopDc = GetWindowDC(desktopHandle);



            System.Diagnostics.Debug.WriteLine(desktopDc);



            // Image myImage = new Bitmap(nScreenWidth, nScreenHeight);



            Graphics g = Graphics.FromImage(pictureBox1.Image);



            System.Diagnostics.Debug.WriteLine(g);



            IntPtr destDeviceContext = g.GetHdc();



            IntPtr srcDeviceContext = GetWindowDC(desktopHandle);



            BitBlt(destDeviceContext, 0, 0, nScreenWidth, nScreenHeight, srcDeviceContext, 0, 0, TernaryRasterOperations.SRCCOPY);



            g.ReleaseHdc(destDeviceContext);



            g.Dispose();

        }

    }



    /**
     * Specifies a raster-operation code. These codes define how the color data for the 
     * source rectangle is to be combined with the color data for the destination 
     * rectangle to achieve the final color.
     */
    enum TernaryRasterOperations : uint
    {
        SRCCOPY = 0x00CC0020,
        SRCPAINT = 0x00EE0086,
        SRCAND = 0x008800C6,
        SRCINVERT = 0x00660046,
        SRCERASE = 0x00440328,
        NOTSRCCOPY = 0x00330008,
        NOTSRCERASE = 0x001100A6,
        MERGECOPY = 0x00C000CA,
        MERGEPAINT = 0x00BB0226,
        PATCOPY = 0x00F00021,
        PATPAINT = 0x00FB0A09,
        PATINVERT = 0x005A0049,
        DSTINVERT = 0x00550009,
        BLACKNESS = 0x00000042,
        WHITENESS = 0x00FF0062,
        CAPTUREBLT = 0x40000000
    }
}
