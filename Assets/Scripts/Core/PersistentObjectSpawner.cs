using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// 此类用来生成持久性对象，如Canvas，用来在不同场景切换时进行FadeOut和FadeIn
    /// 如Saving系统，用来运行保存系统，检测保存行为
    /// </summary>
    public class PersistentObjectSpawner : MonoBehaviour
    {
        /// <summary>
        /// 持久对象预制体，上面会挂载一直存在的对象或脚本
        /// </summary>
        [SerializeField] private GameObject persistentObjectPrefab;

        private static bool hasSpawned = false;
        
        private void Awake()
        {
            // Debug.Log(gameObject + " Awake- " + gameObject.GetHashCode());
            if (hasSpawned) return;

            SpawnPersistentObject();
        }

        // private void Update()
        // {
        //     Debug.Log(gameObject + " Update- " + gameObject.GetHashCode());
        // }

        private void SpawnPersistentObject()
        {
            hasSpawned = true;
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
