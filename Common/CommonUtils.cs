using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SKCell
{
    /// <summary>
    /// Global Utilities
    /// </summary>
    public static class CommonUtils
    {
        #region Fields & Attributes
        private const int DEACTIVATE_DISTANCE = 100000;    //Deactivate a go by teleporting to this position 
        private static Dictionary<int, Vector3> oriPosDict = new Dictionary<int, Vector3>();    //Keep the original position of the teleported go's
        private static int[] lastFrameCounts = new int[3];

        private static Dictionary<string, IEnumerator> crDict = new Dictionary<string, IEnumerator>();
        private static Dictionary<string, Coroutine> procedureDict = new Dictionary<string, Coroutine>();

        private static List<GameObject> worldTexts = new List<GameObject>();
        private static Dictionary<string, GameObject> customMeshes = new Dictionary<string, GameObject>();
        private static string activeCustomMesh = string.Empty;

        public static bool NetworkAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }
        #endregion

        #region Log & Debug Utils
        public static void EditorLogNormal(object message, bool detailed = false)
        {
            if (detailed)
                Debug.Log($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[0]}>");
            else
                Debug.Log($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount}>");
            lastFrameCounts[0] = Time.frameCount;
        }
        public static void EditorLogWarning(object message, bool detailed = false)
        {
            if (detailed)
                Debug.LogWarning($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[1]}>");
            else
                Debug.LogWarning($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount}>");
            lastFrameCounts[1] = Time.frameCount;
        }
        public static void EditorLogError(object message, bool detailed = false)
        {
            if (detailed)
                Debug.LogError($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount} | Delta: {Time.frameCount - lastFrameCounts[2]}>");
            else
                Debug.LogError($"<<color=#CD7F32>{message}</color> | Frame: {Time.frameCount}>");
            lastFrameCounts[2] = Time.frameCount;
        }

        public static void PrintArray<T>(T[] array)
        {
            foreach (T unit in array)
                EditorLogNormal(unit);
        }
        public static void PrintList<T>(List<T> list)
        {
            foreach (T unit in list)
                EditorLogNormal(unit);
        }

        public static void PrintDict<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            foreach (var key in dict.Keys)
                EditorLogNormal($"Key: {key} --- Value: {dict[key]}");
        }

        public static void DebugDrawCircle(Vector3 center, float radius, Color color, float duration, int divisions)
        {
            for (int i = 0; i <= divisions; i++)
            {
                Vector3 vec1 = center + ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * i);
                Vector3 vec2 = center + ApplyRotationToVector(new Vector3(0, 1) * radius, (360f / divisions) * (i + 1));
                Debug.DrawLine(vec1, vec2, color, duration);
            }
        }

        public static void DebugDrawRectangle(Vector3 minXY, Vector3 maxXY, Color color, float duration)
        {
            Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(maxXY.x, minXY.y), color, duration);
            Debug.DrawLine(new Vector3(minXY.x, minXY.y), new Vector3(minXY.x, maxXY.y), color, duration);
            Debug.DrawLine(new Vector3(minXY.x, maxXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
            Debug.DrawLine(new Vector3(maxXY.x, minXY.y), new Vector3(maxXY.x, maxXY.y), color, duration);
        }

        public static void DebugDrawText(string text, Vector3 position, Color color, float size, float duration)
        {
            text = text.ToUpper();
            float kerningSize = size * 0.5f;
            Vector3 basePosition = position;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case '\n':
                        // Newline
                        position.x = basePosition.x;
                        position.y += size;
                        break;
                    case ' ':
                        position.x += kerningSize;
                        break;
                    default:
                        DebugDrawChar(c, position, color, size, duration);
                        position.x += kerningSize;
                        break;
                }
            }
        }

        public static void DebugDrawChar(char c, Vector3 position, Color color, float size, float duration)
        {
            switch (c)
            {
                default:
                case 'A':
                    DebugDrawLines(position, color, size, duration, new[] {
                0.317f,0.041f, 0.5f,0.98f, 0.749f,0.062f, 0.625f,0.501f, 0.408f,0.507f }); break;
                case 'B':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.289f,0.069f, 0.274f,0.937f, 0.609f,0.937f, 0.801f,0.879f, 0.829f,0.708f, 0.756f,0.538f, 0.655f,0.492f, 0.442f,0.495f, 0.271f,0.495f, 0.567f,0.474f, 0.676f,0.465f, 0.722f,0.385f, 0.719f,0.181f, 0.664f,0.087f, 0.527f,0.053f, 0.396f,0.05f, 0.271f,0.078f }); break;
                case 'C':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.695f,0.946f, 0.561f,0.949f, 0.426f,0.937f, 0.317f,0.867f, 0.265f,0.733f, 0.262f,0.553f, 0.292f,0.27f, 0.323f,0.172f, 0.417f,0.12f, 0.512f,0.096f, 0.637f,0.093f, 0.743f,0.117f, }); break;
                case 'D':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.314f,0.909f, 0.329f,0.096f, 0.53f,0.123f, 0.594f,0.197f, 0.673f,0.334f, 0.716f,0.498f, 0.692f,0.666f, 0.609f,0.806f, 0.457f,0.891f, 0.323f,0.919f }); break;
                case 'E':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.344f,0.919f, 0.363f,0.078f, 0.713f,0.096f, 0.359f,0.096f, 0.347f,0.48f, 0.53f,0.492f, 0.356f,0.489f, 0.338f,0.913f, 0.625f,0.919f }); break;
                case 'F':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.682f,0.916f, 0.329f,0.909f, 0.341f,0.66f, 0.503f,0.669f, 0.341f,0.669f, 0.317f,0.087f }); break;
                case 'G':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.618f,0.867f, 0.399f,0.849f, 0.292f,0.654f, 0.241f,0.404f, 0.253f,0.178f, 0.481f,0.075f, 0.612f,0.078f, 0.725f,0.169f, 0.728f,0.334f, 0.71f,0.437f, 0.609f,0.462f, 0.463f,0.462f }); break;
                case 'H':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.277f,0.876f, 0.305f,0.133f, 0.295f,0.507f, 0.628f,0.501f, 0.643f,0.139f, 0.637f,0.873f }); break;
                case 'I':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.487f,0.906f, 0.484f,0.096f }); break;
                case 'J':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.628f,0.882f, 0.679f,0.242f, 0.603f,0.114f, 0.445f,0.066f, 0.317f,0.114f, 0.262f,0.209f, 0.253f,0.3f, 0.259f,0.367f }); break;
                case 'K':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.292f,0.879f, 0.311f,0.111f, 0.305f,0.498f, 0.594f,0.876f, 0.305f,0.516f, 0.573f,0.154f,  }); break;
                case 'L':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.311f,0.879f, 0.308f,0.133f, 0.682f,0.148f,  }); break;
                case 'M':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.262f,0.12f, 0.265f,0.909f, 0.509f,0.608f, 0.71f,0.919f, 0.713f,0.151f,  }); break;
                case 'N':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.737f,0.885f, 0.679f,0.114f, 0.335f,0.845f, 0.353f,0.175f,  }); break;
                case 'O':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.609f,0.906f, 0.396f,0.894f, 0.271f,0.687f, 0.232f,0.474f, 0.241f,0.282f, 0.356f,0.142f, 0.527f,0.087f, 0.655f,0.09f, 0.719f,0.181f, 0.737f,0.379f, 0.737f,0.638f, 0.71f,0.836f, 0.628f,0.919f, 0.582f,0.919f, }); break;
                case 'P':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.314f,0.129f, 0.311f,0.873f, 0.658f,0.906f, 0.746f,0.8f, 0.746f,0.66f, 0.673f,0.544f, 0.509f,0.51f, 0.359f,0.51f, 0.311f,0.516f,  }); break;
                case 'Q':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.497f,0.925f, 0.335f,0.894f, 0.228f,0.681f, 0.213f,0.379f, 0.25f,0.145f, 0.396f,0.096f, 0.573f,0.105f, 0.631f,0.166f, 0.542f,0.245f, 0.752f,0.108f, 0.628f,0.187f, 0.685f,0.261f, 0.728f,0.398f, 0.759f,0.605f, 0.722f,0.794f, 0.64f,0.916f, 0.475f,0.946f,  }); break;
                case 'R':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.347f,0.142f, 0.332f,0.9f, 0.667f,0.897f, 0.698f,0.699f, 0.655f,0.58f, 0.521f,0.553f, 0.396f,0.553f, 0.344f,0.553f, 0.564f,0.37f, 0.655f,0.206f, 0.71f,0.169f }); break;
                case 'S':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.695f,0.842f, 0.576f,0.882f, 0.439f,0.885f, 0.329f,0.8f, 0.289f,0.626f, 0.317f,0.489f, 0.439f,0.44f, 0.621f,0.434f, 0.695f,0.358f, 0.713f,0.224f, 0.646f,0.111f, 0.494f,0.093f, 0.338f,0.105f, 0.289f,0.151f, }); break;
                case 'T':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.497f,0.172f, 0.5f,0.864f, 0.286f,0.858f, 0.719f,0.852f,  }); break;
                case 'U':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.232f,0.858f, 0.247f,0.251f, 0.366f,0.105f, 0.466f,0.078f, 0.615f,0.084f, 0.704f,0.123f, 0.746f,0.276f, 0.74f,0.559f, 0.737f,0.806f, 0.722f,0.864f, }); break;
                case 'V':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.238f,0.855f, 0.494f,0.105f, 0.707f,0.855f,  }); break;
                case 'X':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.783f,0.852f, 0.256f,0.133f, 0.503f,0.498f, 0.305f,0.824f, 0.789f,0.117f,  }); break;
                case 'Y':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.299f,0.842f, 0.497f,0.529f, 0.646f,0.842f, 0.49f,0.541f, 0.487f,0.105f, }); break;
                case 'W':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.228f,0.815f, 0.381f,0.093f, 0.503f,0.434f, 0.615f,0.151f, 0.722f,0.818f,  }); break;
                case 'Z':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.25f,0.87f, 0.795f,0.842f, 0.274f,0.133f, 0.716f,0.142f }); break;


                case '0':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.536f,0.891f, 0.509f,0.891f, 0.42f,0.809f, 0.378f,0.523f, 0.372f,0.215f, 0.448f,0.087f, 0.539f,0.069f, 0.609f,0.099f, 0.637f,0.242f, 0.646f,0.416f, 0.646f,0.608f, 0.631f,0.809f, 0.554f,0.888f, 0.527f,0.894f,  }); break;
                case '1':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.652f,0.108f, 0.341f,0.114f, 0.497f,0.12f, 0.497f,0.855f, 0.328f,0.623f,  }); break;
                case '2':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.311f,0.714f, 0.375f,0.83f, 0.564f,0.894f, 0.722f,0.839f, 0.765f,0.681f, 0.634f,0.483f, 0.5f,0.331f, 0.366f,0.245f, 0.299f,0.126f, 0.426f,0.126f, 0.621f,0.136f, 0.679f,0.136f, 0.837f,0.139f,  }); break;
                case '3':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.289f,0.855f, 0.454f,0.876f, 0.606f,0.818f, 0.685f,0.702f, 0.664f,0.547f, 0.564f,0.459f, 0.484f,0.449f, 0.417f,0.455f, 0.53f,0.434f, 0.655f,0.355f, 0.664f,0.233f, 0.591f,0.105f, 0.466f,0.075f, 0.335f,0.084f, 0.259f,0.142f,  }); break;
                case '4':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.353f,0.836f, 0.262f,0.349f, 0.579f,0.367f, 0.5f,0.376f, 0.49f,0.471f, 0.509f,0.069f,  }); break;
                case '5':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.67f,0.852f, 0.335f,0.858f, 0.347f,0.596f, 0.582f,0.602f, 0.698f,0.513f, 0.749f,0.343f, 0.719f,0.187f, 0.561f,0.133f, 0.363f,0.151f,  }); break;
                case '6':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.567f,0.888f, 0.442f,0.782f, 0.35f,0.544f, 0.326f,0.288f, 0.39f,0.157f, 0.615f,0.142f, 0.679f,0.245f, 0.676f,0.37f, 0.573f,0.48f, 0.454f,0.48f, 0.378f,0.41f, 0.335f,0.367f,  }); break;
                case '7':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.286f,0.852f, 0.731f,0.864f, 0.417f,0.117f, 0.57f,0.498f, 0.451f,0.483f, 0.688f,0.501f, }); break;
                case '8':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.518f,0.541f, 0.603f,0.623f, 0.649f,0.748f, 0.612f,0.858f, 0.497f,0.888f, 0.375f,0.824f, 0.341f,0.708f, 0.381f,0.611f, 0.494f,0.55f, 0.557f,0.513f, 0.6f,0.416f, 0.631f,0.312f, 0.579f,0.178f, 0.509f,0.108f, 0.436f,0.102f, 0.335f,0.181f, 0.308f,0.279f, 0.347f,0.401f, 0.423f,0.486f, 0.497f,0.547f,  }); break;
                case '9':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.475f,0.129f, 0.573f,0.495f, 0.646f,0.824f, 0.509f,0.97f, 0.28f,0.94f, 0.189f,0.827f, 0.262f,0.708f, 0.396f,0.69f, 0.564f,0.745f, 0.646f,0.83f,  }); break;


                case '.':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.515f,0.157f, 0.469f,0.148f, 0.469f,0.117f, 0.515f,0.123f, 0.503f,0.169f }); break;
                case ':':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.515f,0.157f, 0.469f,0.148f, 0.469f,0.117f, 0.515f,0.123f, 0.503f,0.169f });
                    DebugDrawLines(position, color, size, duration, new[] {
                0.515f,.5f+0.157f, 0.469f,.5f+0.148f, 0.469f,.5f+0.117f, 0.515f,.5f+0.123f, 0.503f,.5f+0.169f });
                    break;
                case '-':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.277f,0.51f, 0.716f,0.51f,  }); break;
                case '+':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.265f,0.513f, 0.676f,0.516f, 0.497f,0.529f, 0.49f,0.699f, 0.497f,0.27f,  }); break;

                case '(':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.542f,0.934f, 0.411f,0.797f, 0.344f,0.587f, 0.341f,0.434f, 0.375f,0.257f, 0.457f,0.12f, 0.567f,0.075f, }); break;
                case ')':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.442f,0.94f, 0.548f,0.757f, 0.625f,0.568f, 0.64f,0.392f, 0.554f,0.129f, 0.472f,0.056f,  }); break;
                case ';':
                case ',':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.533f,0.239f, 0.527f,0.154f, 0.487f,0.099f, 0.451f,0.062f,  }); break;
                case '_':
                    DebugDrawLines(position, color, size, duration, new[] {
               0.274f,0.133f, 0.716f,0.142f }); break;

            }
        }

        public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, Vector3[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(position + points[i] * size, position + points[i + 1] * size, color, duration);
            }
        }

        public static void DebugDrawLines(Vector3 position, Color color, float size, float duration, float[] points)
        {
            List<Vector3> vecList = new List<Vector3>();
            for (int i = 0; i < points.Length; i += 2)
            {
                Vector3 vec = new Vector3(points[i + 0], points[i + 1]);
                vecList.Add(vec);
            }
            DebugDrawLines(position, color, size, duration, vecList.ToArray());
        }
        #endregion

        #region MonoUtils
        public static GameObject SpawnObject(GameObject go)
        {
            return SKPoolManager.SpawnObject(go);
        }
        public static void ReleaseObject(GameObject go)
        {
            SKPoolManager.ReleaseObject(go);
        }
        public static void Destroy(GameObject go)
        {
            if (Application.isPlaying)
                GameObject.Destroy(go);
            else
                GameObject.DestroyImmediate(go);
        }
        public static void SetActiveEfficiently(GameObject go, bool enabled)
        {
            if (go == null)
            {
                EditorLogError("SetActiveEffectively: <GameObject go> is null.");
                return;
            }
            if (go.activeSelf ^ enabled)
                go.SetActive(enabled);
        }
        /// <summary>
        /// Deactivate a GameObject by teleporting it to a far position. In some cases doing so will be more efficient 
        /// for the OnEnable/OnDisable Unity events will not be called.
        /// </summary>
        /// <param name="go"></param>
        public static void DeactivateByTeleport(GameObject go)
        {
            if (go == null)
            {
                EditorLogError("DeactivateByTeleport: <GameObject go> is null.");
                return;
            }
            InsertOrUpdateKeyValueInDictionary(oriPosDict, go.GetHashCode(), go.transform.position);
            go.transform.position = new Vector3(DEACTIVATE_DISTANCE, 0, 0);
        }
        /// <summary>
        /// Reactivate a GameObject after you have called DeactivateByTeleport. This will reset the GO's position to which before the teleport.
        /// </summary>
        /// <param name="go"></param>
        public static void ReactivateTeleportedObject(GameObject go)
        {
            if (go == null)
            {
                EditorLogError("ReactivateTeleportedObject: <GameObject go> is null.");
                return;
            }
            int hash = go.GetHashCode();
            if (!oriPosDict.ContainsKey(hash))
            {
                EditorLogWarning("ReactivateTeleportedObject: DeactivateByTeleport not called before.");
                return;
            }
            Vector3 oriPos = GetValueInDictionary(oriPosDict, hash);
            go.transform.position = oriPos;
            RemoveKeyInDictionary(oriPosDict, hash);
        }

        public static T GetComponentNonAlloc<T>(GameObject go) where T : Component
        {
            if (!go.TryGetComponent<T>(out T result))
                return null;
            return result;
        }
        public static void SafeDestroy(GameObject go)
        {
            if (go == null || !go)
                return;
            GameObject.Destroy(go);
        }
        #endregion

        #region InputUtils
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static Vector3 GetDirToMouse(Vector3 fromPosition)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }

        public static void AddMouseDownAction(int mouseID, Action a)
        {
            SKInput.instance.RegisterMouseDownAction(mouseID, a);
        }
        public static void RemoveMouseDownAction(int mouseID, Action a)
        {
            SKInput.instance.RemoveMouseDownAction(mouseID, a);
        }
        public static void AddMouseUpAction(int mouseID, Action a)
        {
            SKInput.instance.RegisterMouseUpAction(mouseID, a);
        }
        public static void RemoveMouseUpAction(int mouseID, Action a)
        {
            SKInput.instance.RemoveMouseUpAction(mouseID, a);
        }
        public static void AddKeyDownAction(KeyCode kc, Action a)
        {
            SKInput.instance.RegisterKeyDownAction(kc, a);
        }
        public static void RemoveKeyDownAction(KeyCode kc, Action a)
        {
            SKInput.instance.RemoveKeyDownAction(kc, a);
        }
        public static void AddKeyUpAction(KeyCode kc, Action a)
        {
            SKInput.instance.RegisterKeyUpAction(kc, a);
        }
        public static void RemoveKeyUpAction(KeyCode kc, Action a)
        {
            SKInput.instance.RemoveKeyUpAction(kc, a);
        }
        #endregion

        #region GraphicUtils

        public static void GLDrawLine(Vector3 v1, Vector3 v2, int drawMode = GL.LINES)
        {
            GL.Begin(drawMode);
            GL.Vertex(v1);
            GL.Vertex(v2);
            GL.End();
        }
        public static void GLDrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, int drawMode = GL.TRIANGLES)
        {
            GL.Begin(drawMode);
            GL.Vertex(v1);
            GL.Vertex(v2);
            GL.Vertex(v3);
            GL.End();
        }
        public static void GLDrawQuads(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int drawMode = GL.QUADS)
        {
            GL.Begin(drawMode);
            GL.Vertex(v1);
            GL.Vertex(v2);
            GL.Vertex(v3);
            GL.Vertex(v4);
            GL.End();
        }

        /// <summary>
        /// Generate a triangle mesh. Vertices are in clockwise order.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Mesh TriangleMesh(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Mesh m = new Mesh();
            m.vertices = new Vector3[] { v1, v2, v3 };
            m.triangles = new int[] { 0, 1, 2 };
            return m;
        }

        public static Mesh QuadMesh(Vector3 v1, Vector3 v2)
        {
            float yDiff = Mathf.Abs(v2.y - v1.y);
            float xDiff = Mathf.Abs(v2.x - v1.x);
            Vector3 bottomLeft = Vector3.zero;
            if (v1.x < v2.x)
            {
                if (v1.y < v2.y)
                {
                    bottomLeft = v1;
                }
                else
                {
                    bottomLeft = v1 - new Vector3(yDiff, 0, 0);
                }
            }
            else
            {
                if (v1.y > v2.y)
                {
                    bottomLeft = v2;
                }
                else
                {
                    bottomLeft = v2 - new Vector3(yDiff, 0, 0);
                }
            }

            Mesh m = new Mesh();
            m.vertices = new Vector3[] { bottomLeft, bottomLeft + new Vector3(0, yDiff, 0),
                bottomLeft + new Vector3(xDiff, yDiff, 0),
                bottomLeft,
                bottomLeft + new Vector3(xDiff, yDiff, 0),
                bottomLeft + new Vector3(xDiff, 0, 0) };
            m.triangles = new int[] { 0, 1, 2, 3, 4, 5 };

            return m;
        }

        public static void NewCustomMesh(string id = null)
        {
            GameObject go = new GameObject("CustomMesh - " + id);
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = new Mesh();
            go.AddComponent<MeshRenderer>();
            go.hideFlags = HideFlags.DontSaveInEditor;
            InsertOrUpdateKeyValueInDictionary(customMeshes, id == null ? go.GetHashCode().ToString() : id, go);

            activeCustomMesh = id == null ? go.GetHashCode().ToString() : id;
        }

        public static void SetActiveCustomMesh(string id)
        {
            if (customMeshes.ContainsKey(id))
                activeCustomMesh = id;
        }

        public static GameObject GetCustomMesh(string id)
        {
            if (customMeshes.ContainsKey(id))
                return customMeshes[id];
            return null;
        }
        public static MeshFilter DrawQuad(Vector3 v1, Vector3 v2, Color color)
        {
            if (activeCustomMesh == string.Empty)
            {
                NewCustomMesh();
            }
            Mesh m = QuadMesh(v1, v2);
            GameObject go = customMeshes[activeCustomMesh];
            MeshFilter mf = go.GetComponent<MeshFilter>();

            GameObject go2 = new GameObject();
            MeshFilter mf2 = go2.AddComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            mf2.sharedMesh = m;
            mr.sharedMaterial = SKAssetLibrary.GridCellMat;
            mr.sharedMaterial.color = color;
            CombineMeshes(go, go2);
            Destroy(go2);
            return mf;
        }

        public static void ClearAllCustomMeshes()
        {
            foreach (var key in customMeshes.Keys)
            {
                Destroy(customMeshes[key]);
            }
            customMeshes.Clear();
        }

        public static void CombineMeshes(GameObject ori, GameObject tar)
        {
            MeshFilter[] meshFilters1 = ori.GetComponents<MeshFilter>();
            MeshFilter[] meshFilters2 = tar.GetComponents<MeshFilter>();

            MeshFilter[] meshFilters = new MeshFilter[meshFilters1.Length + meshFilters2.Length];
            meshFilters1.CopyTo(meshFilters, 0);
            meshFilters2.CopyTo(meshFilters, meshFilters1.Length);

            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }
            ori.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            ori.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        }
        #endregion

        #region AudioUtils
        /// <summary>
        /// Plays a non-identifiable sound.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static AudioSource PlaySound(string fileName, Action onFinish = null, bool loop = false, float volume = 1f)
        {
            return SKAudioManager.instance.PlaySound(fileName, onFinish, loop, volume);
        }

        /// <summary>
        /// Plays an identifiable sound given by id.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="id"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static AudioSource PlayIdentifiableSound(string fileName, string id, bool loop = false, float volume = 1)
        {
            return SKAudioManager.instance.PlayIdentifiableSound(fileName, id, loop, volume);
        }

        /// <summary>
        /// Stops an identifiable sound with a fade-out time of fadeTime seconds.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fadeTime"></param>
        public static void StopIdentifiableSound(string id, float fadeTime = 0.5f)
        {
            SKAudioManager.instance.StopIdentifiableSound(id, fadeTime);
        }

        public static AudioSource Play3dSound(string fileName, Vector3 position, bool loop = false, float volume = 1f)
        {
            return SKAudioManager.instance.Play3dSound(fileName, position, loop, volume);
        }

        /// <summary>
        /// Plays a music. (Separate from sounds)
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public static AudioSource PlayMusic(string filename, bool loop = true, float volume = 1f)
        {
            return SKAudioManager.instance.PlayMusic(filename, loop, 2, volume);
        }

        /// <summary>
        /// Stops the current music.
        /// </summary>
        public static void StopMusic()
        {
            SKAudioManager.instance.StopMusic();
        }

        public static void SetSoundVolume(float volume)
        {
            SKAudioManager.instance.ChangeSoundVolume(volume);
        }
        public static void SetMusicVolume(float volume)
        {
            SKAudioManager.instance.ChangeMusicVolume(volume);
        }
        #endregion

        #region DateTimeUtils
        public static string GetDayofWeekName(TimeNameDisplayMethod m, DateTime d)
        {
            if (d == null)
            {
                EditorLogError("GetDayofWeekName: <DateTime d> is NULL");
                return null;
            }
            switch (m)
            {
                case TimeNameDisplayMethod.Digit:
                    return ((int)(d.DayOfWeek)).ToString();
                case TimeNameDisplayMethod.ShortCamelCase:
                    return (d.DayOfWeek).ToString().Substring(0, 3);
                case TimeNameDisplayMethod.ShortLowerCase:
                    return (d.DayOfWeek).ToString().Substring(0, 3).ToLower();
                case TimeNameDisplayMethod.FullCamelCase:
                    return (d.DayOfWeek).ToString();
                case TimeNameDisplayMethod.FullLowerCase:
                    return (d.DayOfWeek).ToString().ToLower();
            }
            return null;
        }
        public static string GetMonthName(TimeNameDisplayMethod m, int month)
        {
            if (month < 1 || month > 12)
            {
                EditorLogError("GetMonthName: <int month> is out of range");
                return null;
            }
            switch (m)
            {
                case TimeNameDisplayMethod.Digit:
                    return month.ToString();
                case TimeNameDisplayMethod.ShortCamelCase:
                    return ((MonthName)(month)).ToString().Substring(0, 3) + ".";
                case TimeNameDisplayMethod.ShortLowerCase:
                    return ((MonthName)(month)).ToString().Substring(0, 3).ToLower() + ".";
                case TimeNameDisplayMethod.FullCamelCase:
                    return ((MonthName)(month)).ToString();
                case TimeNameDisplayMethod.FullLowerCase:
                    return ((MonthName)(month)).ToString().ToLower();
            }
            return null;
        }
        public static double GetSecondsSince(DateTime since, DateTime now)
        {
            return (now - since).Duration().TotalSeconds;
        }



        #endregion

        #region CoroutineUtils
        /// <summary>
        /// Invokes an action after time seconds, then repeatedly every repeatInterval seconds, stopping at repeatCount times.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="callback"></param>
        /// <param name="repeatCount"></param>
        /// <param name="repeatInterval"></param>
        public static void InvokeAction(float seconds, Action callback, int repeatCount = 0, float repeatInterval = 1, string id = "")
        {
            if (callback == null)
            {
                EditorLogError("InvokeAction: <Action callback> is null.");
                return;
            }

            SKCommonTimer.instance.InvokeAction(seconds, callback, repeatCount, repeatInterval, id);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Invoke an action after time seconds in editor mode.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="callback"></param>
        public static void InvokeActionEditor(float seconds, Action callback)
        {
            SKEditorCoroutineManager.StartEditorCoroutine(EditorActionCoroutine(seconds, callback));
        }
        private static IEnumerator EditorActionCoroutine(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback.Invoke();
        }
#endif
        /// <summary>
        /// Invokes an action after time seconds, then repeatedly every repeatInterval seconds.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="callback"></param>
        /// <param name="repeatInterval"></param>
        public static void InvokeActionUnlimited(float seconds, Action callback, float repeatInterval = 1, string id = "")
        {
            if (callback == null)
            {
                EditorLogError("InvokeAction: <Action callback> is null.");
                return;
            }
            SKCommonTimer.instance.InvokeActionUnlimited(seconds, callback, repeatInterval, id);
        }

        /// <summary>
        /// Cancels the action specified by id when calling InvokeAction.
        /// </summary>
        /// <param name="id">ID of action.</param>
        public static void CancelInvoke(string id)
        {
            SKCommonTimer.instance.CancelInvokeAction(id);
        }

        /// <summary>
        /// Starts a continuous procedure where a variable changes over time. Tweening.
        /// </summary>
        /// <param name="curve">Curve of the procedure.</param>
        /// <param name="variable">Procedure variable.</param>
        /// <param name="startValue">Initial value of the variable.</param>
        /// <param name="endValue">Target value of the variable.</param>
        /// <param name="timeParam">ProcedureType.Linear: Seconds to finish the procedure;  ProcedureType.Lerp: Lerp amount per frame.</param>
        /// <param name="action">Action called per frame.</param>
        /// <param name="onFinish">Action called at the end of the procedure.</param>
        /// <param name="allowMultipleInstances">If false, previous procedures with the same name will be terminated.</param>
        public static void StartProcedure(SKCurve curve, float variable, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish = null, bool allowMultipleInstances = false, string id = "")
        {
            Coroutine cr = StartCoroutine(ProcedureCR(curve, variable, startValue, endValue, timeParam, action, onFinish), allowMultipleInstances);
            if (id.Length > 0)
            {
                InsertOrUpdateKeyValueInDictionary(procedureDict, id, cr);
            }
        }

        /// <summary>
        /// Starts a continuous procedure where a variable changes from 0 to 1 over time. Tweening.
        /// </summary>
        /// <param name="curve">Curve of the procedure.</param>
        /// <param name="variable">Procedure variable.</param>
        /// <param name="action">Action called per frame.</param>
        /// <param name="onFinish">Action called at the end of the procedure.</param>
        /// <param name="id"></param>
        public static void StartProcedure(SKCurve curve, float variable, float time, Action<float> action, Action<float> onFinish = null, string id = "")
        {
            Coroutine cr = StartCoroutine(ProcedureCR(curve, variable, 0, 1, time, action, onFinish), true);
            if (id.Length > 0)
            {
                InsertOrUpdateKeyValueInDictionary(procedureDict, id, cr);
            }
        }

        /// <summary>
        /// Starts a continuous procedure where a variable changes over time. Tweening.
        /// </summary>
        /// <param name="type">Type of the procedure.</param>
        /// <param name="variable">Procedure variable.</param>
        /// <param name="startValue">Initial value of the variable.</param>
        /// <param name="endValue">Target value of the variable.</param>
        /// <param name="timeParam">ProcedureType.Linear: Seconds to finish the procedure;  ProcedureType.Lerp: Lerp amount per frame.</param>
        /// <param name="action">Action called per frame.</param>
        /// <param name="onFinish">Action called at the end of the procedure.</param>
        /// <param name="lerpThreshold">Only in Lerp mode: the threshold of lerp operation.</param>
        /// <param name="allowMultipleInstances">If false, previous procedures with the same name will be terminated.</param>
        public static void StartProcedure(ProcedureType type, float variable, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish = null, float lerpThreshold = 0.05f, bool allowMultipleInstances = false, string id = "")
        {
            Coroutine cr = StartCoroutine(ProcedureCR(type, variable, startValue, endValue, timeParam, action, onFinish, lerpThreshold), allowMultipleInstances);
            if (id.Length > 0)
            {
                InsertOrUpdateKeyValueInDictionary(procedureDict, id, cr);
            }
        }

        /// <summary>
        /// Stops the procedure specified by id when calling StartProcedure.
        /// </summary>
        /// <param name="id"></param>
        public static void StopProcedure(string id)
        {
            if (procedureDict.ContainsKey(id))
            {
                StopCoroutine(procedureDict[id]);
                procedureDict.Remove(id);
            }
        }
        private static IEnumerator ProcedureCR(ProcedureType type, float variable, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish, float lerpThreshold)
        {
            variable = startValue;
            if (type == ProcedureType.Linear)
            {
                float stepValue = ((endValue - startValue) / timeParam) * Time.fixedDeltaTime;
                int step = Mathf.CeilToInt(timeParam / Time.fixedDeltaTime);

                for (int i = 0; i < step; i++)
                {
                    variable += stepValue;

                    if (action != null)
                        action.Invoke(variable);

                    yield return new WaitForFixedUpdate();
                }
            }
            if (type == ProcedureType.Lerp)
            {
                while (Mathf.Abs(endValue - variable) > lerpThreshold)
                {
                    variable = Mathf.Lerp(variable, endValue, timeParam);
                    action.Invoke(variable);
                    yield return new WaitForEndOfFrame();
                }
            }
            variable = endValue;
            if (onFinish != null)
                onFinish.Invoke(variable);
        }
        private static IEnumerator ProcedureCR(SKCurve curve, float variable, float startValue, float endValue, float timeParam, Action<float> action, Action<float> onFinish)
        {
            variable = startValue;
            float step = timeParam / Time.fixedDeltaTime;
            float diff = endValue - startValue;
            for (int i = 0; i <= step; i++)
            {
                variable = startValue + (SKCurveSampler.SampleCurve(curve, i / step)) * diff;
                if (action != null)
                    action.Invoke(variable);

                yield return new WaitForFixedUpdate();
            }
            variable = endValue;
            if (action != null)
                action.Invoke(variable);
            if (onFinish != null)
                onFinish.Invoke(variable);
        }

        public static Coroutine StartCoroutine(IEnumerator cr, bool allowMultipleInstances = false)
        {
            if (!allowMultipleInstances)
            {
                if (crDict.ContainsKey(nameof(cr)))
                {
                    SKCommonTimer.instance.StopCoroutine(crDict[nameof(cr)]);
                }
                InsertOrUpdateKeyValueInDictionary(crDict, nameof(cr), cr);
            }
            return SKCommonTimer.instance.StartCoroutine(cr);
        }

        public static void StopCoroutine(IEnumerator cr)
        {
            SKCommonTimer.instance.StopCoroutine(cr);
        }
        public static void StopCoroutine(Coroutine cr)
        {
            SKCommonTimer.instance.StopCoroutine(cr);
        }

        public static void ReleaseObject(GameObject obj, float time)
        {
            InvokeAction(time, () =>
            {
                ReleaseObject(obj);
            });
        }
        #endregion

        #region EditorUtils
#if UNITY_EDITOR
        public static void RefreshSelection(GameObject toSelect)
        {
            GameObject go = new GameObject();
            Selection.activeGameObject = go;
            Destroy(go);
            Selection.activeGameObject = toSelect;
        }

#endif
        #endregion

        #region BaseUtils

        public static T[] Serialize2DArray<T>(T[,] arr)
        {
           
            int length1 = arr.GetLength(0);
            int length2 = arr.GetLength(1);
            T[] res = new T[length1 * length2];
            for (int i = 0; i < length1; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    res[i * length1 + j] = arr[i, j];
                }
            }
            return res;
        }
        public static T[,] Deserialize2DArray<T>(T[] arr, int len1, int len2)
        {
            int length = arr.Length;

            T[,] res = new T[len1, len2];
            for (int i = 0; i < length; i++)
            {
                res[i / len1, i % len2] = arr[i];
            }
            return res;
        }

        public static T[] ModifyArray<T>(T[] arr, int length)
        {
            T[] res = new T[length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (i >= length)
                    break;
                res[i] = arr[i];
            }
            return res;
        }

        public static T[,] Modify2DArray<T>(T[,] arr, int width, int height)
        {
            T[,] res = new T[width, height];
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                if (i >= width)
                    break;
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (j >= height)
                        break;
                    res[i, j] = arr[i, j];
                }
            }
            return res;
        }
        public static Vector3 Vector2Angle(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector3 Angle2Vector(float angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Vector3 Angle2VectorInt(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float Vector2AngleFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static float Vector2AngleFloatXZ(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static int Vector2Angle(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static int Vector2Angle180(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }
        public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
        {
            return ApplyRotationToVector(vec, Vector2AngleFloat(vecRotation));
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, 0, angle) * vec;
        }

        public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, angle, 0) * vec;
        }
        public static void InsertOrUpdateKeyValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict == null)
            {
                EditorLogError("InsertOrUpdateKeyInDictionary: <Dictionary dict> is null.");
                return;
            }
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }
        public static void RemoveKeyInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict == null)
            {
                EditorLogError("RemoveKeyInDictionary: <Dictionary dict> is null.");
                return;
            }
            if (dict.ContainsKey(key))
                dict.Remove(key);
            else
                return;
        }
        public static TValue GetValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict == null)
            {
                EditorLogError("GetValueInDictionary: <Dictionary dict> is null.");
                return default(TValue);
            }
            if (dict.ContainsKey(key))
                return dict[key];
            else
            {
                EditorLogError("GetValueInDictionary: Key is not present.");
                return default(TValue);
            }
        }

        public static void InsertToList<T>(List<T> list, T item, bool allowMultiple)
        {
            if (!allowMultiple)
            {
                if (list.Contains(item))
                    return;
            }
            list.Add(item);
        }

        public static void RemoveFromList<T>(List<T> list, T item)
        {
            if (list.Contains(item))
            {
                list.Remove(item);
            }
        }

        public static void SwapValue<T>(ref T value1, ref T value2)
        {
            T temp = value1;
            value1 = value2;
            value2 = temp;
        }

        public static void FillArray<T>(T[] arr, T item)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = item;
            }
        }

        public static void FillList<T>(List<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = item;
            }
        }
        public static int CountInArray<T>(T[] arr, T item)
        {
            int count = 0;
            foreach (var i in arr)
            {
                if (i.Equals(item))
                    count++;
            }
            return count;
        }
        public static int CountInList<T>(List<T> list, T item)
        {
            int count = 0;
            foreach (var i in list)
            {
                if (i.Equals(item))
                    count++;
            }
            return count;
        }
        public static T[] RemoveDuplicatesInArray<T>(T[] arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }
            return list.ToArray();
        }

        public static List<T> RemoveDuplicatesInList<T>(List<T> arr)
        {
            List<T> list = new List<T>();
            foreach (T t in arr)
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }
            return list;
        }
        public static int MaxInArray(int[] arr)
        {
            int max = int.MinValue;
            for (int i = 0; i < arr.Length; i++)
            {
                max = arr[i] >= max ? arr[i] : max;
            }
            return max;
        }
        public static float MaxInArray(float[] arr)
        {
            float max = float.MinValue;
            for (int i = 0; i < arr.Length; i++)
            {
                max = arr[i] >= max ? arr[i] : max;
            }
            return max;
        }
        public static int MinInArray(int[] arr)
        {
            int min = int.MaxValue;
            for (int i = 0; i < arr.Length; i++)
            {
                min = arr[i] <= min ? arr[i] : min;
            }
            return min;
        }
        public static float MinInArray(float[] arr)
        {
            float min = float.MaxValue;
            for (int i = 0; i < arr.Length; i++)
            {
                min = arr[i] <= min ? arr[i] : min;
            }
            return min;
        }

        public static int BoolToInt(bool b)
        {
            return b ? 1 : 0;
        }
        public static bool IntToBool(int i)
        {
            i = (int)Mathf.Clamp01(i);
            return i == 1 ? true : false;
        }
        public static string CompressString(string str)
        {
            var compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            var compressAfterByte = Compress(compressBeforeByte);
            string compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;
        }

        public static string DecompressString(string str)
        {
            var compressBeforeByte = Convert.FromBase64String(str);
            var compressAfterByte = Decompress(compressBeforeByte);
            string compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            return compressString;
        }
        private static byte[] Compress(byte[] data)
        {
            try
            {
                var ms = new MemoryStream();
                var zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                var buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        private static byte[] Decompress(byte[] data)
        {
            try
            {
                var ms = new MemoryStream(data);
                var zip = new GZipStream(ms, CompressionMode.Decompress, true);
                var msreader = new MemoryStream();
                var buffer = new byte[0x1000];
                while (true)
                {
                    var reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region ReflectionUtils
        public static FieldInfo[] GetConstants(System.Type type)
        {
            List<FieldInfo> constants = new List<FieldInfo>();

            FieldInfo[] fieldInfos = type.GetFields(
                BindingFlags.Public | BindingFlags.Static |
                BindingFlags.FlattenHierarchy);

            foreach (FieldInfo fi in fieldInfos)
                if (fi.IsLiteral && !fi.IsInitOnly)
                    constants.Add(fi);
            return constants.ToArray();
        }

        public static void CallMethod(string method, Type type, object param)
        {
            var m = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
            if (m != null)
            {
                m.Invoke(null, new object[] { param });
            }
        }

        public static void CallMethod(string method, Type type)
        {
            var m = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
            if (m != null)
            {
                m.Invoke(null, null);
            }
        }

        private static Type GetSpecificType(Type type)
        {
            if (type.IsGenericType == false)
            {
                return type;
            }
            else
            {
                return type.GetGenericTypeDefinition();
            }
        }

        public static List<Type> GetDerivedTypes(Type baseType)
        {
            Type[] types = Assembly.GetAssembly(baseType).GetTypes();
            List<Type> derivedTypes = new List<Type>();

            for (int i = 0, count = types.Length; i < count; i++)
            {
                Type type = types[i];
                if (IsSubclassOf(type, baseType))
                {
                    derivedTypes.Add(type);
                }
            }

            return derivedTypes;
        }

        public static bool IsSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null || type == baseType)
                return false;

            if (baseType.IsGenericType == false)
            {
                if (type.IsGenericType == false)
                    return type.IsSubclassOf(baseType);
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            type = type.BaseType;
            Type objectType = typeof(object);

            while (type != objectType && type != null)
            {
                Type curentType = type.IsGenericType ?
                    type.GetGenericTypeDefinition() : type;
                if (curentType == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        #endregion

        #region FileUtils
        public static void SaveObjectToJson(object obj, string fileName)
        {
            string path = (Application.isMobilePlatform ? Application.persistentDataPath : Application.streamingAssetsPath) + SKAssetLibrary.JSON_PATH_SUFFIX + fileName;
            File.WriteAllText(path, JsonUtility.ToJson(obj, true));
            EditorLogNormal($"Save to json: {fileName}");
        }
        public static T LoadObjectFromJson<T>(string fileName)
        {
            string path = (Application.isMobilePlatform ? Application.persistentDataPath : Application.streamingAssetsPath) + SKAssetLibrary.JSON_PATH_SUFFIX + fileName;
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        public static bool JsonFileExists(string fileName)
        {
            string path = (Application.isMobilePlatform ? Application.persistentDataPath : Application.streamingAssetsPath) + SKAssetLibrary.JSON_PATH_SUFFIX + fileName;
            return File.Exists(path);
        }

        #endregion

        #region HashUtils
        public static int HashCombine<T1, T2>(T1 t1, T2 t2)
        {
            int hash = 17;
            hash = t1 != null ? ((hash * 31) + t1.GetHashCode()) : hash;
            hash = t2 != null ? ((hash * 31) + t2.GetHashCode()) : hash;
            return hash;
        }

        public static int HashCombine<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            int hash = 17;
            hash = t1 != null ? ((hash * 31) + t1.GetHashCode()) : hash;
            hash = t2 != null ? ((hash * 31) + t2.GetHashCode()) : hash;
            hash = t3 != null ? ((hash * 31) + t3.GetHashCode()) : hash;
            return hash;
        }

        public static int HashCombine<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            int hash = 17;
            hash = t1 != null ? ((hash * 31) + t1.GetHashCode()) : hash;
            hash = t2 != null ? ((hash * 31) + t2.GetHashCode()) : hash;
            hash = t3 != null ? ((hash * 31) + t3.GetHashCode()) : hash;
            hash = t4 != null ? ((hash * 31) + t4.GetHashCode()) : hash;
            return hash;
        }

        public static int HashCombine(params object[] args)
        {
            int hash = 17;
            foreach (var arg in args)
            {
                if (arg == null)
                {
                    continue;
                }
                hash = (hash * 31) + arg.GetHashCode();
            }
            return hash;
        }

        public static int HashCombine(params int[] args)
        {
            int hash = 17;
            foreach (var arg in args)
            {
                hash = (hash * 31) + arg;
            }
            return hash;
        }

        public static int HashCombine(params string[] args)
        {
            int hash = 17;
            foreach (var arg in args)
            {
                if (arg == null)
                {
                    continue;
                }
                hash = (hash * 31) + arg.GetHashCode();
            }
            return hash;
        }
        #endregion

        #region MathUtils

        /// <summary>
        /// Returns a float y (0,1) on the curve corresponding to the given x (0,1).
        /// </summary>
        /// <returns></returns>
        public static float SampleCurve(SKCurve curve, float x)
        {
            return SKCurveSampler.SampleCurve(curve, x);
        }
        public static float CalcInputDirection(Quaternion p_from, Quaternion p_to)
        {
            float tmp = p_from.eulerAngles.y - p_to.eulerAngles.y;
            if (tmp < 0)
            {
                tmp += 360;
            }
            if (tmp > 180)
            {
                tmp -= 360;
            }
            return tmp;
        }

        //这个得到的是从-180到180
        public static float getAngle(Vector3 selfDirection, Vector3 compareDirection)
        {
            float a = Vector3.Angle(selfDirection, compareDirection);
            if (Vector2.Dot(selfDirection, compareDirection) < 0) a = -a;
            return a;
        }

        public static float getAngleXYPlane(Vector3 selfDirection, Vector3 compareDirection)
        {
            Vector3 v0 = selfDirection;
            Vector2 vt = new Vector2(v0.x, v0.y);
            Vector2 vn = new Vector2(v0.y, -v0.x);
            Vector3 v1 = compareDirection;
            Vector2 vs = new Vector2(v1.x, v1.y);
            float a = Vector2.Angle(vt, vs);
            if (Vector2.Dot(vn, vs) < 0) a = -a;
            return a;
        }

        public static bool LinesIntersect2DInNoYPlan(Vector3 ptStart0, Vector3 ptEnd0,
                Vector3 ptStart1, Vector3 ptEnd1,
                bool firstIsSegment, bool secondIsSegment
                                                    )
        {
            float d = (ptEnd0.x - ptStart0.x) * (ptStart1.z - ptEnd1.z) - (ptStart1.x - ptEnd1.x) * (ptEnd0.z - ptStart0.z);
            float d0 = (ptStart1.x - ptStart0.x) * (ptStart1.z - ptEnd1.z) - (ptStart1.x - ptEnd1.x) * (ptStart1.z - ptStart0.z);
            float d1 = (ptEnd0.x - ptStart0.x) * (ptStart1.z - ptStart0.z) - (ptStart1.x - ptStart0.x) * (ptEnd0.z - ptStart0.z);
            float kOneOverD = 1 / d;
            float t0 = d0 * kOneOverD;
            float t1 = d1 * kOneOverD;
            if ((!firstIsSegment || ((t0 >= 0.0) && (t0 <= 1.0))) &&
                    (!secondIsSegment || ((t1 >= 0.0) && (t1 <= 1.0))))
            {
                return true;
            }
            return false;
        }

        public static bool LinesIntersect2DInNoYPlanNeedOut(Vector3 ptStart0, Vector3 ptEnd0,
                Vector3 ptStart1, Vector3 ptEnd1,
                bool firstIsSegment, bool secondIsSegment,
                ref Vector3 pIntersectionPt
                                                           )
        {
            float d = (ptEnd0.x - ptStart0.x) * (ptStart1.z - ptEnd1.z) - (ptStart1.x - ptEnd1.x) * (ptEnd0.z - ptStart0.z);
            float d0 = (ptStart1.x - ptStart0.x) * (ptStart1.z - ptEnd1.z) - (ptStart1.x - ptEnd1.x) * (ptStart1.z - ptStart0.z);
            float d1 = (ptEnd0.x - ptStart0.x) * (ptStart1.z - ptStart0.z) - (ptStart1.x - ptStart0.x) * (ptEnd0.z - ptStart0.z);
            float kOneOverD = 1 / d;
            float t0 = d0 * kOneOverD;
            float t1 = d1 * kOneOverD;
            if ((!firstIsSegment || ((t0 >= 0.0) && (t0 <= 1.0))) &&
                    (!secondIsSegment || ((t1 >= 0.0) && (t1 <= 1.0))))
            {
                if (pIntersectionPt != null)
                {
                    pIntersectionPt.x = ptStart0.x + t0 * (ptEnd0.x - ptStart0.x);
                    pIntersectionPt.z = ptStart0.z + t0 * (ptEnd0.z - ptStart0.z);
                    pIntersectionPt.y = ptStart0.y;
                }
                return true;
            }
            return false;
        }


        //返回一条线段 和 一个球表面交点
        public static (bool intersect, Vector3 p1, Vector3 p2) LineIntersectSphere(Vector3 start, Vector3 end, Vector3 center, float radius)
        {
            var pos1 = Vector3.zero;
            var pos2 = Vector3.zero;
            var d = end - start;
            var f = start - center;
            float a = Vector3.Dot(d, d);
            float b = Vector3.Dot(f, d) * 2;
            float c = Vector3.Dot(f, f) - radius * radius;
            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                return (false, pos1, pos2);
            }
            else
            {
                discriminant = Mathf.Sqrt(discriminant);

                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                if (t1 >= 0 && t1 <= 1)
                {
                    pos1 = start + t1 * d;
                    //return true ;
                }
                if (t2 >= 0 && t2 <= 1)
                {
                    pos2 = start + t2 * d;
                    //return true ;
                }
                return (true, pos1, pos2);
            }
        }

        //反会有向线段从球表现射出的交点
        public static Vector3 LineOutInterectSpherePos(Vector3 start, Vector3 end, Vector3 center, float radius)
        {
            var result = LineIntersectSphere(start, end, center, radius);
            if (result.intersect)
            {
                if (Vector3.Distance(start, center) <= radius && Vector3.Distance(end, center) >= radius ||
                    Vector3.Distance(start, center) >= radius && Vector3.Distance(end, center) <= radius)
                    return result.p2;
                if (Vector3.Distance(start, center) >= radius && Vector3.Distance(end, center) >= radius)
                    return result.p1;
            }
            return Vector3.zero;
        }

        public static Vector3 LineInInterectSpherePos(Vector3 start, Vector3 end, Vector3 center, float radius)
        {
            var result = LineIntersectSphere(start, end, center, radius);
            if (result.intersect)
            {
                if (Vector3.Distance(start, center) <= radius && Vector3.Distance(end, center) >= radius ||
                    Vector3.Distance(start, center) >= radius && Vector3.Distance(end, center) <= radius)
                    return result.p1;
                if (Vector3.Distance(start, center) >= radius && Vector3.Distance(end, center) >= radius)
                    return result.p2;
            }
            return Vector3.zero;
        }


        public static float getAngleXZPlane(Vector3 selfDirection, Vector3 compareDirection)
        {
            Vector3 v0 = selfDirection;
            Vector2 vt = new Vector2(v0.x, v0.z);
            Vector2 vn = new Vector2(v0.z, -v0.x);
            Vector3 v1 = compareDirection;
            Vector2 vs = new Vector2(v1.x, v1.z);
            float a = Vector2.Angle(vt, vs);
            if (Vector2.Dot(vn, vs) < 0) a = -a;
            return a;
        }

        public static float getDistanceXZPlane(Vector3 position1, Vector3 position2)
        {
            return new Vector3(position1.x - position2.x, 0, position1.z - position2.z).magnitude;
        }

        public static float getDistanceXYPlane(Vector3 position1, Vector3 position2)
        {
            return new Vector3(position1.x - position2.x, position1.y - position2.y, 0).magnitude;
        }

        public static Vector3 getXZForward(Vector3 start, Vector3 end)
        {
            Vector3 forward = end - start;
            forward.y = .0f;
            return forward.normalized;
        }


        public static float CalcHDist(Vector3 pos0, Vector3 pos1)
        {
            Vector2 h_pos0 = new Vector2(pos0.x, pos0.z);
            Vector2 h_pos1 = new Vector2(pos1.x, pos1.z);
            return Vector2.Distance(h_pos0, h_pos1);
        }

        public static float CalcHDistSqr(Vector3 pos0, Vector3 pos1)
        {
            return (pos0.x - pos1.x) * (pos0.x - pos1.x) + (pos0.z - pos1.z) * (pos0.z - pos1.z);
        }

        public static float CalcHDistToLineSegment(Vector3 p1, Vector3 a1, Vector3 b1)
        {
            Vector2 p = new Vector2(p1.x, p1.z);
            Vector2 a = new Vector2(a1.x, a1.z);
            Vector2 b = new Vector2(b1.x, b1.z);
            float sqrMagnitude = (b - a).sqrMagnitude;
            if (Math.Abs(sqrMagnitude) < 1e-6)
                return (p - a).magnitude;
            float num = Vector2.Dot(p - a, b - a) / sqrMagnitude;
            if (num < 0.0)
                return (p - a).magnitude;
            if (num > 1.0)
                return (p - b).magnitude;
            Vector2 vector2 = a + num * (b - a);
            return (p - vector2).magnitude;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            return Mathf.Acos(Mathf.Clamp(Vector2.Dot(from, to), -1f, 1f));
        }

        public static bool LayerContains(LayerMask p_mask, int layer)
        {
            return ((1 << layer) & p_mask) != 0;
        }

        public static bool ApproximatelyXZ(Vector3 vec, float val, float epsilon = 1e-3f)
        {
            vec.y = .0f;
            return Mathf.Abs(vec.magnitude - val) <= epsilon;
        }

        public static Vector3 GetProjectPositionToSegmentTrigger(Vector3 position, Vector3 start, Vector3 end)
        {
            var dirNormalized = (end - start).normalized;
            var dirNormalizedXZ = dirNormalized;
            dirNormalizedXZ.y = 0;
            dirNormalizedXZ.Normalize();
            float distOrigToPlane = Vector3.Dot(dirNormalizedXZ, position);
            float distStartToPlane = distOrigToPlane - Vector3.Dot(start, dirNormalizedXZ);
            float cosValue = Vector3.Dot(dirNormalized, dirNormalizedXZ);
            float len = distStartToPlane / cosValue;
            return start + len * dirNormalized;
        }

        public static Vector3 GetProjectPositionClampedToSegmentTrigger(Vector3 position, Vector3 startPos, Vector3 endPos, float margin)
        {
            var targetPos = GetProjectPositionToSegmentTrigger(position, startPos, endPos);
            var edgePercent = Mathf.Min(0.5f, margin / (endPos - startPos).magnitude);
            float t = 0;
            if (Vector3.Dot(targetPos - startPos, endPos - startPos) < 0)
                t = edgePercent;
            else
                t = Mathf.Clamp((targetPos - startPos).magnitude / (endPos - startPos).magnitude, edgePercent, 1 - edgePercent);
            targetPos = startPos + (endPos - startPos) * t;
            return targetPos;
        }

        public static Quaternion LerpAngleY(Vector3 startForward, Vector3 targetForward, float t)
        {
            var y = Mathf.LerpAngle(Quaternion.LookRotation(startForward).eulerAngles.y, Quaternion.LookRotation(targetForward).eulerAngles.y, t);
            return Quaternion.Euler(0, y, 0);
        }

        // 目前为止 Animator.StringToHash都是c3c32实现
        public static int CalcStrCRC32(string p_str)
        {
            return Animator.StringToHash(p_str);
        }

        // only for windows
        public static int HashKey(Transform transform)
        {
            var position = transform.position;
            var rotation = transform.rotation;
            var scale = transform.lossyScale;
            var hash = position.GetHashCode() << 1 ^ rotation.GetHashCode() ^ scale.GetHashCode() >> 1;
            return hash;
        }

        //计算xz两个线段交点
        public static (bool success, Vector3 interactXZ) IntersectXZ(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float p = (p1.x - p2.x) * (p3.z - p4.z) - (p1.z - p2.z) * (p3.x - p4.x);
            if (Mathf.Abs(p) < 0.00001f)
            {
                return (false, new Vector3(Mathf.Infinity, 0, 0));
            }
            float px = ((p1.x * p2.z - p1.z * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.z - p3.z * p4.x)) / p;
            float pz = ((p1.x * p2.z - p1.z * p2.x) * (p3.z - p4.z) - (p1.z - p2.z) * (p3.x * p4.z - p3.z * p4.x)) / p;
            return (true, new Vector3(px, 0, pz));
        }
        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planeNormal">平面的法向量</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns></returns>
        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
        }

        public static Vector3 GetRotatePoint(Vector3 center, Vector3 offset, float rotation)
        {
            var cosAngle = Mathf.Cos(rotation);
            var sinAngle = Mathf.Sin(rotation);
            return new Vector3(-offset.x * cosAngle - (-offset.z) * sinAngle + center.x, 0,
                -offset.x * sinAngle + (-offset.z) * cosAngle + center.z);
        }

        public static float GetCross(Vector3 p1, Vector3 p2, Vector3 p)
        {
            return (p2.x - p1.x) * (p.z - p1.z) - (p.x - p1.x) * (p2.z - p1.z);
        }
        /// <summary>
        /// 判断点是否在矩形内
        /// </summary>
        /// <param name="rectCenter">矩形中心</param>
        /// <param name="rectSize">矩形尺寸</param>
        /// <param name="rotation">矩形顺时针旋转角度</param>
        /// <param name="point">目标点</param>
        /// <returns></returns>
        public static bool PointInRectangle(Vector3 rectCenter, Vector3 rectSize, float rotation, Vector3 point)
        {
            CalcRectangleRotate(rectCenter, rectSize, rotation, out Vector3 p1r, out Vector3 p2r, out Vector3 p3r,
                out Vector3 p4r);
            return PointInRectangle(p1r, p2r, p3r, p4r, point);
        }

        public static void CalcRectangleRotate(Vector3 rectCenter, Vector3 rectSize, float rotation,
            out Vector3 p1r, out Vector3 p2r, out Vector3 p3r, out Vector3 p4r)
        {
            var offset = rectSize / 2;
            p1r = GetRotatePoint(rectCenter, new Vector3(-offset.x, 0, -offset.z), rotation);
            p2r = GetRotatePoint(rectCenter, new Vector3(-offset.x, 0, offset.z), rotation);
            p3r = GetRotatePoint(rectCenter, new Vector3(offset.x, 0, offset.z), rotation);
            p4r = GetRotatePoint(rectCenter, new Vector3(offset.x, 0, -offset.z), rotation);
        }

        /// <summary>
        /// 判断点是否在矩形内
        /// </summary>
        /// <param name="p1">左上</param>
        /// <param name="p2">左下</param>
        /// <param name="p3">右下</param>
        /// <param name="p4">右上</param>
        /// <param name="point">目标点</param>
        /// <returns></returns>
        public static bool PointInRectangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 point)
        {
            return GetCross(p1, p2, point) * GetCross(p3, p4, point) >= 0
                   && GetCross(p2, p3, point) * GetCross(p4, p1, point) >= 0;
        }
        public static bool IsPrime(int n)
        {
            if (n <= 1)
                return false;
            int count = 0;
            for (int i = 2; i <= Math.Sqrt(n); ++i)
            {
                if (n % i != 0)
                    continue;
                count++;
            }
            return count == 0;
        }

        public static int NotNegative(int num)
        {
            return num <= 0 ? 0 : num;
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Keep2Decimal(float num)
        {
            return (float)Math.Round(num, 2);
        }

        public static int Floor(float num)
        {
            return (int)Math.Floor(num);
        }

        public static int Convert2Int(float num)
        {
            return Convert.ToInt32(num);
        }

        #endregion

        #region MiscUtils
        public static bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            else
            {
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);
                return hits.Count > 0;
            }
        }

        public static Color RandomColor()
        {
            return new Color(Random(0f, 1f), Random(0f, 1f), Random(0f, 1f), 1);
        }
        public static Color MixColor(Color c1, Color c2, float mix = 0.5f)
        {
            Color diff = c2 - c1;
            return new Color(c1.r + mix * diff.r, c1.g + mix * diff.g, c1.b + mix * diff.b, c1.a + mix * diff.a);
        }
        public static float ColorLuminance(Color c)
        {
            return c.r * 0.2125f + c.g * 0.7154f + c.b * 0.0721f;
        }

        #endregion
    }

    #region CommonEnums

    public enum ProcedureType
    {
        Linear,
        Lerp
    }
    public enum TimeNameDisplayMethod
    {
        Digit,
        ShortCamelCase,
        ShortLowerCase,
        FullCamelCase,
        FullLowerCase,
    }

    public enum MonthName
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }


    #endregion
}

