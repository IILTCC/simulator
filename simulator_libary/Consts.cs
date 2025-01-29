namespace simulator_libary
{
    public class Consts
    {
        public const int PACKET_HEADER_SIZE = 25;
        public const string TIMESTAMP_FORMAT = "dd,MM,yyyy,HH,mm,ss,ffff";
        public const string ICD_REPO_PATH = "./IcdRepo/";
        public const string ICD_FILE_TYPE = ".json";
        public const int TYPE_SIZE = 1;
        public const int IP_SIZE = 4;
        public const int ZERO_ERROR_DELAY = 0;
        public const int BYTE_SIZE = 8;
        public const float PACKET_DELAY_RANDOMNESS = 0.2f;
        public const int LAST_ROW_DIVIDER = 9;
        public const int SIMULATOR_RAND_WINDOW = 15;
        public const int SIMULATOR_OSCILATION_WINDOW_MAX = 400;
        public const int SIMULATOR_OSCILATION_WINDOW_MIN = 100;
        public const float SIMULATOR_JUMP_UP = 0.6f;
        public const float SIMULATOR_JUMP_DOWN = 0.4f;
    }
}
