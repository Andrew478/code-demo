using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Ads_Settings - класс глобальных настроек.
Рекламная сеть: Admob.    
Тип: Interstitial.

Задача: 
- Переключение рекламы из режима "Тест" и "Реальный показ".
 
 */

public static class Ads_Settings
{
    // Тест рекламы или не тест
    static bool isTesting = true; // реклама находится в режиме "Тест (true)" или "Реальный показ рекламы (false)".

    public static bool IsTesting 
    { 
        get { return isTesting; } 
    }

    // GDPR
    static bool GDPR_userChoice_personal = true; // true - показывать персонализированную рекламу. false - не персонализированную.

    public static bool GDPR_UserChoice_Personal
    {
        get { return GDPR_userChoice_personal; }
        set { GDPR_userChoice_personal = value; }
    }

}
