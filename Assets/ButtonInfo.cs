using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInfo : MonoBehaviour
{
    public enum ButtonState
    {
        Up,
        Over,
        Down
    }

    public ButtonState state;

    public string buttonName;
}
