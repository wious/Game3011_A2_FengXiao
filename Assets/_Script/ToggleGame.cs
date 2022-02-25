using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGame : MonoBehaviour
{
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
