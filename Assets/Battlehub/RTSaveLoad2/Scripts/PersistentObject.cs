<<<<<<< HEAD
﻿using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Battlehub.RTSaveLoad2
{
   
    [ProtoContract(AsReferenceDefault = true)]    
    public class PersistentObject : PersistentSurrogate
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public int hideFlags;

        public override void ReadFrom(object obj)
        {
            UnityObject uo = (UnityObject)obj;
            name = uo.name;
            hideFlags = (int)uo.hideFlags;
        }

        public override object WriteTo(object obj)
        {
            UnityObject uo = (UnityObject)obj;
            uo.name = name;
            uo.hideFlags = (HideFlags)hideFlags;
            return obj;
        }       
=======
﻿using UnityEngine;

namespace Battlehub.RTSaveLoad2
{
    //show wizard during project import
    //wizard would ask to create persistent objects and field mapping objects
    //user could choose accept predefined set of types, 
    //selected/unselect additional types or skip generation.
    
    //case 1 - Fresh Install:
    //wizard will display unity object fields to the left
    //and matched persistent objects fields to the right
    //case 2 - Upgrade: 

    //peristent objects will use primitive types and standard .net types or persistent objects
    //to store saved data

    public class PersistentObject 
    {
        public string name;
        public uint hideFlags;
>>>>>>> bfaf5ccd... first
    }

}
