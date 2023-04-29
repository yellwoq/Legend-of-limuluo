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
    [CustomEditor(typeof(Dialogue))]
    public class DialogueInspector : ScriptObjectIconEditor
    {
        Dialogue dialogue;

        SerializedProperty _ID;
        SerializedProperty defaultSprite;
        SerializedProperty talkers;
        SerializedProperty words;

        ReorderableList talkersList;
        ReorderableList wordsList;

        float lineHeight;
        float lineHeightSpace;
        /// <summary>
        /// 所有的人物信息
        /// </summary>
        List<TalkerInformation> npcs=> ResourceManager.LoadAll<TalkerInformation>();
        List<Quest> allQuests=> ResourceManager.LoadAll<Quest>();
        TalkerInformation[] canUseNpcsArray;
        int barIndex;
        bool isPeopleComplete;
        private void OnEnable()
        {
            lineHeight = EditorGUIUtility.singleLineHeight;
            lineHeightSpace = lineHeight + 5;
            dialogue = target as Dialogue;
            _ID = serializedObject.FindProperty("_ID");
            defaultSprite = serializedObject.FindProperty("defaultSprite");
            talkers = serializedObject.FindProperty("talkers");
            words = serializedObject.FindProperty("words");
            HandingTalkersList();
            HandingWordsList();
        }
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            if (dialogue.DefaultSprite != null)
            {
                Type t = GetType("UnityEditor.SpriteUtility");
                if (t != null)
                {
                    MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                    if (method != null)
                    {
                        object ret = method.Invoke("RenderStaticPreview", new object[] { dialogue.DefaultSprite, Color.white, width, height });
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
                EditorGUILayout.HelpBox("该段对话存在未补全信息。", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("该段对话已完整。", MessageType.Info);
            barIndex = GUILayout.Toolbar(barIndex, new string[] { "参与对话者信息", "对话内容" });
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            isPeopleComplete = dialogue.Talkers != null && dialogue.Talkers.FindAll(t => t == null).Count <= 0 && dialogue.Talkers.Count > 0;
            switch (barIndex)
            {
                case 0:
                    #region 参与对话者信息
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(talkers, new GUIContent("对话者列表\t\t"
                 + (talkers.isExpanded ? string.Empty : (talkers.arraySize > 0 ? "数量：" + talkers.arraySize : "无"))), false);
                    if (talkers.isExpanded)
                    {
                        serializedObject.Update();
                        talkersList.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case 1:
                    #region 对话内容
                    defaultSprite.objectReferenceValue = EditorGUILayout.ObjectField("任务默认图标", defaultSprite.objectReferenceValue as Sprite, typeof(Sprite), false);
                    EditorGUILayout.PropertyField(_ID, new GUIContent("识别码"));
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
                    Quest find = allQuests.Find(q =>
                     {
                         return q.OngoingDialogue == dialogue || q.BeginDialogue == dialogue || q.CompleteDialogue == dialogue
                           || (q.TalkObjectives != null && q.TalkObjectives.FindAll(t => t.Dialogue == dialogue).Count > 0);
                     });
                    if (find)
                    {
                        EditorGUILayout.LabelField("当前对话所属任务ID", find._ID);
                        EditorGUILayout.LabelField("当前对话所属任务标题", find.Title);
                        GUI.enabled = false;
                        EditorGUILayout.ObjectField("当前对话所属任务", find, typeof(Quest), true);
                        GUI.enabled = true;
                    }
                    if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                    if (isPeopleComplete)
                    {
                        EditorGUILayout.PropertyField(words, new GUIContent("对话内容\t\t"
                  + (words.isExpanded ? string.Empty : (words.arraySize > 0 ? "数量：" + words.arraySize : "无"))), false);
                        if (words.isExpanded)
                        {
                            serializedObject.Update();
                            wordsList.DoLayoutList();
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("对话参与者信息未补充完整,请至参与对话者信息中补充完整", MessageType.Warning);
                    }
                    #endregion
                    break;
            }
        }
        /// <summary>
        /// 处理对话集合
        /// </summary>
        private void HandingWordsList()
        {
            wordsList = new ReorderableList(serializedObject, words, true, true, true, true)
            {

                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    SerializedProperty word = words.GetArrayElementAtIndex(index);
                    SerializedProperty talkerInfo = word.FindPropertyRelative("talkerInfo");
                    SerializedProperty wordsContent = word.FindPropertyRelative("words");
                    int lineCount = 1;
                    int oIndex = GetTalkerIndex(talkerInfo.objectReferenceValue as TalkerInformation) + 1;
                    List<int> indexes = new List<int>() { 0 };
                    List<string> names = new List<string>() { "" };
                    if (canUseNpcsArray != null)
                    {
                        for (int i = 1; i <= canUseNpcsArray.Length; i++)
                        {
                            indexes.Add(i);
                            names.Add(canUseNpcsArray[i - 1].Name);
                        }
                        oIndex = EditorGUI.IntPopup(new Rect(rect.x, rect.y+lineCount*lineHeightSpace, rect.width, lineHeight),"选择的对话者", oIndex, names.ToArray(), indexes.ToArray());
                        if (oIndex > 0 && oIndex <= canUseNpcsArray.Length) talkerInfo.objectReferenceValue = canUseNpcsArray[oIndex - 1];
                        else talkerInfo.objectReferenceValue = null;
                        lineCount++;
                    }
                    if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                    if (dialogue.Words[index] != null)
                    {
                        if (talkerInfo.objectReferenceValue)
                        {
                            EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), word,
                                   new GUIContent(string.Format("{0}", (talkerInfo.objectReferenceValue as TalkerInformation).Name)));
                        }
                        else
                        {
                            EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), word,
                                  new GUIContent("未找到对话者"));
                        }
                    }
                    TalkerInformation currentTalker = null;
                    if (word.isExpanded)
                    {
                        GUI.enabled = false;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight), talkerInfo, new GUIContent("对话者信息"));
                        GUI.enabled = true;
                        lineCount++;
                        if (talkerInfo.objectReferenceValue)
                        {
                            currentTalker = talkerInfo.objectReferenceValue as TalkerInformation;
                            if (currentTalker.HeadIcon)
                            {
                                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight * 2),
                            new GUIContent() { image = currentTalker.HeadIcon.texture });
                                lineCount += 2;
                            }
                            EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight),
                            new GUIContent() { text = currentTalker.Name + "说：" });
                            lineCount++;
                        }
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight * 4), wordsContent, new GUIContent(string.Empty));
                        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(wordsContent.stringValue))
                        {
                            if (talkerInfo.objectReferenceValue)
                            {
                                currentTalker = talkerInfo.objectReferenceValue as TalkerInformation;
                                GUI.enabled = false;
                                EditorGUI.LabelField(new Rect(rect.x, rect.y + lineCount * lineHeightSpace, rect.width, lineHeight),
                       new GUIContent("对话内容"), new GUIContent() { text = currentTalker.Name + "说：" + wordsContent.stringValue },
                       new GUIStyle() { fontSize = 15, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.blue } });
                                GUI.enabled = true;
                            }
                        }
                    }

                },

                elementHeightCallback = (int index) =>
                {
                    SerializedProperty word = words.GetArrayElementAtIndex(index);
                    if (word.isExpanded)
                        if (word.FindPropertyRelative("talkerInfo").objectReferenceValue)
                            return 10 * lineHeightSpace;
                        else
                            return 6 * lineHeightSpace;
                    else
                        return 3 * lineHeightSpace;
                },

                onAddCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    dialogue.Words.Add(new DialogueWords());
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                onRemoveCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    dialogue.Words.RemoveAt(list.index);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                drawHeaderCallback = (rect) =>
                {
                    int notCmpltCount = dialogue.Words.FindAll(x => x.TalkerInfo == null).Count;
                    EditorGUI.LabelField(rect, "对话内容列表", "数量：" + words.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
                },

                drawNoneElementCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "空列表");
                }
            };
        }
        /// <summary>
        /// 处理对话者集合
        /// </summary>
        private void HandingTalkersList()
        {
            talkersList = new ReorderableList(serializedObject, talkers, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {

                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    SerializedProperty talkerOne = talkers.GetArrayElementAtIndex(index);
                    TalkerInformation currentTalker = talkerOne.objectReferenceValue as TalkerInformation;
                    string talkerID = null;
                    string talkerName = null;
                    CharacterType characterType = CharacterType.None;
                    Sprite headIcon = null;
                    if (dialogue.Talkers[index] != null)
                    {
                        talkerID = currentTalker.ID;
                        talkerName = currentTalker.Name;
                        characterType = currentTalker.ChType;
                        headIcon = currentTalker.HeadIcon;
                        int lineCount = 1;
                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), talkerOne,
                        new GUIContent(string.IsNullOrEmpty(talkerName) ? "无名氏" : talkerName));
                        EditorGUI.LabelField(new Rect(rect.x + 8, rect.y + lineCount * lineHeightSpace, rect.width * 0.8f, lineHeight), "对话者ID", talkerID);
                        lineCount++;
                        if (!string.IsNullOrEmpty(talkerName))
                            EditorGUI.LabelField(new Rect(rect.x + 8, rect.y + lineCount * lineHeightSpace, rect.width * 0.8f, lineHeight), "对话者姓名", talkerName);
                        else
                            EditorGUI.LabelField(new Rect(rect.x + 8, rect.y + lineCount * lineHeightSpace, rect.width * 0.8f, lineHeight), "对话者姓名", "无名氏");
                        lineCount++;
                        EditorGUI.LabelField(new Rect(rect.x + 8, rect.y + lineCount * lineHeightSpace, rect.width * 0.8f, lineHeight), "对话者类型", CharacterInformation.GetSexString(characterType));
                        lineCount++;
                        if (headIcon != null)
                            EditorGUI.LabelField(new Rect(rect.x + 8, rect.y + lineCount * lineHeightSpace, rect.width * 0.8f, lineHeight * 2), new GUIContent("头像"), new GUIContent() { image = headIcon.texture },
                                new GUIStyle() { fontStyle = FontStyle.Bold, fontSize = 30 });
                    }
                    else
                        EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y, rect.width * 0.8f, lineHeight), talkerOne, new GUIContent("对话者尚未确定"));
                    if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                },
                onChangedCallback = (list) =>
                {

                },
                elementHeightCallback = (int index) =>
                {
                    SerializedProperty talkerInfo = talkers.GetArrayElementAtIndex(index);
                    if (talkerInfo.objectReferenceValue)
                    {
                        TalkerInformation currentTalker = talkerInfo.objectReferenceValue as TalkerInformation;
                        if (currentTalker.HeadIcon != null)
                            return 6 * lineHeightSpace;
                        else
                            return 4 * lineHeightSpace;
                    }
                    else
                        return lineHeightSpace;
                },

                onAddCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    if (dialogue.Talkers != null)
                        dialogue.Talkers.Add(null);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                onRemoveCallback = (list) =>
                {
                    serializedObject.Update();
                    EditorGUI.BeginChangeCheck();
                    dialogue.Talkers.RemoveAt(list.index);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();
                },

                drawHeaderCallback = (rect) =>
                {
                    int notCmpltCount = 0;
                    if (dialogue.Talkers != null)
                        notCmpltCount = dialogue.Talkers.FindAll(x => x == null).Count;
                    else
                        notCmpltCount = 1;
                    EditorGUI.LabelField(rect, "人物列表", "数量：" + talkers.arraySize + (notCmpltCount > 0 ? "\t未补全：" + notCmpltCount : string.Empty));
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

            editComplete &= !(string.IsNullOrEmpty(dialogue.ID));
            editComplete &= isPeopleComplete;
            editComplete &= !(dialogue.Words == null || (dialogue.Words != null && dialogue.Words.FindAll(x => x.TalkerInfo == null).Count > 0));

            return editComplete;
        }
        bool ExistsID()
        {
            List<Dialogue> dialogues = ResourceManager.LoadAll<Dialogue>();

            Dialogue find = dialogues.Find(x => x.ID == _ID.stringValue);
            if (!find) return false;//若没有找到，则ID可用
                                    //找到的对象不是原对象 或者 找到的对象是原对象且同ID超过一个 时为true
            return find != dialogue || (find == dialogue && dialogues.FindAll(x => x.ID == _ID.stringValue).Count > 1);
        }
        string GetAutoID()
        {
            string newID = string.Empty;
            List<Dialogue> dialogues = ResourceManager.LoadAll<Dialogue>();
            for (int i = 1; i < 1000; i++)
            {
                newID = "Dialogue" + i.ToString().PadLeft(3, '0');
                if (!dialogues.Exists(x => x.ID == newID))
                    break;
            }
            return newID;
        }
        int GetTalkerIndex(TalkerInformation mytalker)
        {
            canUseNpcsArray = dialogue.Talkers.ToArray();
            return Array.IndexOf(canUseNpcsArray, mytalker);
        }
    }
}
