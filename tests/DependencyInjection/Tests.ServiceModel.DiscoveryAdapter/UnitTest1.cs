using Moq;
using NUnit.Framework;
using System;

namespace Tests
{
    public class Tests
    {
        [Test, CustomAutoData]
        public void Test1(Action<string> action, string message)
        {
            action(message);

            Mock.Get(action).Verify(p => p(message));
        }
    }
}