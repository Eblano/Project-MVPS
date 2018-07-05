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

        // If functions of the script can be triggered via keypresses
        [SerializeField] private bool acceptKeyInput = false;

        [SerializeField] private KeyCode openAssetsFolderKey = KeyCode.Keypad1;
        [SerializeField] private KeyCode addFilesToRTEKey = KeyCode.Keypad2;
        [SerializeField] private KeyCode resetRuntimeAssetsAndRestartKey = KeyCode.Keypad4;
        [SerializeField] private KeyCode exportAssetsKey = KeyCode.Keypad5;
        [SerializeField] private KeyCode importAssetsKey = KeyCode.Keypad6;

        [Header("List Excecuted by Order")]
        [SerializeField] private readonly KeyCode removeGameObjectsKey = KeyCode.Keypad3;
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
        [SerializeField] private Material[] skyboxMats;

        [Header("MarkerUI Camera Properties")]
        [Battlehub.SerializeIgnore] [SerializeField] private GameObject markerUICamera_Prefab;
        private Camera markerUICamera;
        private Camera camToFollow;
        private bool fixedEditorCamViewport = false;

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

            if (acceptKeyInput)
            {
                if (InputController.GetKeyDown(openAssetsFolderKey))
                    OpenRuntimeAssetsFolder();

                if (InputController.GetKeyDown(addFilesToRTEKey))
                    AddFilesToRTE();

                if (Input.GetKeyDown(removeGameObjectsKey))
                    SwitchRTSceneToUnityScenePopup();

                if (Input.GetKeyDown(resetRuntimeAssetsAndRestartKey))
                    ResetRuntimeAssetsAndRestartPopup();

                if (Input.GetKeyDown(exportAssetsKey))
                    ExportAssets();

                if (Input.GetKeyDown(importAssetsKey))
                    ImportAssets();
            }
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

            string filePath = StandaloneFileBrowser.OpenFilePanel("Import Assets", "", extensions, false)[0];

            if (!File.Exists(filePath))
            {
                return;
            }

            if (Directory.Exists(@Application.persistentDataPath + "/Assets"))
                Directory.Delete(@Application.persistentDataPath + "/Assets", true);
            Directory.CreateDirectory(@Application.persistentDataPath + "/Assets");

            zip.ExtractZip(filePath, assetsFolderPath, "");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Popup cofirmation dialog for Method "SwitchRTSceneToUnityScene"
        /// </summary>
        public void OpenNPCScriptEditorPopup()
        {
            if (!NpcScriptStorage.instance)
            {
                PopupWindow.Show("Error", "Please add NPCScriptStorage onto the Scene", "Ok");
            }
            else
            {
                RTEScriptEditor.instance.ShowNPCScriptingUI();
            }
        }

        /// <summary>
        /// Popup cofirmation dialog for Method "SwitchRTSceneToUnityScene"
        /// </summary>
        public void SwitchRTSceneToUnityScenePopup()
        {
            if (!saveSceneButton.IsInteractable())
            {
                if(NpcScriptStorage.instance)
                {
                    RTEScriptEditor.instance.ShowNPCScriptingUI();
                    RTEScriptEditor.instance.HideNPCScriptingUI();
                }
                else
                {
                    PopupWindow.Show("Error", "Please add NPCScriptStorage onto the Scene", "Ok");
                    return;
                }

                if (RTEScriptEditor.instance.DataIsComplete())
                {
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
                else
                    PopupWindow.Show("Error", "NPC Script Editor has missing links, please resolve.", "Ok");
            }
            else
                PopupWindow.Show("Error", "Cannot start scene because scene is not saved.", "Ok");
        }

        /// <summary>
        /// Remove and Add nessesary objects to exit the runtime editor and start the scene properly
        /// </summary>
        public void SwitchRTSceneToUnityScene()
        {
            GameManager.instance.SetSceneInfo(m_projectManager.ActiveScene.Name, GetActiveSceneHash());

            // Destroy selected GameObjects by name in hierarchy
            for (int i = 0; i < gameObjectsToDestroyByName.Count; i++)
            {
                GameObject objectToDestroy = GameObject.Find(gameObjectsToDestroyByName[i]);
                if (objectToDestroy != null)
                {
                    Destroy(objectToDestroy);
                }
            }

            // Spawn selected GameObjects
            for (int i = 0; i < gameObjectsToSpawn.Count; i++)
            {
                GameObject go = Instantiate(gameObjectsToSpawn[i], Vector3.zero, Quaternion.identity);
                go.transform.SetParent(null);
            }

            markerUICamera.gameObject.SetActive(true);
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

            if(hash != null)
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

            if(m_projectManager.ActiveScene != null && m_projectManager.ActiveScene.Name != "New Scene" && !saveSceneButton.interactable)
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

        public Material[] GetSkyboxMats()
        {
            return skyboxMats;
        }

        public void ToggleMarkerVisibility()
        {
            markerUICamera.gameObject.SetActive(!markerUICamera.gameObject.activeInHierarchy);
        }
    }
}
