using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
namespace ComClassLib.core {




    /// <summary>
    /// GDI恾宍曇廤儌乕僪
    /// </summary>
    public enum FigureHandlingMode {
        /// <summary>堏摦偲僒僀僘曄峏</summary>
        Normal,
        /// <summary>夞揮</summary>
        Rotate
    }

    /// <summary>
    /// 僉乕儃乕僪憖嶌忣曬峔憿懱
    /// </summary>
    public struct KeyboardInfo {
        /// <summary>
        /// 墴壓忬懺. 
        /// </summary>
        public bool IsDown;

        /// <summary>
        /// 墴壓偝傟偰偄傞僉乕.
        /// </summary>
        public bool Alt;

        /// <summary>
        /// 墴壓偝傟偰偄傞僉乕.
        /// </summary>
        public bool Control;

        /// <summary>
        /// 墴壓偝傟偰偄傞僉乕.
        /// </summary>
        public bool Shift;

        /// <summary>
        /// 墴壓偝傟偰偄傞僉乕.
        /// </summary>
        public Keys KeyCode;

        /// <summary>
        /// 墴壓偝傟偰偄傞僉乕.
        /// </summary>
        public Keys KeyData;

        /// <summary>
        /// 墴壓偝傟偰偄傞僉乕.
        /// </summary>
        public int KeyValue;
    };

    /// <summary>
    /// 儅僂僗儃僞儞墴壓帪偺嬮宍椞堟偺忣曬 
    /// </summary>
    public struct MouseInfo {
        /// <summary>
        /// 僌儕僢僾忬懺.
        /// </summary>
        public bool bGrip;

        /// <summary>
        /// 僌儕僢僾埵抲.
        /// </summary>
        public int iGripPosition;

        /// <summary>
        /// 儅僂僗億僀儞僞埵抲.
        /// </summary>
        public FVIL.Data.CFviPoint mouse;

        /// <summary>
        /// 慖戰偝傟偨恾宍.
        /// </summary>
        public FVIL.GDI.CFviGdiFigure figure;

        /// <summary>
        /// 儃僞儞墴壓帪偺恾宍埵抲.
        /// </summary>
        public FVIL.Data.CFviPoint position;

        /// <summary>
        /// 儃僞儞墴壓帪偺恾宍偺婡幉.
        /// </summary>
        public FVIL.Data.CFviPoint axis;

        /// <summary>
        /// 儃僞儞墴壓帪偺恾宍偺夞揮妏.
        /// </summary>
        public FVIL.Data.CFviAngle angle;

        /// <summary>
        /// 儃僞儞墴壓帪偺恾宍偺奜愙嬮宍.
        /// </summary>
        public FVIL.Data.CFviRectangle clip;
    };

    /// <summary>
    /// GDI恾宍傪儅僂僗偱憖嶌偡傞僆乕僶儗僀
    /// </summary>
    /// <remarks>
    ///		恾宍偺堏摦傗僒僀僘曄峏傪儅僂僗偱憖嶌偡傞偨傔偺僆乕僶儗僀偱偡丅
    /// </remarks>
    public class FigHandlingOverlay : FVIL.GDI.CFviOverlay {
        /// <summary>
        /// 僐儞僗僩儔僋僞
        /// </summary>
        public FigHandlingOverlay() {
            // 僉乕儃乕僪墴壓帪偺忣曬.
            m_KeyboardInfo.IsDown = false;
            m_KeyboardInfo.Alt = false;
            m_KeyboardInfo.Control = false;
            m_KeyboardInfo.Shift = false;
            m_KeyboardInfo.KeyCode = Keys.None;
            m_KeyboardInfo.KeyData = Keys.None;
            m_KeyboardInfo.KeyValue = 0;

            // 儅僂僗儃僞儞墴壓帪偺忣曬.
            m_MouseInfo.bGrip = false;
            m_MouseInfo.iGripPosition = 0;
            m_MouseInfo.figure = null;
        }

        /// <summary>
        /// 僉乕儃乕僪墴壓帪偺忣曬
        /// </summary>
        private KeyboardInfo m_KeyboardInfo = new KeyboardInfo();

        /// <summary>
        /// 儅僂僗儃僞儞墴壓帪偺忣曬 
        /// </summary>
        private MouseInfo m_MouseInfo = new MouseInfo();

        /// <summary>
        /// 恾宍曇廤儌乕僪
        /// </summary>
        public virtual FigureHandlingMode HandlingMode {
            get { return m_HandlingMode; }
            set { m_HandlingMode = value; }
        }
        private FigureHandlingMode m_HandlingMode;

        /// <summary>
        /// 恾宍偺庢摼 (慖戰懏惈巜掕)
        /// </summary>
        /// <param name="select">慖戰懏惈</param>
        /// <returns>
        ///		巜掕偝傟偨慖戰懏惈偲堦抳偡傞恾宍傪曉偟傑偡丅
        ///		尒偮偐傜側偗傟偽 null 傪曉偟傑偡丅
        /// </returns>
        public virtual FVIL.GDI.CFviGdiFigure GetSelectedFigure(bool select) {
            // 攝楍偺枛旜(倅曽岦偺慜柺)偐傜張棟偡傞.
            for (int i = Figures.Count - 1; i >= 0; i--) {
                FVIL.GDI.CFviGdiFigure figure = Figures[i];
                if (figure.Select == select) return figure;
            }
            return null;
        }

