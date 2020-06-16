using System;
using UnityEngine.Serialization;

namespace UtilityAI {
    [Serializable]
    public struct AISettings {
        [FormerlySerializedAs("updateInterval")]
        public float UpdateIntervalSec;
    }
}