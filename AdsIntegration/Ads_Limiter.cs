using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Ads_Limiter - класс глобальных настроек ограничений по показу рекламы
 * .
Рекламная сеть: Любая.    
Ограничения для: Interstitial и Rewarded.

Задача: 
- ограничение общего числа рекламных показов.

 */

public static class Ads_Limiter
{
    #region Interstitial

    static bool limit_Interstitial = true;
    public static bool Limit_Interstitial { get { return limit_Interstitial; } }

    static int adsCountSummary_Interstitial = 10;
    public static int AdsCountSummary_Interstitial
    {
        get { return adsCountSummary_Interstitial; }
    }

    // обратный отсчёт после каждого показа Interstitial
    public static void CountDown_Interstitial()
    {
        if (adsCountSummary_Interstitial > 0) adsCountSummary_Interstitial--;
        firstPass_Interstitial = false;
    }

    static bool firstPass_Interstitial = true; // Первый показ ли этого типа рекламы в приложении? Нужен для случаев, когда внешний контроллер хочет отложить первый показ
    
    public static bool FirstPass_Interstitial
    {
        get { return firstPass_Interstitial; }
    }
    
    #endregion

    #region Rewarded

    static bool limit_Rewarded = true;
    public static bool Limit_Rewarded { get { return limit_Rewarded;  } }

    static int adsCountSummary_Rewarded = 10;
    public static int AdsCountSummary_Rewarded
    {
        get { return adsCountSummary_Rewarded; }
    }

    // обратный отсчёт после каждого показа Rewarded
    public static void CountDown_Rewarded()
    {
        if (adsCountSummary_Rewarded > 0) adsCountSummary_Rewarded--;
        firstPass_Rewarded = false;
    }

    static bool firstPass_Rewarded = true; // Первый показ ли этого типа рекламы в приложении? Нужен для случаев, когда внешний контроллер хочет отложить первый показ

    public static bool FirstPass_Rewarded
    {
        get { return firstPass_Rewarded; }
    }

    #endregion
}
