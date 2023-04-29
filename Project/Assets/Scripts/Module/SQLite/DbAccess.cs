using Mono.Data.Sqlite;
using System;
using UnityEngine;

/// <summary>
/// SQLite数据库操作类
/// </summary>
public class DbAccess
{
    /// <summary>
    /// SQLite连接
    /// </summary>
	private SqliteConnection conn;
    /// <summary>
    /// SQLite命令
    /// </summary>
	private SqliteCommand cmd;
    /// <summary>
    /// SQLite读取器
    /// </summary>
	private SqliteDataReader reader;

    public string connectionString;
    public DbAccess(string connectionString)
    {
        this.connectionString = connectionString;
        OpenDB(connectionString);
    }
    public DbAccess() { }
    /// <summary>
    /// 打开数据库
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public void OpenDB(string connectionString)
    {
        try
        {
            object lockThis = new object();

            lock (lockThis)
            {
                conn = new SqliteConnection(connectionString);
                conn.Open();
                Debug.Log("Connected to db,连接数据库成功！");
            }
        }
        catch (Exception e)
        {
            string temp1 = e.ToString();
            Debug.Log(temp1);
        }
    }
    /// <summary>
    /// 关闭数据库连接
    /// </summary>
	public void CloseSqlConnection()
    {
        if (cmd != null) { cmd.Dispose(); cmd = null; }
        if (reader != null) { reader.Dispose(); reader = null; }
        if (conn != null) { conn.Close(); conn = null; }
        Debug.Log("Disconnected from db.关闭数据库！");
    }
    /// <summary>
    /// 执行SQL语句
    /// </summary>
    /// <param name="sqlQuery">SQL语句</param>
    /// <returns>读取器</returns>
    public int ExecuteQuery(string sqlQuery)
    {
        {
            int affectline;
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                using (SqliteCommand cmd = new SqliteCommand(sqlQuery, conn))
                {
                    affectline = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return affectline;
        }
    }

    public SqliteDataReader ExecuteReaderQuery(string sqlQuery)
    {
        {
            Debug.Log("ExecuteQuery:: " + sqlQuery);
            cmd = conn.CreateCommand();
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            return reader;
        }
    }

    #region 更新操作
    /// <summary>
    /// 插入数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="values">插入数据内容</param>
    /// <returns></returns>
    public int InsertInto(string tableName, string[] values)
    {
        string query = "INSERT INTO " + tableName + " VALUES (" + values[0];
        for (int i = 1; i < values.Length; ++i)
        {
            query += ", " + values[i];
        }
        query += ")";
        return ExecuteQuery(query);
    }
    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cols">更新字段</param>
    /// <param name="colsvalues">更新内容</param>
    /// <param name="selectkey">查找字段（主键)</param>
    /// <param name="selectvalue">查找内容</param>
    /// <returns></returns>
    public int UpdateInto(string tableName, string[] cols, string[] colsvalues, string selectkey, string selectvalue)
    {
        string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + colsvalues[0];
        for (int i = 1; i < colsvalues.Length; ++i)
        {
            query += ", " + cols[i] + " =" + colsvalues[i];
        }
        query += " WHERE " + selectkey + " = " + selectvalue + " ";
        return ExecuteQuery(query);
    }
    public int UpdateInto(string tableName, string[] cols, string[] colsvalues, string[] selectkeys, string[] operations, string[] selectvalues)
    {
        string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + colsvalues[0];
        for (int i = 1; i < colsvalues.Length; ++i)
        {
            query += ", " + cols[i] + " =" + colsvalues[i];
        }
        query += " WHERE " + selectkeys[0] + operations[0] + "'" + selectvalues[0] + "' ";
        for (int i = 1; i < selectkeys.Length; ++i)
        {
            query += " AND " + selectkeys[i] + operations[i] + "'" + selectvalues[i] + "' ";
        }
        return ExecuteQuery(query);
    }
    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cols">字段</param>
    /// <param name="colsvalues">内容</param>
    /// <returns></returns>
    public int Delete(string tableName, string[] cols, string[] colsvalues)
    {
        string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + "'" + colsvalues[0] + "'";
        for (int i = 1; i < colsvalues.Length; ++i)
        {
            query += " and " + cols[i] + " = " + "'" + colsvalues[i] + "'";
        }
        return ExecuteQuery(query);
    }
    /// <summary>
    /// 指定列插入数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cols">插入字段</param>
    /// <param name="values">插入内容</param>
    /// <returns></returns>
    public int InsertInto(string tableName, string[] cols, string[] values)
    {
        if (cols.Length != values.Length)
        {
            throw new SqliteException("columns.Length != values.Length");
        }
        string query = "INSERT INTO " + tableName + "(" + cols[0];
        for (int i = 1; i < cols.Length; ++i)
        {
            query += ", " + cols[i];
        }
        query += ") VALUES (" + values[0];
        for (int i = 1; i < values.Length; ++i)
        {
            query += ", " + values[i];
        }
        query += ")";
        return ExecuteQuery(query);
    }
    /// <summary>
    /// 删除表中全部数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <returns></returns>
    public int DeleteContents(string tableName)
    {
        string query = "DELETE FROM " + tableName;
        return ExecuteQuery(query);
    }
    /// <summary>
    /// 创建表
    /// </summary>
    /// <param name="name">表名</param>
    /// <param name="col">字段名</param>
    /// <param name="colType">字段类型</param>
    /// <returns></returns>
    public int CreateTable(string name, string[] col, string[] colType)
    {
        if (col.Length != colType.Length)
        {
            throw new SqliteException("columns.Length != colType.Length");
        }
        string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];
        for (int i = 1; i < col.Length; ++i)
        {
            query += ", " + col[i] + " " + colType[i];
        }
        query += ")";
        return ExecuteQuery(query);
    } 
    #endregion

