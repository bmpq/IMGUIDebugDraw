using System;
using System.Collections.Generic;
using UnityEngine;

namespace IMGUIDebugDraw
{
    public class OnGUIDispatcher : MonoBehaviour
    {
        private static OnGUIDispatcher _instance;
        public static OnGUIDispatcher Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<OnGUIDispatcher>();

                if (_instance == null)
                {
                    GameObject runnerObject = new GameObject("OnGUIDispatcher");
                    _instance = runnerObject.AddComponent<OnGUIDispatcher>();
                    DontDestroyOnLoad(runnerObject);
                }

                return _instance;
            }
        }

        private readonly List<Action> _actionsToProcess = new List<Action>();
        private readonly List<Action> _actionsToDraw = new List<Action>();

        public void Enqueue(Action action)
        {
            lock (_actionsToProcess)
            {
                _actionsToProcess.Add(action);
            }
        }

        void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                lock (_actionsToProcess)
                {
                    _actionsToDraw.Clear();
                    _actionsToDraw.AddRange(_actionsToProcess);
                    _actionsToProcess.Clear();
                }
            }

            if (_actionsToDraw.Count == 0)
            {
                return;
            }

            Color originalColor = GUI.color;

            for (int i = 0; i < _actionsToDraw.Count; i++)
            {
                try
                {
                    _actionsToDraw[i]();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error executing enqueued OnGUI action: {e}");
                }
            }

            GUI.color = originalColor;
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}