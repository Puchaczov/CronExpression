﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cron.Compilation
{
    public class CompilationRequest
    {

        private readonly string input;
        private readonly CompilationOptions options;

        public CompilationRequest(string input, CompilationOptions options)
        {
            this.input = input;
            this.options = options;
        }

        public string Input
        {
            get
            {
                return this.input;
            }
        }

        public CompilationOptions Options
        {
            get
            {
                return this.options;
            }
        }
        public class CompilationOptions
        {
            public bool ProduceEndOfFileNode { get; set; }
            public bool ProduceYearIfMissing { get; set; }
        }
    }
}
