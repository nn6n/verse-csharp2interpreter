﻿using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Parser
{
    public class PrimaryRuleParser
    {
        public ExpressionResult ParsePrimary([Antlr4.Runtime.Misc.NotNull] Verse.PrimaryContext context)
        {
            ExpressionResult result = new ExpressionResult();
            // Checks if the there are any subexpressions --> due to brackets for instance
            // Fetches the value / identifer from the current node
            var fetchedValue = context.INT();
            var fetchedIdentifier = context.ID();
            var fetchedMemberAccess = context.type_member_access();
            var fetchedString = context.string_rule();

            if (fetchedValue != null)
            {
                result.IntegerValue = Convert.ToInt32(fetchedValue.ToString());
                return result;
            }
            if (fetchedIdentifier != null)
            {
                result.ValueIdentifier = fetchedIdentifier.GetText();
                return result;

            }
            if (fetchedMemberAccess != null)
            {
                result.ValueIdentifier = fetchedMemberAccess.GetText();
                return result;
            }
            if(fetchedString != null)
            {
                result.StringValue = fetchedString.GetText();
                return result;
            }

            throw new NotImplementedException();
        }
    }
}