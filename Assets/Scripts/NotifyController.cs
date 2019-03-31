using UnityEngine;
using UnityEngine.UI;

public class NotifyController : MonoBehaviour {

	public static void turnOnNotif(int time)
    {
        AndroidNotification.SendNotification(1, time, "Отличный заголовок", "Тут может быть ваша реклама!", "Посмотри сюда!",Color.white,true,AndroidNotification.NotificationExecuteMode.ExactAndAllowWhileIdle,true,2,true,0,500);
    }
    public static void turnOnNotifNew(int time)
    {
        AndroidNotification.SendNotification(1, time, "Отличный заголовок", "Тут может быть ваша реклама!", "Посмотри сюда!", Color.white, true, AndroidNotification.NotificationExecuteMode.Inexact, true, 2, true, 0, 500);
    }
}