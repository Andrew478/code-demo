using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* IAdsController - интерфейс.
Задача: 
- реализовать методы, вызывающие управление рекламным модулем. (Загрузка рекламы, показать, спрятать и т.д.) Внешнее управление рекламным модулем.
 
 */
public interface IAdsController 
{
    void Ads_Load();
    void Ads_Show();
    void Ads_Hide(); // для баннера.
    bool Ads_isLoaded { get; } // готово ли к показу.
}

