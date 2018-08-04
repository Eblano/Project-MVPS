using Battlehub.RTCommon;
using Battlehub.RTSaveLoad;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System.IO;
using SFB;
using UnityEngine.SceneManagement;
using ICSharpCode.SharpZipLib.Zip;
using System;
using Battlehub.UIControls;
using TMPro;
using UnityEngine.UI;

namespace SealTeam4
{
    /// <summary>
    /// Helper Script to extend functions for the Runtime Editor
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class RuntimeEditorUltiltes : MonoBehaviour
    {
        public static RuntimeEditorUltiltes instance;
        private IProjectManager m_projectManager;
        private string assetsFolderPath;

        [SerializeField] private List<string> gameObjectsToDestroyByName;
        [SerializeField] private List<GameObject> gameObjectsToSpawn;

        [Space(5)]

        [Header("Scene Hash Properties")]
        [SerializeField] private Button saveSceneButton;
        [SerializeField] private TextMeshProUGUI sceneHashText;
        [SerializeField] private float sceneHash_refreshRate = 3f;
        private float sceneHash_timeLeftToRefresh;

        [Space(5)]

        [Header("SceneMask")]
        [SerializeField] private GameObject mask;
        [SerializeField] private TextMeshProUGUI maskSceneNameTxt;
        [SerializeField] private TextMeshProUGUI maskSceneHashTxt;

        [Space(5)]

        [Header("GameObjects for RuntimeEditor Prefabs")]
        [SerializeField] private GameObject markerFloatingText_Prefab;
        [SerializeField] private Mesh defCubeMesh_mesh;
        [SerializeField] private Material[] skyboxMats;

        [Header("MarkerUI Camera Properties")]
        [Battlehub.SerializeIgnore] [SerializeField] private GameObject markerUICamera_Prefab;
        private Camera markerUICamera;
        private Camera camToFollow;
        private bool fixedEditorCamViewport = false;

        private bool hideWallCeilling = false;
        private bool hideMarkers = false;

        private void Awake()
        {
            DynamicGI.UpdateEnvironment();
        }

        private void Start()
        {
            if (!instance)
            {
                instance = this;
            }
            else
            {
                Debug.Log("There is already runtime editor utilities in this scene");
                Destroy(this);
            }

            m_projectManager = Dependencies.ProjectManager;
            assetsFolderPath = Application.persistentDataPath + "/Assets";
        }

        private void Update()
        {
            UpdateMarkerUICameraTransform();
            ProcessSceneHash();
        }

        private void ProcessSceneHash()
        {
            if (sceneHash_timeLeftToRefresh <= 0)
            {
                sceneHash_timeLeftToRefresh = sceneHash_refreshRate;
                string hash = GetActiveSceneHash();

                if (hash != null)
                    sceneHashText.text = hash;
                else
                    sceneHashText.text = " - ";
            }
            else
                sceneHash_timeLeftToRefresh -= Time.deltaTime;
        }

        private void UpdateMarkerUICameraTransform()
        {
            if (!camToFollow)
            {
                camToFollow = GameObject.Find("Editor Camera").GetComponent<Camera>();
            }
            else
            {
                if (!markerUICamera)
                {
                    markerUICamera = Instantiate(markerUICamera_Prefab, Vector3.zero, Quaternion.identity).GetComponent<Camera>();
                }
                else
                {
                    camToFollow.rect = new Rect(0, 0, 1, 1);
                    markerUICamera.rect = new Rect(0, 0, 1, 1);
                    markerUICamera.orthographicSize = camToFollow.orthographicSize;
                    markerUICamera.orthographic = camToFollow.orthographic;
                    markerUICamera.transform.position = camToFollow.transform.position;
                    markerUICamera.transform.rotation = camToFollow.transform.rotation;
                }
            }
        }

        /// <summary>
        /// Exporting Runtime Assets out as a zip
        /// </summary>
        public void ExportAssets()
        {
            FastZip zip = new FastZip
            {
                CreateEmptyDirectories = true
            };

            // Set extension filer for file browser
            ExtensionFilter[] extensions =
                new[] {
            new ExtensionFilter("RTA File", "rta")
                };

            string savePath = StandaloneFileBrowser.SaveFilePanel(
                                "Export Assets",
                                "",
                                "ProjectExport_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"),
                                extensions);

            if (savePath == "")
            {
                return;
            }

            try
            {
                zip.CreateZip(savePath, assetsFolderPath, true, "");
            }
            catch (Exception e)
            {
                System.IO.File.WriteAllText(Application.persistentDataPath + "\\LOG.txt", e.Message);
                Application.Quit();
            }
        }

