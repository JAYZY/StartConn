using ComClassLib.core;
using FVIL.Forms;
using System.Windows.Forms;

namespace ComClassLib {
    public partial class CtrlView : UserControl {
        
        public CtrlView(string _name="") {
            InitializeComponent();
            imgView.ScrollBarH.Visible = false;
            imgView.ScrollBarV.Visible = false;
            IniView();
            // FVIL._SetUp.InitVisionLibrary();
            //imgView.Image = new FVIL.Data.CFviImage();
            // FVIL.File.Function.LoadImageFile("ico/VideoNoFound.png", imgView.Image, FVIL.PixelMode.Unpacking);
            //imgView.Refresh();
            if (!string.IsNullOrEmpty(_name)) this.Name = _name;
        }
        private void IniView() { 
            FigHandlingOverlay m_FigHandOverlay = new FigHandlingOverlay();
            imgView.Display.Overlays.Add(m_FigHandOverlay);
            m_FigHandOverlay.AddMouseEventHandler(imgView);
            imgView.MouseWheel += new System.Windows.Forms.MouseEventHandler(ImageView_MouseWheel);
             //imgView.MouseMove += new System.Windows.Forms.MouseEventHandler(ImgView_MouseMove);
            imgView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(ImgView_MouseDoubleClick);
            // imgView.MouseDown += new System.Windows.Forms.MouseEventHandler(ImgView_MouseDown);
            // imgView.MouseUp += new System.Windows.Forms.MouseEventHandler(ImgView_MouseUp);
        }
        public void LoadImg(byte[] imgByte, uint uJpgSize, int iOffset) {
            imgView.Image = JpegCompress.Decompress(imgByte, uJpgSize, iOffset);
            //double mag = imgView.Height * 1.0 / imgView.Display.ImageSize.Height;
            //if (mag < 0.01) {
            //    return;
            //}
            //imgView.Display.Magnification = mag;
        }

        public void LoadImg(string filePath) {

            // byte[] byteData = FileOp.FileHelper.getImageByte(filePath);
            //LoadImg(byteData, (uint)byteData.Length, 0);
            try {

                //imgView.Image.Dispose();
                imgView.Image = new FVIL.Data.CFviImage();
                FVIL.File.Function.LoadImageFile(filePath, imgView.Image, FVIL.PixelMode.Unpacking);
                //Fit();
            } catch {


            }
        }
        #region 鼠标事件

        /// <summary>
        /// 滚轮事件--图像放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ImageView_MouseWheel(object sender, MouseEventArgs e) {

            CFviImageView ImgV = (CFviImageView)sender;
            if (e.Delta > 0) {
                ImgV.Display.Magnification = ImgV.Display.Magnification * 1.2;
            } else if (e.Delta < 0) {
                ImgV.Display.Magnification = ImgV.Display.Magnification * 0.8;
            }

            ImgV.Refresh();

        }
       public void Fit() {
            double mag = imgView.Height * 1.0 / imgView.Display.ImageSize.Height;
            if (mag < 0.01) {
                return;
            }
            imgView.Display.Magnification = mag;
        }
        void ImgView_MouseDoubleClick(object sender, MouseEventArgs e) {
            base.OnMouseDoubleClick(e);
        }
        #endregion


    }
}
