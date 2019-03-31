using UnityEngine;
using System;

class AndroidNotification
{
    //***Не трогать!***//
    private const string fullClassName = "com.ahg.uanotify.UnityNotification";
    private const string mainActivityClassName = "com.unity3d.player.UnityPlayerNativeActivity";
    //***Не трогать***//

    /// <summary>
    /// Inexact (Не точный) использует `set` метод
    /// Exact (Точный) использует `setExact` метод
    /// ExactAndAllowWhileIdle использует `setAndAllowWhileIdle` метод
    /// Документация: https://developer.android.com/intl/ru/reference/android/app/AlarmManager.html
    /// </summary>
    public enum NotificationExecuteMode
    {
        Inexact = 0,
        Exact = 1,
        ExactAndAllowWhileIdle = 2
    }

    /// <summary>
    /// Отправить Push-уведомление
    /// </summary>
    /// <param name="id"></param>
    /// <param name="delay">Задержка (мининум 5 секунд)</param>
    /// <param name="title">Заголовок</param>
    /// <param name="message">Сообщение</param>
    /// <param name="ticker">Сообщение при появлении в статус баре</param>
    public static void SendNotification(int id, TimeSpan delay, string title, string message, string ticker)
    {
        SendNotification(id, (int)delay.TotalSeconds, title, message, ticker, Color.white,true,NotificationExecuteMode.Inexact);
    }
    public static void SendNotification(int id, TimeSpan delay, string title, string message, string ticker, Color32 bgColor, bool lights = true, NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
    {
        SendNotification(id, (int)delay.TotalSeconds, title, message, ticker, bgColor,true,executeMode);
    }
    /// <summary>
    /// Отправить Push-уведомление
    /// </summary>
    /// <param name="id"></param>
    /// <param name="delay">Задержка (мининум 5 секунд)</param>
    /// <param name="title">Заголовок</param>
    /// <param name="message">Сообщение</param>
    /// <param name="ticker">Сообщение при появлении в статус баре</param>
    /// <param name="bgColor">Цвет заднего фона в ARGB формате. Информация: "https://developer.android.com/reference/android/graphics/Color.html#BLACK"</param>
    /// <param name="sound">Включить звук уведомления?</param>
    /// <param name="vibrate">Включить вибрацию?</param>
    /// <param name="lights">Включить световое оповещение?</param>
    /// <param name="executeMode">Способ выполнения</param>
    public static void SendNotification(int id, TimeSpan delay, string title, string message, string ticker, Color32 bgColor, bool lights = true, NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact, bool sound = true, int soundIndex = 2)
    {
        SendNotification(id, (int)delay.TotalSeconds, title,message,ticker,bgColor,lights,executeMode,sound,soundIndex);
    }
    /// <summary>
    /// Отправить Push-уведомление
    /// </summary>
    /// <param name="id"></param>
    /// <param name="delay">Задержка (мининум 5 секунд)</param>
    /// <param name="title">Заголовок</param>
    /// <param name="message">Сообщение</param>
    /// <param name="ticker">Сообщение при появлении в статус баре</param>
    /// <param name="bgColor">Цвет заднего фона в ARGB формате. Информация: "https://developer.android.com/reference/android/graphics/Color.html#BLACK"</param>
    /// <param name="sound">Включить звук уведомления?</param>
    /// <param name="vibrate">Включить вибрацию?</param>
    /// <param name="vibrateDelay">Задержка перед вибрацией</param>
    /// <param name="vibrateTime">Длительность вибрации</param>
    /// <param name="lights">Включить световое оповещение?</param>
    /// <param name="executeMode">Способ выполнения</param>
    public static void SendNotification(int id, long delay, string title, string message, string ticker, Color32 bgColor, bool lights = true, NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact, bool sound = true, int soundIndex = 2, bool vibrate = true, long vibrateDelay = 1000L, long vibrateTime = 1000L)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
            pluginClass.CallStatic("SetNotification", id, delay * 1000L, title, message, ticker, sound ? 1 : 0, soundIndex, vibrate ? 1 : 0, vibrateDelay, vibrateTime, lights ? 1 : 0, bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, mainActivityClassName);
        
#endif
    }
    /// <summary>
    /// Отменить Push-уведомление
    /// </summary>
    /// <param name="id"></param>
    public static void CancelNotification(int id)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) 
            pluginClass.CallStatic("CancelNotification", id);
#endif
    }
    /// <summary>
    /// Отменить все Push-уведомления
    /// </summary>
    public static void CancelAllNotifications()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
            pluginClass.CallStatic("CancelAll");
#endif
    }
}