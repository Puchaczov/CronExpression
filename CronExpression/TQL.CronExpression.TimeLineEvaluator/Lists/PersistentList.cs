﻿using System;
using System.Collections.Generic;

namespace TQL.CronExpression.TimelineEvaluator.Lists
{
    public class PersistentList<T> : List<T>, IComputableElementsList<T>
    {
        public PersistentList(IEnumerable<T> enumerable)
        {
            AddRange(enumerable);
        }

        public void Add(IComputableElementsList<T> list)
        {
            throw new NotImplementedException();
        }

        public T Element(int index) => this[index];
    }
}