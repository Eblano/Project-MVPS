using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSaveLoad2;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract(AsReferenceDefault = true)]
    public class PersistentObject : PersistentSurrogate
    {
        public static implicit operator UnityObject(PersistentObject surrogate)
        {
            return (UnityObject)surrogate.WriteTo(new UnityObject());
        }
        
        public static implicit operator PersistentObject(UnityObject obj)
        {
            PersistentObject surrogate = new PersistentObject();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

