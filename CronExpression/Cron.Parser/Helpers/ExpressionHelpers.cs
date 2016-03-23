﻿using Cron.Parser.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cron.Parser.Helpers
{
    public static partial class ExpressionHelpers
    {
        public static RootComponentNode Parse(this string expression, bool produceMissingYearComponent = true)
        {
            Lexer lexer = new Lexer(expression);
            CronParser parser = new CronParser(lexer, produceMissingYearComponent);
            return parser.ComposeRootComponents();
        }
    }
}