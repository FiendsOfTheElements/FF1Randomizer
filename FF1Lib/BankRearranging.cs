namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void Bank1E()
		{
			// thing in bank 1F for switching to bank 1E when needed
			PutInBank(0x1F, 0xCFD7, Blob.FromHex("2067E92099EBA91E4C03FE"));
			PutInBank(0x1F, 0xC0AD, Blob.FromHex("1E2003FE200080"));
			PutInBank(0x1F, 0xEADF, Blob.FromHex("D7CF"));


			// Moving code and adding new stuff, look in 1E.asm
			// Updating addresses, there are a lot
			// Adresses used below
			Blob Bank1E = Blob.FromHex("1E");
			Blob PtyGen_DrawBoxes = Blob.FromHex("6C82");
			Blob PtyGen_DrawText = Blob.FromHex("A082");
			Blob TurnMenuScreenOn_ClearOAM = Blob.FromHex("5B85");
			Blob DoPartyGen_OnCharacter = Blob.FromHex("C180");
			Blob PtyGen_DrawScreen = Blob.FromHex("A480");
			Blob PtyGen_DrawChars = Blob.FromHex("4A83");
			Blob MenuWaitForBtn_SFX = Blob.FromHex("1A85");
			Blob ClearNT = Blob.FromHex("8485");
			Blob DrawNameInputScreen = Blob.FromHex("AC83");
			Blob CharName_Frame = Blob.FromHex("2A82");
			Blob MainLoop_in_DoNameInput = Blob.FromHex("5E81");
			Blob lut_NameInputRowStart = Blob.FromHex("8406");
			Blob lut_NameInput = Blob.FromHex("840D");
			Blob skip_lut_NameInput_Load = Blob.FromHex("E581");
			Blob NameInput_DrawName = Blob.FromHex("7983");
			Blob PlaySFX_MenuSel = Blob.FromHex("EB84");
			Blob PlaySFX_MenuMove = Blob.FromHex("0485");
			Blob Box = Blob.FromHex("7F82");
			Blob DrawOne_Text = Blob.FromHex("AF82");
			Blob Call_DrawComplexString = Blob.FromHex("E482");
			Blob DrawOne_Chars = Blob.FromHex("5B83");
			Blob MenuFrame = Blob.FromHex("2C85");
			Blob PtyGen_Joy = Blob.FromHex("4C82");
			Blob CharName_DrawCursor = Blob.FromHex("3183");


			// NewGamePartyGeneration
			PutInBank(0x1E, 0x8000, Get(0x39C54, 0xC1));
			PutInBank(0x1E, 0x8005, Blob.FromHex("A90285F2EA")); // Tracking code
			PutInBank(0x1E, 0x8021, Blob.FromHex("AA84"));
			PutInBank(0x1E, 0x8032, DoPartyGen_OnCharacter);
			PutInBank(0x1E, 0x803B, DoPartyGen_OnCharacter);
			PutInBank(0x1E, 0x8044, DoPartyGen_OnCharacter);
			PutInBank(0x1E, 0x804D, DoPartyGen_OnCharacter);
			PutInBank(0x1E, 0x8052, PtyGen_DrawScreen);
			PutInBank(0x1E, 0x8058, PtyGen_DrawChars);
			PutInBank(0x1E, 0x8063, MenuWaitForBtn_SFX);
			PutInBank(0x1E, 0x8073, Blob.FromHex("8180A210208180A220208180"));

			// PtyGen_DrawScreen
			PutInBank(0x1E, 0x80B6, ClearNT);
			PutInBank(0x1E, 0x80B9, PtyGen_DrawBoxes);
			PutInBank(0x1E, 0x80BC, PtyGen_DrawText);
			PutInBank(0x1E, 0x80BF, TurnMenuScreenOn_ClearOAM);

			// DoPartyGen_OnCharacter
			PutInBank(0x1E, 0x80C1, Blob.FromHex("A6678A4A4A4A4AA8B91081859020A480200F82A524D054A525F0023860A520290FC561F0EB8561C900F0E5A667BD0003186901C906D002A9FF9D0003A8C8B914812490F0E8A901853720B0824CD180"));

			// lut_AllowedClasses, defaults to all but None and the anything for the rest
			PutInBank(0x1E, 0x8110, Blob.FromHex("FDFFFFFF"));

			// lut_ClassMask, 0=None, 1=FI,2=TH,  BB,  RM,  WM,  BM
			PutInBank(0x1E, 0x8114, Blob.FromHex("02804020100804"));

			// DoNameInput
			PutInBank(0x1E, 0x812C, Get(0x39D50, 0xE3));
			PutInBank(0x1E, 0x8142, ClearNT);
			PutInBank(0x1E, 0x8145, DrawNameInputScreen);
			PutInBank(0x1E, 0x8158, TurnMenuScreenOn_ClearOAM);
			PutInBank(0x1E, 0x815F, CharName_Frame);
			PutInBank(0x1E, 0x8188, MainLoop_in_DoNameInput);
			PutInBank(0x1E, 0x8197, MainLoop_in_DoNameInput);
			PutInBank(0x1E, 0x81A6, MainLoop_in_DoNameInput);
			PutInBank(0x1E, 0x81B5, MainLoop_in_DoNameInput);
			PutInBank(0x1E, 0x81D3, Blob.FromHex("06841865640AAA9006BD0D854CE581BD0D8485"));
			PutInBank(0x1E, 0x81FD, NameInput_DrawName);
			PutInBank(0x1E, 0x820B, MainLoop_in_DoNameInput);

			// PtyGen_Frame
			PutInBank(0x1E, 0x820F, Get(0x39E33, 0x89));
			PutInBank(0x1E, 0x8213, Blob.FromHex("4A83202283"));
			PutInBank(0x1E, 0x8221, Bank1E);
			PutInBank(0x1E, 0x8228, PtyGen_Joy);

			// CharName_Frame
			PutInBank(0x1E, 0x822E, CharName_DrawCursor);
			PutInBank(0x1E, 0x8246, Bank1E);

			// PtyGen_Joy
			PutInBank(0x1E, 0x825C, PlaySFX_MenuSel);
			PutInBank(0x1E, 0x8269, PlaySFX_MenuMove);

			// PtyGen_DrawBoxes
			PutInBank(0x1E, 0x8271, Box);

			// str_classNone
			PutInBank(0x1E, 0x8298, Blob.FromHex("973CA8FFFFFFFF00"));

			// PtyGen_DrawText
			PutInBank(0x1E, 0x82A0, Get(0x39EBC, 0x10));
			PutInBank(0x1E, 0x82A4, DrawOne_Text);

			// PtyGen_DrawOneText
			PutInBank(0x1E, 0x82B0, Blob.FromHex("BD0803853ABD0903853BBD000318C9FFD00DA9988D3E00A9828D3F004CE38269F08D5F00A9028D5E00A95E8D3E00A9008D3F00A91E8D57008D58008A482036DE68AABD0203855CBD0303855DBD0403855EBD0503855FBD0603853ABD0703853BA95C853EA900853FA91E855785584C36DE"));

			// PtyGen_DrawCursor
			PutInBank(0x1E, 0x8322, Get(0x39F26, 0x1C8));

			// PtyGen_DrawChars
			PutInBank(0x1E, 0x834D, DrawOne_Chars);
			PutInBank(0x1E, 0x8352, DrawOne_Chars);
			PutInBank(0x1E, 0x8357, DrawOne_Chars);

			// NameInput_DrawName
			PutInBank(0x1E, 0x8398, Bank1E);

			// DrawNameInputScreen
			PutInBank(0x1E, 0x83EE, Blob.FromHex("0D853EA984"));
			PutInBank(0x1E, 0x83FE, Bank1E);

			// PlaySFX_MenuSel
			PutInBank(0x1E, 0x84EB, Get(0x3AD84, 0x2F));

			// MenuWaitForBtn_SFX
			PutInBank(0x1E, 0x851A, Get(0x3B613, 0x12));
			PutInBank(0x1E, 0x851B, MenuFrame);
			PutInBank(0x1E, 0x852A, PlaySFX_MenuSel);

			// MenuFrame
			PutInBank(0x1E, 0x852C, Get(0x3B65D, 0x2F));
			PutInBank(0x1E, 0x854A, Bank1E);

			// TurnMenuScreenOn_ClearOAM
			PutInBank(0x1E, 0x855B, Get(0x3B780, 0x29));
			PutInBank(0x1E, 0x857E, Bank1E);

			// ClearNT
			PutInBank(0x1E, 0x8584, Get(0x39C02, 0x2C));

			// Overwrite free space with NOPs so its easier to find
			PutInBank(0x0E, 0x9C54, Enumerable.Repeat((byte)0xEA, 0x49A).ToArray());

		}

		public void Bank1B()
		{
			Blob Bank1B = Blob.FromHex("1B");
			Blob EndOfBattleWrapUp0B = Blob.FromHex("009B");



			Blob DrawEOBCombatBox = Blob.FromHex("B88B");
			Blob WaitForAnyInput = Blob.FromHex("148C");
			Blob RespondDelay_UndrawAllCombatBox = Blob.FromHex("E38B");
			Blob SumBattleReward = Blob.FromHex("078B");
			Blob DivideRewardBySurvivors = Blob.FromHex("438B");
			Blob Draw4EobBoxes = Blob.FromHex("428A");
			Blob LvlUp_AwardAndUpdateExp = Blob.FromHex("DA87");
			Blob GiveRewardToParty = Blob.FromHex("A08A");
			Blob CapAAt200 = Blob.FromHex("998A");
			Blob RandAX = Blob.FromHex("9A8B");
			Blob GiveHpBonusToChar = Blob.FromHex("C18A");
			Blob LvlUp_AdjustBBSubStats = Blob.FromHex("4A87");
			Blob RespondDelay = Blob.FromHex("028C");
			Blob AddBattleRewardToVal = Blob.FromHex("EF8A");
			Blob MultiByteCmp = Blob.FromHex("2E8B");
			Blob SubtractOneFromVal = Blob.FromHex("778C");
			Blob MultiplyXA = Blob.FromHex("788B");
			Blob MusicPlay = Blob.FromHex("228C");
			Blob GetJoyInput = Blob.FromHex("A787");

			// stuff in other banks required for changes
			PutInBank(0x0B, 0x9AE6, EndOfBattleWrapUp0B);
			PutInBank(0x0B, 0x9AF5, Blob.FromHex("A97F48A9FF48A91B4C03FEA98048A92E48A91B4C03FE60"));
			PutInBank(0x1F, 0xFC26, Blob.FromHex("A91B2003FEA983A2B8"));


			// GameOver
			PutInBank(0x1B, 0x8000, Blob.FromHex("A91B8DB16B202ED88DA76BA9008DF86AA904A20920BA8B20168C20E58BA9031869008596A90069938597A90C4C09F2"));
			PutInBank(0x1B, 0x8015, DrawEOBCombatBox);
			PutInBank(0x1B, 0x8018, WaitForAnyInput);
			PutInBank(0x1B, 0x801B, RespondDelay_UndrawAllCombatBox);


			// EndOfBattleWrapUp
			PutInBank(0x1B, 0x802F, Blob.FromHex("A91B8DB16B"));
			PutInBank(0x1B, 0x8034, Get(0x2DB14, 0x6B));
			PutInBank(0x1B, 0x803D, DrawEOBCombatBox);
			PutInBank(0x1B, 0x8040, WaitForAnyInput);
			PutInBank(0x1B, 0x8043, RespondDelay_UndrawAllCombatBox);
			PutInBank(0x1B, 0x8048, SumBattleReward);
			PutInBank(0x1B, 0x804B, DivideRewardBySurvivors);
			PutInBank(0x1B, 0x8061, SumBattleReward);
			PutInBank(0x1B, 0x8076, GiveRewardToParty);
			PutInBank(0x1B, 0x8083, Draw4EobBoxes);
			PutInBank(0x1B, 0x8086, WaitForAnyInput);
			PutInBank(0x1B, 0x8089, RespondDelay_UndrawAllCombatBox);
			PutInBank(0x1B, 0x808E, LvlUp_AwardAndUpdateExp);
			PutInBank(0x1B, 0x8093, LvlUp_AwardAndUpdateExp);
			PutInBank(0x1B, 0x8098, LvlUp_AwardAndUpdateExp);
			PutInBank(0x1B, 0x809C, Blob.FromHex("20DA87A99B48A90A48A90B4C03FE"));

			// data_BattleMessages_Raw, and "Nothing happens" string
			PutInBank(0x1B, 0x80AA, Get(0x2CC40, 0x3C0));
			PutInBank(0x1B, 0x83BA, Blob.FromHex("aa80b180ba80c880cf80db80eb80f48005810e811f8131813d814b81578168817481868191819a81a381aa81ba81c481cc81d981ec8104820f821b8228823f824682528269827f82898292829f82a782b082b782c182ca82d982e782eb82f382fc82038308830d83128317831c8321832583268327832d8336834083488353835b83608365836a836f83748379837e8383838a839a83a283a9835684"));
			PutInBank(0x1B, 0x8456, Get(0x2CFEC, 0x14));


			PutInBank(0x1B, 0x846A, Blob.FromHex("A026B186C931F00748A93285106860686868AA688AC94FD00DA510A2008610C932D0064C53884CD587686860"));

			// LvlUp_AdjustBBSubStats
			PutInBank(0x1B, 0x874A + 0x5, Get(0x2D966, 0x33));
			// Overwrite BB check with Unarmed LUT check
			PutInBank(0x1B, 0x874A, Blob.FromHex("A90085EDA9B285EEA000B1862003B0"));

			// data_MaxRewardPlusOne, and data_MaxHPPlusOne
			PutInBank(0x1B, 0x87A2, Get(0x2D9A3, 0x5));

			// GetJoyInput jmp for up+A
			PutInBank(0x1B, 0x87A7, Blob.FromHex("4C28D8"));

			// LvlUp_NoDisplay (new routine for just processing all possible level ups for a char at once without display)
			PutInBank(0x1B, 0x87AA, Blob.FromHex("A51009808D896C0AA8B9898A8586B98A8A8587B9918A8584B9928A858520608820578890062081884CCA87A90E4C03FE"));


			// LvlUp_AwardAndUpdateExp
			PutInBank(0x1B, 0x87DA, Blob.FromHex("200B88206088206E88"));
			PutInBank(0x1B, 0x87E3, Get(0x2DB88, 0x5A));
			PutInBank(0x1B, 0x87F0, Blob.FromHex("0188"));
			PutInBank(0x1B, 0x8813, Blob.FromHex("898A8586B98A8A8587B9918A8584B9928A"));
			PutInBank(0x1B, 0x883D, Blob.FromHex("20608820A08A205788900E2057889006208188EAEAEA20E38960206E88A002202E8B60"));



			// LvlUp_GetChar_Exp and LvlUp_GetExpToAdvance
			PutInBank(0x1B, 0x8860, Get(0x2DBF3, 0x21));
			PutInBank(0x1B, 0x8877, Blob.FromHex("81"));
			PutInBank(0x1B, 0x887D, Blob.FromHex("8C"));


			// LvlUp_LevelUp
			PutInBank(0x1B, 0x8881, Get(0x2DC14, 0x151));
			PutInBank(0x1B, 0x8881, Blob.FromHex("206A84EAEAEAEAEA"));	
			PutInBank(0x1B, 0x889E, Blob.FromHex("718A"));
			PutInBank(0x1B, 0x88A7, Blob.FromHex("728A"));
			PutInBank(0x1B, 0x88C3, Blob.FromHex("598A20998A"));
			PutInBank(0x1B, 0x88D0, Blob.FromHex("658A20998A"));
			PutInBank(0x1B, 0x8939, Blob.FromHex("9A8B4C4089"));
			PutInBank(0x1B, 0x895E, GiveHpBonusToChar);
			PutInBank(0x1B, 0x89D0, LvlUp_AdjustBBSubStats);
			PutInBank(0x1B, 0x89D2, Blob.FromHex("A5802907C907F008A58029F06907858060"));

			// LvlUp_Display, Draw4EobBoxes, luts, etc.
			PutInBank(0x1B, 0x89E3, Get(0x2DD65, 0x39));
			PutInBank(0x1B, 0x8A1C, Blob.FromHex("8557"));
			PutInBank(0x1B, 0x8A1E, Get(0x2DDA1, 0x19D));
			PutInBank(0x1B, 0x89EE, Draw4EobBoxes);
			PutInBank(0x1B, 0x8A01, Blob.FromHex("30"));
			PutInBank(0x1B, 0x8A1B, Bank1B);
			PutInBank(0x1B, 0x8A28, RespondDelay);
			PutInBank(0x1B, 0x8A2B, WaitForAnyInput);
			PutInBank(0x1B, 0x8A3E, Blob.FromHex("BB4CE38B"));
			PutInBank(0x1B, 0x8A49, DrawEOBCombatBox);
			PutInBank(0x1B, 0x8A71, Blob.FromHex("A98D0B8E6D8ECF8E318F938FA98D0B8E6D8ECF8E318F938F")); // lut for lvlup pointers
			PutInBank(0x1B, 0x8AA1, AddBattleRewardToVal);
			PutInBank(0x1B, 0x8AA4, Blob.FromHex("A2"));
			PutInBank(0x1B, 0x8AA8, Blob.FromHex("87"));
			PutInBank(0x1B, 0x8AAE, MultiByteCmp);
			PutInBank(0x1B, 0x8ABE, SubtractOneFromVal);
			PutInBank(0x1B, 0x8AC2, Blob.FromHex("EF8AA9A58582A9878583A001202E8B"));
			PutInBank(0x1B, 0x8ADF, SubtractOneFromVal);
			PutInBank(0x1B, 0x8BB0, MultiplyXA);


			// DrawEOBCombatBox and RespondDelay_UndrawAllCombatBoxes
			PutInBank(0x1B, 0x8BB8, Blob.FromHex("8DB368A91B85578A0AA8BE328CB9338CA8A98B48A9DE48A9D748A9F548A91B85E8ADB3684C18F2EEF86A6020028CA98B48A9FB48A9D748A9F548A91B85E8ADF86A4C0FF2A9008DF86A60"));


			// RespondDelay, WaitForAnyInput, and MusicPlay
			PutInBank(0x1B, 0x8C02, Get(0x2DF66, 0x33));
			
			PutInBank(0x1B, 0x8C0C, MusicPlay);
			PutInBank(0x1B, 0x8C15, GetJoyInput);
			PutInBank(0x1B, 0x8C1C, MusicPlay);
			PutInBank(0x1B, 0x8C23, Bank1B);
			PutInBank(0x1B, 0x8C24, Blob.FromHex("8557A54B1005ADA76B854B4C09C0"));


			// more luts
			PutInBank(0x1B, 0x8C32, Blob.FromHex("468C4B8C4E8C538C588C5E8C618C688C6B8C718C0F3D0F3C000F49000C786899009098958D000C76689000000F3000"));
			PutInBank(0x1B, 0x8C61, Get(0x2D950, 0x16));
			
			// SubtractOneFromVal
			PutInBank(0x1B, 0x8C77, Get(0x2D999, 0xA));

			// level up data, extra space given for furture use
			PutInBank(0x1B, 0x8C81, Get(0x2D000, 0x94));
			PutInBank(0x1B, 0x8DA9, Get(0x2D094, 0x24C));


			// fill empty space with NOPs
			PutInBank(0x0B, 0x9B0C, Enumerable.Repeat((byte)0xEA, 0x3EF).ToArray());
			PutInBank(0x0B, 0x8C40, Enumerable.Repeat((byte)0xEA, 0x6A0).ToArray());
			PutInBank(0x0B, 0x9950, Enumerable.Repeat((byte)0xEA, 0x58).ToArray());
			// Note, keeping a handfull of small routines that are not needed but might be useful in the future,
			// such as DrawEOBCombatBox, RespondDelay_UndrawAllCombatBoxes, WaitForAnyInput and lut_EOBText. These
			// also dont take up a large amount of space.
		}




	}
}
