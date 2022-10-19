using System;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.UI.InGame
{
    public class DamageUI : MonoBehaviour
    {

        public UnityAction<float, Vector3> showDamageText;

        public class ActiveText
        {
            public TextMeshProUGUI UIText;
            public float MaxTime;
            public float Timer;
            public Vector3 worldPositionStart;

            public void PlaceText(Camera cam, Canvas canvas)
            {
                float ratio = 1.0f - (Timer / MaxTime);
                Vector3 pos = worldPositionStart + new Vector3(ratio, Mathf.Sin(ratio * Mathf.PI), 0);
                pos = cam.WorldToScreenPoint(pos);
                pos.z = 0;

                UIText.transform.position = pos;
            }
        }

        [SerializeField] private TextMeshProUGUI damageTextPrefab = null;
        // [SerializeField] private Camera m_MainCamera = null;

        private Canvas m_Canvas;
        private Queue<TextMeshProUGUI> m_TextPool = new Queue<TextMeshProUGUI>();
        private List<ActiveText> m_ActiveTexts = new List<ActiveText>();

        private Camera m_MainCamera;

        private void OnEnable()
        {
            showDamageText = NewDamage;
        }

        private void Start()
        {
            // Debug.Log(gameObject + " Awake- " + gameObject.GetHashCode());
            
            m_Canvas = GetComponent<Canvas>();

            const int POOL_SIZE = 64;
            for (int i = 0; i < POOL_SIZE; i++)
            {
                var t = Instantiate(damageTextPrefab, m_Canvas.transform);
                t.gameObject.SetActive(false);
                m_TextPool.Enqueue(t);
            }
            
            // Note：切记！！！不能在Awake()里面获取主相机m_MainCamera = Camera.main
            // Awake()执行完后，值就不见了！！！可能是Unity的BUG，坑的一批！！！
            // Tips: 更新：不是Unity的BUG，在Awake()里初始化后Camera的引用消失的原因是
            // SavingSystem在每次启动游戏的时候都会异步加载一次场景，一开始游戏的时候Awake()
            // 里已经初始化好了Camera，但是因为场景重新加载了，而DamageUI是持久化的游戏对象
            // 重新加载场景时不会被销毁，主Camera会重新生成，而DamageUI是不会重新生成的所以
            // 它的Awake()是不会再执行的，所以m_MainCamera的引用会丢失，而如果把初始化操作
            // 放到Start()里面的话，因为Start()的执行时机是在场景异步加载之后的所以大概就是
            // 主Camera销毁又重新生成后才执行的DamageUI的Start()开始初始化所以才没事！
            m_MainCamera = Camera.main;
            
            //手动给每个角色添加持久化监听器（成功运行无报错）手动在Inspector面板拖的话是会报错的
            foreach (Health health in FindObjectsOfType<Health>())
            {
                // Debug.Log("添加监听" + health.gameObject);
                // health.takeDamageEvent.AddListener(showDamageText);
                UnityEventTools.AddPersistentListener(health.getDamageEvent, showDamageText);
            }
        }

        private void Update()
        {
            // Debug.Log(gameObject + " Update- " + gameObject.GetHashCode());
            ShowDamageText();
        }

        // private void Init()
        // {
        //     // Debug.Log("pool.count = " + m_TextPool.Count);
        //
        //     m_Canvas = GetComponent<Canvas>();
        //     const int POOL_SIZE = 64;
        //     for (int i = 0; i < POOL_SIZE; i++)
        //     {
        //         var t = Instantiate(damageTextPrefab, m_Canvas.transform);
        //         t.gameObject.SetActive(false);
        //         m_TextPool.Enqueue(t);
        //     }
        //     m_MainCamera = Camera.main;
        // }

        private void ShowDamageText()
        {
            for (int i = 0; i < m_ActiveTexts.Count; i++)
            {
                var activeText = m_ActiveTexts[i];
                activeText.Timer -= Time.deltaTime;

                if (activeText.Timer <= 0)
                {
                    activeText.UIText.gameObject.SetActive(false);
                    m_TextPool.Enqueue(activeText.UIText);
                    m_ActiveTexts.RemoveAt(i);
                    i--;
                }
                else
                {
                    var color = activeText.UIText.color;
                    color.a = activeText.Timer / activeText.MaxTime;
                    activeText.UIText.color = color;
                    activeText.PlaceText(m_MainCamera, m_Canvas);
                }
            }
        }

        public void NewDamage(float amount, Vector3 worldPos)
        {
            // Debug.Log(gameObject + " NewDamage- " + gameObject.GetHashCode());

            var t = m_TextPool.Dequeue();

            // t.text = amount.ToString();
            t.text = String.Format("{0:0}", amount);    //去除小数
            t.gameObject.SetActive(true);

            ActiveText activeText = new ActiveText();
            activeText.MaxTime = 1f;
            activeText.Timer = activeText.MaxTime;
            activeText.UIText = t;
            activeText.worldPositionStart = worldPos + Vector3.up;  //为什么加上Vector3.up？因为角色的原点坐标在脚底要往上提一点
            activeText.PlaceText(m_MainCamera, m_Canvas);

            m_ActiveTexts.Add(activeText);
        }

        
    }
}
