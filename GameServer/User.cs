using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	public class User
	{
		public String Name { set; get; }
		public String Username { set; get; }
		public String Password { set; get; }
		public String BirthDate { set; get; }
		public String SecurityText { set; get; }
		public Int32 ID { set; get; }
	}
}