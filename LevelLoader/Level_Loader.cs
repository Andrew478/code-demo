using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



// Универсальный загрузчик игрового уровня.

/* FAQ (краткое руководство к работе).

    Как работает?
    Шаг1. Указываем имя или индекс сцены для загрузки. (Вызов метода Set_sceneToLoad()).
    Шаг2. Вызываем загрузку уровня. (Вызов метода LoadLevel() ).
    
    Опции и настройки работы (кастомизация).
    
    - можно выбрать тип загрузки сцены: 1) Моментальный 2) Асинхронный.
    - можно включить отображение: 1) Полосы загрузки. 2) Процентов загрузки. (работает для асинхронного типа загрузки сцены).
    - Показ рекламы - 2 вида загрузки сцены: 1) Без показа рекламы (сразу загрузка). 2) Сначала показ рекламы (если готова к показу), потом загрузка.
    - Отложеная загрузка после появления загрузочного экрана (Time_beforeRealLoading) если в загрузочном экране надо что-то успеть показать.
    - Показ рекламы с определенным шансом (от 1% до 99% из 100%)

    ВАЖНО!:
    1. Для возможности показа рекламы, скрипт рекламы должен реализовывать интерфейс IAdsController (вызов метода Ads_Show()), 
    либо аналогичный свой интерфейс (просто исправить имя интерфейса в этом скрипте).
*/








public class Level_Loader : MonoBehaviour
{
    [Header("Имя (индекс) сцены для загрузки.")]
    string sceneToLoad_name = "";
    int sceneToLoad_index = 0;

    [Space]
    
    public GameObject AdsUnit; // GameObject рекламы. Для вызова показа рекламы. (должен реализовывать интерфейс IAdsController).
    IAdsController adsCTRL;

    enum SceneToLoad_nameType
    {
        loadSceneByName,
        loadSceneByIndex
    }
    SceneToLoad_nameType sceneToLoad_nameType = SceneToLoad_nameType.loadSceneByIndex; // Если вызвать загрузчик без указания сцены, вызовется нулевая сцена по индексу (защита от ошибки).
    


    // Реклама
    public bool ShowAdsWhileLoading = false; 
    
    // Загрузочный экран
    public bool ShowLoadScreen = false; 
    public float Time_beforeRealLoading = 0.1f; // Задержка перед началом загрузки. (Если в загрузочном экране надо что-то успеть показать).
    public UnityEvent Show_LoadScreen; // Плашка загрузочного экрана. Тут либо вызывается скрипт переключения пункта меню на плашку, либо сам геймобъект.

    // Асинхронная или моментальная загрузка
    public enum LoadGameType
    {
        instant,
        async
    }
    public LoadGameType loadGameType = LoadGameType.instant;


    // Внешние показатели процента загрузки (текст "проценты" и лоадинг бар)
    [Header("Какие показывать показатели загруки")]
    public bool show_percent;
    public bool show_loadBar;

    // сами показатели (UI элементы)
    public Text text_loadPercent; 
    public Slider loadBar; 

    [Space]

    [Header("Показ рекламы перед загрузкой с опред. шансом")]
    public bool ShowAds_byChance = false; 

    [Range(1.0f, 99.0f)]
    public float ShowAds_chance = 99.0f;




    void Start()
    {
        adsCTRL = AdsUnit.GetComponent<IAdsController>();
        if (ShowAdsWhileLoading && adsCTRL == null) Debug.LogError("Задан неверный объект рекламы для показа. Отсутствует реализация интерфейса.");

        if (show_percent && text_loadPercent == null) Debug.LogError("Не назначен текст для отображения процентов.");
        if (show_loadBar && loadBar == null) Debug.LogError("Не назначен лоадбар");
    }




    // Задать имя сцены для загрузки или индекс
    // Вызывается кнопкой загрузки игровой сцены
    public void Set_sceneToLoad(string name)
    {
        sceneToLoad_name = name;
        sceneToLoad_nameType = SceneToLoad_nameType.loadSceneByName;
    }
    public void Set_sceneToLoad(int sc_index)
    {
        sceneToLoad_index = sc_index;
        sceneToLoad_nameType = SceneToLoad_nameType.loadSceneByIndex;
    }




    // Главный входной метод показа рекламы и загрузки игровой сцены. Вызывается кнопкой загрузки игровой сцены (после метода Set_sceneToLoad())
    public void LoadLevel()
    {
        if (ShowAdsWhileLoading) LoadScene_withAds_part01_beforeAds();
        else LoadScene_noAds();
    }


    void LoadScene_noAds()
    {
        StartCoroutine(loadGameScene_cor());
    }

    // Здесь происходит непосредственно загрузка игрового уровня (сцены).
    IEnumerator loadGameScene_cor()
    {
        // Показываем "плашка загрузки".
        if(ShowLoadScreen) Show_LoadScreen.Invoke();
        yield return null;// Пропуск до следующего кадра (чтобы плашка загрузки успела появиться)

        // Эмулируем ожидание загрузки (доп. время загрузки).
        yield return new WaitForSeconds(Time_beforeRealLoading);
        // Запускаем асинхронную фоновую загрузку.
        switch (loadGameType)
        {
            case LoadGameType.instant:
                LoadScene_instant();
                break;
            case LoadGameType.async:
                LoadScene_async();
                break;
            default:
                LoadScene_instant();
                break;
        }
    }

    void LoadScene_instant()
    {
        if(sceneToLoad_nameType == SceneToLoad_nameType.loadSceneByName) SceneManager.LoadScene(sceneToLoad_name);
        else SceneManager.LoadScene(sceneToLoad_index);
    }

    void LoadScene_async()
    {

        AsyncOperation operation;
        if (sceneToLoad_nameType == SceneToLoad_nameType.loadSceneByName) operation = SceneManager.LoadSceneAsync(sceneToLoad_name);
        else operation = SceneManager.LoadSceneAsync(sceneToLoad_index);

        // Показ процентов загрузки (если выбрано что их нужно показывать).
        while (!operation.isDone)
        {
            // operation.progress дает значения от 0.0 до 0.9. В этом действии мы их клампим в от 0.0 до 1.0
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // отображаем нужные показатели игры в зависимости от того, что выбрано в настройках
            if (show_percent) text_loadPercent.text = progress * 100f + "%";
            if (show_loadBar) loadBar.value = progress;
        }
    }



    // Версия загрузки игровой сцены с рекламой.
    void LoadScene_withAds_part01_beforeAds()
    {
        // Решаем, загружена ли реклама. Исходя из этого решаем, что делать дальше.
        if (adsCTRL.Ads_isLoaded)
        {
            if (ShowAds_byChance) // Используем шанс на показ рекламы в %
            {
                float chance = Random.Range(1.0f, 99.0f);
                if (chance <= ShowAds_chance) adsCTRL.Ads_Show();
                else LoadScene_withAds_part02_afterAds();
            }
            else adsCTRL.Ads_Show(); // Если реклама показывается в 100% случаев (опция шанса выключена).
        }


        else LoadScene_withAds_part02_afterAds(); // Если реклама не готова к показу. То сразу загружаем игровой уровень.
    }

    // Вызывается событием окончания показа рекламы извне.
    public void LoadScene_withAds_part02_afterAds()
    {
        StartCoroutine(loadGameScene_cor());
    }
}
