﻿using Cron.Parser.Enums;
using Cron.Parser.Tokens;

namespace Cron.Visitors.Exceptions
{
    public class UnexpectedWordNodeAtSegment : BaseCronValidationException
    {
        private readonly Segment segment;

        public UnexpectedWordNodeAtSegment(Token token, Segment segment)
            : base(token)
        {
            this.segment = segment;
        }
    }
}
