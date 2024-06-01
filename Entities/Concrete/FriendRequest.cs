using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public int RequesterId {  get; set; }
        public int ReceiverId {  get; set; }
        public int State {  get; set; }

    }
}
