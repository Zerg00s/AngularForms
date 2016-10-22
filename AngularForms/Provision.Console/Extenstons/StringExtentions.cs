using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console
{
	public static class StringExtentions
	{
		/// <summary>
		/// Returns the <see cref="SecureString"/> for use of passwords.
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static SecureString GetSecureString(this string password)
		{
			SecureString securePassWord = new SecureString();
			foreach (char c in password.ToCharArray()) securePassWord.AppendChar(c);
			
			return securePassWord;
		}
	}
}
