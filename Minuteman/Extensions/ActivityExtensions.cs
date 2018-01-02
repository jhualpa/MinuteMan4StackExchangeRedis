﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minuteman.Abstract;

namespace Minuteman.Extensions
{
    public static class ActivityExtensions
    {
        public static Task<IEnumerable<string>> EventNames<TInfo>(
            this IActivity<TInfo> instance) where TInfo : Info
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            return instance.GetActivityNames(false);
        }
    }
}