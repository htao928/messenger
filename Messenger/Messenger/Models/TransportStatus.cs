﻿namespace Messenger.Models
{
    public enum TransportStatus : int
    {
        默认 = 0,
        等待 = 1,
        运行 = 2,
        暂停 = 4,
        中断 = 8 | 终态,
        取消 = 16 | 终态,
        成功 = 32 | 终态,

        终态 = 1 << 31,
    }
}
