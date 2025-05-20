using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonColor : MonoBehaviour
{
    public Button targetButton;
    public Color activeColor = Color.green;
    public Color normalColor = Color.white;

    public bool isActive = false;

    void Start()
    {
        targetButton = GetComponent<Button>();

        SetButtonColor(normalColor);

        targetButton.onClick.AddListener(ToggleColor);
    }

    public void ToggleColor()
    {
        isActive = !isActive;

        if (isActive)
            SetButtonColor(activeColor);
        else
            SetButtonColor(normalColor);
    }

    public void SetButtonColor(Color color)
    {
        ColorBlock cb = targetButton.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        targetButton.colors = cb;
    }
}
