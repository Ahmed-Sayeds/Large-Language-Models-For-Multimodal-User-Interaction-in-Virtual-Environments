using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    [Serializable]
    public class CommandClass
    {
        public String indexes;
        public List<String> objects;
        public List<String> values;

       /* public List<object> getValues() { return values; }
        public List<object> getObjects() { return objects; }*/

    }
    [Serializable]
    public class CommandList
    {
        public List<CommandClass> command;
    }


}
