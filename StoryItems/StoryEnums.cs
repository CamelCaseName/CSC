using Newtonsoft.Json.Converters;//Token:0x\w|d|\s|:*\n

namespace CSC.StoryItems
{
    ////Token:0x\w|d|\s|:*\n
    //searchtermtoremovealltokencomments
    public static class StoryEnums
    {
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum DialogueStatuses
        {
            WasNotShown,
            WasShown = 10,
            IsCurrentlyShowing = 20,
            NotCurrentlyShowing = 30
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum DoorOptionValues
        {
            IsOpen,
            IsClosed,
            IsLocked,
            IsUnlocked
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum EqualsValues
        {
            Equals,
            DoesNotEqual
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ComparisonEquations
        {
            Equals,
            DoesNotEqual = 10,
            GreaterThan = 20,
            LessThan = 30
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ItemComparisonTypes
        {
            IsActive = 10,
            IsMounted = 20,
            IsMountedTo = 22,
            IsHeldByPlayer = 25,
            IsInventoriedOrHeldByPlayer,
            IsVisibleTo = 30
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerInventoryOptions
        {
            HasItem,
            HasAtLeastOneItem
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum PoseOptions
        {
            IsCurrentlyPosing,
            CurrentPose = 10
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum Gender
        {
            None = 2,
            Female = 1,
            Male = 0
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum BoolCritera
        {
            False,
            True
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ClothingChangeOptions
        {
            ClothingType,
            AllOn,
            AllOff,
            ChangeItem,
            ChangeToOutfit,
            RemoveFromOutfit
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ClothingOnOff
        {
            On,
            Off,
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum SetEnum
        {
            Set0,
            Set1,
            Set2,
            Set3,
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ValueSpecificFormulas
        {
            EqualsValue,
            DoesNotEqualValue,
            GreaterThanValue,
            LessThanValue
        }

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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ClothingOptions
        {
            Change,
            ToggleWetEffect,
            ToggleBloodyEffect
        }
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum CombatOptions
        {
            Fight,
            Die = 10,
            PassOut = 20,
            WakeUp = 30,
            Cancel = 40
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum IntimacyOptions
        {
            SexualAct,
            IncreaseActionSpeed = 10,
            DecreaseActionSpeed
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum GameMessageType
        {
            CenterScreenText,
            Narration = 10,
            ThoughtBubble = 20
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ResponseReactionTypes
        {
            VeryBad,
            Bad,
            Neutral,
            Good,
            VeryGood
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum Importance
        {
            None,
            Important,
            VeryImportant
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum QuestStatus
        {
            NotObtained,
            InProgress,
            Complete,
            Failed,
            Missed
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum PassCondition
        {
            AllSetsAreTrue,
            AnySetIsTrue,
            AllSetsAreFalse,
            AnySetIsFalse
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum DoorAction
        {
            Open,
            Close,
            Lock,
            Unlock,
            OpenSlowly,
            CloseSlowly
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum IKTargets
        {
            LeftHand,
            RightHand,
            LeftFoot,
            RightFoot,
            Head,
            Hips
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum DialogueAction
        {
            Trigger,
            Overhear,
            SetStartDialogue = 5,
            TriggerStartDialogue = 10
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum CutsceneAction
        {
            PlayScene,
            PlayRandomSceneFromLocation,
            PlayRandomSceneFromCurrentLocation,
            EndScene,
            EndAnySceneWithPlayer
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum Modification
        {
            Equals,
            Add
        }
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum TurnOptions
        {
            Around,
            Left,
            Right,
            Toward
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum WarpToOption
        {
            MoveTarget,
            Character,
            Item
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ImportanceSpecified
        {
            Unspecified,
            None,
            Important,
            VeryImportant
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum QuestActions
        {
            Start,
            Increment,
            Complete,
            Fail
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum RadialTriggerOptions
        {
            Item,
            Character
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum EventSpecialHandling
        {
            None,
            DialogueResponse,
            CloseDialogue,
            StartDialogue,
            ItemGroup,
            CutSceneEvents
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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
            Player,
            Rachael,
            Stephanie,
            Vickie,
            Amala,
            DojaCat,
            LizKatz
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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
            Rachael,
            Stephanie,
            Vickie,
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum NoneCharacters
        {
            Nonde,
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ClothingSet
        {
            AnySet,
            Set0,
            Set1
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum CutscenePlaying
        {
            AnyCutScenePlaying = 1,
            AnySexCutscenePlaying,
            CensoredSexScenePlaying
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum Doors
        {
            BathroomDoor,
            BedroomClosetDoorLeft,
            BedroomClosetDoorRight,
            CabinetLeft,
            CabinetRight,
            DeskDrawerLeft,
            DeskDrawerRight,
            EscapeHatch,
            Fridge,
            FrontDoor,
            GarageDoor,
            KitchenCabinet5,
            KitchenCabinet6,
            LaundryRoomDoor,
            MasterBathroomDoor,
            MasterBedroomDoor,
            Nightstand1,
            Nightstand2,
            OfficeDrawerLeft,
            OfficeDrawerRight,
            PantryDoorLeft,
            PantryDoorRight,
            Safe, SliderDoor, SmallSpareClosetDoorL,
            SmallSpareClosetDoorR,
            SpareRoom2,
            DoorSpareRoomDoor,
            SRClosetDoorl,
            SRClosetDoor2,
            SRClosetDoor3,
            SRClosetDoor4,
            StudyClosetDoorL,
            StudyClosetDoorR,
            StudyDoor,
            TerrariumSlider,
            UpstairsBathroomDoor,
            UtilityClosetDoor
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum LookAtTargets
        {
            Character,
            Camera,
            InteractiveItem = 10,
            GameObject = 20,
            PlayerPenis = 30
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerPrefs
        {
            ShowTutorial
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum LocationTargetOption
        {
            MoveTarget,
            Character,
            Item
        }

        //fromhpcontent.asset,replacefollowingquieryby","
        //\s*\n\s+Roamable:\d\s*\n\s+AcceptableSexLocation:\d\s*\n\s+AcceptableWallSexLocation:\d\s*\n\s+AcceptableNavRecoveryTarget:\d\s*\n\s+NonWallSexUsesMyTransform:\d
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        //(\s*\n\s+)EnableItemFunctions: \d(\s*\n\s+)ItemFunctions:\s*\n\s+[- \w*\s\n]*: \d[\s*\n\s+\w+: \d]+- Name: 
        //replace with ,
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum Items
        {
            ACUnit,
            AirventArtRoom,
            AirventDiningRoom,
            AirventDownstairsGuestBathroom,
            AirventGarageWall,
            AirventKitchen,
            AirventLaundryRoom,
            AirventLivingRoom,
            AirventMasterBathroom,
            AirventMasterBedroom,
            AirventSpareRoom,
            AirventStudy,
            AirventUpstairsGuestBathroom,
            ApartmentComputer,
            ApartmentSofaSeatLeft,
            ApartmentSofaSeatRight,
            Armchair,
            ArtRoomWindow,
            AshleysPanties,
            AshleyTop,
            AuxiliaryDock,
            BathroomDoor,
            BatteryPack,
            BedLeft,
            BedRight,
            BedEdge,
            BedroomClosetDoorLeft,
            BedroomClosetDoorRight,
            BeerPongCupTrigger0,
            BikeLock,
            BikeLockKey,
            Billboard,
            BloodWall,
            BlueHairDye,
            Blueprints,
            BoxofNails,
            BreakerPanel,
            Briefcase,
            BriefcaseOpen,
            Broom,
            BugZapper,
            Bushes,
            Cabernet,
            CabinetLeft,
            CabinetRight,
            Camera,
            CatPicture,
            CellPhoneJammer,
            Chardonnay,
            Chicken,
            ChickenNuggets,
            ChiliPeppers,
            ChoclateSyrup,
            ChocolateBar,
            ChristmasGazeboStar,
            ClearEyes,
            ClosedBriefcase,
            Clothes,
            ClothesBasket1,
            ClothesBasket2f,
            ClothesBasket3,
            Coffee,
            Collar,
            CollarBloody,
            CompuBrahBedLeft,
            CompuBrahBedRight,
            CompuBrahSex,
            Computer,
            ComputerChair,
            Condom,
            CouchLeft,
            CouchMiddle,
            CouchRight,
            CowboyScorpiosmus,
            CreditCard,
            Crowd,
            Cucumber,
            DarkLongSleeveWomensTop,
            DeadSnake,
            DereksShirt,
            DeskDrawerLeft,
            DeskDrawerRight,
            Diary,
            DirtMound,
            DirtyClue1,
            DirtyClue2,
            DirtyClue3,
            DiscardedMagazine,
            DisembodiedHead,
            DLiciousOutfit,
            DoorBracingSpot,
            DoorBracingSpot2,
            DoubleSidedDildo,
            DownstairsBathroomMirror,
            DownstairsToilet,
            Dresser,
            Drone,
            DroneRemote,
            Dryer,
            DryerDoor,
            DryerSeat,
            DryerStuckSpot,
            DryerStuckSpotImmediate,
            DuctTape,
            DVD2,
            ECigVape,
            Eggs,
            EmptyCup,
            EscapeHatch,
            EtherealClue,
            FaceCream,
            Faucet,
            FiftyDollars,
            FilmCamera,
            FirePit,
            Fireplace,
            Firetrail,
            FlamingoLeftA,
            FlamingoRightB,
            FlamingoSeatLeft,
            FlamingoSeatRight,
            Flask,
            FloorTowels,
            Flower,
            FoamFinger,
            FranksChair,
            FreeSnake,
            FreeSnakeFemale,
            Freezer,
            Fridge,
            FrontDoor,
            FrontWindowBracingSpot,
            FrontWindowBracingSpot2,
            FrontWindowBracingSpot3,
            GarageDoor,
            GarageBinMovable,
            GarageWallAirventCover,
            GastronomyBook,
            Gazebo,
            GazeboFuseBox,
            Glasses,
            GutGrip,
            Gutter,
            Hammer,
            Hammer2,
            HangingTentacle,
            HedgeClippers,
            HoleInFence,
            HotTubBase,
            HotAirBalloon,
            HotTubSeat1,
            HotTubSeat2,
            HotTubSeat3,
            HotTubSeat4,
            HotTubSeat5,
            HotTubCoverFolded,
            HotTubCoverUnfolded,
            JackDaniels,
            Joint,
            Katana,
            Kettle,
            Key,
            Key2,
            Key3,
            Keychain,
            KitchenCabinet5,
            KitchenCabinet6,
            KitchenCabinet7,
            KitchenCabinet8,
            Knife,
            Laptop,
            LaundryRoomDoor,
            LawnChair1,
            LawnChair2,
            LawnChair3,
            LawnChair4,
            LeahEarpiece,
            LivingRoomTV,
            LizKatzButt,
            LRTVBackButton,
            LRTVPlayAllButton,
            LRTVSongFiveButton,
            LRTVSongFourButton,
            LRTVSongOneButton,
            LRTVSongSixButton,
            LRTVSongThreeButton,
            LRTVSongTwoButton,
            MadisonsPhone,
            Marijuana,
            MasterBathroomDoor,
            MasterBedroomDoor,
            MasterBathTubEdgeSeat,
            MasterBathTubSeat,
            Matchbox,
            Mayonnaise,
            Merlot,
            Microwave,
            MicrowaveDoor,
            MistletoeBallsDecoration,
            MoneyMakersMonthly,
            MotorOil,
            MP3Player,
            MP3PlayerCable,
            MP3PlayerDC,
            Mug,
            NattyLite1,
            NattyLite10,
            NattyLite11,
            NattyLite12,
            NattyLite13,
            NattyLite14,
            NattyLite2,
            NattyLite3,
            NattyLite4,
            NattyLite5,
            NattyLite6,
            NattyLite7,
            NattyLite8,
            NattyLite9,
            NattyLiteOpen,
            NerdyTapedGlasses,
            Nightstand1,
            Nightstand2,
            Notebook,
            Notepad,
            OfficeChair,
            OfficeDrawerLeft,
            OfficeDrawerRight,
            OpenBriefcase,
            OrangeBin1,
            OrganizingBriefcaseSpot,
            OutsideSofa1Left,
            OutsideSofa1Middle,
            OutsideSofa1Right,
            OutsideSofa2Left,
            OutsideSofa2Middle,
            OutsideSofa2Right,
            OwlPainting,
            PaddockButton,
            Painkillers,
            PantryDoorLeft,
            PantryDoorRight,
            Paper,
            PaperBag,
            PaperBagCrunched,
            PatioArmchair,
            PatioArmchair2,
            Pencil,
            Penguin,
            Petition,
            Phone2,
            Phone3,
            Phone4,
            Phone5,
            Phone6,
            PingPongBall,
            PinkUnderwear,
            PipeFix,
            PipeWater,
            PizzaBox,
            PizzaBoxFortification,
            PlankPileInYard,
            PlanksFortification,
            PlanksFortificationExtraNails,
            PlayerBeerPongPractice,
            PlexiglassHotTub,
            PlexiglassStacked,
            Popcorn,
            Popper,
            PopperBox,
            Potatoes,
            Pouf,
            PrintableClue,
            Printer,
            RedSoloCup0,
            RedSoloCup1,
            RedSoloCup2,
            RedSoloCup3,
            RedSoloCup4,
            RedSoloCup5,
            RedSoloCup6,
            RedSoloCup7,
            RedSoloCup8,
            RedSoloCup9,
            RemoteControl,
            RipppedWhalePoster,
            RoofSeatLeft,
            RoofSeatRight,
            Router,
            Rum,
            RumOpen,
            Safe,
            Salami,
            Salmon,
            Satchel,
            ScorpionTequila,
            SDCard,
            ShakeLight,
            SleepingMaskFolded,
            SleepingMaskMountable,
            SliderDoor,
            SmallSpareClosetDoorL,
            SmallSpareClosetDoorR,
            Snowball,
            SnowballBucket,
            Soda,
            SofaLeft,
            SofaRight,
            SoloBeerPongTable,
            SoloPingPongBallStorage,
            SpareRoom2Door,
            SpareRoomDoor,
            SpareRoomSofaLeft,
            SpareRoomSofaMiddle,
            SpareRoomSofaRight,
            Speaker1,
            Speaker2,
            Spoon,
            SprayPaintCan,
            SRClosetDoor1,
            SRClosetDoor2,
            SRClosetDoor3,
            SRClosetDoor4,
            Starbomb,
            StarbombBox,
            Stepstool,
            StepstoolStandSpot,
            Stove,
            StudyClosetDoorL,
            StudyClosetDoorR,
            StudyDoor,
            StudyLaptopTypingSpot,
            Sundress,
            Sunglasses,
            Sweeties,
            Tablet,
            TalkingFish,
            Tentacle,
            Terrarium,
            Thermos,
            Thermostat,
            Toaster,
            Towel,
            Towel2,
            Towel3,
            TrashCan,
            Trunk,
            UndressingNeighbor,
            UpstairsBathroomDoor,
            UpstairsBathroomMirror,
            UpstairsBathroomToilet,
            UpstairsBathTubEdgeSeat,
            UpstairsBathTubSeat,
            UpstairsMasterBathroomMirror,
            USBStick,
            UtilityClosetDoor,
            ValentinesChocolates,
            VanityChair,
            Vibrator,
            VickiesPanties,
            Vodka,
            VoiceRecorder,
            Washer,
            WasherSeat,
            WaterMain,
            WhalePoster,
            WhippedCream,
            WineRack,
            Wrench,
            YouTool,
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum Cutscenes
        {
            AshleyFinale_c,
            DerekFinale_c,
            FrankBlowjob_c,
            FrankFinale_c,
            F_AmyBrittney_AshleyRoom_Voyeur_c,
            F_AshleyFinale_c,
            F_BrittneyFinale_c,
            F_DerekFinale_c,
            F_FrankFinale_c,
            F_KatherineFinale_c,
            F_MP3Reach,
            F_PatrickFinale_c,
            F_PlayerGarageSex_c,
            F_PlayerMasterBedroomSex1_c,
            F_PlayerMasterBedroomSex_Straight_c,
            F_RachaelFinale_c,
            F_ThreesomeFFF_AshleyRoom_c,
            F_ThreesomeFFM_MasterBedroom_c,
            F_VickieFinale_c,
            Intro_F,
            Intro_M,
            KatherineFinale_c,
            MP3Reach,
            Patjump,
            PlayerGarageSex_c,
            PlayerMasterBedroomSex1_c,
            RachaelFinale_c,
            ThreesomeFFM_AshleyRoom_c,
            ThreesomeFFM_EasterEgg_c,
            ThreesomeFFM_Gazebo_c,
            ThreesomeFFM_MasterBedroom_c,
            ThreesomeMMF_1_Gazebo_c,
            ThreesomeMMF_ArtRoom_c,
            VickieFinale_c,
            DC_FenceEvent,
            DC_FenceEvent_Trailer,
            DojaFinale,
            DojaPerformanceCompleteFail,
            DojaPerformanceDroneFail,
            DojaPerformanceGood,
            DojaPerformanceHotTubFail,
            DojaPerformanceLive,
            DojaPerformancePatFail,
            F_DojaFinale,
            AmyPlayerBedroomSex,
            AshleyFinale,
            AshleySupporter,
            DerekFinale,
            DerekSupporter,
            FrankBlowjob,
            FrankFinale,
            F_AmyBrittney_AshleyRoom_Voyeur,
            F_AshleyFinale,
            F_AshleySupporter,
            F_BrittneyFinale,
            F_DerekFinale,
            F_DerekSupporter,
            F_FrankFinale,
            F_KatherineFinale,
            F_LizFinale,
            F_LizVR,
            F_LizWashroom,
            F_PatrickFinale,
            F_PlayerGarageSex,
            F_PlayerMasterBedroomSex1,
            F_PlayerMasterBedroomSex_Straight1,
            F_PlayerMasterBedroomSex_Straight,
            F_RachaelFinale,
            F_TentacleMastSeat1,
            F_TentacleMastSeat2,
            F_TentacleMastSeat3,
            F_TentacleMastSeat4,
            F_ThreesomeFFF_AshleyRoom,
            F_ThreesomeFFM_MasterBedroom,
            F_VickieFinale,
            GenericSpareRoom,
            KatherineFinale,
            LizChanging,
            LizFinale,
            LizTentacle,
            LizVR,
            LizWashroom,
            PlayerGarageSex,
            PlayerMasterBedroomSex1,
            RachaelFinale,
            TentacleMastSeat1,
            TentacleMastSeat2,
            TentacleMastSeat3,
            TentacleMastSeat4,
            ThreesomeFFM_AshleyRoom,
            ThreesomeFFM_EasterEgg,
            ThreesomeFFM_Gazebo,
            ThreesomeFFM_MasterBedroom,
            ThreesomeMMF_1_Gazebo,
            ThreesomeMMF_ArtRoom,
            VickieFinale,
            ACExplosion_A,
            ACExplosion_M,
            ACExplosion_MA,
            FridgeReveal,
            F_FridgeReveal,
            F_LizFinale_c,
            F_LizVR_c,
            F_LizWashroom_c,
            LizChanging_c,
            LizEntry,
            LizFinale_c,
            LizTentacle_c,
            LizVR_c,
            LizWashroom_c,
            TentacleReveal,
            VentRelease,
            AshleySupporter_c,
            DerekSupporter_c,
            F_AshleySupporter_c,
            F_DerekSupporter_c
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum TriggerOptions
        {
            PerformEvent,
            SetEnabled
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum WalkToTargetOptions
        {
            MoveTarget,
            Character,
            Item,
            Cancel
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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
            _033737654signature4,
            LKdoorbell_final
        }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
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
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum ClothingTypeOrName
        {

            ByName,

            ByType
        }
    }
}