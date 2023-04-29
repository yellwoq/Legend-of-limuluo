using DialogueSystem;
using Player;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS0649
namespace StorySystem
{
    /// <summary>
    /// 单个故事类
    /// </summary>
    [CreateAssetMenu(fileName = "Story", menuName = "RPG GAME/Story/new Story")]
    public class Story : ScriptableObject
    {
        [DisplayName("所有的情节集合"), SerializeField]
        private List<Plot> plots = new List<Plot>();
        /// <summary>
        /// 所有的情节集合
        /// </summary>
        public List<Plot> Plots
        {
            get
            {
                return plots;
            }
        }
        /// <summary>
        /// 是否已经经过该段剧情
        /// </summary>
        public bool IsHaveRead { get; set; }
        [SerializeField]
        private List<Variable> variables;
        /// <summary>
        /// 变量集合
        /// </summary>
        public List<Variable> Variables
        {
            get
            {
                return variables;
            }
        }
    }
    /// <summary>
    /// 单个变量
    /// </summary>
    [System.Serializable]
    public class Variable
    {
        [DisplayName("变量名"), SerializeField]
        private string variableName;
        /// <summary>
        /// 变量名
        /// </summary>
        public string VariableName
        {
            get
            {
                return variableName;
            }
        }
        [DisplayName("变量类型"), SerializeField]
        private VariableType variableType;
        /// <summary>
        /// 变量类型
        /// </summary>
        public VariableType MVariableType
        {
            get
            {
                return variableType;
            }
        }
    }
    /// <summary>
    /// 单个情节节点
    /// </summary>
    [System.Serializable]
    public class Plot
    {
        [DisplayName("情节ID"), SerializeField]
        private string plot_ID = string.Empty;
        /// <summary>
        /// 标志
        /// </summary>
        public string PlotID
        {
            get
            {
                return plot_ID;
            }
        }

        [DisplayName("所有的情节的动作命令"), SerializeField]
        private List<PlotActionCommand> actions = new List<PlotActionCommand>();
        /// <summary>
        /// 所有的情节的动作命令
        /// </summary>
        public List<PlotActionCommand> Actions
        {
            get
            {
                return actions;
            }
        }
    }
    /// <summary>
    /// 单个动作命令
    /// </summary>
    [System.Serializable]
    public class PlotActionCommand
    {
        [SerializeField]
#if UNITY_EDITOR
        [EnumMemberNames("空", "对话", "角色移动", "角色动画", "稍等", "相机移动", "相机抖动", "画面缩放", "画面闪烁")]
#endif
        private PlotActionType actionType;
        public PlotActionType ActionType
        {
            get
            {
                return actionType;
            }
        }

        [SerializeField]
        private Dialogue dialogue;
        public Dialogue Dialogue
        {
            get
            {
                return dialogue;
            }
        }

        [SerializeField]
        private bool forPlayer;
        public bool ForPlayer
        {
            get
            {
                return forPlayer;
            }
        }

        [SerializeField]
#if UNITY_EDITOR
        [EnumMemberNames("SetInt", "SetBool", "SetFloat", "SetTrigger", "播放动画片段")]
#endif
        private PlotAnimationType animaActionType;
        public PlotAnimationType AnimaActionType
        {
            get
            {
                return animaActionType;
            }
        }

        [SerializeField]
        private PlayerStatus character;
        public PlayerStatus Character
        {
            get
            {
                return character;
            }
        }

        [SerializeField]
        private string paramName = string.Empty;
        public string ParamName
        {
            get
            {
                return paramName;
            }
        }

        [SerializeField]
        private int intValue;
        public int IntValue
        {
            get
            {
                return intValue;
            }
        }

        [SerializeField]
        private bool boolValue;
        public bool BoolValue
        {
            get
            {
                return boolValue;
            }
        }

        [SerializeField]
        private float floatValue;
        public float FloatValue
        {
            get
            {
                return floatValue;
            }
        }

        [SerializeField]
        private AnimationClip animaClip;
        public AnimationClip AnimaClip
        {
            get
            {
                return animaClip;
            }
        }


        [SerializeField]
#if UNITY_EDITOR
        [EnumMemberNames("向上", "向下", "向左", "向右", "向左上", "向左下", "向右上", "向右下")]
#endif
        private PlotTransferDirection2D direction;
        public PlotTransferDirection2D Direction
        {
            get
            {
                return direction;
            }
        }

        [SerializeField]
        private float distance;
        public float Distance
        {
            get
            {
                return distance;
            }
        }

        [SerializeField]
        private float duration;
        public float Duration
        {
            get
            {
                return duration;
            }
        }

        [SerializeField]
        private float zoomMultiple;
        public float ZoomMultiple
        {
            get
            {
                return zoomMultiple;
            }
        }

        [SerializeField]
        private int extent = 1;
        public int Extent
        {
            get
            {
                return extent;
            }
        }

        [SerializeField]
        private int frequency = 1;
        public int Frequency
        {
            get
            {
                return frequency;
            }
        }
    }
 
    /// <summary>
    /// 动作类型
    /// </summary>
    public enum PlotActionType
    {
        Commend,
        Animation,
        Audio,
        Camera,
        Flow,
        GameObject,
        iTween,
        Dialogue,
        TransferCharacter,
        WaitForSecond,
        TransferCamera,
        ShakeCamera,
        ZoomScreen,
        FlashScreen
    }

    //public enum PlotTransferDirection
    //{
    //    Forward,
    //    Backward,
    //    Left,
    //    Right,
    //    Up,
    //    Down
    //}
    public enum PlotTransferDirection2D
    {
        Up,//往上
        Down,//往下
        Left,//往左
        Right,//往右
        TopLeft,//往左上
        BottomLeft,//往左下
        TopRight,//往右上
        BottomRight//往右下
    }

    public enum PlotAnimationType
    {
        SetInt,
        SetBool,
        SetFloat,
        SetTrigger,
        PlayClip
    }
}