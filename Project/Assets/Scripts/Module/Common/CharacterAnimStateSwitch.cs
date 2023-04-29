using UnityEngine;

namespace Common
{
    /// <summary>
    /// 角色动画切换
    /// </summary>
    public class CharacterAnimStateSwitch
    {
        public static float CaculaterAngle(float x, float y)
        {
            float currentAngleX = x * 90f + 90f;//X轴 当前角度
            float currentAngleY = y * 90f + 90f;//Y轴 当前角度

            //下半圆
            if (currentAngleY < 90f)
            {
                if (currentAngleX < 90f)
                {
                    return 270f + currentAngleY;
                }
                else if (currentAngleX > 90f)
                {
                    return 180f + (90f - currentAngleY);
                }
                else
                {
                    return 270f;
                }
            }
            else if (currentAngleY == 90f)
            {
                if (currentAngleX < 90f)
                {
                    return 0;
                }
                else if (currentAngleX > 90f)
                {
                    return 180f;
                }
                else
                {
                    return 270f;
                }
            }
            return currentAngleX;
        }
    }
}