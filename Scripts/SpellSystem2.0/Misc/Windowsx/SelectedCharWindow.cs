using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCharWindow : Window
{
    public Text charName;
    public Slider healthSlider;
    public Slider resourceSlider;

    [HideInInspector] public Character selectedChar;
    public override void Open()
    {
        gameObject.SetActive(true);

        charName.text = selectedChar.getCharName();

        StatValue health = selectedChar.getStatByName("Health");
        healthSlider.transform.GetChild(1).GetComponentInChildren<Image>().color = health.getStat().getColor();
        healthSlider.maxValue = health.getMax();
        healthSlider.value = health.getValue();

        StatValue resource = selectedChar.getResources()[0]; // For now we'll use only one resource
        resourceSlider.transform.GetChild(1).GetComponentInChildren<Image>().color = resource.getStat().getColor();
        resourceSlider.maxValue = resource.getMax();
        resourceSlider.value = resource.getValue();
    }
    public override void Close()
    {
        gameObject.SetActive(false);
    }
    public override bool isOpened()
    {
        return gameObject.activeSelf;
    }
    public override void Populate()
    {
        StatValue health = selectedChar.getStatByName("Health");
        healthSlider.value = health.getValue();
        StatValue resource = selectedChar.getResources()[0]; 
        resourceSlider.value = resource.getValue();
    }
}
