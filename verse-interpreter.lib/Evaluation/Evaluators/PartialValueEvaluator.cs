﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;

namespace verse_interpreter.lib.Evaluation.Evaluators
{
    public class PartialValueEvaluator
    {
        public Variable EvaluatePartialValues(Variable newVariable, Variable oldVariable)
        {
            // Check if there is a collection variable.
            // If not then just return the unchanged new variable.
            if (oldVariable.Value.CollectionVariable == null || newVariable.Value.CollectionVariable == null)
            {
                return newVariable;
            }

            // Compare the collection count of the two variables and get the smaller one.
            int valueCount = Math.Min(oldVariable.Value.CollectionVariable.Values.Count(), newVariable.Value.CollectionVariable.Values.Count());

            // There are 2 types of partial values:
            // Variables with no value like: x:int
            // Values with no variables like: 1
            for (int i = 0; i < valueCount; i++)
            {
                var oldPartialValue = oldVariable.Value.CollectionVariable.Values[i];
                var newPartialValue = newVariable.Value.CollectionVariable.Values[i];

                if (!oldPartialValue.HasValue())
                {
                    if (newPartialValue.Name == null)
                    {
                        newPartialValue.Name = oldPartialValue.Name!;
                        continue;
                    }
                }

                if (oldPartialValue.Name == null)
                {
                    if (!newPartialValue.HasValue())
                    {
                        newPartialValue.Value = oldPartialValue.Value;
                    }
                }
            }

            return newVariable;
        }
    }
}