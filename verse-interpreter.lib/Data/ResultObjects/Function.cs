﻿using System.Reflection.Metadata;
using verse_interpreter.lib.Lookup;
using static verse_interpreter.lib.Grammar.Verse;

namespace verse_interpreter.lib.Data.ResultObjects
{
    public class Function : IScope<Variable>
    {
        public Function()
        {
            Parameters = new List<Variable>();
            LookupManager = new LookupManager();
            SubScope = new Dictionary<int, IScope<Variable>>();
        }

        public string FunctionName { get; set; } = null!;

        public string ReturnType { get; set; } = null!;

        public int ParameterCount { get { return Parameters.Count; } }

        public List<BlockContext> FunctionBody { get; set; } = null!;

        public List<Variable> Parameters { get; set; } = null!;

        public Dictionary<int, IScope<Variable>> SubScope { get; }

        public LookupManager LookupManager { get; }

        public void AddFunction(Function res)
        {
            this.LookupManager.AddFunction(res);
        }

        public void AddScopedVariable(Variable variable)
        {
            this.LookupManager.AddVariable(variable);
        }

        public Function GetInstance()
        {
            Function function = new Function();
            function.FunctionName = FunctionName;
            function.Parameters = Parameters;
            function.ReturnType = ReturnType;
            function.FunctionBody = FunctionBody;
            return function;
        }
    }
}