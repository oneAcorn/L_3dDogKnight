using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;

    private RaycastHit hitInfo;
    public event Action<Vector3> OnMouseClick;
    public event Action<GameObject> OnEnemyClicked;

    private void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    private void SetCursorTexture()
    {
        //Camera.main在Unity 2020.2中做了大幅的优化,不会导致大幅开销,可以在Update中放心使用!
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClick?.Invoke(hitInfo.point);
            }
            else if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }
}