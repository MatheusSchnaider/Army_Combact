﻿using Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;

namespace GameServer
{
    class Program
    {
        private static bool uniqueUser = true;
        private const int MESSAGE_TYPE_GET_MESSAGES = 1;

        private const int MESSAGE_TYPE_SEND_NEW_MESSAGE = 2;
        private const int MESSAGE_TYPE_READ_NEW_MESSAGE = 3;

        private const int MESSAGE_TYPE_GET_USER_REQUEST = 4;
        private const int MESSAGE_TYPE_GET_USER_SUCCESS = 5;
        private const int MESSAGE_TYPE_GET_USER_ERROR = 6;

        private const int MESSAGE_TYPE_INSERT_USER_REQUEST = 7;
        private const int MESSAGE_TYPE_INSERT_USER_SUCCESS = 8;
        private const int MESSAGE_TYPE_INSERT_USER_ERROR = 9;

        private const int MESSAGE_TYPE_RECOVERY_USER_PASSWORD_REQUEST = 10;
        private const int MESSAGE_TYPE_RECOVERY_USER_PASSWORD_SUCCESS = 11;
        private const int MESSAGE_TYPE_RECOVERY_USER_PASSWORD_ERROR = 12;

        private const int MESSAGE_TYPE_UPDATE_PASSWORD_REQUEST = 13;
        private const int MESSAGE_TYPE_UPDATE_PASSWORD_SUCCESS = 14;
        private const int MESSAGE_TYPE_UPDATE_PASSWORD_ERROR = 15;

        private const int MESSAGE_TYPE_MATCH_DATA_REQUEST = 16;
        private const int MESSAGE_TYPE_MATCH_DATA_SUCCESS = 17;
        private const int MESSAGE_TYPE_MATCH_DATA_WAITING = 18;

        private const int MESSAGE_TYPE_MATCH_ENEMY_ATTACK = 19;

        private const int MESSAGE_TYPE_MATCH_END_GAME = 20;

