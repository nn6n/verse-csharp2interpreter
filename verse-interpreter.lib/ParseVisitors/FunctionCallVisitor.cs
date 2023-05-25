﻿using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors
{
    public class FunctionCallVisitor : AbstractVerseVisitor<FunctionCallResult>
    {
        private readonly FunctionParser _functionParser;
        private readonly GeneralEvaluator _evaluator;
        private readonly FunctionCallPreprocessor _functionCallPreprocessor;
        private readonly DeclarationVisitor _declarationVisitor;
        private readonly ExpressionVisitor _expressionVisitor;

        public FunctionCallVisitor(ApplicationState applicationState,
                                   FunctionParser functionParser,
                                   GeneralEvaluator evaluator,
                                   FunctionCallPreprocessor functionCallPreprocessor,
                                   DeclarationVisitor declarationVisitor,
                                   ExpressionVisitor expressionVisitor) : base(applicationState)
        {
            _functionParser = functionParser;
            _evaluator = evaluator;
            _evaluator.ArithmeticExpressionResolved += ArithmeticExpressionResolvedCallback;
            _evaluator.StringExpressionResolved += StringExpressionResolvedCallback;
            _functionCallPreprocessor = functionCallPreprocessor;
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
        }

        private void StringExpressionResolvedCallback(object? sender, StringExpressionResolvedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ArithmeticExpressionResolvedCallback(object? sender, ArithmeticExpressionResolvedEventArgs e)
        {

        }

        public override FunctionCallResult VisitFunction_call([NotNull] Verse.Function_callContext context)
        {
            var functionName = context.ID();
            var parameters = _functionParser.GetCallParamters(context.param_call_item());
            var body = ApplicationState.CurrentScope.LookupManager.GetFunction(functionName.GetText());
            var functionCallItem = new FunctionCall(parameters, body);
            _functionCallPreprocessor.BuildExecutableFunction(functionCallItem);
            ApplicationState.CurrentScopeLevel += 1;

            ApplicationState.Scopes.Add(ApplicationState.CurrentScopeLevel, functionCallItem.Function);
            ApplicationState.CurrentScope.AddFunction(functionCallItem.Function);

            var results = functionCallItem.Function.FunctionBody.Select(statements => statements.Accept(this)).ToList();
            ParseValueToTopScopedVariable(functionCallItem);

            ApplicationState.Scopes.Remove(ApplicationState.CurrentScopeLevel);
            ApplicationState.CurrentScopeLevel -= 1;
            return results.Last();
        }

        public override FunctionCallResult VisitExpression(Verse.ExpressionContext context)
        {
            var expression = _expressionVisitor.VisitExpression(context);
            _evaluator.ExecuteExpression(expression);
            return null!;
        }

        public override FunctionCallResult VisitChoice_rule(Verse.Choice_ruleContext context)
        {
            throw new NotImplementedException();
        }

        public override FunctionCallResult VisitDeclaration(Verse.DeclarationContext context)
        {
            var variable = _declarationVisitor.VisitDeclaration(context);
            ApplicationState.CurrentScope.LookupManager.AddVariable(variable);
            return null!;
        }

        public override FunctionCallResult VisitType_member_access(Verse.Type_member_accessContext context)
        {
            throw new NotImplementedException();
        }

        public override FunctionCallResult VisitType_member_definition(Verse.Type_member_definitionContext context)
        {
            throw new NotImplementedException();
        }

        private void ParseValueToTopScopedVariable(FunctionCall calle)
        {
            var topLevelScope = ApplicationState.Scopes[ApplicationState.CurrentScopeLevel - 1];
            foreach (var variable in calle.Function.LookupManager.GetAllVariables())
            {
                if (topLevelScope.LookupManager.IsVariable(variable.Name) &&
                    variable.HasValue())
                {
                    topLevelScope.LookupManager.UpdateVariable(variable);
                }
            }
        }
    }
}