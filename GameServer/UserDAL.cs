using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	public class UserDAL
	{
		private static UserDAL _instance;

		private UserDAL()
		{

		}

		public static UserDAL GetInstance()
		{
			if (_instance == null)
			{
				_instance = new UserDAL();
			}

			return _instance;
		}

		public void InsertUser(User user)
		{
			using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
			{
				connection.Open();

				SqlCommand sqlCommand = connection.CreateCommand();

				sqlCommand.CommandText = "INSERT INTO player (name, username, password, birthDate, securityText) values (@name, @username, @password, @birthDate, @securityText)";

				sqlCommand.Parameters.Add(new SqlParameter("name", user.Name));
				sqlCommand.Parameters.Add(new SqlParameter("username", user.Username));
				sqlCommand.Parameters.Add(new SqlParameter("password", user.Password));
				sqlCommand.Parameters.Add(new SqlParameter("birthDate", user.BirthDate));
				sqlCommand.Parameters.Add(new SqlParameter("securityText", user.SecurityText));

				sqlCommand.ExecuteNonQuery();

				connection.Close();
			}
		}

		public User GetUser(User user)
		{
			User result;

			using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
			{
				connection.Open();

				SqlCommand sqlCommand = connection.CreateCommand();

				sqlCommand.CommandText = @"
						SELECT idPlayer
							  ,[name]
							  ,username
							  ,[password]
							  ,birthDate
							  ,securityText
						  FROM player
						WHERE username = @username AND password = @password
					";

				sqlCommand.Parameters.Add(new SqlParameter("username", user.Username));
				sqlCommand.Parameters.Add(new SqlParameter("password", user.Password));

				SqlDataReader reader = sqlCommand.ExecuteReader();

				if (reader.Read() == true)
				{
					result = new User

					{
						ID = reader.GetInt32(reader.GetOrdinal("idPlayer")),
						Username = reader.GetString(reader.GetOrdinal("username")),
						Name = reader.GetString(reader.GetOrdinal("name"))
					};
				}
				else
				{
					result = null;
				}

				connection.Close();

				return result;


			}
		}
	}
}