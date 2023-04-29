using Common;
using QuestSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DialogueSystem
{

    [CustomEditor(typeof(TalkerInformation))]
    public class TalkerInformationEditor : ScriptObjectIconEditor
    {
        TalkerInformation talkerInfo;
        SerializedProperty _ID;
        SerializedProperty _Name;
        SerializedProperty chType;
        SerializedProperty headIcon;
        SerializedProperty defaultDialogue;
        SerializedProperty isVendor;
        SerializedProperty isHasQuest;
        SerializedProperty questsStored;

        ReorderableList questsStoredList;
        List<Quest> quests => ResourceManager.LoadAll<Quest>();

        float lineHeight;
        float lineHeightSpace;
        private void OnEnable()
        {
            lineHeight = EditorGUIUtility.singleLineHeight;
            lineHeightSpace = lineHeight + 5;
            talkerInfo = target as TalkerInformation;
            _ID = serializedObject.FindProperty("_ID");
            _Name = serializedObject.FindProperty("_Name");
            chType = serializedObject.FindProperty("chType");
            headIcon = serializedObject.FindProperty("headIcon");
            defaultDialogue = serializedObject.FindProperty("defaultDialogue");
            isVendor = serializedObject.FindProperty("isVendor");
            isHasQuest = serializedObject.FindProperty("isHasQuest");
            questsStored = serializedObject.FindProperty("questsStored");
            HandlingQuestStore();
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            if (talkerInfo.HeadIcon != null)
            {
                Type t = GetType("UnityEditor.SpriteUtility");
                if (t != null)
                {
                    MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                    if (method != null)
                    {
                        object ret = method.Invoke("RenderStaticPreview", new object[] { talkerInfo.HeadIcon, Color.white, width, height });
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
                EditorGUILayout.HelpBox("该人物信息存在未补全信息。", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("该人物信息已完整。", MessageType.Info);
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            headIcon.objectReferenceValue = EditorGUILayout.ObjectField("角色头像", headIcon.objectReferenceValue as Sprite, typeof(Sprite), false);
            EditorGUILayout.PropertyField(_ID, new GUIContent("人物识别码"));
            if (string.IsNullOrEmpty(_ID.stringValue) || ExistsID())
            {
                if (!string.IsNullOrEmpty(_ID.stringValue) && ExistsID())
                    EditorGUILayout.HelpBox("此识别码已存在！", MessageType.Error);
                else
                    EditorGUILayout.HelpBox("识别码为空！", MessageType.Error);
                if (GUILayout.Button("自动生成识别码"))
                {
                    _ID.stringValue = GetAutoID();
                    EditorGUI.FocusTextInControl(null);
                }
            }
            EditorGUILayout.PropertyField(_Name, new GUIContent("角色姓名"));
            EditorGUILayout.PropertyField(chType, new GUIContent("角色类别"));
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(defaultDialogue, new GUIContent("默认对话"));
            EditorGUILayout.PropertyField(isVendor, new GUIContent("是否为商人"));
            EditorGUILayout.PropertyField(isHasQuest, new GUIContent("是否持有任务"));
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            if (!isVendor.boolValue && isHasQuest.boolValue)
            {
                EditorGUILayout.PropertyField(questsStored, new GUIContent("所持有的任务列表\t\t"
                  + (questsStored.isExpanded ? string.Empty : (questsStored.arraySize > 0 ? "数量：" + questsStored.arraySize : "无"))), false);
                if (questsStored.isExpanded)
                {
                    serializedObject.Update();
                    questsStoredList.DoLayoutList();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        /// <summary>
        /// 处理储藏的任务
        /// </summary>
        private void HandlingQuestStore()
        {
            questsStoredList = new ReorderableList(serializedObject, questsStored, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    SerializedProperty quest = questsStored.GetArrayElementAtIndex(index);
                    int lineCount = 1;
                    if (talkerInfo.QuestsStored[index] != null)
                    {

                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), quest,
                               new GUIContent(string.Format("{0}", (quest.objectReferenceValue as Quest).Title)));

                    }
                    else EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), quest,
                                new GUIContent("任务未指定"));
                    Quest currentQuest = null;
                    if (quest.objectReferenceValue)
                    {
                        currentQuest = quest.objectReferenceValue as Quest;
                        if (!string.IsNullOrEmpty(currentQuest.Title))
                        {
                            EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight),
                            new GUIContent("任务标题"), new GUIContent() { text = currentQuest.Title },
                             new GUIStyle() { fontSize = 13, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.yellow, background = Texture2D.linearGrayTexture } });
                            lineCount++;
                        }
                        if (!string.IsNullOrEmpty(currentQuest.Description))
                        {
                            EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight * 2),
                                new GUIContent("任务描述"), new GUIContent() { text = currentQuest.Description },
                                new GUIStyle() { fontSize = 13 });
                            lineCount += 2;
                        }
                        if (!currentQuest.CmpltOnOriginalNPC)
                        {
                            List<TalkerInformation> npcs = ResourceManager.LoadAll<TalkerInformation>();
                            if (!string.IsNullOrEmpty(currentQuest._IDOfNPCToComplete))
                            {
                                TalkerInformation completeNPC = npcs.Find(x => x.ID == currentQuest._IDOfNPCToComplete);
                                if (completeNPC)
                                {
                                    EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight),
                                        new GUIContent("交付对象："), new GUIContent() { text = completeNPC.Name });
                                    lineCount++;
                                    if (completeNPC.HeadIcon)
                                    {
                                        EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, 2 * lineHeight),
                                          new GUIContent("对象图标："), new GUIContent() { image = completeNPC.HeadIcon.texture });
                                        lineCount += 2;
                                    }
                                }
                                else
                                {
                                    EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, 2 * lineHeight),
                                         new GUIContent("未找到该NPC"));
                                    lineCount++;
                                }
                            }
                            else
                            {
                                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, 2 * lineHeight),
                                          new GUIContent("未找到该NPC"));
                                lineCount++;
                            }
                        }
                        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        if (quest.objectReferenceValue)
                        {
                            currentQuest = quest.objectReferenceValue as Quest;
                            GUI.enabled = false;
                            EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight),
                   new GUIContent("任务标题"), new GUIContent() { text = currentQuest.Title },
                   new GUIStyle() { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.cyan } });
                            GUI.enabled = true;
                        }
                    }

                },

                elementHeightCallback = (int index) =>
                {
                    int lineCount = 1;
                    SerializedProperty quest = questsStored.GetArrayElementAtIndex(index);
                    Quest currentQuest = null;
                    if (quest.objectReferenceValue)
                    {
                        currentQuest = quest.objectReferenceValue as Quest;
                        if (!string.IsNullOrEmpty(currentQuest.Title))
                            lineCount++;//任务标题
                        if (!string.IsNullOrEmpty(currentQuest.Description))
                            lineCount += 2;//任务描述
                        if (!currentQuest.CmpltOnOriginalNPC)
                        {
                            List<TalkerInformation> npcs = ResourceManager.LoadAll<TalkerInformation>();
                            if (!string.IsNullOrEmpty(currentQuest._IDOfNPCToComplete))
                            {
                                TalkerInformation completeNPC = npcs.Find(x => x.ID == currentQuest._IDOfNPCToComplete);
                                if (completeNPC)
                                {
                                    lineCount++; //交付对象名
                                    if (completeNPC.HeadIcon) lineCount += 2;//交付对象图标
                                }
                                else lineCount++;
                            }
                        }
                    }
                    return lineCount * lineHeightSpace;
                },

                onAddCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    talkerInfo.QuestsStored.Add(null);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                onRemoveCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    talkerInfo.QuestsStored.RemoveAt(list.index);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                drawHeaderCallback = (rect) =>
                {
                    int notCmpltCount = talkerInfo.QuestsStored.FindAll(x => x == null).Count;
                    EditorGUI.LabelField(rect, "对话内容列表", "数量：" + questsStored.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
                },

                drawNoneElementCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "空列表");
                }
            };
        }


        bool CheckEditComplete()
        {
            bool editComplete = true;

            editComplete &= !(string.IsNullOrEmpty(talkerInfo.ID));
            editComplete &= !(talkerInfo.QuestsStored != null && talkerInfo.QuestsStored.FindAll(x => x == null).Count > 0);

            return editComplete;
        }
        bool ExistsID()
        {
            List<TalkerInformation> talkerInfos = ResourceManager.LoadAll<TalkerInformation>();

            TalkerInformation find = talkerInfos.Find(x => x.ID == _ID.stringValue);
            if (!find) return false;//若没有找到，则ID可用
                                    //找到的对象不是原对象 或者 找到的对象是原对象且同ID超过一个 时为true
            return find != talkerInfo || (find == talkerInfo && talkerInfos.FindAll(x => x.ID == _ID.stringValue).Count > 1);
        }
        string GetAutoID()
        {
            string newID = string.Empty;
            List<TalkerInformation> talkerInfos = ResourceManager.LoadAll<TalkerInformation>();
            for (int i = 1; i < 1000; i++)
            {
                newID = "NPC" + i.ToString().PadLeft(3, '0');
                if (!talkerInfos.Exists(x => x.ID == newID))
                    break;
            }
            return newID;
        }
    }
}
