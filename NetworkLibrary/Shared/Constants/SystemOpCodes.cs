namespace NetworkLibrary.Shared.Constants
{
    public enum SystemOpCodes : byte
    {
        PING = 106,
        PONG,
        ADVERTISE_SERVER,
        ADVERTISE_SERVER_RESPONSE,
        ADVERTISE_SERVER_LIST,
        ADVERTISE_SERVER_LIST_RESPONSE,
        ACTION,
        ACTION_RESPONSE,
    }
}
