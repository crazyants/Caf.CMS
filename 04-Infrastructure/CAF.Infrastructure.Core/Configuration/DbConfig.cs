using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CAF.Infrastructure.Core
{
    public class DbConfigHelper
    {
        private static DbConfig _config;
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="filePath">绝对路径</param>
        /// <returns></returns>
        public static DbConfig LoadFile(string filePath)
        {
            if (_config == null)
            {
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                XmlSerializer xmlSearializer = new XmlSerializer(typeof(DbConfig));
                _config = (DbConfig)xmlSearializer.Deserialize(file);
            }

            return _config;


        }

        public static void SaveXml(DbConfig config, string filePath)
        {
            //创建XML命名空间
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            System.IO.FileStream stream = new FileStream(filePath, FileMode.Create);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DbConfig));
                serializer.Serialize(stream, config, ns);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SerilizeAnObject Exception: {0}", ex.Message);
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }

        }
    }
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbConfig
    {
        /// <summary>
        /// 必须有默认的构造函数
        /// </summary>
        public DbConfig()
        { }
        /// <summary>
        /// 解决方案名称
        /// </summary>
        public string SolutionName
        {
            get;
            set;
        }
        /// <summary>
        /// EF CodeFirst 配置文件命名空间
        /// </summary>
        public string MappingNamespace
        {
            get;
            set;
        }
    }
}
