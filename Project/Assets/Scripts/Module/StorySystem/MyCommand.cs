using System;
using System.Collections.Generic;
using UnityEngine;

namespace StorySystem
{

    /// <summary>
    /// 单个命令
    /// </summary>
    [Serializable]
    public class MyCommand
    {
        [DisplayName("命令类型",false,true,"设置CanvasGroup", "调用方法","对话","移动")]
        public CommandType commandType;
        [DisplayName("命令内容集合")]
        public List<CommandContent> contentList;
    }
    /// <summary>
    /// 命令内容
    /// </summary>
   [Serializable]
    public class CommandContent
    {
        [DisplayName("变量类型"),EnumMemberNames("bool","float","int","string","GameObject")]
        public VariableType varType;
        [DisplayName("内容值")]
        public string contentValue;

    }
    /// <summary>
    /// 添加的命令类型
    /// </summary>
    public enum CommandType
    {
        SetCanvasGroup,
        InvokeMethod,
        Say,
        Move
    }
    /// <summary>
    /// 变量类型
    /// </summary>
    public enum VariableType
    {
        Boolean,
        Float,
        Integer,
        String,
        GameObject,
    }
}
