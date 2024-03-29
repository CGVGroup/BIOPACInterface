﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unity;
public class ThreadManager : Singleton<ThreadManager>
{
    private static readonly List<Action> executeOnMainThread = new List<Action>();
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
    private static bool actionToExecuteOnMainThread = false;

    private Thread _mainThreadObject;

    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="_action">The action to be executed on the main thread.</param>
    public void ExecuteOnMainThread(Action _action)
    {
        if (_action == null)
        {
            Console.WriteLine("No action to execute on main thread!");
            return;
        }

        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(_action);
            actionToExecuteOnMainThread = true;
        }
    }

    public bool IsMainThread()
    {
        return object.ReferenceEquals(System.Threading.Thread.CurrentThread, _mainThreadObject);
    }

    private void Start()
    {
        _mainThreadObject = Thread.CurrentThread;
    }

    /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
    private void Update()
    {
        if (actionToExecuteOnMainThread)
        {
            executeCopiedOnMainThread.Clear();
            lock (executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread);
                executeOnMainThread.Clear();
                actionToExecuteOnMainThread = false;
            }

            for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
            {
                executeCopiedOnMainThread[i]();
            }
        }
    }



}
