using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Persenter_server
{
    public class Commands
    {
        Dictionary<string, CommandParamsConstraint> _constraints;

        /// <summary>
        /// Manage server commands
        /// </summary>
        public Commands()
        {
            _constraints = new Dictionary<string, CommandParamsConstraint>(30);
        }

        /// <summary>
        /// Add a command without parameters
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Commands Add(string name, Action<Command> action)
        {
            Add(name, (Type)null, action);
            return this;
        }

        /// <summary>
        /// Add a command with only one paramter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Commands Add(string name, Type type, Action<Command> action)
        {
            Add(name, new Type[] { type }, action);
            return this;
        }

        /// <summary>
        /// Add command with multiple paramter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Commands Add(string name, Type[] types, Action<Command> action)
        {
            Add(name, new CommandParamsConstraint(types, action));
            return this;
        }

        /// <summary>
        /// Add command with a constrint object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public Commands Add(string name, CommandParamsConstraint constraint)
        {
            if(!_constraints.ContainsKey(name))
                _constraints.Add(name, constraint);
            return this;
        }

        /// <summary>
        /// Parse a command and his parametres
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Command Parse(string input)
        {
            if(input.Length > 0)
            {
                input = input.TrimStart('/');

                string[] parts = input.Split(' ');

                if (parts.Length == 1 && parts[0] != "")
                    return new Command(parts[0], _constraints);
                else if (parts.Length > 1)
                    return new Command(parts[0], _constraints, parts.ToList().Skip(1).ToArray());                   
            }

            throw new CommandException("Empty command");
        }
    }

    public class CommandException : Exception
    {
        public CommandException() : base()
        {
        }

        public CommandException(string message) : base(message)
        {
        }
    }

    public class Command
    {
        private string _name = "";
        private string[] _params;
        private object[] _parsed;

        public Command(string name, Dictionary<string, CommandParamsConstraint> constraints, params string[] parameters)
        {
            _name = name;
            _params = parameters;

            if (constraints.ContainsKey(_name))
            {
                CommandParamsConstraint constraint = constraints[_name];

                if (constraint.Match(parameters))
                {
                    _parsed = constraint.Parsed;
                    constraint.Action(this);
                }
                else
                {
                    throw new CommandException(string.Format("Command '{0}' don't support these parameters", name));
                }
            }
            else
                throw new CommandException(string.Format("Command '{0}' is unknow", name));
        }

        public string Name { get => _name; }
        public string[] Params { get => _params; }
        public int ParamCount { get => _params.Count(); }
        public bool HasParam { get => ParamCount > 0; }
        public object[] Parsed { get => _parsed; }
    }                                                                                 

    public class CommandParamsConstraint
    {
        private Action<Command> _action;
        private Type[] _types;
        private bool[] _mandatory;
        private object[] _parsed;

        public CommandParamsConstraint(Type[] types, Action<Command> action)
        {
            _action = action;
            _types = types;
        }

        public CommandParamsConstraint(Type[] types, bool[] mandatory, Action<Command> action)
        {
            _action = action;
            _types = types;
        }                                                                                            
        public object[] Parsed { get => _parsed;}
        public Action<Command> Action { get => _action; }

        /// <summary>
        /// Parse param to defined type
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Match(params string[] param)
        {
            // If no param
            if (_types == null)
                return param.Length == 0;

            // If nb of types and params unmatch
            if (_types.Length != param.Length)
                return false;

            _parsed = new object[_types.Length];

            // Try to parse each param with asked type
            for(int i = 0; i < param.Length; i++)
            {
                if(_types[i] == Ty.INT)
                {
                    int val;
                    if(!int.TryParse(param[i], out val))
                        return false;
                    _parsed[i] = val;
                }
                else if(_types[i] == Ty.STRING)
                {
                    if (param[i] == "")
                        return false;
                    _parsed[i] = param[i];
                }
                else if(_types[i] == Ty.DOUBLE)
                {
                    double val;
                    if (!double.TryParse(param[i], out val))
                        return false;
                    _parsed[i] = val;
                }
                else if (_types[i] == Ty.DECIMAL)
                {
                    decimal val;
                    if (!decimal.TryParse(param[i], out val))
                        return false;
                    _parsed[i] = val;
                }

            }

            return true;
        }
    }
}
