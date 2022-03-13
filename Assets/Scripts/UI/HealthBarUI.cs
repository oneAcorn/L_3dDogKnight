using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisible;
    public float visibleDuration;
    private float timeLeft;

    private Image healthSlider;
    private Transform UIBar;
    private Transform cameraTf;

    private CharacterState currentState;

    private void Awake()
    {
        currentState = GetComponent<CharacterState>();
        currentState.updateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cameraTf = Camera.main.transform;
        foreach (var canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                //只有血条是这样渲染的,所以可以这样找
                UIBar = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = UIBar.GetChild(0).GetComponent<Image>();
                UIBar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int curHealth, int maxHealth)
    {
        if (curHealth <= 0)
        {
            Destroy(UIBar.gameObject);
        }

        UIBar.gameObject.SetActive(true);
        timeLeft = visibleDuration;
        float sliderPercent = (float) curHealth / (float) maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIBar == null)
            return;
        UIBar.position = barPoint.position;
        UIBar.forward = -cameraTf.forward;
        if(timeLeft<=0&&!alwaysVisible)
            UIBar.gameObject.SetActive(false);
        else
        {
            timeLeft -= Time.deltaTime;
        }
    }
}