namespace Components
{
    /// <summary>
    /// 设置格子的数据内容接口
    /// </summary>
    public interface ISetLoopItemData
    {
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="chileItem"></param>
        /// <param name="data"></param>
        void SetData(UnityEngine.GameObject chileItem, object data);
    }
}
