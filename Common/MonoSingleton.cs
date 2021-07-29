using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Mono singleton class
/// </summary>
namespace SKCell {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _inst = null;

        public static T instance
        {
            get
            {
                return _inst;
            }
        }

        protected virtual void Awake()
        {
            if (_inst != null)
                CommonUtils.EditorLogError($"{name} already initialized!");

            _inst = (T)this;
        }

        protected virtual void OnDestroy()
        {
            //CommonUtils.EditorLogWarning($"MonoSingleton destroyed: {name}");
        }
    }
}
