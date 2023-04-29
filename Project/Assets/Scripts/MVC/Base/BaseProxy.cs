using Mono.Data.Sqlite;
using TarenaMVC;
using UnityEngine;

namespace MVC
{
    /// <summary>
    ///  封装访问SQLite数据库的功能
    /// </summary>
    public class BaseProxy : Proxy
    {
        /// <summary>
        /// NAME
        /// </summary>
        public new const string NAME = "BaseProxy";

        public BaseProxy()
        {
            this.ProxyName = NAME;
        }
        /// <summary>
        ///  数据库名称
        /// </summary>
        protected string dbName = "mydata.db";
        /// <summary>
        ///  数据库路径
        /// </summary>
        protected string FilePath
        {
            get
            {
                return Application.persistentDataPath + "/" + dbName;
            }
        }

        public string path;
        //public DbAccess db;
        public SqliteDbAssest db;

        protected SqliteDataReader reader;


        /// <summary>
        ///  打开数据库
        /// </summary>
        public void OpenDB()
        {
            Debug.Log("OpenDB: " + FilePath);
            //db = new DbAccess("URI=file:" + FilePath);
            db = new SqliteDbAssest(dbName, "URI=file:" + FilePath);
        }
        /// <summary>
        ///  关闭数据库
        /// </summary>
        public void CloseDB()
        {
            //db.CloseSqlConnection();
            //db = null;
            //reader = null;
            db.CloseSql();
        }
        /// <summary>
        ///  前后添加单引号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        protected string GetStr(object o)
        {
            return "'" + o + "'";
        }

    }
}
