using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Structure that holds the audio clip and the audio group of the clip
/// </summary>
[Serializable]
public struct AudioData 
{
	public AudioClip mAudioClip;
	public string mAudioGroup;
} 

/// <summary>
/// Structure that holds the way of fading in and out of the audio sources
/// </summary>
public struct AudioFadeData
{
	public AudioSource mAudioSource;
	public string mAudioID;
	public float mDv;
	public bool mFadingIn;
	public float mDelay;
}
/// <summary>
/// The object of this class is the audio source that is currently playing
/// </summary>
public class AudioCurrentlyPlaying
{
	public AudioSource mAudioSource;
	public float mNonPlayingTimer;

	public AudioCurrentlyPlaying(AudioSource pAudioSrc)
	{
		mAudioSource = pAudioSrc;
		mNonPlayingTimer = 0;
	}
}

/// <summary>
/// The structure to hold the mixer data for easier instantiation
/// </summary>
[Serializable]
public struct AudioMixerData
{
	public AudioMixerGroup mMixerGroup;
	public string mName;
	public string mExposedVolVariableName;
}

/// <summary>
/// Class with audio groups for improved readability
/// </summary>
public class AudioGroups
{
	public const int Music = 0;
	public const int Sfx = 1;
	public const int AmbientMusic = 2;
	public const int UI = 3;
	public const int GroupCount = 4;
}

/// <summary>
/// Manager of Audio in the entire 2D game
/// </summary>
public class AudioManager : Singleton<AudioManager> 
{
    public AudioMixer mMixer;
	public int[] AudioSrcSizes;
	public AudioMixerData[] mAudioMixerData;
	public AudioData[] mAudioData;


	Dictionary<string, int> mAudioDataDic		 		= new Dictionary<string, int>();
	Dictionary<string, AudioSource[]> mAudioSources		= new Dictionary<string, AudioSource[]>();
	Dictionary<string, AudioCurrentlyPlaying> mAudioSrcPlaying	= new Dictionary<string, AudioCurrentlyPlaying>();

	List<AudioFadeData> mFadingData = new List<AudioFadeData>();

	float mTimerUpdatePlaying = 0;


	/// <summary>
    /// Instantiates the audio sources and assigns the above dictionaries for optimization
    /// </summary>
	void Start () 
	{

		if(mAudioData != null && mAudioData.Length > 0)
		{
			for(int aIx = 0; aIx < mAudioData.Length; aIx++)
			{
				if(mAudioData[aIx].mAudioClip == null)
                {
                    continue;
                }
				string aAudioClipName = mAudioData[aIx].mAudioClip.name;
				mAudioDataDic[aAudioClipName] = aIx;
			}
		}

		if(mAudioMixerData != null && mAudioMixerData.Length > 0)
		{

			for(int aIx = 0; aIx < AudioGroups.GroupCount; aIx++)
			{
				int aNumSrc = 10;
				if(AudioSrcSizes != null && AudioSrcSizes.Length > aIx)
                {
                    aNumSrc = AudioSrcSizes[aIx];
                }

				AudioSource[] aSrcVec = new AudioSource[aNumSrc];

				for(int aJx = 0; aJx < aSrcVec.Length; aJx++)
				{
					var aGObj = new GameObject(string.Format("AudioSrc_Grp{0}_Ix_{1}", aIx, aJx));
					aGObj.transform.SetParent(transform);

					aSrcVec[aJx] = aGObj.AddComponent<AudioSource>();
					aSrcVec[aJx].outputAudioMixerGroup = mAudioMixerData[aIx].mMixerGroup;
					aSrcVec[aJx].playOnAwake = false;
				}

				mAudioSources[mAudioMixerData[aIx].mName] = aSrcVec;
			}
		}
	
	}



