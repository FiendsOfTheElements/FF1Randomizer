using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace FF1Lib.Assembly
{
	/// <summary>
	/// Labels, variables, and constants in FF1, and various related functions.
	///
	/// At compile-time, this should all refer to vanilla values.
	///
	/// They should be updated at run-time when things change.
	/// </summary>
	public static class Symbols
	{
		public static class Calculate
		{
			/// <summary>
			/// The size of space available between the provided label and the next (global) label,
			/// including the provided label but not the next. (If last label, it's until the
			/// end of the bank.)
			/// 
			/// This might be the size of the code, it might not be. Use with discretion.
			/// </summary>
			public static int SpaceToNextLabel(string label)
			{
				BA thisLabelBA = AsDictionaries.Labels[label];
				List<KeyValuePair<string, BA>> listInBank = AsLists.LabelsInAddressOrderForBank(thisLabelBA.Bank);

				int thisLabelIndex = listInBank.FindIndex(entry => entry.Key == label);
				if (thisLabelIndex == listInBank.Count - 1)
				{
					return BA.TopOfBank(thisLabelBA.Bank) - thisLabelBA.Addr;
				}
				else
				{
					return listInBank[thisLabelIndex + 1].Value.Addr - thisLabelBA.Addr;
				}
			}
		}

		public static class AsLists
		{
			public static List<KeyValuePair<string, BA>> LabelsInAddressOrderForBank(int bank)
			{
				IList<FieldInfo> fieldsList = typeof(Labels).GetFields();
				Dictionary<string, BA> labels = fieldsList
					.Where(field => ((BA)field.GetValue(null)).Bank == bank)
					.ToDictionary(field => field.Name, field => (BA)field.GetValue(null));
				return labels.OrderBy(entry => entry.Value.Addr).ToList();
			}
		}

		/// <summary>
		/// Provide dictionary versions of the contents of Symbols.Labels, Symbols.Constants, and Symbols.Variables.
		/// </summary>
		public static class AsDictionaries
		{
			private static Dictionary<string, TValue> StaticFieldsDictionaryForType<TValue>(Type containingType)
			{
				IList<FieldInfo> list = containingType.GetFields();
				return list.ToDictionary(field => field.Name, field => (TValue)field.GetValue(null));
			}

			private static void SetStaticFieldsInType<TValue>(Type containingType, Dictionary<string, TValue> dictionary)
			{
				dictionary.ToList().ForEach(entry => containingType.GetField(entry.Key).SetValue(null, entry.Value));
			}

			public static Dictionary<string, BA> Labels
			{
				get => StaticFieldsDictionaryForType<BA>(typeof(Labels));
				set => SetStaticFieldsInType(typeof(Labels), value);
			}

			public static Dictionary<string, int> Constants
			{
				get => StaticFieldsDictionaryForType<int>(typeof(Constants));
				set => SetStaticFieldsInType(typeof(Constants), value);
			}

			public static Dictionary<string, int> Variables
			{
				get => StaticFieldsDictionaryForType<int>(typeof(Variables));
				set => SetStaticFieldsInType(typeof(Constants), value);
			}

			public static Dictionary<string, int> VariablesAndConstants
			{
				get
				{
					Dictionary<string, int> both = new Dictionary<string, int>();

					// this is apparently a good and fast way to combine dictionaries, even if it looks a little stupid.
					Constants.ToList().ForEach(x => both.Add(x.Key, x.Value));
					Variables.ToList().ForEach(x => both.Add(x.Key, x.Value));

					return both;
				}
			}

			/// <summary>
			/// Labels mapped to their jump locations, dropping bank information.
			/// </summary>
			public static Dictionary<string, int> LabelsWithRunAddresses => Labels.ToDictionary(item => item.Key, item => item.Value.Addr);

			/// <summary>
			/// All symbols. NOTE: Label values are transformed into ints using BA.addr.
			/// </summary>
			public static Dictionary<string, int> All
			{
				get
				{
					Dictionary<string, int> everything = new Dictionary<string, int>();

					// this is apparently a good and fast way to combine dictionaries, even if it looks a little stupid.
					Constants.ToList().ForEach(x => everything.Add(x.Key, x.Value));
					Variables.ToList().ForEach(x => everything.Add(x.Key, x.Value));
					Labels.ToList().ForEach(x => everything.Add(x.Key, x.Value.Addr));

					return everything;
				}
			}

		}

		public static class Labels
		{

			// Don't add any fields to this class unless they are symbols. It will break AsDictionaries/AsLists.
			public static BA DoNextRow = new(0x01, 0xBF8F);
			public static BA MinimapDecompress = new(0x01, 0xBF40);
			public static BA lut_MinimapBGPal = new(0x09, 0xBF20);
			public static BA lut_MinimapSprCHR = new(0x09, 0xBF00);
			public static BA DrawDungeonSprite = new(0x09, 0xBE30);
			public static BA lut_MinimapTileset = new(0x09, 0xBF80);
			public static BA lut_MinimapDecorCHRDest_lo = new(0x09, 0xBCE9);
			public static BA lut_MinimapDecorCHRDest_hi = new(0x09, 0xBD19);
			public static BA MinimapFrame = new(0x09, 0xBD49);
			public static BA Minimap_DrawDecorCHR = new(0x09, 0xBC55);
			public static BA Minimap_DrawRows = new(0x09, 0xBD91);
			public static BA Minimap_PrepRow = new(0x09, 0xBDE7);
			public static BA Minimap_FillNTPal = new(0x09, 0xBE54);
			public static BA Minimap_YouAreHere = new(0x09, 0xBF34);
			public static BA Minimap_PrepDecorCHR = new(0x09, 0xBCA3);
			public static BA Minimap_PrepTitleCHR = new(0x09, 0xBB4E);
			public static BA Minimap_DrawSFX = new(0x09, 0xBDCD);
			public static BA lut_MinimapBitplane = new(0x09, 0xBCD9);
			public static BA lut_MinimapTitleCHRDest_lo = new(0x09, 0xBB8A);
			public static BA lut_MinimapTitleCHRDest_hi = new(0x09, 0xBBB2);
			public static BA Minimap_DrawTitleCHR = new(0x09, 0xBB00);
			public static BA chr_MinimapTitle = new(0x09, 0xB700);
			public static BA chr_MinimapDecor = new(0x09, 0xB400);
			public static BA lut_MinimapNT = new(0x09, 0xB000);
			public static BA EnterMinimap = new(0x09, 0xBC00);
			public static BA GetEnemyStatPtr_0Bduplicate = new(0x0B, 0xA7E7);
			public static BA IncYBy8_0B = new(0x0B, 0xA7B0);
			public static BA Draw6TilesFromX = new(0x0B, 0xA785);
			public static BA BtlTmp_NextRow = new(0x0B, 0xA750);
			public static BA Draw4TilesFromX = new(0x0B, 0xA78D);
			public static BA SetPpuAddr_BtlTmp = new(0x0B, 0xA745);
			public static BA WriteAttributesToPPU = new(0x0B, 0xA702);
			public static BA DrawFormation_Mix = new(0x0B, 0xA590);
			public static BA DrawLargeEnemy = new(0x0B, 0xA75E);
			public static BA DrawFormation_4Large = new(0x0B, 0xA4E4);
			public static BA DrawSmallEnemy = new(0x0B, 0xA71E);
			public static BA IncYBy7_0B = new(0x0B, 0xA7B1);
			public static BA DrawFormation_9Small = new(0x0B, 0xA44C);
			public static BA lut_DrawFormation_NonFiend = new(0x0B, 0xA7AA);
			public static BA WriteAttributes_ClearUnusedEnStats = new(0x0B, 0xA7B9);
			public static BA FinalizeEnemyFormation_FiendChaos = new(0x0B, 0xA31E);
			public static BA ApplyPalette_FiendChaos = new(0x0B, 0xA2C4);
			public static BA BtlTmp6_NextRow = new(0x0B, 0xA2B8);
			public static BA ReadAttributesFromPPU = new(0x0B, 0xA6EB);
			public static BA PrepareEnemyFormation_Chaos = new(0x0B, 0xA2E5);
			public static BA PrepareEnemyFormation_Fiend = new(0x0B, 0xA275);
			public static BA lut_FiendTSAPtrs = new(0x0B, 0xA266);
			public static BA IncYBy4_0B = new(0x0B, 0xA7B4);
			public static BA DrawFormation_NonFiend = new(0x0B, 0xA43A);
			public static BA lut_EnemyCountByBattleType = new(0x0B, 0xA79E);
			public static BA PrepareEnemyFormation_FiendChaos = new(0x0B, 0xA26E);
			public static BA PrepareEnemyFormation_SmallLarge = new(0x0B, 0xA17E);
			public static BA PrepareEnemyFormation_Mix = new(0x0B, 0xA32F);
			public static BA ChaosDeath_FadeNoise = new(0x0B, 0xA03D);
			public static BA Battle_FlipCharSprite = new(0x0B, 0x9FC4);
			public static BA BattleUpdateAudio_0BversionNoSFX = new(0x0B, 0x9F86);
			public static BA lut_EOBText = new(0x0B, 0xA00E);
			public static BA MultiplyXA_0Bduplicate = new(0x0B, 0x9EFB);
			public static BA AddBattleRewardToVal = new(0x0B, 0x9E72);
			public static BA RespondDelay_0BversionNoSFX = new(0x0B, 0x9F66);
			public static BA GiveHpBonusToChar = new(0x0B, 0x9E44);
			public static BA RandAX_0Bduplicate = new(0x0B, 0x9F1D);
			public static BA lut_LvlUpMagDefBonus = new(0x0B, 0x9DE8);
			public static BA CapAAt200 = new(0x0B, 0x9E1C);
			public static BA lut_LvlUpHitRateBonus = new(0x0B, 0x9DDC);
			public static BA lut_LevelUpDataPtrs = new(0x0B, 0x9DF4);
			public static BA LvlUp_AwardExp_RTS = new(0x0B, 0x9BF2);
			public static BA LvlUp_LevelUp = new(0x0B, 0x9C14);
			public static BA MultiByteCmp = new(0x0B, 0x9EB1);
			public static BA lut_CharMagicPtrTable = new(0x0B, 0x9E14);
			public static BA lut_CharStatsPtrTable_0Bduplicate = new(0x0B, 0x9E0C);
			public static BA LvlUp_GetExpToAdvance = new(0x0B, 0x9C01);
			public static BA LvlUp_GetCharExp = new(0x0B, 0x9BF3);
			public static BA LvlUp_AwardExp = new(0x0B, 0x9BB0);
			public static BA LvlUp_AwardAndUpdateExp = new(0x0B, 0x9B7F);
			public static BA Draw4EobBoxes = new(0x0B, 0x9DC5);
			public static BA GiveRewardToParty = new(0x0B, 0x9E23);
			public static BA DivideRewardBySurvivors = new(0x0B, 0x9EC6);
			public static BA SumBattleReward = new(0x0B, 0x9E8A);
			public static BA RespondDelay_UndrawAllCombatBoxes = new(0x0B, 0x9F57);
			public static BA WaitForAnyInput = new(0x0B, 0x9F78);
			public static BA DrawEOBCombatBox = new(0x0B, 0x9F3B);
			public static BA Battle_FlipAllChars = new(0x0B, 0x9F99);
			public static BA EndOfBattleWrapUp = new(0x0B, 0x9B14);
			public static BA BankSwitchExitBattle = new(0x0B, 0x9AEE);
			public static BA ChaosDeath = new(0x0B, 0xA052);
			public static BA GameOver = new(0x0B, 0x9AF5);
			public static BA BattleOver_Run = new(0x0B, 0x9AEB);
			public static BA lut_AilmentIbToOb = new(0x0B, 0x9AB8);
			public static BA lut_AilmentObToIb = new(0x0B, 0x9AB4);
			public static BA ConvertIBAilmentToOB = new(0x0B, 0x9AAD);
			public static BA ConvertIBStatsToOB = new(0x0B, 0x9A71);
			public static BA ConvertOBAilmentToIB = new(0x0B, 0x9AA6);
			public static BA Shift6TAX = new(0x0B, 0x9A69);
			public static BA GetJoyInput = new(0x0B, 0x9A06);
			public static BA BattleOver_ProcessResult = new(0x0B, 0x9AC0);
			public static BA WriteAilment_PrepMagicPtr = new(0x0B, 0x99E8);
			public static BA PrepareEnemyFormation = new(0x0B, 0xA12A);
			public static BA ConvertOBStatsToIB = new(0x0B, 0x9A2A);
			public static BA lut_RespondDelay = new(0x0B, 0x9A22);
			public static BA PrepBattleVarsAndEnterBattle = new(0x0B, 0x99B8);
			public static BA DoCrossPageJump = new(0x0B, 0x99A8);
			public static BA data_MaxHPPlusOne = new(0x0B, 0x99A6);
			public static BA data_MaxRewardPlusOne = new(0x0B, 0x99A3);
			public static BA SubtractOneFromVal = new(0x0B, 0x9999);
			public static BA LvlUp_AdjustBBSubStats = new(0x0B, 0x9966);
			public static BA eobtext_PartyPerished = new(0x0B, 0x9960);
			public static BA eobtext_Npts = new(0x0B, 0x995A);
			public static BA eobtext_HPMax = new(0x0B, 0x9957);
			public static BA eobtext_NameLN = new(0x0B, 0x9950);
			public static BA data_ChaosTSA = new(0x0B, 0x9420);
			public static BA data_FiendTSA = new(0x0B, 0x92E0);
			public static BA data_LevelUpData_Raw = new(0x0B, 0x9094);
			public static BA lut_ExpToAdvance = new(0x0B, 0x9000);
			public static BA data_BattleMessages_Raw = new(0x0B, 0x8C40);
			public static BA data_BridgeNT = new(0x0B, 0xBC00);
			public static BA data_BridgeCHR = new(0x0B, 0xB400);
			public static BA data_EpilogueNT = new(0x0B, 0xB000);
			public static BA data_EpilogueCHR = new(0x0B, 0xA800);
			public static BA BattleOver_ProcessResult_L = new(0x0B, 0x9A03);
			public static BA data_BattleMessages = new(0x0B, 0x8F50);
			public static BA lut_BattleFormations = new(0x0B, 0x8400);
			public static BA lut_BattleRates = new(0x0B, 0x8C00);
			public static BA PrepBattleVarsAndEnterBattle_L = new(0x0B, 0x9A00);
			public static BA data_EnemyNames = new(0x0B, 0x94E0);
			public static BA UpdateBattleSFX_Noise = new(0x0C, 0xBF5E);
			public static BA UpdateBattleSFX_Square = new(0x0C, 0xBF26);
			public static BA data_BattleSoundEffects = new(0x0C, 0xBFA4);
			public static BA SwapBattleSFXBytes = new(0x0C, 0xBF8F);
			public static BA IncYBy4_WriteToOam = new(0x0C, 0xBE8C);
			public static BA Explosion_Write_0088 = new(0x0C, 0xBE7E);
			public static BA Explosion_Write_0808 = new(0x0C, 0xBE68);
			public static BA DrawExplosion_Frame = new(0x0C, 0xBE1B);
			public static BA lut_EraseEnemyPPUAddress_Mix_Small = new(0x0C, 0xBEAC);
			public static BA lut_EraseEnemyPPUAddress_4Large = new(0x0C, 0xBEA4);
			public static BA WriteAToPPU2Times = new(0x0C, 0xBD02);
			public static BA MoveDown1Row_UpdateAudio = new(0x0C, 0xBD09);
			public static BA WriteAToPPU4Times = new(0x0C, 0xBCFF);
			public static BA EraseSmallEnemy = new(0x0C, 0xBCE2);
			public static BA lut_EraseEnemyPPUAddress_9Small = new(0x0C, 0xBE92);
			public static BA __DrawEnemyEffect_9Small_Exit = new(0x0C, 0xBCF8);
			public static BA lut_ExplosionCoords_9Small = new(0x0C, 0xBD1C);
			public static BA DrawExplosions_PreserveX = new(0x0C, 0xBCA5);
			public static BA VBlank_SetPPUAddr = new(0x0C, 0xBC85);
			public static BA SwapBtlTmpBytes_Local = new(0x0C, 0xBDD8);
			public static BA WriteAToPPU6Times = new(0x0C, 0xBCFC);
			public static BA DrawExplosions = new(0x0C, 0xBDDB);
			public static BA DrawEnemyEffect_Mix = new(0x0C, 0xBD82);
			public static BA DrawEnemyEffect_4Large = new(0x0C, 0xBD2E);
			public static BA DrawEnemyEffect_9Small = new(0x0C, 0xBCB0);
			public static BA DrawEnemyEffect = new(0x0C, 0xBBFF);
			public static BA ShouldAddEnemyToRoster = new(0x0C, 0xBBCC);
			public static BA IncYBy4 = new(0x0C, 0xBB9F);
			public static BA IncYBy8 = new(0x0C, 0xBB9B);
			public static BA BtlMag_Effect_RemRst_RTS = new(0x0C, 0xBAD6);
			public static BA BtlMag_SetHPToMax = new(0x0C, 0xB9C6);
			public static BA BtlMag_Effect_CureAil_RTS = new(0x0C, 0xB9E3);
			public static BA BtlMag_Effect_Slow_RTS = new(0x0C, 0xB978);
			public static BA BtlMag_ApplyAilments = new(0x0C, 0xB94A);
			public static BA BtlMag_DoStatPrep = new(0x0C, 0xB929);
			public static BA BtlMag_ApplyDamage = new(0x0C, 0xB8DB);
			public static BA PutEffectivityInDamageMathBuf = new(0x0C, 0xB8F9);
			public static BA BtlMag_ZeroHitChance = new(0x0C, 0xB88B);
			public static BA BtlMag_MarkSpellConnected = new(0x0C, 0xB885);
			public static BA BtlMag_DidSpellConnect = new(0x0C, 0xB87E);
			public static BA BtlMag_LoadBaseHitChance_RTS = new(0x0C, 0xB87D);
			public static BA BtlMag_LoadBaseHitChance = new(0x0C, 0xB873);
			public static BA BtlMag_CalcElemHitChance = new(0x0C, 0xB851);
			public static BA WriteAToMagRandHit = new(0x0C, 0xBA8B);
			public static BA Random_0_200 = new(0x0C, 0xBA83);
			public static BA BtlMag_PrepHitAndDamage = new(0x0C, 0xB82D);
			public static BA BtlMag_Effect_InflictAilment2 = new(0x0C, 0xBAD7);
			public static BA BtlMag_Effect_RemoveResist = new(0x0C, 0xBA94);
			public static BA BtlMag_Effect_EvadeUp = new(0x0C, 0xBA73);
			public static BA BtlMag_Effect_CureAll = new(0x0C, 0xBA68);
			public static BA BtlMag_Effect_EvadeDown = new(0x0C, 0xBA46);
			public static BA BtlMag_Effect_AttackUp2 = new(0x0C, 0xBA28);
			public static BA BtlMag_Effect_Fast = new(0x0C, 0xBA10);
			public static BA BtlMag_Effect_AttackUp = new(0x0C, 0xBA00);
			public static BA BtlMag_Effect_ElemResist = new(0x0C, 0xB9F4);
			public static BA BtlMag_Effect_AbsorbUp = new(0x0C, 0xB9E4);
			public static BA BtlMag_Effect_CureAilment = new(0x0C, 0xB9CD);
			public static BA BtlMag_Effect_RecoverHP = new(0x0C, 0xB999);
			public static BA BtlMag_Effect_LowerMorale = new(0x0C, 0xB979);
			public static BA BtlMag_Effect_Slow = new(0x0C, 0xB95E);
			public static BA BtlMag_Effect_InflictAilment = new(0x0C, 0xB92F);
			public static BA BtlMag_Effect_DamageUndead = new(0x0C, 0xB905);
			public static BA BtlMag_Effect_Damage = new(0x0C, 0xB899);
			public static BA BtlMag_HandleAilmentChanges = new(0x0C, 0xBAF8);
			public static BA lut_CharStatsPtrTable_alt = new(0x0C, 0xB67C);
			public static BA lut_IBCharStatsPtrTable_alt = new(0x0C, 0xB674);
			public static BA BtlMag_SavePlayerDefenderStats = new(0x0C, 0xB790);
			public static BA BtlMag_SaveEnemyDefenderStats = new(0x0C, 0xB757);
			public static BA BtlMag_PerformSpellEffect = new(0x0C, 0xB7CD);
			public static BA BtlMag_LoadEnemyDefenderStats = new(0x0C, 0xB6C8);
			public static BA Battle_CastMagicOnAllEnemies = new(0x0C, 0xB563);
			public static BA Battle_CastMagicOnSelf_Enemy = new(0x0C, 0xB544);
			public static BA Battle_CastMagicOnRandomPlayer = new(0x0C, 0xB59C);
			public static BA Battle_CastMagicOnAllPlayers = new(0x0C, 0xB5B6);
			public static BA Battle_EnemyMagic_CastOnTarget = new(0x0C, 0xB524);
			public static BA PrepareEnemyMagAttack = new(0x0C, 0xB730);
			public static BA PrepEntityPtr_Enemy = new(0x0C, 0xB4BE);
			public static BA Battle_CastMagicOnEnemy = new(0x0C, 0xB593);
			public static BA Battle_CastMagicOnPlayer_NoLoad = new(0x0C, 0xB5B0);
			public static BA BtlMag_LoadPlayerDefenderStats = new(0x0C, 0xB602);
			public static BA Battle_PlMag_IsPlayerValid = new(0x0C, 0xB430);
			public static BA Battle_CastMagicOnPlayer = new(0x0C, 0xB5AD);
			public static BA Battle_PlMag_TargetAllPlayers = new(0x0C, 0xB448);
			public static BA Battle_PlMag_TargetOnePlayer = new(0x0C, 0xB437);
			public static BA Battle_PlMag_TargetSelf = new(0x0C, 0xB427);
			public static BA Battle_PlMag_TargetOneEnemy = new(0x0C, 0xB480);
			public static BA Battle_PlMag_TargetAllEnemies = new(0x0C, 0xB495);
			public static BA PreparePlayerMagAttack = new(0x0C, 0xB684);
			public static BA Battle_EndMagicTurn = new(0x0C, 0xB50E);
			public static BA Battle_PlayerMagic_CastOnTarget = new(0x0C, 0xB40A);
			public static BA PrepEntityPtr_Player = new(0x0C, 0xB5EB);
			public static BA Player_DoMagicEffect = new(0x0C, 0xB3CA);
			public static BA GetRandomPlayerTarget = new(0x0C, 0xB325);
			public static BA Enemy_DoMagicEffect = new(0x0C, 0xB4D5);
			public static BA ChooseAndAttackPlayer = new(0x0C, 0xB319);
			public static BA EnemyAi_ShouldPerformAction = new(0x0C, 0xB294);
			public static BA ClearEnemyID = new(0x0C, 0xB288);
			public static BA TargetSelf = new(0x0C, 0xB53D);
			public static BA Enemy_DoAi = new(0x0C, 0xB2A2);
			public static BA Battle_CastMagicOnRandEnemy = new(0x0C, 0xB54D);
			public static BA Battle_PrepareMagic = new(0x0C, 0xB35C);
			public static BA ShowAltBattleMessage_ClearAllBoxes = new(0x0C, 0xB28E);
			public static BA ApplyEnemyAilmentMask = new(0x0C, 0xB190);
			public static BA Battle_ShowIneffective = new(0x0C, 0xB894);
			public static BA BtlMag_PrintMagicMessage = new(0x0C, 0xB13D);
			public static BA ShowAltBattleMessage = new(0x0C, 0xB0F2);
			public static BA DrawDamageCombatBox = new(0x0C, 0xB0CD);
			public static BA DrawCombatBox_Defender = new(0x0C, 0xB0B4);
			public static BA DrawCombatBox_Attack = new(0x0C, 0xB074);
			public static BA DrawCombatBox_RestoreAXY = new(0x0C, 0xB0C7);
			public static BA DrawCombatBox_Attacker = new(0x0C, 0xB05E);
			public static BA RespondDelay_UndrawAllBut2Boxes = new(0x0C, 0xB042);
			public static BA RestoreAXY = new(0x0C, 0xB03C);
			public static BA ClearAllCombatBoxes = new(0x0C, 0xB022);
			public static BA ClearAltMessageBuffer = new(0x0C, 0xB00C);
			public static BA DoesEnemyXExist = new(0x0C, 0xBBC6);
			public static BA MathBuf_CopyXToY = new(0x0C, 0xAF4D);
			public static BA DoubleXAndY = new(0x0C, 0xAF5D);
			public static BA LoadOneCharacterIBStats = new(0x0C, 0xACFF);
			public static BA TransferByte = new(0x0C, 0xACE1);
			public static BA lut_AilmentWeight = new(0x0C, 0xAC92);
			public static BA SwapCharOBStats = new(0x0C, 0xAC80);
			public static BA SwapCharsForSorting = new(0x0C, 0xAC57);
			public static BA GetCharOBMagDataPointers = new(0x0C, 0xAC3D);
			public static BA GetCharOBPointers = new(0x0C, 0xAC23);
			public static BA Do3Frames_UpdatePalette = new(0x0C, 0xABFC);
			public static BA FadeOutOneShade = new(0x0C, 0xABD8);
			public static BA ResetUsePalette = new(0x0C, 0xAB80);
			public static BA DrawStatusRow = new(0x0C, 0xAAFC);
			public static BA lut_CharStatusPPUAddr = new(0x0C, 0xAB21);
			public static BA FormatHPForText = new(0x0C, 0xAA2E);
			public static BA DoDivision = new(0x0C, 0xAE2B);
			public static BA YXDivideA = new(0x0C, 0xAA1B);
			public static BA DrawBtlMsg_ClearIt = new(0x0C, 0xAA0D);
			public static BA DoPhysicalAttack_Exit = new(0x0C, 0xA985);
			public static BA MathBuf_Sub16 = new(0x0C, 0xAEAC);
			public static BA PrintPlayerAilmentMessageFromAttack = new(0x0C, 0xA988);
			public static BA MathBuf_Add16 = new(0x0C, 0xAE7B);
			public static BA PlayBattleSFX = new(0x0C, 0xBEB8);
			public static BA DoExplosionEffect = new(0x0C, 0xBBFA);
			public static BA ClearMathBufHighBytes = new(0x0C, 0xA65A);
			public static BA EnemyAttackPlayer_Physical = new(0x0C, 0xA581);
			public static BA DoPhysicalAttack = new(0x0C, 0xA67B);
			public static BA CapXYAtFF = new(0x0C, 0xA4AA);
			public static BA Battle_PlayerTryUnstun = new(0x0C, 0xA481);
			public static BA Battle_PlayerTryWakeup = new(0x0C, 0xA42F);
			public static BA Player_DoMagic = new(0x0C, 0xB3C5);
			public static BA Battle_PlayerTryRun = new(0x0C, 0xA3D8);
			public static BA Player_DoItem = new(0x0C, 0xB3B5);
			public static BA Player_DoDrink = new(0x0C, 0xB3BD);
			public static BA PlayerAttackEnemy_Physical = new(0x0C, 0xA4BA);
			public static BA Battle_DoEnemyTurn = new(0x0C, 0xB197);
			public static BA Battle_DoPlayerTurn = new(0x0C, 0xA357);
			public static BA UpdateVariablePalette = new(0x0C, 0xAB2F);
			public static BA WalkForwardAndCastMagic = new(0x0C, 0xA32D);
			public static BA MathBuf_NZ = new(0x0C, 0xAF3C);
			public static BA ApplyPoisonToPlayer = new(0x0C, 0xA2C3);
			public static BA MathBuf_Compare = new(0x0C, 0xADEA);
			public static BA GetEnemyStatPtr = new(0x0C, 0xBB86);
			public static BA ApplyRegenToAllEnemies = new(0x0C, 0xA232);
			public static BA EraseEnemyGraphic = new(0x0C, 0xBBEE);
			public static BA CheckForBattleEnd = new(0x0C, 0xA20D);
			public static BA DoBattleRound_RTS = new(0x0C, 0xA1DA);
			public static BA ApplyEndOfRoundEffects = new(0x0C, 0xA2A6);
			public static BA BattleTurnEnd_CheckForBattleEnd = new(0x0C, 0xA1DB);
			public static BA Battle_DoTurn = new(0x0C, 0xA352);
			public static BA lut_InitialTurnOrder = new(0x0C, 0xA15C);
			public static BA FlashCharacterSprite = new(0x0C, 0xA0FC);
			public static BA lut_CharacterOAMOffset = new(0x0C, 0xA0F8);
			public static BA RespondDelay = new(0x0C, 0xA0E6);
			public static BA RespondDelay_ClearCombatBoxes = new(0x0C, 0xA0D7);
			public static BA lut_UnformattedCombatBoxBuffer = new(0x0C, 0xA0CD);
			public static BA DrawBattleMessageCombatBox = new(0x0C, 0xA09D);
			public static BA PrepareAndDrawSimpleCombatBox = new(0x0C, 0xA0A2);
			public static BA DrawPlayerAttackerCombatBox = new(0x0C, 0xA092);
			public static BA UpdateBattleSFX = new(0x0C, 0xBF13);
			public static BA DoFrame_UpdatePalette = new(0x0C, 0xAB50);
			public static BA lut_InBattleCharPaletteAssign = new(0x0C, 0xA03C);
			public static BA lut_CharMagicData = new(0x0C, 0x9F6D);
			public static BA lut_IBCharStatsPtrTable = new(0x0C, 0x9F65);
			public static BA lut_CharStatsPtrTable = new(0x0C, 0x9F5D);
			public static BA __PrepAttackSprite_Magic_AFrame = new(0x0C, 0x9ED6);
			public static BA __PrepAttackSprite_Weapon_AFrame = new(0x0C, 0x9EEF);
			public static BA PrepAttackSprite_BFrame = new(0x0C, 0x9F15);
			public static BA PrepAttackSprite_AFrame = new(0x0C, 0x9ED1);
			public static BA WalkForwardAndStrike = new(0x0C, 0x9E70);
			public static BA UpdateSprites_TwoFrames = new(0x0C, 0x9ECB);
			public static BA CharacterWalkAnimation = new(0x0C, 0x9E1C);
			public static BA lut_MagicCursorPos = new(0x0C, 0x9FBF);
			public static BA lut_DrinkCursorPos = new(0x0C, 0x9FE7);
			public static BA lut_ItemCursorPos = new(0x0C, 0x9FD7);
			public static BA BattleTarget_Up = new(0x0C, 0x9BD9);
			public static BA BattleTarget_Down = new(0x0C, 0x9BBB);
			public static BA UpdateInputDelayCounter = new(0x0C, 0x9F4D);
			public static BA BattleTarget_DownSub = new(0x0C, 0x9BCB);
			public static BA lut_TargetMixCursorPos = new(0x0C, 0x9FAF);
			public static BA lut_EnemyIndexMix = new(0x0C, 0xA004);
			public static BA lut_Target4LargeCursorPos = new(0x0C, 0x9FA7);
			public static BA lut_EnemyIndex4Large = new(0x0C, 0xA000);
			public static BA EnemyTargetMenu = new(0x0C, 0x9B55);
			public static BA lut_Target9SmallCursorPos = new(0x0C, 0x9F95);
			public static BA lut_EnemyIndex9Small = new(0x0C, 0x9FF7);
			public static BA EnemyTargetMenu_FiendChaos = new(0x0C, 0x9ACF);
			public static BA EnemyTargetMenu_Mix = new(0x0C, 0x9B2E);
			public static BA EnemyTargetMenu_4Large = new(0x0C, 0x9B04);
			public static BA EnemyTargetMenu_9Small = new(0x0C, 0x9ADA);
			public static BA lut_EnemyTargetMenuJumpTbl = new(0x0C, 0x9AC5);
			public static BA lut_PlayerTargetCursorPos = new(0x0C, 0x9F85);
			public static BA MenuSelection_2x4 = new(0x0C, 0x9D0E);
			public static BA lut_MainCombatBoxCursorPos = new(0x0C, 0x9F75);
			public static BA lut_WeaponSwingTSA = new(0x0C, 0xA02C);
			public static BA DrawCharacter_NextTile = new(0x0C, 0x99D3);
			public static BA lut_CharacterPoseTSA = new(0x0C, 0xA00C);
			public static BA DrawCharacter_DeadRow = new(0x0C, 0x99B4);
			public static BA DoMagicFlash = new(0x0C, 0xA051);
			public static BA DrawWeaponGraphicRow = new(0x0C, 0x99E5);
			public static BA DrawCharacter = new(0x0C, 0x9910);
			public static BA Draw16x8SpriteRow = new(0x0C, 0x9998);
			public static BA BattleDraw8x8Sprite = new(0x0C, 0x97E9);
			public static BA DoMiniFrame = new(0x0C, 0xAB29);
			public static BA BattleClearVariableSprite = new(0x0C, 0x97B2);
			public static BA BattleUpdateAudio = new(0x0C, 0xA07C);
			public static BA SetNaturalPose = new(0x0C, 0x9DB2);
			public static BA SetAllNaturalPose = new(0x0C, 0x9776);
			public static BA BattleUpdatePPU = new(0x0C, 0x978A);
			public static BA BattleFrame = new(0x0C, 0x9799);
			public static BA BattleClearOAM = new(0x0C, 0x97A7);
			public static BA MultiplyXA = new(0x0C, 0xAE09);
			public static BA DoFrame_WithInput = new(0x0C, 0x97C7);
			public static BA data_NothingText = new(0x0C, 0xA048);
			public static BA GetPointerToArmorData = new(0x0C, 0xAC14);
			public static BA GetPointerToWeaponData = new(0x0C, 0xAC05);
			public static BA MenuSelection_Item = new(0x0C, 0x9BF8);
			public static BA MenuSelection_Drink = new(0x0C, 0x9C06);
			public static BA SelectPlayerTarget = new(0x0C, 0x9A3A);
			public static BA GetPointerToMagicData = new(0x0C, 0x9711);
			public static BA DoNothingMessageBox = new(0x0C, 0x96FE);
			public static BA MenuSelection_Magic = new(0x0C, 0x9C14);
			public static BA SelectEnemyTarget = new(0x0C, 0x9AA4);
			public static BA BattleSubMenu_Item = new(0x0C, 0x9679);
			public static BA BattleSubMenu_Drink = new(0x0C, 0x95F5);
			public static BA BattleSubMenu_Magic = new(0x0C, 0x94F5);
			public static BA BattleSubMenu_Fight = new(0x0C, 0x94E6);
			public static BA lut_BattleSubMenu = new(0x0C, 0x94DE);
			public static BA CancelBattleAction = new(0x0C, 0x94AF);
			public static BA Battle_MainMenu_APressed = new(0x0C, 0x94B8);
			public static BA PrepAndGetBattleMainCommand = new(0x0C, 0x9A2C);
			public static BA CharWalkAnimationLeft = new(0x0C, 0x9E01);
			public static BA InputCharacterBattleCommand = new(0x0C, 0x9496);
			public static BA BacktrackBattleCommand = new(0x0C, 0x945B);
			public static BA DoBattleRound = new(0x0C, 0xA169);
			public static BA __GetCharacterBattleCommand_Backtrack_2 = new(0x0C, 0x9443);
			public static BA __GetCharacterBattleCommand_Backtrack_1 = new(0x0C, 0x943E);
			public static BA GetCharacterBattleCommand = new(0x0C, 0x9477);
			public static BA __GetCharacterBattleCommand_Backtrack_0 = new(0x0C, 0x9439);
			public static BA UpdateSprites_BattleFrame = new(0x0C, 0x980C);
			public static BA RebuildEnemyRoster = new(0x0C, 0xBBA4);
			public static BA BattleLogicLoop_DoCombat = new(0x0C, 0x9452);
			public static BA DrawBtlMsg_ClearCombatBoxes = new(0x0C, 0xAA07);
			public static BA ZeroXYIfNegative = new(0x0C, 0xA4B2);
			public static BA MathBuf_Sub = new(0x0C, 0xAF0A);
			public static BA MathBuf_Add = new(0x0C, 0xAEDD);
			public static BA RandAX = new(0x0C, 0xAE5D);
			public static BA BattleLogicLoop = new(0x0C, 0x9420);
			public static BA PrepCharStatPointers = new(0x0C, 0xA145);
			public static BA UndoCharacterBattleCommand = new(0x0C, 0x937A);
			public static BA CharWalkAnimationRight = new(0x0C, 0x9E06);
			public static BA SetCharacterBattleCommand = new(0x0C, 0x935D);
			public static BA PlayFanfareAndCheer = new(0x0C, 0x9E43);
			public static BA CheckForEndOfBattle = new(0x0C, 0x933B);
			public static BA Battle_AfterFadeIn = new(0x0C, 0x93AE);
			public static BA DrawCharacterStatus = new(0x0C, 0xAA50);
			public static BA LoadAllCharacterIBStats = new(0x0C, 0xACEB);
			public static BA EnterBattlePrepareSprites = new(0x0C, 0x9724);
			public static BA BattleFadeIn = new(0x0C, 0xAB8C);
			public static BA LoadEnemyStats = new(0x0C, 0xAF64);
			public static BA FinishBattlePrepAndFadeIn = new(0x0C, 0x9306);
			public static BA FinishBattlePrepAndFadeIn_L = new(0x0C, 0x9306);
			public static BA BattleFadeOutAndRestartGame = new(0x0C, 0x9355);
			public static BA BattleFadeOutAndRestartGame_L = new(0x0C, 0x9303);
			public static BA ExitBattle_L = new(0x0C, 0x9300);
			public static BA WaitFrames_BattleResult = new(0x0C, 0x932A);
			public static BA WaitFrames_BattleResult_RTS = new(0x0C, 0x933A);
			public static BA BattleFadeOut = new(0x0C, 0xABC4);
			public static BA ReSortPartyByAilment = new(0x0C, 0xAC96);
			public static BA ExitBattle = new(0x0C, 0x92E0);
			public static BA lut_EnemyAi = new(0x0C, 0x9020);
			public static BA data_EnemyStats = new(0x0C, 0x8520);
			public static BA lut_MagicBattleMessages = new(0x0C, 0x84C0);
			public static BA lut_MagicData = new(0x0C, 0x81E0);
			public static BA lut_ArmorData = new(0x0C, 0x8140);
			public static BA lut_WeaponData = new(0x0C, 0x8000);
			public static BA BankC_CrossBankJumpList = new(0x0C, 0x9300);
			public static BA lut_BattlePalettes = new(0x0C, 0x8F20);
			public static BA DrawPuzzlePiece = new(0x0D, 0xBE30);
			public static BA lut_CreditsText = new(0x0D, 0xBB00);
			public static BA lut_EndingBGPal = new(0x0D, 0xBF10);
			public static BA Wait100Cycs = new(0x0D, 0xB9DB);
			public static BA WaitForShutterStart = new(0x0D, 0xB9E2);
			public static BA Story_CloseShutters = new(0x0D, 0xB93F);
			public static BA Story_Wait = new(0x0D, 0xB911);
			public static BA Story_DrawText = new(0x0D, 0xBA0B);
			public static BA Bridge_LoadPalette = new(0x0D, 0xB9F3);
			public static BA _Story_StartPPU = new(0x0D, 0xB8CD);
			public static BA Ending_LoadPalette = new(0x0D, 0xB9FF);
			public static BA lut_StorySecondaryAttrib = new(0x0D, 0xBA7F);
			public static BA Story_EndFrame = new(0x0D, 0xBA70);
			public static BA Bridge_StartPPU = new(0x0D, 0xB8CA);
			public static BA Story_OpenShutters = new(0x0D, 0xB98D);
			public static BA Story_DoPage = new(0x0D, 0xB8E3);
			public static BA Ending_StartPPU = new(0x0D, 0xB8C4);
			public static BA Story_FillSecAttrib = new(0x0D, 0xB899);
			public static BA EnterBridgeScene = new(0x0D, 0xB847);
			public static BA lut_EnvPatterns = new(0x0D, 0xB3C9);
			public static BA lut_OctaveOffset = new(0x0D, 0xB2F3);
			public static BA lut_NoteFreqs = new(0x0D, 0xB2F9);
			public static BA lut_NoteLengths = new(0x0D, 0xB359);
			public static BA Music_ApplyLength_Rest = new(0x0D, 0xB2A8);
			public static BA Music_ApplyLength = new(0x0D, 0xB292);
			public static BA Music_ApplyTone = new(0x0D, 0xB268);
			public static BA Music_SetTempo = new(0x0D, 0xB2B8);
			public static BA lut_EnvSpeeds = new(0x0D, 0xB3B9);
			public static BA Music_SetEnvPattern = new(0x0D, 0xB2DF);
			public static BA Music_SetOctave = new(0x0D, 0xB2CD);
			public static BA Music_LoopCode = new(0x0D, 0xB217);
			public static BA Music_Rest = new(0x0D, 0xB25A);
			public static BA Music_Note = new(0x0D, 0xB250);
			public static BA Music_DoScore_IncByA = new(0x0D, 0xB194);
			public static BA Music_DoScore = new(0x0D, 0xB1A3);
			public static BA Music_ChannelScore = new(0x0D, 0xB189);
			public static BA Music_NewSong = new(0x0D, 0xB003);
			public static BA MusicPlay = new(0x0D, 0xB099);
			public static BA lut_MGDirection_X = new(0x0D, 0xAF57);
			public static BA lut_MGDirection_Y = new(0x0D, 0xAF53);
			public static BA MiniGame_HorzSlide = new(0x0D, 0xBAA0);
			public static BA MiniGame_CheckVictory = new(0x0D, 0xAF5B);
			public static BA MiniGame_ProcessInput = new(0x0D, 0xAFC8);
			public static BA DrawAllPuzzlePieces = new(0x0D, 0xBE00);
			public static BA MiniGameLoop = new(0x0D, 0xAECF);
			public static BA ClearOAM_BankD = new(0x0D, 0xAFAB);
			public static BA lut_BridgeBGPal = new(0x0D, 0xBF00);
			public static BA lut_PuzzleStart = new(0x0D, 0xAFB8);
			public static BA lut_Ending_LastPage = new(0x0D, 0xAE01);
			public static BA lut_Bridge_LastPage = new(0x0D, 0xAE00);
			public static BA lut_StoryText = new(0x0D, 0xA800);
			public static BA MiniGame_AnimateSlide = new(0x0D, 0xBE9E);
			public static BA MiniGame_VertSlide = new(0x0D, 0xA780);
			public static BA TheEnd_SetPixel = new(0x0D, 0xA64C);
			public static BA lut_TheEndPixelMasks = new(0x0D, 0xA771);
			public static BA lut_TheEndPixelPosition_Yhi = new(0x0D, 0xA6D1);
			public static BA lut_TheEndPixelPosition_Ylo = new(0x0D, 0xA681);
			public static BA lut_TheEndPixelPosition_X = new(0x0D, 0xA721);
			public static BA TheEnd_MovePos = new(0x0D, 0xA604);
			public static BA TheEnd_MoveAndSet = new(0x0D, 0xA5E4);
			public static BA lut_TheEnd_AttrTable = new(0x0D, 0xA639);
			public static BA TheEnd_EndVblank = new(0x0D, 0xA5F4);
			public static BA DrawFancyTheEndGraphic = new(0x0D, 0xA400);
			public static BA data_TheEndDrawData = new(0x0D, 0xA000);
			public static BA lut_MinigameCHR = new(0x0D, 0x9E00);
			public static BA MiniGame_ShufflePuzzle = new(0x0D, 0x9DA0);
			public static BA lut_ScoreData = new(0x0D, 0x8000);
			public static BA lut_IntroStoryText = new(0x0D, 0xBF20);
			public static BA __Nasir_CRC_High_Byte = new(0x0D, 0xBA18);
			public static BA EnterBridgeScene_L = new(0x0D, 0xB800);
			public static BA EnterMiniGame = new(0x0D, 0xAE10);
			public static BA MusicPlay_L = new(0x0D, 0xB000);
			public static BA EnterEndingScene = new(0x0D, 0xB803);
			public static BA EquipMenuCurs_DoDraw = new(0x0E, 0xBEBC);
			public static BA lut_EquipMenuCurs = new(0x0E, 0xBEC6);
			public static BA SetPPUAddrTo_23aa = new(0x0E, 0xBF2E);
			public static BA UpdateEquipMenuModeAttrib = new(0x0E, 0xBEF7);
			public static BA lut_ArmorTypes = new(0x0E, 0xBCD1);
			public static BA lut_ArmorPermissions = new(0x0E, 0xBFA0);
			public static BA lut_WeaponPermissions = new(0x0E, 0xBF50);
			public static BA lut_ClassEquipBit = new(0x0E, 0xBCB9);
			public static BA IsEquipLegal = new(0x0E, 0xBC0A);
			public static BA DrawEquipMenuCursSecondary = new(0x0E, 0xBEA4);
			public static BA MoveEquipMenuCurs = new(0x0E, 0xBD6E);
			public static BA DrawEquipMenuCurs = new(0x0E, 0xBEB5);
			public static BA EquipMenu_DROP = new(0x0E, 0xBBBE);
			public static BA EquipMenu_TRADE = new(0x0E, 0xBB31);
			public static BA EquipMenu_EQUIP = new(0x0E, 0xBB82);
			public static BA CopyEquipFromItemBox = new(0x0E, 0xBDC8);
			public static BA MoveEquipMenuModeCurs = new(0x0E, 0xBD42);
			public static BA EquipMenuFrame = new(0x0E, 0xBCF9);
			public static BA DrawEquipMenuModeCurs = new(0x0E, 0xBEE6);
			public static BA CopyEquipToItemBox = new(0x0E, 0xBD9D);
			public static BA DrawEquipMenu = new(0x0E, 0xBDF3);
			public static BA DrawCharMenuString_Len = new(0x0E, 0xB95B);
			public static BA lut_MainItemBoxes = new(0x0E, 0xBAA2);
			public static BA LoadMainItemBoxDims = new(0x0E, 0xB8F5);
			public static BA DrawMenuString = new(0x0E, 0xB938);
			public static BA DrawMainMenuCharBoxBody = new(0x0E, 0xB982);
			public static BA DrawMainMenuOptionBox = new(0x0E, 0xB911);
			public static BA DrawMainMenuGoldBox = new(0x0E, 0xB86E);
			public static BA DrawOrbBox = new(0x0E, 0xB878);
			public static BA lut_ItemMenuCursor = new(0x0E, 0xB7E6);
			public static BA lut_MainMenuCursor_Y = new(0x0E, 0xB7E1);
			public static BA lut_MainMenuSubCursor = new(0x0E, 0xB7D9);
			public static BA EraseDescBox = new(0x0E, 0xB94D);
			public static BA MoveMagicMenuCursor_Exit = new(0x0E, 0xB6DC);
			public static BA CloseDescBox_Sfx = new(0x0E, 0xB771);
			public static BA MoveItemMenuCurs_Exit = new(0x0E, 0xB5AB);
			public static BA _MenuRecoverHP_Done = new(0x0E, 0xB579);
			public static BA _MenuRecoverHP_OverMax = new(0x0E, 0xB59C);
			public static BA _MenuRecoverHP_CheckLow = new(0x0E, 0xB592);
			public static BA _MenuRecoverHP_Exit = new(0x0E, 0xB591);
			public static BA MenuRecoverHP = new(0x0E, 0xB556);
			public static BA TurnMenuScreenOn = new(0x0E, 0xB783);
			public static BA DrawMenuComplexString = new(0x0E, 0xB944);
			public static BA DrawItemTargetCursor = new(0x0E, 0xB3EE);
			public static BA _UseItem_Heal_CantUse = new(0x0E, 0xB332);
			public static BA UseItem_Exit = new(0x0E, 0xB32F);
			public static BA _UseItem_Heal_Loop = new(0x0E, 0xB309);
			public static BA CloseDescBox = new(0x0E, 0xB779);
			public static BA MenuSaveConfirm = new(0x0E, 0xB2E0);
			public static BA DrawItemDescBox_Fanfare = new(0x0E, 0xB927);
			public static BA UseItem_SetDesc = new(0x0E, 0xB1B8);
			public static BA UseItem_Soft = new(0x0E, 0xB360);
			public static BA UseItem_Pure = new(0x0E, 0xB338);
			public static BA UseItem_Heal = new(0x0E, 0xB301);
			public static BA UseItem_House = new(0x0E, 0xB2BE);
			public static BA UseItem_Cabin = new(0x0E, 0xB2A1);
			public static BA UseItem_Tent = new(0x0E, 0xB284);
			public static BA UseItem_Canoe = new(0x0E, 0xB1EA);
			public static BA UseItem_Oxyale = new(0x0E, 0xB1E6);
			public static BA UseItem_Bottle = new(0x0E, 0xB1EE);
			public static BA UseItem_Cube = new(0x0E, 0xB1E2);
			public static BA UseItem_Tail = new(0x0E, 0xB1DE);
			public static BA UseItem_Chime = new(0x0E, 0xB1DA);
			public static BA UseItem_Floater = new(0x0E, 0xB25F);
			public static BA UseItem_Rod = new(0x0E, 0xB20E);
			public static BA UseItem_Ruby = new(0x0E, 0xB1D6);
			public static BA UseItem_Slab = new(0x0E, 0xB1D2);
			public static BA UseItem_Adamant = new(0x0E, 0xB1CE);
			public static BA UseItem_TNT = new(0x0E, 0xB1CA);
			public static BA UseItem_Key = new(0x0E, 0xB1C6);
			public static BA UseItem_Herb = new(0x0E, 0xB1C2);
			public static BA UseItem_Crystal = new(0x0E, 0xB1BE);
			public static BA UseItem_Crown = new(0x0E, 0xB1B6);
			public static BA UseItem_Lute = new(0x0E, 0xB236);
			public static BA UseItem_Bad = new(0x0E, 0xB1B3);
			public static BA MoveItemMenuCurs = new(0x0E, 0xB5AC);
			public static BA DrawItemMenuCursor = new(0x0E, 0xB7C8);
			public static BA ItemMenu_Loop = new(0x0E, 0xB148);
			public static BA DrawItemTitleBox = new(0x0E, 0xB91D);
			public static BA ResumeItemMenu = new(0x0E, 0xB129);
			public static BA UseMagic_DoEXIT = new(0x0E, 0xB0FF);
			public static BA UseMagic_PURE_CantUse = new(0x0E, 0xB0A1);
			public static BA UseMagic_PURE_Exit = new(0x0E, 0xB09E);
			public static BA UseMagic_PURE_Loop = new(0x0E, 0xB085);
			public static BA CureOBAilment = new(0x0E, 0xB388);
			public static BA MenuRecoverPartyHP = new(0x0E, 0xB53F);
			public static BA HealFamily_Exit = new(0x0E, 0xB015);
			public static BA MenuWaitForBtn = new(0x0E, 0xB625);
			public static BA UseMagic_HealFamily = new(0x0E, 0xAFF5);
			public static BA MenuRecoverHP_Abs = new(0x0E, 0xB561);
			public static BA CureFamily_CantUse = new(0x0E, 0xAFA7);
			public static BA CureFamily_Exit = new(0x0E, 0xAFA4);
			public static BA ItemTargetMenuLoop = new(0x0E, 0xB3A0);
			public static BA CureFamily_Loop = new(0x0E, 0xAF7C);
			public static BA DrawItemTargetMenu = new(0x0E, 0xB400);
			public static BA UseMagic_CureFamily = new(0x0E, 0xAF72);
			public static BA UseMagic_EXIT = new(0x0E, 0xB0F2);
			public static BA UseMagic_SOFT = new(0x0E, 0xB0A7);
			public static BA UseMagic_WARP = new(0x0E, 0xB0D1);
			public static BA UseMagic_LIF2 = new(0x0E, 0xB047);
			public static BA UseMagic_LIFE = new(0x0E, 0xB018);
			public static BA UseMagic_PURE = new(0x0E, 0xB07D);
			public static BA UseMagic_HEL2 = new(0x0E, 0xAFE5);
			public static BA UseMagic_HEL3 = new(0x0E, 0xAFEE);
			public static BA UseMagic_HEAL = new(0x0E, 0xAFDC);
			public static BA UseMagic_CUR4 = new(0x0E, 0xAFAD);
			public static BA UseMagic_CUR3 = new(0x0E, 0xAF6C);
			public static BA UseMagic_CUR2 = new(0x0E, 0xAF64);
			public static BA UseMagic_CURE = new(0x0E, 0xAF5C);
			public static BA DrawItemDescBox = new(0x0E, 0xB92B);
			public static BA UseMagic_GetRequiredMP = new(0x0E, 0xB102);
			public static BA MoveMagicMenuCursor = new(0x0E, 0xB6DD);
			public static BA DrawMagicMenuCursor = new(0x0E, 0xB816);
			public static BA MagicMenu_Loop = new(0x0E, 0xAEC0);
			public static BA DrawCharMenuString = new(0x0E, 0xB959);
			public static BA DrawMainItemBox = new(0x0E, 0xB8EF);
			public static BA DrawMagicMenuMainBox = new(0x0E, 0xBA6D);
			public static BA MoveMainMenuSubCursor = new(0x0E, 0xB68C);
			public static BA DrawMainMenuSubCursor = new(0x0E, 0xB7A9);
			public static BA EnterStatusMenu = new(0x0E, 0xB4AD);
			public static BA EnterEquipMenu = new(0x0E, 0xBACA);
			public static BA EnterMagicMenu = new(0x0E, 0xAE97);
			public static BA MainMenuSubTarget = new(0x0E, 0xAE71);
			public static BA EnterItemMenu = new(0x0E, 0xB11D);
			public static BA MoveCursorUpDown = new(0x0E, 0xB6AB);
			public static BA MenuFrame = new(0x0E, 0xB65D);
			public static BA DrawMainMenuCharSprites = new(0x0E, 0xB635);
			public static BA DrawMainMenuCursor = new(0x0E, 0xB7BA);
			public static BA MainMenuLoop = new(0x0E, 0xADE8);
			public static BA DrawMainMenu = new(0x0E, 0xB83A);
			public static BA ResumeMainMenu = new(0x0E, 0xADCD);
			public static BA lutMenuPalettes = new(0x0E, 0xAD78);
			public static BA lut_BIT = new(0x0E, 0xAC38);
			public static BA lut_MagicPermisPtr = new(0x0E, 0xAD00);
			public static BA lut_ShopBox_Ht = new(0x0E, 0xAC4F);
			public static BA lut_ShopBox_Wd = new(0x0E, 0xAC4A);
			public static BA lut_ShopBox_Y = new(0x0E, 0xAC45);
			public static BA lut_ShopBox_X = new(0x0E, 0xAC40);
			public static BA lut_ShopCurs_List = new(0x0E, 0xA97F);
			public static BA _CommonShopLoop_Main = new(0x0E, 0xA91A);
			public static BA lut_ShopCurs_Cmd = new(0x0E, 0xA977);
			public static BA CommonShopLoop_List = new(0x0E, 0xA912);
			public static BA DrawShopString = new(0x0E, 0xAA26);
			public static BA lut_ShopkeepImage = new(0x0E, 0xAC9C);
			public static BA lut_ShopkeepAdditive = new(0x0E, 0xAC54);
			public static BA lut_ShopAttributes = new(0x0E, 0xAC5C);
			public static BA _ShopFrame_CheckBtns = new(0x0E, 0xA758);
			public static BA DrawShopCursor = new(0x0E, 0xA9EF);
			public static BA ShopFrame = new(0x0E, 0xA727);
			public static BA CommonShopLoop_Cmd = new(0x0E, 0xA907);
			public static BA DrawShopComplexString = new(0x0E, 0xAA32);
			public static BA DrawShopBox = new(0x0E, 0xAA3B);
			public static BA Clinic_SelectTarget = new(0x0E, 0xA6D7);
			public static BA ClinicBuildNameString = new(0x0E, 0xA6ED);
			public static BA ClinicShop_Exit = new(0x0E, 0xA5A1);
			public static BA ShopFrameNoCursor = new(0x0E, 0xA743);
			public static BA DrawShopPartySprites = new(0x0E, 0xA9FA);
			public static BA LoadShopBoxDims = new(0x0E, 0xAA41);
			public static BA SaveGame = new(0x0E, 0xAB69);
			public static BA MenuRecoverPartyMP = new(0x0E, 0xABF3);
			public static BA MenuFillPartyHP = new(0x0E, 0xABD2);
			public static BA InnClinic_CanAfford = new(0x0E, 0xA689);
			public static BA DrawInnClinicConfirm = new(0x0E, 0xAA9B);
			public static BA ShopLoop_BuyExit = new(0x0E, 0xA8B1);
			public static BA ItemShop_Loop = new(0x0E, 0xA481);
			public static BA ItemShop_CancelBuy = new(0x0E, 0xA472);
			public static BA ItemShop_Exit = new(0x0E, 0xA471);
			public static BA DrawShopGoldBox = new(0x0E, 0xA7EF);
			public static BA DrawShopSellItemConfirm = new(0x0E, 0xAAB4);
			public static BA EquipMenu_BuildSellBox = new(0x0E, 0xA806);
			public static BA EquipShop_GiveItemToChar = new(0x0E, 0xA61D);
			public static BA ShopSelectBuyItem = new(0x0E, 0xA857);
			public static BA LoadShopInventory = new(0x0E, 0xA7D1);
			public static BA ShopLoop_BuySellExit = new(0x0E, 0xA8D3);
			public static BA EquipShop_Loop = new(0x0E, 0xA3B9);
			public static BA EquipShop_Cancel = new(0x0E, 0xA3AC);
			public static BA ShopPayPrice = new(0x0E, 0xA4CD);
			public static BA Shop_CanAfford = new(0x0E, 0xA4EB);
			public static BA ShopLoop_YesNo = new(0x0E, 0xA8C2);
			public static BA DrawShopBuyItemConfirm = new(0x0E, 0xAA65);
			public static BA MagicShop_AssertLearn = new(0x0E, 0xAADF);
			public static BA ShopSelectBuyMagic = new(0x0E, 0xA989);
			public static BA ShopLoop_CharNames = new(0x0E, 0xA8E4);
			public static BA MagicShop_Loop = new(0x0E, 0xA365);
			public static BA DrawShopDialogueBox = new(0x0E, 0xAA5B);
			public static BA MagicShop_CancelPurchase = new(0x0E, 0xA358);
			public static BA MagicShop_Exit = new(0x0E, 0xA357);
			public static BA DrawShop = new(0x0E, 0xA778);
			public static BA EnterShop_Caravan = new(0x0E, 0xA47A);
			public static BA EnterShop_Item = new(0x0E, 0xA47C);
			public static BA EnterShop_Inn = new(0x0E, 0xA508);
			public static BA EnterShop_Clinic = new(0x0E, 0xA5A2);
			public static BA EnterShop_Magic = new(0x0E, 0xA360);
			public static BA EnterShop_Equip = new(0x0E, 0xA3B4);
			public static BA lut_ShopEntryJump = new(0x0E, 0xA320);
			public static BA IntroStory_WriteAttr = new(0x0E, 0xA2DA);
			public static BA IntroStory_AnimateRow = new(0x0E, 0xA2A7);
			public static BA IntroStory_Frame = new(0x0E, 0xA2F2);
			public static BA IntroStory_AnimateBlock = new(0x0E, 0xA28C);
			public static BA TitleScreen_DrawRespondRate = new(0x0E, 0xA238);
			public static BA lut_TitleCursor_Y = new(0x0E, 0xA272);
			public static BA lut_TitleText_RespondRate = new(0x0E, 0xA265);
			public static BA lut_TitleText_NewGame = new(0x0E, 0xA25C);
			public static BA lut_TitleText_Continue = new(0x0E, 0xA253);
			public static BA IntroStory_MainLoop = new(0x0E, 0xA274);
			public static BA CharName_DrawCursor = new(0x0E, 0x9F35);
			public static BA PtyGen_Joy = new(0x0E, 0x9E70);
			public static BA PtyGen_DrawCursor = new(0x0E, 0x9F26);
			public static BA NameInput_DrawName = new(0x0E, 0x9F7D);
			public static BA lut_NameInput = new(0x0E, 0xA011);
			public static BA lut_NameInputRowStart = new(0x0E, 0xA00A);
			public static BA CharName_Frame = new(0x0E, 0x9E4E);
			public static BA DrawNameInputScreen = new(0x0E, 0x9FB0);
			public static BA PtyGen_DrawOneText = new(0x0E, 0x9ECC);
			public static BA DoNameInput = new(0x0E, 0x9D50);
			public static BA PtyGen_Frame = new(0x0E, 0x9E33);
			public static BA TurnMenuScreenOn_ClearOAM = new(0x0E, 0xB780);
			public static BA PtyGen_DrawText = new(0x0E, 0x9EBC);
			public static BA PtyGen_DrawBoxes = new(0x0E, 0x9E90);
			public static BA MenuWaitForBtn_SFX = new(0x0E, 0xB613);
			public static BA PtyGen_DrawChars = new(0x0E, 0x9F4E);
			public static BA PtyGen_DrawScreen = new(0x0E, 0x9CF8);
			public static BA DoPartyGen_OnCharacter = new(0x0E, 0x9D15);
			public static BA lut_PtyGenBuf = new(0x0E, 0xA0AE);
			public static BA DrawCharacterName = new(0x0E, 0x9C2E);
			public static BA UnusedRoutine_9B3E = new(0x0E, 0x9B3E);
			public static BA LineupMenu_AnimStep = new(0x0E, 0x9AFB);
			public static BA LUMode_Exit = new(0x0E, 0x9A88);
			public static BA LUJoy_Exit = new(0x0E, 0x9A13);
			public static BA LineupMenu_Finalize = new(0x0E, 0x9BBD);
			public static BA LUJoy_BPressed = new(0x0E, 0x99FD);
			public static BA PlaySFX_MenuMove = new(0x0E, 0xAD9D);
			public static BA PlaySFX_MenuSel = new(0x0E, 0xAD84);
			public static BA LineupMenu_ProcessJoy = new(0x0E, 0x9A14);
			public static BA LineupMenu_ProcessMode = new(0x0E, 0x9A89);
			public static BA LineupMenu_UpdateJoy = new(0x0E, 0x99DD);
			public static BA LineupMenu_DrawCursor = new(0x0E, 0x9B2D);
			public static BA LineupMenu_DrawCharSprites = new(0x0E, 0x9B54);
			public static BA DrawLineupMenuNames = new(0x0E, 0x9B91);
			public static BA lut_LineupSlots = new(0x0E, 0x9B71);
			public static BA ClearNT = new(0x0E, 0x9C02);
			public static BA DoClassChange = new(0x0E, 0x95AE);
			public static BA HideMapObject = new(0x0E, 0x9273);
			public static BA Talk_BlackOrb = new(0x0E, 0x9502);
			public static BA Talk_Chime = new(0x0E, 0x9594);
			public static BA Talk_CubeBotBad = new(0x0E, 0x9586);
			public static BA Talk_CoOGuy = new(0x0E, 0x94A2);
			public static BA Talk_ifearthfire = new(0x0E, 0x9576);
			public static BA Talk_ifairship = new(0x0E, 0x956B);
			public static BA Talk_ifearthvamp = new(0x0E, 0x9559);
			public static BA Talk_ifkeytnt = new(0x0E, 0x9549);
			public static BA Talk_ifcanal = new(0x0E, 0x953E);
			public static BA Talk_ifcanoe = new(0x0E, 0x9533);
			public static BA Talk_4Orb = new(0x0E, 0x951F);
			public static BA Talk_GoBridge = new(0x0E, 0x94F0);
			public static BA Talk_ifevent = new(0x0E, 0x94E2);
			public static BA Talk_ifbridge = new(0x0E, 0x94D7);
			public static BA Talk_Invis = new(0x0E, 0x94C5);
			public static BA Talk_ifitem = new(0x0E, 0x94B8);
			public static BA Talk_Unused = new(0x0E, 0x94B7);
			public static BA Talk_fight = new(0x0E, 0x94AA);
			public static BA Talk_Replace = new(0x0E, 0x9495);
			public static BA Talk_norm = new(0x0E, 0x9492);
			public static BA Talk_CanoeSage = new(0x0E, 0x947D);
			public static BA Talk_Titan = new(0x0E, 0x9468);
			public static BA Talk_Fairy = new(0x0E, 0x9458);
			public static BA Talk_Princess2 = new(0x0E, 0x9448);
			public static BA Talk_CubeBot = new(0x0E, 0x9438);
			public static BA Talk_SubEng = new(0x0E, 0x9428);
			public static BA Talk_ifvis = new(0x0E, 0x941B);
			public static BA Talk_Bahamut = new(0x0E, 0x93FB);
			public static BA Talk_Sarda = new(0x0E, 0x93E4);
			public static BA Talk_Vampire = new(0x0E, 0x93D7);
			public static BA Talk_Unne = new(0x0E, 0x93BA);
			public static BA Talk_Matoya = new(0x0E, 0x9398);
			public static BA Talk_Smith = new(0x0E, 0x936C);
			public static BA Talk_Nerrick = new(0x0E, 0x9352);
			public static BA Talk_Astos = new(0x0E, 0x9338);
			public static BA Talk_ElfPrince = new(0x0E, 0x931E);
			public static BA Talk_ElfDoc = new(0x0E, 0x9301);
			public static BA Talk_Bikke = new(0x0E, 0x92D0);
			public static BA Talk_Princess1 = new(0x0E, 0x92BE);
			public static BA Talk_Garland = new(0x0E, 0x92B1);
			public static BA Talk_KingConeria = new(0x0E, 0x9297);
			public static BA Talk_None = new(0x0E, 0x9296);
			public static BA TalkNormTeleport = new(0x0E, 0x90CC);
			public static BA TalkBattle = new(0x0E, 0x90C5);
			public static BA ShowMapObject = new(0x0E, 0x90A4);
			public static BA HideThisMapObject = new(0x0E, 0x9096);
			public static BA IsObjectVisible = new(0x0E, 0x9091);
			public static BA ClearGameEventFlag = new(0x0E, 0x9088);
			public static BA SetGameEventFlag = new(0x0E, 0x907F);
			public static BA CheckGameEventFlag = new(0x0E, 0x9079);
			public static BA lut_MapObjTalkJumpTbl = new(0x0E, 0x90D3);
			public static BA lut_MapObjTalkData = new(0x0E, 0x95D5);
			public static BA lut_DecD3_lo = new(0x0E, 0x9022);
			public static BA lut_DecD3_md = new(0x0E, 0x9019);
			public static BA lut_DecD4_lo = new(0x0E, 0x9010);
			public static BA lut_DecD4_md = new(0x0E, 0x9007);
			public static BA lut_DecD5_lo = new(0x0E, 0x8FFE);
			public static BA lut_DecD5_md = new(0x0E, 0x8FF5);
			public static BA lut_DecD5_hi = new(0x0E, 0x8FEC);
			public static BA lut_DecD6_lo = new(0x0E, 0x8FE3);
			public static BA lut_DecD6_md = new(0x0E, 0x8FDA);
			public static BA lut_DecD6_hi = new(0x0E, 0x8FD1);
			public static BA TrimZeros_RTS = new(0x0E, 0x8ED1);
			public static BA TrimZeros_6Digits = new(0x0E, 0x8E9F);
			public static BA FormatNumber_6Digits = new(0x0E, 0x8ED2);
			public static BA TrimZeros_5Digits = new(0x0E, 0x8EA9);
			public static BA FormatNumber_5Digits = new(0x0E, 0x8F15);
			public static BA TrimZeros_4Digits = new(0x0E, 0x8EB3);
			public static BA FormatNumber_4Digits = new(0x0E, 0x8F58);
			public static BA PrintNumber_4Digit = new(0x0E, 0x8E7A);
			public static BA TrimZeros_3Digits = new(0x0E, 0x8EBD);
			public static BA FormatNumber_3Digits = new(0x0E, 0x8F89);
			public static BA TrimZeros_2Digits = new(0x0E, 0x8EC7);
			public static BA FormatNumber_2Digits = new(0x0E, 0x8FBA);
			public static BA PrintNumber_Exit = new(0x0E, 0x8E98);
			public static BA PrintNumber_6Digit = new(0x0E, 0x8E8E);
			public static BA PrintNumber_5Digit = new(0x0E, 0x8E84);
			public static BA PrintNumber_3Digit = new(0x0E, 0x8E70);
			public static BA PrintNumber_1Digit = new(0x0E, 0x8E5C);
			public static BA lut_MenuText = new(0x0E, 0x8500);
			public static BA TitleScreen_Music = new(0x0E, 0x84DC);
			public static BA IntroStory_Joy = new(0x0E, 0x84CA);
			public static BA IntroTitlePrepare = new(0x0E, 0xA219);
			public static BA TitleScreen_Copyright = new(0x0E, 0x8480);
			public static BA lut_ShopData = new(0x0E, 0x8300);
			public static BA lut_ShopStrings = new(0x0E, 0x8000);
			public static BA EnterIntroStory = new(0x0E, 0xA0EE);
			public static BA EnterTitleScreen = new(0x0E, 0xA156);
			public static BA EnterShop = new(0x0E, 0xA330);
			public static BA EnterMainMenu = new(0x0E, 0xADB3);
			public static BA NewGamePartyGeneration = new(0x0E, 0x9C54);
			public static BA EnterLineupMenu = new(0x0E, 0x9915);
			public static BA TalkToObject = new(0x0E, 0x902B);
			public static BA PrintGold = new(0x0E, 0x8E4A);
			public static BA PrintCharStat = new(0x0E, 0x8D70);
			public static BA PrintPrice = new(0x0E, 0x8E44);
			public static BA PrintNumber_2Digit = new(0x0E, 0x8E66);
			public static BA PaletteFrame = new(0x1F, 0xFF70);
			public static BA BackUpBatSprPalettes = new(0x1F, 0xFF64);
			public static BA BrightenBatSprPalettes = new(0x1F, 0xFF40);
			public static BA DimBatSprPalettes = new(0x1F, 0xFF20);
			public static BA OnIRQ = new(0x1F, 0xFEBB);
			public static BA OnNMI = new(0x1F, 0xFE9C);
			public static BA ClearBGPalette = new(0x1F, 0xFEBE);
			public static BA OnReset = new(0x1F, 0xFE2E);
			public static BA SetMMC1SwapMode = new(0x1F, 0xFE06);
			public static BA SwapPRG = new(0x1F, 0xFE1A);
			public static BA WaitForVBlank = new(0x1F, 0xFEA8);
			public static BA UNUSED_DrawBattle_IncSrcPtr = new(0x1F, 0xFCBB);
			public static BA lut_CharacterNamePtr = new(0x1F, 0xFCAA);
			public static BA DrawBattleSubString = new(0x1F, 0xFC94);
			public static BA DrawBattleSubString_Max8 = new(0x1F, 0xFC8D);
			public static BA BattleDrawLoadSubSrcPtr = new(0x1F, 0xFC00);
			public static BA DrawEnemyName = new(0x1F, 0xFC7A);
			public static BA DrawEntityName = new(0x1F, 0xFC4F);
			public static BA DrawString_SpaceRun = new(0x1F, 0xFC39);
			public static BA DrawBattleMessage = new(0x1F, 0xFC26);
			public static BA DrawBattleString_Code11_Short = new(0x1F, 0xFB93);
			public static BA DrawBattleString_Code0C = new(0x1F, 0xFB2F);
			public static BA DrawBattle_Number = new(0x1F, 0xFB3D);
			public static BA DrawBattleString_Code11 = new(0x1F, 0xFB1E);
			public static BA DrawBattle_Division = new(0x1F, 0xFAFC);
			public static BA DrawBattleString_IncDstPtr = new(0x1F, 0xFCC2);
			public static BA DrawBattleString_DrawChar = new(0x1F, 0xFAF1);
			public static BA DrawBattle_IncSrcPtr = new(0x1F, 0xFCB2);
			public static BA DrawBattleString_ExpandChar = new(0x1F, 0xFAA6);
			public static BA DrawBattleString_ControlCode = new(0x1F, 0xFB96);
			public static BA lut_EnemyRosterStrings = new(0x1F, 0xFA51);
			public static BA lut_CombatDrinkBox = new(0x1F, 0xFA16);
			public static BA ShiftLeft5 = new(0x1F, 0xF898);
			public static BA BattleMenu_DrawMagicNames = new(0x1F, 0xF844);
			public static BA ShiftLeft6 = new(0x1F, 0xF897);
			public static BA lut_CombatItemMagicBox = new(0x1F, 0xFA11);
			public static BA ClearUnformattedCombatBoxBuffer = new(0x1F, 0xF757);
			public static BA lut_CombatBoxes = new(0x1F, 0xF9E9);
			public static BA lua_BattleCommandBoxInfo = new(0x1F, 0xFA1B);
			public static BA GetPointerToRosterString = new(0x1F, 0xF99C);
			public static BA lut_EnemyRosterBox = new(0x1F, 0xF9E4);
			public static BA BattleDraw_AddBlockToBuffer = new(0x1F, 0xF690);
			public static BA UndrawBattleBlock = new(0x1F, 0xF65A);
			public static BA DrawBlockBuffer = new(0x1F, 0xF648);
			public static BA DrawBattleString = new(0x1F, 0xF9AB);
			public static BA DrawBattleBoxAndText = new(0x1F, 0xF608);
			public static BA DrawBattleBox_Exit = new(0x1F, 0xF607);
			public static BA DrawBattleBox_FetchBlock = new(0x1F, 0xF5FB);
			public static BA DrawBattleBox_NextBlock = new(0x1F, 0xF5ED);
			public static BA DrawBattleBox = new(0x1F, 0xF59B);
			public static BA DrawBattleBox_Row = new(0x1F, 0xF572);
			public static BA GetBattleMessagePtr = new(0x1F, 0xF544);
			public static BA BattleDrawMessageBuffer_Reverse = new(0x1F, 0xF4FF);
			public static BA Battle_DrawMessageRow = new(0x1F, 0xF4E8);
			public static BA Battle_DrawMessageRow_VBlank = new(0x1F, 0xF4E5);
			public static BA BattleDrawMessageBuffer = new(0x1F, 0xF4AA);
			public static BA BattleUpdateAudio_FixedBank = new(0x1F, 0xF493);
			public static BA Battle_UpdatePPU_UpdateAudio_FixedBank = new(0x1F, 0xF485);
			public static BA DrawBattleBackdropRow = new(0x1F, 0xF385);
			public static BA LoadBattlePalette = new(0x1F, 0xF471);
			public static BA lut_BtlAttrTbl = new(0x1F, 0xF400);
			public static BA Battle_PlayerBox = new(0x1F, 0xF3C6);
			public static BA BattleBox_vAXY = new(0x1F, 0xF3E2);
			public static BA SetPPUAddr_XA = new(0x1F, 0xF3BF);
			public static BA Battle_PPUOff = new(0x1F, 0xF3F1);
			public static BA SetBattlePPUAddr = new(0x1F, 0xF233);
			public static BA SwapBtlTmpBytes = new(0x1F, 0xFCCF);
			public static BA FormatBattleString = new(0x1F, 0xFA59);
			public static BA BattleScreenShake = new(0x1F, 0xF440);
			public static BA BattleRNG = new(0x1F, 0xFCE7);
			public static BA DrawDrinkBox = new(0x1F, 0xF921);
			public static BA DrawBattleItemBox = new(0x1F, 0xF89E);
			public static BA BattleWaitForVBlank = new(0x1F, 0xF4A1);
			public static BA DrawBattleMagicBox = new(0x1F, 0xF764);
			public static BA DrawCombatBox = new(0x1F, 0xF71C);
			public static BA DrawCommandBox = new(0x1F, 0xF700);
			public static BA DrawRosterBox = new(0x1F, 0xF6C3);
			public static BA UndrawNBattleBlocks = new(0x1F, 0xF6B3);
			public static BA ClearBattleMessageBuffer = new(0x1F, 0xF620);
			public static BA BattleCrossPageJump = new(0x1F, 0xF284);
			public static BA Battle_ReadPPUData = new(0x1F, 0xF268);
			public static BA Battle_WritePPUData = new(0x1F, 0xF23E);
			public static BA EnterBattle = new(0x1F, 0xF28D);
			public static BA lutItemBoxStrPos = new(0x1F, 0xEFCC);
			public static BA ReadjustBBEquipStats = new(0x1F, 0xEEDB);
			public static BA UnadjustBBEquipStats = new(0x1F, 0xEEB7);
			public static BA lut_EquipStringPositions = new(0x1F, 0xED72);
			public static BA lutCursor2x2SpriteTable = new(0x1F, 0xECB0);
			public static BA DrawOBSprite_Exit = new(0x1F, 0xEBFC);
			public static BA Faux_LoadBattleBGPalettes = new(0x1F, 0xEB93);
			public static BA LoadBattleBackdropPalette = new(0x1F, 0xEB7C);
			public static BA LoadBackdropPalette = new(0x1F, 0xEB5A);
			public static BA lut_ShopTypes = new(0x1F, 0xEBB5);
			public static BA LoadBorderPalette_Black = new(0x1F, 0xEB2D);
			public static BA LoadBattleSpritePalettes = new(0x1F, 0xEB99);
			public static BA LoadBorderPalette_Blue = new(0x1F, 0xEB29);
			public static BA LoadBattleBGPalettes = new(0x1F, 0xEB8D);
			public static BA LoadBattleBGCHRPointers = new(0x1F, 0xEA7A);
			public static BA lut_ShopPalettes = new(0x1F, 0xEB74);
			public static BA LoadShopTypeAndPalette = new(0x1F, 0xEB42);
			public static BA CHRLoad = new(0x1F, 0xE965);
			public static BA LoadOWObjectCHR = new(0x1F, 0xE940);
			public static BA LoadOWBGCHR = new(0x1F, 0xE94B);
			public static BA LoadMapObjCHR = new(0x1F, 0xE99E);
			public static BA LoadTilesetAndMenuCHR = new(0x1F, 0xE975);
			public static BA LoadPlayerMapmanCHR = new(0x1F, 0xE92E);
			public static BA LoadShopBGCHRPalettes = new(0x1F, 0xEA02);
			public static BA LoadBatSprCHRPalettes = new(0x1F, 0xEAE1);
			public static BA LoadBattleBGCHRAndPalettes = new(0x1F, 0xEA28);
			public static BA LoadBatSprCHRPalettes_NewGame = new(0x1F, 0xEAB9);
			public static BA LoadMenuBGCHRAndPalettes = new(0x1F, 0xEA9F);
			public static BA LoadMenuCHR = new(0x1F, 0xE98E);
			public static BA CHRLoad_Cont = new(0x1F, 0xE967);
			public static BA CHRLoadToA = new(0x1F, 0xE95A);
			public static BA LoadSingleMapObject = new(0x1F, 0xE83B);
			public static BA lut_2x2MapObj_Right = new(0x1F, 0xE7AB);
			public static BA lut_2x2MapObj_Left = new(0x1F, 0xE7BB);
			public static BA lut_2x2MapObj_Up = new(0x1F, 0xE7CB);
			public static BA lut_2x2MapObj_Down = new(0x1F, 0xE7DB);
			public static BA CanMapObjMove = new(0x1F, 0xE630);
			public static BA DrawMapObject = new(0x1F, 0xE6D8);
			public static BA MapObjectMove = new(0x1F, 0xE50A);
			public static BA AnimateAndDrawMapObject = new(0x1F, 0xE688);
			public static BA UpdateAndDrawMapObjects = new(0x1F, 0xE4C7);
			public static BA ConvertOWToSprite = new(0x1F, 0xE3DF);
			public static BA DrawOWObj_Exit = new(0x1F, 0xE3A5);
			public static BA lut_PlayerMapmanSprTbl = new(0x1F, 0xE427);
			public static BA lut_VehicleSprTbl = new(0x1F, 0xE467);
			public static BA Draw2x2Sprite = new(0x1F, 0xE301);
			public static BA lut_OWObjectSprTbl = new(0x1F, 0xE4A7);
			public static BA Draw2x2Vehicle_Set = new(0x1F, 0xE2E0);
			public static BA DrawMMV_Ship = new(0x1F, 0xE2D5);
			public static BA DrawMMV_Canoe = new(0x1F, 0xE2DC);
			public static BA DrawMMV_OnFoot = new(0x1F, 0xE2F0);
			public static BA lut_VehicleFacingSprTblOffset = new(0x1F, 0xE417);
			public static BA lut_VehicleSprY = new(0x1F, 0xE36A);
			public static BA DrawOWObj_Airship = new(0x1F, 0xE38C);
			public static BA DrawPlayerMapmanSprite = new(0x1F, 0xE281);
			public static BA DrawOWObj_BridgeCanal = new(0x1F, 0xE3A6);
			public static BA DrawOWObj_Ship = new(0x1F, 0xE373);
			public static BA DrawAirshipShadow = new(0x1F, 0xE2B8);
			public static BA Draw2x2Vehicle = new(0x1F, 0xE2DE);
			public static BA AirshipTransitionFrame = new(0x1F, 0xE1E1);
			public static BA DrawBoxRow_Bot = new(0x1F, 0xE0D7);
			public static BA DrawBoxRow_Mid = new(0x1F, 0xE0A5);
			public static BA DrawBoxRow_Top = new(0x1F, 0xE0FC);
			public static BA MenuCondStall = new(0x1F, 0xE12E);
			public static BA DrawComplexString_Exit = new(0x1F, 0xDE29);
			public static BA lut_ArmorSlots = new(0x1F, 0xDD68);
			public static BA FindEmptyArmorSlot = new(0x1F, 0xDD46);
			public static BA lut_WeaponSlots = new(0x1F, 0xDD58);
			public static BA CoordToNTAddr = new(0x1F, 0xDCAB);
			public static BA lut_NTRowStartLo = new(0x1F, 0xDCF4);
			public static BA lut_NTRowStartHi = new(0x1F, 0xDD14);
			public static BA lut_DTE2 = new(0x1F, 0xF050);
			public static BA lut_DTE1 = new(0x1F, 0xF0A0);
			public static BA SetPPUAddrToDest = new(0x1F, 0xDC80);
			public static BA DrawDialogueString_Done = new(0x1F, 0xDB60);
			public static BA DoAltarScanline = new(0x1F, 0xDB0B);
			public static BA WaitAltarScanline = new(0x1F, 0xDB00);
			public static BA AltarFrame = new(0x1F, 0xDADE);
			public static BA PalCyc_Step = new(0x1F, 0xD9EF);
			public static BA PalCyc_GetInitialPal = new(0x1F, 0xD9B3);
			public static BA PalCyc_SetScroll = new(0x1F, 0xD982);
			public static BA PalCyc_DrawPalette = new(0x1F, 0xD918);
			public static BA WaitVBlank_NoSprites = new(0x1F, 0xD89F);
			public static BA _DrawPalette_Norm = new(0x1F, 0xD880);
			public static BA ProcessJoyButtons = new(0x1F, 0xD7E2);
			public static BA ReadJoypadData = new(0x1F, 0xD7C9);
			public static BA ScreenWipeFrame_Prep = new(0x1F, 0xD72F);
			public static BA ScreenWipe_Finalize = new(0x1F, 0xD723);
			public static BA ScreenWipeFrame = new(0x1F, 0xD742);
			public static BA StartScreenWipe = new(0x1F, 0xD79D);
			public static BA WaitScanline = new(0x1F, 0xD788);
			public static BA Dialogue_CoverSprites_VBl = new(0x1F, 0xFF02);
			public static BA DialogueBox_Frame = new(0x1F, 0xD6A1);
			public static BA DialogueBox_Sfx = new(0x1F, 0xD6C7);
			public static BA DlgBoxPrep_DR = new(0x1F, 0xD5D0);
			public static BA DlgBoxPrep_DL = new(0x1F, 0xD5BE);
			public static BA DlgBoxPrep_UR = new(0x1F, 0xD5AC);
			public static BA DlgBoxPrep_UL = new(0x1F, 0xD59A);
			public static BA DrawDialogueString = new(0x1F, 0xDB64);
			public static BA PrepDialogueBoxAttr = new(0x1F, 0xD53E);
			public static BA PrepDialogueBoxRow = new(0x1F, 0xD549);
			public static BA PrepSMRowCol = new(0x1F, 0xD1E4);
			public static BA Map_RTS = new(0x1F, 0xD156);
			public static BA DecompressMap = new(0x1F, 0xD186);
			public static BA LoadOWMapRow = new(0x1F, 0xD157);
			public static BA PrepRowCol = new(0x1F, 0xD258);
			public static BA ScrollUpOneRow = new(0x1F, 0xD102);
			public static BA DrawMapAttributes = new(0x1F, 0xD46F);
			public static BA DrawMapRowCol = new(0x1F, 0xD2E9);
			public static BA LoadSMCHR = new(0x1F, 0xE912);
			public static BA LoadMapObjects = new(0x1F, 0xE7EB);
			public static BA LoadStandardMap = new(0x1F, 0xD126);
			public static BA AssertNasirCRC = new(0x1F, 0xCFCB);
			public static BA PrepStandardMap = new(0x1F, 0xCF55);
			public static BA LoadStandardMapAndObjects = new(0x1F, 0xCF42);
			public static BA RedrawDoor_Exit = new(0x1F, 0xCEBA);
			public static BA lut_2xNTRowStartHi = new(0x1F, 0xD5F2);
			public static BA lut_2xNTRowStartLo = new(0x1F, 0xD5E2);
			public static BA PlayDoorSFX = new(0x1F, 0xCF1E);
			public static BA SMMove_AltarEffect = new(0x1F, 0xCE40);
			public static BA SMMove_OK = new(0x1F, 0xCE4F);
			public static BA SMMove_HaveSpecialItem = new(0x1F, 0xCE08);
			public static BA SMMove_NoSpecialItem = new(0x1F, 0xCDED);
			public static BA SMMove_AirOrb = new(0x1F, 0xCE36);
			public static BA SMMove_WaterOrb = new(0x1F, 0xCE2A);
			public static BA SMMove_FireOrb = new(0x1F, 0xCE1E);
			public static BA SMMove_EarthOrb = new(0x1F, 0xCE12);
			public static BA SMMove_UseLute = new(0x1F, 0xCE10);
			public static BA SMMove_UseRod = new(0x1F, 0xCE0E);
			public static BA SMMove_4Orbs = new(0x1F, 0xCDFA);
			public static BA SMMove_Cube = new(0x1F, 0xCDF3);
			public static BA SMMove_Crown = new(0x1F, 0xCDE6);
			public static BA SMMove_Dmg = new(0x1F, 0xCDE4);
			public static BA SMMove_Battle = new(0x1F, 0xCDC3);
			public static BA SMMove_Treasure = new(0x1F, 0xCDC1);
			public static BA SMMove_CloseRoom = new(0x1F, 0xCE44);
			public static BA SMMove_Door = new(0x1F, 0xCE53);
			public static BA SMMove_Norm = new(0x1F, 0xCE51);
			public static BA SMMove_Up = new(0x1F, 0xCD64);
			public static BA SMMove_Down = new(0x1F, 0xCD29);
			public static BA SMMove_Left = new(0x1F, 0xCCEB);
			public static BA SMMove_Right = new(0x1F, 0xCCBF);
			public static BA SM_MovePlayer = new(0x1F, 0xCD1B);
			public static BA RedrawDoor = new(0x1F, 0xCEBB);
			public static BA Copy256 = new(0x1F, 0xCC74);
			public static BA LoadSMTilesetData = new(0x1F, 0xCC08);
			public static BA OpenTreasureChest = new(0x1F, 0xDD78);
			public static BA lut_SMMoveJmpTbl = new(0x1F, 0xCDA1);
			public static BA GetSMTileProperties = new(0x1F, 0xCBBE);
			public static BA IsObjectInPath = new(0x1F, 0xCAA2);
			public static BA CanPlayerMoveSM = new(0x1F, 0xCA76);
			public static BA ShowDialogueBox = new(0x1F, 0xD602);
			public static BA SetSMScroll = new(0x1F, 0xCCA1);
			public static BA DrawDialogueBox = new(0x1F, 0xD4B1);
			public static BA TalkToSMTile = new(0x1F, 0xCBE2);
			public static BA DrawMapObjectsNoUpdate = new(0x1F, 0xE4F6);
			public static BA CanTalkToMapObject = new(0x1F, 0xCB25);
			public static BA GetSMTargetCoords = new(0x1F, 0xCB61);
			public static BA LoadEpilogueSceneGFX = new(0x1F, 0xE89C);
			public static BA ReenterStandardMap = new(0x1F, 0xCF3A);
			public static BA GetSMTilePropNow = new(0x1F, 0xCB94);
			public static BA DrawSMSprites = new(0x1F, 0xE40F);
			public static BA ProcessSMInput = new(0x1F, 0xC9C4);
			public static BA DoAltarEffect = new(0x1F, 0xDA4E);
			public static BA StandardMapMovement = new(0x1F, 0xCC80);
			public static BA StandardMapLoop = new(0x1F, 0xC8B6);
			public static BA EnterStandardMap = new(0x1F, 0xCF2E);
			public static BA AssignMapTileDamage = new(0x1F, 0xC861);
			public static BA MapTileDamage = new(0x1F, 0xC7DE);
			public static BA DrawMapPalette = new(0x1F, 0xD862);
			public static BA DrawFullMap = new(0x1F, 0xCFE7);
			public static BA LoadMapPalettes = new(0x1F, 0xD8AB);
			public static BA LoadOWCHR = new(0x1F, 0xE920);
			public static BA AnimateAirshipLanding = new(0x1F, 0xE1C2);
			public static BA DockShip = new(0x1F, 0xC632);
			public static BA Board_Fail = new(0x1F, 0xC5E4);
			public static BA lut_FormationWeight = new(0x1F, 0xC58C);
			public static BA GetBattleFormation = new(0x1F, 0xC54A);
			public static BA BattleStepRNG = new(0x1F, 0xC571);
			public static BA OWMove_Up = new(0x1F, 0xC406);
			public static BA OWMove_Down = new(0x1F, 0xC3D2);
			public static BA OWMove_Left = new(0x1F, 0xC396);
			public static BA DoMapDrawJob = new(0x1F, 0xD0E9);
			public static BA OWMove_Right = new(0x1F, 0xC36C);
			public static BA SetOWScroll = new(0x1F, 0xC34B);
			public static BA MapPoisonDamage = new(0x1F, 0xC7FB);
			public static BA OW_MovePlayer = new(0x1F, 0xC3C4);
			public static BA SetOWScroll_PPUOn = new(0x1F, 0xC346);
			public static BA LoadOWTilesetData = new(0x1F, 0xC30F);
			public static BA UnboardBoat = new(0x1F, 0xC5CC);
			public static BA UnboardBoat_Abs = new(0x1F, 0xC5D2);
			public static BA IsOnCanal = new(0x1F, 0xC666);
			public static BA LandAirship = new(0x1F, 0xC6B8);
			public static BA StartMapMove = new(0x1F, 0xD023);
			public static BA BoardShip = new(0x1F, 0xC609);
			public static BA BoardCanoe = new(0x1F, 0xC5E6);
			public static BA IsOnBridge = new(0x1F, 0xC64D);
			public static BA OWCanMove = new(0x1F, 0xC47D);
			public static BA MinigameReward = new(0x1F, 0xC8A4);
			public static BA AnimateAirshipTakeoff = new(0x1F, 0xE1A8);
			public static BA FlyAirship = new(0x1F, 0xC215);
			public static BA DoStandardMap = new(0x1F, 0xC8B3);
			public static BA ScreenWipe_Close = new(0x1F, 0xD701);
			public static BA EnterBattle_L = new(0x1F, 0xF200);
			public static BA LoadBattleCHRPal = new(0x1F, 0xE900);
			public static BA BattleTransition = new(0x1F, 0xD8CD);
			public static BA EnterOW_PalCyc = new(0x1F, 0xC762);
			public static BA LoadBridgeSceneGFX = new(0x1F, 0xE8CB);
			public static BA CyclePalettes = new(0x1F, 0xD946);
			public static BA DrawOWSprites = new(0x1F, 0xE225);
			public static BA ProcessOWInput = new(0x1F, 0xC23C);
			public static BA DoOWTransitions = new(0x1F, 0xC140);
			public static BA PrepAttributePos = new(0x1F, 0xD401);
			public static BA CallMusicPlay_NoSwap = new(0x1F, 0xC681);
			public static BA OverworldMovement = new(0x1F, 0xC335);
			public static BA GetOWTile = new(0x1F, 0xC696);
			public static BA EnterOverworldLoop = new(0x1F, 0xC0D1);
			public static BA ScreenWipe_Open = new(0x1F, 0xD6DC);
			public static BA PrepOverworld = new(0x1F, 0xC6FD);
			public static BA ClearZeroPage = new(0x1F, 0xC454);
			public static BA NewGame_LoadStartingStats = new(0x1F, 0xC76D);
			public static BA VerifyChecksum = new(0x1F, 0xC888);
			public static BA SwapPRG_L = new(0x1F, 0xFE03);
			public static BA DisableAPU = new(0x1F, 0xC469);
			public static BA GameStart = new(0x1F, 0xC012);
			public static BA ClearBattleMessageBuffer_L = new(0x1F, 0xF20C);
			public static BA BattleCrossPageJump_L = new(0x1F, 0xF209);
			public static BA DrawRosterBox_L = new(0x1F, 0xF212);
			public static BA DrawCommandBox_L = new(0x1F, 0xF215);
			public static BA UndrawNBattleBlocks_L = new(0x1F, 0xF20F);
			public static BA DrawDrinkBox_L = new(0x1F, 0xF224);
			public static BA DrawBattleItemBox_L = new(0x1F, 0xF221);
			public static BA DrawCombatBox_L = new(0x1F, 0xF218);
			public static BA CallMinimapDecompress = new(0x1F, 0xFFC0);
			public static BA Battle_ReadPPUData_L = new(0x1F, 0xF206);
			public static BA Battle_WritePPUData_L = new(0x1F, 0xF203);
			public static BA BattleWaitForVBlank_L = new(0x1F, 0xF21E);
			public static BA BattleRNG_L = new(0x1F, 0xF227);
			public static BA DrawBattleMagicBox_L = new(0x1F, 0xF21B);
			public static BA BattleScreenShake_L = new(0x1F, 0xF22A);
			public static BA FormatBattleString_L = new(0x1F, 0xF22D);
			public static BA SwapBtlTmpBytes_L = new(0x1F, 0xF230);
			public static BA lut_RNG = new(0x1F, 0xF100);
			public static BA LoadPrice = new(0x1F, 0xECB9);
			public static BA LoadMenuCHRPal = new(0x1F, 0xE906);
			public static BA DrawBox = new(0x1F, 0xE063);
			public static BA WaitForVBlank_L = new(0x1F, 0xFE00);
			public static BA DrawCursor = new(0x1F, 0xEC95);
			public static BA DrawOBSprite = new(0x1F, 0xEBFD);
			public static BA LoadNewGameCHRPal = new(0x1F, 0xE8FA);
			public static BA lutClassBatSprPalette = new(0x1F, 0xECA4);
			public static BA DrawSimple2x3Sprite = new(0x1F, 0xEC24);
			public static BA LoadShopCHRPal = new(0x1F, 0xE90C);
			public static BA UnadjustEquipStats = new(0x1F, 0xED92);
			public static BA SortEquipmentList = new(0x1F, 0xEFFC);
			public static BA ReadjustEquipStats = new(0x1F, 0xEE1B);
			public static BA EraseBox = new(0x1F, 0xE146);
			public static BA FadeOutBatSprPalettes = new(0x1F, 0xFF90);
			public static BA FadeInBatSprPalettes = new(0x1F, 0xFFA8);
			public static BA DrawItemBox = new(0x1F, 0xEF18);
			public static BA DrawEquipMenuStrings = new(0x1F, 0xECDA);
			public static BA DrawComplexString_L = new(0x1F, 0xC003);
			public static BA CallMusicPlay_L = new(0x1F, 0xC009);
			public static BA DrawPalette_L = new(0x1F, 0xC00F);
			public static BA UpdateJoy_L = new(0x1F, 0xC00C);
			public static BA DrawBox_L = new(0x1F, 0xC006);
			public static BA UpdateJoy = new(0x1F, 0xD7C2);
			public static BA CallMusicPlay = new(0x1F, 0xC689);
			public static BA FindEmptyWeaponSlot = new(0x1F, 0xDD34);
			public static BA DrawPalette = new(0x1F, 0xD850);
			public static BA ClearOAM = new(0x1F, 0xC43C);
			public static BA DrawComplexString = new(0x1F, 0xDE36);
			public static BA AddGPToParty = new(0x1F, 0xDDEA);
			public static BA DrawImageRect = new(0x1F, 0xDCBC);
			public static BA PlaySFX_Error = new(0x1F, 0xDB26);
			public static BA DoOverworld = new(0x1F, 0xC0CB);
			public static BA GameStart_L = new(0x1F, 0xC000);
			// Don't add any fields to this class unless they are symbols. It will break AsDictionaries/AsLists.
		}

		public static class Constants
		{
			// some of these probably don't need to be here...
			// Don't add any fields to this class unless they are symbols. It will break AsDictionaries/AsLists.

			// ----------------
			//  directions for facing and keys

			public static int RIGHT = 0x01;
			public static int LEFT = 0x02;
			public static int DOWN = 0x04;
			public static int UP = 0x08;
			public static int BTN_START = 0x10;

			// ----------------
			//  music channels

			public static int CHAN_START = 0xB0;
			public static int CHAN_BYTES = 0x10;

			public static int CHAN_SQ1 = CHAN_START;
			public static int CHAN_SQ2 = CHAN_START + CHAN_BYTES;
			public static int CHAN_TRI = CHAN_START + (2 * CHAN_BYTES);

			public static int CHAN_STOP = CHAN_START + (3 * CHAN_BYTES);

			// ----------------
			//  enemy categories
			public static int CATEGORY_UNKNOWN = 0x01;
			public static int CATEGORY_DRAGON = 0x02;
			public static int CATEGORY_GIANT = 0x04;
			public static int CATEGORY_UNDEAD = 0x08;
			public static int CATEGORY_WERE = 0x10;
			public static int CATEGORY_WATER = 0x20;
			public static int CATEGORY_MAGE = 0x40;
			public static int CATEGORY_REGEN = 0x80;

			// ----------------
			//  enemy stats in ROM
			public static int ENROMSTAT_EXP = 0x00;         //  2 bytes
			public static int ENROMSTAT_GP = 0x02;         //  2 bytes
			public static int ENROMSTAT_HPMAX = 0x04;         //  2 bytes
			public static int ENROMSTAT_MORALE = 0x06;
			public static int ENROMSTAT_AI = 0x07;
			public static int ENROMSTAT_EVADE = 0x08;
			public static int ENROMSTAT_ABSORB = 0x09;
			public static int ENROMSTAT_NUMHITS = 0x0A;
			public static int ENROMSTAT_HITRATE = 0x0B;
			public static int ENROMSTAT_DAMAGE = 0x0C;
			public static int ENROMSTAT_CRITRATE = 0x0D;
			public static int ENROMSTAT_UNKNOWN_E = 0x0E;
			public static int ENROMSTAT_ATTACKAIL = 0x0F;
			public static int ENROMSTAT_CATEGORY = 0x10;
			public static int ENROMSTAT_MAGDEF = 0x11;
			public static int ENROMSTAT_ELEMWEAK = 0x12;
			public static int ENROMSTAT_ELEMRESIST = 0x13;

			// ----------------
			//  magic data in ROM
			public static int MAGDATA_HITRATE = 0x00;
			public static int MAGDATA_EFFECTIVITY = 0x01;
			public static int MAGDATA_ELEMENT = 0x02;
			public static int MAGDATA_TARGET = 0x03;         //  (01 = All enemies, 02 = One Enemy, 04 = Spell Caster, 08 = Whole Party, 10 = One party member)
			public static int MAGDATA_EFFECT = 0x04;
			public static int MAGDATA_GRAPHIC = 0x05;
			public static int MAGDATA_PALETTE = 0x06;
			public static int MAGDATA_UNUSED = 0x07;


			// ----------------
			//  banks to swap to for different things
			//

			public static int BANK_TALKTOOBJ = 0x0E;
			public static int BANK_MENUS = 0x0E;
			public static int BANK_TITLE = 0x0E;
			public static int BANK_INTRO = 0x0E;
			public static int BANK_PARTYGEN = 0x0E;
			public static int BANK_MUSIC = 0x0D;
			public static int BANK_EQUIPSTATS = 0x0C;
			public static int BANK_BTLPALETTES = 0x0C;
			public static int BANK_BATTLE = 0x0B;
			public static int BANK_BTLDATA = 0x0B;
			public static int BANK_DOMAINS = 0x0B;
			public static int BANK_ENEMYNAMES = 0x0B;
			public static int BANK_BTLMESSAGES = 0x0B;
			public static int BANK_MINIMAP = 0x09;
			public static int BANK_MENUCHR = 0x09;
			public static int BANK_BATTLECHR = 0x07;
			public static int BANK_STANDARDMAPS = 0x04;         //  used with ORA, so low 2 bits of bank number must be clear
			public static int BANK_TILESETCHR = 0x03;
			public static int BANK_MAPCHR = 0x02;
			public static int BANK_OWMAP = 0x01;
			public static int BANK_OBJINFO = 0x00;
			public static int BANK_MAPMANPAL = 0x00;
			public static int BANK_OWINFO = 0x00;
			public static int BANK_SMINFO = 0x00;
			public static int BANK_TREASURE = 0x00;
			public static int BANK_TELEPORTINFO = 0x00;
			public static int BANK_STARTUPINFO = 0x00;
			public static int BANK_STARTINGSTATS = 0x00;

			public static int BANK_ORBCHR = 0x0D;
			public static int BANK_BTLCHR = 0x09;
			public static int BANK_BACKDROPPAL = 0x00;

			public static int BANK_ITEMPRICES = 0x0D;
			public static int BANK_MINIGAME = 0x0D;
			public static int BANK_BRIDGESCENE = 0x0D;
			public static int BANK_ENDINGSCENE = 0x0D;
			public static int BANK_INTROTEXT = 0x0D;

			public static int BANK_BRIDGEGFX = 0x0B;
			public static int BANK_EPILOGUEGFX = 0x0B;

			public static int BANK_DIALOGUE = 0x0A;
			public static int BANK_ITEMS = BANK_DIALOGUE; //  must be shared

			// ----------------
			//  Special Standard Map tile IDs
			//

			public static int MAPTILE_CLOSEDDOOR = 0x36;         //  tiles used for door graphics (for runtime changes to the map -- when you 
			public static int MAPTILE_OPENDOOR = 0x37;         //    enter/exit rooms)
			public static int MAPTILE_LOCKEDDOOR = 0x3B;

			// ----------------
			//  game flag bits

			public static int GMFLG_OBJVISIBLE = 0b00000001;   //  must be low bit (often shifted out)
			public static int GMFLG_EVENT = 0b00000010;   //  must be bit 1 (shifted out)
			public static int GMFLG_TCOPEN = 0b00000100;


			// ----------------
			//  battle formation IDs

			public static int BTL_VAMPIRE = 0x7C;
			public static int BTL_ASTOS = 0x7D;
			public static int BTL_BIKKE = 0x7E;
			public static int BTL_GARLAND = 0x7F;

			// ----------------
			//  battle message IDs  (1-based)

			public static int BTLMSG_HPUP = 0x01;
			public static int BTLMSG_ASLEEP = 0x04;

			public static int BTLMSG_SIGHTRECOVERED = 0x06;        //  is this ever used?
			public static int BTLMSG_SILENCED = 0x07;

			public static int BTLMSG_DARKNESS = 0x09;

			public static int BTLMSG_NEUTRALIZED = 0x0E;

			public static int BTLMSG_BREAKSILENCE = 0x11;         //  is this ever used??

			public static int BTLMSG_CONFUSED = 0x13;
			public static int BTLMSG_POISONED = 0x14;

			public static int BTLMSG_PARALYZED_A = 0x17;

			public static int BTLMSG_BROKENTOPIECES = 0x1A;

			public static int BTLMSG_SLAIN = 0x20;
			public static int BTLMSG_INEFFECTIVE = 0x21;
			public static int BTLMSG_STRIKEFIRST = 0x22;
			public static int BTLMSG_SURPRISED = 0x23;
			public static int BTLMSG_CANTRUN = 0x24;
			public static int BTLMSG_RUNAWAY = 0x25;
			public static int BTLMSG_CLOSECALL = 0x26;
			public static int BTLMSG_WOKEUP = 0x27;
			public static int BTLMSG_SLEEPING = 0x28;
			public static int BTLMSG_CURED = 0x29;
			public static int BTLMSG_PARALYZED_B = 0x2A;
			public static int BTLMSG_HITS = 0x2B;
			public static int BTLMSG_CRITICALHIT = 0x2C;

			public static int BTLMSG_DMG = 0x2E;
			public static int BTLMSG_STOPPED = 0x2F;

			public static int BTLMSG_STR = 0x33;
			public static int BTLMSG_AGI = 0x34;
			public static int BTLMSG_INT = 0x35;
			public static int BTLMSG_VIT = 0x36;
			public static int BTLMSG_LUCK = 0x37;
			public static int BTLMSG_UP = 0x38;

			public static int BTLMSG_TERMINATED = 0x3F;
			public static int BTLMSG_MISSED = 0x40;

			public static int BTLMSG_INEFFECTIVENOW = 0x4A;

			public static int BTLMSG_NOTHINGHAPPENS = 0x4E;

			// ----------------
			//  Alternative battle message IDs
			//    For whatever reason, the game has a routine where it runs an ID through a LUT
			//  to get a different ID.  Don't ask me why.
			//
			//  These are zero-based and are used exclusively with ShowAltBattleMessage in bank C.
			//  See that routine for more info.
			public static int ALTBTLMSG_RUNAWAY = 0x00;
			public static int ALTBTLMSG_PARALYZED_B = 0x01;
			public static int ALTBTLMSG_SLEEPING = 0x02;
			public static int ALTBTLMSG_SILENCED_1 = 0x03;
			public static int ALTBTLMSG_INEFFECTIVE = 0x04;
			public static int ALTBTLMSG_CONFUSED = 0x05;         //  ALTBTLMSG_CONFUSED through ALTBTLMSG_BROKENTOPIECES
			public static int ALTBTLMSG_SILENCED_2 = 0x06;         //    must be in sequential order, as they are used programmatically.
			public static int ALTBTLMSG_ASLEEP = 0x07;         //    They are the messages printed when an ailment is inflicted.
			public static int ALTBTLMSG_PARALYZED_A = 0x08;
			public static int ALTBTLMSG_DARKNESS = 0x09;
			public static int ALTBTLMSG_POISONED = 0x0A;
			public static int ALTBTLMSG_BROKENTOPIECES = 0x0B;
			public static int ALTBTLMSG_TERMINATED = 0x0C;
			public static int ALTBTLMSG_CURED_1 = 0x0D;         //  Same deal as ALTBTLMSG_CONFUSED -- these are printed when
			public static int ALTBTLMSG_BREAKSILENCE = 0x0E;       //    their matching ailment is cured
			public static int ALTBTLMSG_WOKEUP = 0x0F;
			public static int ALTBTLMSG_CURED_2 = 0x10;
			public static int ALTBTLMSG_SIGHTRECOVERED = 0x11;
			public static int ALTBTLMSG_NEUTRALIZED = 0x12;
			public static int ALTBTLMSG_INEFFECTIVENOW = 0x13;     //  <- message for curing Stone - you cannot cure stone in battle
			public static int ALTBTLMSG_SLAIN = 0x14;
			public static int ALTBTLMSG_NOTHINGHAPPENS = 0x15;

			// ----------------
			//  normal teleport IDs

			public static int NORMTELE_SAVEDPRINCESS = 0x3F;

			// ----------------
			//  misc crap

			public static int WPNID_XCALBUR = 0x27;

			// ----------------
			//  ailments
			public static int AIL_DEAD = 0x01;
			public static int AIL_STONE = 0x02;
			public static int AIL_POISON = 0x04;
			public static int AIL_DARK = 0x08;
			public static int AIL_STUN = 0x10;
			public static int AIL_SLEEP = 0x20;
			public static int AIL_MUTE = 0x40;
			public static int AIL_CONF = 0x80;

			// ----------------
			//  map object IDs
			//

			public static int OBJID_GARLAND = 0x02;         //  Garland (the first one, not ToFR)
			public static int OBJID_PRINCESS_1 = 0x03;         //  kidnapped princess (in ToF)
			public static int OBJID_BIKKE = 0x04;         //  Bikke the Pirate
			public static int OBJID_ELFPRINCE = 0x06;         //  Elf Prince (sleeping man-beauty)
			public static int OBJID_ASTOS = 0x07;         //  Astos -- the dark king!  omg scarey
			public static int OBJID_NERRICK = 0x08;         //  Nerrick -- the dwarf working on the canal
			public static int OBJID_SMITH = 0x09;         //  Smith, the dwarven blacksmith (no, he's not Watts)
			public static int OBJID_MATOYA = 0x0A;
			public static int OBJID_UNNE = 0x0B;         //  you've never heard of him?
			public static int OBJID_VAMPIRE = 0x0C;         //  Earth Cave's Vampire
			public static int OBJID_SARDA = 0x0D;
			public static int OBJID_BAHAMUT = 0x0E;         //  Bahamut
			public static int OBJID_SUBENGINEER = 0x10;         //  Submarine Engineer (blocking Sea Shrine in Onrac)
			public static int OBJID_PRINCESS_2 = 0x12;         //  rescued princess (in Coneria Castle)
			public static int OBJID_FAIRY = 0x13;         //  fairy that appears from the bottle
			public static int OBJID_TITAN = 0x14;         //  Titan in Titan's Tunnel
			public static int OBJID_RODPLATE = 0x16;         //  plate that is removed with the Rod
			public static int OBJID_LUTEPLATE = 0x17;         //  plate that is removed with the Lute

			public static int OBJID_SKYWAR_FIRST = 0x3A;         //  start of the 5 sky warriors
			public static int OBJID_SKYWAR_LAST = OBJID_SKYWAR_FIRST + 4; //  last of the 5 sky warriors

			public static int OBJID_PIRATETERR_1 = 0x3F;         //  townspeople that were terrorized by the
			public static int OBJID_PIRATETERR_2 = 0x40;         //    pirates... they don't become visible until after
			public static int OBJID_PIRATETERR_3 = 0x41;         //    you beat Bikke and claim the ship

			public static int OBJID_BAT = 0x57;         //  normal bat

			public static int OBJID_BLACKORB = 0xCA;

			// ----------------
			//  common dialogue IDs
			//

			public static int DLGID_NOTHING = 0x00;         //  "Nothing Here"
			public static int DLGID_DONTBEGREEDY = 0x3A;         //  from Smith if you have too many weapons
			public static int DLGID_TCGET = 0xF0;         //  "In this chest you find..."
			public static int DLGID_CANTCARRY = 0xF1;         //  "You can't carry anymore"
			public static int DLGID_EMPTYTC = 0xF2;         //  "this treasure chest is empty"

			// ----------------
			//  treasure item type ranges
			public static int TCITYPE_ITEMSTART = 0x00;
			public static int TCITYPE_WEPSTART = TCITYPE_ITEMSTART + 0x1C;
			public static int TCITYPE_ARMSTART = TCITYPE_WEPSTART + 0x28;
			public static int TCITYPE_GPSTART = TCITYPE_ARMSTART + 0x28;


			// ----------------
			//  standard map tile properties

			public static int TP_SPEC_DOOR = 0b00000010;
			public static int TP_SPEC_LOCKED = 0b00000100;
			public static int TP_SPEC_CLOSEROOM = 0b00000110;
			public static int TP_SPEC_TREASURE = 0b00001000;
			public static int TP_SPEC_BATTLE = 0b00001010;
			public static int TP_SPEC_DAMAGE = 0b00001100;
			public static int TP_SPEC_CROWN = 0b00001110;
			public static int TP_SPEC_CUBE = 0b00010000;
			public static int TP_SPEC_4ORBS = 0b00010010;
			public static int TP_SPEC_USEROD = 0b00010100;
			public static int TP_SPEC_USELUTE = 0b00010110;
			public static int TP_SPEC_EARTHORB = 0b00011000;
			public static int TP_SPEC_FIREORB = 0b00011010;
			public static int TP_SPEC_WATERORB = 0b00011100;
			public static int TP_SPEC_AIRORB = 0b00011110;

			public static int TP_SPEC_MASK = 0b00011110;


			public static int TP_TELE_EXIT = 0b11000000;   //  "exit" teleport (standard map to overworld map)
			public static int TP_TELE_NORM = 0b10000000;   //  "normal" teleport (standard map to standard map)
			public static int TP_TELE_WARP = 0b01000000;   //  "warp" teleport (go back to previous floor)
			public static int TP_TELE_NONE = 0;

			public static int TP_TELE_MASK = 0b11000000;

			public static int TP_NOTEXT_MASK = 0b11000010;   //  if any of these bits set, "Nothing Here" is forced when you talk to tile

			public static int TP_BATTLEMARKER = 0b00100000;
			public static int TP_NOMOVE = 0b00000001;

			// ----------------
			//  overworld map tile properties

			public static int OWTP_DOCKSHIP = 0b00100000;
			public static int OWTP_FOREST = 0b00010000;

			public static int OWTP_SPEC_CHIME = 0b01000000;
			public static int OWTP_SPEC_CARAVAN = 0b10000000;
			public static int OWTP_SPEC_FLOATER = 0b11000000;

			public static int OWTP_SPEC_MASK = 0b11000000;

			// ----------------
			//  "Poses" for the characters in battle
			public static int CHARPOSE_STAND = 0x00;
			public static int CHARPOSE_WALK = 0x04;
			public static int CHARPOSE_ATTACK_B = 0x08;
			public static int CHARPOSE_ATTACK_F = 0x0C;
			public static int CHARPOSE_CHEER = 0x10;
			public static int CHARPOSE_CHEER_2 = 0x14;
			public static int CHARPOSE_CROUCH = 0x18;
			public static int CHARPOSE_CROUCH_2 = 0x1C;


			// ----------------
			//  classes

			public static int CLS_FT = 0x00;
			public static int CLS_TH = 0x01;
			public static int CLS_BB = 0x02;
			public static int CLS_RM = 0x03;
			public static int CLS_WM = 0x04;
			public static int CLS_BM = 0x05;
			public static int CLS_KN = 0x06;
			public static int CLS_NJ = 0x07;
			public static int CLS_MA = 0x08;
			public static int CLS_RW = 0x09;
			public static int CLS_WW = 0x0A;
			public static int CLS_BW = 0x0B;




			// ----------------
			//  magic spells

			public static int MG_START = 0xB0;
			public static int MG_CURE = MG_START + 0x00;
			public static int MG_HARM = MG_START + 0x01;
			public static int MG_FOG = MG_START + 0x02;
			public static int MG_RUSE = MG_START + 0x03;
			public static int MG_FIRE = MG_START + 0x04;
			public static int MG_SLEP = MG_START + 0x05;
			public static int MG_LOCK = MG_START + 0x06;
			public static int MG_LIT = MG_START + 0x07;
			public static int MG_LAMP = MG_START + 0x08;
			public static int MG_MUTE = MG_START + 0x09;
			public static int MG_ALIT = MG_START + 0x0A;
			public static int MG_INVS = MG_START + 0x0B;
			public static int MG_ICE = MG_START + 0x0C;
			public static int MG_DARK = MG_START + 0x0D;
			public static int MG_TMPR = MG_START + 0x0E;
			public static int MG_SLOW = MG_START + 0x0F;
			public static int MG_CUR2 = MG_START + 0x10;
			public static int MG_HRM2 = MG_START + 0x11;
			public static int MG_AFIR = MG_START + 0x12;
			public static int MG_HEAL = MG_START + 0x13;
			public static int MG_FIR2 = MG_START + 0x14;
			public static int MG_HOLD = MG_START + 0x15;
			public static int MG_LIT2 = MG_START + 0x16;
			public static int MG_LOK2 = MG_START + 0x17;
			public static int MG_PURE = MG_START + 0x18;
			public static int MG_FEAR = MG_START + 0x19;
			public static int MG_AICE = MG_START + 0x1A;
			public static int MG_AMUT = MG_START + 0x1B;
			public static int MG_SLP2 = MG_START + 0x1C;
			public static int MG_FAST = MG_START + 0x1D;
			public static int MG_CONF = MG_START + 0x1E;
			public static int MG_ICE2 = MG_START + 0x1F;
			public static int MG_CUR3 = MG_START + 0x20;
			public static int MG_LIFE = MG_START + 0x21;
			public static int MG_HRM3 = MG_START + 0x22;
			public static int MG_HEL2 = MG_START + 0x23;
			public static int MG_FIR3 = MG_START + 0x24;
			public static int MG_BANE = MG_START + 0x25;
			public static int MG_WARP = MG_START + 0x26;
			public static int MG_SLO2 = MG_START + 0x27;
			public static int MG_SOFT = MG_START + 0x28;
			public static int MG_EXIT = MG_START + 0x29;
			public static int MG_FOG2 = MG_START + 0x2A;
			public static int MG_INV2 = MG_START + 0x2B;
			public static int MG_LIT3 = MG_START + 0x2C;
			public static int MG_RUB = MG_START + 0x2D;
			public static int MG_QAKE = MG_START + 0x2E;
			public static int MG_STUN = MG_START + 0x2F;
			public static int MG_CUR4 = MG_START + 0x30;
			public static int MG_HRM4 = MG_START + 0x31;
			public static int MG_ARUB = MG_START + 0x32;
			public static int MG_HEL3 = MG_START + 0x33;
			public static int MG_ICE3 = MG_START + 0x34;
			public static int MG_BRAK = MG_START + 0x35;
			public static int MG_SABR = MG_START + 0x36;
			public static int MG_BLND = MG_START + 0x37;
			public static int MG_LIF2 = MG_START + 0x38;
			public static int MG_FADE = MG_START + 0x39;
			public static int MG_WALL = MG_START + 0x3A;
			public static int MG_XFER = MG_START + 0x3B;
			public static int MG_NUKE = MG_START + 0x3C;
			public static int MG_STOP = MG_START + 0x3D;
			public static int MG_ZAP = MG_START + 0x3E;
			public static int MG_XXXX = MG_START + 0x3F;


			// labels that couldn't be inserted in data

			public static int lut_MapObjects = 0xB400;       //  BANK_OBJINFO -- must be on page
			public static int lut_MapObjGfx = 0xAE00;       //  BANK_OBJINFO
			public static int lut_MapObjCHR = 0xA200;       //  BANK_MAPCHR

			public static int lut_MapmanPalettes = 0x83A0;       //  BANK_MAPMANPAL
			public static int lut_OWPtrTbl = 0x8000;       //  BANK_OWMAP
			public static int lut_SMPtrTbl = 0x8000;       //  BANK_STANDARDMAPS
			public static int lut_EnemyAttack = 0xB600;       //  BANK_ITEMS
			public static int lut_ItemNamePtrTbl = 0xB700;       //  BANK_ITEMS
			public static int lut_Domains = 0x8000;       //  BANK_DOMAINS -- MUST be on page boundary

			public static int lut_ShopCHR = 0x8000;       //  BANK_MENUCHR

			public static int lut_BtlBackdrops = 0xB300;       //  BANK_OWINFO

			public static int lut_OrbCHR = 0xB600;       //  BANK_ORBCHR
			public static int lut_BatSprCHR = 0x9000;       //  BANK_BTLCHR -- page
			public static int lut_BatObjCHR = 0xA800;       //  BANK_BTLCHR -- page

			public static int lut_BackdropPal = 0xB200;       //  BANK_BACKDROPPAL

			public static int lut_ItemPrices = 0xBC00;       //  BANK_ITEMPRICES - page

			public static int lut_Weapons = 0x8000;       //  BANK_EQUIPSTATS - page
			public static int lut_Armor = 0x8140;       //  BANK_EQUIPSTATS
			public static int lut_OWTileset = 0x8000;       //  BANK_OWINFO - page

			public static int lut_DialoguePtrTbl = 0x8000;       //  BANK_DIALOGUE

			public static int lut_SMTilesetAttr = 0x8400;       //  BANK_SMINFO - must be on $400 byte bound
			public static int lut_SMTilesetProp = 0x8800;       //  BANK_SMINFO - page
			public static int lut_SMTilesetTSA = 0x9000;       //  BANK_SMINFO - page
			public static int lut_SMPalettes = 0xA000;       //  BANK_SMINFO - $1000 byte bound

			public static int lut_Treasure = 0xB100;       //  BANK_TREASURE

			public static int lut_Tilesets = 0xACC0;       //  BANK_TELEPORTINFO
			public static int lut_NormTele_X = 0xAD00;       //  BANK_TELEPORTINFO
			public static int lut_NormTele_Y = 0xAD40;       //  BANK_TELEPORTINFO
			public static int lut_NormTele_Map = 0xAD80;       //  BANK_TELEPORTINFO
			public static int lut_ExitTele_X = 0xAC60;       //  BANK_TELEPORTINFO
			public static int lut_ExitTele_Y = 0xAC70;       //  BANK_TELEPORTINFO
			public static int lut_EntrTele_X = 0xAC00;       //  BANK_TELEPORTINFO
			public static int lut_EntrTele_Y = 0xAC20;       //  BANK_TELEPORTINFO
			public static int lut_EntrTele_Map = 0xAC40;       //  BANK_TELEPORTINFO

			public static int lut_InitGameFlags = 0xAF00;       //  BANK_STARTUPINFO
			public static int lut_InitUnsramFirstPage = 0xB000;    //  BANK_STARTUPINFO


			public static int lut_ClassStartingStats = 0xB040;     //  BANK_STARTINGSTATS
																   // Don't add any fields to this class unless they are symbols. It will break AsDictionaries/AsLists.
		}

		public static class Variables
		{
			// Don't add any fields to this class unless they are symbols. It will break AsDictionaries/AsLists.
			public static int story_dropinput = 0x07;
			public static int inroom = 0x0D;         //  bit 7 is the actual inroom flag.  $x1=entering room, $x2=entering locked room (different sprite vis), $x5=exiting room, $x6=exiting locked room
			public static int doorppuaddr = 0x0E;         //  2 bytes, PPU address of door drawing work

			public static int tmp = 0x10;         //  16 bytes

			public static int mu_scoreptr = tmp + 8;        // 2 bytes, shared tmp

			public static int dlgbox_row = tmp + 0xC;      //  shared tmp
			public static int palcyc_mode = tmp + 0xC;      //  shared tmp

			public static int joy = 0x20;
			public static int joy_ignore = 0x21;
			public static int joy_select = 0x22;
			public static int joy_start = 0x23;
			public static int joy_a = 0x24;
			public static int joy_b = 0x25;

			public static int sprindex = 0x26;

			public static int ow_scroll_x = 0x27;         //  X scroll of OW in tiles
			public static int ow_scroll_y = 0x28;         //  Y scroll in tiles

			public static int sm_scroll_x = 0x29;         //  ditto, but for standard maps
			public static int sm_scroll_y = 0x2A;

			public static int mapdraw_x = 0x2B;
			public static int mapdraw_y = 0x2C;
			public static int mapflags = 0x2D;         //  bit 0 set when in standard map.  bit 1 set to indicate column drawing instead of row drawing

			public static int scroll_y = 0x2F;         //  Y scroll in tiles (16x16).  range=0-E

			public static int mapdraw_nty = 0x30;
			public static int mapdraw_ntx = 0x31;
			public static int mapdraw_job = 0x32;         //  0=no job, 1=draw attribs, 2=draw tiles

			public static int mg_slidedir = 0x33;         //  shared
			public static int facing = 0x33;         //  1=R  2=L  4=D  8=U
			public static int move_speed = 0x34;         //  pixels to move per frame (map)
			public static int move_ctr_x = 0x35;         //  pixels between tiles (map movement -- 00-0F)
			public static int move_ctr_y = 0x36;         //  ditto but for Y axis

			public static int menustall = 0x37;         //  see MenuCondStall in bank F for explanation

			public static int theend_x = 0x38;
			public static int theend_y = 0x39;
			public static int theend_src = 0x62;
			public static int theend_drawbuf = 0x6800;       //  $700 bytes!

			public static int box_x = 0x38;
			public static int box_y = 0x39;
			public static int box_wd = 0x3C;         //  shared
			public static int box_ht = 0x3D;         //  shared

			public static int dest_x = 0x3A;
			public static int dest_y = 0x3B;
			public static int dest_wd = 0x3C;
			public static int dest_ht = 0x3D;


			public static int image_ptr = 0x3E;         //  shared
			public static int text_ptr = 0x3E;         //  2 bytes

			public static int spr_x = 0x40;
			public static int spr_y = 0x41;

			public static int mm_maprow = 0x41;         //  shared

			public static int vehicle = 0x42;         // 1=walking, 2=canoe, 4=ship, 8=airship

			public static int inforest = 0x43;         //  nonzero if in forest

			public static int tileprop = 0x44;         //  2 bytes

			public static int vehicle_next = 0x46;         //  vehicle we're walking onto

			public static int vehchgpause = 0x47;         //  forced pause when changing vehicles
			public static int cur_map = 0x48;
			public static int cur_tileset = 0x49;

			public static int cur_mapobj = 0x4A;         //  counter for updating which map object

			public static int music_track = 0x4B;
			public static int mu_chanprimer = 0x4C;
			public static int mu_chan = 0x4D;

			public static int entering_shop = 0x50;         //  nonzero = about to enter shop
			public static int shop_id = 0x51;

			public static int tileprop_now = 0x52;         //  special tile properties that we're on (tileprop isn't necessarily what we're standing on)

			public static int ow_tile = 0x53;

			public static int ppu_dest = 0x54;         //  2 bytes

			public static int dlgflg_reentermap = 0x56;         //  flag to indicate the map needs re-entering due to dialogue (Bahamut/class change)
			public static int cur_bank = 0x57;
			public static int ret_bank = 0x58;

			public static int format_buf = 0x60;         //  7 bytes (5A-60) -- must not cross page bound

			public static int shutter_a = 0x61;         //  shared
			public static int shutter_b = 0x62;         //  shared

			public static int lu_cursor = 0x61;         //  shared
			public static int lu_cursor2 = 0x62;         //  shared
			public static int lu_mode = 0x63;         //  shared
			public static int lu_joyprev = 0x64;         //  shared

			public static int mm_bplo = 0x61;         //  shared
			public static int mm_bphi = 0x62;         //  shared

			public static int intro_ataddr = 0x62;         //  shared
			public static int intro_atbyte = 0x63;         //  shared
			public static int intro_color = 0x64;         //  shared

			public static int dlg_itemid = 0x61;         //  shared
			public static int equipmenu_tmp = 0x61;         //  shared
			public static int joy_prevdir = 0x61;
			public static int cursor = 0x62;
			public static int cursor_max = 0x63;
			public static int cursor2 = 0x63;         //  shared (secondary cursor)

			public static int mg_slidespr = 0x64;         //  shared, 3 bytes

			public static int namecurs_x = 0x64;
			public static int shopcurs_x = 0x64;         //  shared
			public static int eq_modecurs = 0x64;         //  shared
			public static int hp_recovery = 0x64;
			public static int mp_required = 0x65;
			public static int namecurs_y = 0x65;
			public static int shopcurs_y = 0x65;         //  shared
			public static int story_credits = 0x65;         //  shared

			public static int minimap_ptr = 0x64;         //  shared, 2 bytes

			public static int submenu_targ = 0x66;         //  shared with shop_type
			public static int shop_type = 0x66;
			public static int story_page = 0x66;         //  shared
			public static int equipoffset = shop_type;    //  MUST be shared with shop_type

			public static int story_timer = 0x67;         //  shared
			public static int draweq_stradd = 0x67;         //  shared
			public static int char_index = 0x67;
			public static int mm_pixrow = 0x67;         //  shared
			public static int talkobj = 0x67;         //  shared -- object you're talking to on SM

			public static int sm_player_x = 0x68;         //  player X/Y position on standard map.  Only used for NPC collision detection
			public static int sm_player_y = 0x69;

			public static int btlformation = 0x6A;
			public static int enCHRpage = 0x6B;

			public static int altareffect = 0x6C;         //  flag to indicate altar effect is to occur (screen shaking, monochrome diagonal window thing)

			public static int dlgmusic_backup = 0x7C;         //  backup music track for restoring music after the dialogue box changes it
			public static int dlgsfx = 0x7D;         //  flag to indicate to play a sound effect after opening dialogue box.  0=no sfx, 1=fanfare, else=treasure

			public static int sq2_sfx = 0x7E;
			public static int descboxopen = 0x7F;

			public static int btlptr = 0x80;         //  ??? don't know how big

			public static int lvlup_curexp = 0x80;         //  2 byte pointer to character's current EXP
			public static int lvlup_exptoadv = 0x82;         //  2 byte pointer to EXP needed to advance
			public static int lvlup_chmagic = 0x84;         //  2 byte pointer to character's magic data
			public static int lvlup_chstats = 0x86;         //  2 byte pointer to character's OB stats


			public static int battlereward = 0x88;         //  3 bytes.  Note that while this var is 3 bytes, this stop behaving properly
														   //  if rewards ever exceed the 2-byte boundary, since the game assumes you
														   //  will never receive more than 65535 XP/GP in any one battle.

			public static int btl_ib_charstat_ptr = 0x80;
			public static int btl_ob_charstat_ptr = 0x82;

			public static int btldraw_blockptrstart = 0x8C;
			public static int btldraw_blockptrend = 0x8E;

			public static int btltmp = 0x90;         //  16 bytes ?

			public static int btl_entityptr_ibram = 0x90;
			public static int btl_entityptr_obrom = 0x92;
			public static int btl_magdataptr = 0x98;

			public static int btldraw_src = 0x90;         //  source data
			public static int btldraw_dst = 0x92;         //  destination pointer
			public static int btldraw_subsrc = 0x94;         //  source pointer of substring
			public static int btldraw_max = 0x9F;         //  maximum characters to draw


			public static int btlsfx_frontseat = 0x90;         //  where battle sfx data is stored in zero page
			public static int btlsfx_backseat = 0x6D97;       //  where it is stored when not in zero page (it swaps between the two)

			public static int btlsfx_framectr = 0x94;         //  The total frame counter for the entire sound effect
			public static int btlsfxsq2_len = 0x98;
			public static int btlsfxnse_len = 0x99;
			public static int btlsfxsq2_framectr = 0x9A;
			public static int btlsfxnse_framectr = 0x9B;
			public static int btlsfxsq2_ptr = 0x9C;
			public static int btlsfxnse_ptr = 0x9E;


			// for sound channels (between Bx-Dx)
			//  see Constants.inc
			public static int ch_scoreptr = 0x0;
			public static int ch_envptr = 0x2;          //  ptr to start of env data
			public static int ch_envpos = 0x4;          //  pos in env data (00-1F)
			public static int ch_lenctr = 0x5;          //  counter for score data (note length)
			public static int ch_frqtblptr = 0x6;          //  pointer to freq table (changes per octave)
			public static int ch_lentblptr = 0x8;          //  pointer to length table (changes per tempo)
			public static int ch_envrate = 0xA;          //  rate/speed of env table traversal
			public static int ch_envrem = 0xB;          //  remaining "fraction" of env data (3 bits of fraction)
			public static int ch_vol = 0xC;          //  output volume
			public static int ch_loopctr = 0xD;          //  remaining loop counter
			public static int ch_freq = 0xE;          //  output freq.  High bit set marks byte has been written (don't rewrite to reset duty)


			public static int framecounter = 0xF0;         //  2 bytes!

			public static int npcdir_seed = 0xF4;         //  RNG seed for determining direction for NPCs to walk

			public static int battlestep = 0xF5;
			public static int battlestep_sign = 0xF6;
			public static int battlecounter = 0xF7;
			public static int battlerate = 0xF8;         //  X/256 chance of a random encounter occuring (SM only apparently)

			public static int startintrocheck = 0xF9;
			public static int respondrate = 0xFA;

			public static int NTsoft2000 = 0xFD;         //  same as soft2000, but used to track coarse NT scroll
			public static int unk_FE = 0xFE;
			public static int soft2000 = 0xFF;

			public static int unk_0100 = 0x0100;

			public static int tmp_hi = 0x0110;       //  3? bytes

			public static int oam = 0x0200;       //  256 bytes -- must be on page bound
			public static int oam_y = oam;
			public static int oam_t = oam + 1;
			public static int oam_a = oam + 2;
			public static int oam_x = oam + 3;

			public static int puzzle = 0x0300;       //  shared
			public static int str_buf = 0x0300;       //  $39 bytes at least -- buffer must not cross page
			public static int item_box = 0x0300;       //  $20? bytes -- shares space with str_buf

			public static int ptygen = 0x0300;       //  $40 bytes, shared
			public static int ptygen_class = ptygen;
			public static int ptygen_name = ptygen + 2;
			public static int ptygen_name_x = ptygen + 6;
			public static int ptygen_name_y = ptygen + 7;
			public static int ptygen_class_x = ptygen + 8;
			public static int ptygen_class_y = ptygen + 9;
			public static int ptygen_spr_x = ptygen + 0xA;
			public static int ptygen_spr_y = ptygen + 0xB;
			public static int ptygen_box_x = ptygen + 0xC;
			public static int ptygen_box_y = ptygen + 0xD;
			public static int ptygen_curs_x = ptygen + 0xE;
			public static int ptygen_curs_y = ptygen + 0xF;

			public static int shop_charindex = 0x030A;       //  shared
			public static int shop_spell = 0x030B;       //  shared
			public static int shop_curitem = 0x030C;       //  shared
			public static int shop_curprice = 0x030E;       //  2 shared bytes

			public static int cur_pal = 0x03C0;       //  32 bytes
			public static int inroom_pal = cur_pal + 0x20; //  16 bytes
			public static int tmp_pal = 0x03F0;       //  16 bytes

			public static int tileset_data = 0x0400;       //  $400 bytes -- must be on page bound

			public static int mm_drawbuf = 0x0500;       // $100 bytes, shared, should be on page bound, but don't think it's absolutely required
			public static int mm_mapbuf = 0x0600;       //  same
			public static int mm_mapbuf2 = 0x0700;       //  same

			public static int tileset_prop = tileset_data; //  256 bytes, 2 bytes per tile
			public static int tsa_ul = tileset_data + 0x100; //  128 bytes
			public static int tsa_ur = tileset_data + 0x180; //  128
			public static int tsa_dl = tileset_data + 0x200; //  128
			public static int tsa_dr = tileset_data + 0x280; //  128
			public static int tsa_attr = tileset_data + 0x300; //  128
			public static int load_map_pal = tileset_data + 0x380; //  $30  (shared with draw_buf -- hence only for loading)

			public static int draw_buf = 0x0780;       //  128
			public static int draw_buf_ul = draw_buf;
			public static int draw_buf_ur = draw_buf + 0x10;
			public static int draw_buf_dl = draw_buf + 0x20;
			public static int draw_buf_dr = draw_buf + 0x30;
			public static int draw_buf_attr = draw_buf + 0x40;
			public static int draw_buf_at_hi = draw_buf + 0x50;
			public static int draw_buf_at_lo = draw_buf + 0x60;
			public static int draw_buf_at_msk = draw_buf + 0x70;


			public static int unk_07D2 = 0x07D2;



			public static int unsram = 0x6000;       //  $400 bytes
			public static int sram = 0x6400;       //  $400 bytes
			public static int sram_checksum = sram + 0xFD;
			public static int sram_assert_55 = sram + 0xFE;
			public static int sram_assert_AA = sram + 0xFF;


			public static int ship_vis = unsram + 0x00;
			public static int ship_x = unsram + 0x01;
			public static int ship_y = unsram + 0x02;

			public static int airship_vis = unsram + 0x04;
			public static int airship_x = unsram + 0x05;
			public static int airship_y = unsram + 0x06;

			public static int bridge_vis = unsram + 0x08;
			public static int bridge_x = unsram + 0x09;
			public static int bridge_y = unsram + 0x0A;

			public static int canal_vis = unsram + 0x0C;
			public static int canal_x = unsram + 0x0D;
			public static int canal_y = unsram + 0x0E;

			public static int unsram_ow_scroll_x = unsram + 0x10;
			public static int unsram_ow_scroll_y = unsram + 0x11;

			public static int has_canoe = unsram + 0x12; //  (not to be confused with item_canoe)
			public static int unsram_vehicle = unsram + 0x14;

			public static int bridgescene = unsram + 0x16; //  00=hasn't happened yet. 01=happens when move is complete, 80=already has happened

			public static int gold = unsram + 0x1C; //  3 bytes
			public static int items = unsram + 0x20;

			public static int item_lute = items + 0x01;
			public static int item_crown = items + 0x02;
			public static int item_crystal = items + 0x03;
			public static int item_herb = items + 0x04;
			public static int item_mystickey = items + 0x05;
			public static int item_tnt = items + 0x06;
			public static int item_adamant = items + 0x07;
			public static int item_slab = items + 0x08;
			public static int item_ruby = items + 0x09;
			public static int item_rod = items + 0x0A;
			public static int item_floater = items + 0x0B;
			public static int item_chime = items + 0x0C;
			public static int item_tail = items + 0x0D;
			public static int item_cube = items + 0x0E;
			public static int item_bottle = items + 0x0F;
			public static int item_oxyale = items + 0x10;
			public static int item_canoe = items + 0x11;
			public static int item_orb_start = items + 0x12;
			public static int orb_fire = item_orb_start + 0;
			public static int orb_water = item_orb_start + 1;
			public static int orb_air = item_orb_start + 2;
			public static int orb_earth = item_orb_start + 3;
			public static int item_qty_start = item_orb_start + 4;
			public static int item_tent = item_qty_start + 0;
			public static int item_cabin = item_qty_start + 1;
			public static int item_house = item_qty_start + 2;
			public static int item_heal = item_qty_start + 3;
			public static int item_pure = item_qty_start + 4;
			public static int item_soft = item_qty_start + 5;
			public static int item_stop = item_qty_start + 6;

			public static int ch_stats = unsram + 0x0100; //  MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats

			public static int ch_class = ch_stats + 0x00;
			public static int ch_ailments = ch_stats + 0x01;
			public static int ch_name = ch_stats + 0x02; //  4 bytes

			public static int ch_exp = ch_stats + 0x07; //  3 bytes
			public static int ch_curhp = ch_stats + 0x0A; //  2 bytes
			public static int ch_maxhp = ch_stats + 0x0C; //  2 bytes

			public static int ch_str = ch_stats + 0x10;
			public static int ch_agil = ch_stats + 0x11;
			public static int ch_int = ch_stats + 0x12;
			public static int ch_vit = ch_stats + 0x13;
			public static int ch_luck = ch_stats + 0x14;

			public static int ch_exptonext = ch_stats + 0x16; //  2 bytes -- only for user display, not actually used.
			public static int ch_weapons = ch_stats + 0x18; //  4
			public static int ch_armor = ch_weapons + 4; //  4

			public static int ch_substats = ch_stats + 0x20;
			public static int ch_dmg = ch_substats + 0x00;
			public static int ch_hitrate = ch_substats + 0x01;
			public static int ch_absorb = ch_substats + 0x02;
			public static int ch_evade = ch_substats + 0x03;
			public static int ch_resist = ch_substats + 0x04;
			public static int ch_magdef = ch_substats + 0x05;

			public static int ch_level = ch_stats + 0x26; //  OB this is 0 based, IB this is 1 based

			public static int game_flags = unsram + 0x0200; //  must be on page bound

			// Out of battle, spell data is stored stupidly so valid values are only 00-08, where 01 to 08 are actual spells
			//   and 00 is 'empty'.  Each spell is conceptually in a "slot" that belongs to each spell level.  Therefore,
			//   both CURE and LAMP are stored as '01' because they're both the first spell in their level, but because
			//   they're in a different level slot, the game distinguishes them.
			// In battle, fortunately, that is thrown out the window (why does it do it at all?) and the spells are stored
			//   in a logical 1-based index where the level simply doesn't matter.

			public static int ch_magicdata = unsram + 0x0300; //  must be on page bound
			public static int ch_spells = ch_magicdata;
			public static int ch_mp = ch_magicdata + 0x20;
			public static int ch_curmp = ch_mp + 0x00;
			public static int ch_maxmp = ch_mp + 0x08;


			public static int btl_chstats = 0x6800;       //  $12 bytes per character
			public static int btlch_slotindex = 0x00;
			public static int btlch_class = 0x01;
			public static int btlch_ailments = 0x02;         //  appears not to be used?  OB always seems to be used
			public static int btlch_hp = 0x03;         //  appears not to be used?  OB always seems to be used
			public static int btlch_hitrate = 0x05;
			public static int btlch_magdef = 0x06;
			public static int btlch_evade = 0x07;
			public static int btlch_absorb = 0x08;
			public static int btlch_dmg = 0x09;
			public static int btlch_elemresist = 0x0A;
			public static int btlch_numhitsmult = 0x0B;
			public static int btlch_numhits = 0x0C;
			public static int btlch_category = 0x0D;         //  always 0 since players have no category assigned
			public static int btlch_elemweak = 0x0E;         //  always 0 (players can't have weaknesses)
			public static int btlch_critrate = 0x0F;
			public static int btlch_wepgfx = 0x10;
			public static int btlch_wepplt = 0x11;


			public static int btl_turnorder = 0x6848;       //  $D entries (9 enemies + 4 characters)

			// Battle stuff
			public static int MATHBUF_HITCHANCE = 0;
			public static int MATHBUF_BASEDAMAGE = 1;
			public static int MATHBUF_NUMHITS = 2;
			public static int MATHBUF_MAGRANDHIT = 2;
			public static int MATHBUF_CATEGORY = 3;
			public static int MATHBUF_ELEMENT = 4;
			public static int MATHBUF_RANDHIT = 4;
			public static int MATHBUF_DMGCALC = 5;
			public static int MATHBUF_CRITCHANCE = 6;
			public static int MATHBUF_AILMENTCHANCE = 7;
			public static int MATHBUF_MAGDEFENDERHP = 0x12;
			public static int MATHBUF_DEFENDERHP = 0x13;
			public static int MATHBUF_MAGDEFENDERMAXHP = 0x15;
			public static int MATHBUF_TOTALDAMAGE = 0x16;

			public static int btl_mathbuf = 0x6856;       //  $14 bytes?, 2 byte pairs, used as buffers for mathematical routines
			public static int math_hitchance = btl_mathbuf + (MATHBUF_HITCHANCE * 2); //  $6856
			public static int math_basedamage = btl_mathbuf + (MATHBUF_BASEDAMAGE * 2); //  $6858
			public static int math_numhits = btl_mathbuf + (MATHBUF_NUMHITS * 2); //  $685A
			public static int math_magrandhit = btl_mathbuf + (MATHBUF_MAGRANDHIT * 2); //  $685A
			public static int math_category = btl_mathbuf + (MATHBUF_CATEGORY * 2); //  $685C not really math... but whatever
			public static int math_element = btl_mathbuf + (MATHBUF_ELEMENT * 2); //  $685E not really math... but whatever
			public static int math_randhit = btl_mathbuf + (MATHBUF_RANDHIT * 2); //  $685E
			public static int math_dmgcalc = btl_mathbuf + (MATHBUF_DMGCALC * 2); //  $6860
			public static int math_critchance = btl_mathbuf + (MATHBUF_CRITCHANCE * 2); //  $6862
			public static int math_ailmentchance = btl_mathbuf + (MATHBUF_AILMENTCHANCE * 2); // $6864

			public static int eob_gp_reward = 0x6876;
			public static int eob_exp_reward = 0x6878;


			public static int battle_ailmentrandchance = 0x6866;

			public static int battle_hitsconnected = 0x686A;       //  number of hits actually connected
			public static int battle_critsconnected = 0x686B;
			public static int btl_attacker_strength = 0x686C;
			public static int btl_attacker_category = 0x686D;
			public static int btl_attacker_element = 0x686E;
			public static int btl_attacker_hitrate = 0x686F;
			public static int btl_attacker_numhitsmult = 0x6870;
			public static int btl_attacker_numhits = 0x6871;
			public static int btl_attacker_critrate = 0x6872;
			public static int btl_attacker_attackailment = 0x6873;

			public static int btl_defender_category = 0x6876;
			public static int btl_defender_elemweakness = 0x6877;
			public static int btl_defender_evade = 0x6878;
			public static int btl_defender_absorb = 0x6879;
			public static int btl_defender_magdef = 0x687A;
			public static int btl_defender_elemresist = 0x687B;
			public static int btl_defender_hp = btl_mathbuf + (MATHBUF_DEFENDERHP * 2); //  $687C -- treated as part of math buffer by some code

			public static int btl_attacker_graphic = 0x6880;       //  the graphic used for an attack
			public static int btl_attacker_varplt = 0x6881;       //  The variable palette color used for an attack

			public static int battle_totaldamage = btl_mathbuf + (MATHBUF_TOTALDAMAGE * 2); //  $6882 -- treated as part of math buffer by some code

			// Battle stuff for magical attacks

			public static int btlmag_spellconnected = 0x685C;
			public static int btlmag_defender_ailments = 0x686D;
			public static int btlmag_effect = 0x686E;
			public static int btlmag_hitrate = 0x6870;
			public static int btlmag_defender_magdef = 0x6872;
			public static int btlmag_defender_unknownRawInit0 = 0x6873;
			public static int btlmag_effectivity = 0x6874;
			public static int btlmag_defender_elemweakness = 0x6876;
			public static int btlmag_defender_elemresist = 0x6877;
			public static int btlmag_element = 0x6878;

			public static int btlmag_attacker_unk686C = 0x686C;    //  Enemy:  ailments              Player:  ailments
			public static int btlmag_attacker_unk6875 = 0x6875;    //  Enemy:  ai                    Player:  damage
			public static int btlmag_attacker_unk6879 = 0x6879;    //  Enemy:  high byte of GP       Player:  class
			public static int btlmag_attacker_unk6883 = 0x6883;    //  Enemy:  0                     Player:  level
			public static int btlmag_attacker_unk6884 = 0x6884;    //  Enemy:  damage                Player:  hit rate

			public static int btlmag_defender_hp = btl_mathbuf + (MATHBUF_MAGDEFENDERHP * 2); //  $687A
			public static int btlmag_defender_numhitsmult = 0x687D;
			public static int btlmag_defender_morale = 0x687E;
			public static int btlmag_defender_absorb = 0x687F;
			public static int btlmag_defender_hpmax = btl_mathbuf + (MATHBUF_MAGDEFENDERMAXHP * 2); //  $6880
			public static int btlmag_defender_strength = 0x6882;

			public static int btlmag_defender_evade = 0x6885;      //  shared with battle_defender_index?  Is that not used for btlmag ?
			public static int btlmag_defender_category = 0x6886;

			public static int btlmag_fakeout_defindex = 0x6D95;    //  See Battle_DoTurn in bank C for a description of
			public static int btlmag_fakeout_ailments = 0x6D94;    //    what these 'fakeout' vars are and how/why they're used
			public static int btlmag_fakeout_defplayer = 0x6D96;

			// These two are kind of redundant
			public static int battle_attacker_index = 0x6884;      //  ?? redundant??  why not just use btl_attacker?
			public static int battle_defender_index = 0x6885;      //  same... but this is necessary for output!  See Battle_DoTurn in bank C!!

			public static int battle_defenderisplayer = 0x6887;    //  nonzero if player is defending, zero if enemy is defending
																   //   important for output!  See Battle_DoTurn in bank C

			public static int btl_attacker_ailments = 0x6888;
			public static int btl_defender_ailments = 0x6889;      //  important for output!

			public static int btl_rngstate = 0x688A;       //  State of RNG used for in-battle

			public static int btltmp_divLo = 0x688B;
			public static int btltmp_divHi = 0x688C;
			public static int btltmp_divV = 0x688D;

			public static int btl_curturn = 0x688E;       //  current turn (index for btl_turnorder)

			//  Buffers to hold character commands for battle.  These must be contiguious in memory
			//  due to the way memory is cleared.  These buffers also contain a bit of redundant data.
			//  btl_charcmdbuf contains 3 bytes (padded to 4) per character:
			//    byte 0 = command
			//    byte 1 = spell effect ID  (used for DRINK/MAGIC/ITEM).  FF if no effect
			//    byte 2 = target.  8x are player targets 0x are enemy targets.  FF=target all enemies, FE=target all players.
			//  Commands can be the following:
			//    00 = no command -- if surprised/asleep/stunned
			//    01 = no command -- if dead
			//    02 = no command -- if stone
			//    04 = attack
			//    08 = drink potion
			//    10 = use item
			//    20 = run   ('target' would be the actual character running)
			//    40 = magic
			//  btl_charcmditem contains 1 byte per character:  the ID of the item they're using.
			//    This is only used when the command is '10'
			//  btl_charcmdconsumetype contains 1 byte per character.  It will be 01 for magic and 02 for DRINK.
			//       unused for other commands.
			//  btl_charcmdconsumeid contains 1 byte per character.  If will be the potion index
			//       for drink, or the spell level for magic

			public static int btl_charcmdbuf = 0x688F;
			public static int btl_charcmditem = btl_charcmdbuf + 0x10; //  $689F
			public static int btl_charcmdconsumetype = btl_charcmditem + 4; //  $68A3
			public static int btl_charcmdconsumeid = btl_charcmdconsumetype + 4; //  $68A7

			public static int char_order_buf = 0x689F;

			// These next 5 vars are all in temp memory, and are mostly just used for
			//  passing into BattleDraw8x8Sprite
			public static int btl8x8spr_x = 0x68AF;       //  X coord
														  //  +1 used in drawing code as original position
			public static int btl8x8spr_y = 0x68B1;       //  Y coord
														  //  +1 used in drawing code as original position
			public static int btl8x8spr_a = 0x68B3;       //  attribute
			public static int btl8x8spr_t = 0x68B4;       //  tile ID
			public static int btl8x8spr_i = 0x68B5;       //  slot to draw to (00-3F)

			public static int btl_tmpindex = 0x68B3;       //  temporary holder for a current index
			public static int btl_tmpchar = 0x68B4;       //  temporary holder for a 0-based character index

			public static int btltmp_multA = 0x68B3;       //  shared
			public static int btltmp_multB = 0x68B4;       //  shared
			public static int btltmp_multC = 0x68B5;


			public static int btltmp_boxleft = 0x68B3;
			public static int btltmp_boxcenter = 0x68B4;
			public static int btltmp_boxright = 0x68B5;

			public static int btl_input = 0x68B3;

			public static int btl_soft2000 = 0x68B7;       //  soft copy of $2000 used in battles
			public static int btl_soft2001 = 0x68B8;       //  soft copy of $2001 used in battles

			public static int btlbox_blockdata = 0x68BA;
			// $68CF   ???

			public static int btl_msgbuffer = 0x691E;       //  $180 bytes  ($0C rows * $20 bytes per row)
															// this buffer contains on-screen tiles to be drawn to ppu$2240
															// (note only $19 bytes are actually drawn, the remaining 7 bytes are padding)

			public static int btl_msgdraw_hdr = 0x6A9E;
			public static int btl_msgdraw_x = 0x6A9F;
			public static int btl_msgdraw_y = 0x6AA0;
			public static int btl_msgdraw_width = 0x6AA1;
			public static int btl_msgdraw_height = 0x6AA2;
			public static int btl_msgdraw_srcptr = 0x6AA1;       //  shared
																 // $6AA2    above +1

			public static int btl_msgdraw_blockcount = 0x6AA3;     //  the number of blocks drawn

			public static int eobbox_slotid = 0x6AA6;
			public static int eobbox_textid = 0x6AA7;

			public static int btlinput_prevstate = 0x6AA6;       //  prev state for input
			public static int inputdelaycounter = 0x6AA7;       //  counter to delay multiple-input processing when holding a direction

			public static int btl_animatingchar = 0x6AA9;       //  the character currently being animated (0-3)

			public static int btlcurs_x = 0x6AAA;       //  battle cursor X position (menu position, not pixel position)
			public static int btlcurs_y = 0x6AAB;       //  battle cursor Y position (menu position, not pixel position)
			public static int btlcurs = 0x6AAA;
			public static int btlcurs_max = 0x6AAB;       //  highest value for the cursor
			public static int btlcurs_positions = 0x6AAC;       //  ?? bytes, 2 bytes per entry, each entry is the pixel coord of where the
																//   cursor should be drawn when its item is selected.

			public static int btl_drawflagsA = 0x6AD1;       //  bits 0-3 = set to indicate character should be drawn as dead
															 // bit    4 = set to draw battle cursor
															 // bit    5 = set to draw weapon attack graphic
															 // bit    6 = set to draw magic graphic & flash BG.

			public static int btl_drawflagsB = 0x6AD2;       //  bits 0-4 = set to indicate character should be drawn as stone

			public static int btl_chardrawinfo = 0x6AD3;       // $10 bytes, 4 bytes for each character
			public static int btl_chardraw_x = btl_chardrawinfo + 0;
			public static int btl_chardraw_y = btl_chardrawinfo + 1;
			public static int btl_chardraw_gfxset = btl_chardrawinfo + 2;
			public static int btl_chardraw_pose = btl_chardrawinfo + 3;

			public static int btlcursspr_x = 0x6AE3;
			public static int btlcursspr_y = 0x6AE4;
			public static int btlattackspr_x = 0x6AE5;
			public static int btlattackspr_y = 0x6AE6;

			public static int btlattackspr_t = 0x6AE7;       //  indicate which tile to draw for the weapon graphic
			public static int btlattackspr_pose = 0x6AE8;       //  for weapons, 0 or 8 to indicate whether or not to flip it
																// for magic, 0 or ?4? to indicate which frame to draw
			public static int btlattackspr_gfx = 0x6AE9;       //  copied to 't' prior to drawing.  Indicates which graphic to use
			public static int btlattackspr_wepmag = 0x6AEA;       //  0 for drawing the weapon, 1 for drawing the magic

			public static int btlattackspr_hidell = 0x6AED;       //  nonzero to hide the lower-left tile of the attack graphic
																  //   This is done for the "behind the back" frame of weapon swing animation.
			public static int btlattackspr_nodraw = 0x6AEE;       //  nonzero to hide the weapon/magic sprite entirely.  This is
																  //   Used when a non-BB player attacks without any weapon equipped
																  //   Also used when using ITEMs to supress the magic flashing effect.

			public static int btltmp_targetlist = 0x6AEF;       //  temporary buffer (9 entries) containing possible targets

			public static int btl_combatboxcount = 0x6AF8;       //  the number of combat boxes that have been drawn

			public static int btl_unfmtcbtbox_buffer = 0x6AFA;     //  $80 bytes total, $10 bytes for each combat box.
																   // houses the unformatted text for each combat box.
																   // Additional bytes are used for other areas

			public static int btlcmd_curchar = 0x6B7A;       //  the current character inputting battle commands (0-3)
			public static int btlcmd_target = 0x6B7B;       //  the current enemy slot that is being targetted

			public static int btlcmd_magicgfx = 0x6B7E;       //  2 bytes per character.  [0] = graphic to draw, [1] = palette to use

			public static int btl_result = 0x6B86;       //    0 = keep battling
														 //   1 = party defeated
														 //   2 = all enemies defeated
														 //   3 = party ran
														 // $FF = wait for 2 seconds after fadeout before exiting (chaos defeated?)

			public static int btl_usepalette = 0x6B87;       //  $20 bytes - the palette that is actually displayed (after fade effects)

			public static int btl_followupmusic = 0x6BA7;       //  song to play in battle after current song finishes.  Moved to music_track
																//    once music_track has its high bit set  (does this ever happen?)

			public static int btl_charattrib = 0x6BA8;       //  attributes to use when drawing charcters in battle  (4 bytes, 1 for each)

			public static int btl_responddelay = 0x6BAC;

			public static int btl_strikingfirst = 0x6BAE;       //  nonzero if players are striking first.  Zero otherwise

			public static int btl_potion_heal = 0x6BAF;       //  battle containers for Heal/Pure potions.  Stored separately because
			public static int btl_potion_pure = 0x6BB0;       //   it can fall out of sync with the ACTUAL items (if a character trying
															  //  to use one dies, for example)

			public static int battle_bank = 0x6BB1;       //  The bank to jump back to for setting up battles
			public static int btl_smallslots = 0x6BB2;       //  Number of small enemy slots available
			public static int btl_largeslots = btl_smallslots + 1; //  Number of large slots available.  Must immediately follow smallslots

			public static int btl_enemyeffect = 0x6BB6;       //  0 to draw expolosion graphics as the effect
															  //   nonzero to erase the enemy as the effect

			public static int btl_enemyIDs = 0x6BB7;       //  9 entries of enemy IDs
			public static int btl_enemygfxplt = 0x6BC0;       //  9 entries of enemy graphic and palette assignment (graphic in high 2 bits, plt in low bit)

			public static int btl_enemyroster = 0x6BC9;       //  4 bytes of enemy IDs printed in the main battle menu, showing enemies in the fight

			public static int btl_attacker_alt = 0x6BCD;       //  An EXTREMELY redundant and stupid copy of btl_attacker

			public static int btl_randomplayer = 0x6BCF;       //  set by GetRandomPlayerTarget  (0-3)

			public static int btl_enemystats = 0x6BD3;       //  $14 bytes per enemy - data does NOT match how it is stored in ROM
			public static int en_romptr = 0x00;         //  2 bytes - pointer to enemy stats in ROM
			public static int en_hp = 0x02;         //  2 bytes
			public static int en_defense = 0x04;
			public static int en_numhitsmult = 0x05;
			public static int en_ailments = 0x06;
			public static int en_aimagpos = 0x07;
			public static int en_aiatkpos = 0x08;
			public static int en_morale = 0x09;
			public static int en_evade = 0x0A;
			public static int en_strength = 0x0B;
			public static int en_ai = 0x0C;
			public static int en_exp = 0x0D;         //  2 bytes
			public static int en_gp = 0x0F;         //  2 bytes
			public static int en_enemyid = 0x11;
			public static int en_unknown12 = 0x12;         //  low byte of HP max
			public static int en_unknown13 = 0x13;         //  not initialized?  probably suppoed to be high byte of HP max

			public static int btl_tmppltassign = 0x6C88;       //  temporary value to assign palette to enemies in a formation

			public static int btl_attacker = 0x6C89;
			public static int btl_defender = 0x6C8A;
			public static int btl_combatboxcount_alt = 0x6C8B;     //  ANOTHER combatbox counter... this is totally redundant
			public static int btl_attackid = 0x6C8C;       //  >= $42 for enemy attacks

			public static int btlmag_magicsource = 0x6C8F;       //  0=magic, 1=drink, 2=item
			public static int btlmag_ailment_orig = 0x6C90;       //  A backup of
			public static int btl_battletype = 0x6C92;       //  0=9 small, 1=4 large, 2=mix, 3=fiend, 4=chaos
			public static int btl_enemycount = 0x6C93;       //  count of the number of enemies being generated for a battle
			public static int btltmp_attr = 0x6C94;       //  $40 bytes of attribute data for the battle setup

			// temporary space used by the lineup menu
			public static int lutmp_ch_stats = 0x6C00;
			public static int lutmp_ch_magic = 0x6D00;


			public static int bigstr_buf = 0x6C00;       //  $81 bytes?

			public static int btl_stringoutputbuf = 0x6CD4;       //  output buffer where decoded strings are printed

			public static int explode_min_x = 0x6D14;
			public static int explode_min_y = 0x6D15;
			public static int explode_max_x = 0x6D16;
			public static int explode_max_y = 0x6D17;
			public static int explode_count = 0x6D18;

			public static int btltmp_altmsgbuffer = 0x6D19;
			public static int btltmp_attackerbuffer = 0x6D2C;

			// ????          = $6D14   ; action buffer?  $20 bytes?  contents for combat boxes are placed here?

			public static int btl_palettes = 0x6D34;       //  $20 bytes

			public static int btl_stringbuf = 0x6D54;       //  $20 byte buffer to contain string data for printing

			public static int btltmp_backseat = 0x6D74;       //  $10 byte buffer -- backup of btltmp


			public static int btl_formdata = 0x6D84;       //  $10 bytes (formation data as appears in ROM)
			public static int btlform_type = btl_formdata + 0x0; //  battle type (high 4 bits) -- low 4 bits are pattern table
			public static int btlform_engfx = btl_formdata + 0x1; //  graphic assignment (2 bits per enemy)
			public static int btlform_enids = btl_formdata + 0x2; //  enemy IDs (4 bytes)
			public static int btlform_enqty = btl_formdata + 0x6; //  enemy quantities (4 bytes)
			public static int btlform_plts = btl_formdata + 0xA; //  palettes for this battle (2 bytes)
			public static int btlform_surprise = btl_formdata + 0xC; //  surprise rate
			public static int btlform_enplt = btl_formdata + 0xD; //  enemy palette assign (in high 4 bits)
			public static int btlform_norun = btlform_enplt; //  no run flag (in low bit)
			public static int btlform_enqtyB = btl_formdata + 0xE; //  enemy quantities for B formation (2 bytes)


			// btlsfx_backseat   = $6D97

			public static int btlmag_playerhitsfx = 0x6DA7;       //  sound effect to play when magic hits player
			public static int btltmp_smallslotpos = 0x6DB0;

			public static int mapobj = 0x6F00;       //  $100 bytes -- page
			public static int mapobj_id = mapobj + 0x00; //  rearranging these is ill advised
			public static int mapobj_flgs = mapobj + 0x01; //   because the loader is pretty rigid
			public static int mapobj_physX = mapobj + 0x02; //   flags:  $80=inroom $40=don't move
			public static int mapobj_physY = mapobj + 0x03;
			public static int mapobj_gfxX = mapobj + 0x04;
			public static int mapobj_gfxY = mapobj + 0x05;
			public static int mapobj_ctrX = mapobj + 0x06;
			public static int mapobj_ctrY = mapobj + 0x07;
			public static int mapobj_spdX = mapobj + 0x08;
			public static int mapobj_spdY = mapobj + 0x09;
			public static int mapobj_rawid = mapobj + 0x0A;
			public static int mapobj_movectr = mapobj + 0x0B;
			public static int mapobj_face = mapobj + 0x0C;
			public static int mapobj_pl = mapobj + 0x0D; //  bit 7 = talking to player (changes facing), other bits = being shoved by player
			public static int mapobj_tsaptr = mapobj + 0x0E;

			public static int mapdata = 0x7000;       //  must be on $1000 byte bound (ie:  pretty much unmovable)

			public static int mm_decorchr = 0x7000;       //  $300 bytes -- should be on page bound, shared
			public static int mm_titlechr = 0x7300;       //  $280 bytes -- should be on page bound, shared
														  // Don't add any fields to this class unless they are symbols. It will break AsDictionaries/AsLists.
		}

	}
}
