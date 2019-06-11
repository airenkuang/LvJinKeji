using UnityEngine;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Threading;

public class SysManager : MonoBehaviour
{
   // public DOTweenAnimation ani;

    public DisplayUGUI mpDisplyer;

    public Button[] btns;

    public DisplayUGUI displayLeft,displayRight;

    public UnityAction<DisplayUGUI> moveCompleted;

    private DisplayUGUI curDis;

    public Button defaultVideoBtn;

    private Button displayLeftBtn,displayRightBtn;
    public GameObject leftPlayIcon,rightPlayIcon;

    public VideoController leftVc,rightVc;

    private string filePath;

    public string videoPath;

    private float scaleFactor;

    private Texture tex_show;
    private float unusedTime;

    private float ttt;

    private bool isdefault = true;
    public static SysManager _instance;
    private void Awake()
    {
        _instance = this;
        displayLeftBtn = displayLeft.GetComponent<Button>();
        displayRightBtn = displayRight.GetComponent<Button>();

        filePath = Application.dataPath + "/StreamingAssets/";
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var btn in btns)
        {
            btn.onClick.AddListener(() =>
            {
                OnBtnClick(btn);
            });

            moveCompleted += delegate (DisplayUGUI dis)
            {
                dis._mediaPlayer.Stop();
                dis.transform.DOScale(0, 1).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    Debug.Log("completed");
                    //dis._mediaPlayer.m_VideoPath = "";
                    dis._mediaPlayer.CloseVideo();
                });
            };
        }

        defaultVideoBtn.onClick.AddListener(() =>
        {
            if (mpDisplyer._mediaPlayer.Control.IsPlaying())
            {
                mpDisplyer._mediaPlayer.Stop();
                mpDisplyer.gameObject.SetActive(false);
                isdefault = false;
                //ani.DOPlay();
            }
        });

        Cursor.visible = !Config._instance.GetHideCursor();

        scaleFactor = Config._instance.ScaleFactor();

        ttt = unusedTime = Config._instance.GetUnusedTime();
    }

    
    void OnBtnClick(Button btn)
    {
        Resources.UnloadUnusedAssets();
        tex_show = ImageLoader.GetInstance().LoadImg(filePath + "Images/" + btn.name + ".jpg");
        videoPath = "Videos/" + btn.name + ".mp4";
        if (btn.tag == "left")
        {
            displayLeft.GetComponent<DisplayUGUI>()._mediaPlayer.m_VideoPath = videoPath;
            displayLeft.GetComponent<VideoController>().playIcon.SetActive(false);
            displayLeft._defaultTexture = tex_show;
            displayLeft.transform.DOScale(scaleFactor, 1).SetEase(Ease.OutBounce);
        }
        else
        {
            displayRight.GetComponent<DisplayUGUI>()._mediaPlayer.m_VideoPath = videoPath;
            displayRight.GetComponent<VideoController>().playIcon.SetActive(false);
            displayRight._defaultTexture = tex_show;
            displayRight.transform.DOScale(scaleFactor, 1).SetEase(Ease.OutBounce);
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            unusedTime = ttt;
        }
      //  Debug.Log(unusedTime);
        if(!isdefault && unusedTime >= 0)
        {
            unusedTime -= Time.deltaTime;
            if(unusedTime <= 0)
            {
                moveCompleted?.Invoke(displayLeft.GetComponent<DisplayUGUI>());
                moveCompleted?.Invoke(displayRight.GetComponent<DisplayUGUI>());
                Thread.Sleep(50);
                mpDisplyer.gameObject.SetActive(true);
                mpDisplyer._mediaPlayer.Control.Seek(0);
                mpDisplyer._mediaPlayer.Play();
                isdefault = true;
                unusedTime = ttt;
            }
        }

        if (Input.touchCount == 2)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                unusedTime = ttt;
                if (EventSystem.current.alreadySelecting && EventSystem.current.currentSelectedGameObject.GetComponent<DisplayUGUI>() != null)
                    moveCompleted?.Invoke(EventSystem.current.currentSelectedGameObject.GetComponent<DisplayUGUI>());
            }
        }
        if(Input.touchCount > 0)
        {
            unusedTime = ttt;
        }

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            moveCompleted?.Invoke(displayLeft.GetComponent<DisplayUGUI>());
            moveCompleted?.Invoke(displayRight.GetComponent<DisplayUGUI>());
        }

        #region
        //if (Input.touchCount > 3)
        //{
        //    unusedTime = ttt;
        //    if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
        //    {
        //        if (EventSystem.current.currentSelectedGameObject.GetComponent<DisplayUGUI>() != null)
        //        {
        //            curDis = EventSystem.current.currentSelectedGameObject.GetComponent<DisplayUGUI>();
        //            moveCompleted?.Invoke(curDis);
        //        }
        //    }

        //    if (Input.GetTouch(2).phase == TouchPhase.Ended || Input.GetTouch(3).phase == TouchPhase.Ended)
        //    {
        //        if (curDis != null)
        //        {
        //            if (curDis == displayLeft.GetComponent<DisplayUGUI>())
        //                moveCompleted?.Invoke(displayRight.GetComponent<DisplayUGUI>());
        //            else
        //                moveCompleted?.Invoke(displayLeft.GetComponent<DisplayUGUI>());
        //        }
        //    }
        //}
        #endregion
    }
}
