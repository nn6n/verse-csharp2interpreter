﻿using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.Visitors
{
    public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private readonly TypeInferencer _typeInferencer;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly TypeConstructorVisitor _constructorVisitor;
        private readonly CollectionParser _collectionParser;

        public event EventHandler<DeclarationInArrayFoundEventArgs> DeclarationInArrayFound;

        public ValueDefinitionVisitor(ApplicationState applicationState,
                                      TypeInferencer typeInferencer,
                                      ExpressionVisitor expressionVisitor,
                                      TypeConstructorVisitor constructorVisitor,
                                      CollectionParser collectionParser) : base(applicationState)
        {
            _typeInferencer = typeInferencer;
            _expressionVisitor = expressionVisitor;
            _constructorVisitor = constructorVisitor;
            _collectionParser = collectionParser;
        }

        public override DeclarationResult VisitValue_definition([NotNull] Verse.Value_definitionContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult();

            var maybeInt = context.INT();
            var maybeArrayLiteral = context.array_literal();
            var maybeExpression = context.expression();
            var maybeString = context.string_rule();
            var maybeConstructor = context.constructor_body();

            if (maybeConstructor != null)
            {
                var typeInstance = maybeConstructor.Accept(_constructorVisitor);
                declarationResult.TypeName = typeInstance.Name;
                declarationResult.DynamicType = typeInstance;
            }

            if (maybeInt != null)
            {
                declarationResult.Value = maybeInt.GetText();
            }

            if (maybeArrayLiteral != null)
            {
                declarationResult = this.VisitArray_literal(maybeArrayLiteral);
            }

            if (maybeExpression != null)
            {
                var expression = _expressionVisitor.Visit(maybeExpression);
                _expressionVisitor.Clean();
                
                declarationResult.ExpressionResults = expression;
            }

            if (maybeString != null)
            {
                declarationResult.Value = maybeString.SEARCH_TYPE().GetText().Replace("\"", "");
            }

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitArray_literal([NotNull] Verse.Array_literalContext context)
        {
            List<Variable> variables = new List<Variable>();
            var result = _collectionParser.GetParameters(context.array_elements());

            foreach (var valueDef in result.ValueElements)
            {
                var variableResult = VariableConverter.Convert(valueDef.Accept(this), this.ApplicationState);
                variables.Add(variableResult);
            }

            foreach (var declDef in result.DeclarationElements)
            {
                this.FireDeclarationInArrayFoundEvent(this, declDef);
            }
            
            DeclarationResult declarationResult = new DeclarationResult();
            declarationResult.TypeName = "collection";
            declarationResult.CollectionVariable = new CollectionVariable(declarationResult.Name, "collection", variables);

            return declarationResult;
        }

        protected virtual void FireDeclarationInArrayFoundEvent(object sender, Verse.DeclarationContext declarationContext)
        {
            if (this.DeclarationInArrayFound != null) 
            {
                this.DeclarationInArrayFound(sender, new DeclarationInArrayFoundEventArgs(declarationContext));
            }
        }
    }
}