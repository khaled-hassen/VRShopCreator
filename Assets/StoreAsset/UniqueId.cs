using System;
using UnityEngine;

namespace StoreAsset
{
    public class UniqueId : MonoBehaviour
    {
        public string uuid;

        public static UniqueId GenerateUniqueId()
        {
            var uniqueId = new GameObject().AddComponent<UniqueId>();
            uniqueId.uuid = Guid.NewGuid().ToString();
            return uniqueId;
        }
    }
}