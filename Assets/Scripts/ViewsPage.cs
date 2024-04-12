using Cdm.Figma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Cdm.Figma.Examples
{
    [FigmaNode("@ViewsPage")]
    public class ViewsPage : MonoBehaviour
    {
        [FigmaNode("@Logo")]
        [SerializeField]
        private RectTransform _logo;

        [FigmaNode("@AboutPkgToggle")]
        [SerializeField]
        private Toggle _aboutPkgToggle;
        
        [FigmaNode("@LayoutsToggle")]
        [SerializeField]
        private Toggle _layoutsToggle;
        
        [FigmaNode("@ComponentsToggle")]
        [SerializeField]
        private Toggle _componentsToggle;
        
        [FigmaNode("@TypographyToggle")]
        [SerializeField]
        private Toggle _typographyToggle;
        
        [FigmaNode("@VectorsToggle")]
        [SerializeField]
        private Toggle _vectorsToggle;
        
        [FigmaNode("@LanguageDropdown")]
        [SerializeField]
        private TMP_Dropdown _languageDropdown;
        
        [FigmaNode("@AboutPackageContent")]
        [SerializeField]
        private Content _aboutPkgContent;
        
        [FigmaNode("@LayoutsContent")]
        [SerializeField]
        private Content _layoutsContent;
        
        [FigmaNode("@ComponentsContent")]
        [SerializeField]
        private Content _componentsContent;
        
        [FigmaNode("@TypographyContent")]
        [SerializeField]
        private Content _typographyContent;
        
        [FigmaNode("@VectorsContent")]
        [SerializeField]
        private Content _vectorsContent;
        
        private void Start()
        {
            _aboutPkgToggle.onValueChanged.AddListener(OnAboutPackageClicked);
            _layoutsToggle.onValueChanged.AddListener(OnLayoutsClicked);
            _componentsToggle.onValueChanged.AddListener(OnComponentsClicked);
            _typographyToggle.onValueChanged.AddListener(OnTyphographyClicked);
            _vectorsToggle.onValueChanged.AddListener(OnVectorsClicked);

            var group = _aboutPkgToggle.GetComponent<RectTransform>().parent.gameObject.AddComponent<ToggleGroup>();
            _aboutPkgToggle.group = group;
            _layoutsToggle.group = group;
            _componentsToggle.group = group;
            _typographyToggle.group = group;
            _vectorsToggle.group = group;
            _aboutPkgToggle.isOn = true;
            
            _languageDropdown.onValueChanged.AddListener(OnLocalizationChange);
            _languageDropdown.options.Add(new TMP_Dropdown.OptionData("English"));
            _languageDropdown.options.Add(new TMP_Dropdown.OptionData("Türkçe"));
            _languageDropdown.value = 0;
            _languageDropdown.RefreshShownValue();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_aboutPkgToggle.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutsToggle.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_componentsToggle.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_typographyToggle.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_vectorsToggle.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(_logo.GetComponent<RectTransform>());
            
            Canvas.ForceUpdateCanvases();
        }

        private void OnDestroy()
        {
            _aboutPkgToggle.onValueChanged.RemoveAllListeners();
            _layoutsToggle.onValueChanged.RemoveAllListeners();
            _componentsToggle.onValueChanged.RemoveAllListeners();
            _languageDropdown.onValueChanged.RemoveAllListeners();
            _typographyToggle.onValueChanged.RemoveAllListeners();
            _vectorsToggle.onValueChanged.RemoveAllListeners();
        }

        private void OnAboutPackageClicked(bool isOn)
        {
            if (isOn)
            {
                _aboutPkgContent.Show();
            }
            else
            {
                _aboutPkgContent.Hide();
            }
        }
        
        private void OnLayoutsClicked(bool isOn)
        {
            if (isOn)
            {
                _layoutsContent.Show();
            }
            else
            {
                _layoutsContent.Hide();
            }
        }
        
        private void OnComponentsClicked(bool isOn)
        {
            if (isOn)
            {
                _componentsContent.Show();
            }
            else
            {
                _componentsContent.Hide();
            }
        }
        
        private void OnVectorsClicked(bool isOn)
        {
            if (isOn)
            {
                _vectorsContent.Show();
            }
            else
            {
                _vectorsContent.Hide();
            }
        }

        private void OnTyphographyClicked(bool isOn)
        {
            if (isOn)
            {
                _typographyContent.Show();
            }
            else
            {
                _typographyContent.Hide();
            }
        }

        private void OnLocalizationChange(int option)
        {
            switch (option)
            {
                case 0:
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                    break;
                case 1:
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                    break;
            }
        }
    }
}