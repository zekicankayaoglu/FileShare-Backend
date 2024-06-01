using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class SharedFile
    {
        public int Id {  get; set; }
        public int SharingUserId {  get; set; }
        public int SharedUserId { get; set; }
        public string FileId {  get; set; }
    }
}
