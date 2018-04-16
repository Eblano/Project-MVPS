﻿using UnityEngine;

namespace Battlehub.RTSaveLoad
{
    public static class Dependencies 
    {
        public static ISerializer Serializer
        {
            get { return new Serializer(); }
        }
        public static IStorage Storage
        {
            get { return new FileSystemStorage(Application.persistentDataPath); }
        }
        public static IProject Project
        {
            get { return new Project(); }
        }
        public static IAssetBundleLoader BundleLoader
        {
            get { return new AssetBundleLoader(); }
        }

        public static IProjectManager ProjectManager
        {
            get { return Object.FindObjectOfType<ProjectManager>(); }
        }

        public static ISceneManager SceneManager
        {
            get { return Object.FindObjectOfType<RuntimeSceneManager>(); }
        }

        public static IRuntimeShaderUtil ShaderUtil
        {
            get { return new RuntimeShaderUtil(); }
        }

        public static IJob Job
        {
            get
            {
                Job job = Object.FindObjectOfType<Job>();
                if (job == null)
                {
                    GameObject go = new GameObject();
                    go.name = "Job";
                    job = go.AddComponent<Job>();
                    go.AddComponent<PersistentIgnore>();
                }
                return job;
            }
        }

    }
}

