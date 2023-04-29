// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    ///此组件为每个保存点保存的游戏对象列表编码和解码。
    ///它知道如何编码/解码具体的游戏类，如流程图和流程图数据。
    /// 要扩展存储系统以处理其他数据类型，只需修改或子类化此组件。
    /// </summary>
    public class SaveData : MonoBehaviour
    {
        protected const string FlowchartDataKey = "FlowchartData";

        protected const string NarrativeLogKey = "NarrativeLogData";

        [Tooltip("其变量将在保存数据中编码的流程图对象的列表。支持布尔、整数、浮点和字符串变量。")]
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        #region Public methods

        /// <summary>
        /// 将要保存为SaveDataItems列表的对象编码。
        /// </summary
        public virtual void Encode(List<SaveDataItem> saveDataItems)
        {
            for (int i = 0; i < flowcharts.Count; i++)
            {
                var flowchart = flowcharts[i];
                var flowchartData = FlowchartData.Encode(flowchart);

                var saveDataItem = SaveDataItem.Create(FlowchartDataKey, JsonUtility.ToJson(flowchartData));
                saveDataItems.Add(saveDataItem);

                var narrativeLogItem = SaveDataItem.Create(NarrativeLogKey, FungusManager.Instance.NarrativeLog.GetJsonHistory());
                saveDataItems.Add(narrativeLogItem);
            }
        }

        /// <summary>
        /// Decodes the loaded list of SaveDataItems to restore the saved game state.
        /// </summary>
        public virtual void Decode(List<SaveDataItem> saveDataItems)
        {
            for (int i = 0; i < saveDataItems.Count; i++)
            {
                var saveDataItem = saveDataItems[i];
                if (saveDataItem == null)
                {
                    continue;
                }

                if (saveDataItem.DataType == FlowchartDataKey)
                {
                    var flowchartData = JsonUtility.FromJson<FlowchartData>(saveDataItem.Data);
                    if (flowchartData == null)
                    {
                        Debug.LogError("Failed to decode Flowchart save data item");
                        return;
                    }

                    FlowchartData.Decode(flowchartData);
                }

                if (saveDataItem.DataType == NarrativeLogKey)
                {
                    FungusManager.Instance.NarrativeLog.LoadHistory(saveDataItem.Data);
                }
            }
        }

        #endregion
    }
}

#endif