	/// <summary>
    /// Checks all playing audiosources timers and stops them when their timer runs out or fades them out
    /// </summary>
	void Update () 
	{
		
		mTimerUpdatePlaying -= Time.deltaTime;
		if(mTimerUpdatePlaying <= 0 && !AudioListener.pause)
		{
			mTimerUpdatePlaying = 0.1f;
			List<string> aKeysToRemove = new List<string>();
			foreach(var aKeyValPair in mAudioSrcPlaying)
			{
				var aPlayingData = aKeyValPair.Value;
				if(aPlayingData.mAudioSource.time < (aPlayingData.mAudioSource.clip.length - 0.1f) || aPlayingData.mAudioSource.isPlaying)
				{
					aPlayingData.mNonPlayingTimer = 0;
					continue;
				}
				else
                {
					aPlayingData.mNonPlayingTimer += Time.deltaTime;
					if(aPlayingData.mNonPlayingTimer < 2.0f)
					{
						continue;
					}
				}

				aKeysToRemove.Add(aKeyValPair.Key);
			}

			if(aKeysToRemove.Count > 0)
			{
				foreach(var aKey in aKeysToRemove)
				{
					mAudioSrcPlaying.Remove(aKey);

				}
				aKeysToRemove.Clear();
			}
		}


		if(mFadingData.Count > 0)
		{
			for(int aI = 0; aI < mFadingData.Count; aI++)
			{
				var aFadeInfo = mFadingData[aI];
				aFadeInfo.mDelay -= Time.deltaTime;
				if(aFadeInfo.mDelay > 0)
				{
					mFadingData[aI] = aFadeInfo;
					continue;
				}

				aFadeInfo.mAudioSource.volume += aFadeInfo.mDv * Time.deltaTime;

				if(aFadeInfo.mFadingIn)
				{
					if(aFadeInfo.mAudioSource.volume >= 0.999f)
					{
						aFadeInfo.mAudioSource.volume = 1.0f;
						mFadingData.RemoveAt(aI);
					}
				}
				else {
					if(aFadeInfo.mAudioSource.volume <= 0.001f)
					{
						aFadeInfo.mAudioSource.volume = 0f;
						aFadeInfo.mAudioSource.Stop();
						mFadingData.RemoveAt(aI);

						if(mAudioSrcPlaying.ContainsKey(aFadeInfo.mAudioID))
                        {
                            mAudioSrcPlaying.Remove(aFadeInfo.mAudioID);
                        }	
					}

				}
			}
		}

	
	}
	
    /// <summary>
    /// Function to get an empty audio source of the given audio group
    /// </summary>
    /// <param name="pGroup">audio group</param>
    /// <returns>audio source</returns>
	AudioSource GetEmptySource(string pGroup)
	{
        if (!mAudioSources.ContainsKey(pGroup))
        {
            return null;
        }

		var aSources = mAudioSources[pGroup];

		for(int aI = 0; aI < aSources.Length; aI++)
		{
            if (!aSources[aI].isPlaying)
            {
                return aSources[aI];
            }	
		}

		return null;
	}

    /// <summary>
    /// Function called to play audio sfx
    /// </summary>
    /// <param name="pSoundID">the name of the audio clip</param>
    /// <param name="pDelay">the delay before playing sfx</param>
	public void PlaySFX(string pSoundID, float pDelay = 0)
	{
		if(mAudioDataDic.ContainsKey(pSoundID))
		{
			var aAudioData = mAudioData[mAudioDataDic[pSoundID]];
			var aAudioSrc = GetEmptySource(aAudioData.mAudioGroup);

            if (aAudioSrc == null)
            {
                return;
            }
			aAudioSrc.loop = false;

			if(pDelay <= 0.001f)
			{
				aAudioSrc.PlayOneShot(aAudioData.mAudioClip);
			}
			else
            {
				aAudioSrc.playOnAwake = false;
				aAudioSrc.clip = aAudioData.mAudioClip;
				aAudioSrc.PlayDelayed(pDelay);
			}
		}
	}

    /// <summary>
    /// Play any sfx in a looped manner
    /// </summary>
    /// <param name="pSoundID">audio clip name</param>
    /// <param name="pDelay">delay before playing</param>
    /// <returns></returns>
	public void PlaySFXLooped(string pSoundID, float pDelay = 0)
	{
		if(mAudioDataDic.ContainsKey(pSoundID))
		{
			var aAudioData = mAudioData[mAudioDataDic[pSoundID]];
			var aAudioSrc = GetEmptySource(aAudioData.mAudioGroup);

            if (aAudioSrc == null)
            {
                return/* null*/;
            }	


			if(pDelay <= 0.001f)
			{
				aAudioSrc.clip = aAudioData.mAudioClip;
				aAudioSrc.Play();
				aAudioSrc.loop = true;
			}
			else
            {
				aAudioSrc.playOnAwake = false;
				aAudioSrc.clip = aAudioData.mAudioClip;
				aAudioSrc.PlayDelayed(pDelay);
				aAudioSrc.loop = true;
			}

			mAudioSrcPlaying[pSoundID] = new AudioCurrentlyPlaying(aAudioSrc);
			//return aAudioSrc;
		}

		//return null;

	}

