using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GangaGame
{
    public static class SettingsScript
    {
        public static string[] settingsTabs = new string[2] { "gameTab", "soundTab" };

        public class SettingsFild
        {
            public SettingsFild(Type _type, string _name, string _title, int _tab = 0, bool _isMainMenu = false, float _minRange = 0.0f, float _maxRange = 1.0f, float _factor = 1.0f, object _defaultValue = null)
            {
                type = _type;
                name = _name;
                title = _title;
                tab = _tab;
                isMainMenu = _isMainMenu;
                minRange = _minRange;
                maxRange = _maxRange;
                factor = _factor;
                defaultValue = _defaultValue;
            }
            public Type type;
            public string name;
            public string title;
            public int tab = 0;
            public bool isMainMenu = false;
            public float minRange = 0.0f;
            public float maxRange = 1.0f;
            public float factor = 1.0f;
            public object defaultValue;
        }

        public static void SetDefaultSettingsIfNotSetted()
        {
            if (PlayerPrefs.GetInt("settingsSetted") != 1)
            {
                SettingsFild[] settings = GetSettingsFields();
                foreach (var setting in settings)
                {
                    if (setting.type == typeof(string))
                        PlayerPrefs.SetString(setting.name, (string)setting.defaultValue);
                    if (setting.type == typeof(float))
                        PlayerPrefs.SetFloat(setting.name, (float)setting.defaultValue);
                    if (setting.type == typeof(bool))
                        PlayerPrefs.SetInt(setting.name, (int)setting.defaultValue);
                }
                PlayerPrefs.SetInt("settingsSetted", 1);
                PlayerPrefs.Save();
            }
        }

        public static SettingsFild[] GetSettingsFields()
        {
            List<SettingsFild> settings = new List<SettingsFild>();
            settings.Add(new SettingsFild(typeof(string), "username", "Username", _tab: 0, _isMainMenu: true, _defaultValue: "Player"));
            settings.Add(new SettingsFild(typeof(bool), "isUnitHealthAlwaysSeen", "Is unit health always displayed", _tab: 0, _defaultValue: 1));
            settings.Add(new SettingsFild(typeof(bool), "isBuildingHealthAlwaysSeen", "Is building health always displayed", _tab: 0, _defaultValue: 0));

            settings.Add(new SettingsFild(typeof(float), "musicVolume", "Music volume", _tab: 1, _factor: 100.0f, _defaultValue: 0.5f));
            settings.Add(new SettingsFild(typeof(float), "soundsVolume", "Sounds volume", _tab: 1, _factor: 100.0f, _defaultValue: 0.5f));
            settings.Add(new SettingsFild(typeof(float), "interfaceVolume", "Interface sounds volume", _tab: 1, _factor: 100.0f, _defaultValue: 0.5f));
            return settings.ToArray();
        }

        public static bool CreateSettings(string window, bool mainMenu = false, int tab = 0)
        {
            UI.document.getElementsByClassName(window)[0].innerHTML = "";

            SettingsFild[] settings = GetSettingsFields();
            foreach (var setting in settings)
            {
                if (setting.tab == tab)
                {
                    if (setting.isMainMenu && !mainMenu)
                        continue;

                    if (setting.type == typeof(string))
                        UI.document.Run("CreateSettingText", window, setting.name, setting.title, String.Format("{0:0F}", PlayerPrefs.GetString(setting.name)));
                    if (setting.type == typeof(float))
                        UI.document.Run("CreateSettingFloat", window, setting.name, setting.title, PlayerPrefs.GetFloat(setting.name) * setting.factor, setting.minRange * setting.factor, setting.maxRange * setting.factor);
                    if (setting.type == typeof(bool))
                        UI.document.Run("CreateSettingCheckbox", window, setting.name, setting.title, PlayerPrefs.GetInt(setting.name) == 1 ? true : false);
                }
            }
            return true;
        }

        public static int GetSettingsTab(string className)
        {
            int tabIndex = -1;
            foreach (string tabName in settingsTabs)
            {
                tabIndex++;
                if (className.Contains(tabName))
                    return tabIndex;
            }
            return -1;
        }

        public static bool SettingsSaveOrError(string errorClassName, string messagePrefix = "")
        {
            var messageDiv = UI.document.getElementsByClassName(errorClassName)[0];
            string errorMessage = SaveSettings();
            if (errorMessage != "")
            {
                messageDiv.innerHTML = messagePrefix + errorMessage;
                messageDiv.style.color = "red";
                return false;
            }
            else
            {
                messageDiv.innerHTML = messagePrefix + "Settings saved!";
                messageDiv.style.color = "green";
                return true;
            }
        }

        public static bool ChangeTabOrSaveSettings(string className, string windowSettings, string saveClassName, string errorClassName, bool mainMenu = false)
        {
            int newTab = GetSettingsTab(className);
            if (newTab != -1)
            {
                bool saved = SettingsSaveOrError(messagePrefix: "Save settings before swich tab: ", errorClassName: errorClassName);
                if (saved)
                    CreateSettings(windowSettings, mainMenu: mainMenu, tab: newTab);
                return true;
            }
            else if (className.Contains(saveClassName))
            {
                SettingsSaveOrError(errorClassName: errorClassName);
                return true;
            }
            return false;
        }

        public static string SaveSettings()
        {
            SettingsFild[] settings = GetSettingsFields();
            foreach (var setting in settings)
            {
                if (UI.document.getElementsByClassName(setting.name).length > 0)
                {
                    string newValue = UI.document.getElementsByClassName(setting.name)[0].innerText;
                    if (setting.type == typeof(string))
                    {
                        if (newValue.Length <= 0)
                            return String.Format("Wrong \"{0}\" value", setting.title);

                        PlayerPrefs.SetString(setting.name, newValue);
                    }
                    if (setting.type == typeof(float))
                    {
                        var floatRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
                        if (!floatRegex.IsMatch(newValue))
                            return String.Format("Wrong \"{0}\" value", setting.title);
                        float floatValue = float.Parse(newValue) / setting.factor;
                        if (floatValue < setting.minRange)
                            return String.Format("\"{0}\" less than {1}", setting.title, setting.minRange);
                        if (floatValue > setting.maxRange)
                            return String.Format("\"{0}\" more than {1}", setting.title, setting.maxRange);

                        PlayerPrefs.SetFloat(setting.name, floatValue);
                    }
                    if (setting.type == typeof(bool))
                    {
                        var checkbox = (HtmlInputElement)UI.document.getElementsByClassName(setting.name)[0];
                        PlayerPrefs.SetInt(setting.name, checkbox.Checked ? 1 : 0);
                    }
                }
            }
            PlayerPrefs.Save();
            return "";
        }
    }
}
