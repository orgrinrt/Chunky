using System;
using System.Linq;

namespace Chunky.Shared
{
    public class Prompter
    {
        public event EventHandler PromptInvalidValueEntered;
        public event EventHandler PromptSuccess;
        
        private const string _errorMsg = "Error: You didn't enter a valid value. Try again!";
        private const string _successMsg = "Thanks!";
        private string _promptMsg = "";
        
        public static string ErrorMsg => _errorMsg;
        public static string SuccessMsg => _successMsg;
        public string PromptMsg => _promptMsg;
        
        public bool Prompt<T>(string message, out T result, params string[] skipCmds)
        {
            Print.Line(message);
            string line = Console.ReadLine();
            result = default;

            if (skipCmds.Contains(line)) return false;

            #region integers
            if (typeof(T) == typeof(short))
            {
                if (!short.TryParse(line, out short s))
                {
                    if (!int.TryParse(line, out int i))
                    {
                        if (!long.TryParse(line, out long l))
                        {
                            PromptInvalidValueEntered?.Invoke(this, EventArgs.Empty);
                            return Prompt<T>(message, out result, skipCmds);
                        }
                        result = (T)(object)(short)l;
                    }
                    result = (T)(object)(short)i;
                }
                result = (T)(object)s;
            }
            else if (typeof(T) == typeof(int))
            {
                if (!short.TryParse(line, out short s))
                {
                    if (!int.TryParse(line, out int i))
                    {
                        if (!long.TryParse(line, out long l))
                        {
                            PromptInvalidValueEntered?.Invoke(this, EventArgs.Empty);
                            return Prompt<T>(message, out result, skipCmds);
                        }
                        result = (T)(object)(int)l;
                    }
                    result = (T)(object)i;
                }
                result = (T)(object)(int)s;
            }
            else if (typeof(T) == typeof(long))
            {
                if (!short.TryParse(line, out short s))
                {
                    if (!int.TryParse(line, out int i))
                    {
                        if (!long.TryParse(line, out long l))
                        {
                            PromptInvalidValueEntered?.Invoke(this, EventArgs.Empty);
                            return Prompt<T>(message, out result, skipCmds);
                        }
                        result = (T)(object)l;
                    }
                    result = (T)(object)(long)i;
                }
                result = (T)(object)(long)s;
            }
            #endregion
            
            #region floats
            else if (typeof(T) == typeof(float))
            {
                if (!float.TryParse(line, out float f))
                {
                    if (!double.TryParse(line, out double d))
                    {
                        PromptInvalidValueEntered?.Invoke(this, EventArgs.Empty);
                        return Prompt<T>(message, out result, skipCmds);
                    }
                    result = (T)(object)(float)d;
                }
                result = (T)(object)f;
            }
            else if (typeof(T) == typeof(double))
            {
                if (!float.TryParse(line, out float f))
                {
                    if (!double.TryParse(line, out double d))
                    {
                        PromptInvalidValueEntered?.Invoke(this, EventArgs.Empty);
                        return Prompt<T>(message, out result, skipCmds);
                    }
                    result = (T)(object)d;
                }
                result = (T)(object)(double)f;
            }
            #endregion
            
            else if (typeof(T) == typeof(string))
            {
                result = (T)(object)line;
            }

            if (result == null)
            {
                PromptInvalidValueEntered?.Invoke(this, EventArgs.Empty);
                return Prompt<T>(message, out result, skipCmds);
            }
            else
            {
                PromptSuccess?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }
        
        public bool PromptYesOrNo(string message)
        {
            //_promptMsg = message + " (y/n)";
            Console.WriteLine(message + " (y/n)");

            string line = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(line))
            {
                if (line.StartsWith("y"))
                {
                    PromptSuccess?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                else if (line.StartsWith("n"))
                {
                    PromptSuccess?.Invoke(this, EventArgs.Empty);
                    return false;
                }
            }
            
            PromptInvalidValueEntered?.Invoke(this, EventArgs.Empty);
            return PromptYesOrNo(message);
        }
    }
}