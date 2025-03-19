using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIhud : MonoBehaviour
    {

        public static UIhud instance;

        [SerializeField] private RectTransform mainpanel;

        [SerializeField] private Image mainColor;
        [SerializeField] private Image animationColor;

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
            animationColor.transform.localScale = new Vector3(0, 1, 1);
            animationColor.transform.DOScaleX(1, 1f).SetEase(Ease.InSine).OnComplete(() =>
            {
                mainColor.color = color;
                animationColor.gameObject.SetActive(false);
            });
        }
    }
}