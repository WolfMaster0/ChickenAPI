﻿using System;
using ChickenAPI.Enums;

namespace ChickenAPI.Dtos
{
    public class WorldServerDto
    {
        public Guid Id { get; set; }
        public short ChannelId { get; set; }
        public ChannelColor Color { get; set; }
        public string WorldGroup { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}