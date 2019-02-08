using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISpace;
using PowerUI;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Text;

namespace GangaGame
{
    public static class MapScript
    {
        public static void CreateOrUpdateMaps(ref Dictionary<HtmlElement, HtmlElement> mapCache)
        {
            mapCache.Clear();
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();

            foreach (var mapBlock in UI.document.getElementsByClassName("mapBlock"))
            {
                // Draw map
                if (terrainGenerator != null)
                {
                    if (((HtmlElement)mapBlock).image == null && terrainGenerator.mapTexture != null)
                    {
                        ((HtmlElement)mapBlock).image = terrainGenerator.mapTexture;
                        mapBlock.style.height = "100%";
                        mapBlock.style.width = "100%";
                    }
                }
                else
                {
                    mapBlock.style.backgroundImage = String.Format("{0}.png", SceneManager.GetActiveScene().name);
                }
                var mapImage = (HtmlElement)mapBlock.getElementsByClassName("mapVision")[0];
                mapCache.Add((HtmlElement)mapBlock, mapImage);

                if (mapImage.image == null && terrainGenerator.mapTexture != null && cameraController.mapTexture.height > 0)
                {
                    mapImage.image = cameraController.mapTexture;
                    mapImage.style.height = "100%";
                    mapImage.style.width = "100%";
                }

                var blindFog = (HtmlElement)mapBlock.getElementsByClassName("blindFog")[0];
                if (blindFog.image == null && terrainGenerator.blindTexture2D != null && terrainGenerator.blindTexture2D.height > 0)
                {
                    blindFog.image = terrainGenerator.blindTexture;
                    blindFog.style.height = "100%";
                    blindFog.style.width = "100%";
                }

                var unitsBlock = (HtmlElement)mapBlock.getElementsByClassName("units")[0];
                unitsBlock.innerHTML = "";
                
                // Draw units + calculate statistic
                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                {
                    BaseBehavior unitBaseBehavior = unit.GetComponent<BaseBehavior>();
                    if (unitBaseBehavior.IsDisplayOnMap() && MapScript.IsInTerrainRange(unit.transform.position))
                    {
                        Dom.Element unitDiv = UI.document.createElement("div");
                        unitDiv.className = "unit clckable";
                        unitDiv.id = unit.GetComponent<PhotonView>().ViewID.ToString();
                        Vector2 positionOnMap = MapScript.GetPositionOnMap(unit.transform.position);
                        unitDiv.style.left = String.Format("{0}%", positionOnMap.x * 100.0f - 1.5);
                        unitDiv.style.bottom = String.Format("{0}%", positionOnMap.y * 100.0f - 1.5);
                        unitDiv.style.backgroundColor = unitBaseBehavior.GetDisplayColor();
                        unitsBlock.appendChild(unitDiv);
                    }
                }
            }
        }

        public static Vector3 mapPointToPosition(Vector2 mapPosition)
        {
            Terrain terrain = Terrain.activeTerrain;
            return new Vector3(terrain.terrainData.size.x * mapPosition.x, 0, terrain.terrainData.size.z - terrain.terrainData.size.z * mapPosition.y);
        }

        public static void MoveCameraToPoint(Vector3 position)
        {

            Camera.main.transform.position = new Vector3(position.x, Camera.main.transform.position.y, position.z);
            Camera.main.transform.position += new Vector3(Camera.main.transform.forward.normalized.x, 0, Camera.main.transform.forward.normalized.z) * CameraController.GetCameraOffset();
        }

        public static Vector2 GetPositionOnMap(Vector3 position)
        {
            Terrain terrain = Terrain.activeTerrain;
            Vector3 positionOnTerrain = position - terrain.GetPosition();
            return new Vector2(positionOnTerrain.x / terrain.terrainData.size.x, positionOnTerrain.z / terrain.terrainData.size.z);
        }

        public static bool IsInTerrainRange(Vector3 target)
        {
            if (target.x > 0.0f && target.x <= Terrain.activeTerrain.terrainData.size.x)
                if (target.z > 0.0f && target.z <= Terrain.activeTerrain.terrainData.size.z)
                    return true;

            return false;
        }

        static Dom.Element cameraDiv = null;
        public static void CreateOrUpdateCameraOnMap(Dom.Element mapBlock, Dom.Element mapImage)
        {
            //var blindFog = (HtmlElement)mapBlock.getElementsByClassName("blindFog")[0];
            TerrainGenerator terrainGenerator = Terrain.activeTerrain.GetComponent<TerrainGenerator>();
            //((HtmlElement)blindFog).image = terrainGenerator.blindTexture2D;

            UnityEngine.Profiling.Profiler.BeginSample("p CreateOrUpdateCameraOnMap"); // Profiler
            Vector3 vameraLookAt = Camera.main.transform.position - new Vector3(Camera.main.transform.forward.normalized.x, 0, Camera.main.transform.forward.normalized.z) * CameraController.GetCameraOffset();
            bool isCameraInTerrain = IsInTerrainRange(vameraLookAt);
            float cameraScale = 20.0f / (Terrain.activeTerrain.terrainData.size.x / 50.0f / 4.0f);
            float cameraHeight = cameraScale / 1.2f;
            float cameraWidtht = cameraScale;
            Vector2 positionCameraOnMap = GetPositionOnMap(vameraLookAt);
            cameraDiv = null;
            if (isCameraInTerrain)
            {
                if (mapBlock.getElementsByClassName("mapCamera").length <= 0)
                {
                    cameraDiv = UI.document.createElement("div");
                    cameraDiv.className = "mapCamera clckable";
                    cameraDiv.style.height = new StringBuilder(7).AppendFormat("{0}%", cameraHeight).ToString();
                    cameraDiv.style.width = new StringBuilder(7).AppendFormat("{0}%", cameraWidtht).ToString();
                    mapBlock.appendChild(cameraDiv);
                }
                else
                    cameraDiv = mapBlock.getElementsByClassName("mapCamera")[0];
                
                if (cameraDiv != null)
                {
                    cameraDiv.style.left = new StringBuilder(7).AppendFormat("{0}%", positionCameraOnMap.x * 100.0f).ToString();
                    cameraDiv.style.bottom = new StringBuilder(7).AppendFormat("{0}%", positionCameraOnMap.y * 100.0f).ToString();
                    cameraDiv.style.transform = new StringBuilder(30).AppendFormat("rotate({0}deg) translate(-50%,-50%)", Camera.main.transform.rotation.eulerAngles.y).ToString();
                }
            }
            else if (mapBlock.getElementsByClassName("mapCamera").length <= 0)
            {
                cameraDiv = mapBlock.getElementsByClassName("mapCamera")[0];
                cameraDiv.remove();
            }
            UnityEngine.Profiling.Profiler.EndSample(); // Profiler
        }

        public static void MapEvent(HtmlDivElement element)
        {
            if (!UnityEngine.Input.GetKey(KeyCode.LeftAlt))
            {
                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                
                var elementPos = new Vector2(element.getBoundingClientRect().X, element.getBoundingClientRect().Y);
                var mousePos = InputPointer.All[0].Position;
                var mapPoint = (mousePos - elementPos) / new Vector2(element.getBoundingClientRect().Width, element.getBoundingClientRect().Height);
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    MoveCameraToPoint(mapPointToPosition(mapPoint));
                }
                else if (UnityEngine.Input.GetMouseButton(1))
                {
                    foreach (var unit in cameraController.selectedObjects)
                    {
                        unit.GetComponent<BaseBehavior>().GiveOrder(mapPointToPosition(mapPoint), true, true);
                    }
                }
            }
        }
    }
}