    /// <summary>
    /// Stop the mentioned sound id to be playing
    /// </summary>
    /// <param name="pSoundID">name of sound clip</param>
	public void StopSound(string pSoundID)
	{
		if(!mAudioSrcPlaying.ContainsKey(pSoundID))
		{
			return;
		}

		mAudioSrcPlaying[pSoundID].mAudioSource.Stop();
		mAudioSrcPlaying.Remove(pSoundID);
	}


    /// <summary>
    /// Play the mentioned sound with delay and loop or without them
    /// </summary>
    /// <param name="pSoundID">sound name</param>
    /// <param name="pLoop">if true then audio loops</param>
    /// <param name="pDelay">delay to start</param>
    /// <returns></returns>
	public void PlayMusic(string pSoundID, bool pLoop, float pDelay = 0)
	{
		if(mAudioDataDic.ContainsKey(pSoundID))
		{
			var aAudioData = mAudioData[mAudioDataDic[pSoundID]];
			var aAudioSrc = GetEmptySource(aAudioData.mAudioGroup);

            if (aAudioSrc == null)
            {
                return/* null*/;
            }	

			aAudioSrc.playOnAwake = false;
			aAudioSrc.clip = aAudioData.mAudioClip;
			aAudioSrc.loop = pLoop;
			if(pDelay <= 0.001f)
			{
				aAudioSrc.Play();
			}
			else
            {
				aAudioSrc.PlayDelayed(pDelay);
			}

			mAudioSrcPlaying[pSoundID] = new AudioCurrentlyPlaying(aAudioSrc);
			//return aAudioSrc;

		}

		//return null;

	}

    /// <summary>
    /// Fade the said sound in the given timer and delay
    /// </summary>
    /// <param name="pSoundID">name of sound</param>
    /// <param name="pFadeTimer">timer of fading</param>
    /// <param name="pFadeOut">if true then fade out</param>
    /// <param name="pDelay">delay in fading</param>
	public void FadeSound(string pSoundID, float pFadeTimer, bool pFadeOut, float pDelay = 0)
	{
		if(!mAudioSrcPlaying.ContainsKey(pSoundID))
		{
			return;
		}


		int aFoundIx = -1;
		for(int aIx = 0; aIx < mFadingData.Count; aIx++)
		{
			if(mFadingData[aIx].mAudioID == pSoundID)
			{
				aFoundIx = aIx;
				break;
			}
		}

		if(aFoundIx > -1)
		{
			mFadingData.RemoveAt(aFoundIx);

		}

		var aAudioSrc = mAudioSrcPlaying[pSoundID].mAudioSource;
		AudioFadeData aFadeData = new AudioFadeData();

		aFadeData.mAudioSource = aAudioSrc;

		if(pFadeOut)
		{
			aFadeData.mDv = -aAudioSrc.volume/pFadeTimer;
		}
		else 
		{
			if(aAudioSrc.volume > 0.99f)
			{
				aAudioSrc.volume = 0;
				aFadeData.mDv = 1/pFadeTimer;
			}
			else
            {
				aFadeData.mDv = (1 - aAudioSrc.volume)/pFadeTimer;
			}
		}

		aFadeData.mFadingIn = !pFadeOut;
		aFadeData.mDelay = pDelay;
		aFadeData.mAudioID = pSoundID;

		mFadingData.Add(aFadeData);
	}


    /// <summary>
    /// Function to set the volume of the mixer
    /// </summary>
    /// <param name="pMixerGroupName">mixer group</param>
    /// <param name="pVolume">volume</param>
	public void SetMixerGroupVolume(string pMixerGroupName, float pVolume)
	{
		for(int aI = 0; aI < mAudioMixerData.Length; aI++)
		{
			if(mAudioMixerData[aI].mName == pMixerGroupName)
			{
				mMixer.SetFloat(mAudioMixerData[aI].mExposedVolVariableName, pVolume);
				break;
			}
		}
	}

    /// <summary>
    /// Check whether the sound is playing or not
    /// </summary>
    /// <param name="pSoundID">name of sound</param>
    /// <returns>true is sound playing</returns>
	public bool IsSoundPlaying(string pSoundID)
	{
        if (!mAudioSrcPlaying.ContainsKey(pSoundID))
        {
            return false;
        }
		
		return true;

	}

}
