using GuruLaghima;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;

public class PointsCounter : MonoBehaviour
{
  // public System.Action OnUpdated;
  [SerializeField] MMFeedbacks mainPointsCounterGainFeedback;
  [SerializeField] MMFeedbacks mainPointsCounterLoseFeedback;
  [SerializeField] NumberCounter counterUpdater;
  [SerializeField] string currencyKey;
  private float _bankMoney;
  private float prev_bankMoney;

  Currency softCurrency = null;


  public float BankMoney
  {
    get
    {
      return _bankMoney;
    }
    set
    {
      _bankMoney = value;
      prev_bankMoney = _bankMoney;
    }
  }

  #region monobehaviours

  private void Start()
  {
    if (GameFoundationSdk.IsInitialized)
    {
      softCurrency = GameFoundationSdk.catalog.Find<Currency>(currencyKey);
    }
  }

  void OnDisable()
  {
    GameFoundationSdk.wallet.balanceChanged -= PlayFeedbacks;
  }

  #endregion
  [ContextMenu("Test Func")]
  public void TestFunc()
  {
    GameFoundationSdk.wallet.Add(softCurrency, 100);
    // PlayFeedbacks(softCurrency, 2000);
  }

  #region Public methods


  public void OnGameFoundationInitialized()
  {
    softCurrency = GameFoundationSdk.catalog.Find<Currency>(currencyKey);

    GameFoundationSdk.wallet.balanceChanged += PlayFeedbacks;

    // LoadBankData();
  }


  // this function should be called from the MMfeedbacks so as to controll its timing from there
  public void UpdateCounterNumbers()
  {
    counterUpdater.Value = Mathf.RoundToInt(BankMoney);
    MyDebug.Log("PointsCounter:: counterUpdater.Value", counterUpdater.Value);

  }

  #endregion

  #region private methods

  void LoadBankData()
  {
    if (GameFoundationSdk.IsInitialized)
    {
      Currency softCurrency = GameFoundationSdk.catalog.Find<Currency>(Keys.CURRENCY_SOFT);
      // PlayFeedbacks(softCurrency, GameFoundationSdk.wallet.Get(softCurrency));
      PlayFeedbacks(GameFoundationSdk.wallet.Get(softCurrency));
    }
  }


  void PlayFeedbacks(IQuantifiable currency, long value)
  {
    // MyDebug.Log("PointsCounter:: prev value", value);
    Currency softCurrency = null;
    softCurrency = GameFoundationSdk.catalog.Find<Currency>(Keys.CURRENCY_SOFT);
    // MyDebug.Log("PointsCounter:: softCurrency.quantity", softCurrency.quantity);
    // MyDebug.Log("PointsCounter:: prev_bankMoney", prev_bankMoney);
    if (((Currency)currency) == softCurrency)
    {
      // if new amount is bigger than previous amount play the gain feedback
      if (softCurrency.quantity > prev_bankMoney)
      {

        // maybe an animation of coins rapidly falling into the counter, embiggening it slightly with each coin
        // MyDebug.Log("PointsCounter:: playing gain feedback ");
        BankMoney = softCurrency.quantity;
        if (mainPointsCounterGainFeedback)
        {
          try
          {
            mainPointsCounterGainFeedback.PlayFeedbacks();
          }
          catch { }
        }
      }
      else if (softCurrency.quantity < prev_bankMoney)
      {
        // maybe an animation of coins rapidly falling out of the counter
        // MyDebug.Log("PointsCounter:: playing lose feedback  ");
        BankMoney = softCurrency.quantity;
        if (mainPointsCounterLoseFeedback)
        {
          try
          {
            mainPointsCounterLoseFeedback.PlayFeedbacks();
          }
          catch { }
        }
      }
    }
    BankMoney = softCurrency.quantity;

  }
  public void PlayFeedbacks(long value)
  {
    // MyDebug.Log("PointsCounter:: prev value", value);
    // MyDebug.Log("PointsCounter:: softCurrency.quantity", softCurrency.quantity);
    // MyDebug.Log("PointsCounter:: prev_bankMoney", prev_bankMoney);
    // if new amount is bigger than previous amount play the gain feedback
    if (softCurrency != null)
    {


      if (softCurrency.quantity > prev_bankMoney)
      {

        // maybe an animation of coins rapidly falling into the counter, embiggening it slightly with each coin
        // MyDebug.Log("PointsCounter:: playing gain feedback ");
        BankMoney = softCurrency.quantity;
        if (mainPointsCounterGainFeedback)
        {
          try
          {
            mainPointsCounterGainFeedback.PlayFeedbacks();
          }
          catch { }
        }
      }
      else if (softCurrency.quantity < prev_bankMoney)
      {
        // maybe an animation of coins rapidly falling out of the counter
        // MyDebug.Log("PointsCounter:: playing lose feedback  ");
        BankMoney = softCurrency.quantity;
        if (mainPointsCounterLoseFeedback)
        {
          try
          {
            mainPointsCounterLoseFeedback.PlayFeedbacks();
          }
          catch { }
        }
      }
      BankMoney = softCurrency.quantity;
    }

  }
  #endregion


}