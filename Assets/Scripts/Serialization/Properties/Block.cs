using System.Collections.Generic;
using System.Linq;

namespace Arctic.Serialization.Properties
{
    public class Block
    {
        public List<IProperty> Properties { get; private set; }
        public List<Block> Blocks { get; private set; }

        public Block()
        {
            Properties = new List<IProperty>();
            Blocks = new List<Block>(); 
        }

        public Block(IEnumerable<IProperty> properties, IEnumerable<Block> blocks) 
        {
            Properties = properties .ToList();
            Blocks = blocks.ToList();
        }
    }
}