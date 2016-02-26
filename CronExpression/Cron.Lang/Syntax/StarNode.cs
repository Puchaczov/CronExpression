﻿using Cron.Parser.Enums;
using Cron.Parser.Extensions;
using Cron.Parser.Tokens;
using Cron.Parser.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cron.Parser.Syntax
{
    public class StarNode : SyntaxOperatorNode
    {
        private Segment segment;

        public StarNode(Segment segment)
        {
            this.segment = segment;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override IList<int> Evaluate(Segment segment)
        {
            switch(segment)
            {
                case Segment.Seconds:
                    return ListExtension.Expand(0, 59, 1);
                case Segment.Minutes:
                    return ListExtension.Expand(0, 59, 1);
                case Segment.Hours:
                    return ListExtension.Expand(0, 23, 1);
                case Segment.DayOfWeek:
                    return ListExtension.Empty();
                case Segment.DayOfMonth:
                    return ListExtension.Expand(1, 32, 1);
                case Segment.Month:
                    return ListExtension.Expand(1, 12, 1);
                case Segment.Year:
                    return ListExtension.Expand(1970, 3000, 1);
                default:
                    throw new UnknownSegmentException(0);
            }
        }

        public Segment Segment
        {
            get
            {
                return segment;
            }
        }

        public override SyntaxNode[] Items
        {
            get
            {
                return new SyntaxNode[] {
                };
            }
        }

        public override Token Token
        {
            get
            {
                return new StarToken();
            }
        }
    }
}