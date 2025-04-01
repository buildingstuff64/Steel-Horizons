using Assets.Scripts.Game_Visuals;
using Assets.Scripts.UI;
using Assets.Scripts.Game_Logic.FormationMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using DG.Tweening;

// string name, image movement, image attack pattern, string support buff, int cost

public class UIPopups : MonoBehaviour
{
    public static UIPopups Instance;
    public PieceType currentPiece;
    [SerializeField] private RawImage movesImage;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textDesc;
    [SerializeField] private TMP_Text textBuff;
    [SerializeField] public TMP_Text cost;
    [SerializeField] private TMP_Text buttontext;
    public Vector3 mouseOffset;

    public bool movesAtks = false;
    public bool isSupported = false;
    private int state = 0;

    [ContextMenu("Save Data to Json")]
    void saveData()
    {
        UIPopupsInfo.saveInfoasjson();
    }

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket) && state > 0)
        {
            state--;
            updateState();
            updatePopupData();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket) && state < 3)
        {
            state++;
            updateState();
            updatePopupData();
        }

    }

    private void updateState()
    {
        switch (state)
        {
            case 0:
                movesAtks = false ;
                isSupported = false;
                buttontext.text = "Unsupported - Moves";
                break;
            case 1:
                movesAtks = true ;
                isSupported = false;
                buttontext.text = "Unsupported - Attacks";
                break;
            case 2:
                movesAtks = false;
                isSupported = true;
                buttontext.text = "Supported - Moves";
                break;
            case 3:
                movesAtks = true;
                isSupported = true;
                buttontext.text = "Supported - Attacks";
                break;
            default: break;
        }

    }

    public void show(bool x)
    {
        if (x)
        {
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
            updatePopupData();
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutCirc);
        }
        else
        {
            if (transform.localScale != Vector3.one) return;
            transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutCirc).OnComplete(() => { gameObject.SetActive(false); });
        }
    }

    public void updatePositions()
    {
        transform.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition + mouseOffset;
    }

    public void updatePopupData(){

        UIPopupsInfo.PieceUIInfo info = SaveSystem.Load<UIPopupsInfo.PieceUIInfo>(string.Format("PieceInfo/{0}_Info", currentPiece.ToString()));

        textName.text = info.Name;
        textDesc.text = info.Description;
        textBuff.text = info.Buff;
        cost.text = string.Format("$ {0}", (info.Cost * 100).ToString());
        movesImage.texture = UIPopupsInfo.getAttacksMoves(currentPiece, movesAtks, isSupported);

    }
    
    public void changeSupported()
    {
        isSupported = !isSupported;
        updatePopupData();
    }

    public void switchAttaksMoves()
    {
        movesAtks = !movesAtks;
        buttontext.text = movesAtks ? "Attacks" : "Moves";
        updatePopupData();

    }
}
