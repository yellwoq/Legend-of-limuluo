using Mono.Data.Sqlite;
using SaveSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
namespace MVC
{
    /// <summary>
    ///  处理角色相关数据
    /// </summary>
    public class HeroProxy : BaseProxy
    {
        /// <summary>
        ///  NAME
        /// </summary>
        public new const string NAME = "HeroProxy";
        /// <summary>
        ///  构造函数
        /// </summary>
        public HeroProxy()
        {
            this.ProxyName = NAME;
        }

        /// <summary>
        ///  获取用户进度数据
        /// </summary>
        public void GetUserHeroList()
        {
            // 打开数据库
            OpenDB();
            // 当前用户uid
            string uid = GameController.instance.crtUser.uid;
            if (uid == null) return;
            // 用uid到User_Hero查询UserID为uid的数据
            string selectStr = "Select * from User_Hero where UserID = @uid";
            SqliteParameter[] parms = new SqliteParameter[] {
                new SqliteParameter("@uid", DbType.String){ Value = uid.Trim() } };
            List<Dictionary<string, object>> result = db.ExecuteQuery(selectStr, parms);
            List<UserHeroVO> list = new List<UserHeroVO>();
            // 如果有数据
            if (result != null)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    // 英雄数据结构
                    UserHeroVO userHero = new UserHeroVO();
                    userHero.userId = uid;
                    userHero.heroId = result[i]["HeroID"].ToString();
                    userHero.heroName = result[i]["Name"].ToString();
                    userHero.heroType = result[i]["Type"].ToString();
                    userHero.lv = int.Parse(result[i]["Lv"].ToString());
                    userHero.currentExp = int.Parse(result[i]["CurrentExp"].ToString());
                    userHero.nextLvNeedExp = int.Parse(result[i]["NextLvNeedExp"].ToString());
                    userHero.money = int.Parse(result[i]["Money"].ToString());
                    userHero.force = int.Parse(result[i]["Force"].ToString());
                    userHero.spirit = int.Parse(result[i]["Spirit"].ToString());
                    userHero.intellect = int.Parse(result[i]["Intellect"].ToString());
                    userHero.speed = int.Parse(result[i]["Speed"].ToString());
                    userHero.DEM = int.Parse(result[i]["DEM"].ToString());
                    userHero.DEF = int.Parse(result[i]["DEF"].ToString());
                    userHero.currentHP = int.Parse(result[i]["CurrentHP"].ToString());
                    userHero.currentMP = int.Parse(result[i]["CurrentMP"].ToString());
                    userHero.maxHP = int.Parse(result[i]["MaxHP"].ToString());
                    userHero.maxMP = int.Parse(result[i]["MaxMP"].ToString());
                    userHero.fileName = result[i]["FileName"].ToString();
                    list.Add(userHero);
                }
                // 发送 获取用户英雄列表 成功
                SendNotification(NotiList.GET_USER_HERO_LIST + NotiList.SUCCESS, list);
            }
            #region 拼接法
            //reader = db.Select("User_Hero", "UserID", GetStr(uid));
            //// 如果有数据
            //if (reader.HasRows)
            //{
            //    // 遍历
            //    while (reader.Read())
            //    {
            //        // 英雄数据结构
            //        UserHeroVO userHero = new UserHeroVO();
            //        userHero.userId = uid;
            //        userHero.heroId = reader.GetString(reader.GetOrdinal("HeroID"));
            //        userHero.heroName = reader.GetValue(reader.GetOrdinal("Name")).ToString();
            //        userHero.heroType = reader.GetString(reader.GetOrdinal("Type"));
            //        userHero.lv = reader.GetInt32(reader.GetOrdinal("Lv"));
            //        userHero.nextLvNeedExp = reader.GetInt32(reader.GetOrdinal("Exp"));
            //        userHero.money = reader.GetInt32(reader.GetOrdinal("Money"));
            //        userHero.force = reader.GetInt32(reader.GetOrdinal("Force"));
            //        userHero.spirit = reader.GetInt32(reader.GetOrdinal("Spirit"));
            //        userHero.intellect = reader.GetInt32(reader.GetOrdinal("Intellect"));
            //        userHero.speed = reader.GetInt32(reader.GetOrdinal("Speed"));
            //        userHero.maxHP = reader.GetInt32(reader.GetOrdinal("MaxHP"));
            //        userHero.maxMP = reader.GetInt32(reader.GetOrdinal("MaxMP"));
            //        userHero.fileName = reader.GetString(reader.GetOrdinal("FileName"));
            //        list.Add(userHero);
            //        Debug.Log(userHero);
            //    }
            //}
            // 发送 获取用户英雄列表 成功
            //SendNotification(NotiList.GET_USER_HERO_LIST + NotiList.SUCCESS, list); 
            #endregion
            // 关闭数据库
            CloseDB();
        }
        /// <summary>
        ///  获取当前英雄
        /// </summary>
        public void GetCurrentHero()
        {
            // 打开数据库
            OpenDB();
            // 当前英雄heroId
            string heroID = string.Empty;
            List<Dictionary<string, object>> result;
            // 获取保存的heroID
            if (PlayerPrefs.HasKey(KeyList.CURRENT_HERO_ID))
                heroID = PlayerPrefs.GetString(KeyList.CURRENT_HERO_ID);
            if (heroID != string.Empty)
            {
                string selectStr = "Select * from User_Hero where HeroID = @heroID";
                SqliteParameter[] param = new SqliteParameter[] { new SqliteParameter("@heroID", DbType.String) { Value = heroID } };
                // 用heroID为条件到 User_Hero查询
                result = db.ExecuteQuery(selectStr, param);
                // 有结果
                if (result != null)
                {
                    // 读取英雄数据
                    ReadCurrentHero(result[0]);
                    // 关闭数据库
                    CloseDB();
                    return;
                }
            }
            // 没有结果
            // 如果没有heroID
            // 当前用户uid
            string uid = GameController.instance.crtUser.uid;
            // 用uid到User_Hero查询UserID为uid的数据
            string newSelectStr = "Select * from User_Hero where UserID = @userID";
            SqliteParameter[] newparam = new SqliteParameter[] { new SqliteParameter("@userID", DbType.String) { Value = uid } };
            // 用heroID为条件到 User_Hero查询
            result = db.ExecuteQuery(newSelectStr, newparam);
            // 如果有数据
            if (result != null)
            {
                // 读取第一个英雄数据
                ReadCurrentHero(result[0]);
            }
            // 如果没有数据
            else
            {
                SendNotification(NotiList.GET_CURRENT_HERO + NotiList.FAILURE, "该用户没有创建英雄");
            }
            // 关闭数据库
            CloseDB();
        }
        /// <summary>
        ///  读取当前英雄数据
        /// </summary>
        private void ReadCurrentHero(Dictionary<string, object> result)
        {
            UserHeroVO userHero = new UserHeroVO();
            userHero.userId = result["UserID"].ToString();
            userHero.heroId = result["HeroID"].ToString();
            userHero.heroName = result["Name"].ToString();
            userHero.heroType = result["Type"].ToString();
            userHero.lv = int.Parse(result["Lv"].ToString());
            userHero.currentExp = int.Parse(result["CurrentExp"].ToString());
            userHero.nextLvNeedExp = int.Parse(result["NextLvNeedExp"].ToString());
            userHero.money = int.Parse(result["Money"].ToString());
            userHero.force = int.Parse(result["Force"].ToString());
            userHero.spirit = int.Parse(result["Spirit"].ToString());
            userHero.intellect = int.Parse(result["Intellect"].ToString());
            userHero.speed = int.Parse(result["Speed"].ToString());
            userHero.DEM = int.Parse(result["DEM"].ToString());
            userHero.DEF = int.Parse(result["DEF"].ToString());
            userHero.currentHP = int.Parse(result["CurrentHP"].ToString());
            userHero.currentMP = int.Parse(result["CurrentMP"].ToString());
            userHero.maxHP = int.Parse(result["MaxHP"].ToString());
            userHero.maxMP = int.Parse(result["MaxMP"].ToString());
            userHero.fileName = result["FileName"].ToString();
            // 发送获取成功
            SendNotification(NotiList.GET_CURRENT_HERO + NotiList.SUCCESS, userHero);
        }

        /// <summary>
        /// 新建用户英雄
        /// </summary>
        /// <param name="userHeroVO">要建的英雄数据</param>
        public void CreateHero(UserHeroVO userHeroVO)
        {
            // 打开数据库
            OpenDB();
            // 插入数据
            string insertStr = string.Format("Insert into User_Hero values({0},{1},{2},{3},{4},{5},{6},{7}," +
                "{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18})", userHeroVO.GetParameterNames());
            int resultline = db.ExecuteNonQuery(insertStr, userHeroVO.GetParameters());
            if (resultline <= 0)
            {
                // 发送 创建失败 消息
                SendNotification(NotiList.CREATE_HERO + NotiList.FAILURE, "无法创建该英雄!");
                CloseDB(); return;
            }
            //保存玩家英雄数据
            HeroStateData pHSData = new HeroStateData(userHeroVO);
            pHSData.currentMainQuestTitle = "";
            pHSData.saveDate = DateTime.Now.ToString().Split('\'')[0];
            pHSData.positionX = -4.59f;
            pHSData.positionY = 2.45f;
            pHSData.positionZ = 0;
            pHSData.sceneName = "MainScene";
            GameController.I.crtHero = userHeroVO;
            //设置文件保存路径
            SaveManager.I.heroDataName = userHeroVO.fileName + ".json";
            SaveManager.I.exceptHeroDataName = userHeroVO.fileName + ".zdat";
            //保存数据
            SaveManager.I.SaveHeroData(pHSData);
            // 发送 创建成功 消息
            SendNotification(NotiList.CREATE_HERO + NotiList.SUCCESS);
            // 关闭数据库
            CloseDB();
        }
        /// <summary>
        /// 删除用户英雄数据
        /// </summary>
        /// <param name="herodata">要删除的数据</param>
        public void DeleteHero(string herodata, DeleteType deletetype = DeleteType.Normal)
        {
            Debug.Log("HeroProxy::DeleteHero " + herodata);
            string[] colValue = herodata.Split('.');
            // 打开数据库
            OpenDB();
            // 查询该英雄数据
            string squeryStr = "Select * from User_Hero where UserID = @uid and HeroID = @hid";
            SqliteParameter[] parameters = new SqliteParameter[]
            {
                new SqliteParameter("@uid",DbType.String){Value=colValue[0]},
                new SqliteParameter("@hid",DbType.String){Value=colValue[1]}
            };
            List<Dictionary<string, object>> selectResult = db.ExecuteQuery(squeryStr, parameters);
            // 有数据
            if (selectResult != null)
            {
                // 读取uid
                string userId = colValue[0];
                string fileName = selectResult[0]["FileName"].ToString();
                //删除记录
                string deleteStr = "Delete from User_Hero where UserID = @uid and HeroID = @hid";
                int resultLine = db.ExecuteNonQuery(deleteStr, parameters);
                if (resultLine > 0)
                {//删除该英雄相关的文件
                    if (File.Exists(SaveManager.I.HeroDataFilePath))
                        File.Delete(SaveManager.I.HeroDataFilePath);
                    if (File.Exists(SaveManager.I.ExceptHeroDataFilePath))
                        File.Delete(SaveManager.I.ExceptHeroDataFilePath);
                    Debug.LogFormat("{0}:/n{1}:", SaveManager.I.HeroDataFilePath, SaveManager.I.ExceptHeroDataFilePath);
                    //文件夹删除
                    string storePathDir = Application.persistentDataPath + "/" + GameController.I.crtUser.uid + "/" + GameController.I.crtHero.heroId;
                    Directory.Delete(storePathDir);
                    string userPathDir = Application.persistentDataPath + "/" + GameController.I.crtUser.uid;
                    if (Directory.GetDirectories(userPathDir).Length <= 0)
                        Directory.Delete(userPathDir);
                    if (deletetype == DeleteType.Auto) return;
                    // 发送删除成功消息
                    SendNotification(NotiList.DELETE + NotiList.USER_HERO_DATA + NotiList.SUCCESS);
                }
                else
                    // 发送登录失败消息
                    SendNotification(NotiList.DELETE + NotiList.USER_HERO_DATA + NotiList.FAILURE);
            }
            else
            {
                // 发送登录失败消息
                SendNotification(NotiList.DELETE + NotiList.USER_HERO_DATA + NotiList.FAILURE);
            }
            // 关闭数据库
            CloseDB();
        }
        /// <summary>
        /// 更新英雄数据
        /// </summary>
        /// <param name="herodata"></param>
        public void RenovateHero(UserHeroVO userHeroVO)
        {
            Debug.Log("HeroProxy::UpDateHero " + userHeroVO);
            // 打开数据库
            OpenDB();
            // 查询该英雄数据
            string squeryStr = "Select * from User_Hero where UserID = @uid and HeroID = @hid";
            SqliteParameter[] parameters = new SqliteParameter[]
            {
                new SqliteParameter("@uid",DbType.String){Value=userHeroVO.userId},
                new SqliteParameter("@hid",DbType.String){Value=userHeroVO.heroId}
            };
            List<Dictionary<string, object>> selectResult = db.ExecuteQuery(squeryStr, parameters);
            // 有数据, 删除成功
            if (selectResult != null)
            {
                string changeStr = "Update User_Hero set Lv = @Lv,CurrentExp=@CurrentExp,NextLvNeedExp = @nExp,Money=@money," +
                    " Force=@force,Spirit=@spirit,Intellect=@intellect,Speed=@speed,DEM=@DEM,DEF=@DEF," +
                    "CurrentHP=@CurrentHP,CurrentMP=@CurrentMP,MaxHP=@maxHp,MaxMP=@maxMp where UserID=@uid and HeroID = @hid";
                SqliteParameter[] newParam = new SqliteParameter[]
                {
                  new SqliteParameter("@uid",DbType.String){Value=userHeroVO.userId},
                  new SqliteParameter("@hid",DbType.String){Value=userHeroVO.heroId},
                  new SqliteParameter("@Lv",DbType.Int32){Value=userHeroVO.lv},
                  new SqliteParameter("@CurrentExp",DbType.Int32){Value=userHeroVO.currentExp},
                  new SqliteParameter("@nExp",DbType.Int32){Value=userHeroVO.nextLvNeedExp},
                  new SqliteParameter("@money",DbType.Int32){Value=userHeroVO.money},
                  new SqliteParameter("@force",DbType.Int32){Value=userHeroVO.force},
                  new SqliteParameter("@spirit",DbType.Int32){Value=userHeroVO.spirit},
                  new SqliteParameter("@intellect",DbType.Int32){Value=userHeroVO.intellect},
                  new SqliteParameter("@speed",DbType.Int32){Value=userHeroVO.speed},
                  new SqliteParameter("@DEM",DbType.Int32){Value=userHeroVO.DEM},
                  new SqliteParameter("@DEF",DbType.Int32){Value=userHeroVO.DEF},
                  new SqliteParameter("@CurrentHP",DbType.Int32){Value=userHeroVO.currentHP},
                  new SqliteParameter("@CurrentMP",DbType.Int32){Value=userHeroVO.currentMP},
                  new SqliteParameter("@maxHp",DbType.Int32){Value=userHeroVO.maxHP},
                  new SqliteParameter("@maxMp",DbType.Int32){Value=userHeroVO.maxMP}
                };
                if (db.ExecuteNonQuery(changeStr, newParam) > 0)
                {
                    // 发送更新成功消息
                    SendNotification(NotiList.CHANGE + NotiList.USER_HERO_DATA + NotiList.SUCCESS, userHeroVO);
                }
                else
                {
                    // 发送更新失败消息
                    SendNotification(NotiList.CHANGE + NotiList.USER_HERO_DATA + NotiList.FAILURE, "更新失败，请检查语法或数据库");
                }

            }
            else
            {
                // 发送更新失败消息
                SendNotification(NotiList.CHANGE + NotiList.USER_HERO_DATA + NotiList.FAILURE, "没有该英雄");
            }

            // 关闭数据库
            CloseDB();
        }
    }
}
//删除类型
public enum DeleteType
{
    Normal,
    Auto
}