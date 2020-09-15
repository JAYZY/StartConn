
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ComClassLib.FileOp {
    public static class JsonHelper {
        /// <summary>
        /// 序列化需要的字段
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="arrFiled"></param>
        /// <returns></returns>
        public static string GetPartModelJson(object obj, string[] arrFiled) {
            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.ContractResolver = new LimitPropsContractResolver(arrFiled);
            return JsonConvert.SerializeObject(obj, Formatting.Indented, jsetting);
        }

        /// <summary>
        /// 转换成json格式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string GetJson(object obj) {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// json格式字符串转化成T类型对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T GetModel<T>(string json) {
            
           
            //var tt = JsonConvert.DeserializeObject(json);
            //Console.WriteLine(tt.ToString());
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static List<T> GetListModel<T>(string json) {

            JObject jobj = JObject.Parse(json);
            List<T> obj2 = new List<T>();
            try {                
                var arrdata = JArray.Parse(jobj["seg"].ToString());
                foreach (var item in arrdata) {
                    T t1 = item.ToObject<T>();
                    obj2.Add(t1);
                }

                 //obj2 = arrdata.ToObject<List<T>>();
            }
            catch(Exception e)  {
                MessageBox.Show(e.ToString());
                return null;
            }
            return obj2;
        }

        /// <summary>
        /// 反序列化成json属性
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static Newtonsoft.Json.Linq.JProperty DeserialJson(string json) {
            return JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JProperty>(json);
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// 调用：var objClass = JsonClass.DeserializeAnonymousType(obj.Data.ToString(), nClass[匿名对象]);
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject) {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }

        public class LimitPropsContractResolver : DefaultContractResolver {
            string[] props = null;
            readonly bool retain;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="props">传入的属性数组</param>
            /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
            public LimitPropsContractResolver(string[] props, bool retain = true) {
                //指定要序列化属性的清单
                this.props = props;
                this.retain = retain;
            }

            protected override IList<JsonProperty> CreateProperties(Type type,

            MemberSerialization memberSerialization) {
                IList<JsonProperty> list =
                base.CreateProperties(type, memberSerialization);
                //只保留清单有列出的属性
                return list.Where(p => {
                    if (retain) {
                        return props.Contains(p.PropertyName);
                    }
                    else {
                        return !props.Contains(p.PropertyName);
                    }
                }).ToList();
            }
        }
    }
}
