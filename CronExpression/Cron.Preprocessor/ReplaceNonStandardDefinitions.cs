﻿using Cron.Utils.Filters;

namespace Cron.Filters
{
    public class ReplaceNonStandardDefinitions : FilterBase<string>
    {
        protected override string Process(string input)
        {
            switch(input)
            {
                case "@yearly":
                case "@annually":
                    return "0 0 0 1 1 * *";
                case "@monthly":
                    return "0 0 0 1 * * *";
                case "@weekly":
                    return "0 0 0 * * 0 *";
                case "@daily":
                    return "0 0 0 * * * *";
                case "@hourly":
                    return "0 0 * * * * *";
            }
            return input;
        }
    }
}
