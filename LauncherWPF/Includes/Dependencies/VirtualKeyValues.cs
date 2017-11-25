using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartographer_Launcher.Includes.Dependencies
{
	public class VirtualKeyValues
	{
		public enum VirtualKeyStates : int
		{
			/*				MOUSE BUTTONS			*/
			VK_LBUTTON = 0x01,      //Left mouse button
			VK_RBUTTON = 0x02,      //Right mouse button
			VK_CANCEL = 0x03,       //Control-break processing
			VK_MBUTTON = 0x04,      //Middle mouse button (three-button mouse)
			VK_XBUTTON1 = 0x05,     //X1 mouse button
			VK_XBUTTON2 = 0x06,     //X2 mouse button
									//0x07					//Undefined

			/*				MISC KEYS I			*/
			VK_BACK = 0x08,         //BACKSPACE key
			VK_TAB = 0x09,          //TAB Key
									//0x0A					//Reserved
									//0x0B					//Reserved
			VK_CLEAR = 0x0C,        //CLEAR key
			VK_RETURN = 0x0D,       //ENTER key
			VK_SHIFT = 0x10,        //SHIFT key
			VK_CONTROL = 0x11,      //CTRL key
			VK_MENU = 0x12,         //ALT key
			VK_PAUSE = 0x13,        //PAUSE key
			VK_CAPITAL = 0x14,      //CAPS LOCK key

			/*			IME MODES (includes ESC)	*/
			VK_KANA = 0x15,         //IME Kana mode
			VK_HANGEUL = 0x15,      //IME Hanguel mode (maintained for compatibility; use VK_HANGUL)
			VK_HANGUL = 0x15,       //IME Hangul mode
									//0x16					//Undefined
			VK_JUNJA = 0x17,        //IME Junja mode
			VK_FINAL = 0x18,        //IME final mode
			VK_HANJA = 0x19,        //IME Hanja mode
			VK_KANJI = 0x19,        //IME Kanji mode
									//0x1A					//Reserved
			VK_ESCAPE = 0x1B,       //ESC key
			VK_CONVERT = 0x1C,      //IME convert
			VK_NONCONVERT = 0x1D,   //IME nonconvert
			VK_ACCEPT = 0x1E,       //IME accept
			VK_MODECHANGE = 0x1F,   //IME mode change request

			/*				MISC KEYS II		*/
			VK_SPACE = 0x20,        //SPACEBAR
			VK_PRIOR = 0x21,        //PAGE UP key
			VK_NEXT = 0x22,         //PAGE DOWN key
			VK_END = 0x23,          //END key
			VK_HOME = 0x24,         //HOME key
			VK_LEFT = 0x25,         //LEFT ARROW key
			VK_UP = 0x26,           //UP ARROW key
			VK_RIGHT = 0x27,        //RIGHT ARROW key
			VK_DOWN = 0x28,         //DOWN ARROW key
			VK_SELECT = 0x29,       //SELECT key
			VK_PRINT = 0x2A,        //PRINT key
			VK_EXECUTE = 0x2B,      //EXECUTE key
			VK_SNAPSHOT = 0x2C,     //PRINT SCREEN key
			VK_INSERT = 0x2D,       //INSERT key
			VK_DELETE = 0x2E,       //DELETE key
			VK_HELP = 0x2F,         //HELP key

			/*			ALPHA-NUMBER KEYS		*/
			VK_0 = 0x30,        //0 Key
			VK_1 = 0x31,        //1 Key
			VK_2 = 0x32,        //2 Key
			VK_3 = 0x33,        //3 Key
			VK_4 = 0x34,        //4 Key
			VK_5 = 0x35,        //5 Key
			VK_6 = 0x36,        //6 Key
			VK_7 = 0x37,        //7 Key
			VK_8 = 0x38,        //8 Key
			VK_9 = 0x39,        //9 Key
								//0x3A				//Undefined
								//0x3B				//Undefined
								//0x3C				//Undefined
								//0x3D				//Undefined
								//0x3E				//Undefined
								//0x3F				//Undefined
								//0x40				//Undefined
			VK_A = 0x41,        //A Key
			VK_B = 0x42,        //B Key
			VK_C = 0x43,        //C Key
			VK_D = 0x44,        //D Key
			VK_E = 0x45,        //E Key
			VK_F = 0x46,        //F Key
			VK_G = 0x47,        //G Key
			VK_H = 0x48,        //H Key
			VK_I = 0x49,        //I Key
			VK_J = 0x4A,        //J Key
			VK_K = 0x4B,        //K Key
			VK_L = 0x4C,        //L Key
			VK_M = 0x4D,        //M Key
			VK_N = 0x4E,        //N Key
			VK_O = 0x4F,        //O Key
			VK_P = 0x50,        //P Key
			VK_Q = 0x51,        //Q Key
			VK_R = 0x52,        //R Key
			VK_S = 0x53,        //S Key
			VK_T = 0x54,        //T Key
			VK_U = 0x55,        //U Key
			VK_V = 0x56,        //V Key
			VK_W = 0x57,        //W Key
			VK_X = 0x58,        //X Key
			VK_Y = 0x59,        //Y Key
			VK_Z = 0x5A,        //Z Key

			/*			WINDOWS KEYS		*/
			VK_LWIN = 0x5B,         //Left Windows key (Natural keyboard)
			VK_RWIN = 0x5C,         //Right Windows key (Natural keyboard)
			VK_APPS = 0x5D,         //Applications key (Natural keyboard)
									//0x5E					//Reserved
			VK_SLEEP = 0x5F,        //Computer Sleep key

			/*			NUMERIC PAD KEYS		*/
			VK_NUMPAD0 = 0x60,      //Numeric keypad 0 key
			VK_NUMPAD1 = 0x61,      //Numeric keypad 1 key
			VK_NUMPAD2 = 0x62,      //Numeric keypad 2 key
			VK_NUMPAD3 = 0x63,      //Numeric keypad 3 key
			VK_NUMPAD4 = 0x64,      //Numeric keypad 4 key
			VK_NUMPAD5 = 0x65,      //Numeric keypad 5 key
			VK_NUMPAD6 = 0x66,      //Numeric keypad 6 key
			VK_NUMPAD7 = 0x67,      //Numeric keypad 7 key
			VK_NUMPAD8 = 0x68,      //Numeric keypad 8 key
			VK_NUMPAD9 = 0x69,      //Numeric keypad 9 key
			VK_MULTIPLY = 0x6A,     //Multiply key
			VK_ADD = 0x6B,          //Add key
			VK_SEPARATOR = 0x6C,    //Separator key
			VK_SUBTRACT = 0x6D,     //Subtract key
			VK_DECIMAL = 0x6E,      //Decimal key
			VK_DIVIDE = 0x6F,       //Divide key

			/*		FUNCTION KEYS		*/
			VK_F1 = 0x70,       //F1 key
			VK_F2 = 0x71,       //F2 key
			VK_F3 = 0x72,       //F3 key
			VK_F4 = 0x73,       //F4 key
			VK_F5 = 0x74,       //F5 key
			VK_F6 = 0x75,       //F6 key
			VK_F7 = 0x76,       //F7 key
			VK_F8 = 0x77,       //F8 key
			VK_F9 = 0x78,       //F9 key
			VK_F10 = 0x79,      //F10 key
			VK_F11 = 0x7A,      //F11 key
			VK_F12 = 0x7B,      //F12 key
			VK_F13 = 0x7C,      //F13 key
			VK_F14 = 0x7D,      //F14 key
			VK_F15 = 0x7E,      //F15 key
			VK_F16 = 0x7F,      //F16 key
			VK_F17 = 0x80,      //F17 key
			VK_F18 = 0x81,      //F18 key
			VK_F19 = 0x82,      //F19 key
			VK_F20 = 0x83,      //F20 key
			VK_F21 = 0x84,      //F21 key
			VK_F22 = 0x85,      //F22 key
			VK_F23 = 0x86,      //F23 key
			VK_F24 = 0x87,      //F24 key
								//0x88				//Unassigned
								//0x88				//Unassigned
								//0x8A				//Unassigned
								//0x8B				//Unassigned
								//0x8C				//Unassigned
								//0x8D				//Unassigned
								//0x8E				//Unassigned
								//0x8F				//Unassigned
			VK_NUMLOCK = 0x90,  //NUM LOCK key
			VK_SCROLL = 0x91,   //SCROLL LOCK key


			/*			OEM SPECIFIC KEYS (0x92 - 0x96)		*/
			VK_OEM_NEC_EQUAL = 0x92,    //Equal key on Numeric Keypad
			VK_OEM_FJ_JISHO = 0x92,     //Dictionary key
			VK_OEM_FJ_MASSHOU = 0x93,   //Unregister word key
			VK_OEM_FJ_TOUROKU = 0x94,   //Register word key
			VK_OEM_FJ_LOYA = 0x95,      //Left OYAYUBI key
			VK_OEM_FJ_ROYA = 0x96,      //Right OYAYUBI key

			/*			MISC KEYS III		*/
			VK_LSHIFT = 0xA0,       //Left SHIFT key
			VK_RSHIFT = 0xA1,       //Right SHIFT key
			VK_LCONTROL = 0xA2,     //Left CONTROL key
			VK_RCONTROL = 0xA3,     //Right CONTROL key
			VK_LMENU = 0xA4,        //Left MENU key
			VK_RMENU = 0xA5,        //Right MENU key

			/*						BROWSER KEYS				*/
			VK_BROWSER_BACK = 0xA6,         //Browser Back key
			VK_BROWSER_FORWARD = 0xA7,      //Browser Forward key
			VK_BROWSER_REFRESH = 0xA8,      //Browser Refresh key
			VK_BROWSER_STOP = 0xA9,         //Browser Stop key
			VK_BROWSER_SEARCH = 0xAA,       //Browser Search key 
			VK_BROWSER_FAVORITES = 0xAB,    //Browser Favorites key
			VK_BROWSER_HOME = 0xAC,         //Browser Start and Home key

			/*						MEDIA KEYS			*/
			VK_VOLUME_MUTE = 0xAD,          //Volume Mute key
			VK_VOLUME_DOWN = 0xAE,          //Volume Down key
			VK_VOLUME_UP = 0xAF,            //Volume Up key
			VK_MEDIA_NEXT_TRACK = 0xB0,     //Next Track key
			VK_MEDIA_PREV_TRACK = 0xB1,     //Previous Track key
			VK_MEDIA_STOP = 0xB2,           //Stop Media key
			VK_MEDIA_PLAY_PAUSE = 0xB3,     //Play/Pause Media key
			VK_LAUNCH_MAIL = 0xB4,          //Start Mail key
			VK_LAUNCH_MEDIA_SELECT = 0xB5,  //Select Media key
			VK_LAUNCH_APP1 = 0xB6,          //Start Application 1 key
			VK_LAUNCH_APP2 = 0xB7,          //Start Application 2 key
											//0xB8							//Reserved
											//0xB9							//Reserved

			/*				OEM MISC KEYS			*/
			VK_OEM_1 = 0xBA,        //Used for miscellaneous characters; it can vary by keyboard.  For the US standard keyboard, the ';:' key 
			VK_OEM_PLUS = 0xBB,     //For any country/region, the '+' key
			VK_OEM_COMMA = 0xBC,    //For any country/region, the ',' key
			VK_OEM_MINUS = 0xBD,    //For any country/region, the '-' key
			VK_OEM_PERIOD = 0xBE,   //For any country/region, the '.' key
			VK_OEM_2 = 0xBF,        //Used for miscellaneous characters; it can vary by keyboard.  For the US standard keyboard, the '/?' key 
			VK_OEM_3 = 0xC0,        //Used for miscellaneous characters; it can vary by keyboard.  For the US standard keyboard, the '`~' key 
									//0xC1					//Reserved
									//0xC2					//Reserved
									//0xC3					//Reserved
									//0xC4					//Reserved
									//0xC5					//Reserved
									//0xC6					//Reserved
									//0xC7					//Reserved
									//0xC8					//Reserved
									//0xC9					//Reserved
									//0xCA					//Reserved
									//0xCB					//Reserved
									//0xCC					//Reserved
									//0xCD					//Reserved
									//0xCE					//Reserved
									//0xCF					//Reserved
									//0xD0					//Reserved
									//0xD1					//Reserved
									//0xD2					//Reserved
									//0xD3					//Reserved
									//0xD4					//Reserved
									//0xD5					//Reserved
									//0xD6					//Reserved
									//0xD7					//Reserved
									//0xD8					//Unassigned
									//0xD9					//Unassigned
									//0xDA					//Unassigned
			VK_OEM_4 = 0xDB,        //Used for miscellaneous characters; it can vary by keyboard.  For the US standard keyboard, the '[{' key
			VK_OEM_5 = 0xDC,        //Used for miscellaneous characters; it can vary by keyboard.  For the US standard keyboard, the '\|' key
			VK_OEM_6 = 0xDD,        //Used for miscellaneous characters; it can vary by keyboard.  For the US standard keyboard, the ']}' key
			VK_OEM_7 = 0xDE,        //Used for miscellaneous characters; it can vary by keyboard.  For the US standard keyboard, the 'single-quote/double-quote' key
			VK_OEM_8 = 0xDF,        //Used for miscellaneous characters; it can vary by keyboard.
									//0xE0					//Reserved
			VK_OEM_AX = 0xE1,       //OEM specific.  For the Japanese AX keyboard, the 'AX' key
			VK_OEM_102 = 0xE2,      //Either the angle bracket key or the backslash key on the RT 102-key keyboard
			VK_ICO_HELP = 0xE3,     //OEM specific.  Help key on ICO
			VK_ICO_00 = 0xE4,       //OEM specific.  00 key on ICO
			VK_PROCESSKEY = 0xE5,   //IME PROCESS key
			VK_ICO_CLEAR = 0xE6,    //OEM specific.  Clear key on ICO
			VK_PACKET = 0xE7,       //Used to pass Unicode characters as if they were keystrokes. 
									//0xE8					//Reserved
			VK_OEM_RESET = 0xE9,    //OEM specific.   
			VK_OEM_JUMP = 0xEA,     //OEM specific.  
			VK_OEM_PA1 = 0xEB,      //OEM specific.  
			VK_OEM_PA2 = 0xEC,      //OEM specific.  
			VK_OEM_PA3 = 0xED,      //OEM specific.  
			VK_OEM_WSCTRL = 0xEE,   //OEM specific.  
			VK_OEM_CUSEL = 0xEF,    //OEM specific.  
			VK_OEM_ATTN = 0xF0,     //OEM specific.  
			VK_OEM_FINISH = 0xF1,   //OEM specific.  
			VK_OEM_COPY = 0xF2,     //OEM specific.  
			VK_OEM_AUTO = 0xF3,     //OEM specific.  
			VK_OEM_ENLW = 0xF4,     //OEM specific.  
			VK_OEM_BACKTAB = 0xF5,  //OEM specific.  
			VK_ATTN = 0xF6,         //Attn key
			VK_CRSEL = 0xF7,        //CrSel key
			VK_EXSEL = 0xF8,        //ExSel key
			VK_EREOF = 0xF9,        //Erase EOF key
			VK_PLAY = 0xFA,         //Play key
			VK_ZOOM = 0xFB,         //Zoom key
			VK_NONAME = 0xFC,       //Reserved
			VK_PA1 = 0xFD,          //PA1 key
			VK_OEM_CLEAR = 0xFE     //Clear key
		}
	}
}
