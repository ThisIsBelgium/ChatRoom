﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IClient
    {
       
  
        void Notify(IClient client);
    }
}
