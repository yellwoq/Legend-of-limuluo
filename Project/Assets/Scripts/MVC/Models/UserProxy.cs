using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
namespace MVC
{
    /// <summary>
    ///  处理用户相关数据:登录,注册和注销
    /// </summary>
    public class UserProxy : BaseProxy
    {
        /// <summary>
        /// NAME
        /// </summary>
        public new const string NAME = "UserProxy";

        public UserProxy()
        {
            this.ProxyName = NAME;
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user">要登陆的数据</param>
        public void Login(UserVO user)
        {
            Debug.Log("UserProxy::Login " + user.username);
            // 打开数据库
            OpenDB();
            // 用用户名和密码到数据库查询
            string searchStr = "Select uid from User where username = @username and password = @password";
            SqliteParameter[] parm = new SqliteParameter[] {
            new SqliteParameter("@username", DbType.String){ Value = user.username.Trim() },
            new SqliteParameter("@password", DbType.StringFixedLength){ Value = user.password.Trim() } };
            //查询结果集合
            List<Dictionary<string, object>> searchList = db.ExecuteQuery(searchStr, parm);
            // 有数据, 登录成功
            if (searchList != null)
            {
                Debug.Log("用户: " + user.username + " 登录成功!");
                for (int i = 0; i < searchList.Count; i++)
                {
                    foreach (var item in searchList[i].Keys)
                    {
                        user.uid = searchList[i]["uid"].ToString();
                        GameController.instance.crtUser = user;
                        if (PlayerPrefs.HasKey(KeyList.REMEMBER_PWD)
                            && PlayerPrefs.GetString(KeyList.REMEMBER_PWD) == "true")
                        {
                            PlayerPrefs.SetString(KeyList.USERNAME, user.username);
                            PlayerPrefs.SetString(KeyList.PWD, user.password);
                        }
                        // 发送登录成功消息
                        SendNotification(NotiList.LOGIN + NotiList.SUCCESS);
                    }
                }
            }
            #region 拼接字符串方法
            //reader = db.SelectWhere("User", // 表名
            //    new string[] { "uid" }, // 取回的字段
            //    new string[] { "username", "password" },// 查询的字段
            //    new string[] { "=", "=" }, // 操作符
            //    new string[] { user.username, user.password });
            //// 有数据, 登录成功
            //if (reader.HasRows)
            //{
            //    Debug.Log("用户: " + user.username + " 登录成功!");
            //    // uid
            //    reader.Read();
            //    // 读取uid
            //    string uid = reader.GetString(reader.GetOrdinal("uid"));
            //    user.uid = uid; // 设置uid
            //    GameController.instance.crtUser = user; // 保存当前登录用户
            //    // 记住密码
            //    if (PlayerPrefs.HasKey(KeyList.REMEMBER_PWD)
            //        && PlayerPrefs.GetString(KeyList.REMEMBER_PWD) == "true")
            //    {
            //        PlayerPrefs.SetString(KeyList.USERNAME, user.username);
            //        PlayerPrefs.SetString(KeyList.PWD, user.password);
            //    }
            //    // 发送登录成功消息
            //    SendNotification(NotiList.LOGIN + NotiList.SUCCESS);
            //}
            // 没有数据, 用户名或密码错误 
            #endregion
            else
            {
                // 发送登录失败消息
                SendNotification(NotiList.LOGIN + NotiList.FAILURE);
            }

            // 关闭数据库
            CloseDB();
        }
        /// <summary>
        ///  注册
        /// </summary>
        /// <param name="user"></param>
        public void Register(UserVO user)
        {
            // 打开数据库
            OpenDB();
            // 查找是否重名
            // 用用户名和密码到数据库查询
            string searchStr = " Select * from User where username = @username";
            SqliteParameter[] parm = new SqliteParameter[] {
            new SqliteParameter("@username", DbType.String){ Value = user.username.Trim() }};
            //查询结果集合
            List<Dictionary<string, object>> searchList = db.ExecuteQuery(searchStr, parm);
            // 如果有重名,发送注册失败的消息
            if (searchList != null)
            {
                Debug.LogError("用户名已存在!");
                SendNotification(NotiList.REGISTER + NotiList.FAILURE, "用户名已存在!");
                CloseDB(); return;
            }
            else
            {
                string insertStr = "Insert into User values(@uid,@username,@password,@dateTime)";
                
                SqliteParameter[] newparms = new SqliteParameter[] {
                new SqliteParameter("@uid", DbType.String){ Value = user.uid.Trim() },
                new SqliteParameter("@username", DbType.String){ Value = user.username.Trim() },
                new SqliteParameter("@password", DbType.StringFixedLength){ Value = user.password.Trim() },
                new SqliteParameter("@dateTime", DbType.DateTime){ Value =DateTime.Parse(DateTime.Now.ToString())}
                };
                int count = db.ExecuteNonQuery(insertStr, newparms);
                if (count > 0)
                    SendNotification(NotiList.REGISTER + NotiList.SUCCESS, user);
                else
                    SendNotification(NotiList.REGISTER + NotiList.FAILURE, "未能成功注册！！");
            }
            CloseDB();
            #region 拼装SQL语句方法
            //reader = db.Select("User", "username",
            //    GetStr(user.username));
            // 如果有重名,发送注册失败的消息
            //if (reader.HasRows)
            //{
            //    Debug.LogError("用户名已存在!");
            //    SendNotification(NotiList.REGISTER + NotiList.FAILURE, "用户名已存在!");
            //    CloseDB(); return;
            //}
            //// 插入数据
            //db.InsertInto("User", new string[]
            //{
            //    GetStr( user.uid ),
            //    GetStr( user.username ),
            //    GetStr( user.password ),
            //    GetStr( DateTime.Now )
            //});
            //// 发送 注册 成功消息
            //SendNotification(NotiList.REGISTER + NotiList.SUCCESS, user);
            //// 关闭数据库
            //CloseDB(); 
            #endregion
        }
        /// <summary>
        ///  注销
        /// </summary>
        public void Logout()
        {
            //清理服务器用户登录状态相关数据
            GameController.I.crtUser = null;
            GameController.I.crtHero = null;
            PlayerPrefs.DeleteKey(KeyList.CURRENT_HERO_ID);
            PlayerPrefs.DeleteKey(KeyList.USERNAME);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="user">要修改的数据</param>
        public void RenovateData(UserVO user)
        {
            Debug.Log("UserProxy::UpDate ");
            // 打开数据库
            OpenDB();
            string uColStr = user.username.Equals("") ? "password" : "username";
            string uValueStr = user.username.Equals("") ? user.password : user.username;
            Debug.Log(user.uid);
            string searchStr = " Select * from User where uid = @uid";
            SqliteParameter[] parm = new SqliteParameter[] {
            new SqliteParameter("@uid", DbType.String){ Value =user.uid.Trim() }};
            //查询结果集合
            List<Dictionary<string, object>> searchList = db.ExecuteQuery(searchStr, parm);
            // 如果有重名,发送更新失败的消息
            if (searchList != null)
            {
                foreach (var map in searchList)
                {
                    foreach (var result in map.Keys)
                    {
                        // 如果与之前数据一样
                        if (map[result].ToString().Equals(uValueStr))
                        {
                            SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE, "不能与之前数据一样！");
                            CloseDB();
                            return;
                        }
                    }
                }
                List<SqliteParameter> updateParams = new List<SqliteParameter>();
                updateParams.AddRange(parm);
                // 用要更新的数据到数据库更改
                string updateStr =string.Format("Update User set {0} = @updateValue where uid = @uid", uColStr);
                updateParams.Add(new SqliteParameter("@updateValue", DbType.String) { Value = uValueStr.Trim() });
                int count = db.ExecuteNonQuery(updateStr, updateParams.ToArray());
                Debug.Log(count);
                if (count > 0)
                {
                    string newSearchStr = "Select username,password from User where uid = @uid";
                    //查询结果集合
                    List<Dictionary<string, object>> resultList = db.ExecuteQuery(newSearchStr, parm);
                    if (resultList != null)
                    {
                        foreach (var keyMap in resultList)
                        {
                            user.username = keyMap["username"].ToString();
                            user.password = keyMap["password"].ToString();
                        }
                        GameController.instance.crtUser = user;
                        if (PlayerPrefs.HasKey(KeyList.REMEMBER_PWD)
                            && PlayerPrefs.GetString(KeyList.REMEMBER_PWD) == "true")
                        {
                            PlayerPrefs.SetString(KeyList.USERNAME, user.username);
                            PlayerPrefs.SetString(KeyList.PWD, user.password);
                        }
                        // 发送修改成功消息
                        SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.SUCCESS, "已经更改:" + (user.username.Equals("") ? "密码" : "用户名") + "为" + uValueStr);
                    }
                    else
                        // 发送修改失败消息
                        SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE, "更新失败，请检查数据库！");
                }
                else // 发送修改失败消息
                    SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE, "更新失败，请检查数据库！");
            }
            else
            {
                SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE, "不存在该用户，请检查数据库！");
            }
            #region 拼接方法
            ////与之前一样
            //reader = db.Select("User", "uid",
            //   GetStr(user.uid));
            //if (reader.HasRows)
            //{
            //    reader.Read();
            //    // 如果与之前数据一样
            //    if (reader.GetString(reader.GetOrdinal(uColStr)).Equals(uValueStr))
            //    {
            //        SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE, "不能与之前数据一样！");
            //        CloseDB();
            //        return;
            //    }
            //    // 用要更新的数据到数据库更改
            //    db.UpdateInto("User",
            //       new string[] { uColStr },
            //       new string[] { GetStr(uValueStr) },
            //        "uid",
            //        GetStr(user.uid));
            //    Debug.Log(" 修改成功!");
            //    //重新读表
            //    reader = db.Select("User", "uid", GetStr(user.uid));
            //    reader.Read();
            //    user.username = reader.GetString(reader.GetOrdinal("username"));
            //    user.password = reader.GetString(reader.GetOrdinal("password"));
            //    //更新储存在GameController的数据
            //    GameController.instance.crtUser = user;
            //    if (PlayerPrefs.HasKey(KeyList.REMEMBER_PWD)
            //        && PlayerPrefs.GetString(KeyList.REMEMBER_PWD) == "true")
            //    {
            //        PlayerPrefs.SetString(KeyList.USERNAME, user.username);
            //        PlayerPrefs.SetString(KeyList.PWD, user.password);
            //    }
            //    // 发送修改成功消息
            //    SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.SUCCESS, "已经更改:" + (user.username.Equals("") ? "密码" : "用户名") + "为" + uValueStr);
            //}
            //else
            //    // 发送修改失败消息
            //    SendNotification(NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE, "不存在该用户，请检查数据库！"); 
            #endregion
            // 关闭数据库
            CloseDB();
        }
    }
}
