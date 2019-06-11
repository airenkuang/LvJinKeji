using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VideoController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public MediaPlayer mp;

    private float startPosX;

    public float seekStep;

    public Text timeShow;

    public GameObject playIcon;

    private string videoPath;

    private DisplayUGUI display;

    public Button closeBtn;

    private void Awake()
    {
        videoPath = Application.dataPath + "/StreamingAssets/Videos/";
        display = GetComponent<DisplayUGUI>();
    }
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            SysManager_CHange._instance.moveCompleted?.Invoke(display);
        });
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!mp.VideoOpened)
                mp.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, mp.m_VideoPath, true);
            if (IsPlaying())
            {
                playIcon.SetActive(true);
                Pause();
            }
            else
            {
                playIcon.SetActive(false);
                Play();
            }
        });

        mp.Events.AddListener(MpEvents);
    }

    void MpEvents(MediaPlayer meida, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        switch (eventType)
        {
            case MediaPlayerEvent.EventType.FinishedPlaying:
                if (!mp.Control.IsLooping())
                {
                    Debug.Log("返回默认");
                    mp.CloseVideo();
                    //mp.m_VideoPath = "";
                }
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                mp.Control.SetLooping(Config._instance.GetLoop());
                Debug.Log(mp.Control.IsLooping());
                break;
        }
    }


    public void Play()
    {
        mp.Play();
    }

    public void Pause()
    {
        mp.Pause();
    }

    public void Seek(float pos)
    {
        mp.Control.Seek(pos);
    }

    public bool IsPlaying()
    {
        return mp.Control.IsPlaying();
    }

    public string FormatTotalTime()
    {
        return FormatTime(mp.Info.GetDurationMs());
    }

    public string FormatCurrentTime()
    {
        return FormatTime(mp.Control.GetCurrentTimeMs());
    }

    private string FormatTime(float timeMs)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeMs);
        string h = timeSpan.Hours > 9 ? timeSpan.Hours.ToString() : "0" + timeSpan.Hours.ToString();
        string m = timeSpan.Minutes > 9 ? timeSpan.Minutes.ToString() : "0" + timeSpan.Minutes.ToString();
        string s = timeSpan.Seconds > 9 ? timeSpan.Seconds.ToString() : "0" + timeSpan.Seconds.ToString();
        StringBuilder sb = new StringBuilder();
        sb.Append(h + ":" + m + ":" + s);

        return sb.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosX = eventData.position.x;
        Pause();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float dis = eventData.position.x - startPosX;
        Seek(mp.Control.GetCurrentTimeMs() + dis * seekStep);
        timeShow.text = FormatCurrentTime() + "/" + FormatTotalTime();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        playIcon.SetActive(false);
        Play();
        timeShow.text = "";
    }
}
