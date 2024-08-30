using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RoadOfGroping.Common.Extensions
{
    public static class ObjectExtensions
    {

        /// <summary>
        /// 实体类转换，要求两个类中的成员一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="pModel"></param>
        /// <returns></returns>
        public static T ConvertModel<P, T>(in P pModel)
        {
            T ret = System.Activator.CreateInstance<T>();

            List<PropertyInfo> p_pis = pModel.GetType().GetProperties().ToList();
            PropertyInfo[] t_pis = typeof(T).GetProperties();

            foreach (PropertyInfo pi in t_pis)
            {
                //可写入数据
                if (pi.CanWrite)
                {
                    //忽略大小写
                    var name = p_pis.Find(s => s.Name.ToLower() == pi.Name.ToLower());
                    if (name != null && pi.PropertyType.Name == name.PropertyType.Name)
                    {
                        pi.SetValue(ret, name.GetValue(pModel, null), null);
                    }
                }
            }

            return ret;
        }

        public static T As<T>(this object obj) where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(this string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data);
        }


        public static bool ObjToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }

            return reval;
        }

        /// <summary>
        /// 把对象类型转换为指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null)
            {
                return null;
            }
            if (conversionType == typeof(string))
            {
                return value?.ToString();
            }
            else if (conversionType == typeof(int) || conversionType == typeof(decimal) || conversionType == typeof(float) || conversionType == typeof(double))
            {
                if ((value?.ToString() ?? "").Trim().Length > 0)
                {
                    if (double.TryParse(value.NotNullString().Trim(), out double result))
                        return Convert.ChangeType(result, conversionType);
                }
                return 0;
            }
            else if (conversionType.IsNullableType())
            {
                if (value.NotNullString().Length == 0)
                {
                    return null;
                }
                conversionType = conversionType.GetUnNullableType();
            }
            else if (conversionType.IsEnum)
            {
                return Enum.Parse(conversionType, value.ToString());
            }
            else if (conversionType == typeof(Guid))
            {
                return Guid.Parse(value.ToString());
            }
            return Convert.ChangeType(value.NotNullString().Trim(), conversionType);
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                if (value == null)
                {
                    return defaultValue ?? default;
                }
                else if (value.GetType() == typeof(string))
                {
                    return (T)value.CastTo(typeof(T));
                }
                else if (value.GetType() == typeof(T))
                {
                    return (T)value;
                }
                object result = value.CastTo(typeof(T));
                return (T)result;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static T CastTo<T>(this object value) => value.CastTo<T>(default);

    }
}
