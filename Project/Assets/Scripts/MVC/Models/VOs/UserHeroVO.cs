using Mono.Data.Sqlite;
using System.Data;
namespace MVC
{
    /// <summary>
    ///  用户英雄数据结构
    /// </summary>
    [System.Serializable]
    public class UserHeroVO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string userId;
        /// <summary>
        /// 英雄id,
        /// </summary>
        public string heroId;
        /// <summary>
        /// 角色类型,
        /// </summary>
        public string heroName;
        /// <summary>
        /// 角色类型,
        /// </summary>
        public string heroType;
        /// <summary>
        /// 角色等级
        /// </summary>
        public int lv = 1;
        /// <summary>
        /// 角色当前生命力
        /// </summary>
        public float currentExp = 0;
        /// <summary>
        /// 角色升级所需经验值
        /// </summary>
        public int nextLvNeedExp = 100;
        /// <summary>
        /// 角色所持金币
        /// </summary>
        public int money = 0;
        /// <summary>
        /// 角色力量
        /// </summary>
        public int force = 5;
        /// <summary>
        /// 角色体力
        /// </summary>
        public int spirit = 5;
        /// <summary>
        /// 角色智力
        /// </summary>
        public int intellect = 5;
        /// <summary>
        /// 角色移速
        /// </summary>
        public int speed = 5;
        /// <summary>
        /// 攻击力
        /// </summary>
        public float DEM = 8;
        /// <summary>
        /// 防御力
        /// </summary>
        public float DEF = 6.5f;
        /// <summary>
        /// 当前生命力
        /// </summary>
        public float currentHP = 500;
        /// <summary>
        /// 角色最大生命力
        /// </summary>
        public int maxHP = 500;
        /// <summary>
        /// 角色当前法力
        /// </summary>
        public float currentMP = 200;
        /// <summary>
        /// 角色最大法力
        /// </summary>
        public int maxMP = 200;
        /// <summary>
        /// 角色数据存储文件名
        /// </summary>
        public string fileName;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string tipMessage;
        public override string ToString()
        {
            return "英雄名称:" + heroName + "\n类型:" + heroType + "\n等级:" + lv + "\n力量：" + force + "\n体力：" + spirit + "\n智力" + intellect + "\n速度：" + speed +
                "\n当前经验：" + currentExp + "\n当前金币：" + money;
        }
        /// <summary>
        ///  获取所有参数
        /// </summary>
        /// <returns></returns>
        public SqliteParameter[] GetParameters()
        {
            return new SqliteParameter[]
            {
                 new SqliteParameter("@userID",DbType.String){Value=userId.ToString()},
                 new SqliteParameter("@heroID",DbType.String){Value=heroId.ToString()},
                 new SqliteParameter("@name",DbType.String){Value=heroName.ToString()},
                 new SqliteParameter("@type",DbType.String){Value=heroType.ToString()},
                 new SqliteParameter("@lv",DbType.Int32){Value=lv},
                 new SqliteParameter("@currentExp",DbType.Int32){Value=currentExp},
                 new SqliteParameter("@nExp",DbType.Int32){Value=nextLvNeedExp},
                 new SqliteParameter("@money",DbType.Int32){Value=money},
                 new SqliteParameter("@force",DbType.Int32){Value=force},
                 new SqliteParameter("@spirit",DbType.Int32){Value=spirit},
                 new SqliteParameter("@intellect",DbType.Int32){Value=intellect},
                 new SqliteParameter("@speed",DbType.Int32){Value=speed},
                 new SqliteParameter("@DEM",DbType.Int32){Value=DEM},
                 new SqliteParameter("@DEF",DbType.Int32){Value=DEF},
                 new SqliteParameter("@CurrentHP",DbType.Int32){Value=currentHP},
                 new SqliteParameter("@CurrentMP",DbType.Int32){Value=currentMP},
                 new SqliteParameter("@maxHp",DbType.Int32){Value=maxHP},
                 new SqliteParameter("@maxMp",DbType.Int32){Value=maxMP},
                 new SqliteParameter("@fileName",DbType.String){Value=fileName}
            };
        }
        /// <summary>
        /// 获取参数名
        /// </summary>
        /// <returns></returns>
        public string[] GetParameterNames()
        {
            return new string[]{
                "@userID",
                "@heroID",
                "@name",
                "@type",
                "@lv",
                "@currentExp",
                "@nExp",
                "@money",
                "@force",
                "@spirit",
                "@intellect",
                "@speed",
                "@DEM",
                "@DEF",
                "@CurrentHP",
                "@CurrentMP",
                "@maxHp",
                "@maxMp",
                "@fileName"
            };
        }
        public string[] GetString()
        {
            return new string[]
            {
                GetStr( this.userId),
                GetStr( this.heroId),
                GetStr( this.heroName),
                GetStr( this.heroType),
                GetStr( this.lv),
                GetStr( this.nextLvNeedExp),
                GetStr( this.money),
                GetStr( this.force),
                GetStr( this.spirit),
                GetStr( this.intellect),
                GetStr( this.speed),
                GetStr( this.maxHP),
                GetStr( this.maxMP),
                GetStr(this.fileName)
            };
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