        /// <summary>
        /// 恾宍偺庢摼 (埵抲巜掕)
        /// </summary>
        /// <param name="x">巜掕埵抲X (僋儔僀傾儞僩嵗昗)</param>
        /// <param name="y">巜掕埵抲Y (僋儔僀傾儞僩嵗昗)</param>
        /// <returns>
        ///		巜掕埵抲偵偁傞恾宍傪曉偟傑偡丅
        ///		尒偮偐傟側偗傟偽 null 傪曉偟傑偡丅
        /// </returns>
        public virtual FVIL.GDI.CFviGdiFigure GetFigure(int x, int y) {
            return GetFigure(new Point(x, y));
        }

        /// <summary>
        /// 恾宍偺庢摼 (埵抲巜掕)
        /// </summary>
        /// <param name="location">巜掕埵抲 (僋儔僀傾儞僩嵗昗)</param>
        /// <returns>
        ///		巜掕埵抲偵偁傞恾宍傪曉偟傑偡丅
        ///		尒偮偐傟側偗傟偽 null 傪曉偟傑偡丅
        /// </returns>
        public virtual FVIL.GDI.CFviGdiFigure GetFigure(Point location) {
            // View僋儔僀傾儞僩椞堟忋偱偺嵗昗.
            // Scaling = true 偺帪偼丄夋憸嵗昗偵曄姺.
            FVIL.Data.CFviPoint position = (Scaling)
                ? DPtoIP(location, ScalingMode)
                : new FVIL.Data.CFviPoint(location);

            // 攝楍偺枛旜(倅曽岦偺慜柺)偐傜張棟偡傞.
            for (int i = Figures.Count - 1; i >= 0; i--) {
                FVIL.GDI.CFviGdiFigure figure = Figures[i];
                int iGripPosition = figure.CheckFocusMarkPosition(position, 8 / Magnification);
                if (iGripPosition != 0) return figure;
            }
            return null;
        }

        /// <summary>
        /// 帺恎偺暋惢偺惗惉 (宲彸昁恵)
        /// </summary>
        /// <returns>
        ///		帺恎偺僀儞僗僞儞僗偺暋惢傪曉偟傑偡丅
        /// </returns>
        public override System.Object Clone() {
            FigHandlingOverlay clone = new FigHandlingOverlay();
            clone.CopyFrom(this);
            return clone;
        }

        /// <summary>
        /// 僀儞僗僞儞僗偺暋惢 (宲彸昁恵)
        /// </summary>
        /// <param name="src">暋惢尦</param>
        /// <returns>
        ///		暋惢屻偺帺恎偺僀儞僗僞儞僗傪曉偟傑偡丅
        /// </returns>
        /// <remarks>
        ///		婎杮僋儔僗偺 CopyFrom 偼丄Figures 攝楍撪偺恾宍偺僀儞僗僞儞僗傪 Shallow 僐僺乕偟傑偡丅
        ///		偙偺応崌偼丄暋惢尦偲暋惢屻偺僆乕僶儗僀偱摨堦偺僀儞僗僞儞僗傪曐桳偡傞偙偲偵側傝傑偡丅
        ///		傕偟 Deep 僐僺乕(僀儞僗僞儞僗偺僐僺乕偱側偔丄撪梕傪暋惢偡傞偙偲)傪偟偨偄応崌偼丄
        ///		壓婰偺傛偆偵 CopyFigures 僆乕僶乕儔僀僪偟偰偔偩偝偄丅
        ///		婎杮僋儔僗偺 CopyFrom 偼丄僀儞僗僞儞僗偺 Shallow 僐僺乕傪峴偆慜偵 CopyFigures 傪屇傃弌偟傑偡丅
        /// </remarks>
        public override System.Object CopyFrom(System.Object src) {
            base.CopyFrom(src);
            return this;
        }

#if false  // TODO: 撈帺偺恾宍暋惢張棟傪峴偆応崌偼丄偙偺僨傿儗僋僥傿僽傪桳岠偵偟偰偔偩偝偄.
		/// <summary>
		/// 撈帺偺恾宍暋惢張棟
		/// </summary>
		/// <param name="src">暋惢尦</param>
		public override void CopyFigures(FVIL.GDI.CFviOverlay src)
		{
			FigureHandlingOverlay _src = (FigureHandlingOverlay)src;

			// TODO: 偙偙偱暋惢張棟傪峴偭偰偔偩偝偄.
		}
#endif

        /// <summary>
        /// 僴僢僔儏僐乕僪偺惗惉 (宲彸昁恵)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>
        /// 僀儞僗僞儞僗偺撪梕斾妑 (宲彸昁恵)
        /// </summary>
        /// <param name="src">斾妑懳徾</param>
        /// <returns>
        ///		堦抳偡傞応崌偼 true 丄堦抳偟側偄応崌偼 false 傪曉偟傑偡丅
        /// </returns>
        public override bool Equals(object src) {
            if (ReferenceEquals(src, null)) return false;

            try {
                FigHandlingOverlay _src = (FigHandlingOverlay)src;

                // TODO: 撈帺偺斾妑張棟偑偁傞応崌偼丄偙偙偱峴偭偰偔偩偝偄.

                return base.Equals(src);
            }
            catch (System.Exception) {
                return false;
            }
        }

        /// <summary>
        /// 斾妑僆儁儗乕僞 (摍壙)
        /// </summary>
        /// <param name="ope1">嵍曈抣</param>
        /// <param name="ope2">塃曈抣</param>
        /// <returns>
        ///		堦抳偡傞応崌偼 true 丄堦抳偟側偄応崌偼 false 傪曉偟傑偡丅
        /// </returns>
        public static bool operator ==(FigHandlingOverlay ope1, FigHandlingOverlay ope2) {
            return ope1.Equals(ope2);
        }