        /// <summary>
        /// Exporting Runtime Assets out as a zip
        /// </summary>
        public void ImportAssets()
        {
            FastZip zip = new FastZip
            {
                CreateEmptyDirectories = true
            };

            // Set extension filer for file browser
            ExtensionFilter[] extensions =
                new[] {
            new ExtensionFilter("RTA File", "rta")
                };

            string[] filePaths = StandaloneFileBrowser.OpenFilePanel("Import Assets", "", extensions, false);
            string filePath = "";

            if (filePaths.Length > 0)
                filePath = filePaths[0];

            if (!File.Exists(filePath))
            {
                return;
            }

            if (Directory.Exists(@Application.persistentDataPath + "/Assets"))
                Directory.Delete(@Application.persistentDataPath + "/Assets", true);
            Directory.CreateDirectory(@Application.persistentDataPath + "/Assets");

            zip.ExtractZip(filePath, assetsFolderPath, "");
            GameManager.instance.RestartScene();
        }

        /// <summary>
        /// Delete all files from Runtime Assets Folder and Restart Scene
        /// </summary>
        public void ResetRuntimeAssetsAndRestartPopup()
        {
            PopupWindow.Show("Confirmation", "Reset Project? This action cannot be Undone.",
                "Yes",
                args =>
                {
                    if (!args.Cancel)
                    {
                        ResetRuntimeAssetsAndRestart();
                    }
                },
                "Cancel"
                );
        }

        /// <summary>
        /// Wipe all data in Runtime Assets Folder and Restart
        /// </summary>
        private void ResetRuntimeAssetsAndRestart()
        {
            if (Directory.Exists(@Application.persistentDataPath + "/Assets"))
                Directory.Delete(@Application.persistentDataPath + "/Assets", true);
            Directory.CreateDirectory(@Application.persistentDataPath + "/Assets");

            GameManager.instance.RestartScene();
        }

        /// <summary>
        /// Popup cofirmation dialog for Method "SwitchRTSceneToUnityScene"
        /// </summary>
        public void OpenNPCScriptEditorPopup()
        {
            if (!ScriptStorage.instance)
            {
                PopupWindow.Show("Error", "Please add ScriptStorage onto the Scene", "Ok");
            }
            else
            {
                RTEScriptEditor.instance.ShowScriptEditorUI();
            }
        }

