using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Singleton<T> where T : class, new()
{
    private static T _instance;
    private static readonly object syslock = new object();

    public static T getInstance()
    {
        if (_instance == null)
        {
            lock (syslock)
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
            }
        }
        return _instance;
    }
}

/*
     class myclass2 {
        public static myclass2 getInstance() {
            return Singleton<myclass2>.getInstance();
        }
        public void ss()
        {
            Console.WriteLine("111");
        }
    }

    class myclass : Singleton<myclass> {
        public void ss() {
            Console.WriteLine("111");
        }
    }
*/
