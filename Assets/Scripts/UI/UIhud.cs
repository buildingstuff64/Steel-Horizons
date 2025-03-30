using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIhud : MonoBehaviour
    {

        public static UIhud instance;

        [SerializeField] private RectTransform mainpanel;

        [SerializeField] private Image mainColor;
        [SerializeField] private Image animationColor;

        [SerializeField] private GameObject winPanel;
        [SerializeField] private TMP_Text wintext;

        [SerializeField] private GameObject nextturnHint;

        private RectTransform panelstartvalue;

        private void Awake()
        {
            instance = this;
            panelstartvalue = mainpanel;
            mainpanel.DOAnchorPosY(200, 0f).SetEase(Ease.OutSine);
        }

        public void show(bool x)
        {
            if (x)
            {
                mainpanel.DOAnchorPosY(0, 1f).SetEase(Ease.InSine);
            }
            else
            {
                mainpanel.DOAnchorPosY(200, 1f).SetEase(Ease.OutSine);
            }
        }

        public void changeTeamColor(Color color)
        {
            animationColor.gameObject.SetActive(true);
            animationColor.color = color;
            animationColor.transform.localScale = new Vector3(0.45f, 1, 1);
            animationColor.transform.DOScaleX(1, 0.5f).SetEase(Ease.InOutQuart).OnComplete(() =>
            {
                mainColor.color = color;
                animationColor.gameObject.SetActive(false);
            });
        }

        public void showWinner(string win, Color c)
        {
            winPanel.SetActive(true);
            winPanel.transform.localScale = Vector3.zero;
            winPanel.GetComponent<Image>().color = c;
            wintext.text = win;
            winPanel.transform.DOScale(1f, 1f);
        }

        public void mainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void showhint(bool x)
        {
            if (x) 
            {
                nextturnHint.SetActive(true);
                nextturnHint.transform.DOScale(1f, 0f); 
            }
            else
            {
                nextturnHint.transform.DOScale(0f, 0f);
                nextturnHint.SetActive(false);
            }
        }
    }
}