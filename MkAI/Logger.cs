using System;
using System.IO;
using System.Reflection;

namespace MkAI
{
    [Serializable]
    public class Logger
    {
        [NonSerialized]
        private string filePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Log.txt";
        [NonSerialized]
        private bool enabled = true;

        public void setLogging(bool b)
        {
            enabled = b;
        }

        public void Log(string message)
        {
            if(enabled)
            {
                using (StreamWriter streamWriter = File.AppendText(filePath))
                {
                    streamWriter.Write("{0} {1} -- ", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            }
        }
    }
}