        /// <summary>
        /// 斾妑僆儁儗乕僞 (晄摍壙)
        /// </summary>
        /// <param name="ope1">嵍曈抣</param>
        /// <param name="ope2">塃曈抣</param>
        /// <returns>
        ///		堦抳偡傞応崌偼 false 丄堦抳偟側偄応崌偼 true 傪曉偟傑偡丅
        /// </returns>
        public static bool operator !=(FigHandlingOverlay ope1, FigHandlingOverlay ope2) {
            return !(ope1 == ope2);
        }

        /// <summary>
        /// 昤夋張棟
        /// </summary>
        /// <param name="hdc">昤夋愭偺僨僶僀僗僐儞僥僉僗僩</param>
        /// <param name="region">昤夋斖埻</param>
        /// <returns>
        ///		昤夋偝傟偨恾宍偺屄悢傪曉偟傑偡丅
        /// </returns>
        public override int Play(System.IntPtr hdc, System.Drawing.Rectangle region) {
            if (!Enable && !Active) return 0;
            int iCount = base.Play(hdc, region);
            return iCount;
        }

        /// <summary>
        /// 儅僂僗僀儀儞僩僴儞僪儔偺搊榐
        /// </summary>
        /// <param name="control"></param>
        public override void AddMouseEventHandler(System.Windows.Forms.Control control) {
            control.KeyDown += new System.Windows.Forms.KeyEventHandler(OnKeyDown);
            control.KeyUp += new System.Windows.Forms.KeyEventHandler(OnKeyUp);
            control.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
            control.MouseMove += new System.Windows.Forms.MouseEventHandler(OnMouseMove);
            control.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);
            control.MouseLeave += new System.EventHandler(OnMouseLeave);
        }

        /// <summary>
        /// 儅僂僗僀儀儞僩僴儞僪儔偺夝彍
        /// </summary>
        /// <param name="control"></param>
        public override void DelMouseEventHandler(System.Windows.Forms.Control control) {
            control.KeyDown -= new System.Windows.Forms.KeyEventHandler(OnKeyDown);
            control.KeyUp -= new System.Windows.Forms.KeyEventHandler(OnKeyUp);
            control.MouseDown -= new System.Windows.Forms.MouseEventHandler(OnMouseDown);
            control.MouseMove -= new System.Windows.Forms.MouseEventHandler(OnMouseMove);
            control.MouseUp -= new System.Windows.Forms.MouseEventHandler(OnMouseUp);
            control.MouseLeave -= new System.EventHandler(OnMouseLeave);
        }

        /// <summary>
        /// 儅僂僗僌儕僢僾壜斲敾掕
        /// </summary>
        /// <param name="sender">儅僂僗僀儀儞僩偑敪惗偟偨僐儞僩儘乕儖</param>
        /// <param name="e">敪惗偟偨儅僂僗僀儀儞僩</param>
        /// <param name="AllowGrip">僌儕僢僾嫋壜僗僥乕僞僗</param>
        public override void CanMouseGrip(System.Object sender, System.Windows.Forms.MouseEventArgs e, ref bool AllowGrip) {
            if (!Active) return;    // 姳徛偟側偄.

            if (AllowGrip) {
                try {
                    FVIL.Data.CFviPoint position = (Scaling)
                        ? DPtoIP(e.Location, ScalingMode)
                        : new FVIL.Data.CFviPoint(e.Location);

                    // 攝楍偺枛旜(倅曽岦偺慜柺)偐傜張棟偡傞.
                    for (int i = Figures.Count - 1; i >= 0; i--) {
                        FVIL.GDI.CFviGdiFigure figure = Figures[i];
                        if (m_MouseInfo.bGrip == true) {
                            if (ReferenceEquals(figure, m_MouseInfo.figure)) {
                                // 懠偺僆乕僶儗僀偱偺僌儕僢僾傪嫋壜偟側偄.
                                AllowGrip = false;
                                break;
                            }
                        }
                        else if (figure.Enable) {
                            int iGripPosition = figure.CheckFocusMarkPosition(position, 8 / Magnification);
                            if (iGripPosition != 0) {
                                if (figure is FVIL.GDI.CFviGdiPolyline) {
                                    // (!) 懡妏宍偵懳偡傞巄掕揑側張抲.
                                    // CFviGdiPolyline 偑 Close 偺偲偒傕捀揰偱斀墳偝偣傞張抲.
                                    // 僗儗僢僪僙乕僼偱偼側偄偺偱拲堄.
                                    // 杮棃偼丄宲彸偟偰 CheckFocusMarkPosition 傪僆乕僶乕儔僀僪偡傞傋偒.

                                    FVIL.GDI.CFviGdiPolyline _figure = (FVIL.GDI.CFviGdiPolyline)figure;
                                    if (_figure.Close) {
                                        _figure.Close = false;
                                        int iGripVetex = _figure.CheckFocusMarkPosition(position, 8 / Magnification);
                                        _figure.Close = true;
                                        // 捀揰傑偨偼曈埲奜偱奜愙嬮宍撪偺偲偒偼堏摦(-1)偲偡傞.
                                        iGripPosition = (iGripVetex != 0) ? iGripVetex : -1;
                                    }
                                }

                                // View僋儔僀傾儞僩椞堟忋偱偺嵗昗.
                                // Scaling = true 偺帪偼丄夋憸嵗昗偵曄姺.
                                FVIL.Data.CFviPoint mouse = (Scaling)
                                    ? DPtoIP(e.Location, ScalingMode)
                                    : new FVIL.Data.CFviPoint(e.Location);

                                // 僌儕僢僾埵抲偲恾宍偺忬懺傪曐帩偡傞.
                                m_MouseInfo.bGrip = true;
                                m_MouseInfo.iGripPosition = iGripPosition;
                                m_MouseInfo.mouse = mouse;
                                m_MouseInfo.figure = figure;
                                m_MouseInfo.position = figure.Position;
                                m_MouseInfo.axis = figure.Axis;
                                m_MouseInfo.angle = figure.Angle;
                                m_MouseInfo.clip = figure.GetClipRect();
                                Validate(ref m_MouseInfo);      // 桳岠壔.

                                // 懠偺僆乕僶儗僀偱偺僌儕僢僾傪嫋壜偟側偄.
                                AllowGrip = false;
                                break;
                            }
                        }
                    }
                }
                catch (System.Exception) {
                }
            }
            return;
        }

