using Bag;
using Common;
using DialogueSystem;
using Enemy;
using MVC;
using SaveSystem;
using StorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace QuestSystem
{
    [CustomEditor(typeof(Quest))]
    public class QuestInspector : ScriptObjectIconEditor
    {
        Quest quest;
        SerializedProperty defaultSprite;
        SerializedProperty ID;
        SerializedProperty isFirstAcceted;
        SerializedProperty title;
        SerializedProperty description;
        SerializedProperty abandonable;
        SerializedProperty acceptConditions;
        SerializedProperty beginDialogue;
        SerializedProperty ongoingDialogue;
        SerializedProperty completeDialogue;
        SerializedProperty questReward;
        SerializedProperty cmpltOnOriginalNPC;
        SerializedProperty IDOfNPCToComplete;
        SerializedProperty cmpltObjectiveInOrder;
        SerializedProperty collectObjectives;
        SerializedProperty killObjectives;
        SerializedProperty talkObjectives;
        SerializedProperty moveObjectives;

        ReorderableList rewardItemList;
        ReorderableList acceptConditionsList;
        ReorderableList collectObjectiveList;
        ReorderableList killObjectiveList;
        ReorderableList talkObjectiveList;
        ReorderableList moveObjectiveList;

        float lineHeight;
        float lineHeightSpace;

        int barIndex;

        Quest[] Quests => ResourceManager.LoadAll<Quest>().ToArray();
        List<StoryAgent> allStorys => StoryManager.I.StoryList;
        TalkerInformation[] npcs => ResourceManager.LoadAll<TalkerInformation>().ToArray();

        List<EnemyInformation> allEnemys => ResourceManager.LoadAll<EnemyInformation>();
        string[] npcNames;

        private void OnEnable()
        {
            quest = target as Quest;
            npcNames = npcs.Select(x => x.Name).ToArray();//Linq分离出NPC名字
            lineHeight = EditorGUIUtility.singleLineHeight;
            lineHeightSpace = lineHeight + 5;
            defaultSprite = serializedObject.FindProperty("defaultSprite");
            ID = serializedObject.FindProperty("ID");
            isFirstAcceted = serializedObject.FindProperty("isFirstAcceted");
            title = serializedObject.FindProperty("title");
            description = serializedObject.FindProperty("description");
            abandonable = serializedObject.FindProperty("abandonable");
            acceptConditions = serializedObject.FindProperty("acceptConditions");
            questReward = serializedObject.FindProperty("questReward");
            beginDialogue = serializedObject.FindProperty("beginDialogue");
            ongoingDialogue = serializedObject.FindProperty("ongoingDialogue");
            completeDialogue = serializedObject.FindProperty("completeDialogue");
            cmpltOnOriginalNPC = serializedObject.FindProperty("cmpltOnOriginalNPC");
            IDOfNPCToComplete = serializedObject.FindProperty("IDOfNPCToComplete");
            cmpltObjectiveInOrder = serializedObject.FindProperty("cmpltObjectiveInOrder");
            collectObjectives = serializedObject.FindProperty("collectObjectives");
            killObjectives = serializedObject.FindProperty("killObjectives");
            talkObjectives = serializedObject.FindProperty("talkObjectives");
            moveObjectives = serializedObject.FindProperty("moveObjectives");
            HandlingQuestConditionList();
            HandlingQuestRewardItemList();
            HandlingCollectObjectiveList();
            HandlingKillObjectiveList();
            HandlingTalkObjectiveList();
            HandlingMoveObjectiveList();
        }
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            if (quest.DefaultSprite != null)
            {
                Type t = GetType("UnityEditor.SpriteUtility");
                if (t != null)
                {
                    MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                    if (method != null)
                    {
                        object ret = method.Invoke("RenderStaticPreview", new object[] { quest.DefaultSprite, Color.white, width, height });
                        if (ret is Texture2D)
                            return ret as Texture2D;
                    }
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
        public override void OnInspectorGUI()
        {
            if (!CheckEditComplete())
                EditorGUILayout.HelpBox("该任务存在未补全信息。", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("该任务信息已完整。", MessageType.Info);
            barIndex = GUILayout.Toolbar(barIndex, new string[] { "基本", "条件", "奖励", "对话", "目标" });
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            switch (barIndex)
            {
                case 0:
                    #region case 0 基本
                    defaultSprite.objectReferenceValue = EditorGUILayout.ObjectField("任务默认图标", defaultSprite.objectReferenceValue as Sprite, typeof(Sprite), false);
                    EditorGUILayout.PropertyField(ID, new GUIContent("识别码"));
                    if (string.IsNullOrEmpty(ID.stringValue) || ExistsID())
                    {
                        if (!string.IsNullOrEmpty(ID.stringValue) && ExistsID())
                            EditorGUILayout.HelpBox("此识别码已存在！", MessageType.Error);
                        else
                            EditorGUILayout.HelpBox("识别码为空！", MessageType.Error);
                        if (GUILayout.Button("自动生成识别码"))
                        {
                            ID.stringValue = GetAutoID();
                            EditorGUI.FocusTextInControl(null);
                        }
                    }
                    EditorGUILayout.PropertyField(title, new GUIContent("标题"));
                    EditorGUILayout.PropertyField(description, new GUIContent("描述"));
                    EditorGUILayout.PropertyField(abandonable, new GUIContent("可放弃"));
                    EditorGUILayout.PropertyField(cmpltOnOriginalNPC, new GUIContent("是否在原NPC上交付"));
                    EditorGUILayout.Space();
                    if (npcs != null && !cmpltOnOriginalNPC.boolValue)
                    {
                        TalkerInformation targetTalker = null;
                        if (!string.IsNullOrEmpty(IDOfNPCToComplete.stringValue))
                            targetTalker = npcs.Find(t => t.ID == IDOfNPCToComplete.stringValue);
                        int oIndex = 1;
                        if (targetTalker != null)
                            oIndex = GetNPCIndex(targetTalker) + 1;
                        List<int> indexes = new List<int>() { 0 };
                        List<string> names = new List<string>() { "" };
                        for (int i = 1; i <= npcs.Length; i++)
                        {
                            indexes.Add(i);
                            names.Add(npcNames[i - 1]);
                        }
                        oIndex = EditorGUILayout.IntPopup("请选择任务交付的对象", oIndex, names.ToArray(), indexes.ToArray());
                        if (oIndex > 0 && oIndex <= npcs.Length) IDOfNPCToComplete.stringValue = npcs[oIndex - 1].ID;
                        else IDOfNPCToComplete.stringValue = null;
                        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                    }
                    GUI.enabled = false;
                    if (!cmpltOnOriginalNPC.boolValue)
                    {
                        EditorGUILayout.PropertyField(IDOfNPCToComplete, new GUIContent("任务完成时交付NPC对象"));
                    }
                    else
                    {
                        TalkerInformation talker = npcs.Find(x => x.QuestsStored != null && x.QuestsStored.Contains(quest));
                        if (talker != null)
                            IDOfNPCToComplete.stringValue = string.IsNullOrEmpty(talker.ID) ?
                             "NPC000" : npcs.Find(x => x.QuestsStored.Contains(quest)).ID;
                        EditorGUILayout.PropertyField(IDOfNPCToComplete, new GUIContent("任务完成时交付NPC对象ID"));
                    }
                    GUI.enabled = true;
                    if ((!string.IsNullOrEmpty(IDOfNPCToComplete.stringValue) && !cmpltOnOriginalNPC.boolValue) || cmpltOnOriginalNPC.boolValue)
                    {
                        TalkerInformation completeTalker = null;
                        completeTalker = npcs.Find(x => x.ID == (!string.IsNullOrEmpty(IDOfNPCToComplete.stringValue) ?
                           IDOfNPCToComplete.stringValue : (quest.MCurrentQuestGiver != null ? quest.MCurrentQuestGiver.ID : "NPC000")));
                        GUI.enabled = false;
                        if (completeTalker)
                            if (completeTalker.HeadIcon)
                            {
                                Texture2D targetIcon = completeTalker.HeadIcon.texture;
                                EditorGUIUtility.SetIconSize(new Vector2(80, 80));
                                EditorGUILayout.LabelField(
                                    new GUIContent("交付的对象"),
                                    new GUIContent(completeTalker.Name),
                                    new GUIStyle()
                                    {
                                        fontSize = 15,
                                        imagePosition = ImagePosition.ImageLeft,
                                        fontStyle = FontStyle.Bold,
                                        normal = new GUIStyleState()
                                        {
                                            textColor = new Color32(20, 100, 40, 255),
                                        },

                                    }
                                    );
                                GUI.enabled = false;
                                EditorGUILayout.ObjectField(new GUIContent("对象头像"), completeTalker.HeadIcon, typeof(Sprite), true);
                                GUI.enabled = true;
                            }
                            else
                                EditorGUILayout.LabelField(new GUIContent() { text = "交付的对象" },
                                new GUIContent() { text = completeTalker.Name },
                                    new GUIStyle() { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = new Color32(20, 100, 40, 255) } });
                        GUI.enabled = true;
                    }
                    if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                    #endregion
                    break;
                case 1:
                    #region case 1 条件
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(acceptConditions, new GUIContent("任务接取条件列表\t\t"
                      + (acceptConditions.isExpanded ? string.Empty : (acceptConditions.arraySize > 0 ? "数量：" + acceptConditions.arraySize : "无"))), false);
                    if (acceptConditions.isExpanded)
                    {
                        serializedObject.Update();
                        acceptConditionsList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case 2:
                    #region case 2 奖励
                    SerializedProperty money = questReward.FindPropertyRelative("money");
                    SerializedProperty exp = questReward.FindPropertyRelative("exp");
                    EditorGUILayout.PropertyField(money, new GUIContent("金钱奖励"));
                    if (money.intValue < 0) money.intValue = 0;
                    EditorGUILayout.PropertyField(exp, new GUIContent("经验奖励"));
                    if (exp.intValue < 0) exp.intValue = 0;
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.HelpBox("目前只设计10个道具奖励。", MessageType.Info);
                    EditorGUILayout.PropertyField(questReward.FindPropertyRelative("Itemrewards"), new GUIContent("物品奖励列表\t\t"
                      + (questReward.FindPropertyRelative("Itemrewards").isExpanded ? string.Empty : (questReward.FindPropertyRelative("Itemrewards").arraySize > 0 ? "数量：" + questReward.FindPropertyRelative("Itemrewards").arraySize : "无"))), false);
                    if (questReward.FindPropertyRelative("Itemrewards").isExpanded)
                    {
                        serializedObject.Update();
                        rewardItemList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case 3:
                    #region case 3 对话
                    EditorGUILayout.PropertyField(beginDialogue, new GUIContent("开始时的对话"));
                    if (quest.BeginDialogue)
                    {
                        Quest find = Array.Find(Quests, x => x != quest && (x.BeginDialogue == quest.BeginDialogue || x.CompleteDialogue == quest.BeginDialogue
                                               || x.OngoingDialogue == quest.BeginDialogue));
                        if (find)
                        {
                            EditorGUILayout.HelpBox("已有任务使用该对话，游戏中可能会产生逻辑错误。\n任务名称：" + find.Title, MessageType.Warning);
                        }
                        string dialogue = string.Empty;
                        for (int i = 0; i < quest.BeginDialogue.Words.Count; i++)
                        {
                            var words = quest.BeginDialogue.Words[i];
                            dialogue += "[" + words.TalkerName + "]说：\n-" + words.Words;
                            dialogue += i == quest.BeginDialogue.Words.Count - 1 ? string.Empty : "\n";
                        }
                        GUI.enabled = false;
                        EditorGUILayout.TextArea(dialogue);
                        GUI.enabled = true;
                    }
                    EditorGUILayout.PropertyField(ongoingDialogue, new GUIContent("进行中的对话"));
                    if (quest.OngoingDialogue)
                    {
                        Quest find = Array.Find(Quests, x => x != quest && (x.BeginDialogue == quest.OngoingDialogue || x.CompleteDialogue == quest.OngoingDialogue
                                               || x.OngoingDialogue == quest.OngoingDialogue));
                        if (find)
                        {
                            EditorGUILayout.HelpBox("已有任务使用该对话，游戏中可能会产生逻辑错误。\n任务名称：" + find.Title, MessageType.Warning);
                        }
                        string dialogue = string.Empty;
                        for (int i = 0; i < quest.OngoingDialogue.Words.Count; i++)
                        {
                            var words = quest.OngoingDialogue.Words[i];
                            dialogue += "[" + words.TalkerName + "]说：\n-" + words.Words;
                            dialogue += i == quest.OngoingDialogue.Words.Count - 1 ? string.Empty : "\n";
                        }
                        GUI.enabled = false;
                        EditorGUILayout.TextArea(dialogue);
                        GUI.enabled = true;
                    }
                    EditorGUILayout.PropertyField(completeDialogue, new GUIContent("完成时的对话"));
                    if (quest.CompleteDialogue)
                    {
                        Quest find = Array.Find(Quests, x => x != quest && (x.BeginDialogue == quest.CompleteDialogue || x.CompleteDialogue == quest.CompleteDialogue
                                               || x.OngoingDialogue == quest.CompleteDialogue));
                        if (find)
                        {
                            EditorGUILayout.HelpBox("已有任务使用该对话，游戏中可能会产生逻辑错误。\n任务名称：" + find.Title, MessageType.Warning);
                        }
                        string dialogue = string.Empty;
                        for (int i = 0; i < quest.CompleteDialogue.Words.Count; i++)
                        {
                            var words = quest.CompleteDialogue.Words[i];
                            dialogue += "[" + words.TalkerName + "]说：\n-" + words.Words;
                            dialogue += i == quest.CompleteDialogue.Words.Count - 1 ? string.Empty : "\n";
                        }
                        GUI.enabled = false;
                        EditorGUILayout.TextArea(dialogue);
                        GUI.enabled = true;
                    }
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                    #endregion
                    break;
                case 4:
                    #region case 4 目标
                    EditorGUILayout.PropertyField(cmpltObjectiveInOrder, new GUIContent("按顺序完成目标"));
                    if (quest.CmpltObjectiveInOrder)
                    {
                        EditorGUILayout.HelpBox("勾选此项，则勾选按顺序的目标按执行顺序从小到大的顺序执行，若相同，则表示可以同时进行；" +
                            "若目标没有勾选按顺序，则表示该目标不受顺序影响。", MessageType.Info);
                    }
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();

                    EditorGUILayout.PropertyField(collectObjectives, new GUIContent("收集类目标\t\t"
                        + (collectObjectives.isExpanded ? string.Empty : (collectObjectives.arraySize > 0 ? "数量：" + collectObjectives.arraySize : "无"))), false);
                    if (collectObjectives.isExpanded)
                    {
                        serializedObject.Update();
                        collectObjectiveList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.PropertyField(killObjectives, new GUIContent("杀敌类目标\t\t"
                        + (killObjectives.isExpanded ? string.Empty : (killObjectives.arraySize > 0 ? "数量：" + killObjectives.arraySize : "无"))), false);
                    if (killObjectives.isExpanded)
                    {
                        serializedObject.Update();
                        killObjectiveList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.PropertyField(talkObjectives, new GUIContent("谈话类目标\t\t"
                        + (talkObjectives.isExpanded ? string.Empty : (talkObjectives.arraySize > 0 ? "数量：" + talkObjectives.arraySize : "无"))), false);
                    if (talkObjectives.isExpanded)
                    {
                        serializedObject.Update();
                        talkObjectiveList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.PropertyField(moveObjectives, new GUIContent("移动到点类目标\t\t"
                        + (moveObjectives.isExpanded ? string.Empty : (moveObjectives.arraySize > 0 ? "数量：" + moveObjectives.arraySize : "无"))), false);
                    if (moveObjectives.isExpanded)
                    {
                        serializedObject.Update();
                        moveObjectiveList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
            }
        }
        /// <summary>
        /// 处理条件列表
        /// </summary>
        void HandlingQuestConditionList()
        {
            acceptConditionsList = new ReorderableList(serializedObject, acceptConditions, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    SerializedProperty acceptCondition = acceptConditions.GetArrayElementAtIndex(index);
                    SerializedProperty type = acceptCondition.FindPropertyRelative("conditionType");
                    ConditionType conditionType = (ConditionType)type.enumValueIndex;
                    if (quest.AcceptConditions[index] != null)
                    {
                        if (acceptCondition != null)
                        {
                            switch (conditionType)
                            {
                                case ConditionType.None:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition, new GUIContent(string.Format("条件:[{0}]{1}", index, "无")));
                                    break;
                                case ConditionType.LevelLargeThen:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "等级大于")));
                                    break;
                                case ConditionType.LevelLessThen:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "等级小于")));
                                    break;
                                case ConditionType.LevelLargeOrEqualsThen:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "等级小于等于")));
                                    break;
                                case ConditionType.LevelLessOrEqualsThen:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "等级大于等于")));
                                    break;
                                case ConditionType.CompleteQuest:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "完成任务")));
                                    break;
                                case ConditionType.HasItem:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "拥有物品")));
                                    break;
                                case ConditionType.StoryPlay:
                                    EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "完成故事情节")));
                                    break;
                            }
                        }
                        else EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), acceptCondition,
                                        new GUIContent(string.Format("条件:[{0}]{1}", index, "(空)")));

                    }
                    int lineCount = 1;
                    if (acceptCondition.isExpanded)
                    //绘制标签
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                           type, new GUIContent("条件类型"), true);
                        lineCount++;
                        switch (conditionType)
                        {
                            case ConditionType.CompleteQuest:
                                SerializedProperty completeQuest = acceptCondition.FindPropertyRelative("completeQuest");
                                SerializedProperty IDOfCompleteQuest = acceptCondition.FindPropertyRelative("IDOfCompleteQuest");
                                if (completeQuest.objectReferenceValue == null)
                                {
                                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), IDOfCompleteQuest, new GUIContent("需完成的任务ID"));
                                    lineCount++;
                                }
                                EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), completeQuest, new GUIContent("需完成的任务"));
                                lineCount++;
                                if (!string.IsNullOrEmpty(IDOfCompleteQuest.stringValue) || completeQuest.objectReferenceValue)
                                {
                                    Quest quest = null;
                                    if (completeQuest.objectReferenceValue)
                                        quest = completeQuest.objectReferenceValue as Quest;
                                    else
                                        quest = ResourceManager.LoadAll<Quest>().Find(q => q._ID == IDOfCompleteQuest.stringValue);
                                    if (quest)
                                        EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), "任务标题", quest.Title
                                            , new GUIStyle() { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = new Color32(12, 23, 169, 255) } });
                                    else
                                        EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), "任务标题", "没有找到指定的任务"
                                             , new GUIStyle() { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = new Color32(12, 23, 169, 255) } });
                                }
                                break;
                            case ConditionType.HasItem:
                                SerializedProperty IDOfOwnedItem = acceptCondition.FindPropertyRelative("IDOfOwnedItem");
                                SerializedProperty OwnedItemNum = acceptCondition.FindPropertyRelative("OwnedItemNum");
                                EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), IDOfOwnedItem, new GUIContent("需拥有的道具ID"));
                                lineCount++;
                                EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), OwnedItemNum, new GUIContent("需拥有的道具数量"));
                                lineCount++;
                                if (OwnedItemNum.intValue < 1) OwnedItemNum.intValue = 1;
                                BagItemVO item = null;
                                item = ItemInfoManager.I.GetObjectInfoById(int.Parse(IDOfOwnedItem.stringValue));
                                if (item != null)
                                    EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), "道具名称", item.name);
                                break;
                            case ConditionType.LevelLargeOrEqualsThen:
                            case ConditionType.LevelLargeThen:
                            case ConditionType.LevelLessThen:
                            case ConditionType.LevelLessOrEqualsThen:
                                SerializedProperty level = acceptCondition.FindPropertyRelative("level");
                                EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), level, new GUIContent("限制的等级"));
                                if (level.intValue < 1) level.intValue = 1;
                                break;
                            case ConditionType.StoryPlay:
                                EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                                    acceptCondition.FindPropertyRelative("IDofStoryAgent"), new GUIContent("完成的故事情节ID"));
                                lineCount++;
                                if (!string.IsNullOrEmpty(acceptCondition.FindPropertyRelative("IDofStoryAgent").stringValue))
                                {
                                    StoryAgent sA = null;
                                    if(allStorys!=null)
                                     sA = allStorys.Find(s => s.FlowChatID == acceptCondition.FindPropertyRelative("IDofStoryAgent").stringValue);
                                    if (sA)
                                        EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), "故事名称", sA.StoryName,
                                             new GUIStyle() { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.blue } });
                                    else
                                        EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), "故事名称", "未找到指定的故事内容",
                                            new GUIStyle() { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.blue } });
                                }
                                break;
                            default: break;
                        }
                    }
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                elementHeightCallback = (int index) =>
                {
                    SerializedProperty condition = acceptConditions.GetArrayElementAtIndex(index);
                    if (condition.isExpanded)
                    {
                        SerializedProperty type = condition.FindPropertyRelative("conditionType");
                        ConditionType conditionType = (ConditionType)type.enumValueIndex;
                        switch (conditionType)
                        {
                            case ConditionType.CompleteQuest:
                                if (!string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfCompleteQuest").stringValue))
                                    return 5 * lineHeightSpace;
                                else return 4 * lineHeightSpace;
                            case ConditionType.HasItem:
                                if (!string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfOwnedItem").stringValue))
                                    return 5 * lineHeightSpace;
                                else return 4 * lineHeightSpace;
                            case ConditionType.LevelLargeOrEqualsThen:
                            case ConditionType.LevelLargeThen:
                            case ConditionType.LevelLessThen:
                            case ConditionType.LevelLessOrEqualsThen:
                            case ConditionType.StoryPlay:
                                if (!string.IsNullOrEmpty(condition.FindPropertyRelative("IDofStoryAgent").stringValue))
                                    return 4 * lineHeightSpace;
                                else
                                    return 3 * lineHeightSpace;
                            default: return 2 * lineHeightSpace;
                        }
                    }
                    else
                        return lineHeightSpace;
                },

                onAddCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    quest.AcceptConditions.Add(new QuestAcceptCondition());
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                onRemoveCallback = (list) =>
                    {
                        serializedObject.Update();
                        EditorGUI.BeginChangeCheck();
                        //展示对话框
                        if (EditorUtility.DisplayDialog("删除", "确定删除这个条件吗？", "确定", "取消"))
                        {
                            acceptConditions.DeleteArrayElementAtIndex(list.index);
                        }
                        if (EditorGUI.EndChangeCheck())
                            serializedObject.ApplyModifiedProperties();
                    },

                drawHeaderCallback = (rect) =>
                {
                    //未完成的数量
                    int notCmpltCount = 0;
                    for (int i = 0; i < acceptConditions.arraySize; i++)
                    {
                        SerializedProperty condition = acceptConditions.GetArrayElementAtIndex(i);
                        SerializedProperty type = condition.FindPropertyRelative("conditionType");
                        ConditionType conditionType = (ConditionType)type.enumValueIndex;
                        switch (conditionType)
                        {
                            case ConditionType.CompleteQuest:
                                if (condition.FindPropertyRelative("completeQuest").objectReferenceValue == null && string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfCompleteQuest").stringValue)) notCmpltCount++;
                                break;
                            case ConditionType.HasItem:
                                if (string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfOwnedItem").stringValue) && condition.FindPropertyRelative("OwnedItemNum").intValue < 0) notCmpltCount++;
                                break;
                            case ConditionType.LevelLargeOrEqualsThen:
                            case ConditionType.LevelLargeThen:
                            case ConditionType.LevelLessThen:
                            case ConditionType.LevelLessOrEqualsThen:
                                if (condition.FindPropertyRelative("level").intValue < 1) notCmpltCount++;
                                break;
                            case ConditionType.StoryPlay:
                                if (string.IsNullOrEmpty(condition.FindPropertyRelative("IDofStoryAgent").stringValue)) notCmpltCount++;
                                break;
                            default: break;
                        }
                    }
                    EditorGUI.LabelField(rect, "条件列表", "数量：" + acceptConditions.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
                },

                drawNoneElementCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "空列表");
                }
            };
        }
        /// <summary>
        /// 处理任务物品回报列表
        /// </summary>
        void HandlingQuestRewardItemList()
        {
            SerializedProperty itemRewards = questReward.FindPropertyRelative("Itemrewards");
            rewardItemList = new ReorderableList(serializedObject, itemRewards, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    SerializedProperty itemInfo = itemRewards.GetArrayElementAtIndex(index);
                    SerializedProperty itemID = itemInfo.FindPropertyRelative("itemID");
                    SerializedProperty rewardNum = itemInfo.FindPropertyRelative("rewardNum");
                    if (quest.MQuestReward.ItemRewards[index] != null)
                    {
#if UNITY_EDITOR
                        if (EditorApplication.isPlaying && ItemInfoManager.I.GetObjectInfoById(quest.MQuestReward.ItemRewards[index].ItemID) != null)
#else
                                    if(Application.isPlaying&& ItemInfoManager.I.GetObjectInfoById(quest.MQuestReward.ItemRewards[index].ItemID) != null)
#endif
                        {
                            EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight),
                       itemInfo, new GUIContent(string.Format("物品ID：{0}，物品名称：{1},物品数量：{2}", itemID.stringValue, ItemInfoManager.I.GetObjectInfoById(quest.MQuestReward.ItemRewards[index].ItemID).name, rewardNum.intValue)));
                        }
                        else
                            EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight),
                       itemInfo, new GUIContent(string.Format("物品ID：{0}，物品名称：空 ,物品数量:{1}", itemID.intValue, rewardNum.intValue)));

                    }
                    int lineCount = 1;
                    if (itemInfo.isExpanded)
                    {
                        if (quest.MQuestReward.ItemRewards[index] != null)
                        {

                            if (quest.MQuestReward.ItemRewards[index] != null && quest.MQuestReward.ItemRewards[index].ItemID > 0)
                            {
                                BagItemVO bV = null;
#if UNITY_EDITOR
                                if (EditorApplication.isPlaying)
#else
                                    if(Application.isPlaying)
#endif
                                {
                                    bV = ItemInfoManager.I.GetObjectInfoById(quest.MQuestReward.ItemRewards[index].ItemID);
                                }
                                if (bV != null)
                                    EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), string.Format("物品名：(" + bV.name) + ")");
                                else
                                    EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), " 物品名：(空)");
                            }
                        }
                        lineCount++;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        itemID, new GUIContent("物品ID"));
                        lineCount++;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                            rewardNum, new GUIContent("数量"));
                        if (rewardNum.intValue < 1) rewardNum.intValue = 1;
                    }
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                elementHeightCallback = (int index) =>
                {
                    SerializedProperty itemInfo = itemRewards.GetArrayElementAtIndex(index);
                    if (itemInfo.isExpanded)
                        return 4 * lineHeightSpace;
                    else
                        return lineHeightSpace;
                },

                onAddCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    quest.MQuestReward.ItemRewards.Add(new ItemReward());
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                onCanAddCallback = (list) =>
                {
                    return list.count < 10;
                },

                onRemoveCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    if (EditorUtility.DisplayDialog("删除", "确定删除这个奖励吗？", "确定", "取消"))
                    {
                        quest.MQuestReward.ItemRewards.RemoveAt(list.index);
                    }
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                drawHeaderCallback = (rect) =>
                {
                    int notCmpltCount = quest.MQuestReward.ItemRewards.FindAll(x => x.ItemID < 1 && x.RewardNum < 1).Count;
                    EditorGUI.LabelField(rect, "道具奖励列表", "数量：" + itemRewards.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
                },

                drawNoneElementCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "空列表");
                }
            };
        }

        void HandlingCollectObjectiveList()
        {
            collectObjectiveList = new ReorderableList(serializedObject, collectObjectives, true, true, true, true);
            collectObjectiveList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                SerializedProperty objective = collectObjectives.GetArrayElementAtIndex(index);
                SerializedProperty displayName = objective.FindPropertyRelative("displayName");
                SerializedProperty amount = objective.FindPropertyRelative("amount");
                SerializedProperty inOrder = objective.FindPropertyRelative("inOrder");
                SerializedProperty orderIndex = objective.FindPropertyRelative("orderIndex");
                SerializedProperty itemID = objective.FindPropertyRelative("itemID");
                SerializedProperty checkBagAtAccept = objective.FindPropertyRelative("checkBagAtAccept");
                SerializedProperty loseItemAtSubmit = objective.FindPropertyRelative("loseItemAtSubmit");

                if (quest.CollectObjectives[index] != null)
                {
                    if (!string.IsNullOrEmpty(quest.CollectObjectives[index].DisplayName))
                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective,
                            new GUIContent(quest.CollectObjectives[index].DisplayName));
                    else EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective, new GUIContent("(空标题)"));
                    EditorGUI.LabelField(new Rect(rect.x + 8 + rect.width * 15 / 24, rect.y, rect.width * 24 / 15, lineHeight),
                        (cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序：" : "显示顺序：") + orderIndex.intValue);
                }
                else EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "(空)");
                int lineCount = 1;
                if (objective.isExpanded)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                    displayName, new GUIContent("标题"));
                    lineCount++;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        amount, new GUIContent("目标数量"));
                    if (amount.intValue < 1) amount.intValue = 1;
                    lineCount++;
                    if (cmpltObjectiveInOrder.boolValue)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                            inOrder, new GUIContent("按顺序"));
                        lineCount++;
                    }
                    orderIndex.intValue = EditorGUI.IntSlider(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序" : "显示顺序", orderIndex.intValue, 1,
                        collectObjectives.arraySize + killObjectives.arraySize + talkObjectives.arraySize + moveObjectives.arraySize);
                    lineCount++;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        itemID, new GUIContent("目标道具ID"));
                    lineCount++;
                    if (!string.IsNullOrEmpty(quest.CollectObjectives[index].ItemID))
                    {
#if UNITY_EDITOR
                        if (EditorApplication.isPlaying)
#else
                                    if(Application.isPlaying)
#endif
                            EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        "道具名称", ItemInfoManager.I.GetObjectInfoById(int.Parse(quest.CollectObjectives[index].ItemID)).name);
                        lineCount++;
                    }
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        checkBagAtAccept, new GUIContent("接取任务时检查数量"));
                    lineCount++;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        loseItemAtSubmit, new GUIContent("提交时失去相应道具"));
                    lineCount++;
                }
                if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            };

            collectObjectiveList.elementHeightCallback = (int index) =>
            {
                SerializedProperty objective = collectObjectives.GetArrayElementAtIndex(index);
                int lineCount = 1;
                if (objective.isExpanded)
                {
                    lineCount++;//目标数量
                    if (cmpltObjectiveInOrder.boolValue)
                        lineCount++;// 按顺序
                    lineCount += 1;//执行顺序
                    if (!quest.CmpltObjectiveInOrder) lineCount++;//标题
                    lineCount += 4;//目标道具、接取时检查、提交时失去
                    if (!string.IsNullOrEmpty(quest.CollectObjectives[index].ItemID))
                        lineCount += 1;//道具名称
                }
                return lineCount * lineHeightSpace;
            };

            collectObjectiveList.onAddCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                quest.CollectObjectives.Add(new CollectObjective());
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            collectObjectiveList.onRemoveCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                if (EditorUtility.DisplayDialog("删除", "确定删除目标 [ " + quest.CollectObjectives[list.index].DisplayName + " ] 吗？", "确定", "取消"))
                {
                    quest.CollectObjectives.RemoveAt(list.index);
                }
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            collectObjectiveList.drawHeaderCallback = (rect) =>
            {
                int notCmpltCount = quest.CollectObjectives.FindAll(x => string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.ItemID)).Count;
                EditorGUI.LabelField(rect, "收集类目标列表", "数量：" + collectObjectives.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
            };

            collectObjectiveList.drawNoneElementCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "空列表");
            };
        }

        void HandlingKillObjectiveList()
        {
            killObjectiveList = new ReorderableList(serializedObject, killObjectives, true, true, true, true);
            killObjectiveList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                SerializedProperty objective = killObjectives.GetArrayElementAtIndex(index);
                SerializedProperty displayName = objective.FindPropertyRelative("displayName");
                SerializedProperty amount = objective.FindPropertyRelative("amount");
                SerializedProperty inOrder = objective.FindPropertyRelative("inOrder");
                SerializedProperty orderIndex = objective.FindPropertyRelative("orderIndex");
                SerializedProperty enemyID = objective.FindPropertyRelative("enermyID");

                if (quest.KillObjectives[index] != null)
                {
                    if (!string.IsNullOrEmpty(quest.KillObjectives[index].DisplayName))
                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective,
                            new GUIContent(quest.KillObjectives[index].DisplayName));
                    else EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective, new GUIContent("(空标题)"));
                    EditorGUI.LabelField(new Rect(rect.x + 8 + rect.width * 15 / 24, rect.y, rect.width * 24 / 15, lineHeight),
                        (cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序：" : "显示顺序：") + orderIndex.intValue);
                }
                int lineCount = 1;
                if (objective.isExpanded)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                    displayName, new GUIContent("标题"));
                    lineCount++;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        amount, new GUIContent("目标数量"));
                    lineCount++;
                    if (cmpltObjectiveInOrder.boolValue)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                            inOrder, new GUIContent("按顺序"));
                        lineCount++;
                    }
                    orderIndex.intValue = EditorGUI.IntSlider(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                       cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序" : "显示顺序", orderIndex.intValue, 1,
                        collectObjectives.arraySize + killObjectives.arraySize + talkObjectives.arraySize + moveObjectives.arraySize);
                    lineCount++;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), enemyID, new GUIContent("目标ID"));
                    lineCount++;
                    EnemyInformation enemyInfo = allEnemys.Find(e => e.EnemyID == int.Parse(enemyID.stringValue));
                    if (enemyInfo != null)
                    {
                        GUI.enabled = false;
                        EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),new GUIContent("目标姓名"),new GUIContent(enemyInfo.EnemyName));
                        lineCount++;
                        if (enemyInfo.EnemyIcon)
                        {
                            EditorGUI.ObjectField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight * 2),new GUIContent("敌人图像"),enemyInfo.EnemyIcon,typeof(Sprite),false);
                            lineCount += 2;
                        }
                        EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),new GUIContent("目标描述"),new GUIContent(enemyInfo.EnemyDes));
                        GUI.enabled = true;
                    }

                }
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            killObjectiveList.elementHeightCallback = (int index) =>
            {
                SerializedProperty objective = killObjectives.GetArrayElementAtIndex(index);
                int lineCount = 1;//头
                if (objective.isExpanded)
                {
                    lineCount++;//目标数量
                    if (cmpltObjectiveInOrder.boolValue)
                        lineCount++;//按顺序
                    lineCount += 1;//执行顺序
                    if (!quest.CmpltObjectiveInOrder) lineCount++;//标题
                    lineCount += 1;//目标ID
                    lineCount++;//图标
                    EnemyInformation enemyInfo = allEnemys.Find(e => e.EnemyID == int.Parse(objective.FindPropertyRelative("enermyID").stringValue));
                    if (enemyInfo)
                    {
                        lineCount += 2;//敌人姓名，描述
                        if (enemyInfo.EnemyIcon) lineCount += 2;//敌人图像
                    }
                }
                return lineCount * lineHeightSpace;
            };

            killObjectiveList.onAddCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                quest.KillObjectives.Add(new KillObjective());
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            killObjectiveList.onRemoveCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                if (EditorUtility.DisplayDialog("删除", "确定删除目标 [ " + quest.KillObjectives[list.index].DisplayName + " ] 吗？", "确定", "取消"))
                {
                    quest.KillObjectives.RemoveAt(list.index);
                }
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            killObjectiveList.drawHeaderCallback = (rect) =>
            {
                int notCmpltCount = quest.KillObjectives.FindAll(x => string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.EnemyID)).Count;
                EditorGUI.LabelField(rect, "杀敌类目标列表", "数量：" + killObjectives.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
            };

            killObjectiveList.drawNoneElementCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "空列表");
            };
        }
        /// <summary>
        /// 处理对话类列表
        /// </summary>
        void HandlingTalkObjectiveList()
        {
            talkObjectiveList = new ReorderableList(serializedObject, talkObjectives, true, true, true, true);
            talkObjectiveList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                SerializedProperty objective = talkObjectives.GetArrayElementAtIndex(index);
                SerializedProperty displayName = objective.FindPropertyRelative("displayName");
                SerializedProperty amount = objective.FindPropertyRelative("amount");
                SerializedProperty inOrder = objective.FindPropertyRelative("inOrder");
                SerializedProperty orderIndex = objective.FindPropertyRelative("orderIndex");
                SerializedProperty TalkerID = objective.FindPropertyRelative("talkerID");
                SerializedProperty dialogue = objective.FindPropertyRelative("dialogue");
                if (quest.TalkObjectives[index] != null)
                {
                    if (!string.IsNullOrEmpty(quest.TalkObjectives[index].DisplayName))
                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective,
                             new GUIContent(quest.TalkObjectives[index].DisplayName));
                    else
                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective,
                             new GUIContent("(空标题)"));
                    EditorGUI.LabelField(new Rect(rect.x + 8 + rect.width * 15 / 24, rect.y, rect.width * 24 / 15, lineHeight),
                        (cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序：" : "显示顺序：") + orderIndex.intValue);
                }
                else EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "(空)");
                int lineCount = 1;
                if (objective.isExpanded)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                    displayName, new GUIContent("标题"));
                    lineCount++;
                    if (cmpltObjectiveInOrder.boolValue)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                            inOrder, new GUIContent("按顺序"));
                        lineCount++;
                    }
                    orderIndex.intValue = EditorGUI.IntSlider(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                       cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序" : "显示顺序", orderIndex.intValue, 1,
                        collectObjectives.arraySize + killObjectives.arraySize + talkObjectives.arraySize + moveObjectives.arraySize);
                    lineCount++;
                    if (npcs != null)
                    {
                        TalkerInformation targetTalker = npcs.Find(t => t.ID == TalkerID.stringValue);
                        int oIndex = 1;
                        if (targetTalker != null)
                            oIndex = GetNPCIndex(targetTalker) + 1;
                        List<int> indexes = new List<int>() { 0 };
                        List<string> names = new List<string>() { "" };
                        for (int i = 1; i <= npcs.Length; i++)
                        {
                            indexes.Add(i);
                            names.Add(npcNames[i - 1]);
                        }
                        oIndex = EditorGUI.IntPopup(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight),
                          "请选择对话者", oIndex, names.ToArray(), indexes.ToArray());
                        if (oIndex > 0 && oIndex <= npcs.Length) TalkerID.stringValue = npcs[oIndex - 1].ID;
                        else TalkerID.stringValue = null;
                        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                    }
                    lineCount++;
                    GUI.enabled = false;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight), TalkerID, new GUIContent("对话者ID"));
                    GUI.enabled = true;
                    lineCount++;
                    if (!string.IsNullOrEmpty(TalkerID.stringValue))
                    {
                        GUI.enabled = false;
                        TalkerInformation targetTalker = npcs.Find(t => t.ID == TalkerID.stringValue);
                        if (targetTalker)
                        {
                            if (targetTalker.HeadIcon)
                                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight * 2),
                                    new GUIContent("玩家信息"),
                                    new GUIContent(string.Format(npcs.Find(t => t.ID == TalkerID.stringValue).Name), npcs.Find(t => t.ID == TalkerID.stringValue).HeadIcon.texture),
                                    new GUIStyle() { fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.red } });
                            else
                                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight * 2),
                                new GUIContent("玩家信息"),
                                new GUIContent(string.Format(npcs.Find(t => t.ID == TalkerID.stringValue).Name)),
                                new GUIStyle() { fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.red } });
                        }
                        GUI.enabled = true;
                        lineCount += 2;
                    }
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        dialogue, new GUIContent("交谈时的对话"));
                    lineCount++;
                    if (quest.TalkObjectives[index].Dialogue)
                    {
                        Quest find = Array.Find(Quests, x => x != quest && x.TalkObjectives.Exists(y => y.Dialogue == quest.TalkObjectives[index].Dialogue));
                        if (find)
                        {
                            EditorGUI.HelpBox(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight * 2.4f),
                                "已有目标使用该对话，游戏中可能会产生逻辑错误。\n任务名称：" + find.Title, MessageType.Warning);
                            lineCount += 2;
                        }
                    }
                    if (quest.TalkObjectives[index].Dialogue && quest.TalkObjectives[index].Dialogue.Words[0] != null)
                    {
                        GUI.enabled = false;
                        EditorGUI.TextArea(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                            quest.TalkObjectives[index].Dialogue.Words[0].TalkerName + "说：" + quest.TalkObjectives[index].Dialogue.Words[0].Words);
                        GUI.enabled = true;
                        lineCount++;
                    }
                }
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            talkObjectiveList.elementHeightCallback = (int index) =>
            {
                SerializedProperty objective = talkObjectives.GetArrayElementAtIndex(index);
                int lineCount = 1;
                if (objective.isExpanded)
                {
                    if (!quest.CmpltObjectiveInOrder)
                        lineCount++;//标题
                    if (cmpltObjectiveInOrder.boolValue)
                        lineCount++;//按顺序
                    lineCount += 3;//执行顺序、目标NPC、选择框
                    if (!string.IsNullOrEmpty(objective.FindPropertyRelative("talkerID").stringValue)) lineCount += 2;//引用资源
                    lineCount++; //交谈时对话
                    if (quest.TalkObjectives[index].Dialogue && Array.Exists(Quests, x => x != quest && x.TalkObjectives.Exists(y => y.Dialogue == quest.TalkObjectives[index].Dialogue)))
                        lineCount += 2;//逻辑错误
                    if (quest.TalkObjectives[index].Dialogue && quest.TalkObjectives[index].Dialogue.Words[0] != null)
                        lineCount += 1;//对话的第一句
                    lineCount++;
                }
                return lineCount * lineHeightSpace;
            };

            talkObjectiveList.onAddCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                quest.TalkObjectives.Add(new TalkObjective());
                if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            };

            talkObjectiveList.onRemoveCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                if (EditorUtility.DisplayDialog("删除", "确定删除目标 [ " + quest.TalkObjectives[list.index].DisplayName + " ] 吗？", "确定", "取消"))
                    quest.TalkObjectives.RemoveAt(list.index);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            talkObjectiveList.drawHeaderCallback = (rect) =>
            {
                int notCmpltCount = quest.TalkObjectives.FindAll(x => string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.TalkerID) || !x.Dialogue).Count;
                EditorGUI.LabelField(rect, "谈话类目标列表", "数量：" + talkObjectives.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
            };

            talkObjectiveList.drawNoneElementCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "空列表");
            };
        }
        /// <summary>
        /// 处理移动类目标
        /// </summary>
        void HandlingMoveObjectiveList()
        {
            moveObjectiveList = new ReorderableList(serializedObject, moveObjectives, true, true, true, true);
            moveObjectiveList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                SerializedProperty objective = moveObjectives.GetArrayElementAtIndex(index);
                SerializedProperty displayName = objective.FindPropertyRelative("displayName");
                SerializedProperty amount = objective.FindPropertyRelative("amount");
                SerializedProperty inOrder = objective.FindPropertyRelative("inOrder");
                SerializedProperty orderIndex = objective.FindPropertyRelative("orderIndex");
                SerializedProperty pointID = objective.FindPropertyRelative("pointID");

                if (quest.MoveObjectives[index] != null)
                {
                    if (!string.IsNullOrEmpty(quest.MoveObjectives[index].DisplayName))
                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective,
                            new GUIContent(quest.MoveObjectives[index].DisplayName));
                    else EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), objective, new GUIContent("(空标题)"));
                    EditorGUI.LabelField(new Rect(rect.x + 8 + rect.width * 15 / 24, rect.y, rect.width * 24 / 15, lineHeight),
                        (cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序：" : "显示顺序：") + orderIndex.intValue);
                }
                else EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "(空)");
                int lineCount = 1;
                if (objective.isExpanded)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                    displayName, new GUIContent("标题"));
                    lineCount++;
                    if (cmpltObjectiveInOrder.boolValue)
                    {
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                            inOrder, new GUIContent("按顺序"));
                        lineCount++;
                    }
                    orderIndex.intValue = EditorGUI.IntSlider(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        cmpltObjectiveInOrder.boolValue && inOrder.boolValue ? "执行顺序" : "显示顺序", orderIndex.intValue, 1,
                        collectObjectives.arraySize + killObjectives.arraySize + talkObjectives.arraySize + moveObjectives.arraySize);
                    lineCount++;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                        pointID, new GUIContent("目标地点识别码"));
                    lineCount++;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * lineCount, rect.width, lineHeight),
                     "目标地点", GameManager.I.AllQuestPoint != null ? (GameManager.I.AllQuestPoint.ContainsKey(pointID.stringValue) ?
                     GameManager.I.AllQuestPoint[pointID.stringValue]._ID : "未找到目标点") : "未找到目标点");
                    lineCount++;
                }
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            moveObjectiveList.elementHeightCallback = (int index) =>
            {
                SerializedProperty objective = moveObjectives.GetArrayElementAtIndex(index);
                int lineCount = 1;
                if (objective.isExpanded)
                {
                    if (cmpltObjectiveInOrder.boolValue)
                        lineCount++;//按顺序
                    lineCount++;//执行顺序
                    if (!quest.CmpltObjectiveInOrder) lineCount++;//标题
                    lineCount += 3;//目标地点
                }
                return lineCount * lineHeightSpace;
            };

            moveObjectiveList.onAddCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                quest.MoveObjectives.Add(new MoveObjective());
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            moveObjectiveList.onRemoveCallback = (list) =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                if (EditorUtility.DisplayDialog("删除", "确定删除目标 [ " + quest.MoveObjectives[list.index].DisplayName + " ] 吗？", "确定", "取消"))
                {
                    quest.MoveObjectives.RemoveAt(list.index);
                }
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            };

            moveObjectiveList.drawHeaderCallback = (rect) =>
            {
                int notCmpltCount = quest.MoveObjectives.FindAll(x => string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.PointID)).Count;
                EditorGUI.LabelField(rect, "移动到点类目标列表", "数量：" + moveObjectives.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
            };

            moveObjectiveList.drawNoneElementCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "空列表");
            };
        }
        /// <summary>
        /// 获得自动ID
        /// </summary>
        /// <returns></returns>
        string GetAutoID()
        {
            string newID = string.Empty;
            Quest[] quests = Resources.LoadAll<Quest>("");
            for (int i = 1; i < 1000; i++)
            {
                newID = "QUEST" + i.ToString().PadLeft(3, '0');
                if (!Array.Exists(quests, x => x._ID == newID))
                    break;
            }
            return newID;
        }
        /// <summary>
        /// 检查是否存在ID
        /// </summary>
        /// <returns></returns>
        bool ExistsID()
        {
            Quest[] quests = ResourceManager.LoadAll<Quest>().ToArray();

            Quest find = Array.Find(quests, x => x._ID == ID.stringValue);
            if (!find) return false;//若没有找到，则ID可用
                                    //找到的对象不是原对象 或者 找到的对象是原对象且同ID超过一个 时为true
            return find != quest || (find == quest && Array.FindAll(quests, x => x._ID == ID.stringValue).Length > 1);
        }
        /// <summary>
        ///检查是否编译完成
        /// </summary>
        /// <returns></returns>
        bool CheckEditComplete()
        {
            bool editComplete = true;

            editComplete &= !(string.IsNullOrEmpty(quest._ID) || string.IsNullOrEmpty(quest.Title) || string.IsNullOrEmpty(quest.Description));
            editComplete &= !(!quest.CmpltOnOriginalNPC && string.IsNullOrEmpty(quest._IDOfNPCToComplete));
            editComplete &= quest.AcceptConditions != null && !quest.AcceptConditions.Exists(x =>
                 {
                     switch (x.ConditionType)
                     {
                         case ConditionType.CompleteQuest:
                             if (x.CompleteQuest || !string.IsNullOrEmpty(x._IDOfCompleteQuest)) return false;
                             else return true;
                         case ConditionType.HasItem:
                             if (string.IsNullOrEmpty(x._IDOfOwnedItem) && x._OwnedItemNum < 1) return false;
                             else return true;
                         case ConditionType.LevelLargeOrEqualsThen:
                         case ConditionType.LevelLargeThen:
                         case ConditionType.LevelLessThen:
                         case ConditionType.LevelLessOrEqualsThen:
                             if (x.Level >= 1) return false;
                             else return true;
                         case ConditionType.StoryPlay:
                             if (!string.IsNullOrEmpty(x._IDofStoryAgent)) return false;
                             else return true;
                         default: return false;
                     }
                 });

            editComplete &= !(quest.MQuestReward == null);

            editComplete &= quest.CollectObjectives == null || (quest.CollectObjectives != null && !quest.CollectObjectives.Exists(x => (!quest.CmpltObjectiveInOrder || (quest.CmpltObjectiveInOrder && (x.InOrder && x.OrderIndex < 0))) && string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.ItemID)));

            editComplete &= quest.KillObjectives == null || quest.KillObjectives != null && !quest.KillObjectives.Exists(x => (!quest.CmpltObjectiveInOrder || (quest.CmpltObjectiveInOrder && (x.InOrder && x.OrderIndex < 0))) && string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.EnemyID));

            editComplete &= quest.TalkObjectives == null || quest.TalkObjectives != null && !quest.TalkObjectives.Exists(x => (!quest.CmpltObjectiveInOrder || (quest.CmpltObjectiveInOrder && (x.InOrder && x.OrderIndex < 0))) && string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.TalkerID) || x.Dialogue == null);

            editComplete &= quest.MoveObjectives == null || quest.MoveObjectives != null && !quest.MoveObjectives.Exists(x => (!quest.CmpltObjectiveInOrder || (quest.CmpltObjectiveInOrder && (x.InOrder && x.OrderIndex < 0))) && string.IsNullOrEmpty(x.DisplayName) || string.IsNullOrEmpty(x.PointID));

            return editComplete;
        }

        int GetNPCIndex(TalkerInformation npc)
        {
            return Array.IndexOf(npcs, npc);
        }

    }
}