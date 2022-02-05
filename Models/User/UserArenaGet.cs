﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Models.User
{
    public class UserArenaGet
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevel { get; set; }
        public int NumberOfGoodDeeds { get; set; }
        public int NumberOfJokes { get; set; }
        public int NumberOfQuotes { get; set; }
        public int NumberOfPuzzles { get; set; }
        public int NumberOfHappenings { get; set; }
        public int NumberOfChallenges { get; set; }
    }
}
