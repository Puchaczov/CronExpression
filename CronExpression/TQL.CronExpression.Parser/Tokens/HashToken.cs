﻿using System.Diagnostics;
using TQL.Core.Tokens;
using TQL.CronExpression.Parser.Enums;

namespace TQL.CronExpression.Parser.Tokens
{
    [DebuggerDisplay("{GetType().Name,nq}: #")]
    public class HashToken : Token
    {
        public HashToken(TextSpan span)
            : base("#", Enums.TokenType.Hash, span)
        {
        }

        public override GenericToken<TokenType> Clone() => new HashToken(Span.Clone());
    }
}