        static int totPlayersInMatch = 0;
        static List<ThreadClient> clients = new List<ThreadClient>();
        static void Main(string[] args)
        {
            Console.WriteLine("Servidor Aguardando conexões...");
            Server server = new Server("127.0.0.1", 5000);

            server.Start(OnClientConnect, OnClientReceiveMessage);
            Console.WriteLine("Servidor OK...");
        }
        static void OnClientConnect(object sender, EventArgs eventArgs)
        {
            ConnectEventArgs connectEventArgs = eventArgs as ConnectEventArgs;

            if (connectEventArgs != null)
            {
                ThreadClient client = connectEventArgs.Client;

                clients.Add(client);

                Console.WriteLine($"Cliente nº {clients.Count()} conectou-se.");
            }
        }
        private enum Type
        {
            Terra = 1,
            Pedra = 2,
            Item = 3,
            Cabo = 4,
            Coronel = 5,
            Sargento = 6,
            Tenente = 7
        }
        private static int[] InitializeArrayWithNoDuplicates(int start, int size)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            return Enumerable.Repeat<int>(0, size).Select((value, index) => new { i = index + start, rand = rand.Next() }).OrderBy(x => x.rand).Select(x => x.i).ToArray();
        }
        private static int[] GenerateMatchData()
        {
            int qtdCabo = 20;
            int qtdCoronel = 20;
            int qtdSargento = 20;
            int qtdTenente = 20;
            int qtdPedra = 110;
            int qtdTerra = 160;
            int qtdPosicoes = 350;

            int index = 0;
            int[] indexes = InitializeArrayWithNoDuplicates(0, qtdPosicoes / 2);

            int[] itens = new int[qtdPosicoes];

            for (int i = 0; i < qtdCabo / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Cabo;
            }

            for (int i = 0; i < qtdCoronel / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Coronel;
            }

            for (int i = 0; i < qtdSargento / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Sargento;
            }

            for (int i = 0; i < qtdTenente / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Tenente;
            }

            for (int i = 0; i < qtdTerra / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Terra;
            }

            for (int i = 0; i < qtdPedra / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Pedra;
            }
            // --------------------------------------------------------------------------------------------------------------- //

            index = 0;
            indexes = InitializeArrayWithNoDuplicates(175, qtdPosicoes / 2);

            for (int i = 0; i < qtdCabo / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Cabo;
            }

            for (int i = 0; i < qtdCoronel / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Coronel;
            }

            for (int i = 0; i < qtdSargento / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Sargento;
            }

            for (int i = 0; i < qtdTenente / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Tenente;
            }

            for (int i = 0; i < qtdTerra / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Terra;
            }

            for (int i = 0; i < qtdPedra / 2; i++)
            {
                itens[indexes[index++]] = (Int32)Type.Pedra;
            }

            return itens;
        }
        static bool InsertUser(string name, string username, string password, string birthDate, string securityText)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
            {
                try
                {
                    connection.Open();

                    SqlCommand sqlCommand = connection.CreateCommand();

                    sqlCommand.CommandText = "INSERT INTO player (name, username, password, birthDate, securityText) values (@name, @username, @password, @birthDate, @securityText)";

                    sqlCommand.Parameters.Add(new SqlParameter("name", name));
                    sqlCommand.Parameters.Add(new SqlParameter("username", username));
                    sqlCommand.Parameters.Add(new SqlParameter("password", password));
                    sqlCommand.Parameters.Add(new SqlParameter("birthDate", birthDate));
                    sqlCommand.Parameters.Add(new SqlParameter("securityText", securityText));

                    sqlCommand.ExecuteNonQuery();

                    connection.Close();
                    uniqueUser = true;
                    result = true;
                    Console.WriteLine($"Usuário {username} inserido com sucesso no banco de dados!");
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627)
                    {
                        uniqueUser = false;
                        result = false;
                        Console.WriteLine($"Usuário {username} não foi inserido no banco de dados. Usuário já existente!");
                    }
                    else
                    {
                        result = false;
                        Console.WriteLine($"Usuário {username} não foi inserido no banco de dados.");
                    }
                }
                catch (Exception)
                {
                    result = false;
                    Console.WriteLine($"Usuário {username} não foi inserido no banco de dados. Usuário já existente!");
                }
                return result;
            }

        }
        static dynamic GetUser(string username, string password)
        {
            dynamic result;

            using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
            {
                try
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

                    sqlCommand.Parameters.Add(new SqlParameter("username", username));
                    sqlCommand.Parameters.Add(new SqlParameter("password", password));

                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if (reader.Read() == true)
                    {
                        result = new
                        {
                            type = MESSAGE_TYPE_GET_USER_SUCCESS,
                            userId = reader.GetInt32(reader.GetOrdinal("idPlayer")),
                            userName = reader.GetString(reader.GetOrdinal("username")),
                            name = reader.GetString(reader.GetOrdinal("name"))
                        };
                        Console.WriteLine($"Usuário {username} realizou login!");
                    }
                    else
                    {
                        result = null;
                        Console.WriteLine($"Usuário {username} falhou no login!");
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }
        static dynamic GetValidateUser(string username, string birthDate, string securityText)
        {
            dynamic result;

            using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
            {
                try
                {
                    connection.Open();

                    SqlCommand sqlCommand = connection.CreateCommand();

                    sqlCommand.CommandText = @"
						SELECT idPlayer
						  FROM player
						WHERE username = @username AND birthDate = @birthDate AND securityText = @securityText
					";

                    sqlCommand.Parameters.Add(new SqlParameter("username", username));
                    sqlCommand.Parameters.Add(new SqlParameter("birthDate", birthDate));
                    sqlCommand.Parameters.Add(new SqlParameter("securityText", securityText));

                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if (reader.Read() == true)
                    {
                        result = new
                        {
                            type = MESSAGE_TYPE_RECOVERY_USER_PASSWORD_SUCCESS,
                            idPlayer = reader.GetInt32(reader.GetOrdinal("idPlayer"))
                        };
                        Console.WriteLine($"As informações foram verificadas para o usuário {username}!");
                    }
                    else
                    {
                        result = null;
                        Console.WriteLine($"As informações não foram verificadas para o usuário {username}!");
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }
        static bool UpdatePassword(Int32 idPlayer, String newPassword)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
                {
                    connection.Open();

                    SqlCommand sqlCommand = connection.CreateCommand();

                    sqlCommand.CommandText = "UPDATE player SET password = @newPassword where idPlayer = @idPlayer";

                    sqlCommand.Parameters.Add(new SqlParameter("idPlayer", idPlayer));
                    sqlCommand.Parameters.Add(new SqlParameter("newPassword", newPassword));

                    sqlCommand.ExecuteNonQuery();

                    connection.Close();

                    result = true;

                    Console.WriteLine($"Senha alterado com sucesso. Jogador de ID {idPlayer}");
                }
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine($"Falha ao alterar senha do Jogador de ID {idPlayer}");
            }

            return result;
        }
        static bool InsertMessage(int idPlayer, string messageText)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
                {
                    connection.Open();

                    SqlCommand sqlCommand = connection.CreateCommand();

                    sqlCommand.CommandText = "INSERT INTO [message] (idPlayer, text, dateTime) values (@idPlayer, @text, CURRENT_TIMESTAMP)";

                    sqlCommand.Parameters.Add(new SqlParameter("idPlayer", idPlayer));
                    sqlCommand.Parameters.Add(new SqlParameter("text", messageText));

                    sqlCommand.ExecuteNonQuery();

                    connection.Close();

                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }
        static List<dynamic> GetMessages()
        {
            List<dynamic> result = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
            {
                try
                {
                    connection.Open();

                    SqlCommand sqlCommand = connection.CreateCommand();

                    sqlCommand.CommandText = @"
						SELECT 
                        plr.idPlayer,
						plr.name AS 'name', 
                        plr.userName,
                        msg.dateTime,
						msg.text
						FROM [message] msg JOIN [player] plr ON plr.idPlayer = msg.idPlayer 
						ORDER BY msg.dateTime
					";

                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read() == true)
                    {
                        result.Add(new
                        {
                            type = MESSAGE_TYPE_READ_NEW_MESSAGE,
                            userName = reader.GetString(reader.GetOrdinal("userName")),
                            namePlayer = reader.GetString(reader.GetOrdinal("name")),
                            dateTime = reader.GetDateTime(reader.GetOrdinal("dateTime")).ToString("dd/MM/yyyy HH:mm:ss"),
                            messageText = reader.GetString(reader.GetOrdinal("text"))
                        });
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    result.Clear();
                }
            }

            return result;
        }
        static bool EndMatch(int idPlayer1, int idPlayer2, string startTime, char sitPlayer1, char sitPlayer2, int expPlayer1, int expPlayer2)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(@"Data Source=JSEL13D06\LOCALHOST;Initial Catalog=game;User ID=game_usr;Password=P@ssw0rd"))
            {
                bool openConnection = false;

                try
                {
                    connection.Open();
                    openConnection = true;
                }
                catch
                {
                    result = false;
                }
                if (openConnection == true)
                {
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction = connection.BeginTransaction("EndMatch");

                    command.Connection = connection;
                    command.Transaction = transaction;

                    command.CommandText = @"
						INSERT match (startTime, endTime) VALUES
						(CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

                        SELECT SCOPE_IDENTITY();";

                    command.Parameters.Clear();

                    //command.Parameters.Add(new SqlParameter("startTime", startTime));

                    Int32? idMatch = null;

                    try
                    {
                        Object objIdMatch = command.ExecuteScalar();
                        idMatch = (objIdMatch == null || Convert.IsDBNull(objIdMatch)) ? (Int32?)null : Convert.ToInt32(objIdMatch);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }

                    if (idMatch != null)
                    {
                        int idMatchNew = (int)idMatch;
                        int idMatchNew2 = (int)idMatch;
                        command.CommandText = @"
						INSERT plyerMatch (idPlayer, idMatch, expPlayer, winner,item) VALUES
						(@idPlayer1, @idMatchNew, @expPlayer1, @sitPlayer1, 1);

                        INSERT plyerMatch (idPlayer, idMatch, expPlayer, winner,item) VALUES
						(@idPlayer2, @idMatchNew2, @expPlayer2, @sitPlayer2, 1);";

                        command.Parameters.Add("idPlayer1", SqlDbType.Int).Value = idPlayer1;
                        command.Parameters.Add(new SqlParameter("@idMatchNew", idMatchNew));
                        command.Parameters.Add(new SqlParameter("@expPlayer1", expPlayer1));
                        command.Parameters.Add(new SqlParameter("@sitPlayer1", sitPlayer1));

                        command.Parameters.Clear();

                        command.Parameters.Add(new SqlParameter("idPlayer2", idPlayer2));
                        command.Parameters.Add(new SqlParameter("idMatchNew2", idMatchNew2));
                        command.Parameters.Add(new SqlParameter("expPlayer2", expPlayer2));
                        command.Parameters.Add(new SqlParameter("sitPlayer2", sitPlayer2));

                        try
                        {
                            command.ExecuteNonQuery();
                            transaction.Commit();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            result = false;
                        }
                    }

                    connection.Close();
                }
            }

            return result;
        }
        static void StartMatch()
        {
            int isFirst = 1;
            int[] data = GenerateMatchData();

            foreach (ThreadClient client in clients)
            {
                client.SendMessage(new
                {
                    type = MESSAGE_TYPE_MATCH_DATA_SUCCESS,
                    data,
                    isFirst
                });

                isFirst = 0;
            }

        }
        static void OnClientReceiveMessage(object sender, EventArgs eventArgs)
        {
            MessageEventArgs messageEventArgs = eventArgs as MessageEventArgs;

            if (messageEventArgs != null)
            {
                Message message = messageEventArgs.Message;

                int type = message.GetInt32("type");
                ThreadClient client = sender as ThreadClient;

                switch (type)
                {
                    case MESSAGE_TYPE_GET_MESSAGES:
                        if (client != null)
                        {
                            List<dynamic> messages = GetMessages();

                            foreach (dynamic messageFromDB in messages)
                            {
                                client.SendMessage(messageFromDB);
                            }
                        }
                        break;
                    case MESSAGE_TYPE_SEND_NEW_MESSAGE:
                        int userIdNewMsg = message.GetInt32("userId");
                        string userName = message.GetString("userName");
                        string namePlayer = message.GetString("namePlayer");
                        string dateTime = message.GetString("dateTime");
                        string messageText = message.GetString("messageText");

                        if (InsertMessage(userIdNewMsg, messageText) == true)
                        {
                            foreach (ThreadClient clientNewMsg in clients)
                            {
                                clientNewMsg.SendMessage(new
                                {
                                    type = MESSAGE_TYPE_READ_NEW_MESSAGE,
                                    userName,
                                    namePlayer,
                                    dateTime,
                                    messageText
                                });
                            }
                        }
                        break;
                    case MESSAGE_TYPE_INSERT_USER_REQUEST:
                        if (client != null)
                        {

                            string name = message.GetString("name");
                            string username = message.GetString("username");
                            string password = message.GetString("password");
                            string birthDate = message.GetString("birthDate");
                            string securityText = message.GetString("securityText");

                            bool userInserted = InsertUser(name, username, password, birthDate, securityText);

                            if (userInserted == true)
                            {
                                client.SendMessage(new
                                {
                                    type = MESSAGE_TYPE_INSERT_USER_SUCCESS,
                                    success = "Usuário cadastrado com sucesso"
                                });
                            }
                            else
                            {
                                if (uniqueUser == false)
                                {
                                    client.SendMessage(new
                                    {
                                        type = MESSAGE_TYPE_INSERT_USER_ERROR,
                                        error = $"Já existe um usuário '{username}' cadastrado no sistema"
                                    });
                                }
                                else
                                {
                                    client.SendMessage(new
                                    {
                                        type = MESSAGE_TYPE_INSERT_USER_ERROR,
                                        error = "Falha ao tentar cadastrar o usuário, favor tentar novamente"
                                    });
                                }
                            }
                        }
                        break;
                    case MESSAGE_TYPE_GET_USER_REQUEST:
                        if (client != null)
                        {
                            string login = message.GetString("login");
                            string password = message.GetString("password");

                            dynamic userData = null;
                            bool hasException = false;

                            try
                            {
                                userData = GetUser(login, password);

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                hasException = true;
                            }

                            if (hasException)
                            {
                                client.SendMessage(new
                                {
                                    type = MESSAGE_TYPE_GET_USER_ERROR,
                                    error = "Falha ao tentar fazer o login, favor tentar novamente"
                                });
                            }
                            else
                            {
                                if (userData != null)
                                {
                                    client.SendMessage(userData);
                                }
                                else
                                {
                                    client.SendMessage(new
                                    {
                                        type = MESSAGE_TYPE_GET_USER_ERROR,
                                        error = "Falha ao tentar fazer o login, usuário ou senha inválido"
                                    });
                                }
                            }
                        }
                        break;
                    case MESSAGE_TYPE_RECOVERY_USER_PASSWORD_REQUEST:
                        if (client != null)
                        {
                            string username = message.GetString("username");
                            string birthDate = message.GetString("birthDate");
                            string securityText = message.GetString("securityText");

                            dynamic recoveryData = null;
                            bool hasException = false;

                            try
                            {
                                recoveryData = GetValidateUser(username, birthDate, securityText);
                            }
                            catch
                            {
                                hasException = true;
                            }


                            if (hasException)
                            {
                                client.SendMessage(new
                                {
                                    type = MESSAGE_TYPE_RECOVERY_USER_PASSWORD_ERROR,
                                    error = "Falha ao recuperar dados, favor tentar novamente"
                                });
                            }
                            else
                            {
                                if (recoveryData != null)
                                {
                                    client.SendMessage(recoveryData);
                                }
                                else
                                {
                                    client.SendMessage(new
                                    {
                                        type = MESSAGE_TYPE_RECOVERY_USER_PASSWORD_ERROR,
                                        error = "Falha ao recuperar dados, favor tentar novamente"
                                    });
                                }
                            }
                        }
                        break;
                    case MESSAGE_TYPE_UPDATE_PASSWORD_REQUEST:
                        if (client != null)
                        {
                            int idPlayer = message.GetInt32("idPlayer");
                            string password = message.GetString("password");

                            bool updateResult = false;
                            bool hasException = false;

                            try
                            {
                                updateResult = UpdatePassword(idPlayer, password);
                            }
                            catch
                            {
                                hasException = true;
                            }


                            if (hasException)
                            {
                                client.SendMessage(new
                                {
                                    type = MESSAGE_TYPE_UPDATE_PASSWORD_ERROR,
                                    error = "Falha ao recuperar dados, favor tentar novamente"
                                });
                            }
                            else
                            {
                                if (updateResult == true)
                                {
                                    client.SendMessage(new
                                    {
                                        type = MESSAGE_TYPE_UPDATE_PASSWORD_SUCCESS
                                    });
                                }
                                else
                                {
                                    client.SendMessage(new
                                    {
                                        type = MESSAGE_TYPE_UPDATE_PASSWORD_ERROR,
                                        error = "Falha ao recuperar dados, favor tentar novamente"
                                    });
                                }
                            }
                        }
                        break;
                    case MESSAGE_TYPE_MATCH_DATA_REQUEST:
                        totPlayersInMatch++;

                        if (totPlayersInMatch == 2)
                        {
                            totPlayersInMatch = 0;
                            StartMatch();
                        }
                        else
                        {
                            client.SendMessage(new
                            {
                                type = MESSAGE_TYPE_MATCH_DATA_WAITING
                            });
                        }
                        break;
                    case MESSAGE_TYPE_MATCH_ENEMY_ATTACK:
                        if (client != null)
                        {
                            foreach (ThreadClient threadClient in clients)
                            {
                                if (threadClient.GetNumber() != client.GetNumber())
                                {
                                    threadClient.SendMessage(new
                                    {
                                        type = MESSAGE_TYPE_MATCH_ENEMY_ATTACK,
                                        line = message.GetInt32("line"),
                                        column = message.GetInt32("column")
                                    });
                                }
                            }

                        }
                        break;
                    case MESSAGE_TYPE_MATCH_END_GAME:
                        int isWinner = 0, expPlayer1 = 0, expPlayer2 = 0;
                        char sitPlayer1 = 'N', sitPlayer2 = 'N';

                        if (client != null)
                        {
                            foreach (ThreadClient threadClient in clients)
                            {
                                if (message.GetInt32("myUnities") != 0)
                                {
                                    isWinner = 1;
                                }
                                threadClient.SendMessage(new
                                {
                                    type = MESSAGE_TYPE_MATCH_END_GAME,
                                    myUnities = message.GetInt32("myUnities"),
                                    enemyUnities = message.GetInt32("enemyUnities"),
                                    isWinner = isWinner
                                });
                            }

                            if (isWinner == 1)
                            {
                                sitPlayer1 = 'S';
                                expPlayer1 = 100;
                                expPlayer2 = 20;
                            }
                            else
                            {
                                sitPlayer2 = 'S';
                                expPlayer1 = 20;
                                expPlayer2 = 100;
                            }
                            EndMatch(1, 2, "0", sitPlayer1, sitPlayer2, expPlayer1, expPlayer2);

                        }
                        break;
                }
            }
        }

    }
}
