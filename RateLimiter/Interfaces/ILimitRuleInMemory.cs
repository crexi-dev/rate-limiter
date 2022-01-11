using System.Collections.Generic;

public interface ILimitRuleInMemory<T> : ILimitRule
{
    List<T> Cache { get; set; }
}
