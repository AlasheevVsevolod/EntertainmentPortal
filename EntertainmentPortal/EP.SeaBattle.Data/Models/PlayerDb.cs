﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EP.SeaBattle.Data.Models
{
    public class PlayerDb
    {

        //TODO Change to Guid
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string NickName { get; set; }

    }
}
