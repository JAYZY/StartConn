using ComClassLib.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComClassLib.core {
    public class PicInfo {
      //"TIM": 20082618512144300,
      //"KMV": "",
      //"CID": 3,
      //"POL": "",
      //"STA": 0,
      //"GPS": "",
      //"STN": "",
      //"STNUTF": "",
      //"TIS": 0.0,
      //"FRN": 622,
      //"UTC": 132429127415432790
        private string sPol;
        //拍摄时间
        public Int64 TIM { get; set; }
        //公里标
        public string KMV { get; set; }
        //相机编号
        public int CID { get; set; }
        //支柱号
        public string POL {
            get {
                return sPol;
            }
            set { sPol = value.Substring(value.IndexOf(':') + 1); }

        }
        //站区编号
        public int STA { get; set; }
        //GPS
        public string GPS { get; set; }
        private string sStationName;
        public int iSubDbInd; //图像数据库下标
        //站区间
        public string STN {
            get { return sStationName; }
            set {
                if (String.IsNullOrEmpty(value)) {
                    sStationName = "";
                }
                sStationName = value;
            }// FileOp.FileHelper.ConvertToStr(value); }

        }
        public string STNUTF {
            get; set;
        }
        public double TIS {
            get; set;
        }
        public long FRN {
            get; set;
        }
        public Int64 UTC {
            get; set;
        }
        //"TIS": 0.0,
        //"FRN": 622,
        //"UTC": 132429127415432790

        public PicInfo(Int64 iImgKey, int cID, long tIM, string pOL, string kMV, int iSubDbInd = 1) {
            this.iImgKey = iImgKey;
            CID = cID;
            TIM = tIM;
            KMV = kMV;
            POL = pOL;
            this.iSubDbInd = iSubDbInd;
        }


        ////拍摄时间
        //public Int64 TIM { get; set; }
        ////公里标
        //public string KMV { get; set; }
        ////相机编号
        //public int CID { get; set; }
        ////支柱号
        //public string POL { get; set; }
        ////站区编号
        //public int STA { get; set; }
        ////GPS
        //public string GPS { get; set; }

        private Int64 iImgKey;
        public void SetImgKey(Int64 _iImgKey) { iImgKey = _iImgKey; }
        public Int64 GetImgKey() { return iImgKey; }

        private bool isValid;
        public void SetValid(bool _isValid) { isValid = _isValid; }
        public bool GetValid() { return isValid; }

        //离线使用
        private byte[] img;
        public void SetImg(byte[] _img) { this.img = _img; }
        public byte[] Getimg(SqliteHelper sqlite, bool isReadDb = false) {
            if (isReadDb && this.img == null) {
                string strSQL = "select imgContent from imgInfo where imgGUID=" + this.iImgKey;
                // SqliteHelper sqlite = SqliteHelper.GetSqlite($"{DbName.CurrDb.ToString()}s{iSubDbInd}");
                if (sqlite == null)
                    return null;
                object obj = sqlite.ExecuteDataRow(strSQL, null)["imgContent"];
                if (obj is System.DBNull)
                    return null;
                this.img = (byte[])obj;
            }
            return this.img;
        }
        public bool HasFault { get; set; }
        private bool isClicked = false;
        //检查该图像是否被点击过
        public bool IsClick() {
            if (isClicked) {
                return true;
            }
            string sSQL = "select * from processedInfo where imgGUID=" + this.iImgKey;
            return null != SqliteHelper.GetSqlite(ComClassLib.DB.DbName.IndexDb.ToString()).ExecuteScalar(sSQL);
        }

        public void AddClickInfo(string user = "") {
            if (isClicked) return;
            try {
                string sSQL = string.Format("insert into processedInfo (imgGUID,clickUser) values ({0},'{1}')", this.iImgKey, user);
                SqliteHelper.GetSqlite(DbName.IndexDb.ToString()).ExecuteNonQuery(sSQL);
                isClicked = true;
            } catch (Exception ex) {

                MessageBox.Show("添加点击信息出错，请联系管理员");

            }

        }

    }
}
