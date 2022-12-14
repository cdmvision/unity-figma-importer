using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownPopulate : MonoBehaviour
{
    private void Start()
    {
        var dropdowns = transform.GetComponentsInChildren<TMP_Dropdown>();
        foreach (var dropdown in dropdowns)
        {
            var options = new List<TMP_Dropdown.OptionData>();
            options.Add(new TMP_Dropdown.OptionData("Option A"));
            options.Add(new TMP_Dropdown.OptionData("Option B"));
            options.Add(new TMP_Dropdown.OptionData("Option C"));
            options.Add(new TMP_Dropdown.OptionData("Option D"));
            options.Add(new TMP_Dropdown.OptionData("Option E"));
            dropdown.AddOptions(options);
        }
    }
}