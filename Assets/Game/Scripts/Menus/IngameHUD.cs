using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameHUD : Menu
{
    public Text mScoreText;
    public Text mScoreMultiplierText;
    public override void Start()
    {
        EffectsManager.Instance.SetInGameHUD(this);
        base.Start();
    }
    private void Update()
    {
        if (DataHandler.IsValidSingleton())
        {
            mScoreText.text = ((int)DataHandler.Instance.mScore.mScore).ToString();
            mScoreMultiplierText.text = "x " + ((int)DataHandler.Instance.mScore.mScoreMultiplier).ToString();
        }
    }
}