        /// <summary>
        /// Popup cofirmation dialog for Method "SwitchRTSceneToUnityScene"
        /// </summary>
        public void SwitchRTSceneToUnityScenePopup()
        {
            // If scene is not saved
            if (saveSceneButton.IsInteractable())
            {
                PopupWindow.Show("Error", "Cannot start scene because scene is not saved.", "Ok");
                return;
            }

            // If there is no Script Storage on scene
            if (!ScriptStorage.instance)
            {
                PopupWindow.Show("Error", "Please add NPCScriptStorage onto the Scene", "Ok");
                return;
            }

            // If there is no PlayerSpawnMarker on scene
            if (!PlayerSpawnMarker.instance)
            {
                PopupWindow.Show("Error", "Please add PlayerSpawnMarker onto the Scene", "Ok");
                return;
            }

            // Open and close ScriptEditorUI to run checks
            RTEScriptEditor.instance.ShowScriptEditorUI();
            RTEScriptEditor.instance.HideScriptEditorUI();

            // If Script editor data is complete
            if (!RTEScriptEditor.instance.DataIsComplete())
            {
                PopupWindow.Show("Error", "NPC Script Editor has missing links, please resolve.", "Ok");
                return;
            }

            // If all point markers are pointing on ground
            if (!GameManager.instance.AllPointMarkersOnPoint())
            {
                PopupWindow.Show("Error", "One or more Markers are not pointing on any Ground, please resolve.", "Ok");
                return;
            }

            PopupWindow.Show("Confirmation", "Start Currently Loaded Scene?",
                "Yes",
                args =>
                {
                    if (!args.Cancel)
                    {
                        SwitchRTSceneToUnityScene();
                    }
                },
                "Cancel"
                );
        }

        /// <summary>
        /// Remove and Add nessesary objects to exit the runtime editor and start the scene properly
        /// </summary>
        public void SwitchRTSceneToUnityScene()
        {
            GameManager.instance.SetSceneInfo(m_projectManager.ActiveScene.Name, GetActiveSceneHash());

            // Spawn selected GameObjects
            for (int i = 0; i < gameObjectsToSpawn.Count; i++)
            {
                GameObject go = Instantiate(gameObjectsToSpawn[i], Vector3.zero, Quaternion.identity);
                go.transform.SetParent(null);
            }

            // Destroy selected GameObjects by name in hierarchy
            for (int i = 0; i < gameObjectsToDestroyByName.Count; i++)
            {
                GameObject objectToDestroy = GameObject.Find(gameObjectsToDestroyByName[i]);
                if (objectToDestroy != null)
                {
                    Destroy(objectToDestroy);
                }
            }
        }

        /// <summary>
        /// Open Runtime Editor resources folder
        /// </summary>
        public void OpenRuntimeAssetsFolder()
        {
            if (!Directory.Exists(@assetsFolderPath))
                Directory.CreateDirectory(@assetsFolderPath);

            System.Diagnostics.Process.Start(@assetsFolderPath);
        }

        /// <summary>
        /// Add files to runtime editor
        /// </summary>
        public void AddFilesToRTE()
        {
            // Set extension filer for file browser
            ExtensionFilter[] extensions =
                new[] {
            new ExtensionFilter("PNG & OBJ Files", "png", "obj")
                };

            // Open file browser and get a list of file names to be imported
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

            // Importing of various files
            List<string> objFilePath = new List<string>();
            List<string> textureFilePaths = new List<string>();

            foreach (string path in paths)
            {
                if (path.EndsWith(".obj"))
                    objFilePath.Add(path);

                if (path.EndsWith(".png"))
                    textureFilePaths.Add(path);
            }

            if (objFilePath.Count > 0)
                LoadAndSaveMeshes(objFilePath);

            if (textureFilePaths.Count > 0)
                LoadAndSaveTextures(textureFilePaths);
        }

        /// <summary>
        /// Load textures from file path
        /// </summary>
        /// <param name="textureFilePaths"></param>
        private void LoadAndSaveTextures(List<string> textureFilePaths)
        {
            ProjectItem rootFolder = m_projectManager.Project;
            List<UnityObject> objects = new List<UnityObject>();

            for (int i = 0; i < textureFilePaths.Count; i++)
            {
                Texture2D texture2D = LoadPNG(textureFilePaths[i]);
                if (texture2D == null)
                {
                    Debug.LogErrorFormat("File {0} not found", textureFilePaths[i]);
                    return;
                }
                texture2D.name = textureFilePaths[i].Substring(textureFilePaths[i].LastIndexOf('\\') + 1);
                objects.Add(texture2D);
            }

            m_projectManager.AddDynamicResources(rootFolder, objects.ToArray(), addedItems =>
            {
                Debug.Log(addedItems[0].ToString() + " added");
            });
        }

