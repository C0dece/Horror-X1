using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RBCC.Scripts.Examples
{
    public class ObjectRespawner : MonoBehaviour
    {
        public GameObject[] objectsToHandle;

        private List<GameObject> objectsList = new List<GameObject>();
        private List<GameObject> objectsCopy = new List<GameObject>();

        private void Start()
        {
            objectsList = objectsToHandle.ToList();
            CopyObjects();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (objectsList.Contains(other.gameObject))
            {
                int idx = objectsList.FindIndex(go => go == other.gameObject);
                Destroy(other.gameObject);
                objectsList[idx] = Instantiate(objectsCopy[idx]);
                objectsList[idx].SetActive(true);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }

        private void CopyObjects()
        {
            foreach (GameObject go in objectsToHandle)
            {
                GameObject copy = Instantiate(go);
                copy.SetActive(false);
                objectsCopy.Add(copy);
            }
        }
    }
}
