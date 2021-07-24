using System;
using System.Threading.Tasks;

namespace Tests
{
    public class TaskTestUtils
    {
        public static Task<T[]> CreateAndRunTasks<T>(Func<T> func, int numberOfTasks)
        {
            Task<T>[] tasks = CreateArrayOfTasks(func, numberOfTasks);
            RunTasks(tasks);
            return Task.WhenAll<T>(tasks);
        }
        
        public static Task[] CreateAndRunTasksVoid(Action action, int numberOfTasks)
        {
            Task[] tasks = CreateArrayOfTasksVoid(action, numberOfTasks);
            RunTasksVoid(tasks);
            return tasks;
        }

        public static Task[] CreateArrayOfTasksVoid(Action action, int numberOfTasks)
        {
            Task[] tasks = new Task[numberOfTasks];
            for (var i = 0; i < numberOfTasks; i++)
            {
                tasks[i] = new Task(action);
            }

            return tasks;
        }
        
        public static Task<T>[] CreateArrayOfTasks<T>(Func<T> func, int numberOfTasks)
        {
            Task<T>[] tasks = new Task<T>[numberOfTasks];
            for (var i = 0; i < numberOfTasks; i++)
            {
                tasks[i] = new Task<T>(func);
            }

            return tasks;
        }

        public static Task<T[]> RunAndWaitAll<T>(Task<T>[] tasks)
        {
            RunTasks(tasks);
            return Task.WhenAll<T>(tasks);
        }
        
        public static void RunTasksVoid(Task[] tasks)
        {
            foreach (var task in tasks)
            {
                task.Start();
            }
        }

        public static void RunTasks<T>(Task<T>[] tasks)
        {
            foreach (var task in tasks)
            {
                task.Start();
            }
        }
    }
}