﻿using Base;
using System.Windows.Forms;
using GameClient.EventArgs;
using GameClient;
using System;
using GameClient.Properties;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace GameClient
{
	public partial class Main : Form
	{
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

		private int userId;
		private string userName;
		private string name;
		private Client client;
		private Match match;
		private Login login;
		private Register register;
		private Recovery recovery;
		private Chat chat;

		private delegate void SetVisibleMenuBarDelegate(bool visible);
		private delegate void SetVisiblePanelDelegate(bool visible);
		private delegate void SetVisibleBtnCloseDelegate(bool visible);

		public Main()
		{
			InitializeComponent();
			menuBar.Visible = false;
			this.BackgroundImage = Resources.Fundo_Red_Blue;
			this.BackColor = Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(35)))), ((int)(((byte)(80)))));
		}
		protected override void OnClosed(System.EventArgs e)
		{
			if (this.client != null)
			{
				this.client.Close();
			}

			base.OnClosed(e);
		}
		public void SetVisibleMenuBar(bool visible)
		{
			if (this.InvokeRequired == true)
			{
				this.Invoke(new SetVisibleMenuBarDelegate(SetVisibleMenuBar), new object[]
				{
					visible
				});
			}
			else
			{
				this.menuBar.Visible = visible;

			}
		}
		private void Main_Load(object sender, System.EventArgs e)
		{
			StartConnection();
		}
		private void StartConnection()
		{
			try
			{
				this.client = new Client("127.0.0.1", 5000);

				this.client.Connect(this.OnReceiveMessage);

				this.login = new Login(this.BtnRegisterOnClick, this.BtnLoginOnClick, this.BtnRecoveryDataOnClick);
				this.register = new Register(this.BtnCancelOnClick, this.BtnSaveOnClick);
				this.chat = new Chat(this.BtnSendOnClick);
				this.recovery = new Recovery(this.BtnCancelOnClick, this.BtnRecoveryOnClick);
				this.match = new Match(this.EnemyAttack, this.EndGame);

				this.login.MdiParent = this;
				this.chat.MdiParent = this;
				this.match.MdiParent = this;
				this.recovery.MdiParent = this;
				this.register.MdiParent = this;

				this.login.SetVisible(true);
			}
			catch
			{
				this.client = null;

				MessageBox.Show("Falha ao tentar conectar com o servidor");
			}
		}
		private void BtnRegisterOnClick(object sender, System.EventArgs eventArgs)
		{
			this.login.Visible = false;
			this.register.Visible = true;
		}
		private void EnemyAttack(object sender, System.EventArgs eventArgs)
		{
			EnemyAttackEventArgs enemyAttackEventArgs = eventArgs as EnemyAttackEventArgs;

			if (enemyAttackEventArgs != null && this.client != null)
			{
				this.client.SendMessage(new
				{
					type = MESSAGE_TYPE_MATCH_ENEMY_ATTACK,
					line = enemyAttackEventArgs.Line,
					column = enemyAttackEventArgs.Column
				});
			}
		}
		private void EndGame(object sender, System.EventArgs eventArgs)
		{
			EndGameEventArgs endGameEventArgs = eventArgs as EndGameEventArgs;

			if (endGameEventArgs != null && this.client != null)
			{
				this.client.SendMessage(new
				{
					type = MESSAGE_TYPE_MATCH_END_GAME,
					myUnities = endGameEventArgs.MyUnities,
					enemyUnities = endGameEventArgs.EnemyUnities
				});
			}
		}
		private void BtnLoginOnClick(object sender, System.EventArgs eventArgs)
		{
			BtnLoginOnClickEventArgs btnLoginOnClickEventArgs = eventArgs as BtnLoginOnClickEventArgs;

			if (btnLoginOnClickEventArgs != null && this.client != null)
			{
				this.client.SendMessage(new
				{
					type = MESSAGE_TYPE_GET_USER_REQUEST,
					login = btnLoginOnClickEventArgs.Login,
					password = btnLoginOnClickEventArgs.Password
				});
			}
		}
		private void BtnRecoveryDataOnClick(object sender, System.EventArgs eventArgs)
		{
			this.login.Visible = false;
			this.register.Visible = false;
			this.recovery.Visible = true;
			recovery.Clear();
		}
		private void BtnRecoveryOnClick(object sender, System.EventArgs eventArgs)
		{
			if (this.client != null)
			{
				BtnRecoveryOnClickEventArgs btnRecoreyOnClickEventArgs = eventArgs as BtnRecoveryOnClickEventArgs;

				if (btnRecoreyOnClickEventArgs != null)
				{
					this.client.SendMessage(new
					{
						type = MESSAGE_TYPE_RECOVERY_USER_PASSWORD_REQUEST,
						username = btnRecoreyOnClickEventArgs.Username,
						birthDate = btnRecoreyOnClickEventArgs.BirthDate,
						securityText = btnRecoreyOnClickEventArgs.SecurityText,
					});
				}
				else
				{
					BtnUpdatePasswordOnClickEventArgs btnUpdatePasswordOnClickEventArgs = eventArgs as BtnUpdatePasswordOnClickEventArgs;
					if (btnUpdatePasswordOnClickEventArgs != null)
					{
						this.client.SendMessage(new
						{
							type = MESSAGE_TYPE_UPDATE_PASSWORD_REQUEST,
							idPlayer = btnUpdatePasswordOnClickEventArgs.IdPlayer,
							password = btnUpdatePasswordOnClickEventArgs.Password
						});
						recovery.Visible = false;
						login.Visible = true;
					}
				}
			}

		}
		private void BtnCancelOnClick(object sender, System.EventArgs eventArgs)
		{
			this.login.Visible = true;
			this.register.Visible = false;
			this.recovery.Visible = false;
		}
		private void BtnSaveOnClick(object sender, System.EventArgs eventArgs)
		{
			BtnSaveOnClickEventArgs btnSaveOnClickEventArgs = eventArgs as BtnSaveOnClickEventArgs;

			if (btnSaveOnClickEventArgs != null && this.client != null)
			{
				this.client.SendMessage(new
				{
					type = MESSAGE_TYPE_INSERT_USER_REQUEST,
					name = btnSaveOnClickEventArgs.Name,
					username = btnSaveOnClickEventArgs.Username,
					password = btnSaveOnClickEventArgs.Password,
					birthDate = btnSaveOnClickEventArgs.BirthDate,
					securityText = btnSaveOnClickEventArgs.SecurityText

				});
			}
			login.Visible = true;
		}
		private void BtnSendOnClick(object sender, System.EventArgs eventArgs)
		{
			BtnSendOnClickEventArgs btnSendOnClickEventArgs = eventArgs as BtnSendOnClickEventArgs;

			if (btnSendOnClickEventArgs != null && this.client != null)
			{
				this.client.SendMessage(new
				{
					type = MESSAGE_TYPE_SEND_NEW_MESSAGE,
					userId = this.userId,
					userName = this.userName,
					namePlayer = this.name,
					dateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
					messageText = btnSendOnClickEventArgs.MessageText
				});
			}
		}
		private void OnReceiveMessage(object sender, System.EventArgs eventArgs)
		{
			MessageEventArgs messageEventArgs = eventArgs as MessageEventArgs;

			if (messageEventArgs != null)
			{
				Base.Message message = messageEventArgs.Message;

				int type = message.GetInt32("type");

				switch (type)
				{
					case MESSAGE_TYPE_READ_NEW_MESSAGE:
						if (this.chat != null)
						{
							string userName = message.GetString("userName");
							string namePlayer = message.GetString("namePlayer");
							string dateTime = message.GetString("dateTime");
							string messageText = message.GetString("messageText");

							this.chat.WriteMessage(namePlayer, dateTime, messageText);
						}
						break;
					case MESSAGE_TYPE_GET_USER_SUCCESS:
						if (this.client != null)
						{
							this.userId = message.GetInt32("userId");
							this.userName = message.GetString("userName");
							this.name = message.GetString("name");

							this.login.Clear();
							this.login.SetVisible(false);
							this.SetVisibleMenuBar(true);

						}
						break;
					case MESSAGE_TYPE_GET_USER_ERROR:
						MessageBox.Show(message.GetString("error"));
						break;
					case MESSAGE_TYPE_INSERT_USER_SUCCESS:
						this.register.Clear();
						MessageBox.Show(message.GetString("success"));
						break;
					case MESSAGE_TYPE_INSERT_USER_ERROR:
						MessageBox.Show(message.GetString("error"));
						break;
					case MESSAGE_TYPE_RECOVERY_USER_PASSWORD_SUCCESS:
						this.recovery.EnabledPassword(message.GetInt32("idPlayer"));
						break;
					case MESSAGE_TYPE_RECOVERY_USER_PASSWORD_ERROR:
						MessageBox.Show(message.GetString("error"));
						break;
					case MESSAGE_TYPE_UPDATE_PASSWORD_SUCCESS:
						this.register.Clear();
						MessageBox.Show("Senha alterada com sucesso!");
						break;
					case MESSAGE_TYPE_UPDATE_PASSWORD_ERROR:
						MessageBox.Show(message.GetString("error"));
						break;
					case MESSAGE_TYPE_MATCH_DATA_SUCCESS:
						if (this.match != null)
						{
							this.match.InitializeMatrix(message.GetSingleDimArrayInt32("data"), message.GetInt32("isFirst"), this.userId);
						}
						break;
					case MESSAGE_TYPE_MATCH_DATA_WAITING:
						if (this.match != null)
						{
							this.match.WaitPlayer();
						}
						break;
					case MESSAGE_TYPE_MATCH_ENEMY_ATTACK:
						if (this.match != null)
						{
							this.match.ShowMyButtonEnemyClick(message.GetInt32("line"),message.GetInt32("column"));
						}
						break;
					case MESSAGE_TYPE_MATCH_END_GAME:
						if (this.match != null)
						{
							this.match.EndGameDel(message.GetInt32("myUnities"), message.GetInt32("enemyUnities"), message.GetInt32("isWinner"));
						}
						break;
				}
			}
		}
		private void chatToolStripMenuItem_Click(object sender, System.EventArgs e)
		{

			this.chat.SetVisible(true, this.name);
			this.client.SendMessage(new
			{
				type = MESSAGE_TYPE_GET_MESSAGES
			});
		}
		private void jogarToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			this.match.SetVisible(true);
			this.client.SendMessage(new
			{
				type = MESSAGE_TYPE_MATCH_DATA_REQUEST
			});
		}
		private void sairToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			this.chat.rtxtHistory.Text = "";
			this.login.Visible = true;
			this.register.Visible = false;
			this.chat.SetVisible(false, this.name);
			this.recovery.Visible = false;
			this.menuBar.Visible = false;
			this.match.Visible = false;
		}

		public void Move(System.IntPtr handle)
		{
			ReleaseCapture();
			SendMessage(handle, 0x112, 0xf012, 0);
		}
		[DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
		private extern static void ReleaseCapture();
		[DllImport("user32.DLL", EntryPoint = "SendMessage")]
		private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

		private void pnlMove_MouseMove(object sender, MouseEventArgs e)
        {
			Move(this.Handle);
		}

        private void btnClose_Click(object sender, System.EventArgs e)
        {
			DialogResult resposta = MessageBox.Show("Deseja finalizar o Jogo?", "Finalizar", MessageBoxButtons.YesNo);

			if (resposta == DialogResult.Yes)
			{
				this.Close();
            }
        }

        private void sairDoJogoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
			DialogResult resposta = MessageBox.Show("Deseja finalizar o Jogo?", "Finalizar", MessageBoxButtons.YesNo);

			if (resposta == DialogResult.Yes)
			{
				this.Close();
			}
		}
		protected override void OnClosing(CancelEventArgs e) => Close();
	}
}
