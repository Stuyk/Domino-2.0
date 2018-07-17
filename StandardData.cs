using System;
using System.Collections.Generic;
using System.Text;

namespace DominoBlockchain
{
    public class StandardData
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public long CreationDate { get; set; } = DateTime.UtcNow.ToBinary();
    }
}
