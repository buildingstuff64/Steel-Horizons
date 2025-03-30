using Assets.Scripts.Game_Logic.SubPieces;
using DG.Tweening;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals.Visual_Sub_Pieces
{
    public class VisualShield : VisualPiece
    {
        GameObject shieldObject;

        private void Start()
        {
            foreach (Transform item in GetComponentsInChildren<Transform>())
            {
                if(item.name == "Shield")
                {
                    shieldObject = item.gameObject;
                    print(shieldObject);
                }
            }
        }

        public override void PlayMoveAnimation(Square from, Square to, Vector3 rotation, Action onComplete)
        {
            ShieldPiece sh = piece as ShieldPiece;
            shieldObject.transform.DOScaleZ(0f, 0.5f).SetEase(Ease.InExpo).OnComplete(() => 
            {
                shieldObject.SetActive(false);
                base.PlayMoveAnimation(from, to, rotation, onComplete);
            });
        }

        public override void PlayTurnStartAnimation(Action onComplete)
        {
            ShieldPiece sh = piece as ShieldPiece;
            print(sh);
            if (sh.turnsSinceMoved > 2)
            {
                shieldObject.SetActive(true);
                if (shieldObject.transform.localScale.x != 0.5f)
                {
                    shieldObject.transform.DOScaleZ(0.5f, 0.5f).SetEase(Ease.InExpo);
                }

                if (sh.isSupported)
                {
                    if (shieldObject.transform.localScale.x != 4.6)
                    {
                        shieldObject.transform.DOScaleX(4.6f, 0.5f).SetEase(Ease.OutBack);
                    }
                }
                else
                {
                    if (shieldObject.transform.localScale.x != 2.6)
                    {
                        shieldObject.transform.DOScaleX(2.6f, 0.5f).SetEase(Ease.OutBack);
                    }
                }
            }
            else
            {
                shieldObject.transform.DOScaleZ(0f, 0.5f).SetEase(Ease.InExpo).OnComplete(() => shieldObject.SetActive(false));
            }

        }
    }
}