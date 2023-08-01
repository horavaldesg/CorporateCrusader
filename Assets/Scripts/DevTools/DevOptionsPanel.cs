using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevOptionsPanel : MonoBehaviour
{
    [SerializeField] private GameObject devOptionsPanel;

    private int numPresses = 0;

    public void OptionsButtonPress()
    {
        StopAllCoroutines();
        numPresses++;
        if(numPresses >= 5)
        {
            ToggleDevOptionsPanel();
            numPresses = 0;
            return;
        }
        StartCoroutine(ResetDelay());
    }

    public void ToggleDevOptionsPanel() => devOptionsPanel.SetActive(!devOptionsPanel.activeSelf);

    private IEnumerator ResetDelay()
    {
        yield return new WaitForSeconds(3);
        numPresses = 0;
    }
}
