﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetHelper_IO_Tests
{
public class BaseTest
{
    public string WorkingDirectory { get;  }

    public BaseTest()
    {
          WorkingDirectory = $"{Environment.CurrentDirectory}";
    }

}

}
