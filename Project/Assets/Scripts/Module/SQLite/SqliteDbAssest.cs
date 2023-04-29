using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 这个类是用来连接数据库的类
/// </summary>
public class SqliteDbAssest : Singleton<SqliteDbAssest>
{
    //连接类对象
    private static SqliteConnection mySqlConnection;
    //数据库名称
    private string databaseName;
    //连接字符串
    private string connectionStr;
    //连接信息提示
    public string conMessage;
    //是否连接成功
    public bool isSuccessed = false;
    public SqliteDbAssest() { }
    public SqliteDbAssest(string databaseName, string connectionString)
    {
        this.databaseName = databaseName;
        connectionStr = connectionString;
        OpenSql();
    }
    /// <summary>
    /// 打开数据库
    /// </summary>
    public void OpenSql()
    {
        try
        {
            mySqlConnection = new SqliteConnection(connectionStr);
            mySqlConnection.Open();
            if (mySqlConnection.State == ConnectionState.Open)
            {
                isSuccessed = true;
                conMessage = "数据库打开成功";
            }
        }
        catch (Exception e)
        {
            isSuccessed = false;
            conMessage = string.Format("数据库连接失败：{0}", e.Message.ToString());
            Debug.LogErrorFormat("服务器连接失败，请重新检查Sqlite服务是否打开。错误原因：{0}", e.Message.ToString());
        }
    }
    /// <summary>
    /// 关闭数据库
    /// </summary>
    public void CloseSql()
    {
        if (mySqlConnection != null)
        {
            mySqlConnection.Close();
            mySqlConnection.Dispose();
            mySqlConnection = null;

        }
    }

    /// <summary>
    /// 执行查询操作
    /// </summary>
    /// <param name="sql">要执行的SQL语句</param>
    /// <param name="pms">参数</param>
    /// <returns>查询的结果</returns>
    public List<Dictionary<string, object>> ExecuteQuery(string sql, params SqliteParameter[] pms)
    {

        // 实例化一个集合, 用来存储最终的结果
        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
        using (SqliteConnection con = new SqliteConnection(connectionStr))
        {
            //使用字符串来创建一个命令集
            using (SqliteCommand command = new SqliteCommand(sql, con))
            {
                con.Open();
                // 执行查询操作
                command.CommandText = sql;
                command.Parameters.AddRange(pms);
                try
                {
                    //使用阅读工具
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        //如果还没有读完
                        if (reader.HasRows)
                        { // 循环读取查询到的每一行的内容
                            while (reader.Read())
                            {
                                // 实例化一个Dictionary<string, object>，用来存储查询到每一行的键值对
                                Dictionary<string, object> item = new Dictionary<string, object>();
                                // 遍历查询到的一行的内容中所有的键
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    // 将查询到的键和值存储到item中
                                    item[reader.GetName(i)] = reader.GetValue(i).ToString();
                                }
                                // 在将这个存储了所有键值信息的Dictionary存储到result中
                                result.Add(item);
                            }
                        }
                        else
                        {
                            return null;
                        }

                    }

                }
                //查询失败
                catch (SqliteException e)
                {
                    Debug.LogWarning("查询异常！" + e.ToString());
                }
                return result;
            }
        }
    }

    #region 修改操作
    /// <summary>
    /// 增删改
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="pms">参数集合</param>
    /// <returns></returns>
    public int ExecuteNonQuery(string sql, params SqliteParameter[] pms)
    {
        using (SqliteConnection con = new SqliteConnection(connectionStr))
        {
            using (SqliteCommand cmd = new SqliteCommand(sql, con))
            {
                if (pms != null)
                {
                    cmd.Parameters.AddRange(pms);
                }
                con.Open();
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch
                {
                    return -1;
                }
            }
        }
    }
    /// <summary>
    /// 拼接查询语句，查询数据
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="items">要查询的列</param>
    /// <param name="whereColumnName">查询的条件列</param>
    /// <param name="operation">条件操作符</param>
    /// <param name="value">条件值</param>
    /// <returns></returns>
    public DataSet Select(string tableName, string[] items, string[] whereColumnName,
        string[] operation, string[] value)
    {
        if (whereColumnName.Length != operation.Length || operation.Length != value.Length)
        {
            throw new Exception("输入不正确:" + "要查询的条件，条件操作符、条件值的数量不一致！");
        }
        string query = "Select " + items[0];
        for (int i = 1; i < items.Length; i++)
        {
            query += "," + items[i];
        }
        query += " from " + tableName + " where " + whereColumnName[0] + " " + operation[0] + " '" + value[0] + "'";
        for (int i = 1; i < whereColumnName.Length; i++)
        {
            query += " and " + whereColumnName[i] + " " + operation[i] + " '" + value[i] + "'";
        }
        return QuerySet(query);
    }
    /// <summary>
    /// 插入数据
    /// </summary>
    /// <param name="tableName">要插入的表名</param>
    /// <param name="items">要插入的列</param>
    /// <param name="value">插入的值</param>
    /// <returns></returns>
    public DataSet Insert(string tableName, string[] items, string[] value)
    {
        if (items.Length != value.Length)
        {
            throw new Exception("输入不正确:" + "要插入的列与值的列的数量不一致！");
        }
        string query = "Insert into " + tableName + "(" + items[0];
        for (int i = 1; i < items.Length - 1; i++)
        {
            query += "," + items[i];
        }
        query += "," + items[items.Length - 1] + ")" + " Value(" + "'" + value[0] + "'";
        for (int i = 1; i < value.Length - 1; i++)
        {
            query += "," + "'" + value[i] + "'";
        }
        query += "," + "'" + value[value.Length - 1] + "')";
        return QuerySet(query);
    }
    #endregion
    /// <summary>
    /// 执行语句
    /// </summary>
    /// <param name="sqlString">sql语句</param>
    /// <returns></returns>
    private DataSet QuerySet(string sqlString)
    {
        if (mySqlConnection.State == ConnectionState.Open)
        {
            DataSet ds = new DataSet();
            try
            {
                SqliteDataAdapter mySqlAdapter = new SqliteDataAdapter(sqlString, mySqlConnection);
                mySqlAdapter.Fill(ds);
            }
            catch (Exception e)
            {
                throw new Exception("SQL:" + sqlString + "/n" + e.Message.ToString());
            }
            return ds;
        }
        return null;
    }
}
