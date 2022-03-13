using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private Text levelText;
    private Image healthSlider;
    private Image expSlider;

    // Start is called before the first frame update
    void Awake()
    {
        levelText = transform.Find("Level").GetComponent<Text>();
        healthSlider = transform.Find("HealthBar Holder/Health Slider").GetComponent<Image>();
        expSlider = transform.Find("Exp Holder/Exp Slider").GetComponent<Image>();
    }

    private void Update()
    {
        levelText.text = "Level " + GameManager.Instance.playerState.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }

    void UpdateHealth()
    {
        float sliderPercent = (float) GameManager.Instance.playerState.CurrentHealth /
                              GameManager.Instance.playerState.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    void UpdateExp()
    {
        float sliderPercent = (float) GameManager.Instance.playerState.characterData.currentExp /
                              GameManager.Instance.playerState.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}