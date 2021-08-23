using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using UnityEngine.Events;


/* AdsUnit_Admob_Interstitial - юнит рекламы.
Рекламная сеть: Admob.    
Тип: Interstitial.

Задача: 
- Базовый юнит рекламы, которым управляют извне.
 
 */


public class AdsUnit_Admob_Interstitial : AdsUnit_Base
{
    
    // ID рекламы
    string adsID_current;
    const string ADSID_TEST = "ca-app-pub-3940256099942544/1033173712"; // тестовый ID, предоставленный сетью Адмоб для рекламы Interstitial.
    public string AdsID_WORK; // рабочий ID


    InterstitialAd interstitial; // сам рекламный модуль Interstitial.


    [Space(3.0f)]
    
    public UnityEvent OnAdLoaded; // Можно вставить сюда свой же метод вызова этой рекламы на показ.
    public UnityEvent OnAdFailed;
    public UnityEvent OnAdShow; // начало показа рекламы.  (можно в это время другую рекламу в виде баннера прятать).
    public UnityEvent OnAdShowEnded; // закончен показ




    void Awake()
    {
        AdsNetworkName = "Admob";
        adsType = AdsType.interstitial; 
    }




    // Метод назначения айди рекламы
    //
    //Определяем айди рекламы исходя из: 1) платформа разработки 2) глобальные настройки (Тест или Реальный показ). 
    void Ads_Initialize()
    {
#if UNITY_ANDROID // если платформа Андроид
        adsID_current = Ads_Settings.IsTesting ? ADSID_TEST : AdsID_WORK;
#else
        adsID_current = "unexpected_platform";
#endif
        // Логирование.
        if (Debug_global_vals.isDebug) Debug.Log("Рекл. модуль " + gameObject.name + ". Рекл. сеть: " + AdsNetworkName + ". Тип рекл.: " + adsType + ". \n Айди рекламы: " + adsID_current);
    }










    #region interface IAdsController implementation

    public override bool Ads_isLoaded
    {
        get
        {
            if (interstitial != null) return interstitial.IsLoaded(); // эта проверка нужна, ибо экземпляр класса AdUnit создается при команде загрузки. Соотв до загрузки этот метод не может опросить AdUnit на тему IsLoaded()
            else return false;
        }
    }

    // Метод для загрузки рекламы
    public override void Ads_Load()
    {
        // 0. Проверка: можем ли мы загружать рекламу?
        if (Ads_Limiter.Limit_Interstitial & Ads_Limiter.AdsCountSummary_Interstitial <= 0) return;
        // Логирование.
        if (Debug_global_vals.isDebug) Debug.Log("Рекл. модуль " + gameObject.name + ". Рекл. сеть: " + AdsNetworkName + ". Тип рекл.: " + adsType + ". \n Загружаю рекламу");
        if (Debug_global_vals.isDebug & Ads_Limiter.Limit_Interstitial) Debug.Log("Рекл. модуль " + gameObject.name + ". Рекл. сеть: " + AdsNetworkName + ". Тип рекл.: " + adsType 
            + ". \n Осталось показов Interstitial: " + Ads_Limiter.AdsCountSummary_Interstitial);

        // 1. Инициализируем ID рекламы. (Здесь, а не в Start(), потому что другой скрипт может попросить загрузить рекламу раньше.)
        Ads_Initialize(); 

        // 2. Инициализируем рекламный блок по Айди.
        this.interstitial = new InterstitialAd(adsID_current);

        // 3. Подписываем события
        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // 4. Загружаем рекламу
        // 4.1 Создаем запрос. УЧИТЫВАЕМ GDPR
        AdRequest request;

        if (Ads_Settings.GDPR_UserChoice_Personal)
        {
            request = new AdRequest.Builder().Build();  // персонализированная реклама
        }
        else
        {
            request = new AdRequest.Builder().AddExtra("npa", "1").Build(); // НЕ персонализированная реклама
        }


        // 4.2 Посылаем его
        this.interstitial.LoadAd(request);
    }

    public override void Ads_Show()
    {
        // 1. Проверка перед показом.
        if (interstitial == null)
        {
            if (Debug_global_vals.isDebug) Debug.Log("Рекл. модуль " + gameObject.name + ". Рекл. сеть: " + AdsNetworkName + ". Тип рекл.: " + adsType 
                + ". \n Реклама interstitial не была инициализирована перед запросом к ней и равна null. Выхожу из метода showADS()");
            return;
        }
        // 2. Показ.
        if (Ads_isLoaded)
        {
            if (Debug_global_vals.isDebug) Debug.Log("Рекл. модуль " + gameObject.name + ". Рекл. сеть: " + AdsNetworkName + ". Тип рекл.: " + adsType + ". \n Показываю рекламу");
            this.interstitial.Show();
        }
    }

    public override void Ads_Hide()
    {
        // Метод не реализуется в Interstitial.
        // Внешний контроллер не вызовет его т.к. будет проверка AdsType (interstitial не пройдет проверку);
    }



    #endregion







    #region Admob Ads Plugin Default Events
    // ================================ СОБЫТИЯ  (НАЧАЛО)
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        OnAdLoaded.Invoke();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        OnAdFailed.Invoke();
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        Ads_Limiter.CountDown_Interstitial(); // Засчитываем показ рекламы в общий счётчик. + помечаем, что первый показ Interstitial состоялся (firstpass).
        OnAdShow.Invoke();
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        OnAdShowEnded.Invoke();   
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        
    }

    // ================================ СОБЫТИЯ  (КОНЕЦ)
    #endregion
}
