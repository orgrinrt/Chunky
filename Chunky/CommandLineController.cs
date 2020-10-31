using System;
using System.Collections.Generic;
using Chunky.Shared;

namespace Chunky
{
    public class CommandLineController
    {
        private Dictionary<string, string> _commands = new Dictionary<string, string>();
        private Dictionary<string, Action<string>> _actions = new Dictionary<string, Action<string>>();
        private Dictionary<string, string> _descriptions = new Dictionary<string, string>();

        public Dictionary<string, string> Commands => _commands;
        public Dictionary<string, Action<string>> Actions => _actions;
        public Dictionary<string, string> Descriptions => _descriptions;

        private string _appName;
        private string _appDesc;

        public CommandLineController(string appName, string appDesc)
        {
            _appName = appName;
            _appDesc = appDesc;

            AddAction("help", "Shows this help screen.", PrintHelp);
            AddCommand("-h", "help");
            AddCommand("--help", "help");
            AddCommand("?", "help");
        }

        public void AddCommand(string option, string mappedAction)
        {
            _commands.Add(option, mappedAction);
        }

        public void AddAction(string id, string desc, Action<string> action)
        {
            _actions.Add(id, action);
            _descriptions.Add(id, desc);
        }

        public void PrintHelp(string dummy = "")
        {
            Print.Hr();
            Print.Line(ConsoleColor.Green, _appName);
            Print.Line(ConsoleColor.Blue, _appDesc);
            Print.Hr();
            foreach (string id in _actions.Keys)
            {
                foreach (string command in _commands.Keys)
                {
                    if (_commands[command] == id)
                    {
                        Print.Inline(command + " ");
                    }
                }

                Print.Line("\t" + _descriptions[id]);
                Print.Line();
            }
        }
    }
}