using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlluviumTest.Services
{
    public class SerilogOutputService : IOutputService
    {
        public SerilogOutputService()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }

        public void Log(string message)
        {
            Serilog.Log.Information(message);
        }
    }
}
