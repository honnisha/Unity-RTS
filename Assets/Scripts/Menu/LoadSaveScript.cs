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

        public static string loadLevel = "";
        public static float loadLevelTimer = 0.0f;

        public static void UpdateSaveList()
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
            
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low";
            savePath = Path.Combine(savePath, Application.companyName);
            savePath = Path.Combine(savePath, Application.productName);
            savePath = Path.Combine(savePath, "QuickSave");
            savePath = Path.Combine(savePath, selectedFile + ".json");
            File.Delete(savePath);
            UpdateSaveList();
        }

        public static void LoadFileSettings()
        {
            if (selectedFile == "")
                return;

            QuickSaveReader reader = QuickSaveReader.Create(selectedFile);

            GameInfo.mapSeed = reader.Read<int>(GameInfo.MAP_SEED);
            GameInfo.mapSize = reader.Read<int>(GameInfo.MAP_SIZE);
            GameInfo.playerTeam = reader.Read<int>("playerTeam");
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

            quickSaveWriter.Write("gold", cameraController.gold);
            quickSaveWriter.Write("wood", cameraController.wood);
            quickSaveWriter.Write("food", cameraController.food);

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

        public static void RestoreGame()
        {
            QuickSaveReader saveReader = QuickSaveReader.Create(LoadSaveScript.selectedFile);

            int indexCount = saveReader.Read<int>("indexCount");

            for (int index = 0; index < indexCount; index++)
                BaseBehavior.Load(ref saveReader, index);

            foreach (GameObject unitObject in GameObject.FindGameObjectsWithTag("Building").Concat(GameObject.FindGameObjectsWithTag("Unit")))
            {
                BaseBehavior unitBaseBehavior = unitObject.GetComponent<BaseBehavior>();
                unitBaseBehavior.RestoreBehavior();
            }

            Vector3 cameraPosition = saveReader.Read<Vector3>("cameraPosition");
            Quaternion cameraRotation = saveReader.Read<Quaternion>("cameraRotation");
            Camera.main.transform.position = cameraPosition;
            Camera.main.transform.rotation = cameraRotation;

            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.gold = saveReader.Read<float>("gold");
            cameraController.wood = saveReader.Read<float>("wood");
            cameraController.food = saveReader.Read<float>("food");

            selectedFile = "";
        }
    }
}