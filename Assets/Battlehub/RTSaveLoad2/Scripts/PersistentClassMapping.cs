<<<<<<< HEAD
﻿using System;
using UnityEngine;

namespace Battlehub.RTSaveLoad2
{
    [Serializable]
    public class PersistentPropertyMapping
    {
        //CodeGen will ignore this PersistentPropertyMapping if IsEnabled is false
        public bool IsEnabled;        
=======
﻿using UnityEngine;

namespace Battlehub.RTSaveLoad2
{
    [System.Serializable]
    public struct PersistentPropertyMapping
    {
        public bool IsEnabled;
>>>>>>> 604c68fa... CodeGen implementation
        public bool IsProperty;
        public int PersistentTag;

        public string PersistentFullTypeName
        {
            get { return PersistentNamespace + "." + PersistentTypeName; }
        }

        public string MappedFullTypeName
        {
            get { return MappedNamespace + "." + MappedTypeName; }
        }

<<<<<<< HEAD
=======

>>>>>>> 604c68fa... CodeGen implementation
        public string MappedAssemblyQualifiedName
        {
            get { return MappedFullTypeName + "," + MappedAssemblyName; }
        }

<<<<<<< HEAD
        public Type MappedType
        {
            get { return Type.GetType(MappedAssemblyQualifiedName); }
        }

        //Namespace, typename and persistent field name
        public string PersistentNamespace;
        public string PersistentTypeName; 
        public string PersistentName;

        //Namespace, typename and name of the property (name of the field) which is member of mapped type
=======
        public string PersistentNamespace;
        public string PersistentTypeName;
        public string PersistentName;

>>>>>>> 604c68fa... CodeGen implementation
        public string MappedAssemblyName;
        public string MappedNamespace;
        public string MappedTypeName;
        public string MappedName;
<<<<<<< HEAD

        //True if property (or field) is non-unityobject persistent class
        public bool UseSurrogate; 

        //True if property (or field) is unity object or non-unity object with dependencies
        public bool HasDependenciesOrIsDependencyItself; 
    }

    [Serializable]
    public class PersistentSubclass
    {
        public int PersistentTag;
        public string FullTypeName
        {
            get { return Namespace + "." + TypeName; }
        }
        public string Namespace;
        public string TypeName;
        public bool IsEnabled;
=======
        
        public bool UseCustomCode;
        public string BuiltInCodeSnippet;
>>>>>>> 604c68fa... CodeGen implementation
    }

    public class PersistentClassMapping : MonoBehaviour
    {
        public string MappedFullTypeName
        {
            get { return MappedNamespace + "." + MappedTypeName; }
        }

        public string PersistentFullTypeName
        {
            get { return PersistentNamespace + "." + PersistentTypeName; }
        }

        public string MappedAssemblyQualifiedName
        {
            get { return MappedFullTypeName + "," + MappedAssemblyName; }
        }

        public bool IsEnabled;
<<<<<<< HEAD
        public int PersistentPropertyTag;
        public int PersistentSubclassTag;
=======
        public int PersistentTag;
>>>>>>> 604c68fa... CodeGen implementation
        public string MappedAssemblyName;
        public string MappedNamespace;
        public string MappedTypeName;
        public string PersistentNamespace;
        public string PersistentTypeName;
<<<<<<< HEAD
        public string PersistentBaseNamespace;
        public string PersistentBaseTypeName;

        /// <summary>
        /// Array of subclasses which is used by CodeGen to generate code of TypeModelCreator
        /// </summary>
        public PersistentSubclass[] Subclasses;
=======
>>>>>>> 604c68fa... CodeGen implementation
        public PersistentPropertyMapping[] PropertyMappings;

        public static string ToPersistentNamespace(string mappedNamespace)
        {
            return mappedNamespace + ".Battlehub.SL2";
        }

        public static string ToMappedNamespace(string persistentNamespace)
        {
            return persistentNamespace.Replace(".Battlehub.SL2", "");
        }
<<<<<<< HEAD

        public static string ToPersistentName(string typeName)
        {
            return "Persistent" + typeName;
        }
=======
>>>>>>> 604c68fa... CodeGen implementation
    }

    
}


