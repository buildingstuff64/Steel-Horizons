using Assets.Scripts;
using Assets.Scripts.Game_Visuals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] BoardViewer Viewer;
    [SerializeField] MainBoardManager manager;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        cam.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.createPrettyBoard();
        }

        if (timer > 10f)
        {
            timer = 0;
            manager.createPrettyBoard();
        }
    }

    private void Start()
    {
        manager.createPrettyBoard();
    }

}
