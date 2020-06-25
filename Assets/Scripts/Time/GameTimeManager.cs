using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    public class GameTimeManager : MonoBehaviour {

        private class Node {
            public string id;
            public Func<float, float> callback;
            public List<Node> children;

            public Node(string id, Func<float, float> callback) {
                children = new List<Node>();
                this.callback = callback;
                this.id = id;
            }
        }

        private Dictionary<string, Node> nodePaths;
        private Dictionary<string, float> cachedValues;

        private Node root;

        private void Awake() {
            cachedValues = new Dictionary<string, float>();
            nodePaths = new Dictionary<string, Node>();

            root = new Node("/", null);
            nodePaths.Add("/", root);
        }

        public float DeltaTime(params string[] path) {

            float deltaTime = 1.0f;
            Node currNode = root;
            for (int n = 0; n < path.Length; ++n) {
                int childId = -1;
                for (int c = 0; childId < 0 && c < currNode.children.Count; ++c) {
                    if (currNode.children[c].id == path[n]) {
                        childId = c;
                    }
                }

                if (childId >= 0) {
                    currNode = currNode.children[childId];
                    deltaTime = currNode.callback(deltaTime);
                }
                else {
                    throw new Exception("Invalid node path " + path.ToString());
                }
            }

            return deltaTime;
        }

        public void AddNode(Func<float, float> callback, List<string> path) {
            Node newNode = new Node(path[path.Count - 1], callback);

            Node currNode = root;
            for (int n = 0; n < path.Count - 1; ++n) {
                int childId = -1;
                for (int c = 0; childId < 0 && c < currNode.children.Count; ++c) {
                    if (currNode.children[c].id == path[n]) {
                        childId = c;
                    }
                }

                if (childId >= 0) {
                    currNode = currNode.children[childId];
                }
                else {
                    throw new Exception("Invalid node path " + path.ToString());
                }
            }
            currNode.children.Add(newNode);
            nodePaths.Add("/." + string.Join(".", path.ToArray()), newNode);
            //cachedValues.Add("/." + string.Join(".", path.ToArray()), 0.0f);
        }
    }
}