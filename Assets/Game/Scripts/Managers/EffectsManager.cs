using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : Singleton<EffectsManager>
{
    IngameHUD mGameHUD = null;
    [SerializeField] MenuClassifier mInGameHUD;
    [SerializeField] Vector3 mScoreScaleUp = new Vector3(3,3,3);
    [SerializeField] Vector3 mScoreMultiplierScaleUp = new Vector3(3,3,3);
    Vector3 mDefaultScoreScale = Vector3.one;
    Vector3 mDefaultScoreMultiplierScale = Vector3.one;
    [SerializeField] float mScaleUpEffectTime = 0.3f;
    [SerializeField] float mScaleUpEffectDelay = 0.2f;
    [SerializeField] float mScaleDownEffectTime = 0.3f;
    [SerializeField] float mScaleDownEffectDelay = 0.2f;
    void Start()
    {
        mGameHUD = MenuManager.Instance.GetMenu<IngameHUD>(mInGameHUD);
        mDefaultScoreScale = mGameHUD.mScoreText.rectTransform.localScale;
        mDefaultScoreMultiplierScale = mGameHUD.mScoreMultiplierText.rectTransform.localScale;
    }

    public void ScoreAddedEffect()
    {
        ScaleRectEffect(mGameHUD.mScoreText.rectTransform, mScoreScaleUp, mDefaultScoreScale);
    }

    public void ScoreMultiplierAddedEffect()
    {
        ScaleRectEffect(mGameHUD.mScoreMultiplierText.rectTransform, mScoreMultiplierScaleUp, mDefaultScoreScale);
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

}
