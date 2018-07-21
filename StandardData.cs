using System;
using System.Collections.Generic;
using System.Text;

namespace Domino
{
    internal class StandardData
    {
        public int ID { get; set; }
        public string Hash { get; set; }
        public long CreationDate { get; set; } = DateTime.UtcNow.ToBinary();
    }
}