        /// <summary>
        /// 儅僂僗僇乕僜儖偺宍忬曄峏壜斲敾掕
        /// </summary>
        /// <param name="sender">儅僂僗僀儀儞僩偑敪惗偟偨僐儞僩儘乕儖</param>
        /// <param name="e">敪惗偟偨儅僂僗僀儀儞僩</param>
        /// <param name="AllowChange">僇乕僜儖曄峏嫋壜僗僥乕僞僗</param>
        public override void CanChangeCursor(System.Object sender, System.Windows.Forms.MouseEventArgs e, ref bool AllowChange) {
            if (!Active) return;    // 姳徛偟側偄.

            try {
                System.Windows.Forms.Cursor cursor = System.Windows.Forms.Cursors.Default;

                // sender 傑偨偼 e 偑柍岠側応崌偼柍帇偡傞.
                if (ReferenceEquals(sender, null)) return;
                if (ReferenceEquals(e, null)) return;

                System.Windows.Forms.Control control = (System.Windows.Forms.Control)sender;

                // 
                // 張棟懳徾偺恾宍.
                // 
                bool selected = false;
                bool redraw = false;

                // View僋儔僀傾儞僩椞堟忋偱偺嵗昗.
                // Scaling = true 偺帪偼丄夋憸嵗昗偵曄姺.
                FVIL.Data.CFviPoint position = (Scaling)
                    ? DPtoIP(e.Location, ScalingMode)
                    : new FVIL.Data.CFviPoint(e.Location);

                // 攝楍偺枛旜(倅曽岦偺慜柺)偐傜張棟偡傞.
                for (int i = Figures.Count - 1; i >= 0; i--) {
                    FVIL.GDI.CFviGdiFigure figure = Figures[i];
                    if (m_MouseInfo.bGrip == true) {
                        if (ReferenceEquals(figure, m_MouseInfo.figure)) {
                            if (figure.Select != true) redraw = true;
                            figure.Select = true;
                            selected = true;
                        }
                    }
                    else if (figure.Enable) {
                        int iGripPosition = figure.CheckFocusMarkPosition(position, 8 / Magnification);
                        if (iGripPosition != 0 && selected == false) {
                            if (figure.Select != true) redraw = true;
                            figure.Select = true;
                            selected = true;
                        }
                        else {
                            if (figure.Select != false) redraw = true;
                            figure.Select = false;
                        }
                    }
                }

                if (redraw)
                    ((System.Windows.Forms.Control)sender).Refresh();

                cursor = System.Windows.Forms.Cursors.SizeAll;

                // (!) 僇乕僜儖宍忬偺曄峏張棟
                //     AllowChange 偼丄懠偺僆乕僶儗僀偺僇乕僜儖曄峏張棟偲姳徛偟側偄傛偆偵偡傞堊偺僼儔僌偱偡.
                //     婛偵 AllowChange 偑 false 偺帪偼丄偙偺僆乕僶儗僀偱偺僇乕僜儖曄峏張棟傪峴偄傑偣傫丅
                //     AllowChange 偑 true 偺帪丄僇乕僜儖宍忬偺曄峏張棟傪峴偄傑偡丅
                //     僇乕僜儖曄峏屻丄AllowChange 傪 false 偵曄峏偟傑偡丅

                // 僇乕僜儖曄峏.
                if (AllowChange && selected == true) {
                    control.Cursor = cursor;
                    AllowChange = false;    // 曄峏傪嫋壜偟側偄.
                }
            }
            catch (System.Exception) {
            }
            return;
        }

