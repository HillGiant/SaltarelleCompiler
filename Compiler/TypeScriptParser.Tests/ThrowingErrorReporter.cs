using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScriptParser.Tests
{
    public class ThrowingErrorReporter : IErrorReporter
    {
        public void ReportError(int line, int col, string message)
        {
            throw new Exception(string.Format("Error at {0},{1}: {2}", line, col, message));
        }
    }
}
