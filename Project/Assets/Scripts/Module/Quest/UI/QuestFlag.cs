using MapSystem;
using QuestSystem;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务标志
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class QuestFlag : MonoBehaviour
{
    /// <summary>
    /// 图像显示
    /// </summary>
    [SerializeField]
    private SpriteRenderer iconRenderer;
    /// <summary>
    /// 未接受时的图像
    /// </summary>
    [SerializeField]
    private Sprite notAccepted;
    /// <summary>
    /// 接取时图像
    /// </summary>
    [SerializeField]
    private Sprite accepted;
    /// <summary>
    /// 完成时头像
    /// </summary>
    [SerializeField]
    private Sprite complete;
    /// <summary>
    /// 所属任务持有者
    /// </summary>
    [SerializeField, HideInInspector]
    private QuestGiver questHolder;
    /// <summary>
    /// 所属图标
    /// </summary>
    [SerializeField, HideInInspector]
    private MapIcon mapIcon;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="questHolder"></param>
    public void Init(QuestGiver questHolder)
    {
        iconRenderer = GetComponent<SpriteRenderer>();
        this.questHolder = questHolder;
        if (MapManager.Instance)
        {
            if (mapIcon) MapManager.Instance.RemoveMapIcon(mapIcon, true);
            mapIcon = MapManager.Instance.CreateMapIcon(notAccepted, Vector2.one * 48, questHolder.currentPosition, false, MapIconType.Quest, false);
            mapIcon.iconImage.raycastTarget = false;
            mapIcon.Hide();
        }
        UpdateUI();
        Update();
        if (QuestManager.Instance) QuestManager.Instance.OnQuestStatusChange += UpdateUI;
    }

    private bool conditionShow;
    /// <summary>
    /// 图标更新检测
    /// </summary>
    public void UpdateUI()
    {
        if (questHolder.ID == "NPC000")
        {
            this.gameObject.SetActive(false);
            List<Quest> compQuests = QuestManager.I.QuestsOngoing.FindAll(q => (q.MOriginQuestGiver.ID == questHolder.ID) && q.IsComplete);
            bool hasQuestComplete = compQuests.Count > 0;
            if (hasQuestComplete)
            {
                foreach (var comQuest in compQuests)
                {
                    QuestManager.I.CompleteQuest(comQuest);
                }
            }
            return;
        }
        else
        //如果有对话目标是本任务持有者并且所有的前继目标完成没有后继目标
        {
            bool hastalkObjective = QuestManager.I.QuestsOngoing.FindAll(x =>
          {
              TalkObjective talkObjective = x.TalkObjectives.Find(e => e.TalkerID == questHolder.ID);
              if (talkObjective != null)
                  return talkObjective.AllPrevObjCmplt && !talkObjective.IsComplete;
              else
                  return false;
          }).Count > 0;
            //如果该NPC身上有未完成的对话目标
            if (hastalkObjective)
            {
                Debug.Log(hastalkObjective);
                iconRenderer.enabled = true;
                iconRenderer.sprite = complete;
                mapIcon.iconImage.overrideSprite = complete;
                conditionShow = true;
                return;
            }
            foreach (var quest in questHolder.QuestInstances)
            {
                //只要有一个没接取
                if (!quest.IsComplete && !QuestManager.I.QuestsOngoing.Find(e => e._ID == quest._ID) && quest.AcceptAble)
                {
                    iconRenderer.enabled = true;
                    iconRenderer.sprite = notAccepted;
                    mapIcon.iconImage.overrideSprite = notAccepted;
                    conditionShow = true;
                    return;
                }
                //只要有一个完成
                else if (quest.IsComplete && QuestManager.I.QuestsOngoing.Find(e => e._ID == quest._ID))
                {
                    iconRenderer.enabled = true;
                    iconRenderer.sprite = complete;
                    mapIcon.iconImage.overrideSprite = complete;
                    conditionShow = true;
                    return;
                }
            }
            //如果没有目标了
            if (questHolder.QuestInstances.Count < 1 && !hastalkObjective)
            {
                iconRenderer.enabled = false;
                mapIcon.Hide();
                conditionShow = false;
                return;
            }
            iconRenderer.enabled = false;
            conditionShow = false;
        }
    }
    /// <summary>
    /// 回收图标
    /// </summary>
    public void Recycle()
    {
        if (MapManager.Instance) MapManager.Instance.RemoveMapIcon(mapIcon);
        questHolder = null;
        mapIcon = null;
        ObjectPool.Put(gameObject);
    }
    void Update()
    {
        if (questHolder)
        {
            if (questHolder.isActiveAndEnabled && conditionShow) mapIcon.Show();
            else mapIcon.Hide();
        }
    }
    private void OnDestroy()
    {
        if (MapManager.Instance) MapManager.Instance.RemoveMapIcon(mapIcon);
        if (MapManager.Instance) QuestManager.Instance.OnQuestStatusChange -= UpdateUI;
    }
}