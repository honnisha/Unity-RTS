using CI.QuickSave;
using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            quickSaveWriter.Write("gold", cameraController.resources[BaseBehavior.ResourceType.Gold]);
            quickSaveWriter.Write("wood", cameraController.resources[BaseBehavior.ResourceType.Wood]);
            quickSaveWriter.Write("food", cameraController.resources[BaseBehavior.ResourceType.Food]);
            quickSaveWriter.Write("favor", cameraController.resources[BaseBehavior.ResourceType.Favor]);

            int index = 0;
            foreach (GameObject unitObject in GameObject.FindGameObjectsWithTag("Building").Concat(GameObject.FindGameObjectsWithTag("Unit")))
            {
                BaseBehavior unitBaseBehavior = unitObject.GetComponent<BaseBehavior>();
                unitBaseBehavior.Save(ref quickSaveWriter, index);
                index++;
            }
            quickSaveWriter.Write("indexCount", index);

            TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();

            // Blind texture
            Color[] colors = terrainGenerator.blindTexture2D.GetPixels();
            int[] blindInfo = new int[colors.Length];
            for (int i = 0; i < colors.Length; i++)
                blindInfo[i] = colors[i] == Color.black ? 1 : 0;
            quickSaveWriter.Write("blindTextureData", blindInfo);

            // Binds
            for (int number = 1; number <= 9; number++)
            {
                int[] binds = new int[cameraController.unitsBinds[KeyCode.Alpha0 + number].Count];
                int i = 0;
                foreach(GameObject bindObject in cameraController.unitsBinds[KeyCode.Alpha0 + number])
                {
                    BaseBehavior bindObjectBaseBehavior = bindObject.GetComponent<BaseBehavior>();
                    binds[i] = bindObjectBaseBehavior.uniqueId;
                    i++;
                }
                quickSaveWriter.Write(new StringBuilder(15).AppendFormat("{0}_{1}", number, "bind").ToString(), binds);
            }

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
            cameraController.resources[BaseBehavior.ResourceType.Gold] = saveReader.Read<float>("gold");
            cameraController.resources[BaseBehavior.ResourceType.Wood] = saveReader.Read<float>("wood");
            cameraController.resources[BaseBehavior.ResourceType.Food] = saveReader.Read<float>("food");
            cameraController.resources[BaseBehavior.ResourceType.Favor] = saveReader.Read<float>("favor");
          
            // Blind texture
            TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();
            Color[] colors = terrainGenerator.blindTexture2D.GetPixels();
            int[] blindInfo = saveReader.Read<int[]>("blindTextureData");
            Color newColor = Color.white;
            newColor.a = 0;
            for (int i = 0; i < colors.Length; i++)
                colors[i] = blindInfo[i] == 1 ? Color.black : newColor;
            terrainGenerator.blindTexture2D.SetPixels(colors);
            terrainGenerator.blindTexture2D.Apply();
            Graphics.Blit(terrainGenerator.blindTexture2D, terrainGenerator.blindTexture);
            terrainGenerator.initBlindTexture = true;

            try
            {
                // Binds
                for (int number = 1; number <= 9; number++)
                {
                    int[] binds = saveReader.Read<int[]>(new StringBuilder(15).AppendFormat("{0}_{1}", number, "bind").ToString());
                    int i = 0;
                    foreach (int bindObjectUniueId in binds)
                    {
                        cameraController.unitsBinds[KeyCode.Alpha0 + number].Add(BaseBehavior.GetObjectByUniqueId(bindObjectUniueId));
                        i++;
                    }
                }
            }
            catch (Exception e)
            { }

            selectedFile = "";
        }
    }
}