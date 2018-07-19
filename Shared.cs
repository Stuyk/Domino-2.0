using System;
using System.Collections.Generic;
using System.Text;

namespace DominoBlockchain
{
    public class Shared
    {
        public uint Id { get; set; }
        public string Hash { get; set; }
        public long Timestamp { get; set; } = DateTime.UtcNow.ToBinary();
    }
}
