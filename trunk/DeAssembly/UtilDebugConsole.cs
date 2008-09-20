using System;
using System.Collections.Generic;
using System.Text;

namespace DeMIPS
{
    class UtilDebugConsole
    {
        static private LinkedList<string> messages = new LinkedList<string>();
        static private int errors = 0;
        static private int warnings = 0;

        static public void Flush()
        {
            messages = new LinkedList<string>();
            errors = 0;
            warnings = 0;
        }

        static public void AddMessage(string message)
        {
            messages.AddLast(message);
        }

        static public void AddException(Exception err)
        {
            if (err is ExceptionWarning)
            {
                messages.AddLast("Warning: " + err.Message);
                warnings++;
            }
            else
            {
                messages.AddLast("Error: " + err.Message);
                errors++;
            }
        }

        static public string[] GetAsArray()
        {
            string[] stringArray = new string[messages.Count + 1];
            messages.CopyTo(stringArray, 0);

            stringArray[stringArray.Length - 1] = errors + " error(s), " + warnings + " warning(s)";

            return stringArray;
        }
    }

    class ExceptionWarning : Exception
    {
        public ExceptionWarning(string message) : base(message)
        {
            
        }
    }

   
}
