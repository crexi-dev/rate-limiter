public interface IRule<T> where T : Info
{
    bool Validate(T info);
}

public abstract class Rule<T> : IRule<T> where T : Info
{        
    public abstract bool Validate(T info);
}

public class Info
{

}