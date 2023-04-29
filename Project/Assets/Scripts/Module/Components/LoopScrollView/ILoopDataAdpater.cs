using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    /// <summary>
    /// 管理滚动窗口数据接口
    /// </summary>
    public interface ILoopDataAdpater
    {
        // 初始化数据
        void InitData(object[] t);

        // 添加数据
        void AddData(object[] t);

        // 获取头部数据
        object GetHeaderData();

        // 移除头部
        bool RemoveHeaderData();

        // 获取尾部的数据
        object GetLastData();

        // 移除尾部的数据
        bool RemoveLastData();

    }
}
