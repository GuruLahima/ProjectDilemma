namespace Workbench.ProjectDilemma
{
  public static class Keys
  {
    // exposed room properties
    public const string MAP_PROP_KEY = "map"; // 0 is Lobby, 1 is Gameplay level
    // custom properties
    public const string SKIP_INTRO = "Skip_intro";
    public const string PLAYER_POINTS = "PLAYER_POINTS";
    public const string PLAYER_COLOR = "NameplateColor_index";
    public const string PLAYER_NUMBER = "Player_Number";
    public const string IS_WOLF = "IsWerewolf";
    public const string TWO_WOLVES = "Two_Wolves";
    public const string WOLF_DIED = "WerewolfDied";
    public const string PLAYER_DIED = "PlayerDied";
    public const string IS_DEAD = "IsDead";
    public const string DAYPHASE_START = "DayPhase_StartTime";
    public const string NIGHTPHASE_START = "NightPhase_StartTime";
    public const string BLMOONPHASE_START = "BloodMoonPhase_StartTime";
    public const string GAME_OVER = "GameOver";
    public const string VILLAGE_CROWN_FEATURE = "VillageCrown_feature";
    public const string VILLAGE_CROWN_FEATURE_START = "VillageCrown_feature_startTime";
    public const string VILLAGE_CROWN_FEATURE_END = "VillageCrown_feature_endTime";
    public const string HAS_IMMUNITY = "HasImmunity";

    // player prefs
    public const string CARD_DECK_CHOSEN = "card_deck_chosen";
    public const string PLAYER_XP = "Player_XP";
    public const string PLAYER_RANK = "Player_Rank";
    public const string PLAYER_POINTS_PREF = "playerPoints";
    public const string SHOULD_OVERRIDE_ROLE = "overrideRole";
    public const string OVERRIDE_ROLE = "roleToStartAs";
    public const string MASTER_ISWOLF = "roomCreatorIsWolf";
    public const string MASTER_ISNOTWOLF = "roomCreatorIsNotWolf";
    public const string DAYPHASE_TIME = "dayPhase";
    public const string NIGHTPHASE_TIME = "nightPhase";
    public const string BLMOONPHASE_TIME = "bloodMoonPhase";
    public const string MEETING_DURATION = "meetingDuration";
    public const string MEETS_PER_PLAYER = "meetingsPerPlayer";
    public const string MEETS_PER_SPOT = "meetingsPerSpot";
    public const string MEET_COOLDOWN = "meetingsCooldown";
    public const string DISC_DURATION = "discussionDuration";
    public const string MIN_PLAYERS = "minPlayersPerRoom";
    public const string MAX_PLAYERS = "maxPlayersPerRoom";
    public const string NO_MIN_PLAYERS = "disregardMinPlayerCountLimit";
    public const string PLAYER_NAME = "PlayerName";


    // input bindings
    public const string PUSH_TO_TALK = "Push to talk";
  }
}