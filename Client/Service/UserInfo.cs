using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Service
{
    static internal class UserInfo
    {
        static private String loginToken = "";
        static internal String LoginToken { get { return loginToken; } set { loginToken = value; } }

        static private String userId = "";
        static internal String UserID { get { return userId; } set { userId = value; } }

        static String name = "";
        static internal String Name { get { return name; } set { name = value; } }
    }
}
