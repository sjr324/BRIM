using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRIM.BackendClassLibrary
{
    public static class NotificationManager
    {
        private static int currentID = 0;
        private static Notification latest;
        private static List<Notification> notifications = new List<Notification>();

        public static void AddNotification(string message)
        {
            Notification notification=new Notification(currentID, message);
            notifications.Add(notification);
            latest = notification;
            currentID++;

        }

        public static void RemoveNotification(int id)
        {
           notifications.Remove(notifications.Where(n => n.ID == id).First());
        }
        
        public static void MarkRead(int id)
        {
            notifications.Where(n => n.ID == id).First().StatusCode=1;
        }

        public static Notification GetMostRecent()
        {
            return latest;
        }
    }
}
