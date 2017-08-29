﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using static System.BitConverter;

namespace Mikodev.Network
{
    public static class LinkExtends
    {
        public static int _SetKeepAlive(this Socket socket, bool enable = true, uint before = Links.KeepAliveBefore, uint interval = Links.KeepAliveInterval)
        {
            if (enable == true && (before < 1 || interval < 1))
                throw new ArgumentOutOfRangeException();
            var len = sizeof(uint);
            var val = new byte[len];
            var buf = new byte[len * 3];
            if (enable)
            {
                Buffer.BlockCopy(GetBytes(1U), 0, buf, 0, len);
                Buffer.BlockCopy(GetBytes(before), 0, buf, len, len);
                Buffer.BlockCopy(GetBytes(interval), 0, buf, len * 2, len);
            }
            socket.IOControl(IOControlCode.KeepAliveValues, buf, val);
            return ToInt32(val, 0);
        }

        public static Task<Socket> _AcceptAsync(this Socket socket) => Task.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, null);

        public static async Task<byte[]> _ReceiveExtendAsync(this Socket socket)
        {
            var buf = await socket._ReceiveAsync(sizeof(int));
            var len = ToInt32(buf, 0);
            if (len < 1 || len > Links.BufferLimit)
                throw new LinkException(LinkError.Overflow);
            var res = await socket._ReceiveAsync(len);
            return res;
        }

        public static async Task<byte[]> _ReceiveAsync(this Socket socket, int length)
        {
            if (length < 1 || length > Links.BufferLimit)
                throw new LinkException(LinkError.AssertFailed, "Buffer size out of range!");
            var buf = new byte[length];
            var idx = 0;
            while (idx < length)
            {
                var sub = length - idx;
                var len = await Task.Factory.FromAsync((a, s) => socket.BeginReceive(buf, idx, sub, SocketFlags.None, a, s), socket.EndReceive, null);
                if (len < 1)
                    throw new SocketException((int)SocketError.ConnectionReset);
                idx += len;
            }
            return buf;
        }
        
        public static async Task _SendExtendAsync(this Socket socket, byte[] buffer)
        {
            var len = GetBytes(buffer.Length);
            await socket._SendAsync(len);
            await socket._SendAsync(buffer);
        }

        public static async Task _SendAsync(this Socket socket, byte[] buffer)
        {
            var idx = 0;
            while (idx < buffer.Length)
            {
                var sub = buffer.Length - idx;
                var len = await Task.Factory.FromAsync((a, o) => socket.BeginSend(buffer, idx, sub, SocketFlags.None, a, o), socket.EndSend, null);
                idx += sub;
            }
        }

        public static LinkPacket _Load(this LinkPacket packet, byte[] buf)
        {
            packet._buf = buf;
            packet._ori = new PacketReader(buf);
            packet._src = packet._ori["source"].Pull<int>();
            packet._tar = packet._ori["target"].Pull<int>();
            packet._pth = packet._ori["path"].Pull<string>();
            packet._dat = packet._ori["data", true];
            return packet;
        }
    }
}
