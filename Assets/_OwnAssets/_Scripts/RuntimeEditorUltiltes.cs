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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TMPro;

/// <summary>
/// Helper Script to extend functions for the Runtime Editor
/// </summary>

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class RuntimeEditorUltiltes : MonoBehaviour
    {
        private IProjectManager m_projectManager;
        private string assetsFolderPath;

        // If functions of the script can be triggered via keypresses
        [SerializeField] private bool acceptKeyInput = false;

        [SerializeField] private KeyCode OpenAssetsFolderKey = KeyCode.Keypad1;
        [SerializeField] private KeyCode AddFilesToRTEKey = KeyCode.Keypad2;
        [SerializeField] private KeyCode resetRuntimeAssetsAndRestartKey = KeyCode.Keypad4;
        [SerializeField] private KeyCode exportAssetsKey = KeyCode.Keypad5;
        [SerializeField] private KeyCode importAssetsKey = KeyCode.Keypad6;

        [Header("List Excecuted by Order")]
        [SerializeField] private KeyCode switchRTSceneToUnityScene = KeyCode.Keypad3;
        [SerializeField] private List<GameObject> gameObjectsToSpawn;
        [SerializeField] private List<GameObject> gameObjectsToMoveToRoot;
        [SerializeField] private List<GameObject> gameObjectsToSetActive;
        [SerializeField] private List<string> gameObjectsToDestroyByName;
        [SerializeField] private List<GameObject> gameObjectsToDestroy;

        private void Start()
        {
            m_projectManager = Dependencies.ProjectManager;
            assetsFolderPath = Application.persistentDataPath + "/Assets";
        }

        private void Update()
        {
            if (!acceptKeyInput)
                return;

            if (InputController.GetKeyDown(OpenAssetsFolderKey))
                OpenRuntimeAssetsFolder();

            if (InputController.GetKeyDown(AddFilesToRTEKey))
                AddFilesToRTE();

            if (Input.GetKeyDown(switchRTSceneToUnityScene))
                SwitchRTSceneToUnityScenePopup();

            if (Input.GetKeyDown(resetRuntimeAssetsAndRestartKey))
                ResetRuntimeAssetsAndRestartPopup();

            if (Input.GetKeyDown(exportAssetsKey))
                ExportAssets();

            if (Input.GetKeyDown(importAssetsKey))
                ImportAssets();
        }

        // Exporting Runtime Assets out as a zip
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

        // Exporting Runtime Assets out as a zip
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

            if(!File.Exists(filePath))
            {
                return;
            }

            if (Directory.Exists(@Application.persistentDataPath + "/Assets"))
                Directory.Delete(@Application.persistentDataPath + "/Assets", true);
            Directory.CreateDirectory(@Application.persistentDataPath + "/Assets");

            zip.ExtractZip(filePath, assetsFolderPath, "");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Delete all files from Runtime Assets Folder and Restart Scene
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

        private void ResetRuntimeAssetsAndRestart()
        {
            if (Directory.Exists(@Application.persistentDataPath + "/Assets"))
                Directory.Delete(@Application.persistentDataPath + "/Assets", true);
            Directory.CreateDirectory(@Application.persistentDataPath + "/Assets");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        // Popup cofirmation dialog for Method "SwitchRTSceneToUnityScene"
        public void SwitchRTSceneToUnityScenePopup()
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

        // Remove and Add nessesary objects to exit the runtime editor and start the scene properly
        private void SwitchRTSceneToUnityScene()
        {
            // Spawn selected prefabs
            for (int i = 0; i < gameObjectsToSpawn.Count; i++)
            {
                if (gameObjectsToSpawn[i] != null)
                    Instantiate(gameObjectsToSpawn[i], Vector3.zero, Quaternion.identity);
            }

            // Move selected GameObjects to root hierarchy
            for (int i = 0; i < gameObjectsToMoveToRoot.Count; i++)
            {
                if (gameObjectsToMoveToRoot[i] != null)
                    gameObjectsToMoveToRoot[i].gameObject.transform.SetParent(null);
            }

            // Set selected Gameobjects to active
            for (int i = 0; i < gameObjectsToSetActive.Count; i++)
            {
                if (gameObjectsToSetActive[i] != null)
                    gameObjectsToSetActive[i].SetActive(true);
            }

            // Destroy selected GameObjects by name in hierarchy
            for (int i = 0; i < gameObjectsToDestroyByName.Count; i++)
            {
                GameObject objectToDestroy = GameObject.Find(gameObjectsToDestroyByName[i]);
                if (objectToDestroy != null)
                    Destroy(objectToDestroy);
            }

            // Destroy selected GameObjects in hierarchy
            for (int i = 0; i < gameObjectsToDestroy.Count; i++)
            {
                if (gameObjectsToDestroy[i] != null)
                    Destroy(gameObjectsToDestroy[i]);
            }

            // Delete this gameobject
            Destroy(this.gameObject);
        }

        // Open Runtime Editor resources folder
        public void OpenRuntimeAssetsFolder()
        {
            if (!Directory.Exists(@assetsFolderPath))
                Directory.CreateDirectory(@assetsFolderPath);

            System.Diagnostics.Process.Start(@assetsFolderPath);
        }

        // Add files to runtime editor
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

        // Load textures from file path
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

        // Load meshes from file path
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

        // Load PNG from file
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
    }
}
