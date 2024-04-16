using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpenButton : MonoBehaviour
{
        public GameObject objectToOpen;

        public void OpenOnClick()
        {
            if (objectToOpen != null)
            {
               objectToOpen.SetActive(true);
            }
        }
}