    #region 查询操作
    /// <summary>
    /// 查询表中全部数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <returns></returns>
    public SqliteDataReader ReadFullTable(string tableName)
    {
        string query = "SELECT * FROM " + tableName;
        return ExecuteReaderQuery(query);
    }
    /// <summary>
    /// 按条件查询数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="items">查询字段</param>
    /// <param name="col">查找字段</param>
    /// <param name="operation">运算符</param>
    /// <param name="values">内容</param>
    /// <returns></returns>
    public SqliteDataReader SelectWhere(string tableName, string[] items, string[] col, string[] operation, string[] values)
    {
        if (col.Length != operation.Length || operation.Length != values.Length)
        {
            throw new SqliteException("col.Length != operation.Length != values.Length");
        }
        string query = "SELECT " + items[0];
        for (int i = 1; i < items.Length; ++i)
        {
            query += ", " + items[i];
        }
        query += " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";
        for (int i = 1; i < col.Length; ++i)
        {
            query += " AND " + col[i] + operation[i] + "'" + values[i] + "' ";
        }
        return ExecuteReaderQuery(query);
    }
    /// <summary>
    /// 查询表
    /// </summary>
    public SqliteDataReader Select(string tableName, string col, string values)
    {
        string query = "SELECT * FROM " + tableName + " WHERE " + col + " = " + values;
        return ExecuteReaderQuery(query);
    }
    /// <summary>
    /// 查询指定操作的表
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="col">查询字段</param>
    /// <param name="operation">查询操作符</param>
    /// <param name="values">查询值</param>
    /// <returns></returns>
    public SqliteDataReader Select(string tableName, string col, string operation, string values)
    {
        string query = "SELECT * FROM " + tableName + " WHERE " + col + operation + values;
        return ExecuteReaderQuery(query);
    } 
    #endregion

    #region 函数查询
    /// <summary>
    /// 升序查询
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="col">升序依据</param>
    /// <returns></returns>
    public SqliteDataReader SelectOrderASC(string tableName, string col)
    {
        string query = "SELECT * FROM " + tableName + " ORDER BY " + col + " ASC";
        return ExecuteReaderQuery(query);
    }
    /// <summary>
    /// 降序查询
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="col">降序依据</param>
    /// <returns></returns>
    public SqliteDataReader SelectOrderDESC(string tableName, string col)
    {
        string query = "SELECT * FROM " + tableName + " ORDER BY " + col + " DESC";
        return ExecuteReaderQuery(query);
    }
    /// <summary>
    /// 查询表行数
    /// </summary>
    public SqliteDataReader SelectCount(string tableName)
    {
        string query = "SELECT COUNT(*) FROM " + tableName;
        return ExecuteReaderQuery(query);
    } 
    #endregion
}