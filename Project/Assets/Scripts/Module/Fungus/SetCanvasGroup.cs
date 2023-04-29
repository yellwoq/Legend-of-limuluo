using Common;
using Fungus;
using UnityEngine;

[CommandInfo("UI",
    "SetCanvasGroup",
    "Set CanvasGroup.Alpha from target")]
public class SetCanvasGroup : Command
{
    [DisplayName("是否使用名称查找")]
    public bool useName;
    [ConditionalHide("名称", "useName", true)]
    public string canvasname;
    public CanvasGroup canvasGroup;
    [Range(0, 1)]
    public float alphaValue;
    public override void OnEnter()
    {
        if (canvasGroup == null)
        {
            canvasGroup = FindObjectsOfType<CanvasGroup>(true).Find(cg => cg.name == canvasname);
        }
        canvasGroup.alpha = alphaValue;
        canvasGroup.blocksRaycasts = true;
        Continue();
    }
    public override Color GetButtonColor()
    {
        return new Color32(124, 1, 45, 219);
    }

    public override string GetSummary()
    {
        if (canvasGroup == null)
            if (canvasname != null)
                return canvasname + "(" + alphaValue + ")";
            else
                return "Error:No CanvasGroup";
        else
            return canvasGroup.name + "(" + alphaValue + ")";
    }
}
