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
                loadNote.onclick = CameraController.SelectLoadNote;
            }
        }

        public static void DeleteSaveFile()
        {
            if (selectedFile == "")
                return;

            // QuickSaveRaw.Delete(selectedFile);
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low";
            savePath = Path.Combine(savePath, Application.companyName);
            savePath = Path.Combine(savePath, Application.productName);
            savePath = Path.Combine(savePath, "QuickSave");
            savePath = Path.Combine(savePath, selectedFile + ".json");
            File.Delete(savePath);
            UpdateSaveList();
        }

        public static void LoadFile()
        {
            if (selectedFile == "")
                return;

            QuickSaveReader reader = QuickSaveReader.Create(selectedFile);

            GameInfo.mapSeed = reader.Read<int>(GameInfo.MAP_SEED);
            GameInfo.mapSize = reader.Read<int>(GameInfo.MAP_SIZE);
            GameInfo.playerTeam = reader.Read<int>("playerTeam");

            UI.document.Run("CreateLoadingScreen", "Loading: " + selectedFile);
            
            SceneManager.LoadSceneAsync("Levels/Map1");
        }

        public static string SaveGame()
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();

            string saveName = DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss");

            QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(saveName);

            quickSaveWriter.Write(GameInfo.MAP_SEED, GameInfo.mapSeed);
            quickSaveWriter.Write(GameInfo.MAP_SIZE, GameInfo.mapSize);
            quickSaveWriter.Write("playerTeam", GameInfo.playerTeam);
            quickSaveWriter.Write("cameraPosition", Camera.main.transform.position);
            quickSaveWriter.Write("cameraRotation", Camera.main.transform.rotation);

            int index = 0;
            foreach (GameObject unitObject in GameObject.FindGameObjectsWithTag("Building").Concat(GameObject.FindGameObjectsWithTag("Unit")))
            {
                BaseBehavior unitBaseBehavior = unitObject.GetComponent<BaseBehavior>();
                unitBaseBehavior.Save(ref quickSaveWriter, index);
                index++;
            }
            quickSaveWriter.Write("indexCount", index);

            quickSaveWriter.Commit();
            
            return saveName;
        }
    }
}