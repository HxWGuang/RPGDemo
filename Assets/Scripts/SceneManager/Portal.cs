using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManger
{
    public class Portal : MonoBehaviour
    {
        /// <summary>
        /// 传送点目的地
        /// </summary>
        enum Destination
        {
            A,
            B,
            C,
            D,
            E
        }

        /// <summary>
        /// 需要加载的场景Index
        /// </summary>
        [SerializeField] private int SceneToLoad = -1;
        
        /// <summary>
        /// 传送点的出生点
        /// </summary>
        [SerializeField] private Transform SpawnPoint;
        
        /// <summary>
        /// 该传送点的目的地
        /// </summary>
        [SerializeField] private Destination _destination;

        /// <summary>
        /// 画面渐隐过渡时间
        /// </summary>
        [Space(10)]
        [SerializeField] private float fadeOutTime = 1f;
        
        /// <summary>
        /// 画面渐显前缓冲时间
        /// </summary>
        [SerializeField] private float fadeWaitTime = 0.5f;
        
        /// <summary>
        /// 画面渐显过渡时间
        /// </summary>
        [SerializeField] private float fadeInTime = 1f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Debug.Log("触发切换场景");
                // SceneManager.LoadScene(SceneToLoad);
                StartCoroutine(Transition());
            }
        }

        //这个方法里面一定要注意潜在的协程冲突问题，当Fade的时间比较长，在这
        //期间来回穿梭不同的Portal会出现协程冲突的问题，解决办法就是暂时取消
        //Player的控制权
        private IEnumerator Transition()
        {
            //加载前做的事
            //不删除gameObject的原因：开始加载的时候会删除
            //上一个场景的所有游戏对象，这个脚本也会一并被删除
            //所以要等把所有事都做完后再删除
            DontDestroyOnLoad(gameObject); //对象必须要Hierarchy的根层级下
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();

            //取消player的控制
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;
            
            yield return fader.FadeOut(fadeOutTime);
            wrapper.Save();

            //加载中
            yield return SceneManager.LoadSceneAsync(SceneToLoad);
            
            //取消player的控制，记住这里要重新获取，因为这是加载了场景后的代码，
            //玩家也会重新生成
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            //加载后做的事
            wrapper.Load();
            
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            wrapper.Save();

            // Debug.Log("场景加载完成");
            yield return new WaitForSeconds(fadeWaitTime);

            //让FadeIn独立运行，不妨碍下面的代码
            fader.FadeIn(fadeInTime);
            
            // 让玩家恢复控制
            playerController.enabled = true;
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            foreach (var portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal._destination != _destination) continue;

                return portal;
            }

            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            // player.transform.position = otherPortal.SpawnPoint.position;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.SpawnPoint.position);
            player.transform.rotation = otherPortal.SpawnPoint.rotation;
        }
    }
}
