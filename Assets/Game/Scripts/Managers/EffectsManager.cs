using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : Singleton<EffectsManager>
{
    [System.Serializable]
    public struct VignetteEffects
    {
        public int mEffectRepeatCount;
        public float mEffectUpTime;
        public float mEffectUpDelay;
        public float mEffectDownTime;
        public float mEffectDownDelay;
        public Color mEffectColor;
        [HideInInspector] public float mCurrentCounter;
    }

    [System.Serializable]
    public struct EffectEvents
    {
        public int mEventType;
        public int mItemType;
    }

    [HideInInspector] public IngameHUD mGameHUD = null;
    [Header("Score Scale Settings")]
    [SerializeField] Vector3 mScoreScaleUp = new Vector3(3, 3, 3);
    [SerializeField] Vector3 mScoreMultiplierScaleUp = new Vector3(3, 3, 3);
    Vector3 mDefaultScoreScale = Vector3.one;
    Vector3 mDefaultScoreMultiplierScale = Vector3.one;
    [SerializeField] float mScaleUpEffectTime = 0.3f;
    [SerializeField] float mScaleUpEffectDelay = 0.2f;
    [SerializeField] float mScaleDownEffectTime = 0.3f;
    [SerializeField] float mScaleDownEffectDelay = 0.2f;
    [Header("Vignette Settings")]
    [HideInInspector] public float mImageDefaultAlpha;
    [Tooltip("Index 0 = Health, Index 1 = Fatigue, Index 2 = Score, Index 3 = Multiplier")]
    [SerializeField] VignetteEffects[] mItemEffects;
    [SerializeField] VignetteEffects mDamageEffect;
    [SerializeField] float mDamageShakeTime = 0.5f;
    [SerializeField] float mAmplitudeGain = 1.5f;
    [SerializeField] float mFrequencyGain = 6.0f;
    CinemachineBasicMultiChannelPerlin mCameraShakeNoise;

    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnClientEvents;
    }
    void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnClientEvents;
    }

    public void OnClientEvents(EventData photonEvent)
    {
        if(photonEvent.Code != NetworkManager.EVNT_EFFECTS)
        {
            return;
        }
        EffectEvents aEvent = new EffectEvents();
        object[] aData = (object[])photonEvent.CustomData;
        GenHelpers.DeSerializeData((byte[])aData[0],ref aEvent);
        switch(aEvent.mEventType)
        {
            //case 0:
            //    {
            //        DamageEffect();
            //        break;
            //    }
            case 1:
                {
                    ItemCollectedEffect((Item.Type)aEvent.mItemType);
                    break;
                }
        }
    }


    public void SetInGameHUD(IngameHUD pHUD)
    {
        mGameHUD = pHUD;
        mDefaultScoreScale = mGameHUD.mScoreText.rectTransform.localScale;
        mDefaultScoreMultiplierScale = mGameHUD.mScoreMultiplierText.rectTransform.localScale;
        mImageDefaultAlpha = mGameHUD.mVignetteImage.color.a;
    }

    public void ScoreAddedEffect()
    {
        ScaleRectEffect(mGameHUD.mScoreText.rectTransform, mScoreScaleUp, mDefaultScoreScale);
    }

    public void ScoreMultiplierAddedEffect()
    {
        ScaleRectEffect(mGameHUD.mScoreMultiplierText.rectTransform, mScoreMultiplierScaleUp, mDefaultScoreMultiplierScale);
    }

    void ScaleRectEffect(RectTransform pTransform, Vector3 pScaleUp, Vector3 pDefaultScale)
    {
        if (LeanTween.isTweening(pTransform))
        {
            LeanTween.cancel(pTransform);
        }
        LeanTween.scale(pTransform, pScaleUp, mScaleUpEffectTime).setDelay(mScaleUpEffectDelay).
            setOnComplete(() =>
            {
                LeanTween.scale(pTransform, pDefaultScale, mScaleDownEffectTime).setDelay(mScaleDownEffectDelay);
            });

    }

    public void ItemCollectedEffect(Item.Type pType)
    {
        int aIx = (int)pType;
        if(PhotonNetwork.IsMasterClient)
        {
            EffectEvents aEventData = new EffectEvents()
            {
                mEventType = 1,
                mItemType = aIx
            };
            NetworkManager.Instance.RaiseEvent(
                NetworkManager.EVNT_EFFECTS,
                new object[] { GenHelpers.SerializeData(aEventData) 
                },new Photon.Realtime.RaiseEventOptions()
                {
                    Receivers = Photon.Realtime.ReceiverGroup.Others
                },
                new SendOptions()
                {
                    Reliability = true
                });
        }
        if(pType == Item.Type.Score)
        {
            AudioManager.Instance.PlaySFX("Coin");
        }
        else
        {
            AudioManager.Instance.PlaySFX("Collect");
        }
        mItemEffects[aIx].mCurrentCounter = 0;
        VignetteEffectCaller(mItemEffects[aIx]);
    }

    public void DamageEffect()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    EffectEvents aEventData = new EffectEvents()
        //    {
        //        mEventType = 0,
        //        mItemType = 0
        //    };
        //    NetworkManager.Instance.RaiseEvent(
        //        NetworkManager.EVNT_EFFECTS,
        //        new object[] { GenHelpers.SerializeData(aEventData)
        //        }, new Photon.Realtime.RaiseEventOptions()
        //        {
        //            Receivers = Photon.Realtime.ReceiverGroup.Others
        //        },
        //        new SendOptions()
        //        {
        //            Reliability = true
        //        });
        //}
        AudioManager.Instance.PlaySFX("Crash");
        mDamageEffect.mCurrentCounter = 0;
        VignetteEffectCaller(mDamageEffect);
        if(mCameraShakeNoise == null)
        {
            mCameraShakeNoise = LevelManager.Instance.mLevelCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        if(LeanTween.isTweening(mCameraShakeNoise.gameObject))
        {
            LeanTween.cancel(mCameraShakeNoise.gameObject);
        }
        SetCinemachineNoise();
        LeanTween.value(0, 1, mDamageShakeTime).setOnComplete(() => SetCinemachineNoise(true));
    }

    void SetCinemachineNoise(bool pReset = false)
    {
        if(pReset)
        {
            mCameraShakeNoise.m_AmplitudeGain = 0;
            mCameraShakeNoise.m_FrequencyGain = 0;
        }
        else
        {
            mCameraShakeNoise.m_AmplitudeGain = mAmplitudeGain;
            mCameraShakeNoise.m_FrequencyGain = mFrequencyGain;
        }
    }

    void VignetteEffectCaller(VignetteEffects pEffect)
    {
        if (LeanTween.isTweening(mGameHUD.mVignetteImage.gameObject))
        {
            LeanTween.cancel(mGameHUD.mVignetteImage.gameObject);
        }
        if(pEffect.mCurrentCounter >= pEffect.mEffectRepeatCount)
        {
            return;
        }
        Color aColor = pEffect.mEffectColor;
        aColor.a = mImageDefaultAlpha;
        mGameHUD.mVignetteImage.color = aColor;
        LeanTween.value(mGameHUD.mVignetteImage.gameObject, mImageDefaultAlpha, pEffect.mEffectColor.a, pEffect.mEffectUpTime).
            setDelay(pEffect.mEffectUpDelay)
            .setOnComplete(() =>
            {
                LeanTween.value(mGameHUD.mVignetteImage.gameObject, pEffect.mEffectColor.a, mImageDefaultAlpha, pEffect.mEffectDownTime)
                .setDelay(pEffect.mEffectDownDelay)
                .setOnComplete(() => VignetteEffectCaller(pEffect))
                .setOnUpdate(UpdateVignetteAlpha);
            })
            .setOnStart(() => { pEffect.mCurrentCounter++; })
            .setOnUpdate(UpdateVignetteAlpha);
    }

    void UpdateVignetteAlpha(float pValue)
    {
        Color aColor = mGameHUD.mVignetteImage.color;
        aColor.a = pValue;
        mGameHUD.mVignetteImage.color = aColor;
    }

}
