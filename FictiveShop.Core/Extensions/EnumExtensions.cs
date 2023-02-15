﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FictiveShop.Core.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue);
    }
}