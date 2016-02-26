﻿using Cron.Parser.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cron.Parser.Visitors;
using Cron.Parser.Enums;
using Cron.Parser.Extensions;

namespace Cron.Parser.Syntax
{
    public class NumericPrecededLNode : NumberNode
    {
        public NumericPrecededLNode(Token token)
            : base(token)
        { }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override IList<int> Evaluate(Segment segment)
        {
            return ListExtension.Empty();
        }
    }
}