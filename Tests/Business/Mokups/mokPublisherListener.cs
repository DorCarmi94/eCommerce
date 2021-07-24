using System;
using System.Collections.Concurrent;
using eCommerce.Publisher;
using System.IO;

namespace Tests.Business.Mokups
{
    public class mokPublisherListener : UserObserver
    {
        private StreamWriter outputFile;
        public int count { get; private set; }
        public string FILE_NAME = "PublisherTstTxt.txt";
        public mokPublisherListener()
        {
        }
        public void Notify(string userName, ConcurrentQueue<string> message)
        {
            using (outputFile = new StreamWriter(FILE_NAME,true))
            {
                foreach (var mes in message)
                {
                    outputFile.WriteLine($"{userName} got new message: {mes}");
                    count++;
                }
            }
        }

        public int GetNumberOfFileLines()
        {
            using (var readOutputFile = new StreamReader(FILE_NAME))
            {
                var file=readOutputFile.ReadToEnd();
                var lines = file.Split(new char[] {'\n'});
                var count = lines.Length;
                return count;
            }
        }
    }
}