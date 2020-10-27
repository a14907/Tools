using System.Collections.Generic;
using System.Management;

namespace WmiHelper
{
    public class Wmi
    {
        public static List<Dictionary<string, object>> QueryDictionary(string query)
        {
            var wql = new ObjectQuery(query);
            var searcher = new ManagementObjectSearcher(wql);


            var values = searcher.Get();
            var res = new List<Dictionary<string, object>>();

            foreach (var value in values)
            {
                var model = new Dictionary<string, object>();
                foreach (var valueProperty in value.Properties)
                {
                    model[valueProperty.Name] = valueProperty.Value;
                }
                res.Add(model);
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">示例：
        /// //cpu序列号
        /// var r = Wmi.QueryDictionary("SELECT * FROM Win32_Processor")[0]["ProcessorId"];
        /// //获取网卡硬件地址 
        /// var r2 = Wmi.QueryDictionary("SELECT * FROM Win32_NetworkAdapterConfiguration");
        /// r2 = r2.Where(m => m["IPEnabled"] is true).ToList();
        /// //获取硬盘ID
        /// var r3 = Wmi.QueryDictionary("SELECT * FROM Win32_DiskDrive");
        /// // PC类型 
        /// var r4 = Wmi.QueryDictionary("SELECT * FROM Win32_ComputerSystem");
        /// </param>
        /// <returns></returns>
        public static List<T> Query<T>(string query) where T : new()
        {
            var wql = new ObjectQuery(query);
            var searcher = new ManagementObjectSearcher(wql);

            var properties = typeof(T).GetProperties();

            var values = searcher.Get();
            var res = new List<T>();

            foreach (var value in values)
            {
                var model = new T();
                foreach (var valueProperty in value.Properties)
                {
                    foreach (var property in properties)
                    {
                        if (valueProperty.Name == property.Name)
                        {
                            model.GetType().GetProperty(property.Name)?.SetValue(model, valueProperty.Value, null);
                            break;
                        }
                    }
                }
                res.Add(model);
            }


            return res;
        }
    }
}
