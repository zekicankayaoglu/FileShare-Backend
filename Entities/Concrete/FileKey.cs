using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class FileKey
    {
        public int Id { get; set; }
        public string FileId { get; set; }
        public string EncryptionKey { get; set; }
        public string IV { get; set; }
        public int UserId {  get; set; }
    }
}
