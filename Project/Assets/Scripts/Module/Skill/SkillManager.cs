using Common;
using MVC;
using Player;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SkillSystem
{
    /// <summary>
    ///  技能管理器,管理玩家技能释放相关
    /// </summary>
    public class SkillManager : MonoSingleton<SkillManager>
    {
        /// <summary>
        /// 存储所有技能
        /// </summary>
        [DisplayName("技能集合")]
        public List<Skill> allSkills;
        [DisplayName("当前技能点")]
        public int skillpoint;
        [SerializeField, DisplayName("显示的图标大小")]
        public Vector2 iconSize = new Vector2(80, 80);
        public void InitSkill()
        {
            allSkills = new List<Skill>();
            allSkills.AddRange(Resources.LoadAll<Skill>(string.Format("Skill/" + GameController.I.crtHero.heroType)));
            foreach (var skill in allSkills)
            {
                skill.Level = 0;
                skill.IsCoding = false;
            }
            if (PlayerPrefs.HasKey(KeyList.SKILL1)) PlayerPrefs.DeleteKey(KeyList.SKILL1);
            if (PlayerPrefs.HasKey(KeyList.SKILL2)) PlayerPrefs.DeleteKey(KeyList.SKILL2);
            if (PlayerPrefs.HasKey(KeyList.SKILL3)) PlayerPrefs.DeleteKey(KeyList.SKILL3);
        }

        #region 技能攻击处理逻辑相关
        /// <summary>
        ///  释放技能
        /// </summary>
        /// <param name="skill"></param>
        public void Fire(Skill skill, UnityAction<int> callback = null)
        {
            // 如果技能处于技能冷却状态
            if (skill.IsCoding) return;
            // 如果没有处于冷却状态,开始冷却
            if (skill.Coolingtime > 0)
            {
                StartCoroutine(ColdDown(skill, skill.Coolingtime, callback));
            }
            //大招特效
            if (skill.SkillEffect != null)
            {
                Transform skillparent = PlayerManager.I.playerTrans;
                //创建大招特效
                if (skill.SkillattackMode == SkillAttackMode.InPlace && skill.skillGainType == Gaintype.Hp)
                {
                    skillparent = PlayerManager.I.playerTrans.parent;
                }
                GameObject skillGO = GameObjectPool.I.CreateObject(skill.SkillID, skill.SkillEffect, skillparent);
                //获取摇杆偏移量
                float joyPositionX = FindObjectOfType<ETCJoystick>(true).axisX.axisValue;
                float joyPositionY = FindObjectOfType<ETCJoystick>(true).axisY.axisValue;
                float currentAngle = CharacterAnimStateSwitch.CaculaterAngle(joyPositionX, joyPositionY);
                Vector3 targetPos = new Vector3(0, 0, 0);
                PlayerMove playerMove = FindObjectOfType<PlayerMove>();
                CreatePos(skill, skillGO, playerMove.lastDir);
                switch (playerMove.lastDir)
                {
                    case Direction.Left:
                        targetPos = skillGO.transform.position - new Vector3(skill.Distance, 0, 0);
                        break;
                    case Direction.Right:
                        targetPos = skillGO.transform.position + new Vector3(skill.Distance, 0, 0);
                        break;
                    case Direction.Up:
                        targetPos = skillGO.transform.position + new Vector3(0, skill.Distance, 0);
                        break;
                    case Direction.Down:
                        targetPos = skillGO.transform.position - new Vector3(0, skill.Distance, 0);
                        break;
                }
                SkillHandle(skill, skillGO, targetPos, playerMove.lastDir);
                GameObjectPool.I.CollectObject(skillGO, skill.CollectDelay);
            }
        }
        /// <summary>
        /// 技能处理
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillGO"></param>
        /// <param name="target"></param>
        private void SkillHandle(Skill skill, GameObject skillGO, Vector3 target, Direction dir)
        {

            float damageValue = 0;
            switch (skill.SkillattackMode)
            {
                case SkillAttackMode.InPlace:
                    UserSkillController uSC = skillGO.GetComponent<UserSkillController>();
                    uSC.mGainType = skill.skillGainType;
                    uSC.upValue = skill.Damage;
                    uSC.delayTime = skill.Damagetime;
                    uSC.SkillGain();
                    break;
                case SkillAttackMode.Linear:
                    LineSkillController lSC = skillGO.GetComponent<LineSkillController>();
                    lSC.targetPos = target;
                    if (skill == FindObjectOfType<PlayerAttack>().normal)
                    {
                        damageValue = FindObjectOfType<PlayerStatus>().DEM;
                        StartCoroutine(SoundManager.I.PlaySfxDelay("PlayerAttack", 1f, SoundManager.ReadAudioClipType.ResourcesLoad));
                    }
                    else
                        damageValue = skill.Damage;
                    skillGO.GetComponent<LineSkillController>().damage = damageValue;
                    break;
                case SkillAttackMode.Round:
                    RangeSkillController roundSC = skillGO.GetComponent<RangeSkillController>();
                    roundSC.CircleAttack(skillGO.transform, skill.Distance, skill.Damage, skill.DamageDelay);
                    break;
                case SkillAttackMode.Sector:
                    RangeSkillController rSC = skillGO.GetComponent<RangeSkillController>();
                    if (skill == FindObjectOfType<PlayerAttack>().normal)
                    {
                        damageValue = FindObjectOfType<PlayerStatus>().DEM;
                        StartCoroutine(SoundManager.I.PlaySfxDelay("PlayerAttack", 1f, SoundManager.ReadAudioClipType.ResourcesLoad));
                    }
                    else
                        damageValue = skill.Damage;
                    rSC.UmbrellaAttact(PlayerManager.I.playerTrans, skill.AttackAngle, skill.Distance, damageValue, skill.DamageDelay);
                    break;
                case SkillAttackMode.Move:
                    MoveSkillController mSC = skillGO.GetComponent<MoveSkillController>();
                    mSC.damage = skill.Damage;
                    mSC.distance = skill.Distance;
                    mSC.delayTime = skill.DamageDelay;
                    mSC.damageCountiueTime = skill.Damagetime;
                    mSC.dir = dir;
                    StartCoroutine(mSC.MoveAttack(mSC.damage, mSC.distance, mSC.dir, mSC.delayTime, mSC.damageCountiueTime));
                    break;
            }
            //耗蓝
            UserHeroVO newUserHeroVO = GameController.I.crtHero;
            newUserHeroVO.currentMP -= skill.MpCons;
            PlayerManager.I.heroData.SetHeroSave(newUserHeroVO);
        }

        /// <summary>
        /// 技能产生位置
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillGO"></param>
        /// <param name="dir"></param>
        private static void CreatePos(Skill skill, GameObject skillGO, Direction dir)
        {
            Releaseposition releasePos = skill.StartOffset.Find(e => { return e.ReleaseDir == dir; });
            if (releasePos != null)
            {
                skillGO.transform.localPosition = releasePos.PosOffset;
                skillGO.transform.localRotation = Quaternion.Euler(releasePos.RotateOffset);
            }
            if (skillGO.GetComponent<Animator>() != null)
            {
                for (int i = 0; i < skillGO.GetComponent<Animator>().parameters.Length; i++)
                {
                    if (skillGO.GetComponent<Animator>().parameters[i].name == dir.ToString())
                    {
                        Debug.Log(dir.ToString());
                        skillGO.GetComponent<Animator>().SetTrigger(dir.ToString());
                        break;
                    }
                }

            }
        }
        /// <summary>
        ///  技能冷却
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="cd"></param>
        /// <returns></returns>
        private IEnumerator ColdDown(Skill skill, int cd, UnityAction<int> callback = null)
        {
            Debug.Log("ColdDown: " + cd);
            callback?.Invoke(cd);
            // 更新冷却状态
            skill.IsCoding = cd > 0;
            // 等待1秒
            yield return new WaitForSeconds(1);
            // 冷却时间递减
            cd--;
            // 是否需要继续冷却
            if (cd >= 0)
                StartCoroutine(ColdDown(skill, cd, callback));
        }
        #endregion

        #region 备用
        ///// <summary>
        /////  延迟创建打击效果
        ///// </summary>
        ///// <param name="skill"></param>
        ///// <returns></returns>
        //private IEnumerator DelayCreate(Skill skill)
        //{
        //    // 时间延迟
        //    yield return new WaitForSeconds(skill.hitDelay);
        //    // 生成打击特效
        //    // 建议使用对象池创建和回收
        //    //GameObject hitEffect = Instantiate(
        //    //    skill.hitEffect,
        //    //    transform.position + skill.startOffset,
        //    //    transform.rotation);
        //    //Destroy(hitEffect, 5);
        //}

        ///// <summary>
        /////  造成伤害
        ///// </summary>
        ///// <param name="skill"></param>
        ///// <returns></returns>
        //private IEnumerator DoAttack(Skill skill)
        //{
        //    // 时间延迟
        //    yield return new WaitForSeconds(skill.DamageDelay);
        //    // 找到攻击范围之内的所有对象
        //    Collider[] colliders = Physics.OverlapSphere(
        //        transform.position, skill.AttackRange, skill.TargetMask);
        //    // 遍历
        //    foreach (Collider c in colliders)
        //    {
        //        // 判断是否满足在攻击范围内
        //        // 计算距离
        //        float distance = Vector3.Distance(
        //            transform.position, c.transform.position);
        //        // 计算伤害值
        //        float damage = (1 - distance / skill.AttackRange) * skill.MpCons;
        //        // TakeDamage
        //        EnemyStatus h = c.GetComponent<EnemyStatus>();
        //        h.TakeDamage(damage);
        //    }
        //} 
        #endregion

        #region 技能UI处理相关
        /// <summary>
        ///  通过技能ID获取技能数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Skill GetSkill(string ID)
        {
            foreach (Skill s in allSkills)
            {
                if (s.SkillID == ID) return s;
            }
            return null;
        }
        /// <summary>
        /// 更新按钮图标
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="skill"></param>
        public void SetButtonIcon(PlayerInputButton btn, Skill skill)
        {
            // 显示技能图标
            Image icon = btn.transform.GetComponent<Image>();
            icon.overrideSprite = skill.SkillIcon;
            icon.rectTransform.sizeDelta = iconSize;
        }
        /// <summary>
        /// 技能升级
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="tipMessage"></param>
        /// <returns></returns>
        public bool SkillLeveUp(Skill skill, out string tipMessage)
        {
            //已经满级
            if (skill.Level >= skill.MaxLevel)
            {
                tipMessage = "等级已满";
                return false;
            }
            //技能点不足
            if (skillpoint <= 0)
            {
                tipMessage = "技能点不足";
                return false;
            }
            skillpoint--;
            skill.Level++;
            tipMessage = "技能已经升至" + skill.Level + "级";
            return true;
        }
        #endregion
    }
}
