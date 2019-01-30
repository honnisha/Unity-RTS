using CI.QuickSave;
using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GangaGame
{
    public static class LoadSaveScript
    {
        public const string saveFolder = "save";

        public static string selectedFile = "";
        
        static HtmlElement loadNote;

        public static void UpdateSaveList() //"secondWindow"
        {
            UI.document.getElementsByClassName("WindowContent")[0].innerHTML = "";
            
            foreach (var saveName in QuickSaveRaw.GetAllFiles())
            {
                bool selected = selectedFile == saveName;

                object createdImageObject = UI.document.Run("CreateLoadingRecord", "WindowContent", saveName, selected);
                loadNote = (HtmlElement)((Jint.Native.JsValue)createdImageObject).ToObject();
                loadNote.onclick = SelectLoadNote;
            }
        }

        private static void SelectLoadNote(MouseEvent mouseEvent)
        {
            selectedFile = mouseEvent.srcElement.id;
            UpdateSaveList();
        }

        public static void LoadFile()
        {
            if (selectedFile == "")
                return;
            
            string simple = BayatGames.SaveGameFree.SaveGame.Load<string>("simple.txt", "The Default Value");
            UI.document.Run("CreateLoadingScreen", "Loading: " + selectedFile);

            selectedFile = "";
            SceneManager.LoadScene("Levels/Map1");
        }

        public static void DeleteSaveFile()
        {
            if (selectedFile == "")
                return;

            QuickSaveRaw.Delete(Path.Combine(saveFolder, selectedFile));
            UpdateSaveList();
        }

        public static string SaveGame()
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();

            string saveName = Path.Combine(saveFolder, DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss"));
            QuickSaveRaw.SaveInt(saveName, "seed", cameraController.mapSeed);
            return saveName;
        }
    }
}