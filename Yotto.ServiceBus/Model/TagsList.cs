using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yotto.ServiceBus.Model
{
    public class TagsList
    {
        public static TagsList Empty => new TagsList();

        public TagsList() { }

        public TagsList(params string[] tags)
        {
            AllTags = tags;
        }

        public string[] AllTags { get; set; } 
    }
}
