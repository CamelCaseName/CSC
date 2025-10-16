using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;//Token:0x\w|d|\s|:*\n

namespace CSC.StoryItems
{
    ////Token:0x\w|d|\s|:*\n
    //searchtermtoremovealltokencomments
    public static class StoryEnums
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum CompareTypes
        {
            Never = 1000,
            CharacterFromCharacterGroup = 17,
            Clothing = 110,
            CoinFlip = 160,
            CompareValues = 1,
            CriteriaGroup = 200,
            CutScene = 175,
            Dialogue = 40,
            Distance = 150,
            Door = 140,
            Gender = 138,
            IntimacyPartner = 136,
            IntimacyState = 135,
            InZone = 910,
            InVicinity = 19,
            InVicinityAndVision = 21,
            Item = 15,
            IsBeingSpokenTo = 81,
            IsPackageInstalled = 85,
            IsOnlyInVicinityOf = 77,
            IsOnlyInVisionOf,
            IsOnlyInVicinityAndVisionOf,
            IsCharacterEnabled = 210,
            IsCurrentlyBeingUsed = 191,
            IsCurrentlyUsing = 190,
            IsExplicitGameVersion = 2000,
            IsGameUncensored,
            IsInFrontOf = 151,
            IsInHouse = 912,
            IsNewGame = 900,
            IsZoneEmpty = 913,
            ItemFromItemGroup = 16,
            MetByPlayer = 30,
            Personality = 5,
            PlayerGender = 170,
            PlayerInventory = 10,
            PlayerPrefs = 950,
            Posing = 120,
            Property = 133,
            Quest = 100,
            Opportunity,
            SameZoneAs = 911,
            ScreenFadeVisible = 180,
            Social = 50,
            State = 130,
            Value = 0,
            Vision = 20,
            UseLegacyIntimacy = 960,
            None = 10000
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Packages
        {
            Base,
            Christmas,
            DojaCat,
            ExplicitContent,
            Halloween,
            LizKatz,
            Valentine,
            BusinessAndPleasure,
            AI,
            Supporter,
            NocturnalTemptations
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DialogueStatuses
        {
            WasNotShown,
            WasShown = 10,
            IsCurrentlyShowing = 20,
            NotCurrentlyShowing = 30
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DoorOptionValues
        {
            IsOpen,
            IsClosed,
            IsLocked,
            IsUnlocked
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum EqualsValues
        {
            Equals,
            DoesNotEqual
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ComparisonEquations
        {
            Equals,
            DoesNotEqual = 10,
            GreaterThan = 20,
            LessThan = 30
        }

        [JsonConverter(typeof(NullEnumConverter<ItemComparisonTypes>))]
        public enum ItemComparisonTypes
        {
            IsActive = 10,
            IsMounted = 20,
            IsMountedTo = 22,
            IsHeldByPlayer = 25,
            IsInventoriedOrHeldByPlayer,
            IsVisibleTo = 30
        }

        [JsonConverter(typeof(NullEnumConverter<ItemFromItemGroupComparisonTypes>))]
        public enum ItemFromItemGroupComparisonTypes
        {
            IsActive = 10,
            IsMounted = 20,
            IsMountedTo = 22,
            IsHeldByPlayer = 25,
            IsInventoriedOrHeldByPlayer,
            IsVisibleTo = 30,
            IsInPlayerInventory = 35
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PlayerInventoryOptions
        {
            HasItem,
            HasAtLeastOneItem
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PoseOptions
        {
            IsCurrentlyPosing,
            CurrentPose = 10
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Gender
        {
            None = 2,
            Female = 1,
            Male = 0
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SocialStatuses
        {
            Mood,
            Drunk = 10,
            Likes = 20,
            Loves = 30,
            Scared = 40,
            Offended,
            TalkTo = 50,
            SendText = 60
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum BoolCritera
        {
            False,
            True
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ClothingChangeOptions
        {
            ClothingType,
            AllOn,
            AllOff,
            ChangeItem,
            ChangeToOutfit,
            RemoveFromOutfit
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ClothingOnOff
        {
            On,
            Off,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SetEnum
        {
            Set0,
            Set1,
            Set2,
            Set3,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ValueSpecificFormulas
        {
            EqualsValue,
            DoesNotEqualValue,
            GreaterThanValue,
            LessThanValue
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum EventTypes
        {
            Never,
            GameStarts,
            EntersVision = 10,
            ExitsVision,
            EntersVicinity,
            EntersZone = 14,
            ReachesTarget,
            IsBlockedByLockedDoor,
            IsAttacked = 20,
            GetsKnockedOut,
            Dies,
            GetsHitWithProjectile,
            FallsOver = 30,
            IsNaked = 40,
            IsBottomless,
            IsTopless,
            ExposesGenitals = 50,
            CaughtMasturbating,
            CaughtHavingSex,
            ExposesChest,
            StartedIntimacyAct = 55,
            Orgasms = 60,
            EjaculatesOnMe = 70,
            GropesMyBreast,
            GropesMyAss,
            PlayerGrabsItem,
            PlayerReleasesItem,
            VapesOnMe,
            PopperedMe,
            PhoneBlindedMe,
            Periodically = 80,
            OnItemFunction = 90,
            OnAnyItemAcceptFallback,
            OnAnyItemRefuseFallback,
            CombatModeToggled = 100,
            PokedByVibrator = 110,
            ImpactsGround = 115,
            ImpactsWall,
            ScoredBeerPongPoint = 120,
            PeesOnMe = 130,
            PeesOnItem,
            StartedPeeing,
            StoppedPeeing,
            PlayerThrowsItem = 140,
            StartedUsingActionItem = 150,
            StoppedUsingActionItem,
            OnFriendshipIncreaseWith = 160,
            OnRomanceIncreaseWith,
            OnFriendshipDecreaseWith,
            OnRomanceDecreaseWith,
            IsDancing = 170,
            StartedLapDance,
            PlayerInventoryOpened = 180,
            PlayerInventoryClosed,
            PlayerOpportunityWindowOpened,
            PlayerInteractsWithCharacter = 185,
            PlayerInteractsWithItem,
            OnScreenFadeInComplete,
            OnScreenFadeOutComplete,
            FinishedPopulatingMainDialogueText = 200,
            PlayerTookCameraPhoto = 250,
            OnAfterCutSceneEnds,
            Ejaculates,
            None = 1000
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum InteractiveStates
        {
            Naked,
            WantsToBeAlone,
            Upset,
            Happy,
            ShooOthers,
            DontMoveForOthers,
            Embarrassed,
            Angry,
            IgnoreNavMeshRestrictions,
            Alive = 10,
            KnockedOut,
            GenitalsExposed,
            ChestExposed,
            ForcePlaySexSound,
            DontReleasePoseAfterSex,
            RechargingOrgasm,
            Standing,
            IgnoredByOthers,
            InCombat,
            Dancing,
            CanHearMusic,
            CurrentlyDisplayingDialogue,
            Crouching,
            Topless,
            Bottomless,
            Sitting,
            AbleToRoam,
            AbleToSocialize,
            AbleToDance,
            AbleToBeDistracted,
            Immobile,
            RunWhenCloseToTarget,
            Running,
            IndefinitelyErect,
            IsKneeling,
            IsLayingDown,
            IsLayingDownOnBack,
            IsLayingDownOnStomache,
            ForcedToDance,
            OnThePhone,
            Idling,
            HoldingADrinkRightHand,
            HoldingADrinkLeftHand,
            UnableToFidget,
            UnableToAnimateConverstion,
            Falling,
            UnableToEmote,
            UnableToAnimateEmotes,
            ActingInCinematic,
            CurrentlyPeeing,
            IsOnFire,
            Enabled,
            AbleToMoanDuringSexCutScene,
            UnableToOpenLabiaOrAsshole,
            UnableToLookAt,
            OverridingOralMouthPose,
            AbleToSexEmoteDuringCutScene,
            RunWhenFarAwayFromTarget,
            AbleToRechargeHealthInCombat,
            Floating,
            EyesAlwaysClosedWhenIdling,
            DontMoveForObstacles
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum InteractiveProperties
        {
            PlayerCombatMode = 1,
            PlayerCantBlock,
            IsBlocking,
            HitsGirls = 50,
            DoesNotFightBack,
            AlwaysBlocks,
            NoSprintingEnergyRegenPenalty,
            DoesNotUseIKToOpenDoors = 60,
            ForceBasicIdling = 80,
            IgnoresFireDamage = 85,
            RanStartEvents = 90,
            PlayerBladderAutoRechargeDisabled = 100,
            GivingStripTease = 110,
            ReceivingStripTease,
            IsFlashingShirt = 150,
            GivingBlowJob = 170,
            GivingCunnilingus,
            GivingOral,
            InCutScene = 200
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PersonalityTraits
        {
            Nice,
            Happy,
            Humerous,
            Creative,
            Jealous,
            LikesMen,
            LikesWomen,
            Aggressive,
            Sociable,
            Optimistic,
            Energetic,
            Serious,
            Intelligent,
            Charismatic,
            Shy,
            Perverse,
            Exhibitionism
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ClothingType
        {
            Top,
            Bottom,
            Underwear,
            Undershirt,
            Shoes,
            Accessory,
            StrapOn,
            Hair
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ClothingOptions
        {
            Change,
            ToggleWetEffect,
            ToggleBloodyEffect
        }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum AddRemoveActions
        {
            Add,
            Remove
        }

        public enum GameEvents
        {
            AddForce = 250,
            AllowPlayerSave = 305,
            ChangeBodyScale = 450,
            CharacterFromCharacterGroup = 192,
            CharacterFunction = 998,
            Clothing = 60,
            Combat = 140,
            CombineValue = 2,
            CutScene = 400,
            Dialogue = 120,
            DisableNPC = 301,
            DisplayGameMessage = 10,
            Door = 170,
            Emote = 50,
            EnableNPC = 300,
            EventTriggers = 5,
            FadeIn = 220,
            FadeOut,
            IKReach = 230,
            Intimacy = 180,
            Item = 190,
            ItemFromItemGroup,
            LookAt = 70,
            Personality = 137,
            Property = 135,
            MatchValue = 1,
            ModifyValue = 0,
            Player = 160,
            PlaySoundboardClip = 500,
            Pose = 150,
            Quest = 80,
            RandomizeIntValue = 3,
            ResetReactionCooldown = 260,
            Roaming = 110,
            SendEvent = 999,
            SetPlayerPref = 800,
            Social = 30,
            State = 130,
            TriggerBGC = 240,
            Turn = 65,
            TurnInstantly,
            UnlockAchievement = 90,
            WalkTo = 100,
            WarpOverTime = 211,
            WarpTo = 210,
            None = 1000
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum CombatOptions
        {
            Fight,
            Die = 10,
            PassOut = 20,
            WakeUp = 30,
            Cancel = 40
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum RoamingOptions
        {
            Allow,
            ChangeLocation,
            ProhibitLocation,
            AllowLocation,
            ClearAllowRoamList,
            ClearProhibitRoamList,
            ClearAllRoamLists,
            SetRoamingDelayToMaximum,
            SetRoamingDelayToMinimum,
            StopMyCurrentRoamingMotion,
            StopAllCurrentRoamingMotionTo
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SendEventOptions
        {
            None,
            TurnAround = 10,
            TurnLeft,
            TurnRight,
            StepForward = 20,
            StepBackwards,
            StepLeft,
            StepRight,
            PickupLeftHand = 50,
            PickupRightHand,
            Throw,
            ThrowPunch = 60,
            SwingWeapon1HRight = 65,
            JumpUp = 70,
            JumpDown,
            JumpAndFall,
            Point = 80,
            SipDrinkLeft = 90,
            SipDrinkRight,
            SipDrinkSittingLeft,
            SipDrinkSittingRight,
            SipDrinkHotTubLeft,
            SipDrinkHotTubRight,
            StopUsingActionItem = 100,
            Cheer = 110,
            Cheer2,
            Cheer3,
            EdgeSlip = 115,
            StartPeeing = 120,
            StopPeeing,
            ToggleGenitals = 130,
            StartStripTease,
            StopStripTease,
            Orgasm = 140,
            StubToe = 150,
            GameOver = 10000
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum IntimacyOptions
        {
            SexualAct,
            IncreaseActionSpeed = 10,
            DecreaseActionSpeed
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum GameMessageType
        {
            CenterScreenText,
            Narration = 10,
            ThoughtBubble = 20
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ResponseReactionTypes
        {
            VeryBad,
            Bad,
            Neutral,
            Good,
            VeryGood
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ResponseTypes
        {
            Generic,
            Declarative = 10,
            Question = 20,
            Compliment = 30,
            Insult = 40,
            Informitive = 50,
            Advice = 60,
            Intimidating = 70,
            Apology = 80
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ResponseTones
        {
            Friendly = 10,
            Mean = 20,
            Flirty = 30,
            Sarcastic = 40,
            Funny = 50,
            Intelligent = 60,
            Crass = 70,
            Insecure = 80,
            Confident = 90,
            Annoying = 100,
            Sweet = 110,
            Clever = 130,
            Gross = 150,
            Sincere = 160,
            Excited = 170,
            Bragging = 180,
            Forward = 190,
            Silly = 200,
            SmallTalk = 210,
            PersonalQuestion = 220
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ConversationalTopics
        {
            Art = 30,
            Weather = 50,
            Shopping = 90,
            Fun = 100,
            Hobbies,
            Drinking,
            Drugs,
            Dancing,
            Business = 110,
            Work,
            Entertainment = 120,
            Music,
            Movies,
            Television,
            Relationships = 130,
            Sex = 140,
            Food = 160,
            Politics = 170,
            Religeon = 180,
            Sports = 190,
            Clothes = 200
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Importance
        {
            None,
            Important,
            VeryImportant
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum QuestStatus
        {
            NotObtained,
            InProgress,
            Complete,
            Failed,
            Missed
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum StoryAspects
        {
            CharacterPersonality,
            ScriptedDialogue,
            DynamicDialogue,
            ItemInteractions,
            CharacterQuests,
            EventTriggers,
            BackgroundChatter,
            Values
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PassCondition
        {
            AllSetsAreTrue,
            AnySetIsTrue,
            AllSetsAreFalse,
            AnySetIsFalse
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DoorAction
        {
            Open,
            Close,
            Lock,
            Unlock,
            OpenSlowly,
            CloseSlowly
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum IKTargets
        {
            LeftHand,
            RightHand,
            LeftFoot,
            RightFoot,
            Head,
            Hips
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum DialogueAction
        {
            Trigger,
            Overhear,
            SetStartDialogue = 5,
            TriggerStartDialogue = 10
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum CutsceneAction
        {
            PlayScene,
            PlayRandomSceneFromLocation,
            PlayRandomSceneFromCurrentLocation,
            EndScene,
            EndAnySceneWithPlayer
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ItemEventAction
        {
            SetEnabled,
            Mount,
            Rename,
            ItemFunction,
            TriggerUseWithMenu,
            WarpItemTo,
            ApplyForceTowards,
            ApplyHotSpots,
            SetInventoryIcon
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ItemGroupAction
        {
            SetEnabled,
            RemoveFromPlayerInventory,
            Mount,
            Rename,
            ItemFunction,
            TriggerUseWithMenu,
            WarpItemTo,
            ApplyForceTowards,
            AddToPlayerInventory,
            GrabFromPlayerInventory
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Modification
        {
            Equals,
            Add
        }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TurnOptions
        {
            Around,
            Left,
            Right,
            Toward
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum WarpToOption
        {
            MoveTarget,
            Character,
            Item
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ImportanceSpecified
        {
            Unspecified,
            None,
            Important,
            VeryImportant
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SendEvents
        {
            TurnAround,
            TurnLeft,
            TurnRight,
            StepForward,
            StepBackward,
            StepLeft,
            StepRight,
            PickupLeftHand,
            PickupRightHand,
            Throw,
            ThrowPunch,
            JumpUp,
            JumpDown,
            JumpAndFall,
            Point,
            SipDrinkLeft,
            SipDrinkRight,
            SipDrinkSittingLeft,
            SipDrinkSittingRight,
            SipDrinkHotTubLeft,
            SipDrinkHotTubRight,
            StopUsingActionItem,
            Cheer,
            StartPeeing,
            StopPeeing,
            ToggleGenitals,
            StartStripTease,
            StopStripTease,
            Orgasm,
            GameOver
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum QuestActions
        {
            Start,
            Increment,
            Complete,
            Fail
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PlayerActions
        {
            Inventory,
            TriggerGiveTo,
            Sit,
            LayDown,
            TogglePenis,
            ToggleMasturbate,
            ToggleRadialFor,
            GrabFromInventory,
            DropCurrentlyHeldItem,
            FlashBreasts
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum RadialTriggerOptions
        {
            Item,
            Character
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum EventSpecialHandling
        {
            None,
            DialogueResponse,
            CloseDialogue,
            StartDialogue,
            ItemGroup,
            CutSceneEvents
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Characters
        {
            Amy,
            Arin,
            Ashley,
            Brittney,
            Compubrah,
            Dan,
            Derek,
            Frank,
            Katherine,
            Leah,
            Lety,
            Madison,
            Patrick,
            PhoneCall,
            Player,
            Rachael,
            Stephanie,
            Vickie,
            Amala,
            DojaCat,
            LizKatz
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum IntimateCharacters
        {
            None,
            Amy,
            Ashley,
            Brittney,
            Derek,
            Frank,
            Katherine,
            Leah,
            Lety,
            Madison,
            Patrick,
            Player,
            Rachael,
            Stephanie,
            Vickie,
            LizKatz
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum StoryCharacters
        {
            Amy,
            Ashley,
            Brittney,
            Derek,
            Frank,
            Katherine,
            Leah,
            Lety,
            Madison,
            Patrick,
            PhoneCall,
            Rachael,
            Stephanie,
            Vickie,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum NoneCharacters
        {
            None,
            Amy,
            Arin,
            Ashley,
            Brittney,
            Compubrah,
            Dan,
            Derek,
            Frank,
            Katherine,
            Leah,
            Lety,
            Madison,
            Patrick,
            PhoneCall,
            Player,
            Rachael,
            Stephanie,
            Vickie,
            Amala,
            DojaCat,
            LizKatz
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum AnybodyCharacters
        {
            Anybody,
            Amy,
            Arin,
            Ashley,
            Brittney,
            Compubrah,
            Dan,
            Derek,
            Frank,
            Katherine,
            Leah,
            Lety,
            Madison,
            Patrick,
            PhoneCall,
            Player,
            Rachael,
            Stephanie,
            Vickie,
            Amala,
            DojaCat,
            LizKatz
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum BGCEmotes
        {
            Happy,
            Sad,
            Angry,
            Surprised,
            Scared,
            Flirty,
            Ecstatic,
            Laugh,
            None = 10
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ClothingSet
        {
            AnySet,
            Set0,
            Set1
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum CutscenePlaying
        {
            AnyCutScenePlaying = 1,
            AnySexCutscenePlaying,
            CensoredSexScenePlaying
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SexualActs
        {
            None,
            MakingOut = 5,
            Masturbating = 10,
            BlowJob = 20,
            MissionarySex = 30,
            DoggieStyleSex = 40,
            Cowgirl = 50,
            Cunnilingus = 60,
            HotTubHandJob = 70,
            SixtyNine = 80,
            WallSex = 90,
            WallSex2,
            WallSex3,
            Fingering = 100,
            Scissoring = 200,
            InSexCutScene = 1000
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum IntimacyEvents
        {
            StartFingering = 7000,
            StartScissoring = 8000,
            StartMakingOut = 9000,
            StartBlowjob = 10000,
            StartHotTubHandJob = 10011,
            StartMissionary = 10030,
            StartDoggieStyle = 10050,
            StartMasturbation,
            StartCowGirl,
            StartCunnilingus,
            StartSixtyNine,
            StartWallSex,
            StartWallSex2,
            StartWallSex3,
            End = 10060
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Zones
        {
            ArtRoomZone,
            BalloonZone,
            CompubrahZone,
            DiningRoomZone,
            DownStairsHallZone,
            FrontEntryWayZone,
            FrontSidewalkZone,
            GarageZone,
            KitchenZone,
            LaundryRoomZone,
            LivingRoomZone,
            LoadingDockZone,
            MasterBedroomZone,
            OutsideRoofZone,
            SpareRoomZone,
            StairwayZone,
            StudyZone,
            YardGazeboZone,
            YardHotTubZone,
            YardLeftCornerZone,
            YardNearFirePitZone,
            YardPathGardenZone,
            YardRearLeftCornerZone,
            YardRightCornerZone,
            UpstairsBathroomZone,
            UpstairsHallZone,
            BehindFenceZone,
            None,
            ApartmentZone
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Poses
        {
            HandsBehindHead,
            SittingOnFloor = 11,
            BentOver = 20,
            HandsBehindBack = 30,
            StickingAssOut = 40,
            Sitting1 = 50,
            Sitting2,
            Sitting3,
            Sitting4,
            BlowjobAction = 60,
            BlowJobReady,
            FemaleLay1 = 70,
            FemaleLay2,
            FemaleLay3,
            ModelPose1 = 80,
            ModelPose2,
            ModelPose3,
            AllFours = 90,
            MissionaryTop = 100,
            MissionaryBottom,
            SexReady = 110,
            DoggieStyleFront = 120,
            DoggieStyleBehind,
            CowGirlTop = 130,
            CowGirlBottom,
            CunnilingusTop = 140,
            CunnilingusBottom,
            HotTubSit1 = 150,
            HotTubHandJob = 160,
            TakeCellPhoto = 170,
            SixtyNineTop = 180,
            SixtyNineBottom,
            Selfie = 190,
            MakingOutPrimary = 200,
            MakingOutSecondary,
            WallSexPrimary = 210,
            WallSexSecondary,
            WallSex2Primary = 220,
            WallSex2Secondary
        }

        public enum Females
        {
            Amy,
            Ashley,
            Brittney,
            Katherine,
            Leah,
            Lety,
            Madison,
            Rachael,
            Stephanie,
            Vickie,
            Amala,
            DojaCat,
            LizKatz
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum FemalePoses
        {
            HandsBehindHead,
            SittingOnFloor,
            BentOver,
            HandsBehindBack,
            StickingAssOut,
            Sitting1,
            Sitting2,
            Sitting3,
            Sitting4,
            BlowjobAction,
            BlowJobReady,
            FemaleLay1,
            FemaleLay2,
            FemaleLay3,
            ModelPose1,
            ModelPose2,
            ModelPose3,
            AllFours,
            MissionaryBottom,
            SexReady,
            DoggieStyleFront,
            CowGirlTop,
            CunnilingusTop,
            HotTubSit1,
            HotTubHandJob,
            TakeCellPhoto,
            SixtyNineTop,
            Selfie,
            MissionaryTop,
            DoggieStyleBehind,
            CowGirlBottom,
            CunnilingusBottom,
            SixtyNineBottom,
            MakingOutPrimary,
            MakingOutSecondary,
            WallSexPrimary,
            WallSexSecondary,
            WallSex2Primary,
            WallSex2Secondary,
            WallSex3Primary,
            WallSex3Secondary,
            HotTubHandJobPrimary,
            Masturbate,
            SitInBathtub,
            SittingOnFlamingo,
            TPose,
            BracingAgainstDoor1,
            BracingAgainstDoor2,
            BracingAgainstWindow,
            Meditate,
            TypingStanding,
            OperatingTablet,
            SixtyNineTopWithFemale,
            FingeringTop,
            FingeringBottom,
            ScissoringBottom,
            ScissoringTop,
            CoverGenitals,
            GiveSpeechArmsOut,
            GetStuckInDryer,
            GetStuckInDryerImmediate,
            HotTubSeat5Pose,
            DefaultCustomize,
            HairCustomize,
            ShoeCustomize,
            EyesCustomize,
            LoadingScren,
            NailsCustomize,
            ScreenshotPose,
            Pondering,
            OrganizingBriefcase
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum MalePoses
        {
            MissionaryTop,
            Sitting,
            Laying,
            DoggieStyleSexBehind,
            HotTubSit,
            CowGirlSexBottom,
            CunnilingusGiver,
            BlowJobGiverReady,
            BlowJobGiverAction,
            SixtyNineBottom,
            MissionaryBottom,
            DoggieStyleFront,
            CowgirlTop,
            MakingOutPrimary,
            MakingOutSecondary,
            WallSexPrimary,
            WallSexSecondary,
            WallSex2Primary,
            WallSex2Secondary,
            WallSex3Primary,
            WallSex3Secondary,
            HotTubHandJobPrimary,
            BathTubSit,
            StickOutAss,
            SitInBathTub,
            SixtyNineTop,
            TPose,
            BracingAgainstDoor1,
            BracingAgainstDoor2,
            BracingAgainstWindow,
            Meditate,
            TypingStanding,
            OperatingTablet,
            FingeringBottom,
            CoverGenitals,
            GiveSpeechArmsOut,
            DefaultCustomize,
            HairCustomize,
            ShoeCustomize,
            EyesCustomize,
            LoadingScreen,
            NailsCustomize,
            ScreenshotPose,
            Pondering,
            OrganizingBriefcase
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum LookAtTargets
        {
            Character,
            Camera,
            InteractiveItem = 10,
            GameObject = 20,
            PlayerPenis = 30
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PlayerPrefs
        {
            ShowTutorial
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum LocationTargetOption
        {
            MoveTarget,
            Character,
            Item
        }

        //from hpcontent.asset, replace following query by","
        //\s*\n\s+Roamable:\d\s*\n\s+AcceptableSexLocation:\d\s*\n\s+AcceptableWallSexLocation:\d\s*\n\s+AcceptableNavRecoveryTarget:\d\s*\n\s+NonWallSexUsesMyTransform:\d
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum MoveTargets
        {
            AboveCompubrah2,
            AboveHotTub,
            AboveHotTub2,
            Apartment,
            ApartmentAcrossFromEntryDoor,
            ApartmentCoffeeTable,
            ArtRoomCloset,
            AshleyDesk,
            BehindBushes,
            BehindGazebo,
            BehindHoleInFence,
            BehindHoleInFenceRight,
            BetweenKitchenAndDiningRoom,
            CenterOfGazebo,
            CenterOfGazeboLeft,
            CenterOfGazeboNearSteps,
            CenterOfGazeboRight,
            CenterOfHotTub,
            CompuBrah,
            CompuBrah2,
            CompuBrahBedSex,
            CompuBrahWallSexLeft,
            CompuBrahWallSexRight,
            DiningRoom,
            DiningRoomLeftOfArmchair,
            DiningRoomNearFrontWindow,
            DiningRoomNearTVMediaPlayer,
            DownstairsBathroom,
            DownstairsBathroomSexSafe1,
            EnterPartyPoint,
            Garage,
            GarageBehindTable,
            GarageBehindTable2,
            GarageNearWallVent,
            GarageSexSafe1,
            GarageSexSafe2,
            GarageSexSafe3,
            GarageSexSafe4,
            HotTubSex,
            InFrontOfBushes,
            InFrontOfCenterOfGazebo,
            InFrontOfCenterOfGazeboLeft,
            InFrontOfCenterOfGazeboRight,
            InFrontOfEasel,
            InFrontOfHoleInFence,
            InFrontOfTV,
            InsideSlider,
            JustInsideUpstairsBathroom,
            JustOutsideDownstairsBathroom,
            JustOutsideLaundryRoom,
            JustOutsideUpstairsBathroom,
            JustOutsideUpstairsMasterBathroom,
            Kitchen,
            KitchenClosestToFridge,
            KitchenInFrontOfFridge,
            KitchenNearFruitAndMicrowave,
            KitchenNearSink,
            KitchenSexSafe1,
            KitchenSexSafe2,
            LaundryRoom,
            LaundryRoomInFrontOfDryer,
            LivingRoom,
            LivingRoomSexSafe1,
            LivingRoomSexSafe2,
            LoadingDock,
            MasterBathroomSexSafe,
            MasterBedroom,
            MasterBedroomBedSex,
            MasterBedroomClosetArea,
            MasterBedroomClosetDoor,
            MasterBedroomClosetMiddle,
            MasterBedroomSexSafe1,
            MasterBedroomSexSafe2,
            NearBugLamp,
            NearFrontDoor,
            NearFrontDoorSexSafe1,
            NearFrontDoorSexSafe2,
            NearFrontDoorThreshold,
            NearGarageDoor1,
            NearGarageDoor2,
            Outside,
            OutsideArtRoomWindow,
            OutsideBetweenHTubFPit,
            OutsideBetweenPaths,
            OutsideByFirePit,
            OutsideByFirePit2,
            OutsideByFirePit3,
            OutsideByFirePit4,
            OutsideByFirePitFireTrailStart,
            OutsideByGarden,
            OutsideByTree1,
            OutsideByTree2,
            OutsideByTree3,
            OutsideDarkCornerLeft,
            OutsideDarkCornerLeft2,
            OutsideDarkCornerLeft3,
            OutsideDarkCornerLeft4,
            OutsideDarkCornerRight,
            OutsideDarkCornerRight2,
            OutsideDarkCornerRight3,
            OutsideDarkRearCornerLeft1,
            OutsideDarkRearCornerLeft2,
            OutsideDarkRearCornerLeft3,
            OutsideDiningRoomWindow,
            OutsideFenceLeftBushes,
            OutsideFenceRightBushes,
            OutsideFenceRightBushes2,
            OutsideFrontDoor,
            OutsideFrontOfHTub,
            OutsideGazeboLeft1,
            OutsideGazeboLeft2,
            OutsideInFrontOfEscapeHatch,
            OutsideInFrontOfEscapeHatch2,
            OutsideInFrontOfEscapeHatch3,
            OutsideInFrontOfEscapeHatch4,
            OutsideInFrontOfEscapeHatch5,
            OutsideInFrontOfGutter,
            OutsideLeftOfHTub,
            OutsideNearAirConditioner,
            OutsideNearAirConditioner2,
            OutsideNearAirConditioner3,
            OutsideOfficeWindow,
            OutsidePastEscapeHatch,
            OutsideRightOfHTub1,
            OutsideRightOfHTub2,
            OutsideSlider,
            OutsideSliderSexSafe1,
            RoofNearGutter,
            SpareBedroom,
            SpareRoom,
            SpareRoom2,
            SpareRoom2NearClosetDoorL,
            SpareRoom2NearClosetDoorR,
            SpareRoomClosetArea,
            SpareRoomMiddleNearWall,
            StairsSexSafe1,
            Study,
            UpstairsBathroom,
            UpstairsHallBannister,
            UpstairsHallway,
            UpstairsHallwayNearLaundryRoom,
            UpstairsMasterBathroom,
            WallSexSpotArtRoom,
            WallSexSpotArtRoom2,
            WallSexSpotArtRoomCloset,
            WallSexSpotDiningRoom,
            WallSexSpotDownstairsBathroom,
            WallSexSpotFence1,
            WallSexSpotFence2,
            WallSexSpotFence3,
            WallSexSpotFence4,
            WallSexSpotFence5,
            WallSexSpotGarage,
            WallSexSpotGarage2,
            WallSexSpotGarage3,
            WallSexSpotGarage4,
            WallSexSpotHouse1,
            WallSexSpotHouse2,
            WallSexSpotHouse3,
            WallSexSpotKitchen,
            WallSexSpotKitchen2,
            WallSexSpotLaundryRoom,
            WallSexSpotLivingRoom,
            WallSexSpotLivingRoom2,
            WallSexSpotMasterBedroom,
            WallSexSpotMasterBedroom2,
            WallSexSpotMasterBedroom3,
            WallSexSpotNearFrontDoor,
            WallSexSpotOffice,
            WallSexSpotSliderInside,
            WallSexSpotSliderOutside,
            WallSexSpotSpareRoom,
            WallSexSpotUpstairsBathroom,
            WallSexSpotUpstairsHallway,
            WallSexSpotUpstairsMasterBathroom,
            WatchTVSpot1,
            WatchTVSpot2,
        }

        //(\s+)EnableItemFunctions: \d\s+ItemFunctions:\s+(- \w+\s*)*SpecialType: 0\s+(\w+: \d\n+\s+)+- Name: 
        //replace with ,\n
        //consists of the Doors, Beds, Chairs, HottubSpots, SexSpots and burnable enums
        //plus all items with specialtype: 0
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Items
        {
            //chairs
            [JsonStringEnumMemberName("Apartment Sofa Seat (Left)")]
            ApartmentSofaSeatLeft,
            [JsonStringEnumMemberName("Apartment Sofa Seat (Right)")]
            ApartmentSofaSeatRight,
            [JsonStringEnumMemberName("Armchair")]
            Armchair,
            [JsonStringEnumMemberName("Computer Chair")]
            ComputerChair,
            [JsonStringEnumMemberName("Couch (Left)")]
            CouchLeft,
            [JsonStringEnumMemberName("Couch (Middle)")]
            CouchMiddle,
            [JsonStringEnumMemberName("Couch (Right)")]
            CouchRight,
            [JsonStringEnumMemberName("DownstairsToilet")]
            DownstairsToilet,
            [JsonStringEnumMemberName("DryerSeat")]
            DryerSeat,
            [JsonStringEnumMemberName("FlamingoSeatLeft")]
            FlamingoSeatLeft,
            [JsonStringEnumMemberName("FlamingoSeatRight")]
            FlamingoSeatRight,
            [JsonStringEnumMemberName("Frank's Chair")]
            FranksChair,
            [JsonStringEnumMemberName("LawnChair1")]
            LawnChair1,
            [JsonStringEnumMemberName("LawnChair2")]
            LawnChair2,
            [JsonStringEnumMemberName("LawnChair3")]
            LawnChair3,
            [JsonStringEnumMemberName("LawnChair4")]
            LawnChair4,
            [JsonStringEnumMemberName("MasterBathTubEdgeSeat")]
            MasterBathTubEdgeSeat,
            [JsonStringEnumMemberName("MasterBathTubSeat")]
            MasterBathTubSeat,
            [JsonStringEnumMemberName("Office Chair")]
            OfficeChair,
            [JsonStringEnumMemberName("Outside Sofa 1 Left")]
            OutsideSofa1Left,
            [JsonStringEnumMemberName("Outside Sofa 1 Middle")]
            OutsideSofa1Middle,
            [JsonStringEnumMemberName("Outside Sofa 1 Right")]
            OutsideSofa1Right,
            [JsonStringEnumMemberName("Outside Sofa 2 Left")]
            OutsideSofa2Left,
            [JsonStringEnumMemberName("Outside Sofa 2 Middle")]
            OutsideSofa2Middle,
            [JsonStringEnumMemberName("Outside Sofa 2 Right")]
            OutsideSofa2Right,
            [JsonStringEnumMemberName("Patio Armchair")]
            PatioArmchair,
            [JsonStringEnumMemberName("Patio Armchair 2")]
            PatioArmchair2,
            [JsonStringEnumMemberName("Pouf")]
            Pouf,
            [JsonStringEnumMemberName("RoofSeatLeft")]
            RoofSeatLeft,
            [JsonStringEnumMemberName("RoofSeatRight")]
            RoofSeatRight,
            [JsonStringEnumMemberName("Sofa (Left)")]
            SofaLeft,
            [JsonStringEnumMemberName("Sofa (Right)")]
            SofaRight,
            [JsonStringEnumMemberName("SpareRoomSofaLeft")]
            SpareRoomSofaLeft,
            [JsonStringEnumMemberName("SpareRoomSofaMiddle")]
            SpareRoomSofaMiddle,
            [JsonStringEnumMemberName("SpareRoomSofaRight")]
            SpareRoomSofaRight,
            [JsonStringEnumMemberName("UpstairsBathroomToilet")]
            UpstairsBathroomToilet,
            [JsonStringEnumMemberName("UpstairsBathTubEdgeSeat")]
            UpstairsBathTubEdgeSeat,
            [JsonStringEnumMemberName("UpstairsBathTubSeat")]
            UpstairsBathTubSeat,
            [JsonStringEnumMemberName("Vanity Chair")]
            VanityChair,
            [JsonStringEnumMemberName("WasherSeat")]
            WasherSeat,
            //beds
            [JsonStringEnumMemberName("Bed (Left)")]
            BedLeft,
            [JsonStringEnumMemberName("Bed (Right)")]
            BedRight,
            [JsonStringEnumMemberName("Compubrah Bed(Left)")]
            CompubrahBedLeft,
            [JsonStringEnumMemberName("Compubrah Bed(Right)")]
            CompubrahBedRight,
            //doors
            [JsonStringEnumMemberName("Bathroom Door")]
            BathroomDoor,
            [JsonStringEnumMemberName("Bedroom Closet Door (Left)")]
            BedroomClosetDoorLeft,
            [JsonStringEnumMemberName("Bedroom Closet Door (Right)")]
            BedroomClosetDoorRight,
            [JsonStringEnumMemberName("CabinetLeft")]
            CabinetLeft,
            [JsonStringEnumMemberName("CabinetRight")]
            CabinetRight,
            [JsonStringEnumMemberName("Desk Drawer Left")]
            DeskDrawerLeft,
            [JsonStringEnumMemberName("Desk Drawer Right")]
            DeskDrawerRight,
            [JsonStringEnumMemberName("Dryer Door")]
            DryerDoor,
            [JsonStringEnumMemberName("Escape Hatch")]
            EscapeHatch,
            [JsonStringEnumMemberName("Freezer")]
            Freezer,
            [JsonStringEnumMemberName("Fridge")]
            Fridge,
            [JsonStringEnumMemberName("Front Door")]
            FrontDoor,
            [JsonStringEnumMemberName("Garage Door")]
            GarageDoor,
            [JsonStringEnumMemberName("KitchenCabinet5")]
            KitchenCabinet5,
            [JsonStringEnumMemberName("KitchenCabinet6")]
            KitchenCabinet6,
            [JsonStringEnumMemberName("KitchenCabinet7")]
            KitchenCabinet7,
            [JsonStringEnumMemberName("KitchenCabinet8")]
            KitchenCabinet8,
            [JsonStringEnumMemberName("Laundry Room Door")]
            LaundryRoomDoor,
            [JsonStringEnumMemberName("Master Bathroom Door")]
            MasterBathroomDoor,
            [JsonStringEnumMemberName("Master Bedroom Door")]
            MasterBedroomDoor,
            [JsonStringEnumMemberName("Microwave Door")]
            MicrowaveDoor,
            [JsonStringEnumMemberName("Nightstand1")]
            Nightstand1,
            [JsonStringEnumMemberName("Nightstand2")]
            Nightstand2,
            [JsonStringEnumMemberName("Office Drawer Left")]
            OfficeDrawerLeft,
            [JsonStringEnumMemberName("Office Drawer Right")]
            OfficeDrawerRight,
            [JsonStringEnumMemberName("Pantry Door (Left)")]
            PantryDoorLeft,
            [JsonStringEnumMemberName("Pantry Door (Right)")]
            PantryDoorRight,
            [JsonStringEnumMemberName("Safe")]
            Safe,
            [JsonStringEnumMemberName("Slider Door")]
            SliderDoor,
            [JsonStringEnumMemberName("Small Spare Closet Door (L)")]
            SmallSpareClosetDoorL,
            [JsonStringEnumMemberName("Small Spare Closet Door (R)")]
            SmallSpareClosetDoorR,
            [JsonStringEnumMemberName("Spare Room 2 Door")]
            SpareRoom2Door,
            [JsonStringEnumMemberName("Spare Room Door")]
            SpareRoomDoor,
            [JsonStringEnumMemberName("SRClosetDoor1")]
            SRClosetDoor1,
            [JsonStringEnumMemberName("SRClosetDoor2")]
            SRClosetDoor2,
            [JsonStringEnumMemberName("SRClosetDoor3")]
            SRClosetDoor3,
            [JsonStringEnumMemberName("SRClosetDoor4")]
            SRClosetDoor4,
            [JsonStringEnumMemberName("Study Closet Door (L)")]
            StudyClosetDoorL,
            [JsonStringEnumMemberName("Study Closet Door (R)")]
            StudyClosetDoorR,
            [JsonStringEnumMemberName("Study Door")]
            StudyDoor,
            [JsonStringEnumMemberName("Upstairs Bathroom Door")]
            UpstairsBathroomDoor,
            [JsonStringEnumMemberName("Utility Closet Door")]
            UtilityClosetDoor,
            //burnables
            [JsonStringEnumMemberName("Chicken")]
            Chicken,
            [JsonStringEnumMemberName("Disembodied Head")]
            DisembodiedHead,
            [JsonStringEnumMemberName("Foam Finger")]
            FoamFinger,
            [JsonStringEnumMemberName("Penguin")]
            Pengiun,
            //minigame spots
            PlayerBeerPongPractice,
            //sexspots
            BedEdge,
            CompuBrahSex,
            //hot tub seats
            [JsonStringEnumMemberName("HotTub Seat 1")]
            HotTubSeat1,
            [JsonStringEnumMemberName("HotTub Seat 2")]
            HotTubSeat2,
            [JsonStringEnumMemberName("HotTub Seat 3")]
            HotTubSeat3,
            [JsonStringEnumMemberName("HotTub Seat 4")]
            HotTubSeat4,
            [JsonStringEnumMemberName("HotTub Seat 5")]
            HotTubSeat5,
            //rest/specialtype:0
            [JsonStringEnumMemberName("AC Unit")]
            ACUnit,
            [JsonStringEnumMemberName("Airvent ArtRoom")]
            AirventArtRoom,
            [JsonStringEnumMemberName("Airvent DiningRoom")]
            AirventDiningRoom,
            [JsonStringEnumMemberName("Airvent DownstairsGuestBathroom")]
            AirventDownstairsGuestBathroom,
            [JsonStringEnumMemberName("Airvent GarageWall")]
            AirventGarageWall,
            [JsonStringEnumMemberName("Airvent Kitchen")]
            AirventKitchen,
            [JsonStringEnumMemberName("Airvent LaundryRoom")]
            AirventLaundryRoom,
            [JsonStringEnumMemberName("Airvent LivingRoom")]
            AirventLivingRoom,
            [JsonStringEnumMemberName("Airvent MasterBathroom")]
            AirventMasterBathroom,
            [JsonStringEnumMemberName("Airvent MasterBedroom")]
            AirventMasterBedroom,
            [JsonStringEnumMemberName("Airvent SpareRoom")]
            AirventSpareRoom,
            [JsonStringEnumMemberName("Airvent Study")]
            AirventStudy,
            [JsonStringEnumMemberName("Airvent UpstairsGuestBathroom")]
            AirventUpstairsGuestBathroom,
            [JsonStringEnumMemberName("Apartment Computer")]
            ApartmentComputer,
            [JsonStringEnumMemberName("ArtRoomWindow")]
            ArtRoomWindow,
            [JsonStringEnumMemberName("Ashley's Panties")]
            AshleysPanties,
            [JsonStringEnumMemberName("AshleyTop")]
            AshleyTop,
            [JsonStringEnumMemberName("Auxiliary Dock")]
            AuxiliaryDock,
            [JsonStringEnumMemberName("Battery Pack")]
            BatteryPack,
            [JsonStringEnumMemberName("BeerPongCupTrigger0")]
            BeerPongCupTrigger0,
            [JsonStringEnumMemberName("BikeLock")]
            BikeLock,
            [JsonStringEnumMemberName("BikeLockKey")]
            BikeLockKey,
            [JsonStringEnumMemberName("Billboard")]
            Billboard,
            [JsonStringEnumMemberName("BloodWall")]
            BloodWall,
            [JsonStringEnumMemberName("Blue Hair Dye")]
            BlueHairDye,
            [JsonStringEnumMemberName("Blueprints")]
            Blueprints,
            [JsonStringEnumMemberName("Box of Nails")]
            BoxofNails,
            [JsonStringEnumMemberName("Breaker Panel")]
            BreakerPanel,
            [JsonStringEnumMemberName("Briefcase")]
            Briefcase,
            [JsonStringEnumMemberName("BriefcaseOpen")]
            BriefcaseOpen,
            [JsonStringEnumMemberName("Broom")]
            Broom,
            [JsonStringEnumMemberName("Bug Zapper")]
            BugZapper,
            [JsonStringEnumMemberName("Bushes")]
            Bushes,
            [JsonStringEnumMemberName("Cabernet")]
            Cabernet,
            [JsonStringEnumMemberName("Camera")]
            Camera,
            [JsonStringEnumMemberName("CatPicture")]
            CatPicture,
            [JsonStringEnumMemberName("Cell Phone Jammer")]
            CellPhoneJammer,
            [JsonStringEnumMemberName("Chardonnay")]
            Chardonnay,
            [JsonStringEnumMemberName("Chicken Nuggets")]
            ChickenNuggets,
            [JsonStringEnumMemberName("Chili Peppers")]
            ChiliPeppers,
            [JsonStringEnumMemberName("Choclate Syrup")]
            ChoclateSyrup,
            [JsonStringEnumMemberName("ChocolateBar")]
            ChocolateBar,
            [JsonStringEnumMemberName("ChristmasGazeboStar")]
            ChristmasGazeboStar,
            [JsonStringEnumMemberName("Clear Eyes")]
            ClearEyes,
            [JsonStringEnumMemberName("Closed Briefcase")]
            ClosedBriefcase,
            [JsonStringEnumMemberName("Clothes")]
            Clothes,
            [JsonStringEnumMemberName("Clothes Basket 1")]
            ClothesBasket1,
            [JsonStringEnumMemberName("Clothes Basket 2f")]
            ClothesBasket2f,
            [JsonStringEnumMemberName("Clothes Basket 3")]
            ClothesBasket3,
            [JsonStringEnumMemberName("Coffee")]
            Coffee,
            [JsonStringEnumMemberName("Collar")]
            Collar,
            [JsonStringEnumMemberName("CollarBloody")]
            CollarBloody,
            [JsonStringEnumMemberName("Computer")]
            Computer,
            [JsonStringEnumMemberName("Condom")]
            Condom,
            [JsonStringEnumMemberName("Cowboy Scorpiosmus")]
            CowboyScorpiosmus,
            [JsonStringEnumMemberName("Credit Card")]
            CreditCard,
            [JsonStringEnumMemberName("Crowd")]
            Crowd,
            [JsonStringEnumMemberName("Cucumber")]
            Cucumber,
            [JsonStringEnumMemberName("Dark Long Sleeve Women's Top")]
            DarkLongSleeveWomensTop,
            [JsonStringEnumMemberName("DeadSnake")]
            DeadSnake,
            [JsonStringEnumMemberName("Derek's Shirt")]
            DereksShirt,
            [JsonStringEnumMemberName("Diary")]
            Diary,
            [JsonStringEnumMemberName("DirtMound")]
            DirtMound,
            [JsonStringEnumMemberName("DirtyClue1")]
            DirtyClue1,
            [JsonStringEnumMemberName("DirtyClue2")]
            DirtyClue2,
            [JsonStringEnumMemberName("DirtyClue3")]
            DirtyClue3,
            [JsonStringEnumMemberName("Discarded Magazine")]
            DiscardedMagazine,
            [JsonStringEnumMemberName("DLiciousOutfit")]
            DLiciousOutfit,
            [JsonStringEnumMemberName("DoorBracingSpot")]
            DoorBracingSpot,
            [JsonStringEnumMemberName("DoorBracingSpot2")]
            DoorBracingSpot2,
            [JsonStringEnumMemberName("Double-Sided Dildo")]
            DoubleSidedDildo,
            [JsonStringEnumMemberName("DownstairsBathroomMirror")]
            DownstairsBathroomMirror,
            [JsonStringEnumMemberName("Dresser")]
            Dresser,
            [JsonStringEnumMemberName("Drone")]
            Drone,
            [JsonStringEnumMemberName("Drone Remote")]
            DroneRemote,
            [JsonStringEnumMemberName("Dryer")]
            Dryer,
            [JsonStringEnumMemberName("DryerStuckSpot")]
            DryerStuckSpot,
            [JsonStringEnumMemberName("DryerStuckSpotImmediate")]
            DryerStuckSpotImmediate,
            [JsonStringEnumMemberName("Duct Tape")]
            DuctTape,
            [JsonStringEnumMemberName("DVD2")]
            DVD2,
            [JsonStringEnumMemberName("E-Cig Vape")]
            ECigVape,
            [JsonStringEnumMemberName("Eggs")]
            Eggs,
            [JsonStringEnumMemberName("Empty Cup")]
            EmptyCup,
            [JsonStringEnumMemberName("EtherealClue")]
            EtherealClue,
            [JsonStringEnumMemberName("Face Cream")]
            FaceCream,
            [JsonStringEnumMemberName("Faucet")]
            Faucet,
            [JsonStringEnumMemberName("Fifty Dollars")]
            FiftyDollars,
            [JsonStringEnumMemberName("Film Camera")]
            FilmCamera,
            [JsonStringEnumMemberName("Fire Pit")]
            FirePit,
            [JsonStringEnumMemberName("Fireplace")]
            Fireplace,
            [JsonStringEnumMemberName("Firetrail")]
            Firetrail,
            [JsonStringEnumMemberName("FlamingoLeftA")]
            FlamingoLeftA,
            [JsonStringEnumMemberName("FlamingoRightB")]
            FlamingoRightB,
            [JsonStringEnumMemberName("Flask")]
            Flask,
            [JsonStringEnumMemberName("Floor Towels")]
            FloorTowels,
            [JsonStringEnumMemberName("Flower")]
            Flower,
            [JsonStringEnumMemberName("FreeSnake")]
            FreeSnake,
            [JsonStringEnumMemberName("FreeSnakeFemale")]
            FreeSnakeFemale,
            [JsonStringEnumMemberName("FrontWindowBracingSpot")]
            FrontWindowBracingSpot,
            [JsonStringEnumMemberName("FrontWindowBracingSpot2")]
            FrontWindowBracingSpot2,
            [JsonStringEnumMemberName("FrontWindowBracingSpot3")]
            FrontWindowBracingSpot3,
            [JsonStringEnumMemberName("GarageBinMovable")]
            GarageBinMovable,
            [JsonStringEnumMemberName("GarageWall Airvent Cover")]
            GarageWallAirventCover,
            [JsonStringEnumMemberName("Gastronomy Book")]
            GastronomyBook,
            [JsonStringEnumMemberName("Gazebo")]
            Gazebo,
            [JsonStringEnumMemberName("GazeboFuseBox")]
            GazeboFuseBox,
            [JsonStringEnumMemberName("Glasses")]
            Glasses,
            [JsonStringEnumMemberName("Gut Grip")]
            GutGrip,
            [JsonStringEnumMemberName("Gutter")]
            Gutter,
            [JsonStringEnumMemberName("Hammer")]
            Hammer,
            [JsonStringEnumMemberName("Hammer2")]
            Hammer2,
            [JsonStringEnumMemberName("HangingTentacle")]
            HangingTentacle,
            [JsonStringEnumMemberName("Hedge Clippers")]
            HedgeClippers,
            [JsonStringEnumMemberName("HoleInFence")]
            HoleInFence,
            [JsonStringEnumMemberName("Hot Tub Base")]
            HotTubBase,
            [JsonStringEnumMemberName("HotAirBalloon")]
            HotAirBalloon,
            [JsonStringEnumMemberName("HotTubCoverFolded")]
            HotTubCoverFolded,
            [JsonStringEnumMemberName("HotTubCoverUnfolded")]
            HotTubCoverUnfolded,
            [JsonStringEnumMemberName("Jack Daniel's")]
            JackDaniels,
            [JsonStringEnumMemberName("Joint")]
            Joint,
            [JsonStringEnumMemberName("Katana")]
            Katana,
            [JsonStringEnumMemberName("Kettle")]
            Kettle,
            [JsonStringEnumMemberName("Key")]
            Key,
            [JsonStringEnumMemberName("Key2")]
            Key2,
            [JsonStringEnumMemberName("Key3")]
            Key3,
            [JsonStringEnumMemberName("Keychain")]
            Keychain,
            [JsonStringEnumMemberName("Knife")]
            Knife,
            [JsonStringEnumMemberName("Laptop")]
            Laptop,
            [JsonStringEnumMemberName("LeahEarpiece")]
            LeahEarpiece,
            [JsonStringEnumMemberName("LivingRoomTV")]
            LivingRoomTV,
            [JsonStringEnumMemberName("LizKatzButt")]
            LizKatzButt,
            [JsonStringEnumMemberName("LRTVBackButton")]
            LRTVBackButton,
            [JsonStringEnumMemberName("LRTVPlayAllButton")]
            LRTVPlayAllButton,
            [JsonStringEnumMemberName("LRTVSongFiveButton")]
            LRTVSongFiveButton,
            [JsonStringEnumMemberName("LRTVSongFourButton")]
            LRTVSongFourButton,
            [JsonStringEnumMemberName("LRTVSongOneButton")]
            LRTVSongOneButton,
            [JsonStringEnumMemberName("LRTVSongSixButton")]
            LRTVSongSixButton,
            [JsonStringEnumMemberName("LRTVSongThreeButton")]
            LRTVSongThreeButton,
            [JsonStringEnumMemberName("LRTVSongTwoButton")]
            LRTVSongTwoButton,
            [JsonStringEnumMemberName("Madison's Phone")]
            MadisonsPhone,
            [JsonStringEnumMemberName("Marijuana")]
            Marijuana,
            [JsonStringEnumMemberName("Matchbox")]
            Matchbox,
            [JsonStringEnumMemberName("Mayonnaise")]
            Mayonnaise,
            [JsonStringEnumMemberName("Merlot")]
            Merlot,
            [JsonStringEnumMemberName("Microwave")]
            Microwave,
            [JsonStringEnumMemberName("MistletoeBallsDecoration")]
            MistletoeBallsDecoration,
            [JsonStringEnumMemberName("Money Makers Monthly")]
            MoneyMakersMonthly,
            [JsonStringEnumMemberName("Motor Oil")]
            MotorOil,
            [JsonStringEnumMemberName("MP3 Player")]
            MP3Player,
            [JsonStringEnumMemberName("MP3 Player Cable")]
            MP3PlayerCable,
            [JsonStringEnumMemberName("MP3 Player DC")]
            MP3PlayerDC,
            [JsonStringEnumMemberName("Mug")]
            Mug,
            [JsonStringEnumMemberName("Natty Lite 1")]
            NattyLite1,
            [JsonStringEnumMemberName("Natty Lite 10")]
            NattyLite10,
            [JsonStringEnumMemberName("Natty Lite 11")]
            NattyLite11,
            [JsonStringEnumMemberName("Natty Lite 12")]
            NattyLite12,
            [JsonStringEnumMemberName("Natty Lite 13")]
            NattyLite13,
            [JsonStringEnumMemberName("Natty Lite 14")]
            NattyLite14,
            [JsonStringEnumMemberName("Natty Lite 2")]
            NattyLite2,
            [JsonStringEnumMemberName("Natty Lite 3")]
            NattyLite3,
            [JsonStringEnumMemberName("Natty Lite 4")]
            NattyLite4,
            [JsonStringEnumMemberName("Natty Lite 5")]
            NattyLite5,
            [JsonStringEnumMemberName("Natty Lite 6")]
            NattyLite6,
            [JsonStringEnumMemberName("Natty Lite 7")]
            NattyLite7,
            [JsonStringEnumMemberName("Natty Lite 8")]
            NattyLite8,
            [JsonStringEnumMemberName("Natty Lite 9")]
            NattyLite9,
            [JsonStringEnumMemberName("Natty Lite Open")]
            NattyLiteOpen,
            [JsonStringEnumMemberName("Nerdy Taped Glasses")]
            NerdyTapedGlasses,
            [JsonStringEnumMemberName("Notebook")]
            Notebook,
            [JsonStringEnumMemberName("Notepad")]
            Notepad,
            [JsonStringEnumMemberName("Open Briefcase")]
            OpenBriefcase,
            [JsonStringEnumMemberName("OrangeBin1")]
            OrangeBin1,
            [JsonStringEnumMemberName("OrganizingBriefcaseSpot")]
            OrganizingBriefcaseSpot,
            [JsonStringEnumMemberName("OwlPainting")]
            OwlPainting,
            [JsonStringEnumMemberName("PaddockButton")]
            PaddockButton,
            [JsonStringEnumMemberName("Painkillers")]
            Painkillers,
            [JsonStringEnumMemberName("Paper")]
            Paper,
            [JsonStringEnumMemberName("Paper Bag")]
            PaperBag,
            [JsonStringEnumMemberName("Paper Bag Crunched")]
            PaperBagCrunched,
            [JsonStringEnumMemberName("Pencil")]
            Pencil,
            [JsonStringEnumMemberName("Petition")]
            Petition,
            [JsonStringEnumMemberName("Phone2")]
            Phone2,
            [JsonStringEnumMemberName("Phone3")]
            Phone3,
            [JsonStringEnumMemberName("Phone4")]
            Phone4,
            [JsonStringEnumMemberName("Phone5")]
            Phone5,
            [JsonStringEnumMemberName("Phone6")]
            Phone6,
            [JsonStringEnumMemberName("PingPongBall")]
            PingPongBall,
            [JsonStringEnumMemberName("PinkUnderwear")]
            PinkUnderwear,
            [JsonStringEnumMemberName("PipeFix")]
            PipeFix,
            [JsonStringEnumMemberName("PipeWater")]
            PipeWater,
            [JsonStringEnumMemberName("Pizza Box")]
            PizzaBox,
            [JsonStringEnumMemberName("PizzaBoxFortification")]
            PizzaBoxFortification,
            [JsonStringEnumMemberName("PlankPileInYard")]
            PlankPileInYard,
            [JsonStringEnumMemberName("PlanksFortification")]
            PlanksFortification,
            [JsonStringEnumMemberName("PlanksFortificationExtraNails")]
            PlanksFortificationExtraNails,
            [JsonStringEnumMemberName("PlexiglassHotTub")]
            PlexiglassHotTub,
            [JsonStringEnumMemberName("PlexiglassStacked")]
            PlexiglassStacked,
            [JsonStringEnumMemberName("Popcorn")]
            Popcorn,
            [JsonStringEnumMemberName("Popper")]
            Popper,
            [JsonStringEnumMemberName("PopperBox")]
            PopperBox,
            [JsonStringEnumMemberName("Potatoes")]
            Potatoes,
            [JsonStringEnumMemberName("PrintableClue")]
            PrintableClue,
            [JsonStringEnumMemberName("Printer")]
            Printer,
            [JsonStringEnumMemberName("Red Solo Cup 0")]
            RedSoloCup0,
            [JsonStringEnumMemberName("Red Solo Cup 1")]
            RedSoloCup1,
            [JsonStringEnumMemberName("Red Solo Cup 2")]
            RedSoloCup2,
            [JsonStringEnumMemberName("Red Solo Cup 3")]
            RedSoloCup3,
            [JsonStringEnumMemberName("Red Solo Cup 4")]
            RedSoloCup4,
            [JsonStringEnumMemberName("Red Solo Cup 5")]
            RedSoloCup5,
            [JsonStringEnumMemberName("Red Solo Cup 6")]
            RedSoloCup6,
            [JsonStringEnumMemberName("Red Solo Cup 7")]
            RedSoloCup7,
            [JsonStringEnumMemberName("Red Solo Cup 8")]
            RedSoloCup8,
            [JsonStringEnumMemberName("Red Solo Cup 9")]
            RedSoloCup9,
            [JsonStringEnumMemberName("Remote Control")]
            RemoteControl,
            [JsonStringEnumMemberName("RipppedWhalePoster")]
            RipppedWhalePoster,
            [JsonStringEnumMemberName("Router")]
            Router,
            [JsonStringEnumMemberName("Rum")]
            Rum,
            [JsonStringEnumMemberName("RumOpen")]
            RumOpen,
            [JsonStringEnumMemberName("Salami")]
            Salami,
            [JsonStringEnumMemberName("Salmon")]
            Salmon,
            [JsonStringEnumMemberName("Satchel")]
            Satchel,
            [JsonStringEnumMemberName("Scorpion Tequila")]
            ScorpionTequila,
            [JsonStringEnumMemberName("SD Card")]
            SDCard,
            [JsonStringEnumMemberName("Shake Light")]
            ShakeLight,
            [JsonStringEnumMemberName("SleepingMaskFolded")]
            SleepingMaskFolded,
            [JsonStringEnumMemberName("SleepingMaskMountable")]
            SleepingMaskMountable,
            [JsonStringEnumMemberName("Snowball")]
            Snowball,
            [JsonStringEnumMemberName("SnowballBucket")]
            SnowballBucket,
            [JsonStringEnumMemberName("Soda")]
            Soda,
            [JsonStringEnumMemberName("SoloBeerPongTable")]
            SoloBeerPongTable,
            [JsonStringEnumMemberName("SoloPingPongBallStorage")]
            SoloPingPongBallStorage,
            [JsonStringEnumMemberName("Speaker1")]
            Speaker1,
            [JsonStringEnumMemberName("Speaker2")]
            Speaker2,
            [JsonStringEnumMemberName("Spoon")]
            Spoon,
            [JsonStringEnumMemberName("SprayPaintCan")]
            SprayPaintCan,
            [JsonStringEnumMemberName("Starbomb")]
            Starbomb,
            [JsonStringEnumMemberName("StarbombBox")]
            StarbombBox,
            [JsonStringEnumMemberName("Stepstool")]
            Stepstool,
            [JsonStringEnumMemberName("StepstoolStandSpot")]
            StepstoolStandSpot,
            [JsonStringEnumMemberName("Stove")]
            Stove,
            [JsonStringEnumMemberName("StudyLaptopTypingSpot")]
            StudyLaptopTypingSpot,
            [JsonStringEnumMemberName("Sundress")]
            Sundress,
            [JsonStringEnumMemberName("Sunglasses")]
            Sunglasses,
            [JsonStringEnumMemberName("Sweeties")]
            Sweeties,
            [JsonStringEnumMemberName("Tablet")]
            Tablet,
            [JsonStringEnumMemberName("Talking Fish")]
            TalkingFish,
            [JsonStringEnumMemberName("Tentacle")]
            Tentacle,
            [JsonStringEnumMemberName("Terrarium")]
            Terrarium,
            [JsonStringEnumMemberName("Thermos")]
            Thermos,
            [JsonStringEnumMemberName("Thermostat")]
            Thermostat,
            [JsonStringEnumMemberName("Toaster")]
            Toaster,
            [JsonStringEnumMemberName("Towel")]
            Towel,
            [JsonStringEnumMemberName("Towel2")]
            Towel2,
            [JsonStringEnumMemberName("Towel3")]
            Towel3,
            [JsonStringEnumMemberName("Trash Can")]
            TrashCan,
            [JsonStringEnumMemberName("Trunk")]
            Trunk,
            [JsonStringEnumMemberName("Undressing Neighbor")]
            UndressingNeighbor,
            [JsonStringEnumMemberName("UpstairsMasterBathroomMirror")]
            UpstairsMasterBathroomMirror,
            [JsonStringEnumMemberName("USB Stick")]
            USBStick,
            [JsonStringEnumMemberName("Valentines Chocolates")]
            ValentinesChocolates,
            [JsonStringEnumMemberName("Vibrator")]
            Vibrator,
            [JsonStringEnumMemberName("Vickie's Panties")]
            VickiesPanties,
            [JsonStringEnumMemberName("Vodka")]
            Vodka,
            [JsonStringEnumMemberName("VoiceRecorder")]
            VoiceRecorder,
            [JsonStringEnumMemberName("Washer")]
            Washer,
            [JsonStringEnumMemberName("WaterMain")]
            WaterMain,
            [JsonStringEnumMemberName("WhalePoster")]
            WhalePoster,
            [JsonStringEnumMemberName("Whipped Cream")]
            WhippedCream,
            [JsonStringEnumMemberName("Wine Rack")]
            WineRack,
            [JsonStringEnumMemberName("Wrench")]
            Wrench,
            [JsonStringEnumMemberName("YouTool")]
            YouTool,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Cutscenes
        {
            ACExplosion_A,
            ACExplosion_M,
            ACExplosion_MA,
            AmyPlayerBedroomSex,
            AshleyFinale,
            AshleyFinale_c,
            AshleySupporter,
            AshleySupporter_c,
            DC_FenceEvent,
            DC_FenceEvent_Trailer,
            DerekFinale,
            DerekFinale_c,
            DerekSupporter,
            DerekSupporter_c,
            DojaFinale,
            DojaPerformanceCompleteFail,
            DojaPerformanceDroneFail,
            DojaPerformanceGood,
            DojaPerformanceHotTubFail,
            DojaPerformanceLive,
            DojaPerformancePatFail,
            F_AmyBrittney_AshleyRoom_Voyeur,
            F_AmyBrittney_AshleyRoom_Voyeur_c,
            F_AshleyFinale,
            F_AshleyFinale_c,
            F_AshleySupporter,
            F_AshleySupporter_c,
            F_BrittneyFinale,
            F_BrittneyFinale_c,
            F_DerekFinale,
            F_DerekFinale_c,
            F_DerekSupporter,
            F_DerekSupporter_c,
            F_DojaFinale,
            F_FrankFinale,
            F_FrankFinale_c,
            F_FridgeReveal,
            F_KatherineFinale,
            F_KatherineFinale_c,
            F_LizFinale,
            F_LizFinale_c,
            F_LizVR,
            F_LizVR_c,
            F_LizWashroom,
            F_LizWashroom_c,
            F_MP3Reach,
            F_PatrickFinale,
            F_PatrickFinale_c,
            F_PlayerGarageSex,
            F_PlayerGarageSex_c,
            F_PlayerMasterBedroomSex_Straight,
            F_PlayerMasterBedroomSex_Straight_c,
            F_PlayerMasterBedroomSex_Straight1,
            F_PlayerMasterBedroomSex1,
            F_PlayerMasterBedroomSex1_c,
            F_RachaelFinale,
            F_RachaelFinale_c,
            F_TentacleMastSeat1,
            F_TentacleMastSeat2,
            F_TentacleMastSeat3,
            F_TentacleMastSeat4,
            F_ThreesomeFFF_AshleyRoom,
            F_ThreesomeFFF_AshleyRoom_c,
            F_ThreesomeFFM_MasterBedroom,
            F_ThreesomeFFM_MasterBedroom_c,
            F_VickieFinale,
            F_VickieFinale_c,
            FrankBlowjob,
            FrankBlowjob_c,
            FrankFinale,
            FrankFinale_c,
            FridgeReveal,
            GenericSpareRoom,
            Intro_F,
            Intro_M,
            KatherineFinale,
            KatherineFinale_c,
            LizChanging,
            LizChanging_c,
            LizEntry,
            LizFinale,
            LizFinale_c,
            LizTentacle,
            LizTentacle_c,
            LizVR,
            LizVR_c,
            LizWashroom,
            LizWashroom_c,
            MP3Reach,
            Patjump,
            PlayerGarageSex,
            PlayerGarageSex_c,
            PlayerMasterBedroomSex1,
            PlayerMasterBedroomSex1_c,
            RachaelFinale,
            RachaelFinale_c,
            TentacleMastSeat1,
            TentacleMastSeat2,
            TentacleMastSeat3,
            TentacleMastSeat4,
            TentacleReveal,
            ThreesomeFFM_AshleyRoom,
            ThreesomeFFM_AshleyRoom_c,
            ThreesomeFFM_EasterEgg,
            ThreesomeFFM_EasterEgg_c,
            ThreesomeFFM_Gazebo,
            ThreesomeFFM_Gazebo_c,
            ThreesomeFFM_MasterBedroom,
            ThreesomeFFM_MasterBedroom_c,
            ThreesomeMMF_1_Gazebo,
            ThreesomeMMF_1_Gazebo_c,
            ThreesomeMMF_ArtRoom,
            ThreesomeMMF_ArtRoom_c,
            VentRelease,
            VickieFinale,
            VickieFinale_c,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TriggerOptions
        {
            PerformEvent,
            SetEnabled
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum WalkToTargetOptions
        {
            MoveTarget,
            Character,
            Item,
            Cancel
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SupportedClipNames
        {
            DunDunDUNNNnnn,
            CompubrahGood1,
            CompubrahGood2,
            CompubrahGood3,
            CompubrahGood4,
            CompubrahGood5,
            CompubrahGoodFinal,
            CompubrahBad1,
            CompubrahBad2,
            CompubrahBad3,
            CompubrahBad4,
            CompubrahBad5,
            DrywallThud,
            FabricRipv2,
            GlassBreakv2,
            TrashKnockedOver1,
            TrashKnockedOver2,
            Zipper,
            [JsonStringEnumMemberName("033737654-signature-4")]
            _033737654signature4,
            LKdoorbell_final
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Emotions
        {
            Happy,
            Sad,
            Angry,
            Surprised,
            Scared,
            Flirty,
            Ecstatic,
            Laugh,
            OpenMouth,
            Squint,
            None,
            EyebrowsUp,
            EyebrowsIn,
            ClosedEyes,
            PuckeredLips,
            BreatheIn,
            OpenWide,
            Erection = 200,
            OpenLabia,
            HappyAlt1 = 300
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum BodyRegion
        {
            Anywhere,
            Head = 10,
            Torso = 20,
            Arms = 30,
            Legs = 40,
            Feet = 50,
            Hands = 60
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ClothingTypeOrName
        {
            ByName,
            ByType
        }

        //sepcialtype: 7
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Burnables
        {
            [JsonStringEnumMemberName("Chicken")]
            Chicken,
            [JsonStringEnumMemberName("Disembodied Head")]
            DisembodiedHead,
            [JsonStringEnumMemberName("Foam Finger")]
            FoamFinger,
            [JsonStringEnumMemberName("Penguin")]
            Pengiun,
        }

        //sepcialtype: 6
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum MiniGameSpot
        {
            PlayerBeerPongPractice,
        }

        //sepcialtype: 5
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SexSpot
        {
            BedEdge,
            CompuBrahSex,
        }

        //sepcialtype: 4
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum HotTubSeats
        {
            [JsonStringEnumMemberName("HotTub Seat 1")]
            HotTubSeat1,
            [JsonStringEnumMemberName("HotTub Seat 2")]
            HotTubSeat2,
            [JsonStringEnumMemberName("HotTub Seat 3")]
            HotTubSeat3,
            [JsonStringEnumMemberName("HotTub Seat 4")]
            HotTubSeat4,
            [JsonStringEnumMemberName("HotTub Seat 5")]
            HotTubSeat5,
        }

        //specialtype: 3
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Beds
        {
            [JsonStringEnumMemberName("Bed (Left)")]
            BedLeft,
            [JsonStringEnumMemberName("Bed (Right)")]
            BedRight,
            [JsonStringEnumMemberName("Compubrah Bed(Left)")]
            CompubrahBedLeft,
            [JsonStringEnumMemberName("Compubrah Bed(Right)")]
            CompubrahBedRight,
        }

        //speacialtype : 2
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Chairs
        {
            [JsonStringEnumMemberName("Apartment Sofa Seat (Left)")]
            ApartmentSofaSeatLeft,
            [JsonStringEnumMemberName("Apartment Sofa Seat (Right)")]
            ApartmentSofaSeatRight,
            [JsonStringEnumMemberName("Armchair")]
            Armchair,
            [JsonStringEnumMemberName("Computer Chair")]
            ComputerChair,
            [JsonStringEnumMemberName("Couch (Left)")]
            CouchLeft,
            [JsonStringEnumMemberName("Couch (Middle)")]
            CouchMiddle,
            [JsonStringEnumMemberName("Couch (Right)")]
            CouchRight,
            [JsonStringEnumMemberName("DownstairsToilet")]
            DownstairsToilet,
            [JsonStringEnumMemberName("DryerSeat")]
            DryerSeat,
            [JsonStringEnumMemberName("FlamingoSeatLeft")]
            FlamingoSeatLeft,
            [JsonStringEnumMemberName("FlamingoSeatRight")]
            FlamingoSeatRight,
            [JsonStringEnumMemberName("Frank's Chair")]
            FranksChair,
            [JsonStringEnumMemberName("LawnChair1")]
            LawnChair1,
            [JsonStringEnumMemberName("LawnChair2")]
            LawnChair2,
            [JsonStringEnumMemberName("LawnChair3")]
            LawnChair3,
            [JsonStringEnumMemberName("LawnChair4")]
            LawnChair4,
            [JsonStringEnumMemberName("MasterBathTubEdgeSeat")]
            MasterBathTubEdgeSeat,
            [JsonStringEnumMemberName("MasterBathTubSeat")]
            MasterBathTubSeat,
            [JsonStringEnumMemberName("Office Chair")]
            OfficeChair,
            [JsonStringEnumMemberName("Outside Sofa 1 Left")]
            OutsideSofa1Left,
            [JsonStringEnumMemberName("Outside Sofa 1 Middle")]
            OutsideSofa1Middle,
            [JsonStringEnumMemberName("Outside Sofa 1 Right")]
            OutsideSofa1Right,
            [JsonStringEnumMemberName("Outside Sofa 2 Left")]
            OutsideSofa2Left,
            [JsonStringEnumMemberName("Outside Sofa 2 Middle")]
            OutsideSofa2Middle,
            [JsonStringEnumMemberName("Outside Sofa 2 Right")]
            OutsideSofa2Right,
            [JsonStringEnumMemberName("Patio Armchair")]
            PatioArmchair,
            [JsonStringEnumMemberName("Patio Armchair 2")]
            PatioArmchair2,
            [JsonStringEnumMemberName("Pouf")]
            Pouf,
            [JsonStringEnumMemberName("RoofSeatLeft")]
            RoofSeatLeft,
            [JsonStringEnumMemberName("RoofSeatRight")]
            RoofSeatRight,
            [JsonStringEnumMemberName("Sofa (Left)")]
            SofaLeft,
            [JsonStringEnumMemberName("Sofa (Right)")]
            SofaRight,
            [JsonStringEnumMemberName("SpareRoomSofaLeft")]
            SpareRoomSofaLeft,
            [JsonStringEnumMemberName("SpareRoomSofaMiddle")]
            SpareRoomSofaMiddle,
            [JsonStringEnumMemberName("SpareRoomSofaRight")]
            SpareRoomSofaRight,
            [JsonStringEnumMemberName("UpstairsBathroomToilet")]
            UpstairsBathroomToilet,
            [JsonStringEnumMemberName("UpstairsBathTubEdgeSeat")]
            UpstairsBathTubEdgeSeat,
            [JsonStringEnumMemberName("UpstairsBathTubSeat")]
            UpstairsBathTubSeat,
            [JsonStringEnumMemberName("Vanity Chair")]
            VanityChair,
            [JsonStringEnumMemberName("WasherSeat")]
            WasherSeat,
        }

        //specialtype:1
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Doors
        {
            [JsonStringEnumMemberName("Bathroom Door")]
            BathroomDoor,
            [JsonStringEnumMemberName("Bedroom Closet Door (Left)")]
            BedroomClosetDoorLeft,
            [JsonStringEnumMemberName("Bedroom Closet Door (Right)")]
            BedroomClosetDoorRight,
            [JsonStringEnumMemberName("CabinetLeft")]
            CabinetLeft,
            [JsonStringEnumMemberName("CabinetRight")]
            CabinetRight,
            [JsonStringEnumMemberName("Desk Drawer Left")]
            DeskDrawerLeft,
            [JsonStringEnumMemberName("Desk Drawer Right")]
            DeskDrawerRight,
            [JsonStringEnumMemberName("Dryer Door")]
            DryerDoor,
            [JsonStringEnumMemberName("Escape Hatch")]
            EscapeHatch,
            [JsonStringEnumMemberName("Freezer")]
            Freezer,
            [JsonStringEnumMemberName("Fridge")]
            Fridge,
            [JsonStringEnumMemberName("Front Door")]
            FrontDoor,
            [JsonStringEnumMemberName("Garage Door")]
            GarageDoor,
            [JsonStringEnumMemberName("KitchenCabinet5")]
            KitchenCabinet5,
            [JsonStringEnumMemberName("KitchenCabinet6")]
            KitchenCabinet6,
            [JsonStringEnumMemberName("KitchenCabinet7")]
            KitchenCabinet7,
            [JsonStringEnumMemberName("KitchenCabinet8")]
            KitchenCabinet8,
            [JsonStringEnumMemberName("Laundry Room Door")]
            LaundryRoomDoor,
            [JsonStringEnumMemberName("Master Bathroom Door")]
            MasterBathroomDoor,
            [JsonStringEnumMemberName("Master Bedroom Door")]
            MasterBedroomDoor,
            [JsonStringEnumMemberName("Microwave Door")]
            MicrowaveDoor,
            [JsonStringEnumMemberName("Nightstand1")]
            Nightstand1,
            [JsonStringEnumMemberName("Nightstand2")]
            Nightstand2,
            [JsonStringEnumMemberName("Office Drawer Left")]
            OfficeDrawerLeft,
            [JsonStringEnumMemberName("Office Drawer Right")]
            OfficeDrawerRight,
            [JsonStringEnumMemberName("Pantry Door (Left)")]
            PantryDoorLeft,
            [JsonStringEnumMemberName("Pantry Door (Right)")]
            PantryDoorRight,
            [JsonStringEnumMemberName("Safe")]
            Safe,
            [JsonStringEnumMemberName("Slider Door")]
            SliderDoor,
            [JsonStringEnumMemberName("Small Spare Closet Door (L)")]
            SmallSpareClosetDoorL,
            [JsonStringEnumMemberName("Small Spare Closet Door (R)")]
            SmallSpareClosetDoorR,
            [JsonStringEnumMemberName("Spare Room 2 Door")]
            SpareRoom2Door,
            [JsonStringEnumMemberName("Spare Room Door")]
            SpareRoomDoor,
            [JsonStringEnumMemberName("SRClosetDoor1")]
            SRClosetDoor1,
            [JsonStringEnumMemberName("SRClosetDoor2")]
            SRClosetDoor2,
            [JsonStringEnumMemberName("SRClosetDoor3")]
            SRClosetDoor3,
            [JsonStringEnumMemberName("SRClosetDoor4")]
            SRClosetDoor4,
            [JsonStringEnumMemberName("Study Closet Door (L)")]
            StudyClosetDoorL,
            [JsonStringEnumMemberName("Study Closet Door (R)")]
            StudyClosetDoorR,
            [JsonStringEnumMemberName("Study Door")]
            StudyDoor,
            [JsonStringEnumMemberName("Upstairs Bathroom Door")]
            UpstairsBathroomDoor,
            [JsonStringEnumMemberName("Utility Closet Door")]
            UtilityClosetDoor,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ItemsType0
        {
            [JsonStringEnumMemberName("AC Unit")]
            ACUnit,
            [JsonStringEnumMemberName("Airvent ArtRoom")]
            AirventArtRoom,
            [JsonStringEnumMemberName("Airvent DiningRoom")]
            AirventDiningRoom,
            [JsonStringEnumMemberName("Airvent DownstairsGuestBathroom")]
            AirventDownstairsGuestBathroom,
            [JsonStringEnumMemberName("Airvent GarageWall")]
            AirventGarageWall,
            [JsonStringEnumMemberName("Airvent Kitchen")]
            AirventKitchen,
            [JsonStringEnumMemberName("Airvent LaundryRoom")]
            AirventLaundryRoom,
            [JsonStringEnumMemberName("Airvent LivingRoom")]
            AirventLivingRoom,
            [JsonStringEnumMemberName("Airvent MasterBathroom")]
            AirventMasterBathroom,
            [JsonStringEnumMemberName("Airvent MasterBedroom")]
            AirventMasterBedroom,
            [JsonStringEnumMemberName("Airvent SpareRoom")]
            AirventSpareRoom,
            [JsonStringEnumMemberName("Airvent Study")]
            AirventStudy,
            [JsonStringEnumMemberName("Airvent UpstairsGuestBathroom")]
            AirventUpstairsGuestBathroom,
            [JsonStringEnumMemberName("Apartment Computer")]
            ApartmentComputer,
            [JsonStringEnumMemberName("ArtRoomWindow")]
            ArtRoomWindow,
            [JsonStringEnumMemberName("Ashley's Panties")]
            AshleysPanties,
            [JsonStringEnumMemberName("AshleyTop")]
            AshleyTop,
            [JsonStringEnumMemberName("Auxiliary Dock")]
            AuxiliaryDock,
            [JsonStringEnumMemberName("Battery Pack")]
            BatteryPack,
            [JsonStringEnumMemberName("BeerPongCupTrigger0")]
            BeerPongCupTrigger0,
            [JsonStringEnumMemberName("BikeLock")]
            BikeLock,
            [JsonStringEnumMemberName("BikeLockKey")]
            BikeLockKey,
            [JsonStringEnumMemberName("Billboard")]
            Billboard,
            [JsonStringEnumMemberName("BloodWall")]
            BloodWall,
            [JsonStringEnumMemberName("Blue Hair Dye")]
            BlueHairDye,
            [JsonStringEnumMemberName("Blueprints")]
            Blueprints,
            [JsonStringEnumMemberName("Box of Nails")]
            BoxofNails,
            [JsonStringEnumMemberName("Breaker Panel")]
            BreakerPanel,
            [JsonStringEnumMemberName("Briefcase")]
            Briefcase,
            [JsonStringEnumMemberName("BriefcaseOpen")]
            BriefcaseOpen,
            [JsonStringEnumMemberName("Broom")]
            Broom,
            [JsonStringEnumMemberName("Bug Zapper")]
            BugZapper,
            [JsonStringEnumMemberName("Bushes")]
            Bushes,
            [JsonStringEnumMemberName("Cabernet")]
            Cabernet,
            [JsonStringEnumMemberName("Camera")]
            Camera,
            [JsonStringEnumMemberName("CatPicture")]
            CatPicture,
            [JsonStringEnumMemberName("Cell Phone Jammer")]
            CellPhoneJammer,
            [JsonStringEnumMemberName("Chardonnay")]
            Chardonnay,
            [JsonStringEnumMemberName("Chicken Nuggets")]
            ChickenNuggets,
            [JsonStringEnumMemberName("Chili Peppers")]
            ChiliPeppers,
            [JsonStringEnumMemberName("Choclate Syrup")]
            ChoclateSyrup,
            [JsonStringEnumMemberName("ChocolateBar")]
            ChocolateBar,
            [JsonStringEnumMemberName("ChristmasGazeboStar")]
            ChristmasGazeboStar,
            [JsonStringEnumMemberName("Clear Eyes")]
            ClearEyes,
            [JsonStringEnumMemberName("Closed Briefcase")]
            ClosedBriefcase,
            [JsonStringEnumMemberName("Clothes")]
            Clothes,
            [JsonStringEnumMemberName("Clothes Basket 1")]
            ClothesBasket1,
            [JsonStringEnumMemberName("Clothes Basket 2f")]
            ClothesBasket2f,
            [JsonStringEnumMemberName("Clothes Basket 3")]
            ClothesBasket3,
            [JsonStringEnumMemberName("Coffee")]
            Coffee,
            [JsonStringEnumMemberName("Collar")]
            Collar,
            [JsonStringEnumMemberName("CollarBloody")]
            CollarBloody,
            [JsonStringEnumMemberName("Computer")]
            Computer,
            [JsonStringEnumMemberName("Condom")]
            Condom,
            [JsonStringEnumMemberName("Cowboy Scorpiosmus")]
            CowboyScorpiosmus,
            [JsonStringEnumMemberName("Credit Card")]
            CreditCard,
            [JsonStringEnumMemberName("Crowd")]
            Crowd,
            [JsonStringEnumMemberName("Cucumber")]
            Cucumber,
            [JsonStringEnumMemberName("Dark Long Sleeve Women's Top")]
            DarkLongSleeveWomensTop,
            [JsonStringEnumMemberName("DeadSnake")]
            DeadSnake,
            [JsonStringEnumMemberName("Derek's Shirt")]
            DereksShirt,
            [JsonStringEnumMemberName("Diary")]
            Diary,
            [JsonStringEnumMemberName("DirtMound")]
            DirtMound,
            [JsonStringEnumMemberName("DirtyClue1")]
            DirtyClue1,
            [JsonStringEnumMemberName("DirtyClue2")]
            DirtyClue2,
            [JsonStringEnumMemberName("DirtyClue3")]
            DirtyClue3,
            [JsonStringEnumMemberName("Discarded Magazine")]
            DiscardedMagazine,
            [JsonStringEnumMemberName("DLiciousOutfit")]
            DLiciousOutfit,
            [JsonStringEnumMemberName("DoorBracingSpot")]
            DoorBracingSpot,
            [JsonStringEnumMemberName("DoorBracingSpot2")]
            DoorBracingSpot2,
            [JsonStringEnumMemberName("Double-Sided Dildo")]
            DoubleSidedDildo,
            [JsonStringEnumMemberName("DownstairsBathroomMirror")]
            DownstairsBathroomMirror,
            [JsonStringEnumMemberName("Dresser")]
            Dresser,
            [JsonStringEnumMemberName("Drone")]
            Drone,
            [JsonStringEnumMemberName("Drone Remote")]
            DroneRemote,
            [JsonStringEnumMemberName("Dryer")]
            Dryer,
            [JsonStringEnumMemberName("DryerStuckSpot")]
            DryerStuckSpot,
            [JsonStringEnumMemberName("DryerStuckSpotImmediate")]
            DryerStuckSpotImmediate,
            [JsonStringEnumMemberName("Duct Tape")]
            DuctTape,
            [JsonStringEnumMemberName("DVD2")]
            DVD2,
            [JsonStringEnumMemberName("E-Cig Vape")]
            ECigVape,
            [JsonStringEnumMemberName("Eggs")]
            Eggs,
            [JsonStringEnumMemberName("Empty Cup")]
            EmptyCup,
            [JsonStringEnumMemberName("EtherealClue")]
            EtherealClue,
            [JsonStringEnumMemberName("Face Cream")]
            FaceCream,
            [JsonStringEnumMemberName("Faucet")]
            Faucet,
            [JsonStringEnumMemberName("Fifty Dollars")]
            FiftyDollars,
            [JsonStringEnumMemberName("Film Camera")]
            FilmCamera,
            [JsonStringEnumMemberName("Fire Pit")]
            FirePit,
            [JsonStringEnumMemberName("Fireplace")]
            Fireplace,
            [JsonStringEnumMemberName("Firetrail")]
            Firetrail,
            [JsonStringEnumMemberName("FlamingoLeftA")]
            FlamingoLeftA,
            [JsonStringEnumMemberName("FlamingoRightB")]
            FlamingoRightB,
            [JsonStringEnumMemberName("Flask")]
            Flask,
            [JsonStringEnumMemberName("Floor Towels")]
            FloorTowels,
            [JsonStringEnumMemberName("Flower")]
            Flower,
            [JsonStringEnumMemberName("FreeSnake")]
            FreeSnake,
            [JsonStringEnumMemberName("FreeSnakeFemale")]
            FreeSnakeFemale,
            [JsonStringEnumMemberName("FrontWindowBracingSpot")]
            FrontWindowBracingSpot,
            [JsonStringEnumMemberName("FrontWindowBracingSpot2")]
            FrontWindowBracingSpot2,
            [JsonStringEnumMemberName("FrontWindowBracingSpot3")]
            FrontWindowBracingSpot3,
            [JsonStringEnumMemberName("GarageBinMovable")]
            GarageBinMovable,
            [JsonStringEnumMemberName("GarageWall Airvent Cover")]
            GarageWallAirventCover,
            [JsonStringEnumMemberName("Gastronomy Book")]
            GastronomyBook,
            [JsonStringEnumMemberName("Gazebo")]
            Gazebo,
            [JsonStringEnumMemberName("GazeboFuseBox")]
            GazeboFuseBox,
            [JsonStringEnumMemberName("Glasses")]
            Glasses,
            [JsonStringEnumMemberName("Gut Grip")]
            GutGrip,
            [JsonStringEnumMemberName("Gutter")]
            Gutter,
            [JsonStringEnumMemberName("Hammer")]
            Hammer,
            [JsonStringEnumMemberName("Hammer2")]
            Hammer2,
            [JsonStringEnumMemberName("HangingTentacle")]
            HangingTentacle,
            [JsonStringEnumMemberName("Hedge Clippers")]
            HedgeClippers,
            [JsonStringEnumMemberName("HoleInFence")]
            HoleInFence,
            [JsonStringEnumMemberName("Hot Tub Base")]
            HotTubBase,
            [JsonStringEnumMemberName("HotAirBalloon")]
            HotAirBalloon,
            [JsonStringEnumMemberName("HotTubCoverFolded")]
            HotTubCoverFolded,
            [JsonStringEnumMemberName("HotTubCoverUnfolded")]
            HotTubCoverUnfolded,
            [JsonStringEnumMemberName("Jack Daniel's")]
            JackDaniels,
            [JsonStringEnumMemberName("Joint")]
            Joint,
            [JsonStringEnumMemberName("Katana")]
            Katana,
            [JsonStringEnumMemberName("Kettle")]
            Kettle,
            [JsonStringEnumMemberName("Key")]
            Key,
            [JsonStringEnumMemberName("Key2")]
            Key2,
            [JsonStringEnumMemberName("Key3")]
            Key3,
            [JsonStringEnumMemberName("Keychain")]
            Keychain,
            [JsonStringEnumMemberName("Knife")]
            Knife,
            [JsonStringEnumMemberName("Laptop")]
            Laptop,
            [JsonStringEnumMemberName("LeahEarpiece")]
            LeahEarpiece,
            [JsonStringEnumMemberName("LivingRoomTV")]
            LivingRoomTV,
            [JsonStringEnumMemberName("LizKatzButt")]
            LizKatzButt,
            [JsonStringEnumMemberName("LRTVBackButton")]
            LRTVBackButton,
            [JsonStringEnumMemberName("LRTVPlayAllButton")]
            LRTVPlayAllButton,
            [JsonStringEnumMemberName("LRTVSongFiveButton")]
            LRTVSongFiveButton,
            [JsonStringEnumMemberName("LRTVSongFourButton")]
            LRTVSongFourButton,
            [JsonStringEnumMemberName("LRTVSongOneButton")]
            LRTVSongOneButton,
            [JsonStringEnumMemberName("LRTVSongSixButton")]
            LRTVSongSixButton,
            [JsonStringEnumMemberName("LRTVSongThreeButton")]
            LRTVSongThreeButton,
            [JsonStringEnumMemberName("LRTVSongTwoButton")]
            LRTVSongTwoButton,
            [JsonStringEnumMemberName("Madison's Phone")]
            MadisonsPhone,
            [JsonStringEnumMemberName("Marijuana")]
            Marijuana,
            [JsonStringEnumMemberName("Matchbox")]
            Matchbox,
            [JsonStringEnumMemberName("Mayonnaise")]
            Mayonnaise,
            [JsonStringEnumMemberName("Merlot")]
            Merlot,
            [JsonStringEnumMemberName("Microwave")]
            Microwave,
            [JsonStringEnumMemberName("MistletoeBallsDecoration")]
            MistletoeBallsDecoration,
            [JsonStringEnumMemberName("Money Makers Monthly")]
            MoneyMakersMonthly,
            [JsonStringEnumMemberName("Motor Oil")]
            MotorOil,
            [JsonStringEnumMemberName("MP3 Player")]
            MP3Player,
            [JsonStringEnumMemberName("MP3 Player Cable")]
            MP3PlayerCable,
            [JsonStringEnumMemberName("MP3 Player DC")]
            MP3PlayerDC,
            [JsonStringEnumMemberName("Mug")]
            Mug,
            [JsonStringEnumMemberName("Natty Lite 1")]
            NattyLite1,
            [JsonStringEnumMemberName("Natty Lite 10")]
            NattyLite10,
            [JsonStringEnumMemberName("Natty Lite 11")]
            NattyLite11,
            [JsonStringEnumMemberName("Natty Lite 12")]
            NattyLite12,
            [JsonStringEnumMemberName("Natty Lite 13")]
            NattyLite13,
            [JsonStringEnumMemberName("Natty Lite 14")]
            NattyLite14,
            [JsonStringEnumMemberName("Natty Lite 2")]
            NattyLite2,
            [JsonStringEnumMemberName("Natty Lite 3")]
            NattyLite3,
            [JsonStringEnumMemberName("Natty Lite 4")]
            NattyLite4,
            [JsonStringEnumMemberName("Natty Lite 5")]
            NattyLite5,
            [JsonStringEnumMemberName("Natty Lite 6")]
            NattyLite6,
            [JsonStringEnumMemberName("Natty Lite 7")]
            NattyLite7,
            [JsonStringEnumMemberName("Natty Lite 8")]
            NattyLite8,
            [JsonStringEnumMemberName("Natty Lite 9")]
            NattyLite9,
            [JsonStringEnumMemberName("Natty Lite Open")]
            NattyLiteOpen,
            [JsonStringEnumMemberName("Nerdy Taped Glasses")]
            NerdyTapedGlasses,
            [JsonStringEnumMemberName("Notebook")]
            Notebook,
            [JsonStringEnumMemberName("Notepad")]
            Notepad,
            [JsonStringEnumMemberName("Open Briefcase")]
            OpenBriefcase,
            [JsonStringEnumMemberName("OrangeBin1")]
            OrangeBin1,
            [JsonStringEnumMemberName("OrganizingBriefcaseSpot")]
            OrganizingBriefcaseSpot,
            [JsonStringEnumMemberName("OwlPainting")]
            OwlPainting,
            [JsonStringEnumMemberName("PaddockButton")]
            PaddockButton,
            [JsonStringEnumMemberName("Painkillers")]
            Painkillers,
            [JsonStringEnumMemberName("Paper")]
            Paper,
            [JsonStringEnumMemberName("Paper Bag")]
            PaperBag,
            [JsonStringEnumMemberName("Paper Bag Crunched")]
            PaperBagCrunched,
            [JsonStringEnumMemberName("Pencil")]
            Pencil,
            [JsonStringEnumMemberName("Petition")]
            Petition,
            [JsonStringEnumMemberName("Phone2")]
            Phone2,
            [JsonStringEnumMemberName("Phone3")]
            Phone3,
            [JsonStringEnumMemberName("Phone4")]
            Phone4,
            [JsonStringEnumMemberName("Phone5")]
            Phone5,
            [JsonStringEnumMemberName("Phone6")]
            Phone6,
            [JsonStringEnumMemberName("PingPongBall")]
            PingPongBall,
            [JsonStringEnumMemberName("PinkUnderwear")]
            PinkUnderwear,
            [JsonStringEnumMemberName("PipeFix")]
            PipeFix,
            [JsonStringEnumMemberName("PipeWater")]
            PipeWater,
            [JsonStringEnumMemberName("Pizza Box")]
            PizzaBox,
            [JsonStringEnumMemberName("PizzaBoxFortification")]
            PizzaBoxFortification,
            [JsonStringEnumMemberName("PlankPileInYard")]
            PlankPileInYard,
            [JsonStringEnumMemberName("PlanksFortification")]
            PlanksFortification,
            [JsonStringEnumMemberName("PlanksFortificationExtraNails")]
            PlanksFortificationExtraNails,
            [JsonStringEnumMemberName("PlexiglassHotTub")]
            PlexiglassHotTub,
            [JsonStringEnumMemberName("PlexiglassStacked")]
            PlexiglassStacked,
            [JsonStringEnumMemberName("Popcorn")]
            Popcorn,
            [JsonStringEnumMemberName("Popper")]
            Popper,
            [JsonStringEnumMemberName("PopperBox")]
            PopperBox,
            [JsonStringEnumMemberName("Potatoes")]
            Potatoes,
            [JsonStringEnumMemberName("PrintableClue")]
            PrintableClue,
            [JsonStringEnumMemberName("Printer")]
            Printer,
            [JsonStringEnumMemberName("Red Solo Cup 0")]
            RedSoloCup0,
            [JsonStringEnumMemberName("Red Solo Cup 1")]
            RedSoloCup1,
            [JsonStringEnumMemberName("Red Solo Cup 2")]
            RedSoloCup2,
            [JsonStringEnumMemberName("Red Solo Cup 3")]
            RedSoloCup3,
            [JsonStringEnumMemberName("Red Solo Cup 4")]
            RedSoloCup4,
            [JsonStringEnumMemberName("Red Solo Cup 5")]
            RedSoloCup5,
            [JsonStringEnumMemberName("Red Solo Cup 6")]
            RedSoloCup6,
            [JsonStringEnumMemberName("Red Solo Cup 7")]
            RedSoloCup7,
            [JsonStringEnumMemberName("Red Solo Cup 8")]
            RedSoloCup8,
            [JsonStringEnumMemberName("Red Solo Cup 9")]
            RedSoloCup9,
            [JsonStringEnumMemberName("Remote Control")]
            RemoteControl,
            [JsonStringEnumMemberName("RipppedWhalePoster")]
            RipppedWhalePoster,
            [JsonStringEnumMemberName("Router")]
            Router,
            [JsonStringEnumMemberName("Rum")]
            Rum,
            [JsonStringEnumMemberName("RumOpen")]
            RumOpen,
            [JsonStringEnumMemberName("Salami")]
            Salami,
            [JsonStringEnumMemberName("Salmon")]
            Salmon,
            [JsonStringEnumMemberName("Satchel")]
            Satchel,
            [JsonStringEnumMemberName("Scorpion Tequila")]
            ScorpionTequila,
            [JsonStringEnumMemberName("SD Card")]
            SDCard,
            [JsonStringEnumMemberName("Shake Light")]
            ShakeLight,
            [JsonStringEnumMemberName("SleepingMaskFolded")]
            SleepingMaskFolded,
            [JsonStringEnumMemberName("SleepingMaskMountable")]
            SleepingMaskMountable,
            [JsonStringEnumMemberName("Snowball")]
            Snowball,
            [JsonStringEnumMemberName("SnowballBucket")]
            SnowballBucket,
            [JsonStringEnumMemberName("Soda")]
            Soda,
            [JsonStringEnumMemberName("SoloBeerPongTable")]
            SoloBeerPongTable,
            [JsonStringEnumMemberName("SoloPingPongBallStorage")]
            SoloPingPongBallStorage,
            [JsonStringEnumMemberName("Speaker1")]
            Speaker1,
            [JsonStringEnumMemberName("Speaker2")]
            Speaker2,
            [JsonStringEnumMemberName("Spoon")]
            Spoon,
            [JsonStringEnumMemberName("SprayPaintCan")]
            SprayPaintCan,
            [JsonStringEnumMemberName("Starbomb")]
            Starbomb,
            [JsonStringEnumMemberName("StarbombBox")]
            StarbombBox,
            [JsonStringEnumMemberName("Stepstool")]
            Stepstool,
            [JsonStringEnumMemberName("StepstoolStandSpot")]
            StepstoolStandSpot,
            [JsonStringEnumMemberName("Stove")]
            Stove,
            [JsonStringEnumMemberName("StudyLaptopTypingSpot")]
            StudyLaptopTypingSpot,
            [JsonStringEnumMemberName("Sundress")]
            Sundress,
            [JsonStringEnumMemberName("Sunglasses")]
            Sunglasses,
            [JsonStringEnumMemberName("Sweeties")]
            Sweeties,
            [JsonStringEnumMemberName("Tablet")]
            Tablet,
            [JsonStringEnumMemberName("Talking Fish")]
            TalkingFish,
            [JsonStringEnumMemberName("Tentacle")]
            Tentacle,
            [JsonStringEnumMemberName("Terrarium")]
            Terrarium,
            [JsonStringEnumMemberName("Thermos")]
            Thermos,
            [JsonStringEnumMemberName("Thermostat")]
            Thermostat,
            [JsonStringEnumMemberName("Toaster")]
            Toaster,
            [JsonStringEnumMemberName("Towel")]
            Towel,
            [JsonStringEnumMemberName("Towel2")]
            Towel2,
            [JsonStringEnumMemberName("Towel3")]
            Towel3,
            [JsonStringEnumMemberName("Trash Can")]
            TrashCan,
            [JsonStringEnumMemberName("Trunk")]
            Trunk,
            [JsonStringEnumMemberName("Undressing Neighbor")]
            UndressingNeighbor,
            [JsonStringEnumMemberName("UpstairsMasterBathroomMirror")]
            UpstairsMasterBathroomMirror,
            [JsonStringEnumMemberName("USB Stick")]
            USBStick,
            [JsonStringEnumMemberName("Valentines Chocolates")]
            ValentinesChocolates,
            [JsonStringEnumMemberName("Vibrator")]
            Vibrator,
            [JsonStringEnumMemberName("Vickie's Panties")]
            VickiesPanties,
            [JsonStringEnumMemberName("Vodka")]
            Vodka,
            [JsonStringEnumMemberName("VoiceRecorder")]
            VoiceRecorder,
            [JsonStringEnumMemberName("Washer")]
            Washer,
            [JsonStringEnumMemberName("WaterMain")]
            WaterMain,
            [JsonStringEnumMemberName("WhalePoster")]
            WhalePoster,
            [JsonStringEnumMemberName("Whipped Cream")]
            WhippedCream,
            [JsonStringEnumMemberName("Wine Rack")]
            WineRack,
            [JsonStringEnumMemberName("Wrench")]
            Wrench,
            [JsonStringEnumMemberName("YouTool")]
            YouTool,
        }
    }
}