namespace Cartographer_Launcher.Includes.Dependencies
{
	public enum SettingsDisplayMode : int
	{
		None = -1,
		Windowed = 0,
		Fullscreen = 1,
	}

	public enum H2ResponseCode : int
	{
		LoginSuccessful = 10,
		LoginInvalidPassword = 11,
		LoginAccountDoesntExsist = 12,
		LoginError = 13,
		LoginBanned = 14,
		LoginRememberTokenSuccessful = 20,
		LoginRememberTokenFailed = 21,
		LoginRememberTokenBanned = 22,
		LoginTokenSuccessful = 30,
		LoginTokenInvalid = 31,
		LoginTokenAccountDoesntExsist = 32,
		RegisterSuccessful = 40,
		RegisterUsernameInvalid = 41,
		RegisterUsernameFailed = 42,
	}
}