        /// <summary>
        /// 僉乕儃乕僪僀儀儞僩 (僉乕傪墴壓偟偨帪)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnKeyDown(object sender, KeyEventArgs e) {
            m_KeyboardInfo.IsDown = true;
            m_KeyboardInfo.Alt = e.Alt;
            m_KeyboardInfo.Control = e.Control;
            m_KeyboardInfo.Shift = e.Shift;
            m_KeyboardInfo.KeyCode = e.KeyCode;
            m_KeyboardInfo.KeyData = e.KeyData;
            m_KeyboardInfo.KeyValue = e.KeyValue;
        }

        /// <summary>
        /// 僉乕儃乕僪僀儀儞僩 (僉乕傪棧偟偨帪)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnKeyUp(object sender, KeyEventArgs e) {
            m_KeyboardInfo.IsDown = false;
        }

        /// <summary>
        /// 儅僂僗儃僞儞偑墴壓偝傟偨帪偺張棟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e) {
        }

        /// <summary>
        /// 儅僂僗儃僞儞偑曻偝傟偨帪偺張棟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e) {
            m_MouseInfo.bGrip = false;
        }

        /// <summary>
        /// 儅僂僗僇乕僜儖偑堏摦偟偰偄傞帪偺張棟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e) {
            if (Active == false) return;

            if (m_MouseInfo.bGrip) {
                // View僋儔僀傾儞僩椞堟忋偱偺嵗昗.
                // Scaling = true 偺帪偼丄夋憸嵗昗偵曄姺.
                FVIL.Data.CFviPoint current = (Scaling)
                    ? DPtoIP(e.Location, ScalingMode)
                    : new FVIL.Data.CFviPoint(e.Location);

                FVIL.GDI.CFviGdiFigure figure = m_MouseInfo.figure;

                // 恾宍偺憖嶌.
                HandlingFigure(m_MouseInfo.figure, current, m_MouseInfo, m_KeyboardInfo);

                ((System.Windows.Forms.Control)sender).Refresh();
            }
        }

        /// <summary>
        /// 儅僂僗僇乕僜儖偑棧傟偨帪偺張棟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMouseLeave(System.Object sender, System.EventArgs e) {
            m_MouseInfo.bGrip = false;
        }

        /// <summary>
        /// 儅僂僗墴壓帪偺忣曬傪桳岠壔偡傞
        /// </summary>
        /// <param name="info">儅僂僗墴壓帪偺忣曬</param>
        /// <returns>
        ///		曄峏偟偨応崌偼 true 傪曉偟傑偡丅偦傟埲奜偼 false 傪曉偟傑偡丅
        /// </returns>
        /// <remarks>
        ///		恾宍偺嵗昗偑斀揮偟偰偄偨応崌偵丄奜愙嬮宍忋偺儅僂僗僌儕僢僾埵抲傕斀揮偟傑偡丅
        ///		奜愙嬮宍偵傛偭偰曄宍傪峴偆恾宍偑懳徾偱偡丅
        ///		偙偺儊僜僢僪偼丄儅僂僗僌儕僢僾帪偵堦搙偩偗屇傃弌偡傛偆偵偟偰偔偩偝偄丅
        /// </remarks>
        protected virtual bool Validate(ref MouseInfo info) {
            if (ReferenceEquals(info.figure, null)) return false;
            if (info.iGripPosition == 0) return false;

            bool bReverseH = false; // 悈暯曽岦斀揮.
            bool bReverseV = false; // 悅捈曽岦斀揮.

            // 懳徾偺恾宍.
            if (info.figure is FVIL.GDI.CFviGdiCircle) {
                FVIL.GDI.CFviGdiCircle figure = (FVIL.GDI.CFviGdiCircle)info.figure;
                bReverseH = (figure.Radius < 0);
                bReverseV = (figure.Radius < 0);
            }
            else if (info.figure is FVIL.GDI.CFviGdiEllipse) {
                FVIL.GDI.CFviGdiEllipse figure = (FVIL.GDI.CFviGdiEllipse)info.figure;
                bReverseH = (figure.AxisX < 0);
                bReverseV = (figure.AxisY < 0);
            }
            else if (info.figure is FVIL.GDI.CFviGdiRectangle) {
                FVIL.GDI.CFviGdiRectangle figure = (FVIL.GDI.CFviGdiRectangle)info.figure;
                bReverseH = (figure.Width < 0);
                bReverseV = (figure.Height < 0);
            }
            else {
                return false;
            }

            // 奜愙嬮宍忋偺埵抲.
            FVIL.GDI.RectPosition rectpos = (FVIL.GDI.RectPosition)info.iGripPosition;
            bool L = ((rectpos & FVIL.GDI.RectPosition.Left) == FVIL.GDI.RectPosition.Left);
            bool R = ((rectpos & FVIL.GDI.RectPosition.Right) == FVIL.GDI.RectPosition.Right);
            bool T = ((rectpos & FVIL.GDI.RectPosition.Top) == FVIL.GDI.RectPosition.Top);
            bool B = ((rectpos & FVIL.GDI.RectPosition.Bottom) == FVIL.GDI.RectPosition.Bottom);

            // 悈暯曽岦斀揮.
            if (bReverseH) {
                if (L && !R)    // 尰嵼丄嵍偑 ON 偺帪丄嵍傪 OFF 偟偰塃傪 ON 偡傞.
                {
                    rectpos &= ~FVIL.GDI.RectPosition.Left;
                    rectpos |= FVIL.GDI.RectPosition.Right;
                }
                if (R && !L)    // 尰嵼丄塃偑 ON 偺帪丄塃傪 OFF 偟偰嵍傪 ON 偡傞.
                {
                    rectpos &= ~FVIL.GDI.RectPosition.Right;
                    rectpos |= FVIL.GDI.RectPosition.Left;
                }
            }

            // 悅捈曽岦斀揮.
            if (bReverseV) {
                if (T && !B)    // 尰嵼丄忋偑 ON 偺帪丄忋傪 OFF 偟偰壓傪 ON 偡傞.
                {
                    rectpos &= ~FVIL.GDI.RectPosition.Top;
                    rectpos |= FVIL.GDI.RectPosition.Bottom;
                }
                if (B && !T)    // 尰嵼丄壓偑 ON 偺帪丄壓傪 OFF 偟偰忋傪 ON 偡傞.
                {
                    rectpos &= ~FVIL.GDI.RectPosition.Bottom;
                    rectpos |= FVIL.GDI.RectPosition.Top;
                }
            }

            // 斀塮.
            info.iGripPosition = (int)rectpos;

            return (bReverseH || bReverseV);
        }

        /// <summary>
        /// 恾宍偺憖嶌 (暘婒)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void HandlingFigure(FVIL.GDI.CFviGdiFigure figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            FigureHandlingMode mode = HandlingMode;

            // 儌乕僪偺愗傝懼偊.
            if (key.IsDown &&
                key.Control == true &&
                key.Alt == false &&
                key.Shift == false) {
                if (mode == FigureHandlingMode.Normal)
                    mode = FigureHandlingMode.Rotate;
                else if (mode == FigureHandlingMode.Rotate)
                    mode = FigureHandlingMode.Normal;
            }

            if (mode == FigureHandlingMode.Rotate) {
                // 恾宍偺夞揮.
                RotateFigure(figure, current, info, key);
            }
            else {
                // 恾宍偺曄宍傑偨偼堏摦.
                if (figure is FVIL.GDI.CFviGdiPoint) {
                    ModifyFigure((FVIL.GDI.CFviGdiPoint)figure, current, info, key);
                }
                else if (figure is FVIL.GDI.CFviGdiAnchor) {
                    ModifyFigure((FVIL.GDI.CFviGdiAnchor)figure, current, info, key);
                }
                else if (figure is FVIL.GDI.CFviGdiLine) {
                    ModifyFigure((FVIL.GDI.CFviGdiLine)figure, current, info, key);
                }
                else if (figure is FVIL.GDI.CFviGdiLineSegment) {
                    ModifyFigure((FVIL.GDI.CFviGdiLineSegment)figure, current, info, key);
                }
                else if (figure is FVIL.GDI.CFviGdiCircle) {
                    ModifyFigure((FVIL.GDI.CFviGdiCircle)figure, current, info, key);
                }
                else if (figure is FVIL.GDI.CFviGdiEllipse) {
                    ModifyFigure((FVIL.GDI.CFviGdiEllipse)figure, current, info, key);
                }
                else if (figure is FVIL.GDI.CFviGdiRectangle) {
                    ModifyFigure((FVIL.GDI.CFviGdiRectangle)figure, current, info, key);
                }
                else if (figure is FVIL.GDI.CFviGdiPolyline) {
                    ModifyFigure((FVIL.GDI.CFviGdiPolyline)figure, current, info, key);
                }
                else {
                    MoveFigure(figure, current, info, key);
                }
            }
        }

        /// <summary>
        /// 恾宍偺夞揮 (斈梡)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void RotateFigure(FVIL.GDI.CFviGdiFigure figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            FVIL.Data.CFviAngle st = Math.Angle(info.mouse, Math.Add(figure.Position, figure.Axis));
            FVIL.Data.CFviAngle ed = Math.Angle(current, Math.Add(figure.Position, figure.Axis));

            // 儃僞儞墴壓帪偐傜偺堏摦検傪寁嶼.
            FVIL.Data.CFviAngle move = Math.Sub(ed, st);

            // 恾宍傪夞揮.
            figure.Angle = Math.Add(info.angle, move);
        }

        /// <summary>
        /// 恾宍偺堏摦 (斈梡)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void MoveFigure(FVIL.GDI.CFviGdiFigure figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 儃僞儞墴壓帪偐傜偺堏摦検傪寁嶼.
            FVIL.Data.CFviPoint move = Math.Sub(current, info.mouse);

            // 恾宍傪堏摦.
            figure.Position = Math.Add(info.position, move);
        }

        /// <summary>
        /// 恾宍偺堏摦 (揰)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiPoint figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 恾宍傪堏摦.
            if (info.iGripPosition != 0) {
                MoveFigure(figure, current, info, key);
                return;
            }
        }

        /// <summary>
        /// 恾宍偺堏摦 (傾儞僇乕)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiAnchor figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 恾宍傪堏摦.
            if (info.iGripPosition != 0) {
                MoveFigure(figure, current, info, key);
                return;
            }

            // 儅僂僗墴壓帪偺忣曬偱嵗昗寁嶼偡傞.
            FVIL.Data.CFviPoint axis = Math.Add(info.position, info.axis);
            FVIL.Data.CFviAngle angle = Math.Invert(info.angle);
            FVIL.Data.CFviPoint pos = FVIL.Math.Rotate(current, axis, angle);

            if (info.iGripPosition == 1)            // 巒揰.
            {
                figure.St = pos;
            }
            else if (info.iGripPosition == 2)       // 廔揰.
            {
                figure.Ed = pos;
            }

            // --- 婡幉偺曗惓.
            if (angle.Degree != 0.0)
                figure.Axis = Math.Sub(axis, figure.Position);
        }

        /// <summary>
        /// 恾宍偺堏摦 (捈慄)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiLine figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 恾宍傪堏摦.
            if (info.iGripPosition < 0) {
                MoveFigure(figure, current, info, key);
                return;
            }

            // 儅僂僗墴壓帪偺忣曬偱嵗昗寁嶼偡傞.
            FVIL.Data.CFviPoint axis = Math.Add(info.position, info.axis);
            FVIL.Data.CFviAngle angle = Math.Invert(info.angle);
            FVIL.Data.CFviPoint pos = FVIL.Math.Rotate(current, axis, angle);

            // 仸捈慄偺巒揰偲廔揰偼 GetClipRect 偐傜庢摼偱偒傑偡偑丄孹偒傪峫椂偡傞昁梫偑偁傝傑偡.
            FVIL.Data.CFviLineSegment ls = new FVIL.Data.CFviLineSegment(info.clip.St, info.clip.Ed);
            if (info.position != info.clip.St)  // 巒揰偑嵍忋偱側偄.
            {
                ls.SX = info.clip.St.X;
                ls.SY = info.clip.Ed.Y;
                ls.EX = info.clip.Ed.X;
                ls.EY = info.clip.St.Y;
            }

            // 恾宍偺曄宍.
            if (info.iGripPosition == 1)            // 巒揰.
            {
                ls.St = pos;
                figure.CopyFrom(ls.ToCFviLine());
            }
            else if (info.iGripPosition == 2)       // 廔揰.
            {
                ls.Ed = pos;
                figure.CopyFrom(ls.ToCFviLine());
            }

            // --- 婡幉偺曗惓.
            //     仸捈慄偼 Size 偵傛偭偰巒揰偲廔揰偑曄峏偝傟傞偨傔丄婡幉偺曗惓偼偱偒傑偣傫.
            if (angle.Degree != 0.0)
                figure.Axis = Math.Sub(axis, figure.Position);
        }

        /// <summary>
        /// 恾宍偺堏摦 (慄暘)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiLineSegment figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 恾宍傪堏摦.
            if (info.iGripPosition < 0) {
                MoveFigure(figure, current, info, key);
                return;
            }

            // 儅僂僗墴壓帪偺忣曬偱嵗昗寁嶼偡傞.
            FVIL.Data.CFviPoint axis = Math.Add(info.position, info.axis);
            FVIL.Data.CFviAngle angle = Math.Invert(info.angle);
            FVIL.Data.CFviPoint pos = FVIL.Math.Rotate(current, axis, angle);

            // 恾宍偺曄宍.
            if (info.iGripPosition == 1)            // 巒揰.
            {
                figure.St = pos;
            }
            else if (info.iGripPosition == 2)       // 廔揰.
            {
                figure.Ed = pos;
            }

            // --- 婡幉偺曗惓.
            if (angle.Degree != 0.0)
                figure.Axis = Math.Sub(axis, figure.Position);
        }

        /// <summary>
        /// 恾宍偺堏摦 (恀墌)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiCircle figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 恾宍傪堏摦.
            FVIL.GDI.RectPosition rectpos = (FVIL.GDI.RectPosition)info.iGripPosition;
            if ((rectpos & FVIL.GDI.RectPosition.ALL) == FVIL.GDI.RectPosition.ALL) {
                MoveFigure(figure, current, info, key);
                return;
            }

            // 儅僂僗墴壓帪偺忣曬偱嵗昗寁嶼偡傞.
            FVIL.Data.CFviPoint axis = Math.Add(info.position, info.axis);
            FVIL.Data.CFviAngle angle = Math.Invert(info.angle);
            FVIL.Data.CFviPoint mouse = FVIL.Math.Rotate(info.mouse, axis, angle);
            FVIL.Data.CFviPoint pos = FVIL.Math.Rotate(current, axis, angle);
            FVIL.Data.CFviPoint move = Math.Sub(pos, mouse);

            // 恾宍偺曄宍.
            bool L = ((rectpos & FVIL.GDI.RectPosition.Left) == FVIL.GDI.RectPosition.Left);
            bool R = ((rectpos & FVIL.GDI.RectPosition.Right) == FVIL.GDI.RectPosition.Right);
            bool T = ((rectpos & FVIL.GDI.RectPosition.Top) == FVIL.GDI.RectPosition.Top);
            bool B = ((rectpos & FVIL.GDI.RectPosition.Bottom) == FVIL.GDI.RectPosition.Bottom);

            FVIL.Data.CFviRectangle clip = figure.GetClipRect();

            if (L && T) {
                clip.Left = (mouse.X + move.X);
                clip.Top = (mouse.Y + move.X);
            }
            else if (L && B) {
                clip.Left = (mouse.X + move.X);
                clip.Bottom = (mouse.Y - move.X);
            }
            else if (R && T) {
                clip.Right = (mouse.X + move.X);
                clip.Top = (mouse.Y - move.X);
            }
            else if (R && B) {
                clip.Right = (mouse.X + move.X);
                clip.Bottom = (mouse.Y + move.X);
            }
            else if (L) {
                clip.Left = (mouse.X + move.X);
                clip.Top += move.X / 2;
                clip.Bottom -= move.X / 2;
            }
            else if (R) {
                clip.Right = (mouse.X + move.X);
                clip.Top -= move.X / 2;
                clip.Bottom += move.X / 2;
            }
            else if (T) {
                clip.Top = (mouse.Y + move.Y);
                clip.Left += move.Y / 2;
                clip.Right -= move.Y / 2;
            }
            else if (B) {
                clip.Bottom = (mouse.Y + move.Y);
                clip.Left -= move.Y / 2;
                clip.Right += move.Y / 2;
            }

            double radius_x = clip.Width / 2;
            double radius_y = clip.Height / 2;

            figure.X = clip.X + radius_x;
            figure.Y = clip.Y + radius_y;

            if (L || R) {
                figure.Radius = clip.Width / 2;
            }
            else {
                figure.Radius = clip.Height / 2;
            }

            // --- 婡幉偺曗惓.
            if (angle.Degree != 0.0)
                figure.Axis = Math.Sub(axis, figure.Position);
        }

        /// <summary>
        /// 恾宍偺堏摦 (懭墌)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiEllipse figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 恾宍傪堏摦.
            FVIL.GDI.RectPosition rectpos = (FVIL.GDI.RectPosition)info.iGripPosition;
            if ((rectpos & FVIL.GDI.RectPosition.ALL) == FVIL.GDI.RectPosition.ALL) {
                MoveFigure(figure, current, info, key);
                return;
            }

            // 儅僂僗墴壓帪偺忣曬偱嵗昗寁嶼偡傞.
            FVIL.Data.CFviPoint axis = Math.Add(info.position, info.axis);
            FVIL.Data.CFviAngle angle = Math.Invert(info.angle);
            FVIL.Data.CFviPoint pos = FVIL.Math.Rotate(current, axis, angle);

            // 恾宍偺曄宍.
            FVIL.Data.CFviRectangle clip = figure.GetClipRect();

            if ((rectpos & FVIL.GDI.RectPosition.Left) == FVIL.GDI.RectPosition.Left) {
                clip.Left = pos.X;
            }
            if ((rectpos & FVIL.GDI.RectPosition.Right) == FVIL.GDI.RectPosition.Right) {
                clip.Right = pos.X;
            }
            if ((rectpos & FVIL.GDI.RectPosition.Top) == FVIL.GDI.RectPosition.Top) {
                clip.Top = pos.Y;
            }
            if ((rectpos & FVIL.GDI.RectPosition.Bottom) == FVIL.GDI.RectPosition.Bottom) {
                clip.Bottom = pos.Y;
            }

            double radius_x = clip.Width / 2;
            double radius_y = clip.Height / 2;

            figure.X = clip.X + radius_x;
            figure.Y = clip.Y + radius_y;
            figure.AxisX = radius_x;
            figure.AxisY = radius_y;

            // --- 婡幉偺曗惓.
            if (angle.Degree != 0.0)
                figure.Axis = Math.Sub(axis, figure.Position);
        }

        /// <summary>
        /// 恾宍偺堏摦 (嬮宍)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiRectangle figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            // 恾宍傪堏摦.
            FVIL.GDI.RectPosition rectpos = (FVIL.GDI.RectPosition)info.iGripPosition;
            if ((rectpos & FVIL.GDI.RectPosition.ALL) == FVIL.GDI.RectPosition.ALL) {
                MoveFigure(figure, current, info, key);
                return;
            }

            // 儅僂僗墴壓帪偺忣曬偱嵗昗寁嶼偡傞.
            FVIL.Data.CFviPoint axis = Math.Add(info.position, info.axis);
            FVIL.Data.CFviAngle angle = Math.Invert(info.angle);
            FVIL.Data.CFviPoint pos = FVIL.Math.Rotate(current, axis, angle);

            // 恾宍偺曄宍.
            FVIL.Data.CFviRectangle clip = figure.GetClipRect();

            if ((rectpos & FVIL.GDI.RectPosition.Left) == FVIL.GDI.RectPosition.Left) {
                clip.Left = pos.X;
            }
            if ((rectpos & FVIL.GDI.RectPosition.Right) == FVIL.GDI.RectPosition.Right) {
                clip.Right = pos.X;
            }
            if ((rectpos & FVIL.GDI.RectPosition.Top) == FVIL.GDI.RectPosition.Top) {
                clip.Top = pos.Y;
            }
            if ((rectpos & FVIL.GDI.RectPosition.Bottom) == FVIL.GDI.RectPosition.Bottom) {
                clip.Bottom = pos.Y;
            }

            figure.Left = clip.Left;
            figure.Right = clip.Right;
            figure.Top = clip.Top;
            figure.Bottom = clip.Bottom;

            // --- 婡幉偺曗惓.
            if (angle.Degree != 0.0)
                figure.Axis = Math.Sub(axis, figure.Position);
        }

        /// <summary>
        /// 恾宍偺堏摦 (懡妏宍)
        /// </summary>
        /// <param name="figure">恾宍</param>
        /// <param name="current">尰嵼偺儅僂僗埵抲</param>
        /// <param name="info">儃僞儞墴壓帪偺忣曬</param>
        /// <param name="key">儃僞儞墴壓帪偺僉乕儃乕僪忣曬</param>
        protected virtual void ModifyFigure(FVIL.GDI.CFviGdiPolyline figure, FVIL.Data.CFviPoint current, MouseInfo info, KeyboardInfo key) {
            if (info.iGripPosition == 0) return;

            if (figure.Close) {
                if (info.iGripPosition < 0) {
                    // 恾宍慡懱傪堏摦偡傞.
                    MoveFigure(figure, current, info, key);
                }
                else if (0 < info.iGripPosition && info.iGripPosition <= figure.Count) {
                    // 捀揰傪堏摦偟偰丄恾宍傪曄宍偡傞.
                    figure.Close = false;
                    ModifyFigure(figure, current, info, key);
                    figure.Close = true;
                }
            }
            else {
                // 儅僂僗墴壓帪偺忣曬偱嵗昗寁嶼偡傞.
                FVIL.Data.CFviPoint axis = Math.Add(info.position, info.axis);
                FVIL.Data.CFviAngle angle = Math.Invert(info.angle);
                FVIL.Data.CFviPoint pos = FVIL.Math.Rotate(current, axis, angle);

                if (info.iGripPosition < 0) {
                    // 恾宍慡懱傪堏摦偡傞.
                    MoveFigure(figure, current, info, key);
                }
                else if (0 < info.iGripPosition && info.iGripPosition <= figure.Count) {
                    // 捀揰傪堏摦偟偰丄恾宍傪曄宍偡傞.
                    int index = info.iGripPosition - 1;
                    figure[index] = pos;

                    // --- 婡幉偺曗惓.
                    if (angle.Degree != 0.0)
                        figure.Axis = Math.Sub(axis, figure.Position);
                }
            }
        }

    }

}
