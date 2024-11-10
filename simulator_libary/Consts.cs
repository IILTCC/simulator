﻿namespace simulator_libary
{
    public class Consts
    {
        public const int PACKET_HEADER_SIZE = 25;
        public const string TIMESTAMP_FORMAT = "dd,MM,yyyy,HH,mm,ss,ffff";
        public const string ICD_REPO_PATH = "./icd_repo/";
        public const string ICD_FILE_TYPE = ".json";
        public const int TYPE_SIZE = 1;
        public const int IP_SIZE = 4;
        public const int ZERO_ERROR_DELAY = 0;
        public const int BYTE_SIZE = 8;
        public const float PACKET_DELAY_RANDOMNESS = 0.2f;
        public const int LAST_ROW_DIVIDER = 9;
    }
}