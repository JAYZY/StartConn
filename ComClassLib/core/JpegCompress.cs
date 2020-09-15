using FVIL;
using FVIL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComClassLib.core {
    public class JpegCompress {
        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tjInitDecompress();

        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tjInitCompress();


        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int tjDecompressHeader3(IntPtr handle, IntPtr jpegBuf,
                                  uint jpegSize, out int width,
                                  out int height, out int jpegSubsamp,
                                   out int jpegColorspace);

        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int tjDecompress2(IntPtr handle, IntPtr jpegBuf,
                         uint jpegSize, IntPtr dstBuf,
                         int width, int pitch, int height, int pixelFormat,
                         int flags);




        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int tjDestroy(IntPtr handle);

        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint tjBufSize(int width, int height, int jpegSubsamp);

        public enum TJColorSpaces {
            TJCS_RGB = 0,
            TJCS_YCbCr,
            TJCS_GRAY,
            TJCS_CMYK,
            TJCS_YCCK
        };


        public enum TJSubsamplingOptions {
            TJSAMP_444 = 0,
            TJSAMP_422,
            TJSAMP_420,
            TJSAMP_GRAY,
            TJSAMP_440,
            TJSAMP_411
        };

        public enum TJPixelFormats {
            TJPF_RGB = 0,
            TJPF_BGR,
            TJPF_RGBX,
            TJPF_BGRX,
            TJPF_XBGR,
            TJPF_XRGB,
            TJPF_GRAY,
            TJPF_RGBA,
            TJPF_BGRA,
            TJPF_ABGR,
            TJPF_ARGB,
            TJPF_CMYK
        };


        [Flags]
        public enum TJFlags {
            NONE = 0,
            BOTTOMUP = 2,
            FASTUPSAMPLE = 256,
            NOREALLOC = 1024,
            FASTDCT = 2048,
            ACCURATEDCT = 4096
        }

        public enum TJTransformOperations {
            TJXOP_NONE = 0,
            TJXOP_HFLIP,
            TJXOP_VFLIP,
            TJXOP_TRANSPOSE,
            TJXOP_TRANSVERSE,
            TJXOP_ROT90,
            TJXOP_ROT180,
            TJXOP_ROT270
        };

        [Flags]
        public enum TJTransformOptions {
            PERFECT = 1,
            TRIM = 2,
            CROP = 4,
            GRAY = 8,
            NOOUTPUT = 16,
        }

        public static readonly Dictionary<TJPixelFormats, int> PixelSizes = new Dictionary<TJPixelFormats, int>
        {
            { TJPixelFormats.TJPF_RGB, 3},
            { TJPixelFormats.TJPF_BGR, 3},
            { TJPixelFormats.TJPF_RGBX, 4},
            { TJPixelFormats.TJPF_BGRX, 4},
            { TJPixelFormats.TJPF_XBGR, 4},
            { TJPixelFormats.TJPF_XRGB, 4},
            { TJPixelFormats.TJPF_GRAY, 1},
            { TJPixelFormats.TJPF_RGBA, 4},
            { TJPixelFormats.TJPF_BGRA, 4},
            { TJPixelFormats.TJPF_ABGR, 4},
            { TJPixelFormats.TJPF_ARGB, 4},
            { TJPixelFormats.TJPF_CMYK, 4}
        };
        static readonly Object obj = new object();

        

        public static CFviImage Decompress(byte[] jpg_buffer, uint uJpgSize, int iOffset=-1) {

            int jpegWidth, jpegHeight, jpegSubsamp, jpegColorspace;

            // var t1 = DateTime.Now;
            var hDec = JpegCompress.tjInitDecompress();
            //左移8个字节            byte imgBuffer[jpg_buffer.Length];

            IntPtr pBufferPtr = IntPtr.Zero;
            //uint uJpgSize = (uint)jpg_buffer.Length;// (uint)jpg_buffer.Length - 8;
            unsafe {
                fixed (void* p = &jpg_buffer[0]) {
                    pBufferPtr = (IntPtr)p;// +8*sizeof(byte);
                    if (iOffset > -1) {
                        pBufferPtr = IntPtr.Add(pBufferPtr, iOffset); //偏移iBeg字节  pBufferPtr = IntPtr.Add(pBufferPtr, 8);
                    }
                }
            }
            //MessageBox.Show(pBufferPtr.ToString());
            lock (obj) {
                /*获取jpg图片相关信息但并不解压缩*/
                int ret = tjDecompressHeader3(hDec, pBufferPtr, uJpgSize, out jpegWidth, out jpegHeight, out jpegSubsamp, out jpegColorspace);
                if (0 != ret) {
                    tjDestroy(hDec);
                    return null;
                }
            }
            if (JpegCompress.TJColorSpaces.TJCS_GRAY == (JpegCompress.TJColorSpaces)jpegColorspace) {
            }
            else if (JpegCompress.TJColorSpaces.TJCS_RGB == (JpegCompress.TJColorSpaces)jpegColorspace) {
            }
            else if (JpegCompress.TJColorSpaces.TJCS_YCbCr == (JpegCompress.TJColorSpaces)jpegColorspace) {
            }
            /*解压缩时，tjDecompress2（）会自动根据设置的大小进行缩放，但是设置的大小要在它的支持范围，如1/2 1/4等*/

            // TJPixelFormats pixelfmt = TJPixelFormats.TJPF_RGB;

            ImageSize imgSize = new ImageSize(jpegWidth, jpegHeight, ImageType.UC8, 1);
            CFviImage cfImg = new CFviImage(imgSize);
            lock (obj) {
                int flags = 0;
                try {
                    JpegCompress.tjDecompress2(hDec, pBufferPtr, uJpgSize, cfImg.GetImageAdrs(0, 0, 0), jpegWidth, 0, jpegHeight, (int)JpegCompress.TJPixelFormats.TJPF_GRAY, flags);
                    JpegCompress.tjDestroy(hDec);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            return cfImg;
        }
       

    }
}
