using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Linq.Expressions;

namespace MySocket
{
	// 我的用户数据库，只需要 用户名，密码，状态 三个数据
	// username;password;online/offline\n;
	public class Item{
		private string _username,_password,_state;

		public string username{ get { return _username; } private set { _username = value; } }
		public string password{	get{ return _password;}private set{ _password = value;}}
		public string state{
			get{ return _state;}
			set{ if(value.Equals ("online")||value.Equals ("offline"))		_state = value;}
		}

		public Item(string uname,string passwd,string st){
			username = uname;
			password = passwd;
			state = st;
		}

		public string formatRecord(){
			return "" + username + ";" + password + ";" + state;
		}

		public static Item parseRecord(string record){
			string[] contents = record.Split (';');
			if (contents.Length != 3)
				return null;
			return new Item (contents[0],contents[1],contents[2]);
		}
	}

	public class AccountDB{
		private List<Item> items;
		private string listPath;
		private string logPath;
		public AccountDB(){
			items = new List<Item> ();
			listPath = "accounts.txt";
			logPath = "log.log";
		}

		/// <summary>
		/// Query the if this json contains an item with same username as q's.
		/// </summary>
		/// <param name="q">Q.</param>
		/// <returns>return string tip</returns>
		public string trylog(Item q){
			readRecord ();
			foreach (Item i in items) {
				if (!i.username.Equals (q.username))
					continue;

				if (i.password.Equals (q.password))
				if (i.state.Equals (q.state))
					return "fail,this account is alreay "+q.state;
				else {
					i.state = q.state;
					writeRecord ();
					return "successful,this account now is "+q.state;
				}
				else
					return "fail,wrong password";
			}
			return "fail,this account doesn't exist";
		}

		public void readRecord(){
			items.Clear ();

			StreamReader listStream = new StreamReader (listPath);
			if (listStream == null) {
				log ("wrong listPath,the logfile doesn't exist");
				return;
			}

			while(true){
				string line = listStream.ReadLine ();
				if(line == null)
					break;
				Item temp = Item.parseRecord (line);
				if (temp != null)
					items.Add (temp);
			}

			log ("read complete");
			listStream.Close ();
		}

		private bool writeRecord(){
			StreamWriter listStream = new StreamWriter (listPath);
			foreach (Item line in items) {
				listStream.WriteLine (line.formatRecord ());
			}
			listStream.Close ();
			log ("write complete");
			return true;
		}

		private void add(Item n){
			items.Add (n);
		}

		public void log(string message){
			StreamWriter logfile = new StreamWriter (logPath,true);
			logfile.WriteLine (message);
			logfile.Close ();
		}
	}
}
