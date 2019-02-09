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
        private static Dom.HTMLCollection mapBlocks = null;
        public static void UpdateMaps()
        {
            mapBlocks = UI.document.getElementsByClassName("mapBlock");
        }

        static HtmlElement mapImage;
        static HtmlElement blindFog;
        static HtmlElement unitsBlock;
        static Dom.Element unitDiv;

        static Dictionary<GameObject, List<Dom.Element>> cacheUnitDivs = new Dictionary<GameObject, List<Dom.Element>>();
        public static void CreateOrUpdateMaps(ref Dictionary<HtmlElement, HtmlElement> mapCache, bool update = false)
        {
            if (update)
            {
                UpdateMaps();
                
                cacheUnitDivs.Clear();
            }
            mapCache.Clear();

            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            int index = 0;
            foreach (var mapBlock in mapBlocks)
            {
                // Draw map
                if (cameraController.terrainGenerator != null)
                {
                    if (((HtmlElement)mapBlock).image == null && cameraController.terrainGenerator.mapTexture != null)
                    {
                        ((HtmlElement)mapBlock).image = cameraController.terrainGenerator.mapTexture;
                        mapBlock.style.height = "100%";
                        mapBlock.style.width = "100%";
                    }
                }
                else
                {
                    mapBlock.style.backgroundImage = String.Format("{0}.png", SceneManager.GetActiveScene().name);
                }

                mapImage = (HtmlElement)mapBlock.getElementsByClassName("mapVision")[0];
                mapCache.Add((HtmlElement)mapBlock, mapImage);

                if (mapImage.image == null && cameraController.terrainGenerator.mapTexture != null && cameraController.mapTexture.height > 0)
                {
                    mapImage.image = cameraController.mapTexture;
                    mapImage.style.height = "100%";
                    mapImage.style.width = "100%";
                }

                blindFog = (HtmlElement)mapBlock.getElementsByClassName("blindFog")[0];
                if (blindFog.image == null && cameraController.terrainGenerator.blindTexture2D != null && cameraController.terrainGenerator.blindTexture2D.height > 0)
                {
                    blindFog.image = cameraController.terrainGenerator.blindTexture;
                    blindFog.style.height = "100%";
                    blindFog.style.width = "100%";
                }

                unitsBlock = (HtmlElement)mapBlock.getElementsByClassName("units")[0];
                if (update)
                    unitsBlock.innerHTML = "";

                // Draw units + calculate statistic
                foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
                {
                    if (unit.GetComponent<BaseBehavior>().IsDisplayOnMap() && IsInTerrainRange(unit.transform.position))
                    {
                        if (!update && cacheUnitDivs.ContainsKey(unit) && cacheUnitDivs[unit].Count > index)
                        {
                            unitDiv = cacheUnitDivs[unit][index];
                        }
                        else
                        {
                            unitDiv = UI.document.createElement("div");
                            unitDiv.className = "unit clckable";
                            // unitDiv.id = unit.GetComponent<PhotonView>().ViewID.ToString();
                            unitsBlock.appendChild(unitDiv);

                            if (!cacheUnitDivs.ContainsKey(unit))
                                cacheUnitDivs[unit] = new List<Dom.Element>();

                            cacheUnitDivs[unit].Add(unitDiv);
                        }

                        Vector2 positionOnMap = GetPositionOnMap(unit.transform.position);
                        unitDiv.style.left = new StringBuilder(5).AppendFormat("{0}%", positionOnMap.x * 100.0f - 1.5).ToString();
                        unitDiv.style.bottom = new StringBuilder(5).AppendFormat("{0}%", positionOnMap.y * 100.0f - 1.5).ToString();
                        unitDiv.style.backgroundColor = unit.GetComponent<BaseBehavior>().GetDisplayColor();
                    }
                    else if (cacheUnitDivs.ContainsKey(unit) && cacheUnitDivs[unit].Count > index)
                    {
                        unitDiv = cacheUnitDivs[unit][index];
                        unitsBlock.removeChild(unitDiv);
                    }
                }
                index++;
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