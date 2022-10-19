using System.Collections;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.SceneManger
{
    /// <summary>
    /// Save系统的包装类
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        private const string DefaultSaveFile = "save";
        [SerializeField] private float fadeInTime = 0.5f;

        /**
         * 注意：这里把加载最后一个场景的代码的执行时机移到了Awake()函数里
         * 面，因为在其他脚本的Start()函数里面有些计算依赖存档里的数据，如果
         * 这里加载最后场景的代码还是在Start()里面，那么它的执行时机会比其他
         * 需要数据的地方晚从而导致一些bug
         */
        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }
        
        private IEnumerator LoadLastScene()
        {
            // Debug.Log("加载最后一个场景 - Awake()");

            yield return GetComponent<SavingSystem>().LoadLastScene(DefaultSaveFile);
            
            //Note: 这里要注意一下，一定要把对Fader的获取和访问放到LoadLastScene
            //的下面，因为Fader对象是在其他类的Awake方法里面动态生成的，而LoadLastScene
            //可以保证在其他所有的Awake方法都执行完后再执行，所以这里就可以保证Fader已经
            //被生成好了
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            
            yield return new WaitForSeconds(0.5f);
            fader.FadeIn(fadeInTime);
        }
        
        /// <summary>
        /// 把Start函数改成协同函数，异步运行
        /// 第一次加载时就会异步的进行FadeOut和FadeIn操作
        /// 以及加载保存的最后一个场景
        /// </summary>
        /// <returns></returns>
        // private IEnumerator Start()
        // {
        //     Debug.Log("加载最后一个场景 - start()");
        //     
        //     Fader fader = FindObjectOfType<Fader>();
        //     fader.FadeOutImmediate();
        //     yield return GetComponent<SavingSystem>().LoadLastScene(DefaultSaveFile);
        //     yield return fader.FadeIn(fadeInTime);
        // }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public void Load()
        {
            StartCoroutine(GetComponent<SavingSystem>().LoadLastScene(DefaultSaveFile));
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(DefaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(DefaultSaveFile);
        }
    }
}
