using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.ComponentModel;
using Mysqlx.Crud;

namespace Server.Service
{   
    class UserSession
    {
        private String _token;
        internal String Token { get { return _token; } }

        private String _userID;
        internal String UserID { get { return  _userID; }  }

        private DateTime _loginTime;
        internal DateTime LoginTime { get { return _loginTime; } }

        private Socket _userSocket;
        internal Socket UserSocket { get { return _userSocket; } }

        internal UserSession(String token, String userID, DateTime loginTime, Socket userSocket)
        {
            _token = token;
            _userID = userID;
            _loginTime = loginTime;
            _userSocket = userSocket;
        }
    }

    internal class ActiveUserManager
    {
        static private ActiveUserManager _instance = new();
        static internal ActiveUserManager Instance { get { return _instance; } }

        private Dictionary<String, UserSession> _activeUsers = new();
        private Dictionary<String, String> _userToToken = new();
        private Queue<UserSession> _activeSessions = new();
        
        private void UpdateSessions()
        {
            DateTime now = DateTime.Now;
            while (_activeSessions.Count > 0)
            {
                var session = _activeSessions.Peek();
                if ((now - session.LoginTime).TotalHours >= 24)
                {
                    _activeSessions.Dequeue();
                    _userToToken.Remove(session.UserID);
                    _activeUsers.Remove(session.Token);
                }
                else
                {
                    break;
                }
            }
        }

        internal void AddUserSession(String token, String userID, DateTime loginTime, Socket userSocket)
        {
            UpdateSessions();
            var session = new UserSession(token, userID, loginTime, userSocket);
            _activeUsers[token] = session;
            _userToToken[userID] = token;
            _activeSessions.Enqueue(session);
        }

        internal UserSession? GetUserSessionByToken(String token)
        {
            UpdateSessions();
            if (_activeUsers.ContainsKey(token))
            {
                return _activeUsers[token];
            }
            return null;
        }

        internal String GetTokenByUserID(String userID)
        {
            UpdateSessions();
            if (_userToToken.ContainsKey(userID))
            {
                return _userToToken[userID];
            }
            return "";
        }
    }
}
