﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DotNetHelper_IO.Helpers
//{
//    internal class SemaphoreLocker
//    {
//        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

//        public async Task LockAsync(Func<Task> worker)
//        {
//            await _semaphore.WaitAsync();
//            try
//            {
//                await worker();
//            }
//            finally
//            {
//                _semaphore.Release();
//            }
//        }
//    }
//}