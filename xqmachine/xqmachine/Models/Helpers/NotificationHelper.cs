using System;
using System.Web;

namespace xqmachine.Models.Helpers
{
    public class ModelNotification
    {
        public string Msg1_5s { get; set; }
        public string MsgType1_5s { get; set; }
        public string Msg3s { get; set; }
        public string MsgType3s { get; set; }
        public string Msg5s { get; set; }
        public string MsgType5s { get; set; }
    }
    public class Notification
    {
        public static bool Has_flash()
        {
            if (HttpContext.Current.Application["Notification"].Equals("")) return false;
            return true;
        }
        public static void SetNotification1_5s(string msg1_5s, string msgType1_5s)
        {
            var tb = new ModelNotification
            {
                Msg1_5s = msg1_5s,
                MsgType1_5s = msgType1_5s
            };
            HttpContext.Current.Application["Notification"] = tb;
        }
        public static void SetNotification3s(string msg3s, string msgType3s)
        {
            var tb = new ModelNotification
            {
                Msg3s = msg3s,
                MsgType3s = msgType3s
            };
            HttpContext.Current.Application["Notification"] = tb;
        }

        public static void SetNotification5s(string msg5s, string msgType5s)
        {
            var tb = new ModelNotification
            {
                Msg5s = msg5s,
                MsgType5s = msgType5s
            };
            HttpContext.Current.Application["Notification"] = tb;
        }
        public static ModelNotification Get_flash()
        {
            var Notifi = (ModelNotification)HttpContext.Current.Application["Notification"];
            HttpContext.Current.Application["Notification"] = "";
            return Notifi;
        }
    }
}