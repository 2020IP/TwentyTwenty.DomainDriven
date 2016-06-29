using System;

namespace TwentyTwenty.DomainDriven
{
    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }
}