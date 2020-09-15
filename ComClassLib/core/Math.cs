using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComClassLib.core {
    public partial class Math {
        /// <summary>
        /// 僐儞僗僩儔僋僞
        /// </summary>
        public Math() {
        }

        /// <summary>
        /// 揰嵗昗偺壛嶼
        /// </summary>
        /// <param name="ope1">嵍曈抣</param>
        /// <param name="ope2">塃曈抣</param>
        /// <returns>
        ///		壛嶼寢壥傪曉偟傑偡丅
        /// </returns>
        public static FVIL.Data.CFviPoint Add(FVIL.Data.CFviPoint ope1, FVIL.Data.CFviPoint ope2) {
            ope1.X += ope2.X;
            ope1.Y += ope2.Y;
            return ope1;
        }

        /// <summary>
        /// 揰嵗昗偺尭嶼
        /// </summary>
        /// <param name="ope1">嵍曈抣</param>
        /// <param name="ope2">塃曈抣</param>
        /// <returns>
        ///		尭嶼寢壥傪曉偟傑偡丅
        /// </returns>
        public static FVIL.Data.CFviPoint Sub(FVIL.Data.CFviPoint ope1, FVIL.Data.CFviPoint ope2) {
            ope1.X -= ope2.X;
            ope1.Y -= ope2.Y;
            return ope1;
        }

        /// <summary>
        /// 俀揰偺拞怱
        /// </summary>
        /// <param name="ope1">嵍曈抣</param>
        /// <param name="ope2">塃曈抣</param>
        /// <returns>
        ///		拞怱傪曉偟傑偡丅
        /// </returns>
        public static FVIL.Data.CFviPoint Center(FVIL.Data.CFviPoint ope1, FVIL.Data.CFviPoint ope2) {
            FVIL.Data.CFviPoint center = new FVIL.Data.CFviPoint((ope1.X + ope2.X) / 2, (ope1.Y + ope2.Y) / 2);
            return center;
        }

        /// <summary>
        /// 妏搙偺壛嶼
        /// </summary>
        /// <param name="ope1">嵍曈抣</param>
        /// <param name="ope2">塃曈抣</param>
        /// <returns>
        ///		壛嶼寢壥傪曉偟傑偡丅
        /// </returns>
        public static FVIL.Data.CFviAngle Add(FVIL.Data.CFviAngle ope1, FVIL.Data.CFviAngle ope2) {
            ope1.Degree += ope2.Degree;
            return ope1;
        }

        /// <summary>
        /// 妏搙偺尭嶼
        /// </summary>
        /// <param name="ope1">嵍曈抣</param>
        /// <param name="ope2">塃曈抣</param>
        /// <returns>
        ///		尭嶼寢壥傪曉偟傑偡丅
        /// </returns>
        public static FVIL.Data.CFviAngle Sub(FVIL.Data.CFviAngle ope1, FVIL.Data.CFviAngle ope2) {
            ope1.Degree -= ope2.Degree;
            return ope1;
        }

        /// <summary>
        /// 妏搙偺斀揮
        /// </summary>
        /// <param name="angle">妏搙</param>
        /// <returns>
        ///		斀揮偟偨妏搙傪曉偟傑偡丅
        /// </returns>
        public static FVIL.Data.CFviAngle Invert(FVIL.Data.CFviAngle angle) {
            angle.Degree = -angle.Degree;
            return angle;
        }

        /// <summary>
        /// 妏搙偺嶼弌
        /// </summary>
        /// <param name="point">巜掕揰</param>
        /// <param name="axis">夞揮偺婡幉</param>
        /// <returns>
        ///		嶼弌偝傟偨妏搙傪曉偟傑偡丅
        /// </returns>
        public static FVIL.Data.CFviAngle Angle(FVIL.Data.CFviPoint point, FVIL.Data.CFviPoint axis) {
            FVIL.Data.CFviAngle angle = new FVIL.Data.CFviAngle();

            // 巜掕偝傟偨揰偺嵗昗偐傜妏搙傪嶼弌偡傞.
            double xL = point.X - axis.X;
            double yL = point.Y - axis.Y;
            if (!(xL == 0 && yL == 0)) {
                double R = System.Math.Atan2(yL, xL);
                angle.Radian = R;
                if (angle.Degree < 0)
                    angle = new FVIL.Data.CFviAngle(360 + angle.Degree);
            }
            return angle;
        }
    }

}
