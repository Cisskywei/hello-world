﻿using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace log
{
    public class log
	{
        public enum enLogMode
        {
            Debug = 0,
            Release = 1,
        }

        static public void trace(StackFrame st, Int64 tmptime, string log, params object[] agrvs)
        {
            if (logMode <= enLogMode.Debug)
            {
                output(st, tmptime, "trace", log, agrvs);
            }
        }

        static public void error(StackFrame st, Int64 tmptime, string log, params object[] agrvs)
        {
            output(st, tmptime, "error", log, agrvs);
        }

        static public void operation(StackFrame st, Int64 tmptime, string log, params object[] agrvs)
        {
            output(st, tmptime, "operation", log, agrvs);
        }

        static void output(StackFrame sf, Int64 tmptime, string level, string log, params object[] agrvs)
        {
            log = string.Format(log, agrvs);

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            System.DateTime time = startTime.AddMilliseconds((double)tmptime);

            string strlog = string.Format("[{0}] [{1}] [{2}] [{3}]:{4}", time, level, sf.GetMethod().DeclaringType.FullName, sf.GetMethod().Name, log);

            lock (logs)
            {
                System.Console.WriteLine(strlog);

                logs.Add(strlog);
            }
        }

        static public void traverse_log(Action<String> func)
        {
            lock(logs)
            {
                foreach(var log in logs)
                {
                    func(log);
                }
            }
        }

        static public enLogMode logMode = enLogMode.Debug;
        static private List<string> logs = new List<string>();
    }
}
