using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
  public string m_sCurrentAdId;
  public string m_sGameId;
  [SerializeField] Action Reward;
    void Start()
    {
      //InitializeAds();
    }

    
    void InitializeAds()
    {
      if (!Advertisement.isInitialized)
      {
      Advertisement.Initialize(m_sGameId, false, this);
      }
    }

  public void OnInitializationComplete()
  {
    Debug.Log("Initialized");
    Advertisement.Load(m_sCurrentAdId, this);
  }

  public void OnInitializationFailed(UnityAdsInitializationError error, string message)
  {

    Debug.Log("UnityAdsInitializationError : " + error);
  }

  public void OnUnityAdsAdLoaded(string placementId)
  {
    Debug.Log("Ad loaded: " + placementId);
    
  }

  public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
  {
    Debug.Log("UnityAdsLoadError  : " + error);
  }

  public void ShowAd()
  {
    Advertisement.Show(m_sCurrentAdId, this);
  }

  public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
  {
    
  }

  public void OnUnityAdsShowStart(string placementId)
  {
  }

  public void OnUnityAdsShowClick(string placementId)
  {
  }

  public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
  {
    
    if(placementId.Equals(m_sCurrentAdId) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
    {
      Debug.Log("Completyeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
      Reward.Invoke();
    }
    else
    {
      Debug.Log("NOOOOOOOOOOOOOOO Completyeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
    }
  }
}
