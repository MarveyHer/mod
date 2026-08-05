using System;
using RSG;

public static class Auth
{
	public static UserLoginWindow userLoginWindow;

	public static bool isLoggedIn = false;

	public static string userId;

	public static string userName;

	public static string displayName;

	public static string emailAddress;

	private static bool initialized = false;

	public static bool authLoaded = false;

	public static Promise authLoadedPromise = new Promise();

	public static void initializeAuth()
	{
		if (!initialized)
		{
			initialized = true;
		}
	}

	public static void AuthStateChanged(object sender, EventArgs eventArgs)
	{
	}

	public static void signOut()
	{
	}

	public static bool isValidUsername(string username)
	{
		return false;
	}

	public static bool isValidEmail(string email)
	{
		return false;
	}
}
