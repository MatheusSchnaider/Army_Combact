using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	public class UserBL
	{
		private static UserBL _instance;

		private UserBL()
		{

		}

		public static UserBL GetInstance()
		{
			if (_instance == null)
			{
				_instance = new UserBL();
			}

            return _instance;
		}

		public const int MESSAGE_TYPE_INSERT_USER_SUCCESS = 8;
		public const int MESSAGE_TYPE_INSERT_USER_ERROR = 9;
		public const int MESSAGE_TYPE_GET_USER_SUCCESS = 5;
		public const int MESSAGE_TYPE_GET_USER_ERROR = 6;

		public InsertUserResult InsertUser(User user)
		{
			InsertUserResult result = new InsertUserResult();

			try
			{
				UserDAL userDAL = UserDAL.GetInstance();

				userDAL.InsertUser(user);

				result.Type = MESSAGE_TYPE_INSERT_USER_SUCCESS;
				result.Text = "Usuário cadastrado com sucesso";
			}
			catch (SqlException sqlEx)
			{
				result.Type = MESSAGE_TYPE_INSERT_USER_ERROR;

				if (sqlEx.Number == 2627)
				{
					result.Text = $"Já existe um usuário '{user.Username}' cadastrado no sistema";
				}
				else
				{
					result.Text = "Falha ao tentar cadastrar o usuário, favor tentar novamente";
				}
			}
			catch
			{
				result.Type = MESSAGE_TYPE_INSERT_USER_ERROR;

				result.Text = "Falha ao tentar cadastrar o usuário, favor tentar novamente";
			}

			return result;
		}

		public GetUserResult GetUser(User user)
        {
			GetUserResult result = new GetUserResult();
			bool hasException = false;

			try
            {
				UserDAL userDAL = UserDAL.GetInstance();
				
				User user1 = userDAL.GetUser(user);

				result.Type = MESSAGE_TYPE_GET_USER_SUCCESS;
				result.UserId = user1.ID;
				result.Name = user.Name;
				result.UserName = user1.Username;
			}
			catch (Exception ex)
			{

			}

			if (hasException)
			{
				result.Type = MESSAGE_TYPE_GET_USER_ERROR;
				result.Text = "Falha ao tentar fazer o login, favor tentar novamente";

			}
			else
			{
				result.Type = MESSAGE_TYPE_GET_USER_ERROR,
				result.Text = "Falha ao tentar fazer o login, usuário ou senha inválido";
			}
			return result;
		}
	}
}