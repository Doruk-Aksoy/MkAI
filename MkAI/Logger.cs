using System.IO;
using System.Reflection;

namespace MkAI
{
    public class Logger
    {
        public string filePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "Log.txt";
        public void Log(string message)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath)) {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
        }
    }
}
