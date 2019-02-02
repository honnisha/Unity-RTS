using Photon.Pun;
using PowerUI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GangaGame
{
    public class GameMenuBehavior : MonoBehaviour
    {
        public enum WindowType { None, MainMenu, BigMap, Settings, LoadSave };
        [HideInInspector]
        public WindowType selectedWindowType = WindowType.None;
        public Dictionary<WindowType, List<object>> menuInfos = new Dictionary<WindowType, List<object>>();

        private void Start()
        {
            menuInfos.Add(WindowType.MainMenu, new List<object>());
            menuInfos[WindowType.MainMenu].Add(KeyCode.F10);
            menuInfos[WindowType.MainMenu].Add("buttonMainMenu");

            menuInfos.Add(WindowType.BigMap, new List<object>());
            menuInfos[WindowType.BigMap].Add(KeyCode.M);
            menuInfos[WindowType.BigMap].Add("buttonBigMap");

            menuInfos.Add(WindowType.Settings, new List<object>());
            menuInfos[WindowType.Settings].Add("CreateSettings");

            menuInfos.Add(WindowType.LoadSave, new List<object>());
            menuInfos[WindowType.LoadSave].Add("LoadSaveButton");

            UpdateMenuUIEvents();
        }

        private void Update()
        {
            // Update windows by keypressed
            if (UnityEngine.Input.anyKeyDown)
            {
                foreach (var menuInfo in menuInfos)
                    foreach (object eventType in menuInfo.Value)
                        if (eventType is KeyCode)
                            if (UnityEngine.Input.GetKeyDown((KeyCode)eventType))
                            {
                                UpdateWindows(menuInfo.Key);
                                break;
                            }
            }
        }

        public void UpdateWindows(WindowType newWindowsType)
        {
            WindowType cacheWindow = selectedWindowType;
            if(selectedWindowType != WindowType.None)
            {
                DestroyWindow();
                selectedWindowType = WindowType.None;
            }

            if (newWindowsType != WindowType.None && cacheWindow != newWindowsType)
            {
                CreateWindow(newWindowsType);
                UpdateMenuUIEvents();
                selectedWindowType = newWindowsType;
            }
        }

        public void UpdateMenuUIEvents()
        {
            foreach (var menuInfo in menuInfos)
                foreach (var eventCall in menuInfo.Value)
                    if (eventCall is string)
                    {
                        foreach (Dom.Element element in UI.document.getElementsByClassName((string)eventCall))
                            element.onclick = MenuUIClick;
                    }

            foreach (Dom.Element element in UI.document.getElementsByClassName("MainMenuButton"))
                element.onclick = MainMenuUIClick;

            foreach (Dom.Element element in UI.document.getElementsByClassName("saveSettings"))
                element.onclick = TabChangeOrSaveSettings;

            foreach (Dom.Element element in UI.document.getElementsByClassName("SettingsTab"))
                element.onclick = TabChangeOrSaveSettings;

            foreach (Dom.Element element in UI.document.getElementsByClassName("saveGame"))
                element.onclick = OnButtonSaveGame;

            foreach (Dom.Element element in UI.document.getElementsByClassName("loadingButton"))
                element.onclick = LoadFileClick;
        }

        public void CreateWindow(WindowType windowType)
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.interfaceSource.PlayOneShot(cameraController.clickSound, PlayerPrefs.GetFloat("interfaceVolume"));
            if (windowType == WindowType.MainMenu)
                UI.document.Run("CreateMenu", !PhotonNetwork.InRoom);
            else if (windowType == WindowType.BigMap)
            {
                UI.document.Run("DisplayBigMapWindow");
                MapScript.CreateOrUpdateMaps(ref cameraController.mapCache);
            }
            else if (windowType == WindowType.Settings)
            {
                UI.document.Run("DisplaySettingsWindow");
                SettingsScript.CreateSettings("WindowContent");
            }
            else if (windowType == WindowType.LoadSave)
            {
                LoadSaveScript.selectedFile = "";
                UI.document.Run("DisplayLoadSaveWindow");
                LoadSaveScript.UpdateSaveList();
            }
        }

        public void DestroyWindow()
        {
            if (UI.document.getElementsByClassName("menu").length > 0)
                UI.document.getElementsByClassName("menu")[0].innerHTML = "";
                
            if (UI.document.getElementsByClassName("window").length > 0)
                UI.document.getElementsByClassName("window")[0].remove();
                
            if (UI.document.getElementsByClassName("secondWindow").length > 0)
                UI.document.getElementsByClassName("secondWindow")[0].remove();
        }

        void MenuUIClick(MouseEvent mouseEvent)
        {
            foreach (var menuInfo in menuInfos)
                foreach (object eventType in menuInfo.Value)
                    if (eventType is string)
                        if (mouseEvent.srcElement.className.Contains((string)eventType))
                        {
                            UpdateWindows(menuInfo.Key);
                            break;
                        }
        }

        void MainMenuUIClick(MouseEvent mouseEvent)
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (mouseEvent.srcElement.className.Contains("CloseMenu"))
            {
                UI.document.getElementsByClassName("menu")[0].innerHTML = "";
                selectedWindowType = WindowType.None;
            }
            else if (mouseEvent.srcElement.className.Contains("GoToMainMenu"))
            {
                UI.document.Run("CreateLoadingScreen", "Leave");
                PhotonNetwork.Disconnect();
                PhotonNetwork.LoadLevel("Levels/menu");
            }
            else if (mouseEvent.srcElement.className.Contains("CloseTheGame"))
            {
                Application.Quit();
                return;
            }
        }

        public void DisplayMessage(string message)
        {
            var messageDiv = UI.document.getElementsByClassName("windowMessage")[0];
            messageDiv.innerHTML = message;
            messageDiv.style.color = "green";
        }

        void OnButtonSaveGame(MouseEvent mouseEvent)
        {
            string saveName = LoadSaveScript.SaveGame();
            DisplayMessage(new StringBuilder(60).AppendFormat("Game saved: {0}", saveName).ToString());
            LoadSaveScript.UpdateSaveList();
        }

        void LoadFileClick(MouseEvent mouseEvent)
        {
            if (mouseEvent.srcElement.className.Contains("LoadFile"))
            {
                LoadSaveScript.LoadFile();
            }
            if (mouseEvent.srcElement.className.Contains("DeleteFile"))
            {
                LoadSaveScript.DeleteSaveFile();
                DisplayMessage(new StringBuilder(60).AppendFormat("File deleted: {0}", LoadSaveScript.selectedFile).ToString());
                LoadSaveScript.selectedFile = "";
            }
        }

        void TabChangeOrSaveSettings(MouseEvent mouseEvent)
        {
            bool changed = SettingsScript.ChangeTabOrSaveSettings(mouseEvent.srcElement.className, windowSettings: "WindowContent", saveClassName: "saveSettings", errorClassName: "windowMessage", mainMenu: false);
            if (changed)
            {
                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                cameraController.interfaceSource.PlayOneShot(cameraController.clickSound, PlayerPrefs.GetFloat("interfaceVolume"));
                cameraController.UpdateSettings();
                return;
            }
        }
    }
}
