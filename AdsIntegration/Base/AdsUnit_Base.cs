using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* AdUnit_Base - абстрактный класс рекламного модуля (interstitial, rewarded, banner).

Содержит базовые сведения, общие для всех модулей и интерфейс управления.
 */

[DisallowMultipleComponent] // Каждый рекламный Юнит должен висеть на отдельном для него геймобъекте.
public abstract class AdsUnit_Base : MonoBehaviour, IAdsController
{
    [HideInInspector]
    public string AdsNetworkName; // имя рекламной сети (Адмоб, Юнити).
    public string AdsUnitName; // имя рекламного юнита в панели Адмоб или Юнити.

    [TextArea]
    public string AdsUnit_Decription;

    public enum AdsType
    {
        banner,
        interstitial,
        rewarded,
        other
    }

    [HideInInspector]
    public AdsType adsType;


    #region interface IAdsController implementation
    public abstract bool Ads_isLoaded { get; }
    public abstract void Ads_Load();
    public abstract void Ads_Show();
    public abstract void Ads_Hide();
    #endregion
}
