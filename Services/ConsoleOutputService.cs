using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Services
{
    public class ConsoleOutputService : IOutputService
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class MockOutputService : IOutputService
    {
        private readonly List<string> _logMessages = new List<string>();

        public void Log(string message)
        {
            _logMessages.Add(message); // Capture each log message
        }

        public List<string> GetLogMessages()
        {
            return new List<string>(_logMessages);
        }

        public void Clear()
        {
            _logMessages.Clear(); // Optional: clear log messages if needed between tests
        }
    }
}
