using Common;
using Fungus;
using UnityEngine;
[CommandInfo("Animation",
    "Play Animation",
    "Play Animation from Object")]
public class PlayAnimation : Command
{
    public FindType findType;
    [ConditionalHide("findType", (int)~FindType.ExistBeforePlay, true)]
    public Animator targetAnimator;
    [ConditionalHide("findType", (int)~FindType.CreateDynamic, true)]
    public Transform parent;
    [ConditionalHide("findType", (int)~FindType.CreateDynamic, true)]
    public int index;
    public string conditionName;
    public override void OnEnter()
    {
        if (findType == FindType.CreateDynamic)
            targetAnimator = parent.GetChild(index).GetComponent<Animator>();
        targetAnimator.SetTrigger(conditionName);
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(223, 12, 235, 220);
    }
    public override string GetSummary()
    {
        return conditionName;
    }
}
public enum FindType
{
    ExistBeforePlay,
    CreateDynamic
}
