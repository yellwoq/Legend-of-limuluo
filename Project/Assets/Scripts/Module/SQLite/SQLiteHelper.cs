using Mono.Data.Sqlite;
using UnityEngine;

namespace SQLite
{
	/// <summary>
	///  封装了SQLite数据库访问功能
	/// </summary>
	public class SQLiteHelper : MonoBehaviour 
	{
        /// <summary>
        /// 文件名
        /// </summary>
        protected string fileName = "myData.db";
        protected string FilePath
        {
            get { return Application.persistentDataPath + "/" + fileName; }
        }
        protected DbAccess db;
        protected SqliteDataReader reader;
        /// <summary>
        ///  打开数据库
        /// </summary>
        protected void OpenDB()
        {
            Debug.Log( "FilePath: " + FilePath );
            db = new DbAccess( "URI=file:" + FilePath );
        }
        /// <summary>
        ///  关闭数据库
        /// </summary>
        protected void CloseDB()
        {
            db.CloseSqlConnection();
            db = null;
            reader = null;
        }

        /// <summary>
        ///  前后添加单引号
        /// </summary>
        /// <param name="o">要添加引号的数据</param>
        /// <returns></returns>
        protected string GetStr(object o)
        {
            return "'" + o + "'";
        }

    }
}
