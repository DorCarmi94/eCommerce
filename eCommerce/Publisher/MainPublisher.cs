using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace eCommerce.Publisher
{
    // public class MainPublisher : PublisherObservable
    // {
    //     private ConcurrentDictionary<string, ConcurrentQueue<string>> messages;
    //     private ConcurrentDictionary<string, bool> connected;
    //     private ConcurrentBag<UserObserver> observers;
    //     
    //     
    //     public MainPublisher()
    //     {
    //         observers = new ConcurrentBag<UserObserver>();
    //         messages = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
    //         connected = new ConcurrentDictionary<string, bool>();
    //     }
    public sealed class MainPublisher : PublisherObservable
    {
        //Fields:
        private static readonly MainPublisher instance = new MainPublisher();  
        private ConcurrentDictionary<string, ConcurrentQueue<string>> messages;
        private ConcurrentDictionary<string, bool> connected;
        private ConcurrentBag<UserObserver> observers;

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  
        //Constructor:
        static MainPublisher(){}

        private MainPublisher()
        {
            observers = new ConcurrentBag<UserObserver>();
            messages = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            connected = new ConcurrentDictionary<string, bool>();
        }  
        public static MainPublisher Instance => instance;

        public async void Connect(string userID)
        {
            // this.connected[userID] = true;
            Console.WriteLine($"Connect: {userID}");
            connected.TryAdd(userID, true);
            // if (messages.ContainsKey(userID) && messages[userID].Count > 0)
            // {
            NotifyAll(userID);
            // }
        }
        
        public void Disconnect(string userId)
        {
            connected.TryRemove(userId, out var tstring);
        }

        public void AddMessageToUser(string userID, string message)
        {
            Console.WriteLine($"Add message: {userID}");
            if (!messages.ContainsKey(userID))
            {
                messages.TryAdd(userID, new ConcurrentQueue<string>());
            }
            messages[userID].Enqueue(message);
            NotifyAll(userID);
        }


        public void Register(UserObserver userObserver)
        {
            observers.Add(userObserver);
        }
        
        public void NotifyAll(string userID)
        {
            Console.WriteLine(userID);
            if (connected.ContainsKey(userID) && messages.ContainsKey(userID) && messages[userID].Count > 0)
            {
                foreach (var observer in this.observers)
                {
                    observer.Notify(userID, messages[userID]);
                }
            }
        }
    }
}