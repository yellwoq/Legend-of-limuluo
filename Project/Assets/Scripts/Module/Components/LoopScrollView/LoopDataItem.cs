using MVC;

namespace Components
{
    /// <summary>
    /// 格子的数据结构
    /// </summary>
    [System.Serializable]
    public class LoopDataItem
    {
        public int id;//格子编号
        public string heroID;//英雄id
        public string heroName;//英雄名称
        public string heroType;//英雄类别
        public int herolv;//英雄等级
        public string currentMainQuestTitle;//当前任务信息
        public string saveTime;//存档时间
        public UserHeroVO currentHeroData;//当前英雄数据

        public LoopDataItem(int id,string heroID,string heroName, string heroType, int herolv, string currentMainQuestTitle, string saveTime)
        {
            this.id = id;
            this.heroID = heroID;
            this.heroName = heroName;
            this.heroType = heroType;
            this.herolv = herolv;
            this.currentMainQuestTitle = currentMainQuestTitle;
            this.saveTime = saveTime;
        }

        public LoopDataItem(int id)
        {
            this.id = id;
        }

    }
}
