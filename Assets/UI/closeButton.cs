using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CloseButton : MonoBehaviour
{
        public GameObject objectToHide;

        public void CloseOnClick()
        {
            if (objectToHide != null)
            {
                objectToHide.SetActive(false);
            }
        }
}
