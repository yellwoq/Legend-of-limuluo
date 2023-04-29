using Bag;
using Common;
using MVC;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace QuestSystem
{

    /// <summary>
    /// 任务条件绘制
    /// </summary>
    public class ConditionGroupDrawer
    {
        /// <summary>
        /// 拥有者
        /// </summary>
        private readonly SerializedObject owner;
        /// <summary>
        /// 属性
        /// </summary>
        private readonly SerializedProperty property;
        /// <summary>
        /// 行高
        /// </summary>
        private readonly float lineHeightSpace;
        /// <summary>
        /// 可重读集合
        /// </summary>
        public ReorderableList List { get; }

        public ConditionGroupDrawer(SerializedObject owner, SerializedProperty property, float lineHeight, float lineHeightSpace, string listTitle = "条件列表")
        {
            this.owner = owner;
            this.property = property;
            this.lineHeightSpace = lineHeightSpace;
            SerializedProperty conditions = property.FindPropertyRelative("acceptConditions");
            List = new ReorderableList(property.serializedObject, conditions, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    owner.Update();
                    SerializedProperty condition = conditions.GetArrayElementAtIndex(index);
                    SerializedProperty type = condition.FindPropertyRelative("acceptCondition");
                    ConditionType conditionType = (ConditionType)type.enumValueIndex;
                    //绘制标签
                    if (condition != null)
                    {
                        switch (conditionType)
                        {
                            case ConditionType.None:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "无");
                                break;
                            case ConditionType.LevelLargeThen:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "等级大于");
                                break;
                            case ConditionType.LevelLessThen:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "等级小于");
                                break;
                            case ConditionType.LevelLargeOrEqualsThen:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "等级小于等于");
                                break;
                            case ConditionType.LevelLessOrEqualsThen:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "等级大于等于");
                                break;
                            case ConditionType.CompleteQuest:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "完成任务");
                                break;
                            case ConditionType.HasItem:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "拥有物品");
                                break;
                            case ConditionType.StoryPlay:
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "[" + index + "]" + "完成故事情节");
                                break;
                        }
                    }
                    else EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "(空)");
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, lineHeight),
                        type, new GUIContent(string.Empty), true);

                    switch (conditionType)
                    {
                        case ConditionType.CompleteQuest:
                            SerializedProperty relatedQuest = condition.FindPropertyRelative("completeQuest");
                            SerializedProperty relateQuestID = condition.FindPropertyRelative("IDOfCompleteQuest");
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * 1, rect.width, lineHeight), relateQuestID, new GUIContent("需完成的任务ID"));
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * 1, rect.width, lineHeight), relatedQuest, new GUIContent("需完成的任务"));
                            if (relatedQuest.objectReferenceValue == owner.targetObject as Quest || (relateQuestID.stringValue == (owner.targetObject as Quest)._ID))
                            {
                                relatedQuest.objectReferenceValue = null;
                                relateQuestID.stringValue = null;
                            }
                            if (!string.IsNullOrEmpty(relateQuestID.stringValue ))
                            {
                                Quest quest;
                                if (relatedQuest.objectReferenceValue)
                                    quest = relatedQuest.objectReferenceValue as Quest;
                                else
                                    quest = ResourceManager.LoadAll<Quest>().Find(q => q._ID == relateQuestID.stringValue);
                                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * 2, rect.width, lineHeight), "任务标题", quest.Title);
                            }
                            break;
                        case ConditionType.HasItem:
                            SerializedProperty relatedItemID = condition.FindPropertyRelative("IDOfOwnedItem");
                            SerializedProperty relatedItemNum = condition.FindPropertyRelative("OwnedItemNum");
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * 1, rect.width, lineHeight), relatedItemID, new GUIContent("需拥有的道具ID"));
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace * 1, rect.width, lineHeight), relatedItemNum, new GUIContent("需拥有的道具数量"));
                            if (!string.IsNullOrEmpty(relatedItemID.stringValue))
                            {
                               BagItemVO item = ItemInfoManager.I.GetObjectInfoById(int.Parse(relatedItemID.stringValue));
                                if(item!=null)
                                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineHeightSpace * 2, rect.width, lineHeight), "道具名称", item.name);
                            }
                            break;
                        case ConditionType.LevelLargeOrEqualsThen:
                        case ConditionType.LevelLargeThen:
                        case ConditionType.LevelLessThen:
                        case ConditionType.LevelLessOrEqualsThen:
                            SerializedProperty level = condition.FindPropertyRelative("level");
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace, rect.width, lineHeight), level, new GUIContent("限制的等级"));
                            if (level.intValue < 1) level.intValue = 1;
                            break;
                        case ConditionType.StoryPlay:
                            EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineHeightSpace, rect.width, lineHeight),
                                condition.FindPropertyRelative("IDofStoryAgent"), new GUIContent("完成的故事情节ID"));
                            break;
                        default: break;
                    }

                    if (EditorGUI.EndChangeCheck())
                        owner.ApplyModifiedProperties();
                },

                elementHeightCallback = (int index) =>
                {
                    SerializedProperty condition = conditions.GetArrayElementAtIndex(index);
                    SerializedProperty type = condition.FindPropertyRelative("acceptCondition");
                    ConditionType conditionType = (ConditionType)type.enumValueIndex;
                    switch (conditionType)
                    {
                        case ConditionType.CompleteQuest:
                            if (condition.FindPropertyRelative("completeQuest").objectReferenceValue 
                            || !string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfCompleteQuest").stringValue))
                                return 3 * lineHeightSpace;
                            else return 2 * lineHeightSpace;
                        case ConditionType.HasItem:
                            if (!string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfOwnedItem").stringValue))
                                return 3 * lineHeightSpace;
                            else return 2 * lineHeightSpace;
                        case ConditionType.LevelLargeOrEqualsThen:
                        case ConditionType.LevelLargeThen:
                        case ConditionType.LevelLessThen:
                        case ConditionType.LevelLessOrEqualsThen:
                            return 2 * lineHeightSpace;
                        default: return lineHeightSpace;
                    }
                },

                onRemoveCallback = (list) =>
                {
                    owner.Update();
                    EditorGUI.BeginChangeCheck();
                    //展示对话框
                    if (EditorUtility.DisplayDialog("删除", "确定删除这个条件吗？", "确定", "取消"))
                    {
                        conditions.DeleteArrayElementAtIndex(list.index);
                    }
                    if (EditorGUI.EndChangeCheck())
                        owner.ApplyModifiedProperties();
                },

                drawHeaderCallback = (rect) =>
                {
                    //未完成的数量
                    int notCmpltCount = 0;
                    for (int i = 0; i < conditions.arraySize; i++)
                    {
                        SerializedProperty condition = conditions.GetArrayElementAtIndex(i);
                        SerializedProperty type = condition.FindPropertyRelative("acceptCondition");
                        ConditionType conditionType = (ConditionType)type.enumValueIndex;
                        switch (conditionType)
                        {
                            case ConditionType.CompleteQuest:
                                if (condition.FindPropertyRelative("completeQuest").objectReferenceValue == null && string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfCompleteQuest").stringValue)) notCmpltCount++;
                                break;
                            case ConditionType.HasItem:
                                if (string.IsNullOrEmpty(condition.FindPropertyRelative("IDOfOwnedItem").stringValue) && string.IsNullOrEmpty(condition.FindPropertyRelative("OwnedItemNum").stringValue)) notCmpltCount++;
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
                    EditorGUI.LabelField(rect, listTitle, "数量：" + conditions.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
                },

                drawNoneElementCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "空列表");
                }
            };
        }
        /// <summary>
        /// 布局
        /// </summary>
        public void DoLayoutDraw()
        {
            owner?.Update();
            List?.DoLayoutList();
            owner?.ApplyModifiedProperties();
            if (List != null && List.count > 0)
            {
                owner?.Update();
                EditorGUI.BeginChangeCheck();
                if (EditorGUI.EndChangeCheck())
                    owner?.ApplyModifiedProperties();
            }
        }

        public void DoDraw(Rect rect)
        {
            owner?.Update();
            List?.DoList(rect);
            owner?.ApplyModifiedProperties();
            if (List != null && List.count > 0)
            {
                owner?.Update();
                EditorGUI.BeginChangeCheck();
                if (EditorGUI.EndChangeCheck())
                    owner?.ApplyModifiedProperties();
            }
        }

        public float GetDrawHeight()
        {
            if (List == null) return 0;
            float height = List.GetHeight();
            if (List.count > 0)
                height += lineHeightSpace;
            return height;
        }
    }
}