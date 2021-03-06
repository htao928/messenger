﻿using System;
using System.ComponentModel;
using System.Text;

namespace Messenger.Modules
{
    /// <summary>
    /// 提供 Unicode 表情文字
    /// </summary>
    internal class Symbols
    {
        private readonly BindingList<string> _list = null;

        private static Symbols s_ins = new Symbols();

        public static BindingList<string> List => s_ins._list;

        private Symbols()
        {
            var lst = new BindingList<string>();
            var idx = 0x1F600;
            for (var i = 0; i < 69; i++)
            {
                var buf = BitConverter.GetBytes(idx + i);
                var str = Encoding.UTF32.GetString(buf);
                lst.Add(str);
            }
            _list = lst;
        }
    }
}
