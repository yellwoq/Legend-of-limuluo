using System;
using Components;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class UserSetPanel : BasePanel
    {
        private Text userText, pwdText;
        private InputField userInput;
        private InputField pwdInput;
        private Button checkButton;
        private Button cancelButton;
        private Button exitLoginButton;
        private Button pwdChangeButton;
        private Button uNameChangeButton;
        protected override void Awake()
        {
            base.Awake();
            userInput = Find<InputField>("userInput");
            pwdInput = Find<InputField>("pwdInput");
            checkButton = Find<Button>("checkButton");
            cancelButton = Find<Button>("cancelButton");
            exitLoginButton = Find<Button>("exitLoginButton");
            pwdChangeButton = Find<Button>("pwdChangeButton");
            uNameChangeButton = Find<Button>("uNameChangeButton");
            userText = Find<Text>("userText");
            pwdText = Find<Text>("pwdText");
            checkButton.onClick.AddListener(UpDateData);
            cancelButton.onClick.AddListener(CancelUpDate);
            exitLoginButton.onClick.AddListener(TryLoginOut);
            pwdChangeButton.onClick.AddListener(ChangePswShow);
            uNameChangeButton.onClick.AddListener(ChangeUnameShow);
        }
        /// <summary>
        /// 更改名称设置
        /// </summary>
        private void ChangeUnameShow()
        {
            pwdText.gameObject.SetActive(false);
            pwdInput.gameObject.SetActive(false);
            userText.gameObject.SetActive(true);
            userInput.gameObject.SetActive(true);

        }

        /// <summary>
        /// 更改密码设置
        /// </summary>
        private void ChangePswShow()
        {
            pwdText.gameObject.SetActive(true);
            pwdInput.gameObject.SetActive(true);
            userText.gameObject.SetActive(false);
            userInput.gameObject.SetActive(false);

        }

        /// <summary>
        /// 注销
        /// </summary>
        private void LoginOut(object o)
        {
            SendNotification(NotiList.LOGOUT);
        }

        /// <summary>
        /// 尝试注销
        /// </summary>
        private void TryLoginOut()
        {
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            Alert.Show("确认注销", "你确定要注销吗?",
                LoginOut);
        }
        /// <summary>
        /// 取消更新
        /// </summary>
        private void CancelUpDate()
        {
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            userInput.text = string.Empty;
            pwdInput.text = string.Empty;
            Hide();
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        private void UpDateData()
        {
            //点击音效
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            if(userInput.gameObject.activeSelf == false && pwdInput.gameObject.activeSelf == false)
            {
                Debug.LogError("不能修改!");
                // 弹出框
                Alert.Show("修改失败", "请选择要修改的类型!");
                return;
            }
            // 检查用户名和密码在激活状态是否为空
            if ((userInput.text == string.Empty && userInput.gameObject.activeSelf==true)||
                (pwdInput.text == string.Empty && pwdInput.gameObject.activeSelf == true))
            {
                Debug.LogError("当前要改的数据不能为空!");
                // 弹出框
                Alert.Show("修改失败", "所输入的内容不能为空!");
                return;
            }
            // 检查是否有非法字符
            if (!StringHelper.IsSafeSqlString(userInput.text)
                || StringHelper.CheckBadWord(userInput.text)
                || !StringHelper.IsSafeSqlString(pwdInput.text)
                || StringHelper.CheckBadWord(pwdInput.text))
            {
                Debug.LogError("用户名和密码均不能有非法字符!");
                // 弹出框
                Alert.Show("修改失败", "不能有非法字符!");
                return;
            }
            // UserVO
            UserVO user = new UserVO();
            if (userInput.gameObject.activeSelf == true)
            {
                user.username = userInput.text;
                user.password = "";
            }
            else
            {
                user.username = "";
                user.password = pwdInput.text;
            }
            user.uid= GameController.instance.crtUser.uid;
            // 发送 修改 消息
            SendNotification(NotiList.CHANGE + NotiList.USER_DATA, user);
        }
    }
}