        /// <summary>
        /// Load meshes from file path
        /// </summary>
        /// <param name="filenames"></param>
        private void LoadAndSaveMeshes(List<string> filenames)
        {
            ObjImporter objImporter = new ObjImporter();
            List<Mesh> meshList = new List<Mesh>();

            foreach (string filename in filenames)
            {
                Mesh newMesh = objImporter.ImportFile(filename);
                meshList.Add(newMesh);
                newMesh.name = filename.Substring(filename.LastIndexOf('\\') + 1);
            }

            ProjectItem rootFolder = m_projectManager.Project;
            List<UnityObject> objects = new List<UnityObject>();

            foreach (Mesh mesh in meshList)
                objects.Add(mesh);

            m_projectManager.AddDynamicResources(rootFolder, objects.ToArray(), addedItems =>
            {
                for (int i = 0; i < addedItems.Length; ++i)
                    Debug.Log(addedItems[i].ToString() + " added");
            });
        }

        /// <summary>
        /// Load PNG from file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }

        public void MaskEditorUIAndUpdateSceneInfo()
        {
            mask.SetActive(true);

            string hash = GetActiveSceneHash();

            if (hash != null)
            {
                maskSceneHashTxt.text = hash;
            }
            else
            {
                maskSceneHashTxt.text = " - ";
            }

            maskSceneNameTxt.text = m_projectManager.ActiveScene.Name;
        }

        public string GetActiveSceneHash()
        {
            ProjectItem currRuntimeScene = m_projectManager.ActiveScene;

            if (m_projectManager.ActiveScene != null && m_projectManager.ActiveScene.Name != "New Scene" && !saveSceneButton.interactable)
            {
                string filePathToScene = Application.persistentDataPath + "/" + currRuntimeScene.Parent + "/" + currRuntimeScene.NameExt;

                string hashText = "";
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = File.OpenRead(filePathToScene))
                    {
                        var hash = md5.ComputeHash(stream);
                        hashText = String.Format("{0:X}", BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant().GetHashCode());
                    }
                }
                return hashText;
            }
            else
            {
                return null;
            }
        }

        public GameObject GetMarkerFloatingtTextPrefab()
        {
            return markerFloatingText_Prefab;
        }

        public Mesh GetDefCubeMesh()
        {
            return defCubeMesh_mesh;
        }

        public Material[] GetSkyboxMats()
        {
            return skyboxMats;
        }

        public void ToggleMarkerVisibility()
        {

            if (hideMarkers)
            {
                markerUICamera.cullingMask =
                    markerUICamera.cullingMask |
                    (1 << LayerMask.NameToLayer("Marker")) |
                    (1 << LayerMask.NameToLayer("AreaMarker")) |
                    (1 << LayerMask.NameToLayer("FloatingUI"));
            }
            else
            {
                markerUICamera.cullingMask =
                    markerUICamera.cullingMask &
                    ~(1 << LayerMask.NameToLayer("Marker")) &
                    ~(1 << LayerMask.NameToLayer("AreaMarker")) &
                    ~(1 << LayerMask.NameToLayer("FloatingUI"));
            }

            hideMarkers = !hideMarkers;
        }

        public void ToggleWallsCeillingVisibility()
        {
            if (hideWallCeilling)
                camToFollow.cullingMask = camToFollow.cullingMask | (1 << LayerMask.NameToLayer("Walls")) | (1 << LayerMask.NameToLayer("Ceilling"));
            else
                camToFollow.cullingMask = camToFollow.cullingMask & ~(1 << LayerMask.NameToLayer("Walls")) & ~(1 << LayerMask.NameToLayer("Ceilling"));

            hideWallCeilling = !hideWallCeilling;
        }

        public void ReloadScene()
        {
            GameManager.instance.RestartScene();
        }
    }
}
